#!/usr/bin/env python3
"""
 SPDX-FileCopyrightText: (c) 2024 Robert Di Pardo
 SPDX-License-Identifier: 0BSD
"""
import os
import re
import subprocess
import sys
import tempfile
from datetime import datetime
from io import StringIO
from itertools import takewhile

import utils as u
from get_sci_doc import ScintillaDefinitions, get_resource

TAG='v8.8.2'
OUTPUT=os.path.join(os.path.dirname(__file__), '..', 'lib', 'Plugin', 'NppMenuCmdIds.cs')
MENUCMDID_H=f'https://raw.githubusercontent.com/notepad-plus-plus/notepad-plus-plus/refs/tags/{TAG}/PowerEditor/src/menuCmdID.h'
RESOURCE_H=f'https://raw.githubusercontent.com/notepad-plus-plus/notepad-plus-plus/refs/tags/{TAG}/PowerEditor/src/resource.h'
PREFERNECE_RC_H=f'https://raw.githubusercontent.com/notepad-plus-plus/notepad-plus-plus/refs/tags/{TAG}/PowerEditor/src/WinControls/Preference/preference_rc.h'

CS_FILE_START=f"""/*
 * SPDX-FileCopyrightText: {datetime.today().year} {
 u.cmd_output_or_default('git config --get user.name', u.get_current_user)} <{
 u.cmd_output_or_default('git config --get user.email', u.get_hostname, f'{u.get_current_user()}@localhost')}>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using static Npp.DotNet.Plugin.Constants;

namespace Npp.DotNet.Plugin
{{"""

def generate(out: StringIO):
    """
    Extract definitions from a C++ header and write them to a new C# source file.
    """
    try:
        docs = ScintillaDefinitions()
        for key, val in \
            ({
                ('menuCmdID.h', 'MenuCmdId'): MENUCMDID_H,
                ('resource.h', 'Resource'): RESOURCE_H,
                ('preference_rc.h', 'Preference'): PREFERNECE_RC_H
             }).items():

            hdr_path = os.path.join(tempfile.gettempdir(), key[0])
            if not os.path.exists(hdr_path):
                with open(hdr_path, 'w', encoding='utf-8') as msgs:
                    msgs.write(get_resource(val))

            version = \
                re.search(r'(?i)(?:^.*NOTEPAD_PLUS_VERSION L"Notepad\+\+ )?(?P<version>.*)"\s*$',
                         get_resource(RESOURCE_H)[:1024], re.MULTILINE)

            version = version.groupdict()['version'] if version is not None else ''
            print(f"\n    /// <remarks>Definitions for Notepad++ {version}</remarks>", file=out)
            print(f"    public enum {key[1]} : uint\n    {{", file=out)

            with open(hdr_path, 'r', encoding='utf-8') as hdr:
                for line in hdr.read().splitlines():
                    try:
                        macro = re.match(r'^#(if|end)(n?def|if)', line)
                        if macro is not None:
                            print(u.c_preproc_to_csharp(line, macro), file=out)
                        elif re.search(r'^\s*(?!\/\/\s*)#define(?!.*_?(VERSION)_?)', line):
                            decl = re.sub(r'\-1$', '0xFFFFFFFF', line).split()
                            if len(decl) >= 3:
                                val = ' '.join(takewhile(lambda s: not s.startswith('/'), decl[2:]))
                                print(f"{docs.style.indent}{decl[1].upper()} = {val},", file=out)

                    except (IndexError, AttributeError):
                        pass

            print('    }', file=out)

    except IOError as io_err:
        print(str(io_err), file=sys.stderr)

# -------------------------------------------------------------------
if __name__ == '__main__':
    try:
        cs_file = StringIO()
        print(CS_FILE_START, end='', file=cs_file)
        generate(cs_file)
        print('}', file=cs_file)
        with open(OUTPUT, 'w', encoding='utf-8') as cs_msgs:
            cs_msgs.write(cs_file.getvalue())

        subprocess.run(['unix2dos', '-k', '-m', '-u', OUTPUT], check=True, encoding='utf-8')

    except (subprocess.CalledProcessError, IOError) as err:
        print(str(err), file=sys.stderr)

    finally:
        cs_file.close()
