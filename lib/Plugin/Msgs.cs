/*
 * SPDX-FileCopyrightText: 2016 Kasper B. Graversen <https://github.com/kbilsted>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System;

namespace Npp.DotNet.Plugin
{
    static class Constants
    {
        public const int WM_USER = 0x400;
        public const int CONT_LEFT = 0;
        public const int CONT_RIGHT = 1;
        public const int CONT_TOP = 2;
        public const int CONT_BOTTOM = 3;
        public const int DOCKCONT_MAX = 4;
        public const int MENU_TITLE_LENGTH = 64;
        /// <summary>
        /// Defined in <c>PowerEditor/src/WinControls/StaticDialog/RunDlg/RunDlg.h</c>
        /// </summary>
        public const int CURRENTWORD_MAXLENGTH = 0x800;
    }

    public enum LangType
    {
        L_TEXT, L_PHP, L_C, L_CPP, L_CS, L_OBJC, L_JAVA, L_RC,
        L_HTML, L_XML, L_MAKEFILE, L_PASCAL, L_BATCH, L_INI, L_ASCII, L_USER,
        L_ASP, L_SQL, L_VB, L_JS_EMBEDDED, L_CSS, L_PERL, L_PYTHON, L_LUA,
        L_TEX, L_FORTRAN, L_BASH, L_FLASH, L_NSIS, L_TCL, L_LISP, L_SCHEME,
        L_ASM, L_DIFF, L_PROPS, L_PS, L_RUBY, L_SMALLTALK, L_VHDL, L_KIX, L_AU3,
        L_CAML, L_ADA, L_VERILOG, L_MATLAB, L_HASKELL, L_INNO, L_SEARCHRESULT,
        L_CMAKE, L_YAML, L_COBOL, L_GUI4CLI, L_D, L_POWERSHELL, L_R, L_JSP,
        L_COFFEESCRIPT, L_JSON, L_JAVASCRIPT, L_FORTRAN_77, L_BAANC, L_SREC,
        L_IHEX, L_TEHEX, L_SWIFT,
        L_ASN1, L_AVS, L_BLITZBASIC, L_PUREBASIC, L_FREEBASIC,
        L_CSOUND, L_ERLANG, L_ESCRIPT, L_FORTH, L_LATEX,
        L_MMIXAL, L_NIM, L_NNCRONTAB, L_OSCRIPT, L_REBOL,
        L_REGISTRY, L_RUST, L_SPICE, L_TXT2TAGS, L_VISUALPROLOG,
        L_TYPESCRIPT, L_JSON5, L_MSSQL, L_GDSCRIPT, L_HOLLYWOOD,
        L_GOLANG, L_RAKU, L_TOML, L_SAS, L_ERRORLIST,
        // Don't use L_JS_EMBEDDED, use L_JAVASCRIPT instead
        // The end of enumerated language type, so it should be always at the end
        L_EXTERNAL
    }

    public enum WinVer
    {
        WV_UNKNOWN, WV_WIN32S, WV_95, WV_98, WV_ME, WV_NT, WV_W2K,
        WV_XP, WV_S2003, WV_XPX64, WV_VISTA, WV_WIN7, WV_WIN8, WV_WIN81, WV_WIN10, WV_WIN11
    }

    /// <summary>
    /// Return value of <see cref="NppMsg.NPPM_GETTOOLBARICONSETCHOICE"/>.
    /// </summary>
    /// <remarks>
    /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/5406b82">8.8.2</a>
    /// </remarks>
    public enum ToolBarStatusType
    {
        /// <summary>
        /// Fluent UI (small)
        /// </summary>
        TB_SMALL,
        /// <summary>
        /// Fluent UI (large)
        /// </summary>
        TB_LARGE,
        /// <summary>
        /// Filled Fluent UI (small)
        /// </summary>
        TB_SMALL2,
        /// <summary>
        /// Filled Fluent UI (large)
        /// </summary>
        TB_LARGE2,
        /// <summary>
        /// Standard bitmap icons (small)
        /// </summary>
        TB_STANDARD
    }

    /// <summary>
    /// Used with <see cref="NppMsg.NPPM_GETEXTERNALLEXERAUTOINDENTMODE"/> and <see cref="NppMsg.NPPM_SETEXTERNALLEXERAUTOINDENTMODE"/>.
    /// </summary>
    /// <remarks>
    /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/9cbd03c">8.3.3</a>
    /// </remarks>
    public enum ExternalLexerAutoIndentMode { Standard, C_Like, Custom }

    /// <summary>
    /// Return value of <see cref="NppMsg.NPPM_GETCURRENTMACROSTATUS"/>.
    /// </summary>
    /// <remarks>
    /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/9cbd03c">8.3.3</a>
    /// </remarks>
    public enum MacroStatus { Idle, RecordInProgress, RecordingStopped, PlayingBack };

    public enum NppMsg : uint
    {
        NPPMSG = Constants.WM_USER + 1000,
        /// <summary>
        /// BOOL NPPM_GETCURRENTSCINTILLA(0, int* iScintillaView)<br/>
        /// Get the index of the currently active Scintilla view.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: <c>iScintillaView</c> - either <see cref="MAIN_VIEW"/> or <see cref="SUB_VIEW"/></para>
        /// <para>Returns TRUE.</para>
        /// </summary>
        NPPM_GETCURRENTSCINTILLA = NPPMSG + 4,
        /// <summary>
        /// BOOL NPPM_GETCURRENTLANGTYPE(0, int* langType)<br/>
        /// Get the <see cref="LangType"/> associated with the current document.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: <c>langType</c> - see <see cref="LangType"/> for all valid values</para>
        /// <para>Returns TRUE.</para>
        /// </summary>
        NPPM_GETCURRENTLANGTYPE = NPPMSG + 5,
        /// <summary>
        /// BOOL NPPM_SETCURRENTLANGTYPE(0, int langType)<br/>
        /// Set the <see cref="LangType"/> associated with the current document.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>langType</c> - see <see cref="LangType"/> for all valid values</para>
        /// <para>Returns TRUE.</para>
        /// </summary>
        NPPM_SETCURRENTLANGTYPE = NPPMSG + 6,
        /// <summary>
        /// int NPPM_GETNBOPENFILES(0, int iViewType)<br/>
        /// Get the number of files currently open in the given view.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>iViewType</c> - can be <see cref="PRIMARY_VIEW"/>, <see cref="SECOND_VIEW"/> or <see cref="ALL_OPEN_FILES"/></para>
        /// <para>Returns the number of opened files.</para>
        /// </summary>
        NPPM_GETNBOPENFILES = NPPMSG + 7,
        /// <summary>See <seealso cref="NPPM_GETNBOPENFILES"/></summary>
        ALL_OPEN_FILES = 0,
        /// <summary>See <seealso cref="NPPM_GETNBOPENFILES"/></summary>
        PRIMARY_VIEW = 1,
        /// <summary>See <seealso cref="NPPM_GETNBOPENFILES"/></summary>
        SECOND_VIEW = 2,
        /// <summary>
        /// BOOL NPPM_GETOPENFILENAMES_DEPRECATED(wchar_t** fileNames, int nbFileNames)<br/>
        /// Get the full path names of all files currently open in both views. The user must allocate a big enough <c>fileNames</c> array using <see cref="NPPM_GETNBOPENFILES"/>.
        /// <para>wParam (<see cref="UIntPtr"/>) [out]: <c>fileNames</c> - pre-allocated array of file paths</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>nbFileNames</c> - the number of file paths in <c>fileNames</c></para>
        /// <para>Returns the number of files copied to the <c>fileNames</c> array.</para>
        /// </summary>
        NPPM_GETOPENFILENAMES_DEPRECATED = NPPMSG + 8,
        [Obsolete("Replaced by NPPM_GETOPENFILENAMES_DEPRECATED", true)]
        NPPM_GETOPENFILENAMES = NPPM_GETOPENFILENAMES_DEPRECATED,
        /// <summary>
        /// HWND NPPM_MODELESSDIALOG(int action, HWND hDlg)<br/>
        /// Register (or unregister) the plugin dialog with the given handle.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>action</c> - either <see cref="MODELESSDIALOGADD"/> (to register) or <see cref="MODELESSDIALOGREMOVE"/> (to unregister)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>hDlg</c> - the handle of the dialog to register/unregister</para>
        /// </summary>
        /// <para>Returns <c>hDlg</c> (<see cref="IntPtr"/>) on success, <see cref="Win32.NULL"/> on failure.</para>
        /// <remarks>
        /// For each dialog created by your plugin, you should use this API to register it (and unregister it on destruction).
        /// If this message is ignored, dialog edit controls will not receive key stroke messages such as TAB, Ctrl-C or Ctrl-V key.
        /// </remarks>
        NPPM_MODELESSDIALOG = NPPMSG + 12,
        /// <summary>See <seealso cref="NPPM_MODELESSDIALOG"/></summary>
        MODELESSDIALOGADD = 0,
        /// <summary>See <seealso cref="NPPM_MODELESSDIALOG"/></summary>
        MODELESSDIALOGREMOVE = 1,
        /// <summary>
        /// int NPPM_GETNBSESSIONFILES (BOOL* pbIsValidXML, wchar_t* sessionFileName)<br/>
        /// Get the number of files listed in the session file with the given <c>sessionFileName</c>.
        /// <para>wParam (<see cref="UIntPtr"/>) [out]: <c>pbIsValidXML</c> - <em>Since
        /// <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/72c5175b33cb3a06d0ece2a7700295b0c9647140">8.6</a></em>:
        /// TRUE if <c>sessionFileName</c> is valid XML, otherwise FALSE. If <c>sessionFileName</c> is a <see cref="Win32.NULL"/> pointer, this parameter is ignored
        /// </para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>sessionFileName</c> - the XML session file's full path</para>
        /// <para>Returns the number of files listed in the XML session file.</para>
        /// </summary>
        NPPM_GETNBSESSIONFILES = NPPMSG + 13,
        /// <summary>
        /// BOOL NPPM_GETSESSIONFILES (wchar_t** sessionFileArray, wchar_t* sessionFileName)<br/>
        /// Get the full path names of all files listed in the session file with the given <c>sessionFileName</c>.
        /// <para>wParam (<see cref="UIntPtr"/>) [out]: <c>sessionFileArray</c> - pre-allocated array of file paths. To allocate the array with the proper size, first send <see cref="NPPM_GETNBSESSIONFILES"/></para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>sessionFileName</c> - the XML session file's full path</para>
        /// <para>Returns FALSE on failure, TRUE on success.</para>
        /// </summary>
        NPPM_GETSESSIONFILES = NPPMSG + 14,
        /// <summary>
        /// wchar_t* NPPM_SAVESESSION(0, sessionInfo* si)<br/>
        /// Save a given set of file paths to a new session file.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>si</c> - a pointer to a <see cref="SessionInfo"/> structure</para>
        /// <para>Returns a pointer to the session file name buffer on success, <see cref="Win32.NULL"/> otherwise.</para>
        /// </summary>
        /// <remarks>Unlike <see cref="NPPM_SAVECURRENTSESSION"/>, which saves the currently open files, this API can be used to add any file to a session.</remarks>
        NPPM_SAVESESSION = NPPMSG + 15,
        /// <summary>
        /// wchar_t* NPPM_SAVECURRENTSESSION(0, wchar_t* sessionFileName)<br/>
        /// Save all files currently open in Notepad++ to an XML session file.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>sessionFileName</c> - the XML session file's full path</para>
        /// <para>Returns a pointer to the <c>sessionFileName</c> buffer on success, <see cref="Win32.NULL"/> otherwise.</para>
        /// </summary>
        NPPM_SAVECURRENTSESSION = NPPMSG + 16,
        /// <summary>
        /// BOOL NPPM_GETOPENFILENAMESPRIMARY_DEPRECATED(wchar_t** fileNames, int nbFileNames)<br/>
        /// Get the full path names of all files currently open in the main view. The user must allocate a big enough <c>fileNames</c> array using <see cref="NPPM_GETNBOPENFILES"/>.
        /// <para>wParam (<see cref="UIntPtr"/>) [out]: <c>fileNames</c> - pre-allocated array of file paths</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>nbFileNames</c> - the number of file paths in <c>fileNames</c></para>
        /// <para>Returns the number of files copied to the <c>fileNames</c> array.</para>
        /// </summary>
        NPPM_GETOPENFILENAMESPRIMARY_DEPRECATED = NPPMSG + 17,
        [Obsolete("Replaced by NPPM_GETOPENFILENAMESPRIMARY_DEPRECATED", true)]
        NPPM_GETOPENFILENAMESPRIMARY = NPPM_GETOPENFILENAMESPRIMARY_DEPRECATED,
        /// <summary>
        /// BOOL NPPM_GETOPENFILENAMESSECOND_DEPRECATED(wchar_t** fileNames, int nbFileNames)<br/>
        /// Get the full path names of all files currently open in the sub-view. The user must allocate a big enough <c>fileNames</c> array using <see cref="NPPM_GETNBOPENFILES"/>.
        /// <para>wParam (<see cref="UIntPtr"/>) [out]: <c>fileNames</c> - pre-allocated array of file paths</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>nbFileNames</c> - the number of file paths in <c>fileNames</c></para>
        /// <para>Returns the number of files copied to the <c>fileNames</c> array.</para>
        /// </summary>
        NPPM_GETOPENFILENAMESSECOND_DEPRECATED = NPPMSG + 18,
        [Obsolete("Replaced by NPPM_GETOPENFILENAMESSECOND_DEPRECATED", true)]
        NPPM_GETOPENFILENAMESSECOND = NPPM_GETOPENFILENAMESSECOND_DEPRECATED,
        /// <summary>
        /// HWND NPPM_CREATESCINTILLAHANDLE(0, HWND hParent)<br/>
        /// Get a handle to the active Scintilla control.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>hParent</c> - if set (i.e., not <see cref="Win32.NULL"/>), it will be the parent window of this created Scintilla handle, otherwise the parent window is Notepad++</para>
        /// <para>Returns the created Scintilla handle.</para>
        /// </summary>
        NPPM_CREATESCINTILLAHANDLE = NPPMSG + 20,
        /// <summary>
        /// BOOL NPPM_DESTROYSCINTILLAHANDLE_DEPRECATED(0, HWND hScintilla)<br/>
        /// Does nothing and always returns TRUE. Notepad++ will deallocate every created Scintilla control on exit.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>hScintilla</c> - Scintilla handle</para>
        /// <para>Returns TRUE.</para>
        /// </summary>
        /// <remarks>
        /// This message exists for backward compatibility only.
        /// </remarks>
        NPPM_DESTROYSCINTILLAHANDLE_DEPRECATED = NPPMSG + 21,
        [Obsolete("Replaced by NPPM_DESTROYSCINTILLAHANDLE_DEPRECATED", true)]
        NPPM_DESTROYSCINTILLAHANDLE = NPPM_DESTROYSCINTILLAHANDLE_DEPRECATED,
        /// <summary>
        /// int NPPM_GETNBUSERLANG(0, int* udlID)<br/>
        /// Get the number of user-defined languages and, optionally, the starting menu id.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [optional]: <c>udlID</c> - 0, if not used; otherwise an integer pointer to retrieve the starting menu id</para>
        /// <para>Returns the number of user-defined languages identified.</para>
        /// </summary>
        NPPM_GETNBUSERLANG = NPPMSG + 22,
        /// <summary>
        /// int NPPM_GETCURRENTDOCINDEX(0, int inView)<br/>
        /// Get the index of the document currently open in the given view.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>inView</c> - either <see cref="MAIN_VIEW"/>) or <see cref="SUB_VIEW"/></para>
        /// <para>Returns -1 if the view is invisible (hidden), otherwise the current document's index.</para>
        /// </summary>
        NPPM_GETCURRENTDOCINDEX = NPPMSG + 23,
        MAIN_VIEW = 0,
        SUB_VIEW = 1,
        /// <summary>
        /// BOOL NPPM_SETSTATUSBAR(int whichPart, wchar_t* str2set)<br/>
        /// Change the text in a field of the status bar.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>whichPart</c> - a value between <see cref="STATUSBAR_DOC_TYPE"/> - <see cref="STATUSBAR_TYPING_MODE"/> indicating the status bar section to set</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>str2set</c> - the string to be shown in the given status bar section</para>
        /// <para>Returns FALSE on failure, TRUE on success.</para>
        /// </summary>
        NPPM_SETSTATUSBAR = NPPMSG + 24,
        STATUSBAR_DOC_TYPE = 0,
        STATUSBAR_DOC_SIZE = 1,
        STATUSBAR_CUR_POS = 2,
        STATUSBAR_EOF_FORMAT = 3,
        STATUSBAR_UNICODE_TYPE = 4,
        STATUSBAR_TYPING_MODE = 5,
        /// <summary>
        /// HMENU NPPM_GETMENUHANDLE(int menuChoice, 0)<br/>
        /// Get a handle to the menu indicated by <c>menuChoice</c>.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>menuChoice</c> - either the main menu (<see cref="NPPMAINMENU"/>) or the plugin menu (<see cref="NPPPLUGINMENU"/>)</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns the desired menu handle (<see cref="IntPtr"/>): either the plugin menu handle or the Notepad++ main menu handle.</para>
        /// </summary>
        NPPM_GETMENUHANDLE = NPPMSG + 25,
        /// <summary>See <seealso cref="NPPM_GETMENUHANDLE"/></summary>
        NPPPLUGINMENU = 0,
        /// <summary>See <seealso cref="NPPM_GETMENUHANDLE"/></summary>
        NPPMAINMENU = 1,
        /// <summary>
        /// int NPPM_ENCODESCI(int inView, 0)<br/>
        /// Change the document encoding of the given view.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>inView</c> - <see cref="MAIN_VIEW"/> (0) or <see cref="SUB_VIEW"/> (1)</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns the new UniMode, with the following value:<br/><br/>
        /// 0: ANSI<br/>
        /// 1: UTF-8 with BOM<br/>
        /// 2: UTF-16 Big Ending with BOM<br/>
        /// 3: UTF-16 Little Ending with BOM<br/>
        /// 4: UTF-8 without BOM<br/>
        /// 5: uni7Bit<br/>
        /// 6: UTF-16 Big Ending without BOM<br/>
        /// 7: UTF-16 Little Ending without BOM</para>
        /// </summary>
        NPPM_ENCODESCI = NPPMSG + 26,
        /// <summary>
        /// int NPPM_DECODESCI(int inView, 0)<br/>
        /// Change the document encoding of the given view to ANSI.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>inView</c> - <see cref="MAIN_VIEW"/> (0) or <see cref="SUB_VIEW"/> (1)</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns the old UniMode - see <see cref="NPPM_ENCODESCI"/>.</para>
        /// </summary>
        NPPM_DECODESCI = NPPMSG + 27,
        /// <summary>
        /// BOOL NPPM_ACTIVATEDOC(int inView, int index2Activate)<br/>
        /// Switch to the document in the given view with the given <c>index2Activate</c>.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>inView</c> - <see cref="MAIN_VIEW"/> (0) or <see cref="SUB_VIEW"/> (1)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>index2Activate</c> - index (in the given view) of the document to be activated</para>
        /// <para>Returns TRUE.</para>
        /// </summary>
        NPPM_ACTIVATEDOC = NPPMSG + 28,
        /// <summary>
        /// BOOL NPPM_LAUNCHFINDINFILESDLG(wchar_t* dir2Search, wchar_t* filter)<br/>
        /// Launch the "Find in Files" dialog and set the "Find in" directory and filters with the given arguments.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>dir2Search</c> - if not <see cref="Win32.NULL"/>, this will be the working directory in which Notepad++ will search</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]:<c>filter</c> - if not <see cref="Win32.NULL"/>, this filter string will be set in the filter field</para>
        /// <para>Returns TRUE.</para>
        /// </summary>
        NPPM_LAUNCHFINDINFILESDLG = NPPMSG + 29,
        /// <summary>
        /// BOOL NPPM_DMMSHOW(0, HWND hDlg)<br/>
        /// Show a dialog that was previously registered with <see cref="NPPM_DMMREGASDCKDLG"/>.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>hDlg</c> - the handle of the dialog to show</para>
        /// <para>Returns TRUE.</para>
        /// </summary>
        NPPM_DMMSHOW = NPPMSG + 30,
        /// <summary>
        /// BOOL NPPM_DMMHIDE(0, HWND hDlg)<br/>
        /// Hide a dialog that was previously registered with <see cref="NPPM_DMMREGASDCKDLG"/>.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>hDlg</c> - the handle of the dialog to hide</para>
        /// <para>Returns TRUE.</para>
        /// </summary>
        NPPM_DMMHIDE = NPPMSG + 31,
        /// <summary>
        /// BOOL NPPM_DMMUPDATEDISPINFO(0, HWND hDlg)<br/>
        /// Redraw the title bar of a dialog that was previously registered with <see cref="NPPM_DMMREGASDCKDLG"/>.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>hDlg</c> - the handle of the dialog to redraw</para>
        /// <para>Returns TRUE.</para>
        /// </summary>
        NPPM_DMMUPDATEDISPINFO = NPPMSG + 32,
        /// <summary>
        /// BOOL NPPM_DMMREGASDCKDLG(0, tTbData* pData)<br/>
        /// Register a plugin dialog with the Docking Manager.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>pData</c> - a pointer to a <see cref="Winforms.NppTbData"/> structure.
        ///             The fields which, at minimum, need to be filled out are <see cref="Winforms.NppTbData.HClient"/>, <see cref="Winforms.NppTbData.PszName"/>, <see cref="Winforms.NppTbData.DlgID"/>, <see cref="Winforms.NppTbData.UMask"/> and <see cref="Winforms.NppTbData.PszModuleName"/>.
        ///             Note that <see cref="Winforms.NppTbData.RcFloat"/> and <see cref="Winforms.NppTbData.IPrevCont"/> should <b>not</b> be filled. They are used internally.</para>
        /// <para>Returns TRUE.</para>
        /// </summary>
        NPPM_DMMREGASDCKDLG = NPPMSG + 33,
        /// <summary>
        /// BOOL NPPM_LOADSESSION(0, wchar_t* sessionFileName)<br/>
        /// Reopen all the files listed in <c>sessionFileName</c>.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>sessionFileName</c> - the full path name of the session file to reload</para>
        /// <para>Returns TRUE.</para>
        /// </summary>
        NPPM_LOADSESSION = NPPMSG + 34,
        /// <summary>
        /// BOOL NPPM_DMMVIEWOTHERTAB(0, wchar_t* name)<br/>
        /// Show the docked dialog with the given <c>name</c> (or switch to the tab of that dialog in a docking group).
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>name</c> - the name used to register the dialog (i.e., the <see cref="Winforms.NppTbData.PszName"/> of the <see cref="Winforms.NppTbData"/> passed to <see cref="NPPM_DMMREGASDCKDLG"/>)</para>
        /// <para>Returns TRUE.</para>
        /// </summary>
        NPPM_DMMVIEWOTHERTAB = NPPMSG + 35,
        /// <summary>
        /// BOOL NPPM_RELOADFILE(BOOL withAlert, wchar_t* filePathName2Reload)<br/>
        /// Reload the document matching the given <c>filePathName2Reload</c>.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>filePathName2Reload</c> - full file path of the document to reload</para>
        /// <para>Returns TRUE if reloading succeeds, otherwise FALSE.</para>
        /// </summary>
        NPPM_RELOADFILE = NPPMSG + 36,
        /// <summary>
        /// BOOL NPPM_SWITCHTOFILE(0, wchar_t* filePathName2switch)<br/>
        /// Switch to the document matching the given <c>filePathName2switch</c>.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>filePathName2switch</c> - the full file path of document to switch to</para>
        /// <para>Returns TRUE.</para>
        /// </summary>
        NPPM_SWITCHTOFILE = NPPMSG + 37,
        /// <summary>
        /// BOOL NPPM_SAVECURRENTFILE(0, 0)<br/>
        /// Save the currently active document.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns TRUE if the file is saved, otherwise FALSE (the file doesn't need to be saved, or another reason).</para>
        /// </summary>
        NPPM_SAVECURRENTFILE = NPPMSG + 38,
        /// <summary>
        /// BOOL NPPM_SAVEALLFILES(0, 0)<br/>
        /// Save all opened documents.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns FALSE when no file needs to be saved, else TRUE if at least one file is saved.</para>
        /// </summary>
        NPPM_SAVEALLFILES = NPPMSG + 39,
        /// <summary>
        /// BOOL NPPM_SETMENUITEMCHECK(UINT pluginCmdID, BOOL doCheck)<br/>
        /// Set or remove the checkmark on a plugin menu item and highlight or unhighlight its toolbar icon (if any).
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>pluginCmdID</c> - the plugin command ID corresponding to the menu item; see <see cref="FuncItem.CmdID"/></para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>doCheck</c> - TRUE to check the item, or FALSE to uncheck it</para>
        /// <para>Returns TRUE.</para>
        /// </summary>
        NPPM_SETMENUITEMCHECK = NPPMSG + 40,
        /// <summary>
        /// BOOL NPPM_ADDTOOLBARICON_DEPRECATED(UINT pluginCmdID, toolbarIcons* iconHandles)<br/>
        /// Add an icon to the toolbar.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>pluginCmdID</c> - the plugin command ID corresponding to the toolbar item whose icon will be set; see <see cref="FuncItem.CmdID"/></para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>iconHandles</c> - pointer to a <see cref="ToolbarIcon"/> structure. 2 formats (ICO and BMP) are needed.
        /// Both handles should be set to ensure correct display in case the user selects a custom toolbar icon set</para>
        /// <para>Returns TRUE.</para>
        /// </summary>
        /// <remarks>
        /// Use <see cref="NPPM_ADDTOOLBARICON_FORDARKMODE"/> instead.
        /// </remarks>
        NPPM_ADDTOOLBARICON_DEPRECATED = NPPMSG + 41,
        [Obsolete("Replaced by NPPM_ADDTOOLBARICON_DEPRECATED", true)]
        NPPM_ADDTOOLBARICON = NPPM_ADDTOOLBARICON_DEPRECATED,
        /// <summary>
        /// winVer NPPM_GETWINDOWSVERSION(0, 0)<br/>
        /// Get the host PC's (Windows) OS version.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns a <see cref="WinVer"/> value.</para>
        /// </summary>
        NPPM_GETWINDOWSVERSION = NPPMSG + 42,
        /// <summary>
        /// HWND NPPM_DMMGETPLUGINHWNDBYNAME(const wchar_t* windowName, const wchar_t* moduleName)<br/>
        /// Retrieve the handle of the docked dialog corresponding to the given <c>windowName</c> and <c>moduleName</c>.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>windowName</c> - if <see cref="Win32.NULL"/>, the handle of the first window matching <c>moduleName</c> will be returned</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>moduleName</c> - if <see cref="Win32.NULL"/>, the return value is also <see cref="Win32.NULL"/></para>
        /// <para>Returns <see cref="Win32.NULL"/> if <c>moduleName</c> is <see cref="Win32.NULL"/>. If <c>windowName</c> is <see cref="Win32.NULL"/>, the handle of the first window matching <c>moduleName</c> is returned.</para>
        /// </summary>
        /// <remarks>
        /// Use this API to communicate with another plugin's docking dialog.
        /// </remarks>
        NPPM_DMMGETPLUGINHWNDBYNAME = NPPMSG + 43,
        /// <summary>
        /// BOOL NPPM_MAKECURRENTBUFFERDIRTY(0, 0)<br/>
        /// Make the current document dirty, i.e., set the current buffer state to "modified".
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns TRUE.</para>
        /// </summary>
        NPPM_MAKECURRENTBUFFERDIRTY = NPPMSG + 44,
        /// <summary>
        /// THEMEAPI NPPM_GETENABLETHEMETEXTUREFUNC(0, 0)<br/>
        /// Get the function address of <c>::EnableThemeDialogTexture()</c>.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns a procedure address, or <see cref="Win32.NULL"/>.</para>
        /// </summary>
        /// <remarks>
        /// Deprecated since <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/50e95d22675b86b6afeacb6fdc217a0011f9528e">8.4.9</a>.
        /// Use <a href="https://learn.microsoft.com/windows/win32/api/uxtheme/nf-uxtheme-enablethemedialogtexture"><c>EnableThemeDialogTexture()</c></a> directly from <c>uxtheme.h</c> instead.
        /// </remarks>
        [Obsolete("Use EnableThemeDialogTexture directly (uxtheme.h) instead", false)]
        NPPM_GETENABLETHEMETEXTUREFUNC = NPPMSG + 45,
        /// <summary>
        /// int NPPM_GETPLUGINSCONFIGDIR(int strLen, wchar_t* str)<br/>
        /// Get the user's plugin config directory path. Useful if plugins need to save/load configuration options across sessions.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>strLen</c> - buffer length of <c>str</c></para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: <c>str</c> - the allocated buffer</para>
        /// <para>When <c>str</c> is <see cref="Win32.NULL"/>, returns the number of UTF-16 characters to copy; when <c>str</c> is an allocated buffer, returns FALSE on failure, TRUE on success.</para>
        /// </summary>
        /// <remarks>
        /// The user should first call this API with <c>str</c> set to <see cref="Win32.NULL"/> to get the number of UTF-16 characters
        /// (not including the <see cref="Win32.NULL"/> character), allocate <c>str</c> with the return value + 1, then call it again to get the directory path.
        /// </remarks>
        NPPM_GETPLUGINSCONFIGDIR = NPPMSG + 46,
        /// <summary>
        /// BOOL NPPM_MSGTOPLUGIN(wchar_t* destModuleName, communicationInfo *info)<br/>
        /// Send a private message to the plugin with the given <c>destModuleName</c>.
        /// For example, plugin X can execute a command of plugin Y if plugin X knows the command ID and the file name of plugin Y.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>destModuleName</c> - the complete module file name of the destination plugin (including the ".dll" file extension)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>communicationInfo</c> - pointer to a <see cref="CommunicationInfo"/> structure</para>
        /// <para>
        /// Returns TRUE if Notepad++ found the plugin by its module name (<c>destModuleName</c>) and passed the info (via <c>communicationInfo</c>),
        /// or FALSE if the destination plugin is not found.
        /// </para>
        /// </summary>
        NPPM_MSGTOPLUGIN = NPPMSG + 47,
        /// <summary>
        /// BOOL NPPM_MENUCOMMAND(0, int cmdID)<br/>
        /// Run the Notepad++ command with the given command ID.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>cmdID</c> - see <see cref="MenuCmdId"/> for all the Notepad++ menu command items</para>
        /// <para>Returns TRUE.</para>
        /// </summary>
        NPPM_MENUCOMMAND = NPPMSG + 48,
        /// <summary>
        /// BOOL NPPM_TRIGGERTABBARCONTEXTMENU(int inView, int index2Activate)<br/>
        /// Switch to the document in the given view with the given <c>index2Activate</c>, then open the context menu in it.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>inView</c> - either <see cref="MAIN_VIEW"/> or <see cref="SUB_VIEW"/></para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>index2Activate</c> - the index (in the given view) of the document where the context menu will be triggered</para>
        /// <para>Returns TRUE.</para>
        /// </summary>
        NPPM_TRIGGERTABBARCONTEXTMENU = NPPMSG + 49,
        /// <summary>
        /// int NPPM_GETNPPVERSION(BOOL ADD_ZERO_PADDING, 0)<br/>
        /// Get the version of the currently running edition of Notepad++.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>ADD_ZERO_PADDING</c> - <em>Since <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/ef609c8">8.4.1</a></em>:
        /// Whether or not to ensure that <c>100 &gt;= LOWORD(version) &lt;= 999</c></para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>
        /// Return value:
        /// <ul><li><c>HIWORD(returned_value)</c> - 1st digit of the version</li>
        /// <li><c>LOWORD(returned_value)</c> - last 3 digits of the version</li></ul>
        /// </para>
        /// <example>
        /// ADD_ZERO_PADDING == TRUE
        /// <code>
        ///     version  | HIWORD | LOWORD
        ///     ------------------------------
        ///     8.9.6.4  | 8      | 964
        ///     9        | 9      | 0
        ///     6.9      | 6      | 900
        ///     6.6.6    | 6      | 660
        ///     13.6.6.6 | 13     | 666
        /// </code>
        /// </example>
        /// <example>
        /// ADD_ZERO_PADDING == FALSE
        /// <code>
        ///     version  | HIWORD | LOWORD
        ///     ------------------------------
        ///     8.9.6.4  | 8      | 964
        ///     9        | 9      | 0
        ///     6.9      | 6      | 9
        ///     6.6.6    | 6      | 66
        ///     13.6.6.6 | 13     | 666
        /// </code>
        /// </example>
        /// </summary>
        /// <remarks>
        /// Changed in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/ef609c896f209ecffd8130c3e3327ca8a8157e72">8.4.1</a>
        /// </remarks>
        NPPM_GETNPPVERSION = NPPMSG + 50,
        /// <summary>
        /// BOOL NPPM_HIDETABBAR(0, BOOL hideOrNot)<br/>
        /// Hide (or show) the tab bar.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>hideOrNot</c> - if TRUE the tab bar will be hidden, otherwise it will be shown</para>
        /// <para>Returns the old status value.</para>
        /// </summary>
        NPPM_HIDETABBAR = NPPMSG + 51,
        /// <summary>
        /// BOOL NPPM_ISTABBARHIDDEN(0, 0)<br/>
        /// Get the visibility (hidden or visible) of the tab bar.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns TRUE if the tab bar is hidden, otherwise FALSE.</para>
        /// </summary>
        NPPM_ISTABBARHIDDEN = NPPMSG + 52,
        /// <summary>
        /// int NPPM_GETPOSFROMBUFFERID(UINT_PTR bufferID, int priorityView)<br/>
        /// Get the position of the document with <c>bufferID</c>, searching in <c>priorityView</c> first.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>bufferID</c> - the buffer ID of the document</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>priorityView</c> - the target view. If <c>bufferID</c> cannot be found in the target view, the other view will be searched. If set to <see cref="SUB_VIEW"/>, the sub view will be searched first</para>
        /// <para>Returns -1 if no document with <c>bufferID</c> exists, otherwise <c>(VIEW &lt;&lt; 30) | INDEX</c>, where <c>VIEW</c> is either <see cref="MAIN_VIEW"/> or <see cref="SUB_VIEW"/>,
        /// and <c>INDEX</c> is the 0-based document index.</para>
        /// </summary>
        NPPM_GETPOSFROMBUFFERID = NPPMSG + 57,
        /// <summary>
        /// int NPPM_GETFULLPATHFROMBUFFERID(UINT_PTR bufferID, wchar_t* fullFilePath)<br/>
        /// Get the full file path of the document with the given buffer ID.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>bufferID</c> - the buffer ID of the document</para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: <c>fullFilePath</c> - the allocated file path buffer</para>
        /// <para>Returns -1 if no document with <c>bufferID</c> exists, otherwise the number of UTF-16 characters copied/to copy.</para>
        /// </summary>
        /// <remarks>
        /// The user should first call this API with <c>fullFilePath</c> set to <see cref="Win32.NULL"/> to get the number of UTF-16 characters
        /// (not including the <see cref="Win32.NULL"/> character), allocate <c>fullFilePath</c> with the return value + 1, then call it again to get the file path.
        /// </remarks>
        NPPM_GETFULLPATHFROMBUFFERID = NPPMSG + 58,
        /// <summary>
        /// UINT_PTR NPPM_GETBUFFERIDFROMPOS(int index, int iView)<br/>
        /// Get the buffer ID of the document in the given view with the given <c>index</c>.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>index</c> - 0-based index of the document</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>inView</c> - <see cref="MAIN_VIEW"/> or <see cref="SUB_VIEW"/></para>
        /// <para>Returns <see cref="Win32.NULL"/> if invalid, otherwise the buffer ID of the document.</para>
        /// </summary>
        NPPM_GETBUFFERIDFROMPOS = NPPMSG + 59,
        /// <summary>
        /// UINT_PTR NPPM_GETCURRENTBUFFERID(0, 0)<br/>
        /// Get the active document's buffer ID.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns the buffer ID of the active document.</para>
        /// </summary>
        NPPM_GETCURRENTBUFFERID = NPPMSG + 60,
        /// <summary>
        /// BOOL NPPM_RELOADBUFFERID(UINT_PTR bufferID, BOOL alert)<br/>
        /// Reload the document with the given buffer ID.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>bufferID</c> - buffer ID of the document to reload</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>alert</c> - TRUE to let the user confirm or reject the reload, or FALSE to reload with no alert</para>
        /// <para>Returns TRUE on success, FALSE otherwise.</para>
        /// </summary>
        NPPM_RELOADBUFFERID = NPPMSG + 61,
        /// <summary>
        /// int NPPM_GETBUFFERLANGTYPE(UINT_PTR bufferID, 0)<br/>
        /// Get the <see cref="LangType"/> associated with the current document matching the given buffer ID.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>bufferID</c> - the buffer ID of the document</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns a <see cref="LangType"/> value, or -1 on error.</para>
        /// </summary>
        NPPM_GETBUFFERLANGTYPE = NPPMSG + 64,
        /// <summary>
        /// BOOL NPPM_SETBUFFERLANGTYPE(UINT_PTR bufferID, int langType)<br/>
        /// Set the <see cref="LangType"/> associated with the current document matching the given buffer ID.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>bufferID</c> - buffer ID of the document</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>langType</c> - a <see cref="LangType"/> value</para>
        /// <para>Returns TRUE on success, FALSE otherwise.</para>
        /// </summary>
        /// <remarks>The <c>langType</c> parameter cannot be <see cref="LangType.L_USER"/> or <see cref="LangType.L_EXTERNAL"/>.</remarks>
        NPPM_SETBUFFERLANGTYPE = NPPMSG + 65,
        /// <summary>
        /// int NPPM_GETBUFFERENCODING(UINT_PTR bufferID, 0)<br/>
        /// Get the encoding of the document with the given buffer ID.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>bufferID</c> - buffer ID of the document</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns -1 on error, otherwise the UniMode, with the following value:<br/><br/>
        /// 0: ANSI<br/>
        /// 1: UTF-8 with BOM<br/>
        /// 2: UTF-16 Big Ending with BOM<br/>
        /// 3: UTF-16 Little Ending with BOM<br/>
        /// 4: UTF-8 without BOM<br/>
        /// 5: uni7Bit<br/>
        /// 6: UTF-16 Big Ending without BOM<br/>
        /// 7: UTF-16 Little Ending without BOM
        /// </para>
        /// </summary>
        NPPM_GETBUFFERENCODING = NPPMSG + 66,
        /// <summary>
        /// BOOL NPPM_SETBUFFERENCODING(UINT_PTR bufferID, int encoding)<br/>
        /// Set the encoding of the document with the given buffer ID.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>bufferID</c> - buffer ID of the document</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>encoding</c> - one of the UniMode values returned by <see cref="NPPM_GETBUFFERENCODING"/></para>
        /// <para>Returns TRUE on success, FALSE otherwise.</para>
        /// </summary>
        /// <remarks>Can only be called on new, unedited files.</remarks>
        NPPM_SETBUFFERENCODING = NPPMSG + 67,
        /// <summary>
        /// int NPPM_GETBUFFERFORMAT(UINT_PTR bufferID, 0)<br/>
        /// Get the EOL format of the document with the given buffer ID.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>bufferID</c> - buffer ID of the document</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>
        /// Returns -1 on error, otherwise one of the following EOL formats:<br/><br/>
        ///  0: Windows (CRLF)<br/>
        ///  1: Macos (CR)<br/>
        ///  2: Unix (LF)<br/>
        ///  3. Unknown
        /// </para>
        /// </summary>
        NPPM_GETBUFFERFORMAT = NPPMSG + 68,
        /// <summary>
        /// BOOL NPPM_SETBUFFERFORMAT(UINT_PTR bufferID, int format)<br/>
        /// Set the EOL format of the document with the given buffer ID.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>bufferID</c> - buffer ID of the document</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>format</c> - one of the EOL formats returned by <see cref="NPPM_GETBUFFERFORMAT"/></para>
        /// <para>Returns TRUE on success, FALSE otherwise.</para>
        /// </summary>
        NPPM_SETBUFFERFORMAT = NPPMSG + 69,
        /// <summary>
        /// BOOL NPPM_HIDETOOLBAR(0, BOOL hideOrNot)<br/>
        /// Hide (or show) the toolbar.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>hideOrNot</c> - if TRUE the toolbar will be hidden, otherwise it will be shown</para>
        /// <para>Returns the old status value.</para>
        /// </summary>
        NPPM_HIDETOOLBAR = NPPMSG + 70,
        /// <summary>
        /// BOOL NPPM_ISTOOLBARHIDDEN(0, 0)<br/>
        /// Get the visibility (hidden or visible) of the toolbar.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns TRUE if the toolbar is hidden, otherwise FALSE.</para>
        /// </summary>
        NPPM_ISTOOLBARHIDDEN = NPPMSG + 71,
        /// <summary>
        /// BOOL NPPM_HIDEMENU(0, BOOL hideOrNot)<br/>
        /// Hide (or show) the menu bar.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>hideOrNot</c> - if TRUE the menu bar will be hidden, otherwise it will be shown</para>
        /// <para>Returns the old status value.</para>
        /// </summary>
        NPPM_HIDEMENU = NPPMSG + 72,
        /// <summary>
        /// BOOL NPPM_ISMENUHIDDEN(0, 0)<br/>
        /// Get the visibility (hidden or visible) of the menu bar.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns TRUE if the menu bar is hidden, otherwise FALSE.</para>
        /// </summary>
        NPPM_ISMENUHIDDEN = NPPMSG + 73,
        /// <summary>
        /// BOOL NPPM_HIDESTATUSBAR(0, BOOL hideOrNot)<br/>
        /// Hide (or show) the status bar.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>hideOrNot</c> - if TRUE the status bar will be hidden, otherwise it will be shown</para>
        /// <para>Returns the old status value.</para>
        /// </summary>
        NPPM_HIDESTATUSBAR = NPPMSG + 74,
        /// <summary>
        /// BOOL NPPM_ISSTATUSBARHIDDEN(0, 0)<br/>
        /// Get the visibility (hidden or visible) of the status bar.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns TRUE if the status bar is hidden, otherwise FALSE.</para>
        /// </summary>
        NPPM_ISSTATUSBARHIDDEN = NPPMSG + 75,
        /// <summary>
        /// BOOL NPPM_GETSHORTCUTBYCMDID(int cmdID, ShortcutKey* sk)<br/>
        /// Get the shortcut mapped to the plugin command with the given <c>cmdID</c>.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>cmdID</c> - plugin command ID</para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: <c>sk</c> - pre-allocated pointer to a <see cref="ShortcutKey"/></para>
        /// <para>Returns TRUE if the shortcut has been enabled, otherwise FALSE.</para>
        /// </summary>
        /// <remarks>Should only be called <em>after</em> the <see cref="NPPN_READY"/> notification has been sent.</remarks>
        NPPM_GETSHORTCUTBYCMDID = NPPMSG + 76,
        /// <summary>
        /// BOOL NPPM_DOOPEN(0, const wchar_t* fullPathName2Open)<br/>
        /// Open the file with the given <c>fullPathName2Open</c>.
        /// If it is already open in Notepad++, it will be activated and become the current document.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>fullPathName2Open</c> - full path name of the file to be opened</para>
        /// <para>Returns TRUE if the operation is successful, otherwise FALSE.</para>
        /// </summary>
        NPPM_DOOPEN = NPPMSG + 77,
        /// <summary>
        /// BOOL NPPM_SAVECURRENTFILEAS (BOOL saveAsCopy, const wchar_t* filename)<br/>
        /// Save the currently active document with the given <c>filename</c> and, optionally, save a copy under the current file name.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>saveAsCopy</c> - FALSE to rename the current file, or TRUE to save a copy of it (like the "Save a Copy As..." action)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>filename</c> - full path name of the file to be saved</para>
        /// <para>Returns TRUE if the operation is successful, otherwise FALSE.</para>
        /// </summary>
        NPPM_SAVECURRENTFILEAS = NPPMSG + 78,
        /// <summary>
        /// int NPPM_GETCURRENTNATIVELANGENCODING(0, 0)<br/>
        /// Get the code page associated with the current localization of Notepad++.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns the <a href="https://learn.microsoft.com/windows/win32/intl/code-page-identifiers">code page identifier</a> of the current native language encoding.</para>
        /// </summary>
        NPPM_GETCURRENTNATIVELANGENCODING = NPPMSG + 79,
        /// <summary>
        /// BOOL NPPM_ALLOCATESUPPORTED_DEPRECATED(0, 0)<br/>
        /// Get the status of support for the <see cref="NPPM_ALLOCATECMDID"/> API.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns TRUE if NPPM_ALLOCATECMDID is supported.</para>
        /// </summary>
        /// <remarks>
        /// Used to identify if subclassing is necessary.
        /// This message was added (in 2010) only for checking if NPPM_ALLOCATECMDID was supported; it is no longer needed.
        /// </remarks>
        NPPM_ALLOCATESUPPORTED_DEPRECATED = NPPMSG + 80,
        [Obsolete("Replaced by NPPM_ALLOCATESUPPORTED_DEPRECATED", true)]
        NPPM_ALLOCATESUPPORTED = NPPM_ALLOCATESUPPORTED_DEPRECATED,
        /// <summary>
        /// BOOL NPPM_ALLOCATECMDID(int numberRequested, int* startNumber)<br/>
        /// Allocate a consecutive number of menu item IDs for a plugin.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>numberRequested</c> - the number of IDs requested for reservation</para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: <c>startNumber</c> - will be set to the initial command ID if successful</para>
        /// <example>
        /// For example, if a plugin needs 4 menu item IDs, the following code can be used:
        /// <code>
        ///     int idBegin;
        ///     BOOL isAllocatedSuccessful = ::SendMessage(nppData._nppHandle, NPPM_ALLOCATECMDID, 4, &amp;idBegin);
        /// </code>
        /// </example>
        /// <para>If <c>isAllocatedSuccessful</c> is TRUE, and the value of <c>idBegin</c> is 46581,
        /// then menu item IDs 46581, 46582, 46583 and 46584 are reserved by Notepad++, and they are safe to be used by the plugin.</para>
        /// <para>Returns TRUE if successful, FALSE otherwise; <c>startNumber</c> will also be set to 0 if unsuccessful.</para>
        /// </summary>
        /// <remarks>
        /// The obtained menu IDs are guaranteed not to conflict with other plugins.
        /// </remarks>
        NPPM_ALLOCATECMDID = NPPMSG + 81,
        /// <summary>
        /// BOOL NPPM_ALLOCATEMARKER(int numberRequested, int* startNumber)<br/>
        /// Allocate a consecutive number of marker IDs for a plugin.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>numberRequested</c> - the number of IDs requested for reservation</para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: <c>startNumber</c> - will be set to the initial command ID if successful</para>
        /// <example>
        /// For example, if a plugin needs 3 marker IDs, the following code can be used:
        /// <code>
        ///     int idBegin;
        ///     BOOL isAllocatedSuccessful = ::SendMessage(nppData._nppHandle, NPPM_ALLOCATEMARKER, 3, &amp;idBegin);
        /// </code>
        /// </example>
        /// <para>If <c>isAllocatedSuccessful</c> is TRUE, and the value of <c>idBegin</c> is 16,
        /// then marker IDs 16, 17 and 18 are reserved by Notepad++, and they are safe to be used by the plugin.</para>
        /// <para>Returns TRUE if successful, FALSE otherwise; <c>startNumber</c> will also be set to 0 if unsuccessful.</para>
        /// </summary>
        /// <remarks>
        /// If a plugin needs to add a marker on Notepad++'s Scintilla marker margin, it should reserve it using this API, in
        /// order to prevent a conflict with other plugins.
        /// </remarks>
        NPPM_ALLOCATEMARKER = NPPMSG + 82,
        /// <summary>
        /// int NPPM_GETLANGUAGENAME(int langType, wchar_t* langName)<br/>
        /// Get the name of the programming language associated with the given <c>langType</c>.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>langType</c> - a <see cref="LangType"/> value</para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: <c>langName</c> - the allocated language name buffer</para>
        /// <para>Returns the number of UTF-16 characters copied/to copy.</para>
        /// </summary>
        /// <remarks>
        /// Users should first call this API with <c>langName</c> set to <see cref="Win32.NULL"/> to get the required number of UTF-16 characters
        /// (not including the terminating <see cref="Win32.NULL"/> character), allocate the <c>langName</c> buffer with the return value + 1, then
        /// call it again to get the language name.
        /// </remarks>
        NPPM_GETLANGUAGENAME = NPPMSG + 83,
        /// <summary>
        /// int NPPM_GETLANGUAGEDESC(int langType, wchar_t* langDesc)<br/>
        /// Get a short description of the programming language with the given <c>langType</c>.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>langType</c> - a <see cref="LangType"/> value</para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: <c>langDesc</c> - the allocated language description buffer</para>
        /// <para>Returns the number of UTF-16 characters copied/to copy.</para>
        /// </summary>
        /// <remarks>
        /// Users should first call this API with <c>langDesc</c> set to <see cref="Win32.NULL"/> to get the required number of UTF-16 characters
        /// (not including the terminating <see cref="Win32.NULL"/> character), allocate the <c>langDesc</c> buffer with the return value + 1, then
        /// call it again to get the description text.
        /// </remarks>
        NPPM_GETLANGUAGEDESC = NPPMSG + 84,
        /// <summary>
        /// BOOL NPPM_SHOWDOCLIST(0, BOOL toShowOrNot)<br/>
        /// Show or hide the Document List panel.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>toShowOrNot</c> - if TRUE, the Document List panel is shown, otherwise it is hidden</para>
        /// <para>Returns TRUE.</para>
        /// </summary>
        NPPM_SHOWDOCLIST = NPPMSG + 85,
        /// <summary>
        /// BOOL NPPM_ISDOCLISTSHOWN(0, 0)<br/>
        /// Get the visibility (hidden or visible) of the Document List panel.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns TRUE if the Document List panel is currently shown, FALSE otherwise.</para>
        /// </summary>
        NPPM_ISDOCSWITCHERSHOWN = NPPMSG + 86,
        /// <summary>
        /// BOOL NPPM_GETAPPDATAPLUGINSALLOWED(0, 0)<br/>
        /// Check to see if loading plugins from <c>"%APPDATA%\..\Local\Notepad++\plugins"</c> is allowed.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns TRUE if loading plugins from <c>%APPDATA%</c> is allowed, FALSE otherwise.</para>
        /// </summary>
        NPPM_GETAPPDATAPLUGINSALLOWED = NPPMSG + 87,
        /// <summary>
        /// int NPPM_GETCURRENTVIEW(0, 0)<br/>
        /// Get the currently active view.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns the current edit view: either <see cref="MAIN_VIEW"/> or <see cref="SUB_VIEW"/>.</para>
        /// </summary>
        NPPM_GETCURRENTVIEW = NPPMSG + 88,
        /// <summary>
        /// BOOL NPPM_DOCLISTDISABLEEXTCOLUMN(0, BOOL disableOrNot)<br/>
        /// Disable or enable the file extension column of the Document List.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>disableOrNot</c> - if TRUE, the extension column is hidden, otherwise it is shown</para>
        /// <para>Returns TRUE.</para>
        /// </summary>
        NPPM_DOCLISTDISABLEEXTCOLUMN = NPPMSG + 89,
        /// <summary>
        /// int NPPM_GETEDITORDEFAULTFOREGROUNDCOLOR(0, 0)<br/>
        /// Get the editor's current default foreground color.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns the color as an integer in hex format <c>0x00bbggrr</c>.</para>
        /// </summary>
        NPPM_GETEDITORDEFAULTFOREGROUNDCOLOR = NPPMSG + 90,
        /// <summary>
        /// int NPPM_GETEDITORDEFAULTBACKGROUNDCOLOR(0, 0)<br/>
        /// Get the editor's current default background color.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns the color as an integer in hex format <c>0x00bbggrr</c>.</para>
        /// </summary>
        NPPM_GETEDITORDEFAULTBACKGROUNDCOLOR = NPPMSG + 91,
        /// <summary>
        /// BOOL NPPM_SETSMOOTHFONT(0, BOOL setSmoothFontOrNot)<br/>
        /// Enable or disable "smooth fonts" (i.e., <a href="https://learn.microsoft.com/windows/win32/gdi/cleartype-antialiasing">ClearType Antialiasing</a>).
        /// This API simply sends the <see cref="SciMsg.SCI_SETFONTQUALITY"/> message to Scintilla with an equivalent font quality setting.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>setSmoothFontOrNot</c> - TRUE to send <see cref="SciMsg.SCI_SETFONTQUALITY"/> with <see cref="SciMsg.SC_EFF_QUALITY_LCD_OPTIMIZED"/>, or FALSE to send it with <see cref="SciMsg.SC_EFF_QUALITY_DEFAULT"/></para>
        /// <para>Returns TRUE.</para>
        /// </summary>
        NPPM_SETSMOOTHFONT = NPPMSG + 92,
        /// <summary>
        /// BOOL NPPM_SETEDITORBORDEREDGE(0, BOOL withEditorBorderEdgeOrNot)<br/>
        /// Add or remove an additional sunken edge style to the Scintilla window.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>withEditorBorderEdgeOrNot</c> - TRUE to add a border edge to the Scintilla window, FALSE to remove it</para>
        /// <para>Returns TRUE.</para>
        /// </summary>
        NPPM_SETEDITORBORDEREDGE = NPPMSG + 93,
        /// <summary>
        /// BOOL NPPM_SAVEFILE(0, const wchar_t* fileNameToSave)<br/>
        /// Save the (currently open) file with the given <c>fileNameToSave</c>.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>fileNameToSave</c> - the full path of the file to be saved</para>
        /// <para>Returns TRUE on success, FALSE if <c>fileNameToSave</c> is not found.</para>
        /// </summary>
        NPPM_SAVEFILE = NPPMSG + 94,
        /// <summary>
        /// BOOL NPPM_DISABLEAUTOUPDATE(0, 0)<br/>
        /// Disable automatic updates of Notepad++.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns TRUE.</para>
        /// </summary>
        NPPM_DISABLEAUTOUPDATE = NPPMSG + 95, // 2119 in decimal
        /// <summary>
        /// BOOL NPPM_REMOVESHORTCUTBYCMDID(int pluginCmdID, 0)<br/>
        /// Remove a shortcut that was mapped to the plugin command with the given ID.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>pluginCmdID</c> - the ID of a plugin command</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns TRUE if successful, otherwise FALSE.</para>
        /// </summary>
        NPPM_REMOVESHORTCUTBYCMDID = NPPMSG + 96,  // 2120 in decimal
        /// <summary>
        /// int NPPM_GETPLUGINHOMEPATH(size_t strLen, wchar_t* pluginRootPath)<br/>
        /// Get the root path of all the installed Notepad++ plugins.
        /// For example, the full path to a plugin's installation folder would be: <c>&lt;pluginRootPath&gt;\\&lt;pluginFolderName&gt;</c>,
        /// where <c>&lt;pluginFolderName&gt;</c> is the name of the plugin without the ".dll" file extension.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>strLen</c> - buffer length of <c>pluginRootPath</c></para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: <c>pluginRootPath</c> - the allocated buffer</para>
        /// <para>Returns the number of UTF-16 characters copied/to copy, or 0 on failure.</para>
        /// </summary>
        /// <remarks>
        /// Users should first call this API with <c>pluginRootPath</c> set to <see cref="Win32.NULL"/> to get the required number of UTF-16 characters
        /// (not including the terminating <see cref="Win32.NULL"/> character), allocate the <c>pluginRootPath</c> buffer with the return value + 1, then
        /// call it again to get the directory path.
        /// </remarks>
        NPPM_GETPLUGINHOMEPATH = NPPMSG + 97,
        /// <summary>
        /// int NPPM_GETSETTINGSONCLOUDPATH(size_t strLen, wchar_t* settingsOnCloudPath)<br/>
        /// Get the user's cloud settings file path. Useful if plugins want to store settings in the cloud, provided a valid path is set.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>strLen</c> - buffer length of <c>settingsOnCloudPath</c></para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: <c>settingsOnCloudPath</c> - the allocated file path buffer</para>
        /// <para>Returns the number of UTF-16 characters copied/to copy. The return value is 0 if this path is not set, or if <c>strLen</c>
        /// is not enough to copy the path.</para>
        /// </summary>
        /// <remarks>
        /// Users should first call this API with <c>settingsOnCloudPath</c> set to <see cref="Win32.NULL"/> to get the required number of UTF-16 characters
        /// (not including the terminating <see cref="Win32.NULL"/> character), allocate the <c>settingsOnCloudPath</c> buffer with the returns value + 1, then
        /// call it again to get the file path.
        /// </remarks>
        NPPM_GETSETTINGSONCLOUDPATH = NPPMSG + 98,
        /// <summary>
        /// BOOL NPPM_SETLINENUMBERWIDTHMODE(0, int widthMode)<br/>
        /// Set the line number margin width mode to "dynamic" (<see cref="LINENUMWIDTH_DYNAMIC"/>) or "constant" (<see cref="LINENUMWIDTH_CONSTANT"/>).
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>widthMode</c> - either <see cref="LINENUMWIDTH_DYNAMIC"/> or <see cref="LINENUMWIDTH_CONSTANT"/></para>
        /// <para>Returns TRUE if successful, otherwise FALSE.</para>
        /// </summary>
        /// <remarks>
        /// Plugins should disable constant width mode to ensure a smoother visual effect during vertical content scrolling.
        /// </remarks>
        NPPM_SETLINENUMBERWIDTHMODE = NPPMSG + 99,
        /// <summary>See <seealso cref="NPPM_SETLINENUMBERWIDTHMODE"/></summary>
        LINENUMWIDTH_DYNAMIC = 0,
        /// <summary>See <seealso cref="NPPM_SETLINENUMBERWIDTHMODE"/></summary>
        LINENUMWIDTH_CONSTANT = 1,
        /// <summary>
        /// int NPPM_GETLINENUMBERWIDTHMODE(0, 0)<br/>
        /// Get the current line number margin width mode.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns the current line number margin width mode: either <see cref="LINENUMWIDTH_DYNAMIC"/> or <see cref="LINENUMWIDTH_CONSTANT"/>.</para>
        /// </summary>
        NPPM_GETLINENUMBERWIDTHMODE = NPPMSG + 100,
        /// <summary>
        /// BOOL NPPM_ADDTOOLBARICON_FORDARKMODE(UINT pluginCmdID, toolbarIconsWithDarkMode* iconHandles)<br/>
        /// Add an icon with a dark mode variant to the toolbar.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>pluginCmdID</c> - the plugin command ID corresponding to the toolbar item whose icon will be set; <see cref="FuncItem.CmdID"/></para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>iconHandles</c> - a pointer to a <see cref="ToolbarIconDarkMode"/> structure</para>
        /// <para>Returns TRUE.</para>
        /// </summary>
        /// <remarks>
        /// Added in <a href="https://community.notepad-plus-plus.org/topic/21652/add-new-api-nppm_addtoolbaricon_fordarkmode-for-dark-mode">8.0</a>
        /// </remarks>
        NPPM_ADDTOOLBARICON_FORDARKMODE = NPPMSG + 101,
        /// <summary>
        /// BOOL NPPM_DOCLISTDISABLEPATHCOLUMN(0, BOOL disableOrNot)<br/>
        /// Disable or enable the path column of the Document List.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>disableOrNot</c> - if TRUE, the path column is hidden, otherwise it is shown</para>
        /// <para>Returns TRUE.</para>
        /// </summary>
        NPPM_DOCLISTDISABLEPATHCOLUMN = NPPMSG + 102,
        /// <summary>
        /// BOOL NPPM_GETEXTERNALLEXERAUTOINDENTMODE(const wchar_t* languageName, ExternalLexerAutoIndentMode* autoIndentMode)<br/>
        /// Get the <see cref="ExternalLexerAutoIndentMode"/> of the external lexer identified by <c>languageName</c>.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>languageName</c> -  name of an external lexer provided by a plugin</para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: <c>autoIndentMode</c> - see <see cref="NPPM_SETEXTERNALLEXERAUTOINDENTMODE"/></para>
        /// <para>Returns TRUE on success, otherwise FALSE.</para>
        /// </summary>
        /// <remarks>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/9cbd03c">8.3.3</a>
        /// </remarks>
        NPPM_GETEXTERNALLEXERAUTOINDENTMODE = NPPMSG + 103,
        /// <summary>
        /// BOOL NPPM_SETEXTERNALLEXERAUTOINDENTMODE(const wchar_t* languageName, ExternalLexerAutoIndentMode autoIndentMode)<br/>
        /// Set the <see cref="ExternalLexerAutoIndentMode"/> of the external lexer identified by <c>languageName</c>.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>languageName</c> - name of an external lexer provided by a plugin</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>autoIndentMode</c> - one of the following values:<ul>
        ///             <li>Standard (0) - Notepad++ will keep the same TAB indentation between lines</li>
        ///             <li>C_Like (1) - Notepad++ will perform a C-Language style indentation for the selected external language</li>
        ///             <li>Custom (2) - a plugin will be controlling auto-indentation for the current language</li></ul></para>
        /// <para>Returns TRUE on success, otherwise FALSE.</para>
        /// </summary>
        /// <remarks>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/9cbd03c">8.3.3</a>
        /// </remarks>
        NPPM_SETEXTERNALLEXERAUTOINDENTMODE = NPPMSG + 104,
        /// <summary>
        /// BOOL NPPM_ISAUTOINDENTON(0, 0)<br/>
        /// Get the current state of the "Use Auto-Indentation" setting.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns TRUE if Auto-Indentation is on, FALSE otherwise.</para>
        /// </summary>
        /// <remarks>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/9cbd03c">8.3.3</a>
        /// </remarks>
        NPPM_ISAUTOINDENTON = NPPMSG + 105,
        /// <summary>
        /// MacroStatus NPPM_GETCURRENTMACROSTATUS(0, 0)<br/>
        /// Get the current <see cref="MacroStatus"/>.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns a <see cref="MacroStatus"/> value:<ul>
        /// <li>0: Idle - macro is not in use and it's empty</li>
        /// <li>1: RecordInProgress - macro is currently being recorded</li>
        /// <li>2: RecordingStopped - macro recording has been stopped</li>
        /// <li>3: PlayingBack - macro is currently being played back</li>
        /// </ul></para>
        /// </summary>
        /// <remarks>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/9cbd03c">8.3.3</a>
        /// </remarks>
        NPPM_GETCURRENTMACROSTATUS = NPPMSG + 106,
        /// <summary>
        /// BOOL NPPM_ISDARKMODEENABLED(0, 0)<br/>
        /// Get the current status (ON or OFF) of the dark mode setting.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns TRUE if dark mode is enabled, otherwise FALSE.</para>
        /// </summary>
        /// <remarks>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/1eb5b10e41d7ab92b60aa32b28d4fe7739d15b53">8.4.1</a>
        /// </remarks>
        NPPM_ISDARKMODEENABLED = NPPMSG + 107,
        /// <summary>
        /// BOOL NPPM_GETDARKMODECOLORS (size_t cbSize, NppDarkMode::Colors* returnColors)<br/>
        /// Get the color values of the active dark mode theme.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>cbSize</c> - must be equal to <c>sizeof(NppDarkMode::Colors)</c></para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: <c>returnColors</c> - a pointer to a <see cref="Winforms.DarkMode.DarkModeColors"/> structure</para>
        /// </summary>
        /// <para>Returns TRUE when successful, FALSE otherwise.</para>
        /// <remarks>
        /// <para>
        /// If calling this API fails (i.e., FALSE is returned), you may need to change the <see cref="Winforms.DarkMode.DarkModeColors"/> structure as shown
        /// <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/blob/master/PowerEditor/src/NppDarkMode.h#L25">here</a>.
        /// </para>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/1eb5b10e41d7ab92b60aa32b28d4fe7739d15b53">8.4.1</a>
        /// </remarks>
        NPPM_GETDARKMODECOLORS = NPPMSG + 108,
        /// <summary>
        /// int NPPM_GETCURRENTCMDLINE(size_t strLen, wchar_t* commandLineStr)<br/>
        /// Get the current command line string.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>strLen</c> - buffer length of <c>commandLineStr</c></para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: <c>commandLineStr</c> - allocated command line string buffer</para>
        /// <para>Returns the number of UTF-16 characters copied/to copy.</para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Users should first call this API with <c>commandLineStr</c> set to <see cref="Win32.NULL"/> to get the required number of UTF-16 characters
        /// (not including the terminating <see cref="Win32.NULL"/> character), allocate the <c>commandLineStr</c> buffer with the value + 1, then call it
        /// again to get the current command line string.
        /// </para>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/0f8d5724afb0a540e8b4024252945ab60bc88c71">8.4.2</a>
        /// </remarks>
        NPPM_GETCURRENTCMDLINE = NPPMSG + 109,
        /// <summary>
        /// void* NPPM_CREATELEXER(0, const wchar_t* lexer_name)<br/>
        /// Get an <c>ILexer</c> pointer created by Lexilla. This calls lexilla's <c>CreateLexer()</c> function so that plugins can set the lexer for a Scintilla instance created by <see cref="NPPM_CREATESCINTILLAHANDLE"/>.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>lexer_name</c> - the name of the lexer</para>
        /// <para>Returns the <c>ILexer</c> pointer.</para>
        /// </summary>
        /// <remarks>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/f1ed4de78dbe8f5d85f4d199bae2970148cca8ed">8.4.3</a>
        /// </remarks>
        NPPM_CREATELEXER = NPPMSG + 110,
        /// <summary>
        /// int NPPM_GETBOOKMARKID(0, 0)<br/>
        /// Get a stable bookmark ID.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>): 0 (not used)</para>
        /// <para>Returns the bookmark ID.</para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// This API guarantees you always get the right bookmark ID even if it's been changed by a newer version of Notepad++.
        /// </para>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/4d5069280900ee249d358bc2b311bdb4b03f30a9">8.4.7</a>
        /// </remarks>
        NPPM_GETBOOKMARKID = NPPMSG + 111,
        /// <summary>
        /// ULONG NPPM_DARKMODESUBCLASSANDTHEME(ULONG dmFlags, HWND hwnd)<br/>
        /// Make the plugin dialog with the given handle participate in automatic dark mode theming.
        /// Subclassing will be applied automatically unless the <see cref="Winforms.NppTbMsg.DWS_USEOWNDARKMODE"/> flag is used.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>dmFlags</c> - either <see cref="Winforms.DarkMode.dmfInit"/> or <see cref="Winforms.DarkMode.dmfHandleChange"/></para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>hwnd</c> - the dialog handle of the plugin</para>
        /// <example>
        /// Examples
        /// <ul><li>
        /// after controls have been initialized (i.e., <c>WM_INITDIALOG</c> or <c>WM_CREATE</c> has been handled, or <c>::CreateWindow()</c> has been called):
        /// <code>
        ///     auto success = static_cast&lt;ULONG&gt;(::SendMessage(nppData._nppHandle, NPPM_DARKMODESUBCLASSANDTHEME, static_cast&lt;WPARAM&gt;(NppDarkMode::dmfInit), reinterpret_cast&lt;LPARAM&gt;(mainHwnd)));
        /// </code></li><li>
        /// after a change of dark mode preference:
        /// <code>
        ///   extern "C" __declspec(dllexport) void beNotified(SCNotification * notifyCode)
        ///   {
        ///      switch (notifyCode->nmhdr.code)
        ///      {
        ///         case NPPN_DARKMODECHANGED:
        ///         {
        ///          ::SendMessage(nppData._nppHandle, NPPM_DARKMODESUBCLASSANDTHEME, static_cast&lt;WPARAM&gt;(dmfHandleChange), reinterpret_cast&lt;LPARAM&gt;(mainHwnd));
        ///          ::SetWindowPos(mainHwnd, NULLlptr, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
        ///          // to redraw titlebar and window
        ///          break;
        ///         }
        ///      }
        ///   }
        /// </code>
        /// </li></ul></example>
        /// <para>Returns the combination of <c>dmFlags</c>, if successful.</para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Docking panels are always automatically subclassed and do not need to call this API.
        /// </para>
        /// <para>
        /// <em>Might not work properly with C# plugins.</em>
        /// </para>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/e7f321f21a2feae3669b286ae2b64e6e033f231f">8.5.4</a>
        /// </remarks>
        NPPM_DARKMODESUBCLASSANDTHEME = NPPMSG + 112,
        /// <summary>
        /// BOOL NPPM_ALLOCATEINDICATOR(int numberRequested, int* startNumber)<br/>
        /// Allocates an indicator number for a plugin.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>numberRequested</c> - the ID number requested for reservation</para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: <c>startNumber</c> - will be set to the initial command ID, if successful</para>
        /// <example>
        /// For example, if a plugin needs 1 indicator ID, the following code can be used :
        /// <code>
        ///     int idBegin;
        ///     BOOL isAllocatedSuccessful = ::SendMessage(nppData._nppHandle, NPPM_ALLOCATEINDICATOR, 1, &amp;idBegin);
        /// </code>
        /// </example>
        /// <para>If <c>isAllocatedSuccessful</c> is TRUE, and the value of <c>idBegin</c> is 7,
        /// then indicator ID 7 is reserved by Notepad++, and it is safe to be used by the plugin.</para>
        /// <para>Returns TRUE if successful, FALSE otherwise; <c>startNumber</c> will also be set to 0 if unsuccessful.</para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// If a plugin needs to add an indicator, it should reserve it using this API, in order to prevent a conflict with other plugins.
        /// </para>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/de25873cb3352ee59d883e95e80c91806944e348">8.5.6</a>
        /// </remarks>
        NPPM_ALLOCATEINDICATOR = NPPMSG + 113,
        /// <summary>
        /// int NPPM_GETTABCOLORID (int view, int tabIndex)<br/>
        /// Get the color ID of the tab in the given view with the given tab index.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>view</c> - 0 for <see cref="MAIN_VIEW"/>, 1 for <see cref="SUB_VIEW"/>, or -1 for the currently active view</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>tabIndex</c> - 0-based tab index, i.e., 0 for the first tab, 1 for the second tab, etc., or -1 for the currently active tab</para>
        /// </summary>
        /// <para>Returns the tab color id value.</para>
        /// <remarks>
        /// <example>
        /// The return value is one of:
        /// <code>
        ///     -1 (no color)
        ///      0 (yellow)
        ///      1 (green)
        ///      2 (blue)
        ///      3 (orange)
        ///      4 (pink)
        /// </code>
        /// </example>
        /// <example>
        /// There is no matching "NPPM_SETTABCOLORID" API for setting the tab color.
        /// Plugins can instead use <see cref="NPPM_MENUCOMMAND"/> to set the active tab's color
        /// using the menu command IDs from <see cref="MenuCmdId.IDM_VIEW_TAB_COLOUR_NONE"/> to <see cref="MenuCmdId.IDM_VIEW_TAB_COLOUR_5"/>, i.e.:
        /// <code>
        ///     IDM_VIEW_TAB_COLOUR_NONE (no color)
        ///     IDM_VIEW_TAB_COLOUR_1 (yellow)
        ///     IDM_VIEW_TAB_COLOUR_2 (green)
        ///     IDM_VIEW_TAB_COLOUR_3 (blue)
        ///     IDM_VIEW_TAB_COLOUR_4 (orange)
        ///     IDM_VIEW_TAB_COLOUR_5 (pink)
        /// </code>
        /// </example>
        /// <para>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/9244cd09430c82ecff805ea862c9133d5cb56ded">8.6.8</a>
        /// </para>
        /// </remarks>
        NPPM_GETTABCOLORID = NPPMSG + 114,
        /// <summary>
        /// BOOL NPPM_SETUNTITLEDNAME(BufferID id, const wchar_t* newName)<br/>
        /// Set the name of the untitled tab with the given buffer ID.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>id</c> - buffer ID of the untitled tab</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>newName</c> - the desired new name of the tab</para>
        /// <para>Returns TRUE on success, FALSE otherwise.</para>
        /// </summary>
        /// <remarks>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/b3daf0a98220ffc6e206133aa645d5a2d1d63a4f">8.6.9</a>
        /// </remarks>
        NPPM_SETUNTITLEDNAME = NPPMSG + 115,
        /// <summary>
        /// int NPPM_GETNATIVELANGFILENAME(size_t strLen, char* nativeLangFileName)<br/>
        /// Get the name of the currently active XML localization file.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>strLen</c> - buffer length of <c>nativeLangFileName</c></para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>nativeLangFileName</c> - allocated language file name buffer</para>
        /// <para>Returns the number of single-byte characters copied/to copy. If there's no localization file in use, 0 is returned.</para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Users should first call this API with <c>nativeLangFileName</c> set to <see cref="Win32.NULL"/> to get the required number of single-byte characters
        /// (not including the terminating <see cref="Win32.NULL"/> character), allocate the <c>nativeLangFileName</c> buffer with the return value + 1, then
        /// call it again to get the localization file name. Call this <em>after</em> the <see cref="NPPN_READY"/> notification has been sent.
        /// </para>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/446cc980e871d04885f13055bb56acee820636c8">8.7</a>
        /// </remarks>
        NPPM_GETNATIVELANGFILENAME = NPPMSG + 116,
        /// <summary>
        /// BOOL NPPM_ADDSCNMODIFIEDFLAGS(0, unsigned long scnMotifiedFlags2Add)<br/>
        /// Add the necessary <see cref="ModificationFlags"/> so that your plugin will receive additional <see cref="SciMsg.SCN_MODIFIED"/> notifications.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>scnMotifiedFlags2Add</c> - <see cref="ModificationFlags"/> to add</para>
        /// <example>
        /// Example
        /// <code>
        ///   extern "C" __declspec(dllexport) void beNotified(SCNotification* notifyCode)
        ///   {
        ///     switch (notifyCode->nmhdr.code)
        ///     {
        ///       case NPPN_READY:
        ///       {
        ///         // Add SC_MOD_BEFOREDELETE and SC_MOD_BEFOREINSERT to listen to the 2 events of SCN_MODIFIED
        ///         ::SendMessage(nppData._nppHandle, NPPM_ADDSCNMODIFIEDFLAGS, 0, SC_MOD_BEFOREDELETE | SC_MOD_BEFOREINSERT);
        ///         break;
        ///       }
        ///       // ...
        ///     }
        ///   // ...
        ///   }
        /// </code>
        /// </example>
        /// <para>Returns TRUE.</para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// By default, Notepad++ only forwards the 5 flags/events in <see cref="ModificationFlags.NPP_DEFAULT_SC_MOD_MASK"/>.
        /// If your plugin needs to process other <see cref="ModificationFlags"/> events, you should add the required flags by sending this message
        /// <em>after</em> the <see cref="NPPN_READY"/> notification has been sent, or only when your plugin needs to listen to specific events
        /// (to avoid penalizing Notepad++'s performance). Just ensure this message is sent only once.
        /// </para>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/d888fb5f1263f5ea036c610b6980e5c4381ce7eb">8.7.7</a>
        /// </remarks>
        NPPM_ADDSCNMODIFIEDFLAGS = NPPMSG + 117,
        /// <summary>
        /// ToolBarStatusType NPPM_GETTOOLBARICONSETCHOICE(0, 0)<br/>
        /// Get the currently selected Notepad++ <a href="https://npp-user-manual.org/docs/preferences/#toolbar">toolbar icon set</a>.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: 0 (not used)</para>
        /// <para>Returns a <see cref="ToolBarStatusType"/> value.</para>
        /// </summary>
        /// <remarks>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/5406b82">8.8.2</a>
        /// </remarks>
        NPPM_GETTOOLBARICONSETCHOICE = NPPMSG + 118,

        RUNCOMMAND_USER = Constants.WM_USER + 3000,
        /// <summary>
        /// BOOL NPPM_GETXXXXXXXXXXXXXXXX(size_t strLen, wchar_t* str)<br/>
        /// where:<ul>
        /// <li><c>str</c> is the allocated <c>wchar_t</c> array</li>
        /// <li><c>strLen</c> is the allocated array size</li></ul>
        /// <para>Returns TRUE when copying to the string buffer succeeds, otherwise FALSE (when, e.g., the allocated array size is too small).</para>
        /// </summary>
        NPPM_GETFULLCURRENTPATH = RUNCOMMAND_USER + FULL_CURRENT_PATH,
        /// <summary>See <seealso cref="NPPM_GETFULLCURRENTPATH"/></summary>
        NPPM_GETCURRENTDIRECTORY = RUNCOMMAND_USER + CURRENT_DIRECTORY,
        /// <summary>See <seealso cref="NPPM_GETFULLCURRENTPATH"/></summary>
        NPPM_GETFILENAME = RUNCOMMAND_USER + FILE_NAME,
        /// <summary>See <seealso cref="NPPM_GETFULLCURRENTPATH"/></summary>
        NPPM_GETNAMEPART = RUNCOMMAND_USER + NAME_PART,
        /// <summary>See <seealso cref="NPPM_GETFULLCURRENTPATH"/></summary>
        NPPM_GETEXTPART = RUNCOMMAND_USER + EXT_PART,
        /// <summary>See <seealso cref="NPPM_GETFULLCURRENTPATH"/></summary>
        NPPM_GETCURRENTWORD = RUNCOMMAND_USER + CURRENT_WORD,
        /// <summary>See <seealso cref="NPPM_GETFULLCURRENTPATH"/></summary>
        NPPM_GETNPPDIRECTORY = RUNCOMMAND_USER + NPP_DIRECTORY,
        /// <summary>See <seealso cref="NPPM_GETFULLCURRENTPATH"/></summary>
        NPPM_GETFILENAMEATCURSOR = RUNCOMMAND_USER + GETFILENAMEATCURSOR,
        /// <summary>See <seealso cref="NPPM_GETFULLCURRENTPATH"/></summary>
        NPPM_GETCURRENTLINESTR = RUNCOMMAND_USER + CURRENT_LINESTR,
        /// <summary>
        /// INT NPPM_GETCURRENTLINE(0, 0);
        /// <para>Returns the caret's current position line.</para>
        /// </summary>
        NPPM_GETCURRENTLINE = RUNCOMMAND_USER + CURRENT_LINE,
        /// <summary>
        /// INT NPPM_GETCURRENTCOLUMN(0, 0);
        /// <para>Returns the caret's current position column.</para>
        /// </summary>
        NPPM_GETCURRENTCOLUMN = RUNCOMMAND_USER + CURRENT_COLUMN,
        /// <summary>See <seealso cref="NPPM_GETFULLCURRENTPATH"/></summary>
        NPPM_GETNPPFULLFILEPATH = RUNCOMMAND_USER + NPP_FULL_FILE_PATH,
        VAR_NOT_RECOGNIZED = 0,
        FULL_CURRENT_PATH = 1,
        CURRENT_DIRECTORY = 2,
        FILE_NAME = 3,
        NAME_PART = 4,
        EXT_PART = 5,
        CURRENT_WORD = 6,
        NPP_DIRECTORY = 7,
        CURRENT_LINE = 8,
        CURRENT_COLUMN = 9,
        NPP_FULL_FILE_PATH = 10,
        GETFILENAMEATCURSOR = 11,
        CURRENT_LINESTR = 12,

        /// <summary>Notification code.</summary>
        NPPN_FIRST = 1000,
        /// <summary>
        /// To notify plugins that all of Notepad++'s initialization routines are complete
        /// <example>
        /// <code>
        ///     scnNotification->nmhdr.code = NPPN_READY;
        ///     scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///     scnNotification->nmhdr.idFrom = 0;
        /// </code>
        /// </example>
        /// </summary>
        NPPN_READY = NPPN_FIRST + 1,

        /// <summary>
        /// To notify plugins that toolbar icons can be registered
        /// <example>
        /// <code>
        ///     scnNotification->nmhdr.code = NPPN_TB_MODIFICATION;
        ///     scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///     scnNotification->nmhdr.idFrom = 0;
        /// </code>
        /// </example>
        /// </summary>
        NPPN_TBMODIFICATION = NPPN_FIRST + 2,

        /// <summary>
        /// To notify plugins that a file is about to be closed
        /// <example>
        /// <code>
        ///     scnNotification->nmhdr.code = NPPN_FILEBEFORECLOSE;
        ///     scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///     scnNotification->nmhdr.idFrom = BufferID;
        /// </code>
        /// </example>
        /// </summary>
        NPPN_FILEBEFORECLOSE = NPPN_FIRST + 3,

        /// <summary>
        /// To notify plugins that a file was just opened
        /// <example>
        /// <code>
        ///     scnNotification->nmhdr.code = NPPN_FILEOPENED;
        ///     scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///     scnNotification->nmhdr.idFrom = BufferID;
        /// </code>
        /// </example>
        /// </summary>
        NPPN_FILEOPENED = NPPN_FIRST + 4,

        /// <summary>
        /// To notify plugins that a file was just closed
        /// <example>
        /// <code>
        ///     scnNotification->nmhdr.code = NPPN_FILECLOSED;
        ///     scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///     scnNotification->nmhdr.idFrom = BufferID;
        /// </code>
        /// </example>
        /// </summary>
        NPPN_FILECLOSED = NPPN_FIRST + 5,

        /// <summary>
        /// To notify plugins that a file is about to be opened
        /// <example>
        /// <code>
        ///     scnNotification->nmhdr.code = NPPN_FILEBEFOREOPEN;
        ///     scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///     scnNotification->nmhdr.idFrom = BufferID;
        /// </code>
        /// </example>
        /// </summary>
        NPPN_FILEBEFOREOPEN = NPPN_FIRST + 6,

        /// <summary>
        /// To notify plugins that the current file is about to be saved
        /// <example>
        /// <code>
        ///     scnNotification->nmhdr.code = NPPN_FILEBEFOREOPEN;
        ///     scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///     scnNotification->nmhdr.idFrom = BufferID;
        /// </code>
        /// </example>
        /// </summary>
        NPPN_FILEBEFORESAVE = NPPN_FIRST + 7,

        /// <summary>
        /// To notify plugins that the current file was just saved
        /// <example>
        /// <code>
        ///     scnNotification->nmhdr.code = NPPN_FILESAVED;
        ///     scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///     scnNotification->nmhdr.idFrom = BufferID;
        /// </code>
        /// </example>
        /// </summary>
        NPPN_FILESAVED = NPPN_FIRST + 8,

        /// <summary>
        /// To notify plugins that Notepad++ is about to shut down
        /// <example>
        /// <code>
        ///     scnNotification->nmhdr.code = NPPN_SHUTDOWN;
        ///     scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///     scnNotification->nmhdr.idFrom = 0;
        /// </code>
        /// </example>
        /// </summary>
        NPPN_SHUTDOWN = NPPN_FIRST + 9,

        /// <summary>
        /// To notify plugins that a buffer has been activated (i.e., put into the foreground)
        /// <example>
        /// <code>
        ///     scnNotification->nmhdr.code = NPPN_BUFFERACTIVATED;
        ///     scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///     scnNotification->nmhdr.idFrom = activatedBufferID;
        /// </code>
        /// </example>
        /// </summary>
        NPPN_BUFFERACTIVATED = NPPN_FIRST + 10,

        /// <summary>
        /// To notify plugins that the programming language of the current document was just changed
        /// <example>
        /// <code>
        ///     scnNotification->nmhdr.code = NPPN_LANGCHANGED;
        ///     scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///     scnNotification->nmhdr.idFrom = currentBufferID;
        /// </code>
        /// </example>
        /// </summary>
        NPPN_LANGCHANGED = NPPN_FIRST + 11,

        /// <summary>
        /// To notify plugins that the user has made a change in the "Style Configurator" dialog
        /// <example>
        /// <code>
        ///     scnNotification->nmhdr.code = NPPN_WORDSTYLESUPDATED;
        ///     scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///     scnNotification->nmhdr.idFrom = currentBufferID;
        /// </code>
        /// </example>
        /// </summary>
        NPPN_WORDSTYLESUPDATED = NPPN_FIRST + 12,

        /// <summary>
        /// To notify plugins that a plugin command shortcut was remapped
        /// <example>
        /// <code>
        ///     scnNotification->nmhdr.code = NPPN_SHORTCUTSREMAPPED;
        ///     scnNotification->nmhdr.hwndFrom = ShortcutKeyStructurePointer;
        ///     scnNotification->nmhdr.idFrom = cmdID;
        /// </code>
        /// where <c>ShortcutKeyStructurePointer</c> is a pointer to a C-compatible ShortcutKey structure:
        /// <code>
        ///struct ShortcutKey {
        ///		bool _isCtrl;
        ///		bool _isAlt;
        ///		bool _isShift;
        ///		UCHAR _key;
        ///};
        /// </code>
        /// </example>
        /// </summary>
        NPPN_SHORTCUTREMAPPED = NPPN_FIRST + 13,

        /// <summary>
        /// To notify plugins that a file is about to be loaded
        /// <example>
        /// <code>
        ///     scnNotification->nmhdr.code = NPPN_FILEBEFOREOPEN;
        ///     scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///     scnNotification->nmhdr.idFrom = NULL;
        /// </code>
        /// </example>
        /// </summary>
        /// <remarka>
        /// Plugins can use this notification to filter <see cref="SciMsg.SCN_MODIFIED"/>.
        /// </remarka>
        NPPN_FILEBEFORELOAD = NPPN_FIRST + 14,

        /// <summary>
        /// To notify plugins that a file-open operation failed
        /// <example>
        /// <code>
        ///     scnNotification->nmhdr.code = NPPN_FILEOPENFAILED;
        ///     scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///     scnNotification->nmhdr.idFrom = BufferID;
        /// </code>
        /// </example>
        /// </summary>
        NPPN_FILELOADFAILED = NPPN_FIRST + 15,

        /// <summary>
        /// To notify plugins that the readonly status of the current document has changed
        /// <example>
        /// <code>
        ///     scnNotification->nmhdr.code = NPPN_READONLYCHANGED;
        ///     scnNotification->nmhdr.hwndFrom = bufferID;
        ///     scnNotification->nmhdr.idFrom = docStatus;
        /// </code>
        /// where:<ul>
        /// <li><c>bufferID</c> is the document's buffer ID</li>
        /// <li><c>docStatus</c> is a combination of <see cref="DOCSTATUS_READONLY"/> and <see cref="DOCSTATUS_BUFFERDIRTY"/></li></ul>
        /// </example>
        /// </summary>
        NPPN_READONLYCHANGED = NPPN_FIRST + 16,
        DOCSTATUS_READONLY = 1,
        DOCSTATUS_BUFFERDIRTY = 2,

        /// <summary>
        /// To notify plugins that the document order has changed
        /// <example>
        /// <code>
        ///     scnNotification->nmhdr.code = NPPN_DOCORDERCHANGED;
        ///     scnNotification->nmhdr.hwndFrom = newIndex;
        ///     scnNotification->nmhdr.idFrom = BufferID;
        /// </code>
        /// </example>
        /// </summary>
        NPPN_DOCORDERCHANGED = NPPN_FIRST + 17,

        /// <summary>
        /// To notify plugins that a dirty file snapshot was loaded on startup
        /// <example>
        /// <code>
        ///     scnNotification->nmhdr.code = NPPN_SNAPSHOTDIRTYFILELOADED;
        ///     scnNotification->nmhdr.hwndFrom = NULL;
        ///     scnNotification->nmhdr.idFrom = BufferID;
        /// </code>
        /// </example>
        /// </summary>
        NPPN_SNAPSHOTDIRTYFILELOADED = NPPN_FIRST + 18,

        /// <summary>
        /// To notify plugins that an application shutdown has been triggered; no files have been closed yet
        /// <example>
        /// <code>
        ///     scnNotification->nmhdr.code = NPPN_BEFORESHUTDOWN;
        ///     scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///     scnNotification->nmhdr.idFrom = 0;
        /// </code>
        /// </example>
        /// </summary>
        NPPN_BEFORESHUTDOWN = NPPN_FIRST + 19,

        /// <summary>
        /// To notify plugins that an application shutdown has been cancelled
        /// <example>
        /// <code>
        ///     scnNotification->nmhdr.code = NPPN_CANCELSHUTDOWN;
        ///     scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///     scnNotification->nmhdr.idFrom = 0;
        /// </code>
        /// </example>
        /// </summary>
        NPPN_CANCELSHUTDOWN = NPPN_FIRST + 20,

        /// <summary>
        /// To notify plugins that a file is about to be renamed
        /// <example>
        /// <code>
        ///     scnNotification->nmhdr.code = NPPN_FILEBEFORERENAME;
        ///     scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///     scnNotification->nmhdr.idFrom = BufferID;
        /// </code>
        /// </example>
        /// </summary>
        NPPN_FILEBEFORERENAME = NPPN_FIRST + 21,

        /// <summary>
        /// To notify plugins that a file rename has been cancelled
        /// <example>
        /// <code>
        ///     scnNotification->nmhdr.code = NPPN_FILERENAMECANCEL;
        ///     scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///     scnNotification->nmhdr.idFrom = BufferID;
        /// </code>
        /// </example>
        /// </summary>
        NPPN_FILERENAMECANCEL = NPPN_FIRST + 22,

        /// <summary>
        /// To notify plugins that a file has been renamed
        /// <example>
        /// <code>
        ///     scnNotification->nmhdr.code = NPPN_FILERENAMED;
        ///     scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///     scnNotification->nmhdr.idFrom = BufferID;
        /// </code>
        /// </example>
        /// </summary>
        NPPN_FILERENAMED = NPPN_FIRST + 23,

        /// <summary>
        /// To notify plugins that a file is about to be deleted
        /// <example>
        /// <code>
        ///     scnNotification->nmhdr.code = NPPN_FILEBEFOREDELETE;
        ///     scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///     scnNotification->nmhdr.idFrom = BufferID;
        /// </code>
        /// </example>
        /// </summary>
        NPPN_FILEBEFOREDELETE = NPPN_FIRST + 24,

        /// <summary>
        /// To notify plugins that a file deletion has failed
        /// <example>
        /// <code>
        ///     scnNotification->nmhdr.code = NPPN_FILEDELETEFAILED;
        ///     scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///     scnNotification->nmhdr.idFrom = BufferID;
        /// </code>
        /// </example>
        /// </summary>
        NPPN_FILEDELETEFAILED = NPPN_FIRST + 25,

        /// <summary>
        /// To notify plugins that a file has been deleted
        /// <example>
        /// <code>
        ///     scnNotification->nmhdr.code = NPPN_FILEDELETED;
        ///     scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///     scnNotification->nmhdr.idFrom = BufferID;
        /// </code>
        /// </example>
        /// </summary>
        NPPN_FILEDELETED = NPPN_FIRST + 26,

        /// <summary>
        /// To notify plugins that dark mode was enabled/disabled
        /// <example>
        /// <code>
        /// scnNotification->nmhdr.code = NPPN_DARKMODECHANGED;
        /// scnNotification->nmhdr.hwndFrom = hwndNpp;
        /// scnNotification->nmhdr.idFrom = 0;
        /// </code>
        /// </example>
        /// </summary>
        /// <remarks>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/1eb5b10e41d7ab92b60aa32b28d4fe7739d15b53">8.4.1</a>
        /// </remarks>
        NPPN_DARKMODECHANGED = NPPN_FIRST + 27,

        /// <summary>
        /// To notify plugins that the command line argument <c>-pluginMessage="YOUR_PLUGIN_ARGUMENT"</c> is available
        /// <example>
        /// <code>
        /// scnNotification->nmhdr.code = NPPN_CMDLINEPLUGINMSG;
        /// scnNotification->nmhdr.hwndFrom = hwndNpp;
        /// scnNotification->nmhdr.idFrom = pluginMessage; //where pluginMessage is pointer of type wchar_t
        /// </code>
        /// </example>
        /// </summary>
        /// <remarks>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/0f8d5724afb0a540e8b4024252945ab60bc88c71">8.4.2</a>
        /// </remarks>
        NPPN_CMDLINEPLUGINMSG = NPPN_FIRST + 28,

        ///<summary>
        /// To notify lexer plugins that the buffer (passed in <c>idFrom</c>) was just applied to an external lexer
        /// <example>
        /// <code>
        /// scnNotification->nmhdr.code = NPPN_EXTERNALLEXERBUFFER;
        /// scnNotification->nmhdr.hwndFrom = hwndNpp;
        /// scnNotification->nmhdr.idFrom = BufferID; //where pluginMessage is pointer of type wchar_t
        /// </code>
        /// </example>
        ///</summary>
        /// <remarks>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/03a5c4795b764fa5e719092da0c37fc66ec82889">8.5</a>
        /// </remarks>
        NPPN_EXTERNALLEXERBUFFER = NPPN_FIRST + 29,

        /// <summary>
        /// To notify plugins that the current document was just modified by the "Replace All" action.
        /// <para>To solve a performance issue, since v8.6.4 Notepad++ doesn't trigger <see cref="SciMsg.SCN_MODIFIED"/> during the "Replace All" action anymore.</para>
        /// <para>As a result, plugins that monitor <see cref="SciMsg.SCN_MODIFIED"/> should also monitor <see cref="NPPN_GLOBALMODIFIED"/>.</para>
        /// <example>
        /// <code>
        /// scnNotification->nmhdr.code = NPPN_GLOBALMODIFIED;<br/>
        /// scnNotification->nmhdr.hwndFrom = BufferID;<br/>
        /// scnNotification->nmhdr.idFrom = 0; // preserved for the future use, must be zero
        /// </code>
        /// </example>
        /// </summary>
        /// <remarks>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/49e6957d486c360e05ba85ceb1c179a891831779">8.6.5</a>
        /// </remarks>
        NPPN_GLOBALMODIFIED = NPPN_FIRST + 30,

        /// <summary>
        /// To notify plugins that the current native language is just changed to another one.
        /// <para>Use <see cref="NPPM_GETNATIVELANGFILENAME"/> to get the current native language file name.</para>
        /// <para>Use <see cref="NPPM_GETMENUHANDLE"/>(<see cref="NPPPLUGINMENU"/>, 0) to get the "Plugins" submenu handle (<see cref="IntPtr"/>).</para>
        /// <example>
        /// <code>
        /// scnNotification->nmhdr.code = NPPN_NATIVELANGCHANGED;<br/>
        /// scnNotification->nmhdr.hwndFrom = hwndNpp;<br/>
        /// scnNotification->nmhdr.idFrom = 0; // preserved for the future use, must be zero
        /// </code>
        /// </example>
        /// </summary>
        /// <remarks>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/446cc980e871d04885f13055bb56acee820636c8">8.7</a>
        /// </remarks>
        NPPN_NATIVELANGCHANGED = NPPN_FIRST + 31,

        /// <summary>
        /// To notify plugins that the toolbar icon set selection has changed.
        /// <example>
        /// <code>
        /// scnNotification->nmhdr.code = NPPN_TOOLBARICONSETCHANGED;
        /// scnNotification->nmhdr.hwndFrom = hwndNpp;
        /// scnNotification->nmhdr.idFrom = iconSetChoice;
        /// // where iconSetChoice could be 1 of 5 possible values:
        /// // 0 (Fluent UI: small), 1 (Fluent UI: large), 2 (Filled Fluent UI: small), 3 (Filled Fluent UI: large) and 4 (Standard icons: small).
        /// </code>
        /// </example>
        /// </summary>
        /// <remarks>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/76b1cba609bc04127a748ff24cea6434ab00fc86">8.8.2</a>
        /// </remarks>
        NPPN_TOOLBARICONSETCHANGED = NPPN_FIRST + 32,
    }

    /// <summary>Enumerates the allowed values of the <c>whichPart</c> parameter of <see cref="NppMsg.NPPM_SETSTATUSBAR"/></summary>
    public enum StatusBarSection
    {
        DocType,
        DocSize,
        CurPos,
        EofFormat,
        UnicodeType,
        TypingMode,
    }
}
