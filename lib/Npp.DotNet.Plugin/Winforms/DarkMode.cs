/*
 * SPDX-FileCopyrightText: 2025 Robert Di Pardo <https://github.com/rdipardo>
 *
 * SPDX-License-Identifier: Apache-2.0
 */
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Npp.DotNet.Plugin.Winforms
{
	/// <summary>
	/// Types and methods that interface with the Notepad++ dark mode API.
	/// </summary>
	public static class DarkMode
	{
		/// <summary>
		/// Contains the color values of the active dark mode theme.
		/// See <see href="https://github.com/notepad-plus-plus/notepad-plus-plus/blob/master/PowerEditor/src/NppDarkMode.h"/>
		/// </summary>
		public class DarkModeColors
		{
			public DarkModeColors()
			{
				IntPtr cbSize = new IntPtr(Marshal.SizeOf<Colors>());
				IntPtr pColors = Marshal.AllocHGlobal(cbSize);
				try
				{
					_ = Win32.SendMessage(PluginData.NppData.NppHandle, (uint)NppMsg.NPPM_GETDARKMODECOLORS, (uint)cbSize, pColors);
					_colors = Marshal.PtrToStructure<Colors>(pColors);
				}
				finally
				{
					Marshal.FreeHGlobal(pColors);
				}
			}

			public Color Background => DialogUtils.GetColorAlpha(new Colour(_colors.Background));
			public Color SofterBackground => DialogUtils.GetColorAlpha(new Colour(_colors.SofterBackground));
			public Color HotBackground => DialogUtils.GetColorAlpha(new Colour(_colors.HotBackground));
			public Color PureBackground => DialogUtils.GetColorAlpha(new Colour(_colors.PureBackground));
			public Color ErrorBackground => DialogUtils.GetColorAlpha(new Colour(_colors.ErrorBackground));
			public Color Text => DialogUtils.GetColorAlpha(new Colour(_colors.Text));
			public Color DarkerText => DialogUtils.GetColorAlpha(new Colour(_colors.DarkerText));
			public Color DisabledText => DialogUtils.GetColorAlpha(new Colour(_colors.DisabledText));
			public Color LinkText => DialogUtils.GetColorAlpha(new Colour(_colors.LinkText));
			public Color Edge => DialogUtils.GetColorAlpha(new Colour(_colors.Edge));
			public Color HotEdge => DialogUtils.GetColorAlpha(new Colour(_colors.HotEdge));
			public Color DisabledEdge => DialogUtils.GetColorAlpha(new Colour(_colors.DisabledEdge));

			private readonly Colors _colors;

			[StructLayout(LayoutKind.Sequential)]
			struct Colors
			{
				public int Background;
				public int SofterBackground;
				public int HotBackground;
				public int PureBackground;
				public int ErrorBackground;
				public int Text;
				public int DarkerText;
				public int DisabledText;
				public int LinkText;
				public int Edge;
				public int HotEdge;
				public int DisabledEdge;
			}
		}
	}
}
