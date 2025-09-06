"""
 SPDX-FileCopyrightText: (c) 2024 Robert Di Pardo
 SPDX-License-Identifier: 0BSD
"""
__all__ = ['MESSAGES', 'STUBS']

_NO_STYLE_BITS = '[Obsolete("Scintilla no longer supports style byte indicators: https://www.scintilla.org/ScintillaDoc.html#Indicators")]'
_NO_SINGE_PHASE_DRAW = '[Obsolete("Use SC_PHASES_TWO or SC_PHASES_MULTIPLE instead: https://www.scintilla.org/ScintillaDoc.html#SCI_GETTWOPHASEDRAW")]'
_USE_ELEMENT_APIS = '[Obsolete("Use the element colours APIs instead: https://www.scintilla.org/ScintillaDoc.html#ElementColours")]'
_USE_LAYER_APIS = '[Obsolete("Use SCI_SETSELECTIONLAYER instead: https://www.scintilla.org/ScintillaDoc.html#SCI_SETSELECTIONLAYER")]'
_USE_NEW_INDICATORS = '[Obsolete("Use the INDICATOR_* range numbers instead: https://www.scintilla.org/ScintillaDoc.html#Indicators")]'
_USE_NEW_CHRNG_APIS = '[Obsolete("Use the 64-bit character range APIs instead: https://groups.google.com/g/scintilla-interest/c/mPLwYdC0-FE")]'
_USE_GETPROPERTY = \
    '[Obsolete("This is now the same as SCI_GETPROPERTY - no expansion is performed. ' + \
    'See https://www.scintilla.org/ScintillaDoc.html#SCI_GETPROPERTYEXPANDED")]'

MESSAGES = {
    'SCI_SETSTYLEBITS': _NO_STYLE_BITS,
    'SCI_GETSTYLEBITS': _NO_STYLE_BITS,
    'SCI_GETSTYLEBITSNEEDED': _NO_STYLE_BITS,
    'SC_PHASES_ONE': _NO_SINGE_PHASE_DRAW,
    'SCI_SETSELFORE': _USE_ELEMENT_APIS,
    'SCI_SETSELBACK': _USE_ELEMENT_APIS,
    'SCI_SETSELALPHA': _USE_ELEMENT_APIS,
    'SCI_GETSELALPHA': _USE_ELEMENT_APIS,
    'SCI_SETCARETFORE': _USE_ELEMENT_APIS,
    'SCI_GETCARETFORE': _USE_ELEMENT_APIS,
    'SCI_SETCARETLINEVISIBLE': _USE_ELEMENT_APIS,
    'SCI_GETCARETLINEVISIBLE': _USE_ELEMENT_APIS,
    'SCI_SETCARETLINEBACK': _USE_ELEMENT_APIS,
    'SCI_GETCARETLINEBACK': _USE_ELEMENT_APIS,
    'SCI_SETCARETLINEBACKALPHA': _USE_ELEMENT_APIS,
    'SCI_GETCARETLINEBACKALPHA': _USE_ELEMENT_APIS,
    'SCI_SETHOTSPOTACTIVEFORE': _USE_ELEMENT_APIS,
    'SCI_GETHOTSPOTACTIVEFORE': _USE_ELEMENT_APIS,
    'SCI_SETHOTSPOTACTIVEBACK': _USE_ELEMENT_APIS,
    'SCI_GETHOTSPOTACTIVEBACK': _USE_ELEMENT_APIS,
    'SCI_SETADDITIONALSELALPHA': _USE_ELEMENT_APIS,
    'SCI_GETADDITIONALSELALPHA': _USE_ELEMENT_APIS,
    'SCI_SETADDITIONALSELFORE': _USE_ELEMENT_APIS,
    'SCI_SETADDITIONALSELBACK': _USE_ELEMENT_APIS,
    'SCI_SETADDITIONALCARETFORE': _USE_ELEMENT_APIS,
    'SCI_GETADDITIONALCARETFORE': _USE_ELEMENT_APIS,
    'SC_ALPHA_NOALPHA': _USE_LAYER_APIS,
    'INDIC_CONTAINER':_USE_NEW_INDICATORS,
    'INDIC_IME': _USE_NEW_INDICATORS,
    'INDIC_IME_MAX': _USE_NEW_INDICATORS,
    'INDIC_MAX': _USE_NEW_INDICATORS,
    'SCI_FINDTEXT': _USE_NEW_CHRNG_APIS,
    'SCI_FORMATRANGE': _USE_NEW_CHRNG_APIS,
    'SCI_GETTEXTRANGE': _USE_NEW_CHRNG_APIS,
    'SCI_GETSTYLEDTEXT': _USE_NEW_CHRNG_APIS,
    'SCI_GETPROPERTYEXPANDED': _USE_GETPROPERTY,
}

# Dummy implementations of deprecated or obsolete APIs
STUBS = {
    'GetCaretFore': '=> new Colour(0);',
    'GetCaretLineBack': '=> new Colour(0xffffff);',
    'GetCaretLineBackAlpha': '=> default;',
    'GetCaretLineVisible': '=> true;',
    'GetHotspotActiveBack': '=> new Colour(0xffffff);',
    'GetHotspotActiveFore': '=> new Colour(0);',
    'GetKeysUnicode': '=> true;',
    'GetSelAlpha': '=> default;',
    'GetStyleBits': '=> 8;',
    'GetStyleBitsNeeded': '=> GetStyleBits();',
    'GetTwoPhaseDraw': '=> true;',
    'SetCaretFore': '{ }',
    'SetCaretLineBack': '{ }',
    'SetCaretLineBackAlpha': '{ }',
    'SetCaretLineVisible': '{ }',
    'SetHotspotActiveBack': '{ }',
    'SetHotspotActiveFore': '{ }',
    'SetKeysUnicode': '{ }',
    'SetSelAlpha': '{ }',
    'SetSelBack': '{ }',
    'SetSelFore': '{ }',
    'SetStyleBits': '{ }',
    'SetTwoPhaseDraw': '{ }',
}
