#!/usr/bin/env python3
"""
 SPDX-FileCopyrightText: (c) 2024 Robert Di Pardo
 SPDX-License-Identifier: 0BSD
"""
import os
import re
import sys
import tempfile
from html import escape as html_escape
from html.parser import HTMLParser
from http import client
from textwrap import TextWrapper
from urllib import request, error as urllib_error

import deprecated

class ScintillaDefinitions(HTMLParser):
    """
    Maps message names to their descriptions in Scintilla's HTML documentation.
    """
    def __init__(self):
        super().__init__()
        self.text_wrapper = TextWrapper(width=120)
        self.defs = {}
        self.current_symbol = ''
        self.skip_content = False
        self.feed(fetch_docs())

    def handle_starttag(self, tag: str, attrs: list[tuple[str, str]]):
        """
        Called once for the start tag of each element.
        """
        _tag = tag.lower()

        self.skip_content = \
            bool(re.match(r'h[1-6]', _tag)) or \
            _tag in [ 'pre', 'table', 'thead', 'tbody', 'tfoot', 'th', 'td', 'caption' ] or \
            bool({ v for a, v in attrs if a.lower() == 'class' and v.startswith('S')})

        if not self.skip_content and _tag in [ 'a', 'b', 'br', 'code' ]:
            for name, value in attrs:
                if name.lower() == 'id' and re.match(r'^SC[IN]?_', value):
                    self.current_symbol = value.upper()
                    self.defs[self.current_symbol] = ''

            if _tag == 'br' and bool(self.current_symbol):
                self.defs[self.current_symbol] += '<br/>'

            # revisit parent element
            self.handle_starttag('p', [])

    def handle_data(self, data: str):
        """
        Called once for the text context of each element.
        """
        # description should be more than just the symbol's name
        doc = re.sub(fr'^({self.current_symbol})$', '', data).strip()
        if not self.skip_content and bool(self.current_symbol) and bool(doc):
            self.defs[self.current_symbol] += f"{doc}\r\n"

    def handle_endtag(self, tag: str):
        """
        Called once for the end tag of each element.
        """
        # avoid picking up adjacent paragraphs -- they tend to be lengthy
        if tag.lower() == 'p':
            self.current_symbol = ''

    def describe(self, sym: str) -> str:
        """
        Return a formatted XML comment for the given symbol, if found.
        """
        indent =  ' ' * 8
        prefix = fr"{indent}///"
        doc_lines = []

        def xmlify(s: str) -> str:
            return re.sub(r'\s{2,}', ' ', html_escape(s, quote=False).replace('\u2192', '-&gt;'))

        if sym in self.defs:
            doc = str(self.defs.get(sym))
            summary = doc.split('<br/>')

            is_obsolete = \
                sym in deprecated.MESSAGES or \
                bool(re.search(r'(discouraged)', doc, re.IGNORECASE))

            if summary and summary[0].strip():
                doc_lines.append(fr"{prefix} <summary>")
                signature = xmlify(summary[0].strip())

                if signature:
                    doc_lines.append(f"{prefix} {signature}")

                    is_obsolete = \
                        bool(re.search(r'(deprecated)', signature, re.IGNORECASE)) or is_obsolete

                if len(summary) > 1:
                    doc_lines[-1] += '<br/>'
                    for ln in self.text_wrapper.wrap('\n'.join(summary[1:])):
                        doc_line = re.sub(r'\s+((?!-&gt;)\W )', r'\1', xmlify(ln)).strip()
                        doc_lines.append(f"{prefix} {doc_line}")

                doc_lines.append(fr"{prefix} </summary>")

            if is_obsolete:
                doc_lines.append(
                    indent +\
                    deprecated.MESSAGES.get(
                        sym,
                        f'[Obsolete("https://www.scintilla.org/ScintillaDoc.html#{sym}")]'))

        return '\n'.join(doc_lines)

# -------------------------------------------------------------------
def fetch_docs() -> str:
    """
    Get the HTML content of Scintilla's documentation.
    """
    sci_doc = os.path.join(tempfile.gettempdir(), 'ScintillaDoc.html')
    html = ''
    try:
        if not os.path.exists(sci_doc):
            html = get_resource('https://www.scintilla.org/ScintillaDoc.html')
            with open(sci_doc, 'w', encoding='utf-8') as doc:
                doc.write(html)
        else:
            with open(sci_doc, 'r', encoding='utf-8') as doc:
                html = doc.read()
    except IOError as err:
        print(repr(err), file=sys.stderr)

    return html

def get_resource(resource: str) -> str:
    """
    Request a Web resource and return its content.
    """
    response = ''
    try:
        with request.urlopen(request.Request(resource)) as response:
            if response.status == 200:
                try:
                    response = response.read().decode('utf8')
                except (client.IncompleteRead, IndexError, TypeError):
                    print(f"Error parsing {resource}")
            else:
                print(f"Got response [{response.status_code}] requesting {resource}")

    except (urllib_error.HTTPError, urllib_error.URLError) as err:
        print(repr(err), file=sys.stderr)

    return response

# -------------------------------------------------------------------
if __name__ == '__main__':
    if len(sys.argv) > 1:
        MSG = sys.argv[1].upper()
        DESCR = ScintillaDefinitions().describe(MSG)
        if bool(DESCR):
            print(DESCR)
        else:
            print(f'No doc for "{MSG}" could be found!', file=sys.stderr)
