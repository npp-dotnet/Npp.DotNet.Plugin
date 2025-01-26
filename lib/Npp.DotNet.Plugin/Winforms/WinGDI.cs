/*
 * SPDX-FileCopyrightText: 2024 Robert Di Pardo <https://github.com/rdipardo>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System;
using System.Runtime.InteropServices;

namespace Npp.DotNet.Plugin.Winforms
{
	public static class WinGDI
	{
		[DllImport("gdi32.dll")]
		public static extern int GetDeviceCaps(IntPtr hWnd, [MarshalAs(UnmanagedType.I4)] DeviceCapability index);

		/// <summary>
		/// Retrieves the number of logical pixels in the window or screen area bound by <paramref name="pixelsX"/>
		/// and <paramref name="pixelsY"/> physical pixels, based on <paramref name="deviceDPI"/>.
		/// </summary>
		/// <param name="pixelsX">Number of physical pixels in the x-axis of the window or screen area being measured.</param>
		/// <param name="pixelsY">Number of physical pixels in the y-axis of the window or screen area being measured.</param>
		/// <param name="deviceDPI">The DPI of the window or screen area being measured.</param>
		/// <param name="hWnd">Handle to the window or, if <see cref="Win32.NULL"/>, the screen whose area is being measured.</param>
		public static (int, int) GetLogicalPixels(int pixelsX, int pixelsY, long deviceDPI = 96L, IntPtr hWnd = default)
		{
			int logPixelsX = pixelsX;
			int logPixelsY = pixelsY;
			IntPtr hHDC = WinUser.GetDC(hWnd);

			unchecked
			{
				logPixelsX = (int)((Math.BigMul(pixelsX, GetDeviceCaps(hHDC, DeviceCapability.LOGPIXELSX)) + (deviceDPI >> 1)) / deviceDPI);
				logPixelsY = (int)((Math.BigMul(pixelsY, GetDeviceCaps(hHDC, DeviceCapability.LOGPIXELSY)) + (deviceDPI >> 1)) / deviceDPI);
			}

			_ = WinUser.ReleaseDC(Win32.NULL, hHDC);
			return (logPixelsX, logPixelsY);
		}

		/// <summary>
		/// Device parameters for <see cref="GetDeviceCaps"/>.
		/// </summary>
		/// <remarks>
		/// See <see href="https://learn.microsoft.com/windows/win32/api/wingdi/nf-wingdi-getdevicecaps"/>
		/// </remarks>
		public enum DeviceCapability
		{
			///<summary>Device driver version</summary>
			DRIVERVERSION = 0,
			///<summary>Device classification</summary>
			TECHNOLOGY = 2,
			///<summary>Horizontal size in millimeters</summary>
			HORZSIZE = 4,
			///<summary>Vertical size in millimeters</summary>
			VERTSIZE = 6,
			///<summary>Horizontal width in pixels</summary>
			HORZRES = 8,
			///<summary>Vertical height in pixels</summary>
			VERTRES = 10,
			///<summary>Number of bits per pixel</summary>
			BITSPIXEL = 12,
			///<summary>Number of planes</summary>
			PLANES = 14,
			///<summary>Number of brushes the device has</summary>
			NUMBRUSHES = 16,
			///<summary>Number of pens the device has</summary>
			NUMPENS = 18,
			///<summary>Number of markers the device has</summary>
			NUMMARKERS = 20,
			///<summary>Number of fonts the device has</summary>
			NUMFONTS = 22,
			///<summary>Number of colors the device supports</summary>
			NUMCOLORS = 24,
			///<summary>Size required for device descriptor</summary>
			PDEVICESIZE = 26,
			///<summary>Curve capabilities</summary>
			CURVECAPS = 28,
			///<summary>Line capabilities</summary>
			LINECAPS = 30,
			///<summary>Polygonal capabilities</summary>
			POLYGONALCAPS = 32,
			///<summary>Text capabilities</summary>
			TEXTCAPS = 34,
			///<summary>Clipping capabilities</summary>
			CLIPCAPS = 36,
			///<summary>Bitblt capabilities</summary>
			RASTERCAPS = 38,
			///<summary>Length of the X leg</summary>
			ASPECTX = 40,
			///<summary>Length of the Y leg</summary>
			ASPECTY = 42,
			///<summary>Length of the hypotenuse</summary>
			ASPECTXY = 44,
			///<summary>Logical pixels/inch in X</summary>
			LOGPIXELSX = 88,
			///<summary>Logical pixels/inch in Y</summary>
			LOGPIXELSY = 90,
			///<summary>Number of entries in physical palette</summary>
			SIZEPALETTE = 104,
			///<summary>Number of reserved entries in palette</summary>
			NUMRESERVED = 106,
			///<summary>Actual color resolution</summary>
			COLORRES = 108,
			///<summary>Physical width in device units</summary>
			PHYSICALWIDTH = 110,
			///<summary>Physical height in device units</summary>
			PHYSICALHEIGHT = 111,
			///<summary>Physical printable area x margin</summary>
			PHYSICALOFFSETX = 112,
			///<summary>Physical printable area y margin</summary>
			PHYSICALOFFSETY = 113,
			///<summary>Scaling factor x</summary>
			SCALINGFACTORX = 114,
			///<summary>Scaling factor y</summary>
			SCALINGFACTORY = 115,
			///<summary>Current vertical refresh rate of the display device (for displays only) in Hz</summary>
			VREFRESH = 116,
			///<summary>Horizontal width of entire desktop in pixels</summary>
			DESKTOPVERTRES = 117,
			///<summary>Vertical height of entire desktop in pixels</summary>
			DESKTOPHORZRES = 118,
			///<summary>Preferred blt alignment</summary>
			BLTALIGNMENT = 119,
			///<summary>Shading and blending caps</summary>
			SHADEBLENDCAPS = 120,
			///<summary>Color management caps</summary>
			COLORMGMTCAPS = 121
		}

		/// <summary>
		/// Possible bit flags for the return value of <see cref="GetDeviceCaps"/>
		/// when the <c>index</c> parameter is <see cref="DeviceCapability.TECHNOLOGY"/>.
		/// </summary>
		[Flags]
		public enum DeviceTechnology : uint
		{
			///<summary>Vector plotter</summary>
			DT_PLOTTER = 0,
			///<summary>Raster display</summary>
			DT_RASDISPLAY = 1,
			///<summary>Raster printer</summary>
			DT_RASPRINTER = 2,
			///<summary>Raster camera</summary>
			DT_RASCAMERA = 3,
			///<summary>Character-stream, PLP</summary>
			DT_CHARSTREAM = 4,
			///<summary>Metafile, VDM</summary>
			DT_METAFILE = 5,
			///<summary>Display-file</summary>
			DT_DISPFILE = 6
		}

		/// <summary>
		/// Possible bit flags for the return value of <see cref="GetDeviceCaps"/>
		/// when the <c>index</c> parameter is <see cref="DeviceCapability.SHADEBLENDCAPS"/>.
		/// </summary>
		[Flags]
		public enum ShadingAndBlendingCapability : uint
		{
			/// <summary>Does not support shading and blending capabilities</summary>
			SB_NONE = 0x00000000,
			/// <summary>Handles the SourceConstantAlpha member of the BLENDFUNCTION structure</summary>
			SB_CONST_ALPHA = 0x00000001,
			/// <summary>Can handle per-pixel alpha in AlphaBlend</summary>
			SB_PIXEL_ALPHA = 0x00000002,
			/// <summary>Can handle premultiplied alpha in AlphaBlend</summary>
			SB_PREMULT_ALPHA = 0x00000004,
			/// <summary>GradientFill rectangles</summary>
			SB_GRAD_RECT = 0x00000010,
			/// <summary>GradientFill triangles</summary>
			SB_GRAD_TRI = 0x00000020,
		}

		/// <summary>
		/// Possible bit flags for the return value of <see cref="GetDeviceCaps"/>
		/// when the <c>index</c> parameter is <see cref="DeviceCapability.RASTERCAPS"/>.
		/// </summary>
		public enum RasterCapability : uint
		{
			///<summary>Can do standard BLT</summary>
			RC_BITBLT = 1,
			///<summary>Device requires banding support</summary>
			RC_BANDING = 2,
			///<summary>Device requires scaling support</summary>
			RC_SCALING = 4,
			///<summary>Device can support >64K bitmap</summary>
			RC_BITMAP64 = 8,
			///<summary>Has 2.0 output calls</summary>
			RC_GDI20_OUTPUT = 0x0010,
			RC_GDI20_STATE = 0x0020,
			RC_SAVEBITMAP = 0x0040,
			///<summary>Supports DIB to memory</summary>
			RC_DI_BITMAP = 0x0080,
			///<summary>Supports a palette</summary>
			RC_PALETTE = 0x0100,
			///<summary>Supports DIBitsToDevice</summary>
			RC_DIBTODEV = 0x0200,
			///<summary>Supports >64K fonts</summary>
			RC_BIGFONT = 0x0400,
			///<summary>Supports StretchBlt</summary>
			RC_STRETCHBLT = 0x0800,
			///<summary>Supports FloodFill</summary>
			RC_FLOODFILL = 0x1000,
			///<summary>Supports StretchDIBits</summary>
			RC_STRETCHDIB = 0x2000,
			RC_OP_DX_OUTPUT = 0x4000,
			RC_DEVBITS = 0x8000,
		}

		/// <summary>
		/// Possible bit flags for the return value of <see cref="GetDeviceCaps"/>
		/// when the <c>index</c> parameter is <see cref="DeviceCapability.CURVECAPS"/>.
		/// </summary>
		[Flags]
		public enum CurveCapability : uint
		{
			///<summary>Curves not supported</summary>
			CC_NONE = 0,
			///<summary>Can do circles</summary>
			CC_CIRCLES = 1,
			///<summary>Can do pie wedges</summary>
			CC_PIE = 2,
			///<summary>Can do chord arcs</summary>
			CC_CHORD = 4,
			///<summary>Can do ellipses</summary>
			CC_ELLIPSES = 8,
			///<summary>Can do wide lines</summary>
			CC_WIDE = 16,
			///<summary>Can do styled lines</summary>
			CC_STYLED = 32,
			///<summary>Can do wide styled lines</summary>
			CC_WIDESTYLED = 64,
			///<summary>Can do interiors</summary>
			CC_INTERIORS = 128,
			///<summary>Can draw rounded rectangles</summary>
			CC_ROUNDRECT = 256,
		}

		/// <summary>
		/// Possible bit flags for the return value of <see cref="GetDeviceCaps"/>
		/// when the <c>index</c> parameter is <see cref="DeviceCapability.LINECAPS"/>.
		/// </summary>
		[Flags]
		public enum LineCapability : uint
		{
			///<summary>Lines not supported</summary>
			LC_NONE = 0,
			///<summary>Can do polylines</summary>
			LC_POLYLINE = 2,
			///<summary>Can do markers</summary>
			LC_MARKER = 4,
			///<summary>Can do polymarkers</summary>
			LC_POLYMARKER = 8,
			///<summary>Can do wide lines</summary>
			LC_WIDE = 16,
			///<summary>Can do styled lines</summary>
			LC_STYLED = 32,
			///<summary>Can do wide styled lines</summary>
			LC_WIDESTYLED = 64,
			///<summary>Can do interiors</summary>
			LC_INTERIORS = 128,
		}

		/// <summary>
		/// Possible bit flags for the return value of <see cref="GetDeviceCaps"/>
		/// when the <c>index</c> parameter is <see cref="DeviceCapability.POLYGONALCAPS"/>.
		/// </summary>
		[Flags]
		public enum PolygonalCapability : uint
		{
			///<summary>Polygons not supported</summary>
			PC_NONE = 0,
			///<summary>Can do polygons</summary>
			PC_POLYGON = 1,
			///<summary>Can do rectangles</summary>
			PC_RECTANGLE = 2,
			///<summary>Can do winding polygons</summary>
			PC_WINDPOLYGON = 4,
			///<summary>Can do trapezoids</summary>
			PC_TRAPEZOID = 4,
			///<summary>Can do scanlines</summary>
			PC_SCANLINE = 8,
			///<summary>Can do wide borders</summary>
			PC_WIDE = 16,
			///<summary>Can do styled borders</summary>
			PC_STYLED = 32,
			///<summary>Can do wide styled borders</summary>
			PC_WIDESTYLED = 64,
			///<summary>Can do interiors</summary>
			PC_INTERIORS = 128,
			///<summary>Can do polypolygons</summary>
			PC_POLYPOLYGON = 256,
			///<summary>Can do paths</summary>
			PC_PATHS = 512,
		}

		/// <summary>
		/// Possible bit flags for the return value of <see cref="GetDeviceCaps"/>
		/// when the <c>index</c> parameter is <see cref="DeviceCapability.TEXTCAPS"/>.
		/// </summary>
		[Flags]
		public enum TextCapability : uint
		{
			///<summary>Can do character output precision</summary>
			TC_OP_CHARACTER = 0x00000001,
			///<summary>Can do stroke output precision</summary>
			TC_OP_STROKE = 0x00000002,
			///<summary>Can do stroke clip precision</summary>
			TC_CP_STROKE = 0x00000004,
			///<summary>Can do 90&#176; character rotation</summary>
			TC_CR_90 = 0x00000008,
			///<summary>Can do any character rotation</summary>
			TC_CR_ANY = 0x00000010,
			///<summary>Can scale independently in the x- and y-directions</summary>
			TC_SF_X_YINDEP = 0x00000020,
			///<summary>Can do doubled character for scaling</summary>
			TC_SA_DOUBLE = 0x00000040,
			///<summary>Uses integer multiples only for character scaling</summary>
			TC_SA_INTEGER = 0x00000080,
			///<summary>Uses any multiples for exact character scaling</summary>
			TC_SA_CONTIN = 0x00000100,
			///<summary>Can do double-weight characters</summary>
			TC_EA_DOUBLE = 0x00000200,
			///<summary>Can italicize</summary>
			TC_IA_ABLE = 0x00000400,
			///<summary>Can underline</summary>
			TC_UA_ABLE = 0x00000800,
			///<summary>Can draw strikeouts</summary>
			TC_SO_ABLE = 0x00001000,
			///<summary>Can draw raster fonts</summary>
			TC_RA_ABLE = 0x00002000,
			///<summary>Can draw vector fonts</summary>
			TC_VA_ABLE = 0x00004000,
			TC_RESERVED = 0x00008000,
			///<summary><em><b>Cannot</b></em> scroll using a bit-block transfer</summary>
			TC_SCROLLBLT = 0x00010000,
		}

		/// <summary>
		/// Possible bit flags for the return value of <see cref="GetDeviceCaps"/>
		/// when the <c>index</c> parameter is <see cref="DeviceCapability.COLORMGMTCAPS"/>.
		/// </summary>
		[Flags]
		public enum ColorManagementCapability : uint
		{
			///<summary>Does not support ICM</summary>
			CM_NONE = 0x00000000,
			///<summary>Can perform ICM on either the device driver or the device itself</summary>
			CM_DEVICE_ICM = 0x00000001,
			///<summary>Supports GetDeviceGammaRamp and SetDeviceGammaRamp</summary>
			CM_GAMMA_RAMP = 0x00000002,
			///<summary>Can accept CMYK color space ICC color profile</summary>
			CM_CMYK_COLOR = 0x00000004,
		}
	}
}
