/*
 * SPDX-FileCopyrightText: 2023 Mark Johnston Olson <https://github.com/molsonkiko>
 *                         2016 Kasper B. Graversen <https://github.com/kbilsted>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Npp.DotNet.Plugin;
using Npp.DotNet.Plugin.Extensions;
using Npp.DotNet.Plugin.Winforms;
using static Npp.DotNet.Plugin.Win32;
using static Npp.DotNet.Plugin.Winforms.WinUser;
using static Npp.DotNet.Plugin.Winforms.WinGDI;

namespace Npp.DotNet.Plugin.Gui.Demo
{
    /// <summary>
    /// Extends <see cref="DotNetPlugin"/>.
    /// </summary>
    partial class Main : DotNetPlugin
    {

        #region "1. Initialize"
        /// <summary>
        /// Use this to initialize all data your plugin needs when starting up.
        /// At the very least, assign a unique name to the static <see cref="Npp.DotNet.Plugin.PluginData.PluginNamePtr"/> property.
        /// Otherwise <see cref="PluginData.DefaultPluginName"/> will be used.
        /// </summary>
        /// <remarks>
        /// This constructor must be <see langword="static"/> to ensure data is initialized
        /// before the host application calls any unmanaged methods.
        /// </remarks>
        static Main()
        {
            Instance = new Main();
            PluginData.PluginNamePtr = Marshal.StringToHGlobalUni(PluginMenuName);
        }
        #endregion

        #region "2. Implement the plugin interface"
        /// <inheritdoc cref="SetInfo" />
        public override void OnSetInfo()
        {
            Kbg.Demo.Namespace.Main.CommandMenuInit();
        }

        /// <inheritdoc cref="BeNotified" />
        public override void OnBeNotified(ScNotification notification)
        {
            uint code = notification.Header.Code;

            if (notification.Header.HwndFrom == PluginData.NppData.NppHandle)
            {
                switch ((NppMsg)code)
                {
                    case NppMsg.NPPN_TBMODIFICATION:
                        PluginData.FuncItems.RefreshItems();
                        SetToolBarIcon();
                        break;
                    case NppMsg.NPPN_DARKMODECHANGED:
                        Kbg.Demo.Namespace.Main.frmGoToLine?.ToggleDarkMode(PluginData.Notepad.IsDarkModeEnabled());
                        break;
                    case NppMsg.NPPN_SHUTDOWN:
                        PluginCleanUp();
                        PluginData.PluginNamePtr = nint.Zero;
                        PluginData.FuncItems.Dispose();
                        break;
                }
            }
            else
            {
                switch ((SciMsg)code)
                {
                    case SciMsg.SCN_CHARADDED:
                        Kbg.Demo.Namespace.Main.doInsertHtmlCloseTag((char)notification.Ch);
                        break;
                }
            }
        }
        #endregion

        static internal void PluginCleanUp()
        {
            Kbg.Demo.Namespace.Main.PluginCleanUp();
        }

        static internal void SetToolBarIcon()
        {
            Kbg.Demo.Namespace.Main.SetToolBarIcon();
        }

        /// <summary><see cref="Main"/> should be a singleton class</summary>
        private Main() { }
        private static readonly Main Instance;
        private static readonly string PluginMenuName = ".NET WinForms Demo Plugin\0";
        internal static string PluginName { get { return Kbg.Demo.Namespace.Main.PluginName; } }
    }
}

namespace Kbg.Demo.Namespace
{
    class Main
    {
        #region " Fields "
        internal static readonly string PluginName = typeof(Npp.DotNet.Plugin.Gui.Demo.Main).Namespace!;
        static string iniFilePath = string.Empty;
        static string toolbarIconPath = string.Empty;
        static readonly string sectionName = "Insert Extension";
        static readonly string keyName = "doCloseTag";
        static bool doCloseTag = false;
        static string? sessionFilePath = null;
        internal static frmGoToLine? frmGoToLine = null;
        static internal int idFrmGotToLine = -1;

        // toolbar icons
        static Bitmap? tbBmp_tbTab = null;
        static Icon? tbIco = null;
        static Icon? tbIcoDM = null;
        static Icon? tbIcon = null;

        // static IScintillaGateway editor = new ScintillaGateway(PluginBase.GetCurrentScintilla());
        // static INotepadPPGateway notepad = new NotepadPPGateway();
        #endregion

        #region " Startup/CleanUp "

        static internal void CommandMenuInit()
        {
            // Initialization of your plugin commands
            // You should fill your plugins commands here

            //
            // Firstly we get the parameters from your plugin config file (if any)
            //

            // get path of plugin configuration
            StringBuilder sbIniFilePath = new StringBuilder(MAX_PATH);
            SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_GETPLUGINSCONFIGDIR, MAX_PATH, sbIniFilePath);
            iniFilePath = sbIniFilePath.ToString();
            toolbarIconPath = Path.Combine(PluginData.Notepad.GetPluginsHomePath(), PluginName, "Properties");

            // if config path doesn't exist, we create it
            if (!Directory.Exists(iniFilePath))
            {
                Directory.CreateDirectory(iniFilePath);
            }

            // make your plugin config file full file path name
            iniFilePath = Path.Combine(iniFilePath, PluginName + ".ini");

            // get the parameter value from plugin config
            doCloseTag = (GetPrivateProfileInt(sectionName, keyName, 0, iniFilePath) != 0);

            // with function :
            // SetCommand(int index,                            // zero based number to indicate the order of command
            //            string commandName,                   // the command name that you want to see in plugin menu
            //            NppFuncItemDelegate functionPointer,  // the symbol of function (function pointer) associated with this command. The body should be defined below. See Step 4.
            //            ShortcutKey *shortcut,                // optional. Define a shortcut to trigger this command
            //            bool check0nInit                      // optional. Make this menu item be checked visually
            //            );
            Utils.SetCommand("Hello Notepad++", hello);
            Utils.SetCommand("Hello (with FX)", helloFX);
            Utils.SetCommand("What is Notepad++?", WhatIsNpp);

            // Here you insert a separator
            Utils.MakeSeparator();

            // Shortcut :
            // Following makes the command bind to the shortcut Alt-F
            Utils.SetCommand("Current Full Path", insertCurrentFullPath, new ShortcutKey(false, true, false, Keys.F));
            Utils.SetCommand("Current File Name", insertCurrentFileName);
            Utils.SetCommand("Current Directory", insertCurrentDirectory);
            Utils.SetCommand("Date && Time - short format", insertShortDateTime);
            Utils.SetCommand("Date && Time - long format", insertLongDateTime);

            Utils.SetCommand("Close HTML/XML tag automatically", checkInsertHtmlCloseTag, new ShortcutKey(false, true, false, Keys.Q), doCloseTag);

            Utils.MakeSeparator();

            Utils.SetCommand("Get File Names Demo", getFileNamesDemo);
            Utils.SetCommand("Get Session File Names Demo", getSessionFileNamesDemo);
            Utils.SetCommand("Save Current Session Demo", saveCurrentSessionDemo);

            Utils.MakeSeparator();

            Utils.SetCommand("Dockable Dialog Demo", DockableDlgDemo); idFrmGotToLine = 15;

            Utils.MakeSeparator();

            Utils.SetCommand("Print Scroll and Row Information", PrintScrollInformation);

            Utils.SetCommand("Use NanInf class for -inf, inf, nan!!", PrintNanInf);
        }

        /// <summary>
        ///
        /// </summary>
        static void PrintScrollInformation()
        {
            var editor = PluginData.Editor;
            ScrollInfo scrollInfo = editor.GetScrollInfo(ScrollInfoMask.SIF_RANGE | ScrollInfoMask.SIF_TRACKPOS | ScrollInfoMask.SIF_PAGE, ScrollInfoBar.SB_VERT);
            var scrollRatio = (double)scrollInfo.nTrackPos / (scrollInfo.nMax - scrollInfo.nPage);
            var scrollPercentage = Math.Min(scrollRatio, 1) * 100;
            editor.ReplaceSel($@"The maximum row in the current document was {scrollInfo.nMax + 1}.
A maximum of {scrollInfo.nPage} rows is visible at a time.
The current scroll ratio is {Math.Round(scrollPercentage, 2)}%.
");
        }

        static internal void SetToolBarIcon()
        {
            // TODO: use ::GetDpiForMonitor
            long screenDPI = 96;
            int minBmpHeight = 16, minBmpWidth = 16;

            IntPtr hHDC = GetDC(NULL);
            int bmpX =
                unchecked((int)((Math.BigMul(minBmpHeight, GetDeviceCaps(hHDC, DeviceCapability.LOGPIXELSX)) + (screenDPI >> 1)) / screenDPI));
            int bmpY =
                unchecked((int)((Math.BigMul(minBmpWidth, GetDeviceCaps(hHDC, DeviceCapability.LOGPIXELSY)) + (screenDPI >> 1)) / screenDPI));
            _ = ReleaseDC(NULL, hHDC);

            var loadFlags =
                LoadImageFlag.LR_LOADFROMFILE | LoadImageFlag.LR_LOADTRANSPARENT | LoadImageFlag.LR_LOADMAP3DCOLORS;
            IntPtr hBmp =
                LoadImage(NULL, Path.Combine(toolbarIconPath, "star_bmp.bmp"), LoadImageType.IMAGE_BITMAP, bmpX, bmpY, loadFlags);

            // add bmp icon
            if (hBmp != NULL)
                tbBmp_tbTab = Image.FromHbitmap(hBmp);
            else
            {
                using Bitmap bmp = new(bmpX, bmpY);
                Graphics bmpIcon = Graphics.FromImage(bmp);
                Rectangle rect = new(0, 0, bmpX, bmpY);
                bmpIcon.FillRectangle(Brushes.BlueViolet, rect);
                tbBmp_tbTab = Image.FromHbitmap(bmp.GetHbitmap());
            }

            IntPtr hIco =
                LoadImage(NULL, Path.Combine(toolbarIconPath, "star_black.ico"), LoadImageType.IMAGE_ICON, 32, 32, loadFlags);
            IntPtr hIcoDark =
                LoadImage(NULL, Path.Combine(toolbarIconPath, "star_white.ico"), LoadImageType.IMAGE_ICON, 32, 32, loadFlags);

            if (hIco == NULL) hIco = GetStandardIcon(WindowsIcon.IDI_APPLICATION);
            if (hIcoDark == NULL) hIcoDark = hIco;
            tbIco = Icon.FromHandle(hIco);
            tbIcoDM = Icon.FromHandle(hIcoDark);

            ToolbarIconDarkMode tbIcons = new()
            {
                HToolbarBmp = (tbBmp_tbTab?.GetHbitmap()).GetValueOrDefault(), // add bmp icon
                HToolbarIcon = (tbIco?.Handle).GetValueOrDefault(),            // icon with black lines
                HToolbarIconDarkMode = (tbIcoDM?.Handle).GetValueOrDefault()   // icon with light grey lines
            };

            // convert to c++ pointer
            IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(tbIcons));
            Marshal.StructureToPtr(tbIcons, pTbIcons, false);

            // call Notepad++ api
            SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_ADDTOOLBARICON_FORDARKMODE, (uint)PluginData.FuncItems.Items[idFrmGotToLine].CmdID, pTbIcons);

            // release pointer
            Marshal.FreeHGlobal(pTbIcons);
        }

        static internal void PluginCleanUp()
        {
            WritePrivateProfileString(sectionName, keyName, doCloseTag ? "1" : "0", iniFilePath);
        }
        #endregion

        #region " Menu functions "
        static void hello()
        {
            PluginData.Notepad.FileNew();
            var editor = PluginData.Editor;
            editor.SetText("Hello, Notepad++ ... from .NET!");
            var rest = editor.GetLine(0);
            editor.SetText(string.Join(editor.LineDelimiter, new string[] { rest, rest, rest }));
        }

        static void helloFX()
        {
            hello();
            new Thread(callbackHelloFX).Start();
        }

        static void callbackHelloFX()
        {
            var editor = PluginData.Editor;
            int currentZoomLevel = editor.GetZoom();
            int i = currentZoomLevel;
            for (int j = 0; j < 4; j++)
            {
                for (; i >= -10; i--)
                {
                    editor.SetZoom(i);
                    Thread.Sleep(30);
                }
                Thread.Sleep(100);
                for (; i <= 20; i++)
                {
                    Thread.Sleep(30);
                    editor.SetZoom(i);
                }
                Thread.Sleep(100);
            }
            for (; i >= currentZoomLevel; i--)
            {
                Thread.Sleep(30);
                editor.SetZoom(i);
            }
        }

        static void WhatIsNpp()
        {
            string text2display = "Notepad++ is a free (as in \"free speech\" and also as in \"free beer\") " +
                "source code editor and Notepad replacement that supports several languages.\n" +
                "Running in the MS Windows environment, its use is governed by GPL License.\n\n" +
                "Based on a powerful editing component Scintilla, Notepad++ is written in C++ and " +
                "uses pure Win32 API and STL which ensures a higher execution speed and smaller program size.\n" +
                "By optimizing as many routines as possible without losing user friendliness, Notepad++ is trying " +
                "to reduce the world carbon dioxide emissions. When using less CPU power, the PC can throttle down " +
                "and reduce power consumption, resulting in a greener environment.";
            new Thread(new ParameterizedThreadStart(callbackWhatIsNpp!)).Start(text2display);
        }

        static void callbackWhatIsNpp(object data)
        {
            string text2display = (string)data;
            PluginData.Notepad.FileNew();

            Random srand = new Random(DateTime.Now.Millisecond);
            int rangeMin = 0;
            int rangeMax = 250;
            for (int i = 0; i < text2display.Length; i++)
            {
                Thread.Sleep(srand.Next(rangeMin, rangeMax) + 30);
                PluginData.Editor.AppendTextAndMoveCursor(text2display[i].ToString());
            }
        }

        static void insertCurrentFullPath()
        {
            insertCurrentPath(NppMsg.FULL_CURRENT_PATH);
        }
        static void insertCurrentFileName()
        {
            insertCurrentPath(NppMsg.FILE_NAME);
        }
        static void insertCurrentDirectory()
        {
            insertCurrentPath(NppMsg.CURRENT_DIRECTORY);
        }
        static void insertCurrentPath(NppMsg which)
        {
            NppMsg msg = NppMsg.NPPM_GETFULLCURRENTPATH;
            if (which == NppMsg.FILE_NAME)
                msg = NppMsg.NPPM_GETFILENAME;
            else if (which == NppMsg.CURRENT_DIRECTORY)
                msg = NppMsg.NPPM_GETCURRENTDIRECTORY;

            StringBuilder path = new StringBuilder(MAX_PATH);
            SendMessage(PluginData.NppData.NppHandle, (uint)msg, 0, path);

            PluginData.Editor.ReplaceSel(path.ToString());
        }

        static void insertShortDateTime()
        {
            insertDateTime(false);
        }
        static void insertLongDateTime()
        {
            insertDateTime(true);
        }
        static void insertDateTime(bool longFormat)
        {
            string dateTime = string.Format("{0} {1}", DateTime.Now.ToShortTimeString(), longFormat ? DateTime.Now.ToLongDateString() : DateTime.Now.ToShortDateString());
            PluginData.Editor.ReplaceSel(dateTime);
        }

        static void checkInsertHtmlCloseTag()
        {
            Utils.CheckMenuItemToggle(9, ref doCloseTag); // 9 = menu item index
        }

        static readonly Regex regex = new Regex(@"[\._\-:\w]", RegexOptions.Compiled);

        static internal void doInsertHtmlCloseTag(char newChar)
        {
            LangType docType = LangType.L_TEXT;
            SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_GETCURRENTLANGTYPE, 0, ref docType);
            bool isDocTypeHTML = (docType == LangType.L_HTML || docType == LangType.L_XML || docType == LangType.L_PHP);

            if (!doCloseTag || !isDocTypeHTML)
                return;

            if (newChar != '>')
                return;

            int bufCapacity = 512;
            var editor = PluginData.Editor;
            var pos = editor.GetCurrentPos();
            long currentPos = pos;
            long beginPos = currentPos - (bufCapacity - 1);
            long startPos = (beginPos > 0) ? beginPos : 0;
            int size = unchecked(Convert.ToInt32(currentPos - startPos));

            if (size < 3)
                return;

            using (TextRangeFull tr = new(startPos, currentPos, bufCapacity))
            {
                editor.GetTextRange(tr);
                string buf = tr.LpStrText!;

                if (buf[size - 2] == '/')
                    return;

                int pCur = size - 2;
                while ((pCur > 0) && (buf[pCur] != '<') && (buf[pCur] != '>'))
                    pCur--;

                if (buf[pCur] == '<')
                {
                    pCur++;

                    var insertString = new StringBuilder("</");

                    while (regex.IsMatch(buf[pCur].ToString()))
                    {
                        insertString.Append(buf[pCur]);
                        pCur++;
                    }
                    insertString.Append('>');

                    if (insertString.Length > 3)
                    {
                        editor.BeginUndoAction();
                        editor.ReplaceSel(insertString.ToString());
                        editor.SetSel(pos, pos);
                        editor.EndUndoAction();
                    }
                }
            }
        }

        static void getFileNamesDemo()
        {
            int nbFile = (int)SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_GETNBOPENFILES, 0, 0);
            MessageBox.Show(nbFile.ToString(), "Number of opened files:");

            using (ClikeStringArray cStrArray = new ClikeStringArray(nbFile, MAX_PATH))
            {
                if (SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_GETOPENFILENAMES, (UIntPtr)cStrArray.NativePointer, nbFile) != nint.Zero)
                    foreach (string? file in cStrArray.ManagedStringsUnicode) MessageBox.Show(file);
            }
        }
        static void getSessionFileNamesDemo()
        {
            sessionFilePath = PluginData.Notepad.GetSessionFilePath();
            int nbFile = (int)SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_GETNBSESSIONFILES, 0, sessionFilePath);

            if (nbFile < 1)
            {
                MessageBox.Show($"\"{sessionFilePath}\" is missing or contains no files", "Error");
                return;
            }
            MessageBox.Show(nbFile.ToString(), "Number of session files:");

            using (ClikeStringArray cStrArray = new ClikeStringArray(nbFile, MAX_PATH))
            {
                if (SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_GETSESSIONFILES, (UIntPtr)cStrArray.NativePointer, sessionFilePath) != nint.Zero)
                    foreach (string? file in cStrArray.ManagedStringsUnicode) MessageBox.Show(file);
            }
        }
        static void saveCurrentSessionDemo()
        {
            string sessionPath = Marshal.PtrToStringUni(SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_SAVECURRENTSESSION, 0, sessionFilePath))!;
            if (!string.IsNullOrEmpty(sessionPath))
                MessageBox.Show(sessionPath, "Saved Session File :", MessageBoxButtons.OK);
        }

        static void DockableDlgDemo()
        {
            // Dockable Dialog Demo
            //
            // This demonstration shows you how to do a dockable dialog.
            // You can create your own non dockable dialog - in this case you don't nedd this demonstration.
            if (frmGoToLine == null)
            {
                frmGoToLine = new frmGoToLine(PluginData.Editor);

                using (Bitmap newBmp = new Bitmap(16, 16))
                {
                    Graphics g = Graphics.FromImage(newBmp);
                    ColorMap[] colorMap = new ColorMap[1];
                    colorMap[0] = new ColorMap();
                    colorMap[0].OldColor = Color.Fuchsia;
                    colorMap[0].NewColor = Color.FromKnownColor(KnownColor.ButtonFace);
                    ImageAttributes attr = new ImageAttributes();
                    attr.SetRemapTable(colorMap);
                    g.DrawImage(tbBmp_tbTab!, new Rectangle(0, 0, 16, 16), 0, 0, 16, 16, GraphicsUnit.Pixel, attr);
                    tbIcon = Icon.FromHandle(newBmp.GetHicon());
                }

                NppTbData _nppTbData = new()
                {
                    HClient = frmGoToLine.Handle,
                    PszName = "Go To Line #",
                    // the dlgDlg should be the index of funcItem where the current function pointer is in
                    // this case is 15.. so the initial value of funcItem[15].CmdID - not the updated internal one !
                    DlgID = idFrmGotToLine,
                    // define the default docking behaviour
                    UMask = NppTbMsg.DWS_DF_CONT_RIGHT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR,
                    HIconTab = (tbIcon?.Handle).GetValueOrDefault(),
                    PszModuleName = $"{PluginName}.dll"
                };
                IntPtr _ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(_nppTbData));
                Marshal.StructureToPtr(_nppTbData, _ptrNppTbData, false);

                SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_DMMREGASDCKDLG, 0, _ptrNppTbData);
                // Following message will toogle both menu item state and toolbar button
                SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK, (uint)PluginData.FuncItems.Items[idFrmGotToLine].CmdID, 1);
            }
            else
            {
                if (!frmGoToLine.Visible)
                {
                    SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_DMMSHOW, 0, frmGoToLine.Handle);
                    SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK, (uint)PluginData.FuncItems.Items[idFrmGotToLine].CmdID, 1);
                }
                else
                {
                    SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_DMMHIDE, 0, frmGoToLine.Handle);
                    SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK, (uint)PluginData.FuncItems.Items[idFrmGotToLine].CmdID, 0);
                }
            }
            frmGoToLine.textBox1.Focus();
        }

        static void PrintNanInf()
        {
            bool neginf_correct = double.IsNegativeInfinity(NanInf.NegInf);
            bool inf_correct = double.IsPositiveInfinity(NanInf.Inf);
            bool nan_correct = double.IsNaN(NanInf.Nan);
            string naninf = $@"-infinity == NanInf.NegInf: {neginf_correct}
infinity == NanInf.Inf: {inf_correct}
NaN == NanInf.Nan: {nan_correct}
If you want these constants in your plugin, you can find them in the NanInf class in PluginInfrastructure.
DO NOT USE double.PositiveInfinity, double.NegativeInfinity, or double.NaN.
You will get a compiler error if you do.";
            PluginData.Notepad.FileNew();
            PluginData.Editor.AppendTextAndMoveCursor(naninf);
        }
        #endregion
    }
}
