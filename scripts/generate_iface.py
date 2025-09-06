#!/usr/bin/env python3
# pylint: disable=C0103
# Adapted from 'cs.py', part of NotepadPlusPlusPluginPack.Net
"""
SPDX-FileCopyrightText:  2016 Justin Dailey <https://github.com/dail8859>
                         2016 Kasper B. Graversen <https://github.com/kbilsted>
                         2020 Magesh K. <https://github.com/mahee96>
                         2022 Philipp Schmidt <https://github.com/Fruchtzwerg94>
                         2025 Robert Di Pardo <https://github.com/rdipardo>

SPDX-License-Identifier: Apache-2.0
"""
import os
import subprocess
import sys
import tempfile

import deprecated
import specs
from scintilla import Face, FileGenerator
from sources import SCINTILLA_IFACE
from get_sci_doc import CommentLineStyle, get_resource, xmlify

_TAB = ' ' * 4

def isTypeUnsupported(t):
    """
    The non-64-bit safe-character range APIs have been removed from the Notepad++ code base:
    https://github.com/notepad-plus-plus/notepad-plus-plus/commit/34677a5
    """
    return t in ['textrange', 'findtext', 'formatrange', 'formatrangefull']

def translateType(t):
    """
    Map API types to their corresponding CLR/user-defined types.
    The 64-bit-safe `Position` type is used for position, line and length values.
    """
    return ({
        'cells': 'Cells',
        'colour': 'Colour',
        'colouralpha': 'ColourAlpha',
        'line': 'Position',
        'length': 'Position',
        'pointer': 'IntPtr',
        'position': 'Position',
        'textrangefull': 'TextRangeFull',
        'findtextfull': 'TextToFindFull',
        'keymod': 'KeyModifier',
    }).get(t, t)

def appendComment(style: CommentLineStyle, out, v):
    """Format API descriptions in .NET comment doc style."""
    if 'Comment' in v:
        if len (v['Comment']) == 1 and len (v['Comment'][0]) < 120:
            out.append(style.format(f'<summary>{xmlify(v["Comment"][0])} (Scintilla feature {v["Value"]})</summary>'))
        else:
            out.append(style.format('<summary>'))
            out.extend([style.format(xmlify(line)) for line in v['Comment']])
            out.append(style.format(f'(Scintilla feature {v["Value"]})'))
            out.append(style.format('</summary>'))

def getExceptionDoc(style: CommentLineStyle, reason, exception='ArgumentException'):
    """Get an exception doc string."""
    return style.format(f'<exception cref="{exception}">{reason}</exception>')

def checkIfDeprecated(f: Face, style: CommentLineStyle, name, out):
    """Annotate an API as deprecated."""
    msg = f'SCI_{name.upper()}'
    for api in [msg, name]:
        if f.features.get(api, {}).get('Category', '') == 'Deprecated' or api in deprecated.MESSAGES:
            out.append(style.indent +
                deprecated.MESSAGES.get(
                    api,
                    f'[Obsolete("See https://www.scintilla.org/ScintillaDoc.html#{msg}")]'))
            break

def getUnsafeModifier(methodName, returnType, param1Type, param2Type):
    """Add the `unsafe` qualifier if needed."""
    return 'unsafe ' \
        if (returnType == 'string' and methodName in specs.UNSAFE) \
            or 'Cells' in [returnType, param1Type, param2Type] \
        else ''

def translateReturnType(v, param1Type, param2Type):
    """Get the CLR type of the API's return value."""
    return 'string' \
        if param1Type == 'stringresult' or param2Type == 'stringresult' \
        else translateType(v['ReturnType'])

def getParameterList(param1Type, param1Name, param2Type, param2Name):
    """Format the API's parameter list."""
    first  = f'{param1Type} {param1Name}' if param1Type and param1Type != 'stringresult' else ''
    second = f'{param2Type} {param2Name}' if param2Type and param2Type != 'stringresult' else ''
    separator = ', ' if first and second else ''
    return first + separator + second

def printLexGatewayFile(f: Face, style: CommentLineStyle):
    """Generate the interface implementation source file."""
# -------------------------------------------------------------------
    def translateVariableAccess(name, t, wparam=False):
        """Get the CLR type of a marshalled parameter."""
        if not bool(name):
            return 'UnusedW' if wparam else 'Unused'
        res = name
        usign = 'U' if wparam else ''
        if t == 'bool':
            return f'new {usign}IntPtr({res} ? 1{usign} : 0{usign})'
        if t in ['string', 'stringresult', 'Cells']:
            return f'({usign}IntPtr){res}Ptr'
        if t in ['Colour', 'ColourAlpha', 'KeyModifier']:
            res = f'({usign}IntPtr){res}.Value'
        elif t in ['TextRangeFull', 'TextToFindFull']:
            res += '.NativePointer'
        else:
            res = f'({usign}IntPtr){res}'
        return res
# -------------------------------------------------------------------
    out = []
    for name in f.order:
        v = f.features[name]
        if v['FeatureType'] in ['fun', 'get', 'set']:
            param1Type = translateType(v['Param1Type'])
            param1Name = v['Param1Name']
            param2Type = translateType(v['Param2Type'])
            param2Name = v['Param2Name']
            returnType = translateReturnType(v, param1Type, param2Type)

            # don't clobber custom implementations that are too complex to script
            if name == 'SetRepresentation' or \
                (isTypeUnsupported(param1Type) or isTypeUnsupported(param2Type) or isTypeUnsupported(returnType)):
                continue

            # use the underlying API message and the document's encoding to
            # obtain the correct buffer length -- keep only the 'text' parameter
            if param1Name.lower() == 'length' and \
                (name in specs.INFERS_TEXT_LENGTH or \
                    (param2Name.lower() == 'text' and param2Type in ['string', 'stringresult'])):
                param1Type = ''

            if v['Category'] == 'Provisional':
                out.append('#if !SCI_DISABLE_PROVISIONAL')

            appendComment(style, out, v)

            if name == 'GetTag':
                out.append(getExceptionDoc(style,'Thrown if <paramref name="tagNumber"/> is less than 0.'))

            checkIfDeprecated(f, style, name, out)

            out.append(style.indent
                + f'public {getUnsafeModifier(name, returnType, param1Type, param2Type)}'
                + f'{returnType} {name}({getParameterList(param1Type, param1Name, param2Type, param2Name)})')

            if name in deprecated.STUBS:
                out[-1] += (f' {deprecated.STUBS.get(name, "{ }")}')
                out.append('')
                continue

            out.append(style.indent + '{')
            style.indent += _TAB

            if name == 'GetTag':
                out.append(f'{style.indent}if ({param1Name} < 0)')
                out.append(style.indent + '{')
                style.indent += _TAB
                out.append(f'{style.indent}throw new ArgumentException("{param1Name} must be non-negative integer");')
                style.indent = style.indent[len(_TAB):]
                out.append(style.indent + '}')

            if 'stringresult' in [param1Type, param2Type] and name in specs.UNSAFE:
                encoding = specs.UNSAFE.get(name, 'Encoding.UTF8')
                out.append(f'{style.indent}fixed (byte* {param1Name}Ptr = {encoding}.GetBytes($"{{{param1Name}}}\\0"))')
                out.append(style.indent + '{')

            if param1Type == 'string':
                style.indent += _TAB

            if param2Type == 'string':
                style.indent += _TAB

            if param2Type == 'Cells':
                out.append(f'{style.indent}fixed (char* {param2Name}Ptr = {param2Name}.Value)')
                out.append(style.indent + '{')
                style.indent += _TAB

            if param2Type == 'stringresult':
                style.indent += _TAB

            featureConstant = f'SciMsg.SCI_{name.upper()}'
            firstArg = translateVariableAccess(param1Name, param1Type, True)
            secondArg = translateVariableAccess(param2Name, param2Type)
            atMethodEnd = False

            if 'stringresult' in [param1Type, param2Type]:
                atMethodEnd = True
                params = ''

                if param1Type in ['Position', 'string', 'int'] or \
                    (name not in specs.INFERS_TEXT_LENGTH and bool(firstArg) and firstArg.rfind('Unused') < 0):
                    params = f', {firstArg}'

                encoding = \
                    'Encoding.UTF8' \
                    if name in specs.UNSAFE and name not in specs.INFERS_TEXT_LENGTH \
                    else specs.INFERS_TEXT_LENGTH.get(name, 'CodePage')

                res = f'GetNullStrippedStringFromMessageThatReturnsLength({featureConstant}, {encoding}{params})'
                out.append(f'{style.indent[len(_TAB):]}return {res};')

                if name in specs.UNSAFE:
                    out.append(style.indent[2*len(_TAB):] + '}')

            else:
                res = f'SendMessage(_scintilla, {featureConstant}, {firstArg}, {secondArg})'

            if (param2Type in ['string', 'RepresentationAppearance'] and \
                    returnType in ['Position', 'void']) or \
                  (returnType in ['RepresentationAppearance', 'Colour', 'ColourAlpha'] and \
                    param1Type == 'string') or \
                  name in ['ClearRepresentation', 'SetRepresentationColour', 'TextWidth']:

                atMethodEnd = True
                indent = style.indent[len(_TAB):]
                identifier = 'SendUTF8Bytes' if name in specs.ALWAYS_UNICODE else 'SendEncodedBytes'

                if param1Type == 'string' and param2Type == 'string':
                    indent = indent[len(_TAB):]
                    res = f'{identifier}({featureConstant}, {param1Name}, {param2Name})'
                elif param1Type == 'string':
                    res = f'{identifier}({featureConstant}, {param1Name}, {secondArg})'
                elif param2Type == 'string' and name not in specs.INFERS_TEXT_LENGTH:
                    res = f'{identifier}({featureConstant}, {param2Name}, {firstArg})'
                else:
                    res = f'{identifier}({featureConstant}, {param2Name})'

                if returnType in ['Colour', 'ColourAlpha']:
                    res = f'new {returnType}((int){res})'
                elif returnType not in ['Position', 'void']:
                    res = f'({returnType}){res}'

                out.append(f'{indent}{"return " if returnType != "void" else ''}{res};')

            elif not atMethodEnd and \
                (returnType in ['IntPtr', 'Position'] or 'stringresult' in [param1Type, param2Type]):
                out.append(f'{style.indent}return {res};')

            elif returnType == 'bool':
                out.append(f'{style.indent}return 1 == (int){res};')

            elif returnType in ['Colour', 'ColourAlpha']:
                out.append(f'{style.indent}return new {returnType}((int){res});')

            elif not atMethodEnd and returnType != 'void':
                indent = style.indent
                if name in ['GetPropertyInt', 'PropertyType']:
                    atMethodEnd = True
                    indent = indent[len(_TAB):]
                    res = f'SendUTF8Bytes({featureConstant}, {param1Name}, {secondArg})'

                out.append(f'{indent}return ({returnType}){res};')

            elif not atMethodEnd:
                out.append(f'{style.indent}{res};')

            if param1Type in ['string', 'Cells', 'stringresult']:
                style.indent = style.indent[len(_TAB):]

            if param2Type in ['string', 'Cells', 'stringresult']:
                style.indent = style.indent[len(_TAB):]
                if not atMethodEnd:
                    out.append(style.indent + '}')

            style.indent = style.indent[len(_TAB):]
            out.append(style.indent + '}')

            if v['Category'] == 'Provisional':
                out.append('#endif')

            out.append('')
    return out

def printLexIGatewayFile(f: Face, style: CommentLineStyle):
    """Generate the interface definition source file."""
    out = []
    for name in f.order:
        v = f.features[name]
        if v['FeatureType'] in ['fun', 'get', 'set']:
            param1Type = translateType(v['Param1Type'])
            param1Name = v['Param1Name']
            param2Type = translateType(v['Param2Type'])
            param2Name = v['Param2Name']
            returnType = translateReturnType(v, param1Type, param2Type)

            if (isTypeUnsupported(param1Type) or isTypeUnsupported(param2Type) or isTypeUnsupported(returnType)):
                continue

            if param1Name.lower() == 'length' and \
                (name in specs.INFERS_TEXT_LENGTH or \
                    (param2Name.lower() == 'text' and param2Type in ['string', 'stringresult'])):
                param1Type = ''

            if v['Category'] == 'Provisional':
                out.append('#if !SCI_DISABLE_PROVISIONAL')

            out.append(style.format(f'<inheritdoc cref="ScintillaGateway.{name}"/>'))

            checkIfDeprecated(f, style, name, out)

            out.append(style.indent
                + f'{getUnsafeModifier(name, returnType, param1Type, param2Type)}'
                + f'{returnType} {name}({getParameterList(param1Type, param1Name, param2Type, param2Name)});')

            if v['Category'] == 'Provisional':
                out.append('#endif')

            out.append('')
    return out

def printEnumDefinitions(f: Face, style: CommentLineStyle):
    """Generate enumerated interface constants."""
    out = []
    for name in f.order:
        v = f.features[name]
        if v['FeatureType'] == 'enu' and \
                name not in ['Keys', 'IndicatorStyle', 'ModificationFlags']: # for all except excluded enums [conflicting]

            if v['Category'] == 'Provisional':
                out.append('#if !SCI_DISABLE_PROVISIONAL')

            appendComment(style, out, v)

            if name in specs.BITMASKS:
                out.append(f'{style.indent}[Flags]')

            out.append(f'{style.indent}public enum {name}')
            out.append(style.indent + '{')

            for ename in f.order:
                ve = f.features[ename]
                prefix = v['Value']
                startPos = len(prefix)

                if ve['FeatureType'] == 'val' and ename.startswith(prefix):
                    style.indent += _TAB
                    checkIfDeprecated(f, style, ename, out)
                    style.indent = style.indent[len(_TAB):]

                    if startPos < len(ename) and ename[startPos] == '_':
                        startPos += 1

                    valname = ename[startPos:]

                    if valname[0].isdigit():
                        valname = '_' + valname	# for enums labels such as char encoding

                    if ve['Value'] == '0xFFFFFFFF':
                        ve['Value'] = '-1'	# reset back since these are signed enums

                    out.append(f'{style.indent}{_TAB}{valname} = {ve["Value"]},')

            out[-1] = out[-1].rstrip(',')
            out.append(style.indent + '}')

            if v['Category'] == 'Provisional':
                out.append('#endif')

            out.append('')
    return out

def generate():
    """Write the interface to the corresponding C# source files."""
    templatePath = os.path.join(os.path.dirname(__file__), '..', 'lib', 'Plugin')
    scintillaIfacePath = os.path.join(tempfile.gettempdir(), 'Scintilla.iface')

    if len(sys.argv) > 1:
        scintillaIfacePath = os.path.realpath(sys.argv[1])

    try:
        if not os.path.exists(scintillaIfacePath):
            with open(scintillaIfacePath, 'w', encoding='utf-8') as iface:
                iface.write(get_resource(SCINTILLA_IFACE))

        f = Face.Face()
        f.ReadFromFile(scintillaIfacePath)

        for file, generator in \
            ({
                'IScintillaGateway.cs': printLexIGatewayFile,
                'ScintillaGateway.cs': printLexGatewayFile,
                'GatewayDomain.cs': printEnumDefinitions,
             }).items():
            out_file = os.path.join(templatePath, file)
            FileGenerator.Regenerate(out_file, '/* ', generator(f, CommentLineStyle()))
            subprocess.run(['unix2dos', '-k', '-m', '-u', out_file], check=True, encoding='utf-8')
    except (subprocess.CalledProcessError, IOError) as err:
        print(str(err), file=sys.stderr)

# -------------------------------------------------------------------
if __name__ == '__main__':
    generate()
