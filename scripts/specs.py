"""
 SPDX-FileCopyrightText: (c) 2025 Robert Di Pardo
 SPDX-License-Identifier: 0BSD
"""
# API methods that invariably encode the input buffer as a .NET string
ALWAYS_UNICODE = [
    'AnnotationSetStyles',
    'AnnotationSetText',
    'AutoCSelect',
    'AutoCSetFillUps',
    'AutoCShow',
    'AutoCStops',
    'CallTipShow',
    'GetPropertyInt',
    'MarginSetStyles',
    'MarginSetText',
    'MarkerDefinePixmap',
    'MarkerDefineRGBAImage',
    'PropertyType',
    'RegisterImage',
    'RegisterRGBAImage',
    'SetDefaultFoldDisplayText',
    'SetIdentifiers',
    'SetKeyWords',
    'SetProperty',
    'SetPunctuationChars',
    'SetStylingEx',
    'SetWhitespaceChars',
    'SetWordChars',
    'StyleSetFont',
    'ToggleFoldShowText',
    'TypeProperty',
    'UserListShow',
]

# API methods the obtain the required buffer size implictly, mapped to their preferred encodings
INFERS_TEXT_LENGTH = {
    'AddText': 'CodePage',
    'AnnotationGetStyles':'Encoding.UTF8',
    'AnnotationGetText': 'Encoding.UTF8',
    'AppendText': 'CodePage',
    'AutoCGetCurrentText': 'Encoding.UTF8',
    'ChangeInsertion': 'CodePage',
    'ChangeLastUndoActionText': 'CodePage',
    'CopyText': 'CodePage',
    'DescribeKeyWordSets': 'Encoding.UTF8',
    'DescriptionOfStyle': 'Encoding.UTF8',
    'EOLAnnotationGetText': 'Encoding.UTF8',
    'EncodedFromUTF8': 'CodePage',
    'GetCopySeparator': 'Encoding.UTF8',
    'GetCurLine': 'CodePage',
    'GetDefaultFoldDisplayText': 'Encoding.UTF8',
    'GetFontLocale': 'Encoding.UTF8',
    'GetLexerLanguage': 'Encoding.UTF8',
    'GetPunctuationChars': 'Encoding.UTF8',
    'GetSubStyleBases': 'Encoding.UTF8',
    'GetText': 'CodePage',
    'GetWhitespaceChars': 'Encoding.UTF8',
    'GetWordChars': 'Encoding.UTF8',
    'MarginGetStyles': 'Encoding.UTF8',
    'MarginGetText': 'Encoding.UTF8',
    'NameOfStyle': 'Encoding.UTF8',
    'PropertyNames': 'Encoding.UTF8',
    'ReplaceRectangular': 'CodePage',
    'ReplaceTarget': 'CodePage',
    'ReplaceTargetMinimal': 'CodePage',
    'ReplaceTargetRE': 'CodePage',
    'SearchInTarget': 'CodePage',
    'StyleGetFont': 'Encoding.UTF8',
    'TagsOfStyle': 'Encoding.UTF8',
    'TargetAsUTF8': 'Encoding.UTF8',
}

# API methods requiring the `unsafe` qualifier,  mapped to their preferred encodings
UNSAFE = {
    'DescribeProperty': 'Encoding.UTF8',
    'EncodedFromUTF8': 'Encoding.UTF8',
    'GetProperty': 'Encoding.UTF8',
    'GetPropertyExpanded': 'Encoding.UTF8',
    'GetRepresentation': 'CodePage',
    'SetRepresentation': 'Encoding.UTF8',
}

# API enumeratations requiring the `System.FlagsAttribute` annotation
BITMASKS = [
    'AutomaticFold',
    'CaretPolicy',
    'CaretStyle',
    'FindOption',
    'FoldAction',
    'FoldFlag',
    'KeyMod',
    'ModificationFlags',
    'Update',
]

