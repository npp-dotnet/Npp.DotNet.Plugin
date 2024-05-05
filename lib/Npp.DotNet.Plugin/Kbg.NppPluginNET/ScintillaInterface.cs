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
    /// Compatible with Windows NMHDR.
    /// hwndFrom is really an environment specific window handle or pointer
    /// but most clients of Scintilla.h do not have this type visible.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ScNotificationHeader
    {
        /// <summary>
        /// environment specific window handle/pointer
        /// </summary>
        public IntPtr HwndFrom;

        /// <summary>
        /// CtrlID of the window issuing the notification
        /// </summary>
        public UIntPtr IdFrom;

        /// <summary>
        /// The SCN_* notification Code
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
        public Position Position { get { return new Position(Pos); } }

        /// <summary>
        /// Character of the notification - eg keydown
        /// SCN_CHARADDED, SCN_KEY, SCN_AUTOCCOMPLETE, SCN_AUTOCSELECTION, SCN_USERLISTSELECTION
        /// </summary>
        public char Character { get { return (char)Ch; } }
    }

    public class TextToFind : IDisposable
    {
        Sci_TextToFind _sciTextToFind;
        IntPtr _ptrSciTextToFind;
        bool _disposed = false;

        /// <summary>
        /// text to find
        /// </summary>
        /// <param name="chrRange">range to search</param>
        /// <param name="searchText">the search pattern</param>
        public TextToFind(CharacterRange chrRange, string searchText)
        {
            _sciTextToFind.chrg = chrRange;
            _sciTextToFind.lpstrText = Marshal.StringToHGlobalAnsi(searchText);
        }

        /// <summary>
        /// text to find
        /// </summary>
        /// <param name="cpmin">range to search</param>
        /// <param name="cpmax">range to search</param>
        /// <param name="searchText">the search pattern</param>
        public TextToFind(int cpmin, int cpmax, string searchText)
        {
            _sciTextToFind.chrg.cpMin = new IntPtr(cpmin);
            _sciTextToFind.chrg.cpMax = new IntPtr(cpmax);
            _sciTextToFind.lpstrText = Marshal.StringToHGlobalAnsi(searchText);
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Sci_TextToFind
        {
            public CharacterRange chrg;
            public IntPtr lpstrText;
            public CharacterRange chrgText;
        }

        public IntPtr NativePointer { get { _initNativeStruct(); return _ptrSciTextToFind; } }
        public string lpstrText { set { _freeNativeString(); _sciTextToFind.lpstrText = Marshal.StringToHGlobalAnsi(value); } }
        public CharacterRange chrg { get { _readNativeStruct(); return _sciTextToFind.chrg; } set { _sciTextToFind.chrg = value; _initNativeStruct(); } }
        public CharacterRange chrgText { get { _readNativeStruct(); return _sciTextToFind.chrgText; } }

        void _initNativeStruct()
        {
            if (_ptrSciTextToFind == IntPtr.Zero)
                _ptrSciTextToFind = Marshal.AllocHGlobal(Marshal.SizeOf(_sciTextToFind));
            Marshal.StructureToPtr(_sciTextToFind, _ptrSciTextToFind, false);
        }

        unsafe void _readNativeStruct()
        {
            if (_ptrSciTextToFind != IntPtr.Zero)
                _sciTextToFind = *(Sci_TextToFind*)_ptrSciTextToFind;
        }

        void _freeNativeString()
        {
            if (_sciTextToFind.lpstrText != IntPtr.Zero) Marshal.FreeHGlobal(_sciTextToFind.lpstrText);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _freeNativeString();
                if (_ptrSciTextToFind != IntPtr.Zero) Marshal.FreeHGlobal(_ptrSciTextToFind);
                GC.SuppressFinalize(this);
                _disposed = true;
            }
        }

        ~TextToFind()
        {
            Dispose();
        }
    }
}
