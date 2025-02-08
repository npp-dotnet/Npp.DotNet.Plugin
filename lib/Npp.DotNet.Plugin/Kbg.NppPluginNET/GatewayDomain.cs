/*
 * SPDX-FileCopyrightText: 2016 Kasper B. Graversen <https://github.com/kbilsted>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System;
using System.Runtime.InteropServices;

namespace Npp.DotNet.Plugin
{
    /// <summary>
    /// Colours are set using the RGB format (Red, Green, Blue). The intensity of each colour is set in the range 0 to 255.
    /// If you have three such intensities, they are combined as: red | (green &lt;&lt; 8) | (blue &lt;&lt; 16).
    /// If you set all intensities to 255, the colour is white. If you set all intensities to 0, the colour is black.
    /// When you set a colour, you are making a request. What you will get depends on the capabilities of the system and the current screen mode.
    /// </summary>
    public readonly struct Colour
    {
        public readonly int Red, Green, Blue;

        public Colour(int rgb)
        {
            Red = rgb & 0xFF;
            Green = (rgb >> 8) & 0xFF;
            Blue = (rgb >> 16) & 0xFF;
        }

        /// <summary></summary>
        /// <param name="red">a number 0-255</param>
        /// <param name="green">a number 0-255</param>
        /// <param name="blue">a number 0-255</param>
        public Colour(int red, int green, int blue)
        {
            if (red > 255 || red < 0)
                throw new ArgumentOutOfRangeException("red", "must be 0-255");
            if (green > 255 || green < 0)
                throw new ArgumentOutOfRangeException("green", "must be 0-255");
            if (blue > 255 || blue < 0)
                throw new ArgumentOutOfRangeException("blue", "must be 0-255");
            Red = red;
            Green = green;
            Blue = blue;
        }

        public int Value
        {
            get { return Red + (Green << 8) + (Blue << 16); }
        }
    }

    /// <summary>
    /// Positions within the Scintilla document refer to a character or the gap before that character.
    /// The first character in a document is 0, the second 1 and so on. If a document contains nLen characters, the last character is numbered nLen-1. The caret exists between character positions and can be located from before the first character (0) to after the last character (nLen).
    /// <br/>
    /// There are places where the caret can not go where two character bytes make up one character.
    /// This occurs when a DBCS character from a language like Japanese is included in the document or when line ends are marked with the CP/M
    /// standard of a carriage return followed by a line feed.The INVALID_POSITION constant(-1) represents an invalid position within the document.
    /// <br/>
    /// All lines of text in Scintilla are the same height, and this height is calculated from the largest font in any current style.This restriction
    /// is for performance; if lines differed in height then calculations involving positioning of text would require the text to be styled first.
    /// <br/>
    /// If you use messages, there is nothing to stop you setting a position that is in the middle of a CRLF pair, or in the middle of a 2 byte character.
    /// However, keyboard commands will not move the caret into such positions.
    /// </summary>
    public readonly struct Position
    {
        private readonly Int64 _pos;

        public Position(IntPtr ptr) : this(ptr.ToInt64())
        { }

        public Position(Int64 pos)
        {
            this._pos = pos;
        }

        public Int64 Value
        {
            get { return _pos; }
        }

        public static explicit operator IntPtr(Position pos) => new IntPtr(pos._pos);
        public static explicit operator UIntPtr(Position pos) => new UIntPtr((ulong)pos._pos);
        public static implicit operator Position(IntPtr ptrInt) => new Position(ptrInt);
        public static implicit operator Position(long pLong) => new Position(pLong);
        public static implicit operator long(Position pos) => pos._pos;

        public static Position operator +(Position a, Position b)
        {
            return new Position(a._pos + b._pos);
        }

        public static Position operator -(Position a, Position b)
        {
            return new Position(a._pos - b._pos);
        }

        public static bool operator ==(Position a, Position b)
        {
            return a._pos == b._pos;
        }

        public static bool operator !=(Position a, Position b)
        {
            return !(a == b);
        }

        public static bool operator >(Position a, Position b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <(Position a, Position b)
        {
            return a.Value < b.Value;
        }

        public static Position Min(Position a, Position b)
        {
            return a < b ? a : b;
        }

        public static Position Max(Position a, Position b)
        {
            return a > b ? a : b;
        }

        public override string ToString()
        {
            return "Postion: " + _pos;
        }

        public bool Equals(Position other)
        {
            return _pos == other._pos;
        }

        public override bool Equals(object obj)
        {
            return obj.GetType() == this.GetType() && Equals((Position)obj);
        }

        public override int GetHashCode()
        {
            return _pos.GetHashCode();
        }
    }

    /// <summary>
    /// Class containing key and modifiers
    /// <br/>
    /// The key code is a visible or control character or a key from the SCK_* enumeration, which contains:
    /// SCK_ADD, SCK_BACK, SCK_DELETE, SCK_DIVIDE, SCK_DOWN, SCK_END, SCK_ESCAPE, SCK_HOME, SCK_INSERT, SCK_LEFT, SCK_MENU, SCK_NEXT(Page Down), SCK_PRIOR(Page Up), S
    /// CK_RETURN, SCK_RIGHT, SCK_RWIN, SCK_SUBTRACT, SCK_TAB, SCK_UP, and SCK_WIN.
    /// <br/>
    /// The modifiers are a combination of zero or more of SCMOD_ALT, SCMOD_CTRL, SCMOD_SHIFT, SCMOD_META, and SCMOD_SUPER.
    /// On OS X, the Command key is mapped to SCMOD_CTRL and the Control key to SCMOD_META.SCMOD_SUPER is only available on GTK+ which is commonly the Windows key.
    /// If you are building a table, you might want to use SCMOD_NORM, which has the value 0, to mean no modifiers.
    /// </summary>
    public readonly struct KeyModifier
    {
        private readonly int _value;

        /// <summary>
        /// The key code is a visible or control character or a key from the SCK_* enumeration, which contains:
        /// SCK_ADD, SCK_BACK, SCK_DELETE, SCK_DIVIDE, SCK_DOWN, SCK_END, SCK_ESCAPE, SCK_HOME, SCK_INSERT, SCK_LEFT, SCK_MENU, SCK_NEXT(Page Down), SCK_PRIOR(Page Up),
        /// SCK_RETURN, SCK_RIGHT, SCK_RWIN, SCK_SUBTRACT, SCK_TAB, SCK_UP, and SCK_WIN.
        /// <br/>
        /// The modifiers are a combination of zero or more of SCMOD_ALT, SCMOD_CTRL, SCMOD_SHIFT, SCMOD_META, and SCMOD_SUPER.
        /// On OS X, the Command key is mapped to SCMOD_CTRL and the Control key to SCMOD_META.SCMOD_SUPER is only available on GTK+ which is commonly the Windows key.
        /// If you are building a table, you might want to use SCMOD_NORM, which has the value 0, to mean no modifiers.
        /// </summary>
        public KeyModifier(SciMsg SCK_KeyCode, SciMsg SCMOD_modifier)
        {
            _value = (int)SCK_KeyCode | ((int)SCMOD_modifier << 16);
        }

        public int Value
        {
            get { return _value; }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CharacterRangeFull
    {
        public CharacterRangeFull(long cpmin, long cpmax) { CpMin = new IntPtr(cpmin); CpMax = new IntPtr(cpmax); }
        public IntPtr CpMin;
        public IntPtr CpMax;
    }

    [Obsolete("Use CharacterRangeFull instead")]
    [StructLayout(LayoutKind.Sequential)]
    public struct CharacterRange
    {
        public CharacterRange(int cpmin, int cpmax) { cpMin = new IntPtr(cpmin); cpMax = new IntPtr(cpmax); }
        public IntPtr cpMin;
        public IntPtr cpMax;
    }

    public readonly struct Cells
    {
        internal readonly char[] CharactersAndStyles;

        public Cells(char[] charactersAndStyles)
        {
            this.CharactersAndStyles = charactersAndStyles;
        }
        public char[] Value { get { return CharactersAndStyles; } }
    }

    public class TextRangeFull : IDisposable
    {
        Sci_TextRangeFull _sciTextRange;
        IntPtr _ptrSciTextRange;
        bool _disposed = false;

        public TextRangeFull(CharacterRangeFull chrRange, int stringCapacity)
        {
            _sciTextRange.ChRg = chrRange;
            _sciTextRange.LpStrText = Marshal.AllocHGlobal(stringCapacity * Marshal.SystemDefaultCharSize);
        }

        public TextRangeFull(long cpmin, long cpmax, int stringCapacity)
        {
            _sciTextRange.ChRg.CpMin = new IntPtr(cpmin);
            _sciTextRange.ChRg.CpMax = new IntPtr(cpmax);
            _sciTextRange.LpStrText = Marshal.AllocHGlobal(stringCapacity * Marshal.SystemDefaultCharSize);
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Sci_TextRangeFull
        {
            public CharacterRangeFull ChRg;
            public IntPtr LpStrText;
        }

        public IntPtr NativePointer { get { InitNativeStruct(); return _ptrSciTextRange; } }
        public string LpStrText { get { ReadNativeStruct(); return Marshal.PtrToStringAnsi(_sciTextRange.LpStrText); } }
        public CharacterRangeFull ChRg { get { ReadNativeStruct(); return _sciTextRange.ChRg; } set { _sciTextRange.ChRg = value; InitNativeStruct(); } }

        void InitNativeStruct()
        {
            if (_ptrSciTextRange == IntPtr.Zero)
                _ptrSciTextRange = Marshal.AllocHGlobal(Marshal.SizeOf(_sciTextRange));
            Marshal.StructureToPtr(_sciTextRange, _ptrSciTextRange, false);
        }

        unsafe void ReadNativeStruct()
        {
            if (_ptrSciTextRange != IntPtr.Zero)
                _sciTextRange = *(Sci_TextRangeFull*)_ptrSciTextRange;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_sciTextRange.LpStrText != IntPtr.Zero) Marshal.FreeHGlobal(_sciTextRange.LpStrText);
                if (_ptrSciTextRange != IntPtr.Zero) Marshal.FreeHGlobal(_ptrSciTextRange);
                GC.SuppressFinalize(this);
                _disposed = true;
            }
        }

        ~TextRangeFull()
        {
            Dispose();
        }
    }

    [Obsolete("Use TextRangeFull instead")]
    public class TextRange : IDisposable
    {
        Sci_TextRange _sciTextRange;
        IntPtr _ptrSciTextRange;
        bool _disposed = false;

        public TextRange(CharacterRange chrRange, int stringCapacity)
        {
            _sciTextRange.chrg = chrRange;
            _sciTextRange.lpstrText = Marshal.AllocHGlobal(stringCapacity * Marshal.SystemDefaultCharSize);
        }
        public TextRange(int cpmin, int cpmax, int stringCapacity)
        {
            _sciTextRange.chrg.cpMin = new IntPtr(cpmin);
            _sciTextRange.chrg.cpMax = new IntPtr(cpmax);
            _sciTextRange.lpstrText = Marshal.AllocHGlobal(stringCapacity * Marshal.SystemDefaultCharSize);
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Sci_TextRange
        {
            public CharacterRange chrg;
            public IntPtr lpstrText;
        }
        public IntPtr NativePointer { get { _initNativeStruct(); return _ptrSciTextRange; } }
        public string lpstrText { get { _readNativeStruct(); return Marshal.PtrToStringAnsi(_sciTextRange.lpstrText); } }
        public CharacterRange chrg { get { _readNativeStruct(); return _sciTextRange.chrg; } set { _sciTextRange.chrg = value; _initNativeStruct(); } }

        void _initNativeStruct()
        {
            if (_ptrSciTextRange == IntPtr.Zero)
                _ptrSciTextRange = Marshal.AllocHGlobal(Marshal.SizeOf(_sciTextRange));
            Marshal.StructureToPtr(_sciTextRange, _ptrSciTextRange, false);
        }

        unsafe void _readNativeStruct()
        {
            if (_ptrSciTextRange != IntPtr.Zero)
                _sciTextRange = *(Sci_TextRange*)_ptrSciTextRange;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_sciTextRange.lpstrText != IntPtr.Zero) Marshal.FreeHGlobal(_sciTextRange.lpstrText);
                if (_ptrSciTextRange != IntPtr.Zero) Marshal.FreeHGlobal(_ptrSciTextRange);
                GC.SuppressFinalize(this);
                _disposed = true;
            }
        }

        ~TextRange()
        {
            Dispose();
        }
    }


    /* ++Autogenerated -- start of section automatically generated from Scintilla.iface */
    /// <summary>Is undo history being collected? (Scintilla feature SCWS_)</summary>
    public enum WhiteSpace
    {
        INVISIBLE = 0,
        VISIBLEALWAYS = 1,
        VISIBLEAFTERINDENT = 2,
        VISIBLEONLYININDENT = 3
    }
    /// <summary>Make white space characters invisible, always visible or visible outside indentation. (Scintilla feature SCTD_)</summary>
    public enum TabDrawMode
    {
        LONGARROW = 0,
        STRIKEOUT = 1
    }
    /// <summary>Retrieve the position of the last correctly styled character. (Scintilla feature SC_EOL_)</summary>
    public enum EndOfLine
    {
        CRLF = 0,
        CR = 1,
        LF = 2
    }
    /// <summary>
    /// Set the code page used to interpret the bytes of the document as characters.
    /// The SC_CP_UTF8 value can be used to enter Unicode mode.
    /// (Scintilla feature SC_IME_)
    /// </summary>
    public enum IMEInteraction
    {
        WINDOWED = 0,
        INLINE = 1
    }
    /// <summary>Choose to display the the IME in a winow or inline. (Scintilla feature SC_MARK_)</summary>
    public enum MarkerSymbol
    {
        CIRCLE = 0,
        ROUNDRECT = 1,
        ARROW = 2,
        SMALLRECT = 3,
        SHORTARROW = 4,
        EMPTY = 5,
        ARROWDOWN = 6,
        MINUS = 7,
        PLUS = 8,
        VLINE = 9,
        LCORNER = 10,
        TCORNER = 11,
        BOXPLUS = 12,
        BOXPLUSCONNECTED = 13,
        BOXMINUS = 14,
        BOXMINUSCONNECTED = 15,
        LCORNERCURVE = 16,
        TCORNERCURVE = 17,
        CIRCLEPLUS = 18,
        CIRCLEPLUSCONNECTED = 19,
        CIRCLEMINUS = 20,
        CIRCLEMINUSCONNECTED = 21,
        BACKGROUND = 22,
        DOTDOTDOT = 23,
        ARROWS = 24,
        PIXMAP = 25,
        FULLRECT = 26,
        LEFTRECT = 27,
        AVAILABLE = 28,
        UNDERLINE = 29,
        RGBAIMAGE = 30,
        BOOKMARK = 31,
        VERTICALBOOKMARK = 32,
        CHARACTER = 10000
    }
    /// <summary>Invisible mark that only sets the line background colour. (Scintilla feature SC_MARKNUM_)</summary>
    public enum MarkerOutline
    {
        FOLDEREND = 25,
        FOLDEROPENMID = 26,
        FOLDERMIDTAIL = 27,
        FOLDERTAIL = 28,
        FOLDERSUB = 29,
        FOLDER = 30,
        FOLDEROPEN = 31
    }
    /// <summary>Set the alpha used for a marker that is drawn in the text area, not the margin. (Scintilla feature SC_MARGIN_)</summary>
    public enum MarginType
    {
        SYMBOL = 0,
        NUMBER = 1,
        BACK = 2,
        FORE = 3,
        TEXT = 4,
        RTEXT = 5,
        COLOUR = 6
    }
    /// <summary>Styles in range 32..39 are predefined for parts of the UI and are not used as normal styles. (Scintilla feature STYLE_)</summary>
    public enum StylesCommon
    {
        DEFAULT = 32,
        LINENUMBER = 33,
        BRACELIGHT = 34,
        BRACEBAD = 35,
        CONTROLCHAR = 36,
        INDENTGUIDE = 37,
        CALLTIP = 38,
        FOLDDISPLAYTEXT = 39,
        LASTPREDEFINED = 39,
        MAX = 255
    }
    /// <summary>
    /// Character set identifiers are used in StyleSetCharacterSet.
    /// The values are the same as the Windows *_CHARSET values.
    /// (Scintilla feature SC_CHARSET_)
    /// </summary>
    public enum CharacterSet
    {
        ANSI = 0,
        DEFAULT = 1,
        BALTIC = 186,
        CHINESEBIG5 = 136,
        EASTEUROPE = 238,
        GB2312 = 134,
        GREEK = 161,
        HANGUL = 129,
        MAC = 77,
        OEM = 255,
        RUSSIAN = 204,
        OEM866 = 866,
        CYRILLIC = 1251,
        SHIFTJIS = 128,
        SYMBOL = 2,
        TURKISH = 162,
        JOHAB = 130,
        HEBREW = 177,
        ARABIC = 178,
        VIETNAMESE = 163,
        THAI = 222,
        _8859_15 = 1000
    }
    /// <summary>Set a style to be underlined or not. (Scintilla feature SC_CASE_)</summary>
    public enum CaseVisible
    {
        MIXED = 0,
        UPPER = 1,
        LOWER = 2,
        CAMEL = 3
    }
    /// <summary>Get the size of characters of a style in points multiplied by 100 (Scintilla feature SC_WEIGHT_)</summary>
    public enum FontWeight
    {
        NORMAL = 400,
        SEMIBOLD = 600,
        BOLD = 700
    }
    /// <summary>Indicator style enumeration and some constants (Scintilla feature INDIC_)</summary>
    public enum IndicatorStyle
    {
        PLAIN = 0,
        SQUIGGLE = 1,
        TT = 2,
        DIAGONAL = 3,
        STRIKE = 4,
        HIDDEN = 5,
        BOX = 6,
        ROUNDBOX = 7,
        STRAIGHTBOX = 8,
        DASH = 9,
        DOTS = 10,
        SQUIGGLELOW = 11,
        DOTBOX = 12,
        SQUIGGLEPIXMAP = 13,
        COMPOSITIONTHICK = 14,
        COMPOSITIONTHIN = 15,
        FULLBOX = 16,
        TEXTFORE = 17,
        POINT = 18,
        POINTCHARACTER = 19,
        GRADIENT = 20,
        GRADIENTCENTRE = 21,
        /// <summary>
        /// Draw a triangle above the start of the indicator range.
        /// </summary>
        /// <remarks>
        /// Added in <a href="https://sourceforge.net/p/scintilla/code/ci/2ca5745f7e8e">5.3.0</a>
        /// </remarks>
        POINTTOP = 22,
        /// <summary>
        /// Defined by Notepad++ (not in Scintilla).
        /// </summary>
        EXPLORERLINK = 23,
    }
    /// <remarks>
    /// Added in <a href="https://sourceforge.net/p/scintilla/code/ci/7f7dbf80b0d6">5.3.0</a>
    /// </remarks>
    public enum ChangeHistoryIndicators
    {
        /// <summary>Text was deleted and saved but then reverted to its original state.</summary>
        REVERTED_TO_ORIGIN_INSERTION = 36,
        /// <summary>Text was inserted and saved but then reverted to its original state.</summary>
        REVERTED_TO_ORIGIN_DELETION = 37,
        /// <summary>Text was inserted and saved.</summary>
        SAVED_INSERTION = 38,
        /// <summary>Text was deleted and saved.</summary>
        SAVED_DELETION = 39,
        /// <summary>Text was inserted but not yet saved.</summary>
        MODIFIED_INSERTION = 40,
        /// <summary>Text was deleted but not yet saved.</summary>
        MODIFIED_DELETION = 41,
        /// <summary>Text was deleted and saved but then reverted but not to its original state.</summary>
        REVERTED_TO_MODIFIED_INSERTION = 42,
        /// <summary>Text was inserted and saved but then reverted but not to its original state.</summary>
        REVERTED_TO_MODIFIED_DELETION = 43,
    }
    /// <summary>
    /// INDIC_CONTAINER, INDIC_IME, INDIC_IME_MAX, and INDIC_MAX are indicator numbers,
    /// not IndicatorStyles so should not really be in the INDIC_ enumeration.
    /// They are redeclared in IndicatorNumbers INDICATOR_.
    /// (Scintilla feature INDICATOR_)
    /// </summary>
    public enum IndicatorNumbers
    {
        CONTAINER = 8,
        IME = 32,
        IME_MAX = 35,
        MAX = 43
    }
    /// <summary>Retrieve the foreground hover colour of an indicator. (Scintilla feature SC_INDICVALUE)</summary>
    public enum IndicValue
    {
        BIT = 0x1000000,
        MASK = 0xFFFFFF
    }
    /// <summary>Retrieve the foreground hover colour of an indicator. (Scintilla feature SC_INDICFLAG_)</summary>
    public enum IndicFlag
    {
        VALUEFORE = 1
    }
    /// <summary>Is the horizontal scroll bar visible? (Scintilla feature SC_IV_)</summary>
    public enum IndentView
    {
        NONE = 0,
        REAL = 1,
        LOOKFORWARD = 2,
        LOOKBOTH = 3
    }
    /// <summary>Returns the print magnification. (Scintilla feature SC_PRINT_)</summary>
    public enum PrintOption
    {
        NORMAL = 0,
        INVERTLIGHT = 1,
        BLACKONWHITE = 2,
        COLOURONWHITE = 3,
        COLOURONWHITEDEFAULTBG = 4,
        SCREENCOLOURS = 5
    }
    /// <summary>Returns the print colour mode. (Scintilla feature SCFIND_)</summary>
    [Flags]
    public enum FindOption
    {
        NONE = 0x0,
        WHOLEWORD = 0x2,
        MATCHCASE = 0x4,
        WORDSTART = 0x00100000,
        REGEXP = 0x00200000,
        POSIX = 0x00400000,
        CXX11REGEX = 0x00800000
    }
    /// <summary>The number of display lines needed to wrap a document line (Scintilla feature SC_FOLDLEVEL)</summary>
    public enum FoldLevel
    {
        BASE = 0x400,
        WHITEFLAG = 0x1000,
        HEADERFLAG = 0x2000,
        NUMBERMASK = 0x0FFF
    }
    /// <summary>Switch a header line between expanded and contracted and show some text after the line. (Scintilla feature SC_FOLDDISPLAYTEXT_)</summary>
    public enum FoldDisplayTextStyle
    {
        HIDDEN = 0,
        STANDARD = 1,
        BOXED = 2
    }
    /// <summary>Get the default fold display text. (Scintilla feature SC_FOLDACTION_)</summary>
    [Flags]
    public enum FoldAction
    {
        CONTRACT = 0,
        EXPAND = 1,
        TOGGLE = 2,
        /// <summary>
        /// Used for <see cref="SciMsg.SCI_FOLDALL"/> only, can be combined with <see cref="CONTRACT"/> or <see cref="TOGGLE"/> to contract all levels instead of only top-level.
        /// </summary>
        /// <remarks>
        /// Added in <a href="https://sourceforge.net/p/scintilla/code/ci/7de5b28bba1c">5.3.0</a>
        /// </remarks>
        CONTRACTEVERYLEVEL = 4
    }
    /// <summary>Ensure a particular line is visible by expanding any header line hiding it. (Scintilla feature SC_AUTOMATICFOLD_)</summary>
    [Flags]
    public enum AutomaticFold
    {
        NONE = 0,
        SHOW = 0x0001,
        CLICK = 0x0002,
        CHANGE = 0x0004
    }
    /// <summary>Get automatic folding behaviours. (Scintilla feature SC_FOLDFLAG_)</summary>
    [Flags]
    public enum FoldFlag
    {
        NONE = 0,
        LINEBEFORE_EXPANDED = 0x0002,
        LINEBEFORE_CONTRACTED = 0x0004,
        LINEAFTER_EXPANDED = 0x0008,
        LINEAFTER_CONTRACTED = 0x0010,
        LEVELNUMBERS = 0x0040,
        LINESTATE = 0x0080
    }
    /// <summary>Is the range start..end considered a word? (Scintilla feature SC_IDLESTYLING_)</summary>
    public enum IdleStyling
    {
        NONE = 0,
        TOVISIBLE = 1,
        AFTERVISIBLE = 2,
        ALL = 3
    }
    /// <summary>Retrieve the limits to idle styling. (Scintilla feature SC_WRAP_)</summary>
    public enum Wrap
    {
        NONE = 0,
        WORD = 1,
        CHAR = 2,
        WHITESPACE = 3
    }
    /// <summary>Retrieve whether text is word wrapped. (Scintilla feature SC_WRAPVISUALFLAG_)</summary>
    public enum WrapVisualFlag
    {
        NONE = 0x0000,
        END = 0x0001,
        START = 0x0002,
        MARGIN = 0x0004
    }
    /// <summary>Retrive the display mode of visual flags for wrapped lines. (Scintilla feature SC_WRAPVISUALFLAGLOC_)</summary>
    public enum WrapVisualLocation
    {
        DEFAULT = 0x0000,
        END_BY_TEXT = 0x0001,
        START_BY_TEXT = 0x0002
    }
    /// <summary>Retrive the start indent for wrapped lines. (Scintilla feature SC_WRAPINDENT_)</summary>
    public enum WrapIndentMode
    {
        FIXED = 0,
        SAME = 1,
        INDENT = 2,
        DEEPINDENT = 3
    }
    /// <summary>Retrieve how wrapped sublines are placed. Default is fixed. (Scintilla feature SC_CACHE_)</summary>
    public enum LineCache
    {
        NONE = 0,
        CARET = 1,
        PAGE = 2,
        DOCUMENT = 3
    }
    /// <summary>Append a string to the end of the document without changing the selection. (Scintilla feature SC_PHASES_)</summary>
    public enum PhasesDraw
    {
        [Obsolete("Use SC_PHASES_TWO or SC_PHASES_MULTIPLE instead: https://www.scintilla.org/ScintillaDoc.html#SCI_GETTWOPHASEDRAW")]
        ONE = 0,
        TWO = 1,
        MULTIPLE = 2
    }
    /// <summary>Control font anti-aliasing. (Scintilla feature SC_EFF_)</summary>
    public enum FontQuality
    {
        QUALITY_MASK = 0xF,
        QUALITY_DEFAULT = 0,
        QUALITY_NON_ANTIALIASED = 1,
        QUALITY_ANTIALIASED = 2,
        QUALITY_LCD_OPTIMIZED = 3
    }
    /// <summary>Scroll so that a display line is at the top of the display. (Scintilla feature SC_MULTIPASTE_)</summary>
    public enum MultiPaste
    {
        ONCE = 0,
        EACH = 1
    }
    /// <summary>Set the other colour used as a chequerboard pattern in the fold margin (Scintilla feature SC_ACCESSIBILITY_)</summary>
    public enum Accessibility
    {
        DISABLED = 0,
        ENABLED = 1
    }
    /// <summary>Set which document modification events are sent to the container. (Scintilla feature EDGE_)</summary>
    public enum EdgeVisualStyle
    {
        NONE = 0,
        LINE = 1,
        BACKGROUND = 2,
        MULTILINE = 3
    }
    /// <summary>Retrieves the number of lines completely visible. (Scintilla feature SC_POPUP_)</summary>
    public enum PopUp
    {
        NEVER = 0,
        ALL = 1,
        TEXT = 2
    }
    /// <summary>Retrieve the zoom level. (Scintilla feature SC_DOCUMENTOPTION_)</summary>
    public enum DocumentOption
    {
        DEFAULT = 0,
        STYLES_NONE = 0x1,
        TEXT_LARGE = 0x100
    }
    /// <summary>Get internal focus flag. (Scintilla feature SC_STATUS_)</summary>
    public enum Status
    {
        OK = 0,
        FAILURE = 1,
        BADALLOC = 2,
        WARN_START = 1000,
        WARN_REGEX = 1001
    }
    /// <summary>Get whether mouse wheel can be active outside the window. (Scintilla feature SC_CURSOR)</summary>
    public enum CursorShape
    {
        NORMAL = -1,
        ARROW = 2,
        WAIT = 4,
        REVERSEARROW = 7
    }
    /// <summary>Constants for use with SetVisiblePolicy, similar to SetCaretPolicy. (Scintilla feature VISIBLE_)</summary>
    public enum VisiblePolicy
    {
        SLOP = 0x01,
        STRICT = 0x04
    }
    /// <summary>Set the focus to this Scintilla widget. (Scintilla feature CARET_)</summary>
    [Flags]
    public enum CaretPolicy
    {
        SLOP = 0x01,
        STRICT = 0x04,
        JUMPS = 0x10,
        EVEN = 0x08
    }
    /// <summary>Copy argument text to the clipboard. (Scintilla feature SC_SEL_)</summary>
    public enum SelectionMode
    {
        STREAM = 0,
        RECTANGLE = 1,
        LINES = 2,
        THIN = 3
    }
    /// <summary>
    /// Get currently selected item text in the auto-completion list
    /// Returns the length of the item text
    /// Result is NUL-terminated.
    /// (Scintilla feature SC_CASEINSENSITIVEBEHAVIOUR_)
    /// </summary>
    public enum CaseInsensitiveBehaviour
    {
        RESPECTCASE = 0,
        IGNORECASE = 1
    }
    /// <summary>Get auto-completion case insensitive behaviour. (Scintilla feature SC_MULTIAUTOC_)</summary>
    public enum MultiAutoComplete
    {
        ONCE = 0,
        EACH = 1
    }
    /// <summary>Retrieve the effect of autocompleting when there are multiple selections. (Scintilla feature SC_ORDER_)</summary>
    public enum Ordering
    {
        PRESORTED = 0,
        PERFORMSORT = 1,
        CUSTOM = 2
    }
    /// <summary>Stop the caret preferred x position changing when the user types. (Scintilla feature SC_CARETSTICKY_)</summary>
    public enum CaretSticky
    {
        OFF = 0,
        ON = 1,
        WHITESPACE = 2
    }
    /// <summary>Duplicate the selection. If selection empty duplicate the line containing the caret. (Scintilla feature SC_ALPHA_)</summary>
    public enum Alpha
    {
        TRANSPARENT = 0,
        OPAQUE = 255,
        [Obsolete("Use SCI_SETSELECTIONLAYER instead: https://www.scintilla.org/ScintillaDoc.html#SCI_SETSELECTIONLAYER")]
        NOALPHA = 256
    }
    /// <summary>Get the background alpha of the caret line. (Scintilla feature CARETSTYLE_)</summary>
    [Flags]
    public enum CaretStyle
    {
        INVISIBLE = 0,
        LINE = 1,
        BLOCK = 2,
        OVERSTRIKE_BAR = 0,
        OVERSTRIKE_BLOCK = 0x10,
        INS_MASK = 0xF,
        CURSES = 0x20,
        BLOCK_AFTER = 0x100
    }
    /// <summary>Get the start of the range of style numbers used for margin text (Scintilla feature SC_MARGINOPTION_)</summary>
    public enum MarginOption
    {
        NONE = 0,
        SUBLINESELECT = 1
    }
    /// <summary>Clear the annotations from all lines (Scintilla feature ANNOTATION_)</summary>
    public enum AnnotationVisible
    {
        HIDDEN = 0,
        STANDARD = 1,
        BOXED = 2,
        INDENTED = 3
    }
    /// <summary>Allocate some extended (>255) style numbers and return the start of the range (Scintilla feature UNDO_)</summary>
    public enum UndoFlags
    {
        NONE = 0,
        MAY_COALESCE = 1
    }
    /// <summary>Return the virtual space of the anchor of the rectangular selection. (Scintilla feature SCVS_)</summary>
    public enum VirtualSpace
    {
        NONE = 0,
        RECTANGULARSELECTION = 1,
        USERACCESSIBLE = 2,
        NOWRAPLINESTART = 4
    }
    /// <summary>Scroll to end of document. (Scintilla feature SC_TECHNOLOGY_)</summary>
    public enum Technology
    {
        DEFAULT = 0,
        DIRECTWRITE = 1,
        DIRECTWRITERETAIN = 2,
        DIRECTWRITEDC = 3
    }
    /// <summary>
    /// Line end types which may be used in addition to LF, CR, and CRLF
    /// SC_LINE_END_TYPE_UNICODE includes U+2028 Line Separator,
    /// U+2029 Paragraph Separator, and U+0085 Next Line
    /// (Scintilla feature SC_LINE_END_TYPE_)
    /// </summary>
    public enum LineEndType
    {
        DEFAULT = 0,
        UNICODE = 1
    }
    /// <summary>
    /// Retrieve a '\n' separated list of properties understood by the current lexer.
    /// Result is NUL-terminated.
    /// (Scintilla feature SC_TYPE_)
    /// </summary>
    public enum TypeProperty
    {
        BOOLEAN = 0,
        INTEGER = 1,
        STRING = 2
    }
    /// <summary>
    /// The <see cref="ScNotification.ModificationType"/> field of a <see cref="ScNotification"/>.
    /// These are defined as a bit mask to make it easy to specify which notifications are wanted.
    /// One bit is set from each of SC_MOD_* and SC_PERFORMED_*.
    /// (Scintilla feature SC_MOD_ SC_PERFORMED_ SC_MULTISTEPUNDOREDO SC_LASTSTEPINUNDOREDO SC_MULTILINEUNDOREDO SC_STARTACTION SC_MODEVENTMASKALL)
    /// </summary>
    [Flags]
    public enum ModificationFlags
    {
        /// <summary>
        /// Base value with no fields valid. Will not occur but is useful in tests.
        /// </summary>
        SC_MOD_NONE = 0x00,
        /// <summary>
        /// Text has been inserted into the document.
        /// <para>
        /// Fields: <see cref="ScNotification.Position"/>, <see cref="ScNotification.Length"/>, <see cref="ScNotification.TextPointer"/>, <see cref="ScNotification.LinesAdded"/>
        /// </para>
        /// </summary>
        SC_MOD_INSERTTEXT = 0x01,
        /// <summary>
        /// Text has been removed from the document.
        /// <para>
        /// Fields: <see cref="ScNotification.Position"/>, <see cref="ScNotification.Length"/>, <see cref="ScNotification.TextPointer"/>, <see cref="ScNotification.LinesAdded"/>
        /// </para>
        /// </summary>
        SC_MOD_DELETETEXT = 0x02,
        /// <summary>
        /// A style change has occurred.
        /// <para>
        /// Fields: <see cref="ScNotification.Position"/>, <see cref="ScNotification.Length"/>
        /// </para>
        /// </summary>
        SC_MOD_CHANGESTYLE = 0x04,
        /// <summary>
        /// A folding change has occurred.
        /// <para>
        /// Fields: <see cref="ScNotification.LineNumber"/>, <see cref="ScNotification.FoldLevelNow"/>, <see cref="ScNotification.FoldLevelPrev"/>
        /// </para>
        /// </summary>
        SC_MOD_CHANGEFOLD = 0x08,
        /// <summary> Information: the operation was done by the user. </summary>
        SC_PERFORMED_USER = 0x10,
        /// <summary> Information: this was the result of an Undo. </summary>
        SC_PERFORMED_UNDO = 0x20,
        /// <summary> Information: this was the result of a Redo. </summary>
        SC_PERFORMED_REDO = 0x40,
        /// <summary> This is part of a multi-step Undo or Redo transaction. </summary>
        SC_MULTISTEPUNDOREDO = 0x80,
        /// <summary> This is the final step in an Undo or Redo transaction. </summary>
        SC_LASTSTEPINUNDOREDO = 0x100,
        /// <summary>
        /// One or more markers has changed in a line.
        /// <para>Fields: <see cref="ScNotification.LineNumber"/></para>
        /// </summary>
        SC_MOD_CHANGEMARKER = 0x200,
        /// <summary> Text is about to be inserted into the document.
        /// <para>
        /// Fields: <see cref="ScNotification.Position"/>.
        /// If performed by user (i.e., <c>ModificationType &amp; SC_PERFORMED_USER == SC_PERFORMED_USER</c>)
        /// then also <see cref="ScNotification.TextPointer"/>  in bytes and  <see cref="ScNotification.Length"/>  in bytes
        /// </para>
        /// </summary>
        SC_MOD_BEFOREINSERT = 0x400,
        /// <summary>
        /// Text is about to be deleted from the document.
        /// <para>
        /// Fields: <see cref="ScNotification.Position"/>, <see cref="ScNotification.Length"/>
        /// </para>
        /// </summary>
        SC_MOD_BEFOREDELETE = 0x800,
        /// <summary>
        /// An indicator has been added or removed from a range of text.
        /// <para>
        /// Fields: <see cref="ScNotification.Position"/>, <see cref="ScNotification.Length"/>
        /// </para>
        /// </summary>
        SC_MOD_CHANGEINDICATOR = 0x4000,
        /// <summary>
        /// A line state has changed because <see cref="SciMsg.SCI_SETLINESTATE"/> was called.
        /// <para>Fields: <see cref="ScNotification.LineNumber"/></para>
        /// </summary>
        SC_MOD_CHANGELINESTATE = 0x8000,
        /// <summary>
        /// The explicit tab stops on a line have changed because <see cref="SciMsg.SCI_CLEARTABSTOPS"/> or <see cref="SciMsg.SCI_ADDTABSTOP"/> was called.
        /// <para>Fields: <see cref="ScNotification.LineNumber"/></para>
        /// </summary>
        SC_MOD_CHANGETABSTOPS = 0x200000,
        /// <summary>
        /// The internal state of a lexer has changed over a range.
        /// <para>
        /// Fields: <see cref="ScNotification.Position"/>, <see cref="ScNotification.Length"/>
        /// </para>
        /// </summary>
        SC_MOD_LEXERSTATE = 0x80000,
        /// <summary>
        /// A text margin has changed.
        /// <para>Fields: <see cref="ScNotification.LineNumber"/></para>
        /// </summary>
        SC_MOD_CHANGEMARGIN = 0x10000,
        /// <summary>
        /// An annotation has changed.
        /// <para>Fields: <see cref="ScNotification.LineNumber"/></para>
        /// </summary>
        SC_MOD_CHANGEANNOTATION = 0x20000,
        /// <summary>
        /// An EOL annotation has changed.
        /// <para>Fields: <see cref="ScNotification.LineNumber"/></para>
        /// </summary>
        SC_MOD_CHANGEEOLANNOTATION = 0x400000,
        /// <summary>
        /// Text is about to be inserted. The handler may change the text being inserted by calling <see cref="SciMsg.SCI_CHANGEINSERTION"/>.
        /// No other modifications may be made in this handler.
        /// <para>
        /// Fields: <see cref="ScNotification.Position"/>, <see cref="ScNotification.Length"/>, <see cref="ScNotification.TextPointer"/>
        /// </para>
        /// </summary>
        SC_MOD_INSERTCHECK = 0x100000,
        /// <summary>
        /// This is part of an Undo or Redo with multi-line changes.
        /// </summary>
        SC_MULTILINEUNDOREDO = 0x1000,
        /// <summary>
        /// This is set on a SC_PERFORMED_USER action when it is the first or only step in an undo transaction.
        /// This can be used to integrate the Scintilla undo stack with an undo stack in the container application by adding a Scintilla
        /// action to the container's stack for the currently opened container transaction or to open a new container transaction if there
        /// is no open container transaction.
        /// </summary>
        SC_STARTACTION = 0x2000,
        /// <summary> This is set on for actions that the container stored into the undo stack with <see cref="SciMsg.SCI_ADDUNDOACTION"/>.
        /// <para>Fields: <see cref="ScNotification.Token"/></para>
        /// </summary>
        SC_MOD_CONTAINER = 0x40000,
        /// <summary>
        /// This is a mask for all valid flags. This is the default mask state set by <see cref="SciMsg.SCI_SETMODEVENTMASK"/>.
        /// </summary>
        SC_MODEVENTMASKALL = 0x7FFFFF,
        /// <summary>
        /// Used with <see cref="NotepadPPGateway.DefaultModificationFlagsChanged"/> to determine if the default <see cref="ModificationFlags"/>
        /// have been changed by another plugin.
        /// </summary>
        NPP_DEFAULT_SC_MOD_MASK =
            SC_MOD_DELETETEXT |
            SC_MOD_INSERTTEXT |
            SC_PERFORMED_UNDO |
            SC_PERFORMED_REDO |
            SC_MOD_CHANGEINDICATOR,
    }
    /// <summary>
    /// Notifications
    /// Type of modification and the action which caused the modification.
    /// These are defined as a bit mask to make it easy to specify which notifications are wanted.
    /// One bit is set from each of SC_MOD_* and SC_PERFORMED_*.
    /// (Scintilla feature SC_UPDATE_)
    /// </summary>
    [Flags]
    public enum Update
    {
        CONTENT = 0x1,
        SELECTION = 0x2,
        V_SCROLL = 0x4,
        H_SCROLL = 0x8
    }
    /// <summary>
    /// Symbolic key codes and modifier flags.
    /// ASCII and other printable characters below 256.
    /// Extended keys above 300.
    /// (Scintilla feature SCMOD_)
    /// </summary>
    public enum KeyMod
    {
        NORM = 0,
        SHIFT = 1,
        CTRL = 2,
        ALT = 4,
        SUPER = 8,
        META = 16
    }
    /// <summary>
    /// Symbolic key codes and modifier flags.
    /// ASCII and other printable characters below 256.
    /// Extended keys above 300.
    /// (Scintilla feature SC_AC_)
    /// </summary>
    public enum CompletionMethods
    {
        FILLUP = 1,
        DOUBLECLICK = 2,
        TAB = 3,
        NEWLINE = 4,
        COMMAND = 5,
        /// <summary>
        /// There was only a single choice in the list and 'choose single' mode was active as set by <see cref="SciMsg.SCI_AUTOCSETCHOOSESINGLE"/>. ch is 0.
        /// </summary>
        /// <remarks>
        /// Added in <a href="https://sourceforge.net/p/scintilla/code/ci/2cd5a02a022b">5.3.2</a>
        /// </remarks>
        SINGLECHOICE = 6
    }
    /// <summary>characterSource for SCN_CHARADDED (Scintilla feature SC_CHARACTERSOURCE_)</summary>
    public enum CharacterSource
    {
        DIRECT_INPUT = 0,
        TENTATIVE_INPUT = 1,
        IME_RESULT = 2
    }
    /// <remarks>
    /// Notepad++ adopted Scintilla's external lexer API in v8.4 (released in April 2022).
    /// See <see href="https://community.notepad-plus-plus.org/post/76117"/>
    /// </remarks>
    [Obsolete("Removed in Scintilla v5.0: https://www.scintilla.org/Scintilla5Migration.html")]
    public enum Lexer
    {
        CONTAINER = 0,
        NULL = 1,
        PYTHON = 2,
        CPP = 3,
        HTML = 4,
        XML = 5,
        PERL = 6,
        SQL = 7,
        VB = 8,
        PROPERTIES = 9,
        ERRORLIST = 10,
        MAKEFILE = 11,
        BATCH = 12,
        XCODE = 13,
        LATEX = 14,
        LUA = 15,
        DIFF = 16,
        CONF = 17,
        PASCAL = 18,
        AVE = 19,
        ADA = 20,
        LISP = 21,
        RUBY = 22,
        EIFFEL = 23,
        EIFFELKW = 24,
        TCL = 25,
        NNCRONTAB = 26,
        BULLANT = 27,
        VBSCRIPT = 28,
        BAAN = 31,
        MATLAB = 32,
        SCRIPTOL = 33,
        ASM = 34,
        CPPNOCASE = 35,
        FORTRAN = 36,
        F77 = 37,
        CSS = 38,
        POV = 39,
        LOUT = 40,
        ESCRIPT = 41,
        PS = 42,
        NSIS = 43,
        MMIXAL = 44,
        CLW = 45,
        CLWNOCASE = 46,
        LOT = 47,
        YAML = 48,
        TEX = 49,
        METAPOST = 50,
        POWERBASIC = 51,
        FORTH = 52,
        ERLANG = 53,
        OCTAVE = 54,
        MSSQL = 55,
        VERILOG = 56,
        KIX = 57,
        GUI4CLI = 58,
        SPECMAN = 59,
        AU3 = 60,
        APDL = 61,
        BASH = 62,
        ASN1 = 63,
        VHDL = 64,
        CAML = 65,
        BLITZBASIC = 66,
        PUREBASIC = 67,
        HASKELL = 68,
        PHPSCRIPT = 69,
        TADS3 = 70,
        REBOL = 71,
        SMALLTALK = 72,
        FLAGSHIP = 73,
        CSOUND = 74,
        FREEBASIC = 75,
        INNOSETUP = 76,
        OPAL = 77,
        SPICE = 78,
        D = 79,
        CMAKE = 80,
        GAP = 81,
        PLM = 82,
        PROGRESS = 83,
        ABAQUS = 84,
        ASYMPTOTE = 85,
        R = 86,
        MAGIK = 87,
        POWERSHELL = 88,
        MYSQL = 89,
        PO = 90,
        TAL = 91,
        COBOL = 92,
        TACL = 93,
        SORCUS = 94,
        POWERPRO = 95,
        NIMROD = 96,
        SML = 97,
        MARKDOWN = 98,
        TXT2TAGS = 99,
        A68K = 100,
        MODULA = 101,
        COFFEESCRIPT = 102,
        TCMD = 103,
        AVS = 104,
        ECL = 105,
        OSCRIPT = 106,
        VISUALPROLOG = 107,
        LITERATEHASKELL = 108,
        STTXT = 109,
        KVIRC = 110,
        RUST = 111,
        DMAP = 112,
        AS = 113,
        DMIS = 114,
        REGISTRY = 115,
        BIBTEX = 116,
        SREC = 117,
        IHEX = 118,
        TEHEX = 119,
        JSON = 120,
        EDIFACT = 121,
        INDENT = 122,
        MAXIMA = 123,
        STATA = 124,
        SAS = 125,
        NIM = 126,
        CIL = 127,
        X12 = 128,
        DATAFLEX = 129,
        AUTOMATIC = 1000
    }
    /// <summary>GTK Specific to work around focus and accelerator problems: (Scintilla feature SC_BIDIRECTIONAL_)</summary>
    public enum Bidirectional
    {
        DISABLED = 0,
        L2R = 1,
        R2L = 2
    }
    /// <summary>Set bidirectional text display state. (Scintilla feature SC_LINECHARACTERINDEX_)</summary>
    public enum LineCharacterIndexType
    {
        NONE = 0,
        UTF32 = 1,
        UTF16 = 2
    }
    /* --Autogenerated -- end of section automatically generated from Scintilla.iface */

}
