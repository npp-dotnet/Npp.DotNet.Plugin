/*
 * SPDX-FileCopyrightText: 2016 Kasper B. Graversen <https://github.com/kbilsted>
 *						   2023 Mark Johnston Olson <https://github.com/molsonkiko>
 *						   2024, 2025 Robert Di Pardo  <https://github.com/rdipardo>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Npp.DotNet.Plugin.Scintilla;

namespace Npp.DotNet.Plugin
{
	/// <summary>
	/// Helpers for sending messages defined in <see cref="NppMsg"/>.
	/// </summary>
	public class NotepadPPGateway
	{
		private static readonly int Unused = 0;
		private static readonly uint UnusedW = 0U;

		/// <summary>
		/// Sends the given <see cref="MenuCmdId"/> to the Notepad++ application window.
		/// </summary>
		protected void ExecuteMenuCommand(MenuCmdId cmdId)
		{
			Win32.SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_MENUCOMMAND, UnusedW, (int)cmdId);
		}

		/// <summary>
		/// Creates a new buffer by invoking the File - New menu command.
		/// </summary>
		public void FileNew() => ExecuteMenuCommand(MenuCmdId.IDM_FILE_NEW);

		/// <summary>
		/// Associates the plugin command identified by <paramref name="funcItemsIndex"/>
		/// with the set of icons defined by the given <see cref="ToolbarIconDarkMode"/> instance.
		/// </summary>
		public void AddToolbarIcon(int funcItemsIndex, ToolbarIconDarkMode icon)
		{
			IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf<ToolbarIconDarkMode>());
			try
			{
				Marshal.StructureToPtr(icon, pTbIcons, false);
				_ = Win32.SendMessage(
					PluginData.NppData.NppHandle,
					(uint)NppMsg.NPPM_ADDTOOLBARICON_FORDARKMODE,
					(UIntPtr)(PluginData.FuncItems?.Items[funcItemsIndex].CmdID),
					pTbIcons);
			}
			finally
			{
				Marshal.FreeHGlobal(pTbIcons);
			}
		}

		/// <summary>
		/// Associates the plugin command identified by <paramref name="funcItemsIndex"/>
		/// with the set of icons defined by the given <see cref="ToolbarIcon"/> instance.
		/// </summary>
		[Obsolete("Use AddToolbarIcon(System.Int32, Npp.DotNet.Plugin.ToolbarIconDarkMode) instead")]
		public void AddToolbarIcon(int funcItemsIndex, ToolbarIcon icon)
		{
			IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf<ToolbarIcon>());
			try
			{
				Marshal.StructureToPtr(icon, pTbIcons, false);
				_ = Win32.SendMessage(
					PluginData.NppData.NppHandle,
					(uint)NppMsg.NPPM_ADDTOOLBARICON_DEPRECATED,
					(UIntPtr)(PluginData.FuncItems?.Items[funcItemsIndex].CmdID),
					pTbIcons);
			}
			finally
			{
				Marshal.FreeHGlobal(pTbIcons);
			}
		}

		/// <summary>
		/// Associates the given bitmap with the plugin command identified by <paramref name="funcItemsIndex"/>.
		/// </summary>
		[Obsolete("Use AddToolbarIcon(System.Int32, Npp.DotNet.Plugin.ToolbarIconDarkMode) instead")]
		public void AddToolbarIcon(int funcItemsIndex, Bitmap icon)
		{
			// The dark mode API requires a least one ICO, otherwise nothing will display
			var tbi = new ToolbarIcon()
			{
				HToolbarBmp = icon.GetHbitmap()
			};
			AddToolbarIcon(funcItemsIndex, tbi);
		}

		/// <summary>
		/// Gets the word currently under the caret.
		/// </summary>
		/// <returns>The word under the caret.</returns>
		public string GetCurrentWord()
			=> GetUnicodeString(NppMsg.NPPM_GETCURRENTWORD);

		/// <summary>
		/// Gets the text of the currently active line.
		/// </summary>
		/// <returns>The text of the active line.</returns>
		public string GetCurrentLine()
			=> GetUnicodeString(NppMsg.NPPM_GETCURRENTLINESTR);

		/// <summary>
		/// Gets the path of the current document.
		/// </summary>
		public string GetCurrentFilePath()
			=> GetUnicodeString(NppMsg.NPPM_GETFULLCURRENTPATH);

		/// <summary>
		/// Gets a string from a null-terminated buffer of <c>wchar_t</c> that was allocated by Notepad++.
		/// </summary>
		/// <param name="message">The <see cref="NppMsg"/> to be sent.</param>
		/// <returns>A string decoded from a <c>wchar_t</c> buffer, or <see cref="string.Empty"/>.</returns>
		static unsafe string GetUnicodeString(NppMsg message)
		{
			string result = string.Empty;
			byte[] buffer = new byte[Constants.CURRENTWORD_MAXLENGTH * Marshal.SystemDefaultCharSize];
			fixed (byte* pBuf = buffer)
			{
				if (Win32.TRUE == (NativeBool)Win32.SendMessage(PluginData.NppData.NppHandle, (uint)message, (UIntPtr)buffer.Length, (IntPtr)pBuf))
					result = ScintillaGateway.NullTerminatedBufferToString(buffer, Encoding.Unicode);
			}
			return result;
		}

		/// <summary>
		/// This method encapsulates a common pattern in the Notepad++ API: when
		/// you need to retrieve a string, you can first query the buffer size.
		/// This method queries the necessary buffer size, allocates the temporary
		/// memory, then returns the string retrieved through that buffer.
		/// </summary>
		/// <param name="message">Message ID of the data string to query.</param>
		/// <param name="returnsWideString">
		/// <see langword="true"/> if the out parameter of the given API method is a wide character buffer (<c>wchar_t*</c> or <c>TCHAR*</c>);
		/// <see langword="false"/> if the out parameter is a byte buffer (<c>char*</c>).
		/// </param>
		/// <returns>String returned by Notepad++.</returns>
		static string GetString(NppMsg message, bool returnsWideString = true)
		{
			int len = Win32.SendMessage(
					PluginData.NppData.NppHandle,
					(uint)message, UnusedW, Unused).ToInt32()
				+ 1;
			string res = new string('\0', len);
			IntPtr pRes = returnsWideString ? Marshal.StringToHGlobalUni(res) : Marshal.StringToHGlobalAnsi(res);
			_ = Win32.SendMessage(PluginData.NppData.NppHandle, (uint)message, (uint)res.Length, pRes);
			res = returnsWideString ? Marshal.PtrToStringUni(pRes) : Marshal.PtrToStringAnsi(pRes);
			Marshal.FreeHGlobal(pRes);
			return res;
		}

		/// <returns>The path to the Notepad++ executable.</returns>
		public string GetNppPath()
			=> GetUnicodeString(NppMsg.NPPM_GETNPPDIRECTORY);

		/// <returns>The path to the top directory of all installed plugins.</returns>
		public string GetPluginsHomePath()
			=> GetString(NppMsg.NPPM_GETPLUGINHOMEPATH);

		/// <returns>The path to the Config folder for plugins.</returns>
		public string GetConfigDirectory()
			=> GetString(NppMsg.NPPM_GETPLUGINSCONFIGDIR);

		/// <returns>The path to <c>session.xml</c>.</returns>
		public string GetSessionFilePath()
		{
			// portable installation ?
			var sessionPath = Directory.GetParent(GetPluginsHomePath());
			var sessionFile = Path.Combine(sessionPath?.FullName, "session.xml");
			if (!File.Exists(sessionFile))
			{
				// system-wide installation ?
				sessionPath = Directory.GetParent(Directory.GetParent(GetConfigDirectory())?.FullName);
				sessionFile = Path.Combine(sessionPath?.FullName, "session.xml");
			}
			return sessionFile;
		}

		/// <summary>
		/// Open a file for editing in Notepad++, pretty much like using the app's
		/// File - Open menu.
		/// </summary>
		/// <param name="path">The path to the file to open.</param>
		/// <returns><see langword="true"/> on success, otherwise <see langword="false"/>.</returns>
		public bool OpenFile(string path)
			=> Win32.SendMessage(
				PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_DOOPEN, UnusedW, path).ToInt32()
			!= 0;

		/// <summary>
		/// Gets the path of the document with the given <paramref name="bufferId"/>.
		/// </summary>
		/// <returns>The full file path of the document, if <paramref name="bufferId"/> is valid; otherwise, an empty string.</returns>
		public string GetFilePath(UIntPtr bufferId)
		{
			int len = Win32.SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_GETFULLPATHFROMBUFFERID, bufferId, Unused).ToInt32() + 1;
			var path = new StringBuilder(len);
			Win32.SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_GETFULLPATHFROMBUFFERID, bufferId, path);
			return path.ToString();
		}

		/// <summary>
		/// Gets the syntax mode of the current buffer.
		/// </summary>
		public LangType GetCurrentLanguage()
		{
			Win32.SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_GETCURRENTLANGTYPE, UnusedW, out int langInt);
			return (LangType)langInt;
		}

		/// <summary>
		/// Sets the syntax mode of the current buffer to the given programming <paramref name="language"/>.
		/// </summary>
		public void SetCurrentLanguage(LangType language)
		{
			Win32.SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_SETCURRENTLANGTYPE, UnusedW, (int)language);
		}

		/// <summary>
		/// Open a standard save file dialog to save the current file.
		/// </summary>
		/// <returns><see langword="true"/> if the file was saved, otherwise <see langword="false"/>.</returns>
		public bool SaveCurrentFile()
		{
			IntPtr result = Win32.SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_SAVECURRENTFILEAS);
			return result.ToInt32() == 1;
		}

		/// <returns>
		/// 3-int tuple: (major, minor, bugfix)<br></br>
		/// Thus GetNppVersion() would return (8, 5, 0) for version 8.5.0
		/// and (7, 7, 1) for version 7.7.1
		/// </returns>
		public (int, int, int) GetNppVersion()
		{
			int version = (int)Win32.SendMessage(PluginData.NppData.NppHandle, (int)NppMsg.NPPM_GETNPPVERSION);
			int major = version >> 16;
			int minor = Math.DivRem((version & 0xffff) * 10, 10, out int bugfix);
			while (minor > 9)
				minor = Math.DivRem(minor, 10, out bugfix);
			return (major, minor, bugfix);
		}

		/// <summary>
		/// Get the full path names of all files currently open in all views.
		/// </summary>
		/// <returns>
		/// The combined list of open files in both the <see cref="NppMsg.PRIMARY_VIEW"/> and the <see cref="NppMsg.SECOND_VIEW"/>.
		/// </returns>
		public string[] GetOpenFileNames()
		{
			var fileList = new List<string>();
			foreach (var viewType in new NppMsg[] { NppMsg.PRIMARY_VIEW, NppMsg.SECOND_VIEW })
			{
				int filesInView = (int)Win32.SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_GETNBOPENFILES, UnusedW, (int)viewType);
				for (int i = 0; i < filesInView; i++)
				{
					int view = (int)(viewType == NppMsg.PRIMARY_VIEW ? NppMsg.MAIN_VIEW : NppMsg.SUB_VIEW);
					UIntPtr bufferId = (UIntPtr)(ulong)Win32.SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_GETBUFFERIDFROMPOS, (uint)i, view);
					if ((long)bufferId > 0L) fileList.Add(GetFilePath(bufferId));
				}
			}
			return fileList.ToArray();
		}

		/// <summary>
		/// The status bar is the bar at the bottom with the document type, EOL type, current position, line, etc.<br></br>
		/// Set the message for one of the sections of that bar.
		/// </summary>
		/// <param name="message">Text to display.</param>
		/// <param name="section">A <see cref="StatusBarSection"/> value.</param>
		public void SetStatusBarSection(string message, StatusBarSection section)
		{
			Win32.SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_SETSTATUSBAR, (uint)section, message);
		}

		/// <summary>
		/// Get the name of the active localization file.
		/// </summary>
		/// <returns>
		/// <para><b>N++ 8.7 and later</b></para>
		/// <para>The name of the active localization file, without the <c>.xml</c> extension.</para>
		/// <para><b>Older versions</b></para>
		/// <para><c>"english"</c></para>
		/// </returns>
		public string GetNativeLanguage()
		{
			string langName, defaultLang = "english";
			(int major, int minor, int _) = GetNppVersion();
			if (!(major > 8 || (major == 8 && minor >= 7))) return defaultLang;
			langName = GetString(NppMsg.NPPM_GETNATIVELANGFILENAME, false);
			return string.IsNullOrEmpty(langName) ? defaultLang : langName.Replace(".xml", "");
		}

		/// <summary>
		/// Sends <see cref="NppMsg.NPPM_ADDSCNMODIFIEDFLAGS"/> with the given <paramref name="flags"/>.
		/// </summary>
		/// <param name="flags">The additional <see cref="ModificationFlags"/> to monitor.</param>
		/// <remarks>
		/// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/d888fb5f1263f5ea036c610b6980e5c4381ce7eb">8.7.7</a>
		/// </remarks>
		public void SetModificationFlags(ModificationFlags flags)
		{
			_ = Win32.SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_ADDSCNMODIFIEDFLAGS, UnusedW, (int)flags);
		}

		/// <summary>
		/// Returns <see langword="true"/> if the active <see cref="SciMsg.SCN_MODIFIED"/> event mask has been changed from <see cref="ModificationFlags.NPP_DEFAULT_SC_MOD_MASK"/>.
		/// </summary>
		/// <remarks>
		/// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/d888fb5f1263f5ea036c610b6980e5c4381ce7eb">8.7.7</a>
		/// </remarks>
		public bool DefaultModificationFlagsChanged()
		{
			var sci = new ScintillaGateway(Utils.GetCurrentScintilla());
			return sci.GetModEventMask() != ModificationFlags.NPP_DEFAULT_SC_MOD_MASK;
		}

		/// <summary>
		/// Checks the return value of the <see cref="NppMsg.NPPM_ISDARKMODEENABLED"/> message.
		/// </summary>
		/// <returns><see langword="true"/> if <see cref="NppMsg.NPPM_ISDARKMODEENABLED"/> returns 1, otherwise <see langword="false"/>.</returns>
		/// <remarks>
		/// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/1eb5b10e41d7ab92b60aa32b28d4fe7739d15b53">8.4.1</a>
		/// </remarks>
		public bool IsDarkModeEnabled()
		{
			return Win32.TRUE == (NativeBool)Win32.SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_ISDARKMODEENABLED);
		}

		/// <summary>
		/// Allocates one or more unused indicator IDs, which can then be assigned styles and used to style regions of text.
		/// <para>
		/// See <see href="https://www.scintilla.org/ScintillaDoc.html#Indicators"/> for more info on the indicator API.
		/// </para>
		/// </summary>
		/// <param name="numberOfIndicators">The number of consecutive indicator IDs to allocate.</param>
		/// <param name="indicators">Target buffer of the allocated indicator IDs.</param>
		/// <returns>
		/// On success, the return value is <see langword="true"/> and <paramref name="indicators"/> will be initialzed with the
		/// requested range of indicators. If <paramref name="numberOfIndicators"/> is less than 1, or the requested indicators
		/// could not be allocated, <paramref name="indicators"/> is left empty and <see langword="false"/> is returned.
		/// </returns>
		/// <remarks>
		/// Added in <a href="https://github.com/notepad-plus-plus/notepad-plus-plus/commit/de25873">8.5.6</a>
		/// </remarks>
		public bool AllocateIndicators(int numberOfIndicators, out int[] indicators)
		{
			indicators = default;
			return numberOfIndicators >= 1 && DoAllocateIndicators(numberOfIndicators, out indicators);
		}

		unsafe bool DoAllocateIndicators(int nbIndicators, out int[] indicRng)
		{
			bool success = false;
			indicRng = new int[nbIndicators];
			fixed (int* indicRngPtr = indicRng)
			{
				IntPtr hresult =
					Win32.SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_ALLOCATEINDICATOR,
						(uint)nbIndicators, (IntPtr)indicRngPtr);
				success = hresult != IntPtr.Zero;
				if (success)
					for (int i = 1; i < nbIndicators; i++)
						indicRng[i] = indicRng[i - 1] + 1;
			}
			return success;
		}
	}
}
