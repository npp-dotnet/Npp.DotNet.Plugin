/*
 * SPDX-FileCopyrightText: 2016 Kasper B. Graversen <https://github.com/kbilsted>
 *						   2023 Mark Johnston Olson <https://github.com/molsonkiko>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace Npp.DotNet.Plugin
{
	public interface INotepadPPGateway
	{
		void FileNew();
		void AddToolbarIcon(int funcItemsIndex, ToolbarIconDarkMode icon);
		void AddToolbarIcon(int funcItemsIndex, Bitmap icon);
		string GetNppPath();
		string GetPluginConfigPath();
		string GetCurrentFilePath();
		string GetFilePath(UIntPtr bufferId);
		void SetCurrentLanguage(LangType language);
		bool OpenFile(string path);
		bool SaveCurrentFile();
		string GetConfigDirectory();
		(int, int, int) GetNppVersion();
		string[] GetOpenFileNames();
		void SetStatusBarSection(string message, StatusBarSection section);
	}

	/// <summary>
	/// This class holds helpers for sending messages defined in the Msgs_h.cs file. It is at the moment
	/// incomplete. Please help fill in the blanks.
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
			IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(icon));
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

		public void AddToolbarIcon(int funcItemsIndex, Bitmap icon)
		{
			var tbi = new ToolbarIconDarkMode()
			{
				HToolbarBmp = icon.GetHbitmap()
			};
			AddToolbarIcon(funcItemsIndex, tbi);
		}

		/// <summary>
		/// Gets the path of the current document.
		/// </summary>
		public string GetCurrentFilePath()
		{
			return NppUtils.GetCurrentPath(NppUtils.PathType.FULL_CURRENT_PATH);
		}

		/// <summary>
		/// This method incapsulates a common pattern in the Notepad++ API: when
		/// you need to retrieve a string, you can first query the buffer size.
		/// This method queries the necessary buffer size, allocates the temporary
		/// memory, then returns the string retrieved through that buffer.
		/// </summary>
		/// <param name="message">Message ID of the data string to query.</param>
		/// <returns>String returned by Notepad++.</returns>
		public string GetString(NppMsg message)
		{
			int len = Win32.SendMessage(
					PluginData.NppData.NppHandle,
					(uint)message, UnusedW, Unused).ToInt32()
				+ 1;
			var res = new StringBuilder(len);
			_ = Win32.SendMessage(
				PluginData.NppData.NppHandle, (uint)message, (uint)res.Capacity, res);
			return res.ToString();
		}

		/// <returns>The path to the Notepad++ executable.</returns>
		public string GetNppPath()
			=> GetString(NppMsg.NPPM_GETNPPDIRECTORY);

		/// <returns>The path to the Config folder for plugins.</returns>
		public string GetPluginConfigPath()
			=> GetString(NppMsg.NPPM_GETPLUGINSCONFIGDIR);

		/// <summary>
		/// Open a file for editing in Notepad++, pretty much like using the app's
		/// File - Open menu.
		/// </summary>
		/// <param name="path">The path to the file to open.</param>
		/// <returns>True on success.</returns>
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
		/// open a standard save file dialog to save the current file<br></br>
		/// Returns true if the file was saved
		/// </summary>
		public bool SaveCurrentFile()
		{
			IntPtr result = Win32.SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_SAVECURRENTFILEAS);
			return result.ToInt32() == 1;
		}

		/// <summary>
		/// Figure out default N++ config file path<br></br>
		/// Path is usually -&gt; .\Users\&lt;username&gt;\AppData\Roaming\Notepad++\plugins\config\
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
		/// the status bar is the bar at the bottom with the document type, EOL type, current position, line, etc.<br></br>
		/// Set the message for one of the sections of that bar.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="section"></param>
		public void SetStatusBarSection(string message, StatusBarSection section)
		{
			Win32.SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_SETSTATUSBAR, (uint)section, message);
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
