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
        public Position Position { get { return new Position(Pos); } }

        /// <summary>
        /// Character of the notification - eg keydown
        /// SCN_CHARADDED, SCN_KEY, SCN_AUTOCCOMPLETE, SCN_AUTOCSELECTION, SCN_USERLISTSELECTION
        /// </summary>
        public char Character { get { return (char)Ch; } }
    }

    public class TextToFindFull : IDisposable
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

}
