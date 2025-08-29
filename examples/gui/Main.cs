/*
 * SPDX-FileCopyrightText: 2023 Mark Johnston Olson <https://github.com/molsonkiko>
 *                         2016 Kasper B. Graversen <https://github.com/kbilsted>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Npp.DotNet.Plugin;
using Npp.DotNet.Plugin.Extensions;
using Npp.DotNet.Plugin.Gui.Demo.Properties;
using static Npp.DotNet.Plugin.Win32;
using static Npp.DotNet.Plugin.Winforms.WinUser;
using static Npp.DotNet.Plugin.Winforms.WinGDI;

namespace Npp.DotNet.Plugin.Gui.Demo
{
    /// <summary>
    /// Implements <see cref="IDotNetPlugin"/>.
    /// </summary>
    partial class Main : IDotNetPlugin
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
            using MemoryStream bmpStream = new(Resources.star_bmp);
            using MemoryStream icoStream = new(Resources.star_black);
            using MemoryStream icoDMStream = new(Resources.star_white);
            (int icoX, int icoY) = GetLogicalPixels(32, 32);
            Kbg.Demo.Namespace.Main.tbBmp_tbTab = (Bitmap)Image.FromStream(bmpStream);
            Kbg.Demo.Namespace.Main.tbIco = new Icon(icoStream, new Size(icoX, icoY));
            Kbg.Demo.Namespace.Main.tbIcoDM = new Icon(icoDMStream, new Size(icoX, icoY));
        }
        #endregion

        #region "2. Implement the plugin interface"
        /// <inheritdoc cref="SetInfo" />
        public void OnSetInfo()
        {
            Kbg.Demo.Namespace.Main.CommandMenuInit();
        }

        /// <inheritdoc cref="BeNotified" />
        public void OnBeNotified(ScNotification notification)
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

        /// <inheritdoc cref="MessageProc" />
        public NativeBool OnMessageProc(uint msg, UIntPtr wParam, IntPtr lParam) => TRUE;
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
        private static readonly IDotNetPlugin Instance;
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
        static readonly string sectionName = "Insert Extension";
        static readonly string keyName = "doCloseTag";
        static bool doCloseTag = false;
        static string? sessionFilePath = null;
        internal static frmGoToLine? frmGoToLine = null;
        static internal int idFrmGotToLine = -1, idDoCloseTag = -1;

        // toolbar icons
        internal static Bitmap? tbBmp_tbTab = null;
        internal static Icon? tbIco = null;
        internal static Icon? tbIcoDM = null;

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
            // if config path doesn't exist, we create it
            NppUtils.CreateConfigSubDirectoryIfNotExists();

            // make your plugin config file full file path name
            iniFilePath = Path.Combine(NppUtils.ConfigDirectory, PluginName + ".ini");

            // get the parameter value from plugin config
            doCloseTag = (GetPrivateProfileInt(sectionName, keyName, 0, iniFilePath) != 0);

            // with function :
            // int SetCommand(                                  // returns: zero based number to indicate the order of command
            //            string commandName,                   // the command name that you want to see in plugin menu
            //            PluginFunc functionPointer,           // function (function pointer) associated with this command. The body should be defined below. See Step 4.
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
            idDoCloseTag = Utils.SetCommand("Date && Time - long format", insertLongDateTime); // idDoCloseTag = 9

            Utils.SetCommand("Close HTML/XML tag automatically", checkInsertHtmlCloseTag, new ShortcutKey(false, true, false, Keys.Q), doCloseTag);

            Utils.MakeSeparator();

            Utils.SetCommand("Get File Names Demo", getFileNamesDemo);
            Utils.SetCommand("Get Session File Names Demo", getSessionFileNamesDemo);
            Utils.SetCommand("Save Current Session Demo", saveCurrentSessionDemo);

            idFrmGotToLine = Utils.MakeSeparator(); // idFrmGotToLine = 15;

            Utils.SetCommand("Dockable Dialog Demo", DockableDlgDemo);

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
            int minBmpHeight = 16, minBmpWidth = 16;

            // add bmp icon
            if (tbBmp_tbTab?.GetHbitmap() == NULL)
            {
                (int bmpX, int bmpY) = GetLogicalPixels(minBmpWidth, minBmpHeight);
                using Bitmap bmp = new(bmpX, bmpY);
                Graphics bmpIcon = Graphics.FromImage(bmp);
                Rectangle rect = new(0, 0, bmpX, bmpY);
                bmpIcon.FillRectangle(Brushes.BlueViolet, rect);
                tbBmp_tbTab = Image.FromHbitmap(bmp.GetHbitmap());
            }

            if (tbIco?.Handle == NULL) tbIco = Icon.FromHandle(GetStandardIcon(WindowsIcon.IDI_APPLICATION));
            if (tbIcoDM?.Handle == NULL) tbIcoDM = tbIco;

            ToolbarIconDarkMode tbIcons = new()
            {
                HToolbarBmp = (tbBmp_tbTab?.GetHbitmap()).GetValueOrDefault(), // add bmp icon
                HToolbarIcon = (tbIco?.Handle).GetValueOrDefault(),            // icon with black lines
                HToolbarIconDarkMode = (tbIcoDM?.Handle).GetValueOrDefault()   // icon with light grey lines
            };

            PluginData.Notepad.AddToolbarIcon(idFrmGotToLine, tbIcons);
        }

        static internal void PluginCleanUp()
        {
            WritePrivateProfileString(sectionName, keyName, doCloseTag ? "1" : "0", iniFilePath);
            frmGoToLine?.Dispose();
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
            insertCurrentPath(NppUtils.PathType.FULL_CURRENT_PATH);
        }
        static void insertCurrentFileName()
        {
            insertCurrentPath(NppUtils.PathType.FILE_NAME);
        }
        static void insertCurrentDirectory()
        {
            insertCurrentPath(NppUtils.PathType.DIRECTORY);
        }
        static void insertCurrentPath(NppUtils.PathType which)
        {
            PluginData.Editor.ReplaceSel(NppUtils.GetCurrentPath(which));
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
            Utils.CheckMenuItemToggle(idDoCloseTag, ref doCloseTag); // 9 = menu item index
        }

        static readonly Regex regex = new Regex(@"[\._\-:\w]", RegexOptions.Compiled);

        static internal void doInsertHtmlCloseTag(char newChar)
        {
            LangType docType = PluginData.Notepad.GetCurrentLanguage();
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
            var fileList = PluginData.Notepad.GetOpenFileNames();
            MessageBox.Show(fileList.Count().ToString(), "Number of opened files:");
            foreach (string file in fileList) MessageBox.Show(file);
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
                frmGoToLine = new frmGoToLine(PluginData.Editor,
                    idFrmGotToLine,
                    $"{PluginName}.dll",
                    (PluginData.Notepad.IsDarkModeEnabled() ? tbIcoDM : tbIco)!);
            }
            else
            {
                if (!frmGoToLine.Visible)
                {
                    frmGoToLine.ShowDockingForm();
                }
                else
                {
                    frmGoToLine.HideDockingForm();
                }
            }
            frmGoToLine.textBox1?.Focus();
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
