﻿/*
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

namespace Npp.DotNet.Plugin
{
	public interface INotepadPPGateway
	{
		void FileNew();
		void AddToolbarIcon(int funcItemsIndex, ToolbarIconDarkMode icon);
		[Obsolete("Use AddToolbarIcon(System.Int32, Npp.DotNet.Plugin.ToolbarIconDarkMode) instead")]
		void AddToolbarIcon(int funcItemsIndex, ToolbarIcon icon);
		[Obsolete("Use AddToolbarIcon(System.Int32, Npp.DotNet.Plugin.ToolbarIconDarkMode) instead")]
		void AddToolbarIcon(int funcItemsIndex, Bitmap icon);
		/// <inheritdoc cref="NotepadPPGateway.GetNppPath"/>
		string GetNppPath();
		/// <inheritdoc cref="NotepadPPGateway.GetPluginsHomePath"/>
		string GetPluginsHomePath();
		/// <inheritdoc cref="NotepadPPGateway.GetPluginConfigPath"/>
		string GetPluginConfigPath();
		string GetSessionFilePath();
		/// <inheritdoc cref="NotepadPPGateway.GetCurrentWord"/>
		string GetCurrentWord();
		/// <inheritdoc cref="NotepadPPGateway.GetCurrentLine"/>
		string GetCurrentLine();
		/// <inheritdoc cref="NotepadPPGateway.GetCurrentFilePath"/>
		string GetCurrentFilePath();
		/// <inheritdoc cref="NotepadPPGateway.GetFilePath"/>
		string GetFilePath(UIntPtr bufferId);
		/// <inheritdoc cref="NotepadPPGateway.GetNativeLanguage"/>
		string GetNativeLanguage();
		void SetCurrentLanguage(LangType language);
		/// <inheritdoc cref="NotepadPPGateway.OpenFile"/>
		bool OpenFile(string path);
		/// <inheritdoc cref="NotepadPPGateway.SaveCurrentFile"/>
		bool SaveCurrentFile();
		/// <inheritdoc cref="NotepadPPGateway.GetConfigDirectory"/>
		string GetConfigDirectory();
		/// <inheritdoc cref="NotepadPPGateway.GetNppVersion"/>
		(int, int, int) GetNppVersion();
		string[] GetOpenFileNames();
		/// <inheritdoc cref="NotepadPPGateway.SetStatusBarSection"/>
		void SetStatusBarSection(string message, StatusBarSection section);
		/// <inheritdoc cref="NotepadPPGateway.SetModificationFlags"/>
		void SetModificationFlags(ModificationFlags flags);
		/// <inheritdoc cref="NotepadPPGateway.DefaultModificationFlagsChanged"/>
		bool DefaultModificationFlagsChanged();
		/// <inheritdoc cref="NotepadPPGateway.IsDarkModeEnabled"/>
		bool IsDarkModeEnabled();
	}

	/// <summary>
	/// Helpers for sending messages defined in <a href="https://github.com/npp-dotnet/Npp.DotNet.Plugin/blob/main/lib/Npp.DotNet.Plugin/Kbg.NppPluginNET/Msgs.cs">Msgs.cs</a>.
	/// </summary>
	public class NotepadPPGateway : INotepadPPGateway
	{
		protected const int Unused = 0;
		protected const uint UnusedW = 0U;

		public void FileNew()
		{
			Win32.SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_MENUCOMMAND, UnusedW, MenuCmdId.IDM_FILE_NEW);
		}

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

#pragma warning disable CS0618
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

		public void AddToolbarIcon(int funcItemsIndex, Bitmap icon)
		{
			// The dark mode API requires a least one ICO, otherwise nothing will display
			var tbi = new ToolbarIcon()
			{
				HToolbarBmp = icon.GetHbitmap()
			};
			AddToolbarIcon(funcItemsIndex, tbi);
		}
#pragma warning restore CS0618

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
		{
			return NppUtils.GetCurrentPath(NppUtils.PathType.FULL_CURRENT_PATH);
		}

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
			=> GetString(NppMsg.NPPM_GETNPPDIRECTORY);

		/// <returns>The path to the top directory of all installed plugins.</returns>
		public string GetPluginsHomePath()
			=> GetString(NppMsg.NPPM_GETPLUGINHOMEPATH);

		/// <returns>The path to the Config folder for plugins.</returns>
		public string GetPluginConfigPath()
			=> GetString(NppMsg.NPPM_GETPLUGINSCONFIGDIR);

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
		/// Gets the path of the current document.
		/// </summary>
		public string GetFilePath(UIntPtr bufferId)
		{
			int len = Win32.SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_GETFULLPATHFROMBUFFERID, (uint)bufferId).ToInt32() + 1;
			var path = new StringBuilder(len);
			Win32.SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_GETFULLPATHFROMBUFFERID, bufferId, path);
			return path.ToString();
		}

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

		/// <summary>
		/// Figure out default N++ config file path<br></br>
		/// Path is usually <c>%UserProfile%\AppData\Roaming\Notepad++\plugins\config</c>
		/// </summary>
		public string GetConfigDirectory()
		{
			return GetString(NppMsg.NPPM_GETPLUGINSCONFIGDIR);
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

		public string[] GetOpenFileNames()
		{
			int nbFile = (int)Win32.SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_GETNBOPENFILES);

			using (ClikeStringArray cStrArray = new ClikeStringArray(nbFile, Win32.MAX_PATH))
			{
				if (Win32.SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_GETOPENFILENAMES, (UIntPtr)cStrArray.NativePointer.ToInt64(), nbFile) != IntPtr.Zero)
					return cStrArray.ManagedStringsUnicode.ToArray();
			}
			return new List<string>().ToArray();
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
		/// This class holds helpers for sending messages defined in the Resource_h.cs file. It is at the moment
		/// incomplete. Please help fill in the blanks.
		/// </summary>
		class NppResource
		{
			public static void ClearIndicator()
			{
				Win32.SendMessage(PluginData.NppData.NppHandle, (uint)Resource.NPPM_INTERNAL_CLEARINDICATOR, UnusedW, Unused);
			}
		}
	}
}
