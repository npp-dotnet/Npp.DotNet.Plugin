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
    /// Compatible with Windows' <see cref="Win32.TagNMHDR"/>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ScNotificationHeader
    {
        /// <summary>
        /// Environment specific window handle or pointer
        /// </summary>
        public IntPtr HwndFrom;

        /// <summary>
        /// Control ID of the window issuing the notification
        /// </summary>
        public UIntPtr IdFrom;

        /// <summary>
        /// A <c>SCN_*</c> notification code
        /// </summary>
        public uint Code;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ScNotification
    {
        public ScNotificationHeader Header;
        internal IntPtr Pos;            /* SCN_STYLENEEDED, SCN_DOUBLECLICK, SCN_MODIFIED, SCN_MARGINCLICK, SCN_NEEDSHOWN, SCN_DWELLSTART, SCN_DWELLEND, SCN_CALLTIPCLICK, SCN_HOTSPOTCLICK, SCN_HOTSPOTDOUBLECLICK, SCN_HOTSPOTRELEASECLICK, SCN_INDICATORCLICK, SCN_INDICATORRELEASE, SCN_USERLISTSELECTION, SCN_AUTOCSELECTION */
        public int Ch;                       /* SCN_CHARADDED, SCN_KEY, SCN_AUTOCCOMPLETE, SCN_AUTOCSELECTION, SCN_USERLISTSELECTION */
        public int Modifiers;              /* SCN_KEY, SCN_DOUBLECLICK, SCN_HOTSPOTCLICK, SCN_HOTSPOTDOUBLECLICK, SCN_HOTSPOTRELEASECLICK, SCN_INDICATORCLICK, SCN_INDICATORRELEASE */
        public int ModificationType;        /* SCN_MODIFIED - modification types are name "SC_MOD_*" */
        public IntPtr TextPointer;          /* SCN_MODIFIED, SCN_USERLISTSELECTION, SCN_AUTOCSELECTION, SCN_URIDROPPED */
        public IntPtr Length;               /* SCN_MODIFIED */
        public IntPtr LinesAdded;           /* SCN_MODIFIED */
        public int Message;                 /* SCN_MACRORECORD */
        public UIntPtr WParam;               /* SCN_MACRORECORD */
        public IntPtr LParam;               /* SCN_MACRORECORD */

        /// <summary>
        /// 0-based index
        /// </summary>
        public IntPtr LineNumber;        /* SCN_MODIFIED */
        public int FoldLevelNow;         /* SCN_MODIFIED */
        public int FoldLevelPrev;        /* SCN_MODIFIED */
        public int Margin;               /* SCN_MARGINCLICK */
        public int ListType;             /* SCN_USERLISTSELECTION */
        public int X;                    /* SCN_DWELLSTART, SCN_DWELLEND */
        public int Y;                    /* SCN_DWELLSTART, SCN_DWELLEND */
        public int Token;                /* SCN_MODIFIED with SC_MOD_CONTAINER */
        public IntPtr AnnotationLinesAdded; /* SC_MOD_CHANGEANNOTATION */
        public int Updated;              /* SCN_UPDATEUI */
        public int ListCompletionMethod; /* SCN_AUTOCSELECTION, SCN_AUTOCCOMPLETED, SCN_USERLISTSELECTION */
        public int CharacterSource;      /* SCN_CHARADDED */

        /// <summary>
        /// SCN_STYLENEEDED, SCN_DOUBLECLICK, SCN_MODIFIED, SCN_MARGINCLICK, SCN_NEEDSHOWN, SCN_DWELLSTART, SCN_DWELLEND, SCN_CALLTIPCLICK, SCN_HOTSPOTCLICK, SCN_HOTSPOTDOUBLECLICK, SCN_HOTSPOTRELEASECLICK, SCN_INDICATORCLICK, SCN_INDICATORRELEASE, SCN_USERLISTSELECTION, SCN_AUTOCSELECTION
        /// </summary>
        public Scintilla.Position Position { get { return new Scintilla.Position(Pos); } }

        /// <summary>
        /// Character of the notification - eg keydown
        /// SCN_CHARADDED, SCN_KEY, SCN_AUTOCCOMPLETE, SCN_AUTOCSELECTION, SCN_USERLISTSELECTION
        /// </summary>
        public char Character { get { return (char)Ch; } }
    }

    namespace Scintilla
    {
        /// <remarks>
        /// See <see href="https://www.scintilla.org/ScintillaDoc.html#colour"/>
        /// </remarks>
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

        /// <remarks>
        /// See <see href="https://www.scintilla.org/ScintillaDoc.html#TextRetrievalAndModification"/>
        /// </remarks>
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
        /// A visible or control character. The lower 16-bits are used to form a key definition.
        /// </summary>
        /// <remarks>
        /// See <see cref="KeyModifier(ScKeyCode, KeyMod)"/>
        /// </remarks>
        public enum ScKeyCode
        {
            DOWN = (int)SciMsg.SCK_DOWN,
            UP = (int)SciMsg.SCK_UP,
            LEFT = (int)SciMsg.SCK_LEFT,
            RIGHT = (int)SciMsg.SCK_RIGHT,
            HOME = (int)SciMsg.SCK_HOME,
            END = (int)SciMsg.SCK_END,
            /// <summary>Page Up</summary>
            PRIOR = (int)SciMsg.SCK_PRIOR,
            /// <summary>Page Down</summary>
            NEXT = (int)SciMsg.SCK_NEXT,
            DELETE = (int)SciMsg.SCK_DELETE,
            INSERT = (int)SciMsg.SCK_INSERT,
            ESCAPE = (int)SciMsg.SCK_ESCAPE,
            BACK = (int)SciMsg.SCK_BACK,
            TAB = (int)SciMsg.SCK_TAB,
            RETURN = (int)SciMsg.SCK_RETURN,
            ADD = (int)SciMsg.SCK_ADD,
            SUBTRACT = (int)SciMsg.SCK_SUBTRACT,
            DIVIDE = (int)SciMsg.SCK_DIVIDE,
            WIN = (int)SciMsg.SCK_WIN,
            RWIN = (int)SciMsg.SCK_RWIN,
            MENU = (int)SciMsg.SCK_MENU,
        }

        /// <summary>
        /// Class containing key and modifiers.
        /// </summary>
        /// <remarks>
        /// See <see href="https://www.scintilla.org/ScintillaDoc.html#keyDefinition"/>
        /// </remarks>
        public readonly struct KeyModifier
        {
            private readonly int _value;

            /// <param name="keyCode">
            /// A visible or control character or a key from the <see cref="ScKeyCode"/> enumeration.
            /// </param>
            /// <param name="modifier">
            /// A combination of zero or more of <see cref="KeyMod.ALT"/>, <see cref="KeyMod.CTRL"/>, <see cref="KeyMod.SHIFT"/>,
            /// <see cref="KeyMod.META"/>, and <see cref="KeyMod.SUPER"/>.
            /// On macOS, the Command key is mapped to <see cref="KeyMod.CTRL"/> and the Control key to <see cref="KeyMod.META"/>.
            /// <see cref="KeyMod.SUPER"/> is only available on GTK which is commonly the Windows key.
            /// If you are building a table, you might want to use <see cref="KeyMod.NORM"/>, which has the value 0, to mean no modifiers.
            /// </param>
            public KeyModifier(ScKeyCode keyCode, KeyMod modifier)
            {
                _value = (int)keyCode | ((int)modifier << 16);
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

        /// <summary>
        /// Each byte in a Scintilla document is associated with a byte of styling information.
        /// The combination of a character byte and a style byte is called a cell.
        /// </summary>
        /// <remarks>
        /// See <see href="https://www.scintilla.org/ScintillaDoc.html#TextRetrievalAndModification"/>
        /// </remarks>
        public readonly struct Cells
        {
            internal readonly char[] CharactersAndStyles;

            public Cells(char[] charactersAndStyles)
            {
                this.CharactersAndStyles = charactersAndStyles;
            }
            public char[] Value { get { return CharactersAndStyles; } }
        }

        public sealed class TextRangeFull : IDisposable
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

        public sealed class TextToFindFull : IDisposable
        {
            Sci_TextToFindFull _sciTextToFind;
            IntPtr _ptrSciTextToFind;
            bool _disposed = false;

            public TextToFindFull(CharacterRangeFull chrRange, string searchText)
            {
                _sciTextToFind.ChRg = chrRange;
                _sciTextToFind.LpStrText = Marshal.StringToHGlobalAnsi(searchText);
            }

            public TextToFindFull(long cpmin, long cpmax, string searchText)
            {
                _sciTextToFind.ChRg.CpMin = new IntPtr(cpmin);
                _sciTextToFind.ChRg.CpMax = new IntPtr(cpmax);
                _sciTextToFind.LpStrText = Marshal.StringToHGlobalAnsi(searchText);
            }

            [StructLayout(LayoutKind.Sequential)]
            struct Sci_TextToFindFull
            {
                public CharacterRangeFull ChRg;
                public IntPtr LpStrText;
                public CharacterRangeFull ChRgText;
            }

            public IntPtr NativePointer { get { InitNativeStruct(); return _ptrSciTextToFind; } }
            public string LpStrText { set { FreeNativeString(); _sciTextToFind.LpStrText = Marshal.StringToHGlobalAnsi(value); } }
            public CharacterRangeFull ChRg { get { ReadNativeStruct(); return _sciTextToFind.ChRg; } set { _sciTextToFind.ChRg = value; InitNativeStruct(); } }
            public CharacterRangeFull ChRgText { get { ReadNativeStruct(); return _sciTextToFind.ChRgText; } }

            void InitNativeStruct()
            {
                if (_ptrSciTextToFind == IntPtr.Zero)
                    _ptrSciTextToFind = Marshal.AllocHGlobal(Marshal.SizeOf(_sciTextToFind));
                Marshal.StructureToPtr(_sciTextToFind, _ptrSciTextToFind, false);
            }

            unsafe void ReadNativeStruct()
            {
                if (_ptrSciTextToFind != IntPtr.Zero)
                    _sciTextToFind = *(Sci_TextToFindFull*)_ptrSciTextToFind;
            }

            void FreeNativeString()
            {
                if (_sciTextToFind.LpStrText != IntPtr.Zero) Marshal.FreeHGlobal(_sciTextToFind.LpStrText);
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    FreeNativeString();
                    if (_ptrSciTextToFind != IntPtr.Zero) Marshal.FreeHGlobal(_ptrSciTextToFind);
                    GC.SuppressFinalize(this);
                    _disposed = true;
                }
            }

            ~TextToFindFull()
            {
                Dispose();
            }
        }



        /* ++Autogenerated -- start of section automatically generated from Scintilla.iface */
        /// <summary>Make white space characters invisible, always visible or visible outside indentation. (Scintilla feature SCWS_)</summary>
        /// <remarks>See <see cref="IScintillaGateway.SetViewWS"/></remarks>
        public enum WhiteSpace
        {
            INVISIBLE = 0,
            VISIBLEALWAYS = 1,
            VISIBLEAFTERINDENT = 2,
            VISIBLEONLYININDENT = 3
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.SetTabDrawMode"/>. (Scintilla feature SCTD_)</summary>
        public enum TabDrawMode
        {
            LONGARROW = 0,
            STRIKEOUT = 1
        }
        /// <summary>Return value of <see cref="IScintillaGateway.GetEOLMode"/>. (Scintilla feature SC_EOL_)</summary>
        public enum EndOfLine
        {
            CRLF = 0,
            CR = 1,
            LF = 2
        }
        /// <summary>Choose to display the the IME in a window or inline. (Scintilla feature SC_IME_)</summary>
        /// <remarks>See <see cref="IScintillaGateway.SetIMEInteraction"/></remarks>
        public enum IMEInteraction
        {
            WINDOWED = 0,
            INLINE = 1
        }
        /// <summary>Symbols used for a particular marker number. (Scintilla feature SC_MARK_)</summary>
        /// <remarks>See <see cref="IScintillaGateway.MarkerDefine"/></remarks>
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
            /// <summary>Invisible mark that only sets the line background colour.</summary>
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
        /// <summary>Markers used for outlining and change history columns. (Scintilla feature SC_MARKNUM_)</summary>
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
        /// <summary>Set a margin to be either numeric or symbolic. (Scintilla feature SC_MARGIN_)</summary>
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
        /// Character set identifiers are used in <see cref="IScintillaGateway.StyleSetCharacterSet"/>.
        /// The values are the same as the <a href="https://learn.microsoft.com/windows/win32/intl/code-page-identifiers">Windows *CHARSET values</a>.
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
        /// <summary>Possible options for <see cref="IScintillaGateway.StyleSetCase"/>. (Scintilla feature SC_CASE_)</summary>
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
        /// <summary>
        /// Range of indicators reserved for change history.
        /// </summary>
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
        /// Indicator numbers. (Scintilla feature INDICATOR_)
        /// </summary>
        /// <remarks>
        /// <see cref="IndicatorNumbers.CONTAINER"/>, <see cref="IndicatorNumbers.IME"/>, <see cref="IndicatorNumbers.IME_MAX"/>,
        /// and <see cref="IndicatorNumbers.MAX"/> are indicator <em>numbers</em>, not indicator <em>styles</em>, so should not really be in
        /// the <see cref="IndicatorStyle"/> enumeration.
        /// </remarks>
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
        /// <summary>Possible options for <see cref="IScintillaGateway.IndicSetFlags"/>. (Scintilla feature SC_INDICFLAG_)</summary>
        public enum IndicFlag
        {
            NONE = 0,
            VALUEFORE = 1
        }
        /// <summary><see cref="IndentView.NONE"/> turns the feature off but the other 3 states determine how far the guides appear on empty lines. (Scintilla feature SC_IV_)</summary>
        /// <remarks>See <see cref="IScintillaGateway.SetIndentationGuides"/></remarks>
        public enum IndentView
        {
            NONE = 0,
            REAL = 1,
            LOOKFORWARD = 2,
            LOOKBOTH = 3
        }
        /// <summary>Print colour mode. (Scintilla feature SC_PRINT_)</summary>
        public enum PrintOption
        {
            /// <summary>Use same colours as screen, with the exception of line number margins, which use a white background.</summary>
            NORMAL = 0,
            /// <summary>Invert the light value of each style.</summary>
            INVERTLIGHT = 1,
            /// <summary>Force black text on white background.</summary>
            BLACKONWHITE = 2,
            /// <summary>Text stays coloured, but all background is forced to be white.</summary>
            COLOURONWHITE = 3,
            /// <summary>Only the default-background is forced to be white for printing.</summary>
            COLOURONWHITEDEFAULTBG = 4,
            /// <summary>Use same colours as screen, including line number margins.</summary>
            SCREENCOLOURS = 5
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.FindText(FindOption, TextToFindFull)"/>. (Scintilla feature SCFIND_)</summary>
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
        /// <summary>Possible options for <see cref="IScintillaGateway.SetFoldLevel"/>. (Scintilla feature SC_FOLDLEVEL)</summary>
        public enum FoldLevel
        {
            BASE = 0x400,
            WHITEFLAG = 0x1000,
            HEADERFLAG = 0x2000,
            NUMBERMASK = 0x0FFF
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.FoldDisplayTextSetStyle"/>. (Scintilla feature SC_FOLDDISPLAYTEXT_)</summary>
        public enum FoldDisplayTextStyle
        {
            HIDDEN = 0,
            STANDARD = 1,
            BOXED = 2
        }
        /// <summary>Expand or contract a fold header. (Scintilla feature SC_FOLDACTION_)</summary>
        /// <remarks>See <see cref="IScintillaGateway.FoldLine"/></remarks>
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
        /// <summary>Get automatic folding behaviours. (Scintilla feature SC_AUTOMATICFOLD_)</summary>
        /// <remarks>See <see cref="IScintillaGateway.GetAutomaticFold"/></remarks>
        [Flags]
        public enum AutomaticFold
        {
            NONE = 0,
            SHOW = 0x0001,
            CLICK = 0x0002,
            CHANGE = 0x0004
        }
        /// <summary>Set some style options for folding. (Scintilla feature SC_FOLDFLAG_)</summary>
        /// <remarks>See <see cref="IScintillaGateway.SetFoldFlags"/></remarks>
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
        /// <summary>Possible options for <see cref="IScintillaGateway.SetIdleStyling"/>. (Scintilla feature SC_IDLESTYLING_)</summary>
        public enum IdleStyling
        {
            NONE = 0,
            TOVISIBLE = 1,
            AFTERVISIBLE = 2,
            ALL = 3
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.SetWrapMode"/>. (Scintilla feature SC_WRAP_)</summary>
        public enum Wrap
        {
            NONE = 0,
            WORD = 1,
            CHAR = 2,
            WHITESPACE = 3
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.SetWrapVisualFlags"/>. (Scintilla feature SC_WRAPVISUALFLAG_)</summary>
        public enum WrapVisualFlag
        {
            NONE = 0x0000,
            END = 0x0001,
            START = 0x0002,
            MARGIN = 0x0004
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.SetWrapVisualFlagsLocation"/>. (Scintilla feature SC_WRAPVISUALFLAGLOC_)</summary>
        public enum WrapVisualLocation
        {
            DEFAULT = 0x0000,
            END_BY_TEXT = 0x0001,
            START_BY_TEXT = 0x0002
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.SetWrapIndentMode"/>. (Scintilla feature SC_WRAPINDENT_)</summary>
        public enum WrapIndentMode
        {
            FIXED = 0,
            SAME = 1,
            INDENT = 2,
            DEEPINDENT = 3
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.SetLayoutCache"/>. (Scintilla feature SC_CACHE_)</summary>
        public enum LineCache
        {
            NONE = 0,
            CARET = 1,
            PAGE = 2,
            DOCUMENT = 3
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.SetPhasesDraw"/>. (Scintilla feature SC_PHASES_)</summary>
        public enum PhasesDraw
        {
            [Obsolete("Use SC_PHASES_TWO or SC_PHASES_MULTIPLE instead: https://www.scintilla.org/ScintillaDoc.html#SCI_GETTWOPHASEDRAW")]
            ONE = 0,
            TWO = 1,
            MULTIPLE = 2
        }
        /// <summary>Control font anti-aliasing. (Scintilla feature SC_EFF_)</summary>
        /// <remarks>See <see cref="IScintillaGateway.SetFontQuality"/></remarks>
        public enum FontQuality
        {
            QUALITY_MASK = 0xF,
            QUALITY_DEFAULT = 0,
            QUALITY_NON_ANTIALIASED = 1,
            QUALITY_ANTIALIASED = 2,
            QUALITY_LCD_OPTIMIZED = 3
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.SetMultiPaste"/>. (Scintilla feature SC_MULTIPASTE_)</summary>
        public enum MultiPaste
        {
            ONCE = 0,
            EACH = 1
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.SetAccessibility"/>. (Scintilla feature SC_ACCESSIBILITY_)</summary>
        public enum Accessibility
        {
            DISABLED = 0,
            ENABLED = 1
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.SetEdgeMode"/>. (Scintilla feature EDGE_)</summary>
        public enum EdgeVisualStyle
        {
            NONE = 0,
            LINE = 1,
            BACKGROUND = 2,
            MULTILINE = 3
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.UsePopUp"/>. (Scintilla feature SC_POPUP_)</summary>
        public enum PopUp
        {
            NEVER = 0,
            ALL = 1,
            TEXT = 2
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.CreateDocument"/>. (Scintilla feature SC_DOCUMENTOPTION_)</summary>
        public enum DocumentOption
        {
            DEFAULT = 0,
            STYLES_NONE = 0x1,
            TEXT_LARGE = 0x100
        }
        /// <summary>Return value of <see cref="IScintillaGateway.GetStatus"/>. (Scintilla feature SC_STATUS_)</summary>
        public enum Status
        {
            OK = 0,
            FAILURE = 1,
            BADALLOC = 2,
            WARN_START = 1000,
            WARN_REGEX = 1001
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.SetCursor"/>. (Scintilla feature SC_CURSOR)</summary>
        public enum CursorShape
        {
            NORMAL = -1,
            ARROW = 2,
            WAIT = 4,
            REVERSEARROW = 7
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.SetVisiblePolicy"/>. (Scintilla feature VISIBLE_)</summary>
        public enum VisiblePolicy
        {
            SLOP = 0x01,
            STRICT = 0x04
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.SetXCaretPolicy"/> and <see cref="IScintillaGateway.SetYCaretPolicy"/>. (Scintilla feature CARET_)</summary>
        [Flags]
        public enum CaretPolicy
        {
            SLOP = 0x01,
            STRICT = 0x04,
            JUMPS = 0x10,
            EVEN = 0x08
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.SetSelectionMode"/>. (Scintilla feature SC_SEL_)</summary>
        public enum SelectionMode
        {
            STREAM = 0,
            RECTANGLE = 1,
            LINES = 2,
            THIN = 3
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.AutoCSetCaseInsensitiveBehaviour"/>. (Scintilla feature SC_CASEINSENSITIVEBEHAVIOUR_)</summary>
        public enum CaseInsensitiveBehaviour
        {
            RESPECTCASE = 0,
            IGNORECASE = 1
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.AutoCSetMulti"/>. (Scintilla feature SC_MULTIAUTOC_)</summary>
        public enum MultiAutoComplete
        {
            ONCE = 0,
            EACH = 1
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.AutoCSetOrder"/>. (Scintilla feature SC_ORDER_)</summary>
        public enum Ordering
        {
            PRESORTED = 0,
            PERFORMSORT = 1,
            CUSTOM = 2
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.SetCaretSticky"/>. (Scintilla feature SC_CARETSTICKY_)</summary>
        public enum CaretSticky
        {
            OFF = 0,
            ON = 1,
            WHITESPACE = 2
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.SetSelAlpha"/>. (Scintilla feature SC_ALPHA_)</summary>
        public enum Alpha
        {
            TRANSPARENT = 0,
            OPAQUE = 255,
            [Obsolete("Use SCI_SETSELECTIONLAYER instead: https://www.scintilla.org/ScintillaDoc.html#SCI_SETSELECTIONLAYER")]
            NOALPHA = 256
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.SetCaretStyle"/>. (Scintilla feature CARETSTYLE_)</summary>
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
        /// <summary>Possible options for <see cref="IScintillaGateway.SetMarginOptions"/>. (Scintilla feature SC_MARGINOPTION_)</summary>
        public enum MarginOption
        {
            NONE = 0,
            SUBLINESELECT = 1
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.AnnotationSetVisible"/>. (Scintilla feature ANNOTATION_)</summary>
        public enum AnnotationVisible
        {
            HIDDEN = 0,
            STANDARD = 1,
            BOXED = 2,
            INDENTED = 3
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.AddUndoAction"/>. (Scintilla feature UNDO_)</summary>
        public enum UndoFlags
        {
            NONE = 0,
            MAY_COALESCE = 1
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.SetVirtualSpaceOptions"/>. (Scintilla feature SCVS_)</summary>
        public enum VirtualSpace
        {
            NONE = 0,
            RECTANGULARSELECTION = 1,
            USERACCESSIBLE = 2,
            NOWRAPLINESTART = 4
        }
        /// <summary>Possible options for <see cref="IScintillaGateway.SetTechnology"/>. (Scintilla feature SC_TECHNOLOGY_)</summary>
        public enum Technology
        {
            /// <summary>Use older GDI API which is compatible with all versions of Windows including Windows Vista and XP</summary>
            DEFAULT = 0,
            /// <summary>Use the Direct2D and DirectWrite APIs for higher quality anti-aliased drawing</summary>
            DIRECTWRITE = 1,
            /// <summary>Request that the frame is retained after being presented which may prevent drawing failures on some cards and drivers</summary>
            DIRECTWRITERETAIN = 2,
            /// <summary>Use DirectWrite to draw into a GDI DC. This mode may work for remote access sessions</summary>
            DIRECTWRITEDC = 3,
            /// <summary>Use DirectWrite in a lower level way that manages graphics state more explicitly</summary>
            DIRECTWRITE_1 = 4
        }
        /// <summary>
        /// Line end types which may be used in addition to LF, CR, and CRLF.
        /// (Scintilla feature SC_LINE_END_TYPE_)
        /// </summary>
        /// <remarks>
        /// <see cref="LineEndType.UNICODE"/> includes U+2028 Line Separator,
        /// U+2029 Paragraph Separator, and U+0085 Next Line
        /// </remarks>
        public enum LineEndType
        {
            DEFAULT = 0,
            UNICODE = 1
        }
        /// <summary>Return value of <see cref="IScintillaGateway.PropertyType"/>. (Scintilla feature SC_TYPE_)</summary>
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
        /// (Scintilla feature SC_MOD_)
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
            /// <remarks>
            /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/d888fb5f1263f5ea036c610b6980e5c4381ce7eb">8.7.7</a>
            /// </remarks>
            NPP_DEFAULT_SC_MOD_MASK =
                SC_MOD_DELETETEXT |
                SC_MOD_INSERTTEXT |
                SC_PERFORMED_UNDO |
                SC_PERFORMED_REDO |
                SC_MOD_CHANGEINDICATOR,
        }
        /// <summary>
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
        [Flags]
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
        /// The value passed in <see cref="ScNotification.ListCompletionMethod"/> indicating the way in which the completion occurred.
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
        /// <summary>
        /// The value passed in <see cref="ScNotification.CharacterSource"/> when <see cref="SciMsg.SCN_CHARADDED"/> is emitted.
        /// </summary>
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

#if !SCI_DISABLE_PROVISIONAL
        /// <summary>Possible options for <see cref="IScintillaGateway.SetBidirectional"/>. (Scintilla feature SC_BIDIRECTIONAL_)</summary>
        public enum Bidirectional
        {
            DISABLED = 0,
            L2R = 1,
            R2L = 2
        }
#endif

        /// <summary>Return value of <see cref="IScintillaGateway.GetLineCharacterIndex"/>. (Scintilla feature SC_LINECHARACTERINDEX_)</summary>
        public enum LineCharacterIndexType
        {
            NONE = 0,
            UTF32 = 1,
            UTF16 = 2
        }
        /* --Autogenerated -- end of section automatically generated from Scintilla.iface */
    }
}
