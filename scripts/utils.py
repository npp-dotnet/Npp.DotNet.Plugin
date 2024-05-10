"""
 SPDX-FileCopyrightText: (c) 2024 Robert Di Pardo
 SPDX-License-Identifier: 0BSD
"""
import os
import subprocess
import sys

def get_current_user() -> str:
    return os.environ.get('USERNAME', 'user') \
        if sys.platform.lower() == 'windows' else os.environ.get('USER', 'user')

def get_hostname() -> str:
    return subprocess.check_output(['hostname'] \
        if sys.platform.lower() == 'windows' else ['uname', '-n']).decode('utf-8').rstrip()

def cmd_output_or_default(cmd: str, fallback: callable, default='') -> str:
    try:
        result = subprocess.check_output(cmd.split()).decode('utf-8')
    except (subprocess.CalledProcessError, IOError, AttributeError):
        try:
            result = fallback()
        except (subprocess.CalledProcessError, OSError, IOError, AttributeError):
            result = default

    return result.strip()
