/*
 * SPDX-FileCopyrightText: 2024 Mark Johnston Olson <https://github.com/molsonkiko>
 *                         2016 Kasper B. Graversen <https://github.com/kbilsted>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static Npp.DotNet.Plugin.Win32;

namespace Npp.DotNet.Plugin.Winforms
{
	/// <summary>
	/// Types and methods extracted from <see href="https://github.com/molsonkiko/NppCSharpPluginPack"/>.
	/// </summary>
	[System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	class NamespaceDoc { }

	/// <summary>
	/// This class holds helpers for sending messages defined in the Msgs_h.cs file. It is at the moment
	/// incomplete. Please help fill in the blanks.
	/// </summary>
	public class PluginDialogBase : NotepadPPGateway
	{
		/// <summary>
		/// Register a modeless form (i.e., a form that doesn't block the parent application until closed)<br></br>
		/// with Notepad++ using NPPM_MODELESSDIALOG<br></br>
		/// If you don't do this, Notepad++ may intercept some keystrokes in uIntPtrended ways.
		/// </summary>
		/// <param name="formHandle">the Handle attribute of a Windows form</param>
		public void AddModelessDialog(IntPtr formHandle)
		{
			SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_MODELESSDIALOG, 0U, formHandle);
		}

		/// <summary>
		/// unregister a modelesss form that was registered with AddModelessDialog.<br></br>
		/// This MUST be called in the Dispose method of the form, BEFORE the components of the form are disposed.
		/// </summary>
		/// <param name="formHandle">the Handle attribute of a Windows form</param>
		public void RemoveModelessDialog(IntPtr formHandle)
		{
			SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_MODELESSDIALOG, 1U, formHandle);
		}

		/// <summary>
		/// Introduced in Notepad++ 8.5.6.<br></br>
		/// NPPM_ALLOCATEINDICATOR: allocate one or more unused indicator IDs,
		/// which can then be assigned styles and used to style regions of text.<br></br>
		/// returns false and sets indicators to null if numberOfIndicators is less than 1, or if the requested number of indicators could not be allocated.<br></br>
		/// Otherwise, returns true, and sets indicators to an array of numberOfIndicators indicator IDs.<br></br>
		/// See https://www.scintilla.org/ScintillaDoc.html#Indicators for more info on the indicator API.
		/// </summary>
		/// <param name="numberOfIndicators">number of consecutive indicator IDs to allocate</param>
		/// <param name="indicators"></param>
		/// <returns></returns>
		public unsafe bool AllocateIndicators(uint numberOfIndicators, out int[] indicators)
		{
			indicators = new List<int>(0).ToArray();
			if (numberOfIndicators < 1)
				return false;
			indicators = new int[numberOfIndicators];
			fixed (int* indicatorsPtr = indicators)
			{
				IntPtr success = SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_ALLOCATEINDICATOR, numberOfIndicators, (IntPtr)indicatorsPtr);
				for (int ii = 1; ii < numberOfIndicators; ii++)
					indicators[ii] = indicators[ii - 1] + 1;
				return success != IntPtr.Zero;
			}
		}

		public void HideDockingForm(Form form)
		{
			SendMessage(PluginData.NppData.NppHandle,
					(uint)(NppMsg.NPPM_DMMHIDE),
					0, form.Handle);
		}

		public void ShowDockingForm(Form form)
		{
			SendMessage(PluginData.NppData.NppHandle,
					(uint)(NppMsg.NPPM_DMMSHOW),
					0, form.Handle);
		}

		public Color GetDefaultForegroundColor()
		{
			var rawColor = (int)SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_GETEDITORDEFAULTFOREGROUNDCOLOR);
			return Color.FromArgb(rawColor & 0xff, (rawColor >> 8) & 0xff, (rawColor >> 16) & 0xff);
		}

		public Color GetDefaultBackgroundColor()
		{
			var rawColor = (int)SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_GETEDITORDEFAULTBACKGROUNDCOLOR);
			return Color.FromArgb(rawColor & 0xff, (rawColor >> 8) & 0xff, (rawColor >> 16) & 0xff);
		}
	}
}
