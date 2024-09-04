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
    }

    public enum LangType
    {
        L_TEXT, L_PHP, L_C, L_CPP, L_CS, L_OBJC, L_JAVA, L_RC,
        L_HTML, L_XML, L_MAKEFILE, L_PASCAL, L_BATCH, L_INI, L_ASCII, L_USER,
        L_ASP, L_SQL, L_VB, L_JS, L_CSS, L_PERL, L_PYTHON, L_LUA,
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
        L_GOLANG, L_RAKU, L_TOML,
        // Don't use L_JS, use L_JAVASCRIPT instead
        // The end of enumerated language type, so it should be always at the end
        L_EXTERNAL
    }

    public enum WinVer
    {
        WV_UNKNOWN, WV_WIN32S, WV_95, WV_98, WV_ME, WV_NT, WV_W2K,
        WV_XP, WV_S2003, WV_XPX64, WV_VISTA, WV_WIN7, WV_WIN8, WV_WIN81, WV_WIN10, WV_WIN11
    }

    [Flags]
    public enum NppMsg : uint
    {
        NPPMSG = Constants.WM_USER + 1000,
        /// <summary>
        /// BOOL NPPM_GETCURRENTSCINTILLA(0, int* iScintillaView)<br/>
        /// Get current Scintilla view.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: <c>iScintillaView</c> could be <see cref="MAIN_VIEW"/> or <see cref="SUB_VIEW"/></para>
        /// </summary>
        /// <returns>TRUE</returns>
        NPPM_GETCURRENTSCINTILLA = NPPMSG + 4,
        /// <summary>
        /// BOOL NPPM_GETCURRENTLANGTYPE(0, int* langType)<br/>
        /// Get the programming language type from the current used document.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: langType - see <see cref="LangType"/> for all valid values</para>
        /// </summary>
        /// <returns>TRUE</returns>
        NPPM_GETCURRENTLANGTYPE = NPPMSG + 5,
        /// <summary>
        /// BOOL NPPM_SETCURRENTLANGTYPE(0, int langType)<br/>
        /// Set a new programming language type to the current used document.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: langType - see <see cref="LangType"/> for all valid values</para>
        /// </summary>
        /// <returns>TRUE</returns>
        NPPM_SETCURRENTLANGTYPE = NPPMSG + 6,
        /// <summary>
        /// int NPPM_GETNBOPENFILES(0, int iViewType)<br/>
        /// Get the number of files currently open.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: iViewType - could be <see cref="PRIMARY_VIEW"/>, <see cref="SECOND_VIEW"/> or <see cref="ALL_OPEN_FILES"/></para>
        /// </summary>
        /// <returns>the number of opened files</returns>
        NPPM_GETNBOPENFILES = NPPMSG + 7,
        /// <inheritdoc cref="NPPM_GETNBOPENFILES"/>
        ALL_OPEN_FILES = 0,
        /// <inheritdoc cref="NPPM_GETNBOPENFILES"/>
        PRIMARY_VIEW = 1,
        /// <inheritdoc cref="NPPM_GETNBOPENFILES"/>
        SECOND_VIEW = 2,
        /// <summary>
        /// BOOL NPPM_GETOPENFILENAMES(TCHAR** fileNames, int nbFileNames)<br/>
        /// Get the open files full paths of both views. User is responsible to allocate an big enough fileNames array by using <see cref="NPPM_GETNBOPENFILES"/>.
        /// <para>wParam (<see cref="UIntPtr"/>) [out]: fileNames - array of file path</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: nbFileNames is the number of file path.</para>
        /// </summary>
        /// <returns>value: The number of files copied into fileNames array</returns>
        NPPM_GETOPENFILENAMES = NPPMSG + 8,
        /// <summary>
        /// HWND NPPM_MODELESSDIALOG(int action, HWND hDlg)<br/>
        /// Register (or unregister) plugin's dialog handle.
        /// For each created dialog in your plugin, you should register it (and unregister while destroy it) to Notepad++ by using this message.
        /// If this message is ignored, then your dialog won't react with the key stroke messages such as TAB, Ctrl-C or Ctrl-V key.
        /// For the good functioning of your plugin dialog, you're recommended to not ignore this message.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: action is <see cref="MODELESSDIALOGADD"/> (for registering your hDlg) or <see cref="MODELESSDIALOGREMOVE"/> (for unregistering your hDlg)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: hDlg is the handle of dialog to register/unregister</para>
        /// </summary>
        /// <returns>hDlg (HWND) on success, NULL on failure</returns>
        NPPM_MODELESSDIALOG = NPPMSG + 12,
        /// <inheritdoc cref="NPPM_MODELESSDIALOG"/>
        MODELESSDIALOGADD = 0,
        /// <inheritdoc cref="NPPM_MODELESSDIALOG"/>
        MODELESSDIALOGREMOVE = 1,
        /// <summary>
        /// int NPPM_GETNBSESSIONFILES (BOOL* pbIsValidXML /* added in v8.6 */, TCHAR* sessionFileName)<br/>
        /// Get the number of files to load in the session sessionFileName. sessionFileName should be a full path name of an xml file.
        /// <para>wParam (<see cref="UIntPtr"/>) [out]: pbIsValidXML, if the lParam pointer is null, then this parameter will be ignored. TRUE if XML is valid, otherwise FALSE.</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: sessionFileName is XML session full path</para>
        /// </summary>
        /// <returns>value: The number of files in XML session file</returns>
        /// <remarks>
        /// Changed in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/72c5175b33cb3a06d0ece2a7700295b0c9647140">8.6</a>
        /// </remarks>
        NPPM_GETNBSESSIONFILES = NPPMSG + 13,
        /// <summary>
        /// NPPM_GETSESSIONFILES (TCHAR** sessionFileArray, TCHAR* sessionFileName)<br/>
        /// the files' full path name from a session file.
        /// <para>wParam (<see cref="UIntPtr"/>) [out]: sessionFileArray is the array in which the files' full path of the same group are written. To allocate the array with the proper size, send <see cref="NPPM_GETNBSESSIONFILES"/>.</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: sessionFileName is XML session full path</para>
        /// </summary>
        /// <returns>FALSE on failure, TRUE on success</returns>
        NPPM_GETSESSIONFILES = NPPMSG + 14,
        /// <summary>
        /// NPPM_SAVESESSION(0, sessionInfo* si)<br/>
        /// Creates an session file for a defined set of files.
        /// Unlike <see cref="NPPM_SAVECURRENTSESSION"/>, which saves the current opened files, this call can be used to freely define any file which should be part of a session.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: si is a pointer to a <see cref="SessionInfo"/> structure</para>
        /// </summary>
        /// <returns>sessionFileName on success, NULL otherwise</returns>
        NPPM_SAVESESSION = NPPMSG + 15,
        /// <summary>
        /// TCHAR* NPPM_SAVECURRENTSESSION(0, TCHAR* sessionFileName)<br/>
        /// Saves the current opened files in Notepad++ as a group of files (session) as an xml file.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: sessionFileName is the xml full path name</para>
        /// </summary>
        /// <returns>sessionFileName on success, NULL otherwise</returns>
        NPPM_SAVECURRENTSESSION = NPPMSG + 16,
        /// <summary>
        /// BOOL NPPM_GETOPENFILENAMESPRIMARY(TCHAR** fileNames, int nbFileNames)<br/>
        /// Get the open files full paths of main view. User is responsible to allocate an big enough fileNames array by using <see cref="NPPM_GETNBOPENFILES"/>.
        /// <para>wParam (<see cref="UIntPtr"/>) [out]: fileNames - array of file path</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: nbFileNames is the number of file path.</para>
        /// </summary>
        /// <returns>value: The number of files copied into fileNames array</returns>
        NPPM_GETOPENFILENAMESPRIMARY = NPPMSG + 17,
        /// <summary>
        /// BOOL NPPM_GETOPENFILENAMESSECOND(TCHAR** fileNames, int nbFileNames)<br/>
        /// Get the open files full paths of sub-view. User is responsible to allocate an big enough fileNames array by using <see cref="NPPM_GETNBOPENFILES"/>.
        /// <para>wParam (<see cref="UIntPtr"/>) [out]: fileNames - array of file path</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: nbFileNames is the number of file path.</para>
        /// </summary>
        /// <returns>value: The number of files copied into fileNames array</returns>
        NPPM_GETOPENFILENAMESSECOND = NPPMSG + 18,
        /// <summary>
        /// HWND NPPM_CREATESCINTILLAHANDLE(0, HWND hParent)<br/>
        /// A plugin can create a Scintilla for its usage by sending this message to Notepad++.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: hParent - If set (non NULL), it will be the parent window of this created Scintilla handle, otherwise the parent window is Notepad++</para>
        /// </summary>
        /// <returns>the handle of created Scintilla handle</returns>
        NPPM_CREATESCINTILLAHANDLE = NPPMSG + 20,
        /// <summary>
        /// BOOL NPPM_DESTROYSCINTILLAHANDLE_DEPRECATED(0, HWND hScintilla) - DEPRECATED: It is kept for the compatibility.
        /// Notepad++ will deallocate every createed Scintilla control on exit, this message returns TRUE but does nothing.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: hScintilla is Scintilla handle</para>
        /// </summary>
        /// <returns>TRUE</returns>
        NPPM_DESTROYSCINTILLAHANDLE_DEPRECATED = NPPMSG + 21,
        /// <inheritdoc cref="NPPM_DESTROYSCINTILLAHANDLE_DEPRECATED"/>
        [Obsolete("Replaced by NPPM_DESTROYSCINTILLAHANDLE_DEPRECATED", true)]
        NPPM_DESTROYSCINTILLAHANDLE = NPPM_DESTROYSCINTILLAHANDLE_DEPRECATED,
        /// <summary>
        /// int NPPM_GETNBUSERLANG(0, int* udlID)<br/>
        /// Get the number of user defined languages and, optionally, the starting menu id.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: <c>udlID</c> is optional, if not used set it to 0, otherwise an integer pointer is needed to retrieve the menu identifier.</para>
        /// </summary>
        /// <returns>the number of user defined languages identified</returns>
        NPPM_GETNBUSERLANG = NPPMSG + 22,
        /// <summary>
        /// int NPPM_GETCURRENTDOCINDEX(0, int inView)<br/>
        /// Get the current index of the given view.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: inView, should be <see cref="MAIN_VIEW"/>) or <see cref="SUB_VIEW"/></para>
        /// </summary>
        /// <returns>-1 if the view is invisible (hidden), otherwise is the current index.</returns>
        NPPM_GETCURRENTDOCINDEX = NPPMSG + 23,
        MAIN_VIEW = 0,
        SUB_VIEW = 1,
        /// <summary>
        /// BOOL NPPM_SETSTATUSBAR(int whichPart, TCHAR *str2set)<br/>
        /// Set string in the specified field of a statusbar.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>whichPart</c> for indicating the statusbar part you want to set. It can be only the above value (0 - 5)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>str2set</c> is the string you want to write to the part of statusbar.</para>
        /// <returns>FALSE on failure, TRUE on success</returns>
        /// </summary>
        NPPM_SETSTATUSBAR = NPPMSG + 24,
        /// <inheritdoc cref="NPPM_SETSTATUSBAR"/>
        STATUSBAR_DOC_TYPE = 0,
        /// <inheritdoc cref="NPPM_SETSTATUSBAR"/>
        STATUSBAR_DOC_SIZE = 1,
        /// <inheritdoc cref="NPPM_SETSTATUSBAR"/>
        STATUSBAR_CUR_POS = 2,
        /// <inheritdoc cref="NPPM_SETSTATUSBAR"/>
        STATUSBAR_EOF_FORMAT = 3,
        /// <inheritdoc cref="NPPM_SETSTATUSBAR"/>
        STATUSBAR_UNICODE_TYPE = 4,
        /// <inheritdoc cref="NPPM_SETSTATUSBAR"/>
        STATUSBAR_TYPING_MODE = 5,
        /// <summary>
        /// int NPPM_GETMENUHANDLE(int menuChoice, 0)<br/>
        /// Get menu handle (HMENU) of choice.
        /// <para>wParam (<see cref="UIntPtr"/>)b[in]: <c>menuChoice</c> could be main menu (<see cref="NPPMAINMENU"/>) or Plugin menu (<see cref="NPPPLUGINMENU"/>)</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// </summary>
        /// <returns>menu handle (HMENU) of choice (plugin menu handle or Notepad++ main menu handle)</returns>
        NPPM_GETMENUHANDLE = NPPMSG + 25,
        /// <inheritdoc cref="NPPM_GETMENUHANDLE"/>
        NPPPLUGINMENU = 0,
        /// <inheritdoc cref="NPPM_GETMENUHANDLE"/>
        NPPMAINMENU = 1,
        /// <summary>
        /// int NPPM_ENCODESCI(int inView, 0)<br/>
        /// Changes current buffer in view to UTF-8.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>inView</c> - <see cref="MAIN_VIEW"/> (0) or <see cref="SUB_VIEW"/> (1)</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// 0: ANSI<br/>
        /// 1: UTF-8 with BOM<br/>
        /// 2: UTF-16 Big Ending with BOM<br/>
        /// 3: UTF-16 Little Ending with BOM<br/>
        /// 4: UTF-8 without BOM<br/>
        /// 5: uni7Bit<br/>
        /// 6: UTF-16 Big Ending without BOM<br/>
        /// 7: UTF-16 Little Ending without BOM
        /// </summary>
        /// <returns>new UniMode, with the following value:</returns>
        NPPM_ENCODESCI = NPPMSG + 26,
        /// <summary>
        /// int NPPM_DECODESCI(int inView, 0)<br/>
        /// Changes current buffer in view to ANSI.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>inView</c> - <see cref="MAIN_VIEW"/> (0) or <see cref="SUB_VIEW"/> (1)</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// </summary>
        /// <returns>old UniMode - see <see cref="NPPM_ENCODESCI"/></returns>
        NPPM_DECODESCI = NPPMSG + 27,
        /// <summary>
        /// BOOL NPPM_ACTIVATEDOC(int inView, int index2Activate)<br/>
        /// Switch to the document by the given view and index.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>inView</c> - <see cref="MAIN_VIEW"/> (0) or <see cref="SUB_VIEW"/> (1)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>index2Activate</c> - index (in the view indicated above) where is the document to be activated</para>
        /// </summary>
        /// <returns>TRUE</returns>
        NPPM_ACTIVATEDOC = NPPMSG + 28,
        /// <summary>
        /// BOOL NPPM_LAUNCHFINDINFILESDLG(TCHAR * dir2Search, TCHAR * filter)<br/>
        /// Launch Find in Files dialog and set "Find in" directory and filters with the given arguments.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: if <c>dir2Search</c> is not NULL, it will be set as working directory in which Notepad++ will search</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: if filter is not NULL, filter string will be set into filter field</para>
        /// </summary>
        /// <returns>TRUE</returns>
        NPPM_LAUNCHFINDINFILESDLG = NPPMSG + 29,
        /// <summary>
        /// BOOL NPPM_DMMSHOW(0, HWND hDlg)<br/>
        /// Show the dialog which was previously registered by <see cref="NPPM_DMMREGASDCKDLG"/>.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: hDlg is the handle of dialog to show</para>
        /// </summary>
        /// <returns>TRUE</returns>
        NPPM_DMMSHOW = NPPMSG + 30,
        /// <summary>
        /// BOOL NPPM_DMMHIDE(0, HWND hDlg)<br/>
        /// Hide the dialog which was previously regeistered by <see cref="NPPM_DMMREGASDCKDLG"/>.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: hDlg is the handle of dialog to hide</para>
        /// </summary>
        /// <returns>TRUE</returns>
        NPPM_DMMHIDE = NPPMSG + 31,
        /// <summary>
        /// BOOL NPPM_DMMUPDATEDISPINFO(0, HWND hDlg)<br/>
        /// Redraw the dialog.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: hDlg is the handle of dialog to redraw</para>
        /// </summary>
        /// <returns>TRUE</returns>
        NPPM_DMMUPDATEDISPINFO = NPPMSG + 32,
        /// <summary>
        /// BOOL NPPM_DMMREGASDCKDLG(0, tTbData* pData)<br/>
        /// Pass the necessary dockingData to Notepad++ in order to make your dialog dockable.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: pData is the pointer of tTbData. Please check the <see cref="NppTbData"/> structure</para>
        ///             Minimum information which needs to be filled out are hClient, pszName, dlgID, uMask and pszModuleName.
        ///             Notice that <see cref="NppTbData.RcFloat"/> and <see cref="NppTbData.IPrevCont"/>  shouldn't be filled. They are used internally.
        /// </summary>
        /// <returns>TRUE</returns>
        NPPM_DMMREGASDCKDLG = NPPMSG + 33,
        /// <summary>
        /// BOOL NPPM_LOADSESSION(0, TCHAR* sessionFileName)<br/>
        /// Open all files of same session in Notepad++ via a xml format session file sessionFileName.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: sessionFileName is the full file path of session file to reload</para>
        /// </summary>
        /// <returns>TRUE</returns>
        NPPM_LOADSESSION = NPPMSG + 34,
        /// <summary>
        /// BOOL WM_DMM_VIEWOTHERTAB(0, TCHAR* name)<br/>
        /// Show the plugin dialog (switch to plugin tab) with the given name.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: name should be the same value as previously used to register the dialog (pszName of tTbData)</para>
        /// </summary>
        /// <returns>TRUE</returns>
        NPPM_DMMVIEWOTHERTAB = NPPMSG + 35,
        /// <summary>
        /// BOOL NPPM_RELOADFILE(BOOL withAlert, TCHAR *filePathName2Reload)<br/>
        /// Reload the document which matches with the given filePathName2Reload.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: filePathName2Reload is the full file path of document to reload</para>
        /// <returns>TRUE</returns> if reloading file succeeds, otherwise FALSE
        /// </summary>
        NPPM_RELOADFILE = NPPMSG + 36,
        /// <summary>
        /// BOOL NPPM_SWITCHTOFILE(0, TCHAR* filePathName2switch)<br/>
        /// Switch to the document which matches with the given filePathName2switch.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: filePathName2switch is the full file path of document to switch</para>
        /// </summary>
        /// <returns>TRUE</returns>
        NPPM_SWITCHTOFILE = NPPMSG + 37,
        /// <summary>
        /// BOOL NPPM_SAVECURRENTFILE(0, 0)<br/>
        /// Save current activated document.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// <returns>TRUE</returns> if file is saved, otherwise FALSE (the file doesn't need to be saved, or other reasons).
        /// </summary>
        NPPM_SAVECURRENTFILE = NPPMSG + 38,
        /// <summary>
        /// BOOL NPPM_SAVEALLFILES(0, 0)<br/>
        /// Save all opened document.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// <returns>FALSE when no file needs to be saved, else TRUE if there is at least one file saved.</returns>
        /// </summary>
        NPPM_SAVEALLFILES = NPPMSG + 39,
        /// <summary>
        /// BOOL NPPM_SETMENUITEMCHECK(UINT pluginCmdID, BOOL doCheck)<br/>
        /// Set or remove the check on a item of plugin menu and tool bar (if any).
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>pluginCmdID</c> is the plugin command ID which corresponds to the menu item: <see cref="FuncItem.CmdID"/></para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: if <c>doCheck</c> value is TRUE, item will be checked, FALSE makes item unchecked.</para>
        /// </summary>
        /// <returns>TRUE</returns>
        NPPM_SETMENUITEMCHECK = NPPMSG + 40,
        /// <summary>
        /// BOOL NPPM_ADDTOOLBARICON_DEPRECATED(UINT pluginCmdID, toolbarIcons* iconHandles) -- DEPRECATED: use NPPM_ADDTOOLBARICON_FORDARKMODE instead
        /// Add an icon to the toolbar.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>pluginCmdID</c> is the plugin command ID which corresponds to the menu item: <see cref="FuncItem.CmdID"/></para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>iconHandles</c> of a <see cref="ToolbarIcon"/> structure. 2 formats ".ico" and ".bmp" are needed</para>
        ///             Both handles should be set so the icon will be displayed correctly if toolbar icon sets are changed by users
        /// </summary>
        /// <returns>TRUE</returns>
        NPPM_ADDTOOLBARICON_DEPRECATED = NPPMSG + 41,
        /// <inheritdoc cref="NPPM_ADDTOOLBARICON_DEPRECATED"/>
        [Obsolete("Replaced by NPPM_ADDTOOLBARICON_DEPRECATED", true)]
        NPPM_ADDTOOLBARICON = NPPM_ADDTOOLBARICON_DEPRECATED,
        /// <summary>
        /// winVer NPPM_GETWINDOWSVERSION(0, 0)<br/>
        /// Get OS (Windows) version.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// </summary>
        /// <returns>a <see cref="WinVer"/></returns>
        NPPM_GETWINDOWSVERSION = NPPMSG + 42,
        /// <summary>
        /// HWND NPPM_DMMGETPLUGINHWNDBYNAME(const TCHAR *windowName, const TCHAR *moduleName)<br/>
        /// Retrieve the dialog handle corresponds to the windowName and moduleName. You may need this message if you want to communicate with another plugin "dockable" dialog.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: windowName - if windowName is NULL, then the first found window handle which matches with the moduleName will be returned</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in] : moduleName - if moduleName is NULL, then <returns>value is NULL</returns></para>
        /// <returns>NULL if moduleName is NULL. If windowName is NULL, then the first found window handle which matches with the moduleName will be returned.</returns>
        /// </summary>
        NPPM_DMMGETPLUGINHWNDBYNAME = NPPMSG + 43,
        /// <summary>
        /// BOOL NPPM_MAKECURRENTBUFFERDIRTY(0, 0)<br/>
        /// Make the current document dirty, aka set the save state to unsaved.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// </summary>
        /// <returns>TRUE</returns>
        NPPM_MAKECURRENTBUFFERDIRTY = NPPMSG + 44,
        /// <summary>
        /// THEMEAPI NPPM_GETENABLETHEMETEXTUREFUNC(0, 0) -- DEPRECATED: plugin can use EnableThemeDialogTexture directly from uxtheme.h instead
        /// Get "EnableThemeDialogTexture" function address.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// <returns>a proc address or NULL</returns>
        /// </summary>
        /// <remarks>
        /// Deprecated since <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/50e95d22675b86b6afeacb6fdc217a0011f9528e">8.4.9</a>
        /// </remarks>
        [Obsolete("Use EnableThemeDialogTexture directly (uxtheme.h) instead", false)]
        NPPM_GETENABLETHEMETEXTUREFUNC = NPPMSG + 45,
        /// <summary>
        /// int NPPM_GETPLUGINSCONFIGDIR(int strLen, TCHAR *str)<br/>
        /// Get user's plugin config directory path. It's useful if plugins want to save/load parameters for the current user
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: strLen is length of  allocated buffer in which directory path is copied</para>
        /// <para>lParam (<see cref="IntPtr"/>) [out] : str is the allocated buffere. User should call this message twice -</para>
        ///               The 1st call with "str" be NULL to get the required number of TCHAR (not including the terminating nul character)<br/>
        ///               The 2nd call to allocate "str" buffer with the 1st call's <returns>value + 1, then call it again to get the path</returns>
        /// <returns>value: The 1st call - the number of TCHAR to copy.</returns>
        ///               The 2nd call - FALSE on failure, TRUE on success
        /// </summary>
        NPPM_GETPLUGINSCONFIGDIR = NPPMSG + 46,
        /// <summary>
        /// BOOL NPPM_MSGTOPLUGIN(TCHAR *destModuleName, CommunicationInfo *info)<br/>
        /// Send a private information to a plugin with given plugin name. This message allows the communication between 2 plugins.
        /// For example, plugin X can execute a command of plugin Y if plugin X knows the command ID and the file name of plugin Y.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: destModuleName is the destination complete module file name (with the file extension ".dll")</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: info - pointer to a <see cref="CommunicationInfo"/> structure</para>
        /// </summary>
        /// <returns>
        /// TRUE if Notepad++ found the plugin by its module name (destModuleName), and pass the info (communicationInfo) to the module.<br/>
        /// FALSE if no plugin with such name is found.
        /// </returns>
        NPPM_MSGTOPLUGIN = NPPMSG + 47,
        /// <summary>
        /// BOOL NPPM_MENUCOMMAND(0, int cmdID)<br/>
        /// Run Notepad++ command with the given command ID.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: cmdID - See <see cref="MenuCmdId"/>for all the Notepad++ menu command items</para>
        /// </summary>
        /// <returns>TRUE</returns>
        NPPM_MENUCOMMAND = NPPMSG + 48,
        /// <summary>
        /// BOOL NPPM_TRIGGERTABBARCONTEXTMENU(int inView, int index2Activate)<br/>
        /// Switch to the document by the given view and index and trigger the context menu
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>inView</c> - <see cref="MAIN_VIEW"/> or <see cref="SUB_VIEW"/> </para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>index2Activate</c> - index (in the view indicated above) where is the document to have the context menu</para>
        /// </summary>
        /// <returns>TRUE</returns>
        NPPM_TRIGGERTABBARCONTEXTMENU = NPPMSG + 49,
        /// <summary>
        /// int NPPM_GETNPPVERSION(BOOL ADD_ZERO_PADDING, 0)<br/>
        /// Get Notepad++ version.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: ADD_ZERO_PADDING (see below)</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// <returns>value:</returns>
        /// HIWORD(returned_value) is major part of version: the 1st number
        /// LOWORD(returned_value) is minor part of version: the 3 last numbers
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
        /// Hide (or show) tab bar.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: if hideOrNot is set as TRUE then tab bar will be hidden, otherwise it'll be shown.</para>
        /// </summary>
        /// <returns>value: the old status value</returns>
        NPPM_HIDETABBAR = NPPMSG + 51,
        /// <summary>
        /// BOOL NPPM_ISTABBARHIDDEN(0, 0)<br/>
        /// Get tab bar status.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// </summary>
        /// <returns>value: TRUE if tool bar is hidden, otherwise FALSE</returns>
        NPPM_ISTABBARHIDDEN = NPPMSG + 52,
        /// <summary>
        /// int NPPM_GETPOSFROMBUFFERID(UINT_PTR bufferID, int priorityView)<br/>
        /// Get document position (VIEW and INDEX) from a buffer ID, according priorityView.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: BufferID of document</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: priorityView is the target VIEW. However, if the given bufferID cannot be found in the target VIEW, the other VIEW will be searched.</para>
        ///
        /// VIEW takes 2 highest bits and INDEX (0 based) takes the rest (30 bits).<br/><br/>
        /// Here's the values for the view:
        /// <code>
        ///     MAIN_VIEW 0
        ///     SUB_VIEW  1
        /// </code>
        /// If priorityView set to SUB_VIEW, then SUB_VIEW will be search firstly
        /// </summary>
        /// <returns>-1 if the bufferID non existing, else return value contains VIEW and INDEX:</returns>///
        NPPM_GETPOSFROMBUFFERID = NPPMSG + 57,
        /// <summary>
        /// int NPPM_GETFULLPATHFROMBUFFERID(UINT_PTR bufferID, TCHAR* fullFilePath)<br/>
        /// Get full path file name from a bufferID (the pointer of buffer).
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>bufferID</c></para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: <c>fullFilePath</c> - User should call it with fullFilePath be NULL to get the number of TCHAR (not including the nul character),</para>
        ///         allocate fullFilePath with the <returns>values + 1, then call it again to get full path file name</returns>
        /// </summary>
        /// <returns>-1 if the bufferID non existing, otherwise the number of TCHAR copied/to copy</returns>
        NPPM_GETFULLPATHFROMBUFFERID = NPPMSG + 58,
        /// <summary>
        /// UINT_PTR NPPM_GETBUFFERIDFROMPOS(int index, int iView)<br/>
        /// Get the document bufferID from the given position (iView and index).
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>index</c> (0 based) of document</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>iView</c> <see cref="MAIN_VIEW"/> or <see cref="SUB_VIEW"/></para>
        /// </summary>
        /// <returns>NULL if invalid, otherwise bufferID</returns>
        NPPM_GETBUFFERIDFROMPOS = NPPMSG + 59,
        /// <summary>
        /// UINT_PTR NPPM_GETCURRENTBUFFERID(0, 0)<br/>
        /// Get active document BufferID.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// </summary>
        /// <returns>active document BufferID</returns>
        NPPM_GETCURRENTBUFFERID = NPPMSG + 60,
        /// <summary>
        /// BOOL NPPM_RELOADBUFFERID(UINT_PTR bufferID, BOOL alert)<br/>
        /// Reloads document with the given BufferID
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: BufferID of document to reload</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: set TRUE to let user confirm or reject the reload; setting FALSE will reload with no alert.</para>
        /// </summary>
        /// <returns>TRUE on success, FALSE otherwise</returns>
        NPPM_RELOADBUFFERID = NPPMSG + 61,
        /// <summary>
        /// int NPPM_GETBUFFERLANGTYPE(UINT_PTR bufferID, 0)<br/>
        /// Retrieves the language type of the document with the given bufferID.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: BufferID of document to get LangType from</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// </summary>
        /// <returns>as int, see LangType. -1 on error</returns>
        NPPM_GETBUFFERLANGTYPE = NPPMSG + 64,
        /// <summary>
        /// BOOL NPPM_SETBUFFERLANGTYPE(UINT_PTR bufferID, int langType)<br/>
        /// Set the language type of the document based on the given bufferID.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: BufferID to set LangType of</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: langType as int, <see cref="LangType"/> for valid values (<see cref="LangType.L_USER"/> and <see cref="LangType.L_EXTERNAL"/> are not supported)</para>
        /// </summary>
        /// <returns>TRUE on success, FALSE otherwise</returns>
        NPPM_SETBUFFERLANGTYPE = NPPMSG + 65,
        /// <summary>
        /// int NPPM_GETBUFFERENCODING(UINT_PTR bufferID, 0)<br/>
        /// Get encoding from the document with the given bufferID
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: BufferID to get encoding from</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// 0: ANSI<br/>
        /// 1: UTF-8 with BOM<br/>
        /// 2: UTF-16 Big Ending with BOM<br/>
        /// 3: UTF-16 Little Ending with BOM<br/>
        /// 4: UTF-8 without BOM<br/>
        /// 5: uni7Bit<br/>
        /// 6: UTF-16 Big Ending without BOM<br/>
        /// 7: UTF-16 Little Ending without BOM
        /// </summary>
        /// <returns>-1 on error, otherwise UniMode, with the following value:</returns>
        NPPM_GETBUFFERENCODING = NPPMSG + 66,
        /// <summary>
        /// BOOL NPPM_SETBUFFERENCODING(UINT_PTR bufferID, int encoding)<br/>
        /// Set encoding to the document with the given bufferID
        /// <para>wParam (<see cref="UIntPtr"/>): BufferID to set encoding of</para>
        /// <para>lParam (<see cref="IntPtr"/>) : encoding, see UniMode value in <see cref="NPPM_GETBUFFERENCODING"/></para>
        /// Can only be done on new, unedited files
        /// </summary>
        /// <returns>TRUE on success, FALSE otherwise</returns>
        NPPM_SETBUFFERENCODING = NPPMSG + 67,
        /// <summary>
        /// int NPPM_GETBUFFERFORMAT(UINT_PTR bufferID, 0)<br/>
        /// Get the EOL format of the document with given bufferID.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: BufferID to get EolType format from</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// </summary>
        /// <returns>
        /// -1 on error, otherwise EolType format:<br/><br/>
        ///  0: Windows (CRLF)<br/>
        ///  1: Macos (CR)<br/>
        ///  2: Unix (LF)<br/>
        ///  3. Unknown
        ///  </returns>
        NPPM_GETBUFFERFORMAT = NPPMSG + 68,
        /// <summary>
        /// BOOL NPPM_SETBUFFERFORMAT(UINT_PTR bufferID, int format)<br/>
        /// Set the EOL format to the document with given bufferID.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: BufferID to set EolType format of</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: EolType format. For EolType format value, see <see cref="NPPM_GETBUFFERFORMAT"/> (above)</para>
        /// </summary>
        /// <returns>TRUE on success, FALSE otherwise</returns>
        NPPM_SETBUFFERFORMAT = NPPMSG + 69,
        /// <summary>
        /// BOOL NPPM_HIDETOOLBAR(0, BOOL hideOrNot)<br/>
        /// Hide (or show) the toolbar.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: if <c>hideOrNot</c> is set as TRUE then tool bar will be hidden, otherwise it'll be shown.</para>
        /// </summary>
        /// <returns>value: the old status value</returns>
        NPPM_HIDETOOLBAR = NPPMSG + 70,
        /// <summary>
        /// BOOL NPPM_ISTOOLBARHIDDEN(0, 0)<br/>
        /// Get toolbar status.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// </summary>
        /// <returns>value: TRUE if tool bar is hidden, otherwise FALSE</returns>
        NPPM_ISTOOLBARHIDDEN = NPPMSG + 71,
        /// <summary>
        /// BOOL NPPM_HIDEMENU(0, BOOL hideOrNot)<br/>
        /// Hide (or show) menu bar.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: if <c>hideOrNot</c> is set as TRUE then menu will be hidden, otherwise it'll be shown.</para>
        /// </summary>
        /// <returns>value: the old status value</returns>
        NPPM_HIDEMENU = NPPMSG + 72,
        /// <summary>
        /// BOOL NPPM_ISMENUHIDDEN(0, 0)<br/>
        /// Get menu bar status.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// </summary>
        /// <returns>value: TRUE if menu bar is hidden, otherwise FALSE</returns>
        NPPM_ISMENUHIDDEN = NPPMSG + 73,
        /// <summary>
        /// BOOL NPPM_HIDESTATUSBAR(0, BOOL hideOrNot)<br/>
        /// Hide (or show) status bar.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: if <c>hideOrNot</c> is set as TRUE then status bar will be hidden, otherwise it'll be shown.</para>
        /// </summary>
        /// <returns>value: the old status value</returns>
        NPPM_HIDESTATUSBAR = NPPMSG + 74,
        /// <summary>
        /// BOOL NPPM_ISSTATUSBARHIDDEN(0, 0)<br/>
        /// Get status bar status.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// </summary>
        /// <returns>value: TRUE if status bar is hidden, otherwise FALSE</returns>
        NPPM_ISSTATUSBARHIDDEN = NPPMSG + 75,
        /// <summary>
        /// BOOL NPPM_GETSHORTCUTBYCMDID(int cmdID, ShortcutKey* sk)<br/>
        /// Get your plugin command current mapped shortcut into sk via cmdID.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: cmdID is your plugin command ID</para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: sk is a pointer to a <see cref="ShortcutKey"/> strcture which will receive the requested CMD shortcut. It should be allocated in the plugin before being used.</para>
        /// You may need it after getting <see cref="NPPN_READY"/> notification.
        /// </summary>
        /// <returns>TRUE if this function call is successful and shortcut is enable, otherwise FALSE</returns>
        NPPM_GETSHORTCUTBYCMDID = NPPMSG + 76,
        /// <summary>
        /// BOOL NPPM_DOOPEN(0, const TCHAR* fullPathName2Open)<br/>
        /// Open a file with given <c>fullPathName2Open</c>.
        /// If <c>fullPathName2Open</c> has been already opened in Notepad++, the it will be activated and becomes the current document.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>fullPathName2Open</c> indicates the full file path name to be opened</para>
        /// </summary>
        /// <returns>TRUE if the operation is successful, otherwise FALSE</returns>
        NPPM_DOOPEN = NPPMSG + 77,
        /// <summary>
        /// BOOL NPPM_SAVECURRENTFILEAS (BOOL saveAsCopy, const TCHAR* filename)<br/>
        /// Save the current activated document.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: saveAsCopy must be either FALSE to save, or TRUE to save a copy of the current filename ("Save a Copy As..." action)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: filename indicates the full file path name to be saved</para>
        /// </summary>
        /// <returns>TRUE if the operation is successful, otherwise FALSE</returns>
        NPPM_SAVECURRENTFILEAS = NPPMSG + 78,
        /// <summary>
        /// int NPPM_GETCURRENTNATIVELANGENCODING(0, 0)<br/>
        /// Get the code page associated with the current localisation of Notepad++.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// </summary>
        /// <returns>the current native language encoding</returns>
        NPPM_GETCURRENTNATIVELANGENCODING = NPPMSG + 79,
        /// <summary>
        /// DEPRECATED: the message has been made (since 2010 AD) for checking if NPPM_ALLOCATECMDID is supported. This message is no more needed.
        /// BOOL NPPM_ALLOCATESUPPORTED_DEPRECATED(0, 0)<br/>
        /// Get NPPM_ALLOCATECMDID supported status. Use to identify if subclassing is necessary
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// </summary>
        /// <returns>TRUE if NPPM_ALLOCATECMDID is supported</returns>
        NPPM_ALLOCATESUPPORTED_DEPRECATED = NPPMSG + 80,
        /// <inheritdoc cref="NPPM_ALLOCATESUPPORTED_DEPRECATED"/>
        [Obsolete("Replaced by NPPM_ALLOCATESUPPORTED_DEPRECATED", true)]
        NPPM_ALLOCATESUPPORTED = NPPM_ALLOCATESUPPORTED_DEPRECATED,
        /// <summary>
        /// BOOL NPPM_ALLOCATECMDID(int numberRequested, int* startNumber)<br/>
        /// Obtain a number of consecutive menu item IDs for creating menus dynamically, with the guarantee of these IDs not clashing with any other plugins.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: numberRequested is the number of ID you request for the reservation</para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: startNumber will be set to the initial command ID if successful</para>
        /// <example>
        /// Example: If a plugin needs 4 menu item ID, the following code can be used:
        /// <code>
        ///     int idBegin;
        ///     BOOL isAllocatedSuccessful = ::SendMessage(nppData._nppHandle, NPPM_ALLOCATECMDID, 4, &amp;idBegin);
        /// </code>
        /// </example>
        /// If isAllocatedSuccessful is TRUE, and value of idBegin is 46581,
        /// then menu iten ID 46581, 46582, 46583 and 46584 are preserved by Notepad++, and they are safe to be used by the plugin.
        /// </summary>
        /// <returns>TRUE if successful, FALSE otherwise. startNumber will also be set to 0 if unsuccessful</returns>
        NPPM_ALLOCATECMDID = NPPMSG + 81,
        /// <summary>
        /// BOOL NPPM_ALLOCATEMARKER(int numberRequested, int* startNumber)<br/>
        /// Allocate a number of consecutive marker IDs to a plugin: if a plugin need to add a marker on Notepad++'s Scintilla marker margin,
        /// it has to use this message to get marker number, in order to prevent from the conflict with the other plugins.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: numberRequested is the number of ID you request for the reservation</para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: startNumber will be set to the initial command ID if successful</para>
        /// <example>
        /// Example: If a plugin needs 3 marker ID, the following code can be used:
        /// <code>
        ///     int idBegin;
        ///     BOOL isAllocatedSuccessful = ::SendMessage(nppData._nppHandle, NPPM_ALLOCATEMARKER, 3, &amp;idBegin);
        /// </code>
        /// </example>
        /// if isAllocatedSuccessful is TRUE, and value of idBegin is 16
        /// then marker ID 16, 17 and 18 are preserved by Notepad++, and they are safe to be used by the plugin.
        /// </summary>
        /// <returns>TRUE if successful, FALSE otherwise. startNumber will also be set to 0 if unsuccessful</returns>
        NPPM_ALLOCATEMARKER = NPPMSG + 82,
        /// <summary>
        /// int NPPM_GETLANGUAGENAME(LangType langType, TCHAR* langName)<br/>
        /// Get programming language name from the given language type (<see cref="LangType"/> enum).
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: langType is the number of <see cref="LangType"/></para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: langName is the buffer to receive the language name string</para>
        /// You should call this function 2 times - the first time you pass langName as NULL to get the number of characters to copy.
        /// You allocate a buffer of the length of (the number of characters + 1) then send <see cref="NPPM_GETLANGUAGENAME"/> the 2nd time
        /// by passing allocated buffer as argument langName
        /// </summary>
        /// <returns>value is the number of copied character / number of character to copy (0 is not included)</returns>
        NPPM_GETLANGUAGENAME = NPPMSG + 83,
        /// <summary>
        /// INT NPPM_GETLANGUAGEDESC(int langType, TCHAR *langDesc)<br/>
        /// Get programming language short description from the given language type (<see cref="LangType"/> enum).
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: langType is the number of <see cref="LangType"/></para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: langDesc is the buffer to receive the language description string</para>
        ///
        /// You should call this function 2 times - the first time you pass langDesc as NULL to get the number of characters to copy.
        /// You allocate a buffer of the length of (the number of characters + 1) then send <see cref="NPPM_GETLANGUAGENAME"/> the 2nd time
        /// by passing allocated buffer as argument langDesc
        /// </summary>
        /// <returns>value is the number of copied character / number of character to copy (\0 is not included)</returns>
        NPPM_GETLANGUAGEDESC = NPPMSG + 84,
        /// <summary>
        /// BOOL NPPM_SHOWDOCLIST(0, BOOL toShowOrNot)<br/>
        /// Show or hide the Document List panel.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>toShowOrNot</c> - if toShowOrNot is TRUE, the Document List panel is shown otherwise it is hidden.</para>
        /// </summary>
        /// <returns>TRUE</returns>
        NPPM_SHOWDOCLIST = NPPMSG + 85,
        /// <summary>
        /// BOOL NPPM_ISDOCLISTSHOWN(0, 0)<br/>
        /// Checks the visibility of the Document List panel.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// </summary>
        /// <returns>TRUE if the Document List panel is currently shown, FALSE otherwise</returns>
        NPPM_ISDOCSWITCHERSHOWN = NPPMSG + 86,
        /// <summary>
        /// BOOL NPPM_GETAPPDATAPLUGINSALLOWED(0, 0)<br/>
        /// Check to see if loading plugins from <c>"%APPDATA%\..\Local\Notepad++\plugins"</c> is allowed.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// <returns>TRUE if loading plugins from <c>%APPDATA%</c> is allowed, FALSE otherwise</returns>
        /// </summary>
        NPPM_GETAPPDATAPLUGINSALLOWED = NPPMSG + 87,
        /// <summary>
        /// int NPPM_GETCURRENTVIEW(0, 0)<br/>
        /// Get the current used view.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// <returns>current edit view of Notepad++. Only 2 possible values: 0 = <see cref="MAIN_VIEW"/>, 1 = <see cref="SUB_VIEW"/></returns>
        /// </summary>
        NPPM_GETCURRENTVIEW = NPPMSG + 88,
        /// <summary>
        /// BOOL NPPM_DOCLISTDISABLEEXTCOLUMN(0, BOOL disableOrNot)<br/>
        /// Disable or enable extension column of Document List
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>disableOrNot</c> - if TRUE, extension column is hidden otherwise it is visible.</para>
        /// </summary>
        /// <returns>TRUE</returns>
        NPPM_DOCLISTDISABLEEXTCOLUMN = NPPMSG + 89,
        /// <summary>
        /// int NPPM_GETEDITORDEFAULTFOREGROUNDCOLOR(0, 0)<br/>
        /// Get the current editor default foreground color.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// <returns>the color as integer with hex format being <c>0x00bbggrr</c></returns>
        /// </summary>
        NPPM_GETEDITORDEFAULTFOREGROUNDCOLOR = NPPMSG + 90,
        /// <summary>
        /// int NPPM_GETEDITORDEFAULTBACKGROUNDCOLOR(0, 0)<br/>
        /// Get the current editor default background color.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// </summary>
        /// <returns>the color as integer with hex format being <c>0x00bbggrr</c></returns>
        NPPM_GETEDITORDEFAULTBACKGROUNDCOLOR = NPPMSG + 91,
        /// <summary>
        /// BOOL NPPM_SETSMOOTHFONT(0, BOOL setSmoothFontOrNot)<br/>
        /// Set (or remove) smooth font. The API uses underlying Scintilla command <see cref="SciMsg.SCI_SETFONTQUALITY"/> to manage the font quality.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>setSmoothFontOrNot</c> - if TRUE, this message sets <see cref="SciMsg.SC_EFF_QUALITY_LCD_OPTIMIZED"/> else <see cref="SciMsg.SC_EFF_QUALITY_DEFAULT"/></para>
        /// </summary>
        /// <returns>TRUE</returns>
        NPPM_SETSMOOTHFONT = NPPMSG + 92,
        /// <summary>
        /// BOOL NPPM_SETEDITORBORDEREDGE(0, BOOL withEditorBorderEdgeOrNot)<br/>
        /// Add (or remove) an additional sunken edge style to the Scintilla window else it removes the extended style from the window.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>withEditorBorderEdgeOrNot</c> - TRUE for adding border edge on Scintilla window, FALSE for removing it</para>
        /// </summary>
        /// <returns>TRUE</returns>
        NPPM_SETEDITORBORDEREDGE = NPPMSG + 93,
        /// <summary>
        /// BOOL NPPM_SAVEFILE(0, const TCHAR *fileNameToSave)<br/>
        /// Save the file (opened in Notepad++) with the given full file name path.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>fileNameToSave</c> must be the full file path for the file to be saved.</para>
        /// </summary>
        /// <returns>TRUE on success, FALSE if <c>fileNameToSave</c> is not found</returns>
        NPPM_SAVEFILE = NPPMSG + 94,
        /// <summary>
        /// BOOL NPPM_DISABLEAUTOUPDATE(0, 0)<br/>
        /// Disable Notepad++ auto-update.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// </summary>
        /// <returns>TRUE</returns>
        NPPM_DISABLEAUTOUPDATE = NPPMSG + 95, // 2119 in decimal
        /// <summary>
        /// BOOL NPPM_REMOVESHORTCUTBYCMDID(int pluginCmdID, 0)<br/>
        /// Remove the assigned shortcut mapped to pluginCmdID
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: pluginCmdID</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// </summary>
        /// <returns>TRUE if function call is successful, otherwise FALSE</returns>
        NPPM_REMOVESHORTCUTBYCMDID = NPPMSG + 96,  // 2120 in decimal
        /// <summary>
        /// int NPPM_GETPLUGINHOMEPATH(size_t strLen, TCHAR* pluginRootPath)<br/>
        /// Get plugin home root path. It's useful if plugins want to get its own path by appending <c>pluginFolderName</c> which is the name of plugin without extension part.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>strLen</c> - size of allocated <c>pluginRootPath</c> buffer</para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: <c>pluginRootPath</c> - Users should call it with <c>pluginRootPath</c> be NULL to get the required number of TCHAR (not including the terminating nul character),</para>
        ///              allocate pluginRootPath buffer with the <returns>value + 1, then call it again to get the path.</returns>
        /// </summary>
        /// <returns>the number of TCHAR copied/to copy, or 0 on failure</returns>
        NPPM_GETPLUGINHOMEPATH = NPPMSG + 97,
        /// <summary>
        /// int NPPM_GETSETTINGSONCLOUDPATH(size_t strLen, TCHAR *settingsOnCloudPath)<br/>
        /// Get settings on cloud path. It's useful if plugins want to store its settings on Cloud, if this path is set.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: <c>strLen</c> - size of allocated <c>settingsOnCloudPath</c> buffer</para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: <c>settingsOnCloudPath</c> - Users should call it with <c>settingsOnCloudPath</c> be NULL to get the required number of TCHAR (not including the terminating nul character),</para>
        ///              allocate settingsOnCloudPath buffer with the <returns>value + 1, then call it again to get the path.</returns>
        /// </summary>
        /// <returns>the number of TCHAR copied/to copy. If the <returns>value is 0, then this path is not set, or the "strLen" is not enough to copy the path.</returns></returns>
        NPPM_GETSETTINGSONCLOUDPATH = NPPMSG + 98,
        /// <summary>
        /// BOOL NPPM_SETLINENUMBERWIDTHMODE(0, int widthMode)<br/>
        /// Set line number margin width in dynamic width mode (<see cref="LINENUMWIDTH_DYNAMIC"/>) or constant width mode (<see cref="LINENUMWIDTH_CONSTANT"/>)<br/>
        /// It may help some plugins to disable non-dynamic line number margins width to have a smoothly visual effect while vertical scrolling the content in Notepad++
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: widthMode should be <see cref="LINENUMWIDTH_DYNAMIC"/> or <see cref="LINENUMWIDTH_CONSTANT"/></para>
        /// </summary>
        /// <returns>TRUE</returns> if calling is successful, otherwise <returns>FALSE</returns>
        NPPM_SETLINENUMBERWIDTHMODE = NPPMSG + 99,
        /// <inheritdoc cref="NPPM_SETLINENUMBERWIDTHMODE"/>
        LINENUMWIDTH_DYNAMIC = 0,
        /// <inheritdoc cref="NPPM_SETLINENUMBERWIDTHMODE"/>
        LINENUMWIDTH_CONSTANT = 1,
        /// <summary>
        /// int NPPM_GETLINENUMBERWIDTHMODE(0, 0)<br/>
        /// Get line number margin width in dynamic width mode (<see cref="LINENUMWIDTH_DYNAMIC"/>) or constant width mode (<see cref="LINENUMWIDTH_CONSTANT"/>)<br/>
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// </summary>
        /// <returns>current line number margin width mode (<see cref="LINENUMWIDTH_DYNAMIC"/> or <see cref="LINENUMWIDTH_CONSTANT"/>)</returns>
        NPPM_GETLINENUMBERWIDTHMODE = NPPMSG + 100,
        /// <summary>
        /// BOOL NPPM_ADDTOOLBARICON_FORDARKMODE(UINT pluginCmdID, toolbarIconsWithDarkMode* iconHandles)<br/>
        /// Use this instead of the obsolete <see cref="NPPM_ADDTOOLBARICON"/> (DEPRECATED) which doesn't support the dark mode
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: pluginCmdID</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: iconHandles is the pointer to a <see cref="ToolbarIconDarkMode"/> structure</para>
        ///             All 3 handles below should be set so the icon will be displayed correctly if toolbar icon sets are changed by users, also in dark mode
        /// </summary>
        /// <remarks>
        /// Added in <a href="https://community.notepad-plus-plus.org/topic/21652/add-new-api-nppm_addtoolbaricon_fordarkmode-for-dark-mode">8.0</a>
        /// </remarks>
        /// <returns>TRUE</returns>
        NPPM_ADDTOOLBARICON_FORDARKMODE = NPPMSG + 101,
        /// <summary>
        /// BOOL NPPM_DOCLISTDISABLEPATHCOLUMN(0, BOOL disableOrNot)<br/>
        /// Disable or enable path column of Document List
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: <c>disableOrNot</c> - if disableOrNot is TRUE, extension column is hidden otherwise it is visible.</para>
        /// </summary>
        /// <remarks>See <seealso cref="NPPM_DOCLISTDISABLEEXTCOLUMN"/>.</remarks>
        /// <returns>TRUE</returns>
        NPPM_DOCLISTDISABLEPATHCOLUMN = NPPMSG + 102,
        /// <summary>
        /// BOOL NPPM_GETEXTERNALLEXERAUTOINDENTMODE(const TCHAR* languageName, ExternalLexerAutoIndentMode* autoIndentMode)<br/>
        /// Get ExternalLexerAutoIndentMode for an installed external programming language.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: languageName is external language name to search</para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: autoIndentMode could receive one of three following values</para>
        ///              - Standard (0) means Notepad++ will keep the same TAB indentation between lines;
        ///              - C_Like (1) means Notepad++ will perform a C-Language style indentation for the selected external language;
        ///              - Custom (2) means a Plugin will be controlling auto-indentation for the current language.
        /// returned values: TRUE for successful searches, otherwise FALSE.
        /// </summary>
        NPPM_GETEXTERNALLEXERAUTOINDENTMODE = NPPMSG + 103,
        /// <summary>
        /// BOOL NPPM_SETEXTERNALLEXERAUTOINDENTMODE(const TCHAR* languageName, ExternalLexerAutoIndentMode autoIndentMode)<br/>
        /// Set ExternalLexerAutoIndentMode for an installed external programming language.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: languageName is external language name to set</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: autoIndentMode could receive one of three following values</para>
        ///             - Standard (0) means Notepad++ will keep the same TAB indentation between lines;
        ///             - C_Like (1) means Notepad++ will perform a C-Language style indentation for the selected external language;
        ///             - Custom (2) means a Plugin will be controlling auto-indentation for the current language.
        /// <returns>value: TRUE if function call was successful, otherwise FALSE.</returns>
        /// </summary>
        NPPM_SETEXTERNALLEXERAUTOINDENTMODE = NPPMSG + 104,
        /// <summary>
        /// BOOL NPPM_ISAUTOINDENTON(0, 0)<br/>
        /// Get the current use Auto-Indentation setting in Notepad++ Preferences.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// <returns>TRUE</returns> if Auto-Indentation is on, FALSE otherwise
        /// </summary>
        NPPM_ISAUTOINDENTON = NPPMSG + 105,
        /// <summary>
        /// MacroStatus NPPM_GETCURRENTMACROSTATUS(0, 0)<br/>
        /// Get current enum class MacroStatus { Idle, RecordInProgress, RecordingStopped, PlayingBack }
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// <returns>MacroStatus as int:</returns>
        /// 0: Idle - macro is not in use and it's empty
        /// 1: RecordInProgress - macro is currently being recorded
        /// 2: RecordingStopped - macro recording has been stopped
        /// 3: PlayingBack - macro is currently being played back
        /// </summary>
        NPPM_GETCURRENTMACROSTATUS = NPPMSG + 106,
        /// <summary>
        /// BOOL NPPM_ISDARKMODEENABLED(0, 0)<br/>
        /// Get Notepad++ Dark Mode status (ON or OFF).
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// <returns>TRUE</returns> if Dark Mode is enable, otherwise FALSE
        /// since 8.4.1
        /// https://github.com/notepad-plus-plus/notepad-plus-plus/commit/1eb5b10e41d7ab92b60aa32b28d4fe7739d15b53
        /// </summary>
        NPPM_ISDARKMODEENABLED = NPPMSG + 107,
        /// <summary>
        /// BOOL NPPM_GETDARKMODECOLORS (size_t cbSize, NppDarkMode::Colors* returnColors)<br/>
        /// Get the colors used in Dark Mode.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: cbSize must be filled with sizeof(NppDarkMode::Colors).</para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: returnColors must be a pre-allocated NppDarkMode::Colors struct.</para>
        /// <example>
        /// You need to uncomment the following code to use NppDarkMode::Colors structure:
        /// <code>
        ///     namespace NppDarkMode
        ///     {
        ///      struct Colors
        ///      {
        ///        COLORREF background = 0;
        ///        COLORREF softerBackground = 0;
        ///        COLORREF hotBackground = 0;
        ///        COLORREF pureBackground = 0;
        ///        COLORREF errorBackground = 0;
        ///        COLORREF text = 0;
        ///        COLORREF darkerText = 0;
        ///        COLORREF disabledText = 0;
        ///        COLORREF linkText = 0;
        ///        COLORREF edge = 0;
        ///        COLORREF hotEdge = 0;
        ///        COLORREF disabledEdge = 0;
        ///      };
        ///     }
        /// </code>
        /// </example>
        /// Note: in the case of calling failure ("false" is returned), you may need to change NppDarkMode::Colors structure to:
        /// https://github.com/notepad-plus-plus/notepad-plus-plus/blob/master/PowerEditor/src/NppDarkMode.h#L32
        /// </summary>
        /// <returns>TRUE when successful, FALSE otherwise.</returns>
        /// <remarks>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/1eb5b10e41d7ab92b60aa32b28d4fe7739d15b53">8.4.1</a>
        /// </remarks>
        NPPM_GETDARKMODECOLORS = NPPMSG + 108,
        /// <summary>
        /// int NPPM_GETCURRENTCMDLINE(size_t strLen, TCHAR *commandLineStr)<br/>
        /// Get the Current Command Line string.
        /// Users should call it with commandLineStr as NULL to get the required number of TCHAR (not including the terminating nul character),
        /// allocate commandLineStr buffer with the <returns>value + 1, then call it again to get the current command line string.</returns>
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: strLen is "commandLineStr" buffer length</para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: commandLineStr receives all copied command line string</para>
        /// <returns>the number of TCHAR copied/to copy</returns>
        /// since 8.4.2
        /// https://github.com/notepad-plus-plus/notepad-plus-plus/commit/0f8d5724afb0a540e8b4024252945ab60bc88c71
        /// </summary>
        NPPM_GETCURRENTCMDLINE = NPPMSG + 109,
        /// <summary>
        /// void* NPPM_CREATELEXER(0, const TCHAR* lexer_name)<br/>
        /// Get the ILexer pointer created by Lexilla. Call the lexilla "CreateLexer()" function to allow plugins to set the lexer for a Scintilla instance created by NPPM_CREATESCINTILLAHANDLE.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: lexer_name is the name of the lexer</para>
        /// <returns>the ILexer pointer</returns>
        /// since 8.4.3
        /// https://github.com/notepad-plus-plus/notepad-plus-plus/commit/f1ed4de78dbe8f5d85f4d199bae2970148cca8ed
        /// </summary>
        NPPM_CREATELEXER = NPPMSG + 110,
        /// <summary>
        /// int NPPM_GETBOOKMARKID(0, 0)<br/>
        /// Get the bookmark ID - use this API to get bookmark ID dynamically that garantees you get always the right bookmark ID even it's been changed through the different versions.
        /// <para>wParam (<see cref="UIntPtr"/>): 0 (not used)</para>
        /// <para>lParam (<see cref="IntPtr"/>) : 0 (not used)</para>
        /// <returns>bookmark ID</returns>
        /// since 8.4.7
        /// https://github.com/notepad-plus-plus/notepad-plus-plus/commit/4d5069280900ee249d358bc2b311bdb4b03f30a9
        /// </summary>
        NPPM_GETBOOKMARKID = NPPMSG + 111,
        /// <summary>
        /// ULONG NPPM_DARKMODESUBCLASSANDTHEME(ULONG dmFlags, HWND hwnd)<br/>
        /// Add support for generic dark mode to plugin dialog. Subclassing is applied automatically unless DWS_USEOWNDARKMODE flag is used.
        /// Might not work properly in C# plugins.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: dmFlags has 2 possible value dmfInit (0x0000000BUL) &amp; dmfHandleChange (0x0000000CUL) - see above definition</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: hwnd is the dialog handle of plugin -  Docking panels don't need to call NPPM_DARKMODESUBCLASSANDTHEME</para>
        /// Examples:
        /// <example>
        /// - after controls initializations in WM_INITDIALOG, in WM_CREATE or after CreateWindow:
        /// <code>
        ///     auto success = static_cast&lt;ULONG&gt;(::SendMessage(nppData._nppHandle, NPPM_DARKMODESUBCLASSANDTHEME, static_cast&lt;WPARAM&gt;(NppDarkMode::dmfInit), reinterpret_cast&lt;LPARAM&gt;(mainHwnd)));
        /// </code></example><example>
        /// - handling dark mode change:
        /// <code>
        ///     extern "C" __declspec(dllexport) void beNotified(SCNotification * notifyCode)
        ///     {
        ///     	switch (notifyCode->nmhdr.code)
        ///     	{
        ///     		case NPPN_DARKMODECHANGED:
        ///     		{
        ///     			::SendMessage(nppData._nppHandle, NPPM_DARKMODESUBCLASSANDTHEME, static_cast&lt;WPARAM&gt;(dmfHandleChange), reinterpret_cast&lt;LPARAM&gt;(mainHwnd));
        ///     			::SetWindowPos(mainHwnd, nullptr, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
        ///                 // to redraw titlebar and window
        ///     			break;
        ///     		}
        ///     	}
        ///     }
        /// </code>
        /// </example>
        /// </summary>
        /// <returns>succesful combinations of flags.</returns>
        /// <remarks>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/e7f321f21a2feae3669b286ae2b64e6e033f231f">8.5.4</a>
        /// </remarks>
        NPPM_DARKMODESUBCLASSANDTHEME = NPPMSG + 112,
        /// <summary>
        /// BOOL NPPM_ALLOCATEINDICATOR(int numberRequested, int* startNumber)<br/>
        /// Allocates an indicator number to a plugin: if a plugin needs to add an indicator,
        /// it has to use this message to get the indicator number, in order to prevent a conflict with the other plugins.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: numberRequested is the number of ID you request for the reservation</para>
        /// <para>lParam (<see cref="IntPtr"/>) [out]: startNumber will be set to the initial command ID if successful</para>
        /// <example>
        /// Example: If a plugin needs 1 indicator ID, the following code can be used :
        /// <code>
        ///     int idBegin;
        ///     BOOL isAllocatedSuccessful = ::SendMessage(nppData._nppHandle, NPPM_ALLOCATEINDICATOR, 1, &amp;idBegin);
        /// </code>
        /// </example>
        /// If isAllocatedSuccessful is TRUE, and value of idBegin is 7,
        /// then indicator ID 7 is preserved by Notepad++, and it is safe to be used by the plugin.
        /// </summary>
        /// <returns>TRUE if successful, FALSE otherwise. startNumber will also be set to 0 if unsuccessful</returns>
        /// <remarks>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/de25873cb3352ee59d883e95e80c91806944e348">8.5.6</a>
        /// </remarks>
        NPPM_ALLOCATEINDICATOR = NPPMSG + 113,
        /// <summary>
        /// int NPPM_GETTABCOLORID (int view, int tabIndex)<br/>
        /// Gets the tab color id for the given view and tab index.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: 0 for <see cref="MAIN_VIEW"/>, 1 for <see cref="SUB_VIEW"/>, -1 for currently-active view</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: zero-based tab index, i.e., use 0 for first tab, 1 for second tab, etc.; use -1 for active tab</para>
        /// </summary>
        /// <returns>the tab color id value</returns>
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
        /// There is no symmetric plugin command for setting the tab color.
        /// Plugins can use <see cref="NPPM_MENUCOMMAND"/> to set active tab's color
        /// with the desired tab color using these menu-command ids:
        /// <code>
        ///     44110 (no color)
        ///     44111 (yellow)
        ///     44112 (green)
        ///     44113 (blue)
        ///     44114 (orange)
        ///     44115 (pink)
        /// </code>
        /// </example>
        /// <para>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/9244cd09430c82ecff805ea862c9133d5cb56ded">8.6.8</a>
        /// </para>
        /// </remarks>
        NPPM_GETTABCOLORID = NPPMSG + 114,
        /// <summary>
        /// int NPPM_SETUNTITLEDNAME(BufferID id, const TCHAR* newName)<br/>
        /// Rename the tab name for an untitled tab.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: id - BufferID of the tab. -1 for currently active tab</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: newName - the desired new name of the tab</para>
        /// </summary>
        /// <returns>TRUE upon success; FALSE upon failure</returns>
        /// <remarks>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/b3daf0a98220ffc6e206133aa645d5a2d1d63a4f">8.6.9</a>
        /// </remarks>
        NPPM_SETUNTITLEDNAME = NPPMSG + 115,
        /// <summary>
        /// int NPPM_GETNATIVELANGFILENAME(size_t strLen, char* nativeLangFileName)<br/>
        /// Get the Current native language file name string. Use it after getting <see cref="NPPN_READY"/> notification to find out which native language is used.
        /// Users should call it with <c>nativeLangFileName</c> as NULL to get the required number of char (not including the terminating nul character),
        /// allocate language file name string buffer with the return value + 1, then call it again to get the current native language file name string.
        /// <para>wParam (<see cref="UIntPtr"/>) [in]: strLen is &quot;language file name string&quot; buffer length</para>
        /// <para>lParam (<see cref="IntPtr"/>) [in]: language file name string receives all copied native language file name string</para>
        /// </summary>
        /// <returns>The number of chars copied/to copy</returns>
        /// <remarks>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/446cc980e871d04885f13055bb56acee820636c8">8.7</a>
        /// </remarks>
        NPPM_GETNATIVELANGFILENAME = NPPMSG + 116,

        RUNCOMMAND_USER = Constants.WM_USER + 3000,
        /// <summary>
        /// BOOL NPPM_GETXXXXXXXXXXXXXXXX(size_t strLen, TCHAR *str)<br/>
        /// where:<br/>
        /// str is the allocated TCHAR array,<br/>
        /// strLen is the allocated array size
        /// </summary>
        /// <returns>TRUE when get generic_string operation success, otherwise FALSE (allocated array size is too small)</returns>
        NPPM_GETFULLCURRENTPATH = RUNCOMMAND_USER + FULL_CURRENT_PATH,
        /// <inheritdoc cref="NPPM_GETFULLCURRENTPATH"/>
        NPPM_GETCURRENTDIRECTORY = RUNCOMMAND_USER + CURRENT_DIRECTORY,
        /// <inheritdoc cref="NPPM_GETFULLCURRENTPATH"/>
        NPPM_GETFILENAME = RUNCOMMAND_USER + FILE_NAME,
        /// <inheritdoc cref="NPPM_GETFULLCURRENTPATH"/>
        NPPM_GETNAMEPART = RUNCOMMAND_USER + NAME_PART,
        /// <inheritdoc cref="NPPM_GETFULLCURRENTPATH"/>
        NPPM_GETEXTPART = RUNCOMMAND_USER + EXT_PART,
        /// <inheritdoc cref="NPPM_GETFULLCURRENTPATH"/>
        NPPM_GETCURRENTWORD = RUNCOMMAND_USER + CURRENT_WORD,
        /// <inheritdoc cref="NPPM_GETFULLCURRENTPATH"/>
        NPPM_GETNPPDIRECTORY = RUNCOMMAND_USER + NPP_DIRECTORY,
        /// <inheritdoc cref="NPPM_GETFULLCURRENTPATH"/>
        NPPM_GETFILENAMEATCURSOR = RUNCOMMAND_USER + GETFILENAMEATCURSOR,
        /// <inheritdoc cref="NPPM_GETFULLCURRENTPATH"/>
        NPPM_GETCURRENTLINESTR = RUNCOMMAND_USER + CURRENT_LINESTR,
        /// <summary>
        /// INT NPPM_GETCURRENTLINE(0, 0);
        /// </summary>
        /// <returns>the caret current position line</returns>
        NPPM_GETCURRENTLINE = RUNCOMMAND_USER + CURRENT_LINE,
        /// <summary>
        /// INT NPPM_GETCURRENTCOLUMN(0, 0);
        /// </summary>
        /// <returns>the caret current position column</returns>
        NPPM_GETCURRENTCOLUMN = RUNCOMMAND_USER + CURRENT_COLUMN,
        /// <inheritdoc cref="NPPM_GETFULLCURRENTPATH"/>
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
        ///scnNotification->nmhdr.code = NPPN_READY;
        ///scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///scnNotification->nmhdr.idFrom = 0;
        /// </summary>
        NPPN_READY = NPPN_FIRST + 1,

        /// <summary>
        ///scnNotification->nmhdr.code = NPPN_TB_MODIFICATION;
        ///scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///scnNotification->nmhdr.idFrom = 0;
        /// </summary>
        NPPN_TBMODIFICATION = NPPN_FIRST + 2,

        /// <summary>
        ///scnNotification->nmhdr.code = NPPN_FILEBEFORECLOSE;
        ///scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///scnNotification->nmhdr.idFrom = BufferID;
        /// </summary>
        NPPN_FILEBEFORECLOSE = NPPN_FIRST + 3,

        /// <summary>
        ///scnNotification->nmhdr.code = NPPN_FILEOPENED;
        ///scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///scnNotification->nmhdr.idFrom = BufferID;
        /// </summary>
        NPPN_FILEOPENED = NPPN_FIRST + 4,

        /// <summary>
        ///scnNotification->nmhdr.code = NPPN_FILECLOSED;
        ///scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///scnNotification->nmhdr.idFrom = BufferID;
        /// </summary>
        NPPN_FILECLOSED = NPPN_FIRST + 5,

        /// <summary>
        ///scnNotification->nmhdr.code = NPPN_FILEBEFOREOPEN;
        ///scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///scnNotification->nmhdr.idFrom = BufferID;
        /// </summary>
        NPPN_FILEBEFOREOPEN = NPPN_FIRST + 6,

        /// <summary>
        ///scnNotification->nmhdr.code = NPPN_FILEBEFOREOPEN;
        ///scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///scnNotification->nmhdr.idFrom = BufferID;
        /// </summary>
        NPPN_FILEBEFORESAVE = NPPN_FIRST + 7,

        /// <summary>
        ///scnNotification->nmhdr.code = NPPN_FILESAVED;
        ///scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///scnNotification->nmhdr.idFrom = BufferID;
        /// </summary>
        NPPN_FILESAVED = NPPN_FIRST + 8,

        /// <summary>
        ///scnNotification->nmhdr.code = NPPN_SHUTDOWN;
        ///scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///scnNotification->nmhdr.idFrom = 0;
        /// </summary>
        NPPN_SHUTDOWN = NPPN_FIRST + 9,

        /// <summary>
        ///scnNotification->nmhdr.code = NPPN_BUFFERACTIVATED;
        ///scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///scnNotification->nmhdr.idFrom = activatedBufferID;
        /// </summary>
        NPPN_BUFFERACTIVATED = NPPN_FIRST + 10,

        /// <summary>
        ///scnNotification->nmhdr.code = NPPN_LANGCHANGED;
        ///scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///scnNotification->nmhdr.idFrom = currentBufferID;
        /// </summary>
        NPPN_LANGCHANGED = NPPN_FIRST + 11,

        /// <summary>
        ///scnNotification->nmhdr.code = NPPN_WORDSTYLESUPDATED;
        ///scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///scnNotification->nmhdr.idFrom = currentBufferID;
        /// </summary>
        NPPN_WORDSTYLESUPDATED = NPPN_FIRST + 12,

        /// <summary>
        ///scnNotification->nmhdr.code = NPPN_SHORTCUTSREMAPPED;
        ///scnNotification->nmhdr.hwndFrom = ShortcutKeyStructurePointer;
        ///scnNotification->nmhdr.idFrom = cmdID;
        ///where ShortcutKeyStructurePointer is pointer of struct ShortcutKey:
        ///struct ShortcutKey {
        ///	bool _isCtrl;
        ///	bool _isAlt;
        ///	bool _isShift;
        ///	UCHAR _key;
        ///};
        /// </summary>
        NPPN_SHORTCUTREMAPPED = NPPN_FIRST + 13,

        /// <summary>
        ///scnNotification->nmhdr.code = NPPN_FILEBEFOREOPEN;
        ///scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///scnNotification->nmhdr.idFrom = NULL;
        /// </summary>
        NPPN_FILEBEFORELOAD = NPPN_FIRST + 14,

        /// <summary>
        ///scnNotification->nmhdr.code = NPPN_FILEOPENFAILED;
        ///scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///scnNotification->nmhdr.idFrom = BufferID;
        /// </summary>
        NPPN_FILELOADFAILED = NPPN_FIRST + 15,

        /// <summary>
        ///scnNotification->nmhdr.code = NPPN_READONLYCHANGED;
        ///scnNotification->nmhdr.hwndFrom = bufferID;
        ///scnNotification->nmhdr.idFrom = docStatus;
        /// where bufferID is BufferID
        ///       docStatus can be combined by <see cref="DOCSTATUS_READONLY"/> and <see cref="DOCSTATUS_BUFFERDIRTY"/>
        /// </summary>
        NPPN_READONLYCHANGED = NPPN_FIRST + 16,
        /// <inheritdoc cref="NPPN_READONLYCHANGED"/>
        DOCSTATUS_READONLY = 1,
        /// <inheritdoc cref="NPPN_READONLYCHANGED"/>
        DOCSTATUS_BUFFERDIRTY = 2,

        /// <summary>
        ///scnNotification->nmhdr.code = NPPN_DOCORDERCHANGED;
        ///scnNotification->nmhdr.hwndFrom = newIndex;
        ///scnNotification->nmhdr.idFrom = BufferID;
        /// </summary>
        NPPN_DOCORDERCHANGED = NPPN_FIRST + 17,

        /// <summary>
        ///scnNotification->nmhdr.code = NPPN_SNAPSHOTDIRTYFILELOADED;
        ///scnNotification->nmhdr.hwndFrom = NULL;
        ///scnNotification->nmhdr.idFrom = BufferID;
        /// </summary>
        NPPN_SNAPSHOTDIRTYFILELOADED = NPPN_FIRST + 18,

        /// <summary>
        ///scnNotification->nmhdr.code = NPPN_BEFORESHUTDOWN;
        ///scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///scnNotification->nmhdr.idFrom = 0;
        /// </summary>
        NPPN_BEFORESHUTDOWN = NPPN_FIRST + 19,

        /// <summary>
        ///scnNotification->nmhdr.code = NPPN_CANCELSHUTDOWN;
        ///scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///scnNotification->nmhdr.idFrom = 0;
        /// </summary>
        NPPN_CANCELSHUTDOWN = NPPN_FIRST + 20,

        /// <summary>
        ///scnNotification->nmhdr.code = NPPN_FILEBEFORERENAME;
        ///scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///scnNotification->nmhdr.idFrom = BufferID;
        /// </summary>
        NPPN_FILEBEFORERENAME = NPPN_FIRST + 21,

        /// <summary>
        ///scnNotification->nmhdr.code = NPPN_FILERENAMECANCEL;
        ///scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///scnNotification->nmhdr.idFrom = BufferID;
        /// </summary>
        NPPN_FILERENAMECANCEL = NPPN_FIRST + 22,

        /// <summary>
        ///scnNotification->nmhdr.code = NPPN_FILERENAMED;
        ///scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///scnNotification->nmhdr.idFrom = BufferID;
        /// </summary>
        NPPN_FILERENAMED = NPPN_FIRST + 23,

        /// <summary>
        ///scnNotification->nmhdr.code = NPPN_FILEBEFOREDELETE;
        ///scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///scnNotification->nmhdr.idFrom = BufferID;
        /// </summary>
        NPPN_FILEBEFOREDELETE = NPPN_FIRST + 24,

        /// <summary>
        ///scnNotification->nmhdr.code = NPPN_FILEDELETEFAILED;
        ///scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///scnNotification->nmhdr.idFrom = BufferID;
        /// </summary>
        NPPN_FILEDELETEFAILED = NPPN_FIRST + 25,

        /// <summary>
        ///scnNotification->nmhdr.code = NPPN_FILEDELETED;
        ///scnNotification->nmhdr.hwndFrom = hwndNpp;
        ///scnNotification->nmhdr.idFrom = BufferID;
        /// </summary>
        NPPN_FILEDELETED = NPPN_FIRST + 26,

        /// <summary>
        /// To notify plugins that Dark Mode was enabled/disabled
        /// scnNotification->nmhdr.code = NPPN_DARKMODECHANGED;
        /// scnNotification->nmhdr.hwndFrom = hwndNpp;
        /// scnNotification->nmhdr.idFrom = 0;
        /// </summary>
        /// <remarks>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/1eb5b10e41d7ab92b60aa32b28d4fe7739d15b53">8.4.1</a>
        /// </remarks>
        NPPN_DARKMODECHANGED = NPPN_FIRST + 27,

        /// <summary>
        /// To notify plugins that the new argument for plugins (via '-pluginMessage="YOUR_PLUGIN_ARGUMENT"' in command line) is available
        /// scnNotification->nmhdr.code = NPPN_CMDLINEPLUGINMSG;
        /// scnNotification->nmhdr.hwndFrom = hwndNpp;
        /// scnNotification->nmhdr.idFrom = pluginMessage; //where pluginMessage is pointer of type wchar_t
        /// </summary>
        /// <remarks>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/0f8d5724afb0a540e8b4024252945ab60bc88c71">8.4.2</a>
        /// </remarks>
        NPPN_CMDLINEPLUGINMSG = NPPN_FIRST + 28,

        ///<summary>
        /// To notify lexer plugins that the buffer (in idFrom) is just applied to a external lexer
        /// scnNotification->nmhdr.code = NPPN_EXTERNALLEXERBUFFER;
        /// scnNotification->nmhdr.hwndFrom = hwndNpp;
        /// scnNotification->nmhdr.idFrom = BufferID; //where pluginMessage is pointer of type wchar_t
        ///</summary>
        /// <remarks>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/03a5c4795b764fa5e719092da0c37fc66ec82889">8.5</a>
        /// </remarks>
        NPPN_EXTERNALLEXERBUFFER = NPPN_FIRST + 29,

        /// <summary>
        /// To notify plugins that the current document is just modified by Replace All action.<br/>
        /// For solving the performance issue (from v8.6.4), Notepad++ doesn't trigger SCN_MODIFIED during Replace All action anymore.<br/>
        /// As a result, the plugins which monitor SCN_MODIFIED should also monitor NPPN_GLOBALMODIFIED.<br/>
        /// scnNotification->nmhdr.code = NPPN_GLOBALMODIFIED;<br/>
        /// scnNotification->nmhdr.hwndFrom = BufferID;<br/>
        /// scnNotification->nmhdr.idFrom = 0; // preserved for the future use, must be zero
        /// </summary>
        /// <remarks>
        /// <strong>This notification is implemented in Notepad++
        /// <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/49e6957d486c360e05ba85ceb1c179a891831779">v8.6.5</a></strong>
        /// </remarks>
        NPPN_GLOBALMODIFIED = NPPN_FIRST + 30,

        /// <summary>
        /// To notify plugins that the current native language is just changed to another one.<br/>
        /// Use <see cref="NPPM_GETNATIVELANGFILENAME"/> to get current native language file name.<br/>
        /// Use <see cref="NPPM_GETMENUHANDLE"/>(NPPPLUGINMENU, 0) to get submenu "Plugins" handle (HMENU)<br/>
        /// scnNotification->nmhdr.code = NPPN_NATIVELANGCHANGED;<br/>
        /// scnNotification->nmhdr.hwndFrom = hwndNpp;<br/>
        /// scnNotification->nmhdr.idFrom = 0; // preserved for the future use, must be zero
        /// </summary>
        /// <remarks>
        /// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/446cc980e871d04885f13055bb56acee820636c8">8.7</a>
        /// </remarks>
        NPPN_NATIVELANGCHANGED = NPPN_FIRST + 31,
    }

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
