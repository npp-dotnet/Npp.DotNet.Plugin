/*
 * SPDX-FileCopyrightText: 2016 Kasper B. Graversen <https://github.com/kbilsted>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System;
using System.Runtime.InteropServices;
using System.Text;
using static Npp.DotNet.Plugin.Win32;

namespace Npp.DotNet.Plugin
{
    /// <summary>
    /// This it the plugin-writers primary interface to Notepad++/Scintilla.
    /// It takes away all the complexity with command numbers and Int-pointer casting.
    ///
    /// See http://www.scintilla.org/ScintillaDoc.html for further details.
    /// </summary>
    public class ScintillaGateway : IScintillaGateway
    {
        private static readonly UIntPtr UnusedW = UIntPtr.Zero;
        private static readonly IntPtr Unused = IntPtr.Zero;
        private readonly IntPtr _scintilla;

        public static readonly int LengthZeroTerminator = "\0".Length;

        /// <summary>
        /// returns bytes decoded from UTF-8 as a string, with all trailing NULL bytes stripped off.
        /// </summary>
        public static string Utf8BytesToNullStrippedString(byte[] bytes)
        {
            int lastNullCharPos = bytes.Length - 1;
            // this only bypasses NULL chars because no char
            // other than NULL can have any 0-valued bytes in UTF-8.
            // See https://en.wikipedia.org/wiki/UTF-8#Encoding
            for (; lastNullCharPos >= 0 && bytes[lastNullCharPos] == '\x00'; lastNullCharPos--) { }
            return Encoding.UTF8.GetString(bytes, 0, lastNullCharPos + 1);
        }

        /// <summary>
        /// Recall that all Scintilla methods have the signature
        /// (scintilla* scin, SciMsg msg, void* wParam, void* lParam) -&gt; void*<br></br>
        /// Many of these scintilla methods are bimodal in the following way<br></br>
        /// * if lParam is 0, return the length of the buffer to be filled and have no side effects. The wParam may be involved in telling Scintilla how big the buffer needs to be.<br></br>
        /// * if lParam is greater than 0, it is assumed to be a pointer to a buffer. Now the wParam indicates what the text will need to be.<br></br><br></br>
        /// This sets lParam to 0 to get the length, allocates a buffer of that length,<br></br>
        /// uses the second mode to fill a buffer,<br></br>
        /// and returns a string of the UTF8-decoded buffer with all trailing '\x00' chars stripped off.
        /// </summary>
        /// <param name="msg">message to send</param>
        /// <param name="wParam">another parameter for defining what the buffer should contain</param>
        /// <returns></returns>
        private unsafe string GetNullStrippedStringFromMessageThatReturnsLength(SciMsg msg, UIntPtr wParam = default)
        {
            int length = SendMessage(_scintilla, msg, wParam, Unused).ToInt32();
            byte[] textBuffer = new byte[length];
            fixed (byte* textPtr = textBuffer)
            {
                SendMessage(_scintilla, msg, wParam, (IntPtr)textPtr);
                return Utf8BytesToNullStrippedString(textBuffer);
            }
        }

        public ScintillaGateway(IntPtr scintilla)
        {
            this._scintilla = scintilla;
        }

        public int GetSelectionLength()
        {
            var selectionLength = (int)SendMessage(_scintilla, SciMsg.SCI_GETSELTEXT, UnusedW, Unused) - LengthZeroTerminator;
            return selectionLength;
        }

        public void AppendTextAndMoveCursor(string text)
        {
            AppendText(text.Length, text);
            GotoPos(GetCurrentPos() + text.Length);
        }

        public void InsertTextAndMoveCursor(string text)
        {
            var currentPos = GetCurrentPos();
            InsertText(currentPos, text);
            GotoPos(currentPos + text.Length);
        }

        public void SelectCurrentLine()
        {
            int line = GetCurrentLineNumber();
            SetSelection(PositionFromLine(line), PositionFromLine(line + 1));
        }

        /// <summary>
        /// clears the selection without changing the position of the cursor
        /// </summary>
        public void ClearSelectionToCursor()
        {
            var pos = GetCurrentPos();
            SetSelection(pos, pos);
        }

        /// <summary>
        /// Get the current line from the current position
        /// </summary>
        public int GetCurrentLineNumber()
        {
            return LineFromPosition(GetCurrentPos());
        }

        /// <summary>
        /// Get the scroll information for the current Scintilla window.
        /// </summary>
        /// <param name="mask">Arguments for the scroll information such as tracking</param>
        /// <param name="scrollBar">Which scroll bar information are you looking for</param>
        /// <returns>A ScrollInfo struct with information of the current scroll state</returns>
        public ScrollInfo GetScrollInfo(ScrollInfoMask mask = ScrollInfoMask.SIF_ALL, ScrollInfoBar scrollBar = ScrollInfoBar.SB_BOTH)
        {
            ScrollInfo scrollInfo = new ScrollInfo();
            scrollInfo.cbSize = (uint)Marshal.SizeOf(scrollInfo);
            scrollInfo.fMask = (uint)mask;
            _ = Win32.GetScrollInfo(_scintilla, (int)scrollBar, ref scrollInfo);
            return scrollInfo;
        }

        /* ++Autogenerated -- start of section automatically generated from Scintilla.iface */
        /// <summary>Add text to the document at current position. (Scintilla feature 2001)</summary>
        public unsafe void AddText(int length, string text)
        {
            fixed (byte* textPtr = Encoding.UTF8.GetBytes(text))
            {
                SendMessage(_scintilla, SciMsg.SCI_ADDTEXT, (UIntPtr)length, (IntPtr)textPtr);
            }
        }

        /// <summary>Add array of cells to document. (Scintilla feature 2002)</summary>
        public unsafe void AddStyledText(int length, Cells c)
        {
            fixed (char* cPtr = c.Value)
            {
                SendMessage(_scintilla, SciMsg.SCI_ADDSTYLEDTEXT, (UIntPtr)length, (IntPtr)cPtr);
            }
        }

        /// <summary>Insert string at a position. (Scintilla feature 2003)</summary>
        public unsafe void InsertText(int pos, string text)
        {
            fixed (byte* textPtr = Encoding.UTF8.GetBytes(text))
            {
                SendMessage(_scintilla, SciMsg.SCI_INSERTTEXT, (UIntPtr)pos, (IntPtr)textPtr);
            }
        }

        /// <summary>Change the text that is being inserted in response to SC_MOD_INSERTCHECK (Scintilla feature 2672)</summary>
        public unsafe void ChangeInsertion(int length, string text)
        {
            fixed (byte* textPtr = Encoding.UTF8.GetBytes(text))
            {
                SendMessage(_scintilla, SciMsg.SCI_CHANGEINSERTION, (UIntPtr)length, (IntPtr)textPtr);
            }
        }

        /// <summary>Delete all text in the document. (Scintilla feature 2004)</summary>
        public void ClearAll()
        {
            SendMessage(_scintilla, SciMsg.SCI_CLEARALL, UnusedW, Unused);
        }

        /// <summary>Delete a range of text in the document. (Scintilla feature 2645)</summary>
        public void DeleteRange(int start, int lengthDelete)
        {
            SendMessage(_scintilla, SciMsg.SCI_DELETERANGE, (UIntPtr)start, (IntPtr)lengthDelete);
        }

        /// <summary>Set all style bytes to 0, remove all folding information. (Scintilla feature 2005)</summary>
        public void ClearDocumentStyle()
        {
            SendMessage(_scintilla, SciMsg.SCI_CLEARDOCUMENTSTYLE, UnusedW, Unused);
        }

        /// <summary>Returns the number of bytes in the document. (Scintilla feature 2006)</summary>
        public int GetLength()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETLENGTH, UnusedW, Unused);
        }

        /// <summary>Returns the character byte at the position. (Scintilla feature 2007)</summary>
        public int GetCharAt(int pos)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETCHARAT, (UIntPtr)pos, Unused);
        }

        /// <summary>Returns the position of the caret. (Scintilla feature 2008)</summary>
        public int GetCurrentPos()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETCURRENTPOS, UnusedW, Unused);
        }

        /// <summary>Returns the position of the opposite end of the selection to the caret. (Scintilla feature 2009)</summary>
        public int GetAnchor()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETANCHOR, UnusedW, Unused);
        }

        /// <summary>Returns the style byte at the position. (Scintilla feature 2010)</summary>
        public int GetStyleAt(int pos)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETSTYLEAT, (UIntPtr)pos, Unused);
        }

        /// <summary>Redoes the next action on the undo history. (Scintilla feature 2011)</summary>
        public void Redo()
        {
            SendMessage(_scintilla, SciMsg.SCI_REDO, UnusedW, Unused);
        }

        /// <summary>
        /// Choose between collecting actions into the undo
        /// history and discarding them.
        /// (Scintilla feature 2012)
        /// </summary>
        public void SetUndoCollection(bool collectUndo)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETUNDOCOLLECTION, new UIntPtr(collectUndo ? 1U : 0U), Unused);
        }

        /// <summary>Select all the text in the document. (Scintilla feature 2013)</summary>
        public void SelectAll()
        {
            SendMessage(_scintilla, SciMsg.SCI_SELECTALL, UnusedW, Unused);
        }

        /// <summary>
        /// Remember the current position in the undo history as the position
        /// at which the document was saved.
        /// (Scintilla feature 2014)
        /// </summary>
        public void SetSavePoint()
        {
            SendMessage(_scintilla, SciMsg.SCI_SETSAVEPOINT, UnusedW, Unused);
        }

        /// <summary>
        /// Retrieve a buffer of cells.
        /// Returns the number of bytes in the buffer not including terminating NULs.
        /// (Scintilla feature 2015)
        /// </summary>
        public int GetStyledText(TextRange tr)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETSTYLEDTEXT, UnusedW, tr.NativePointer);
        }

        /// <summary>Are there any redoable actions in the undo history? (Scintilla feature 2016)</summary>
        public bool CanRedo()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_CANREDO, UnusedW, Unused);
        }

        /// <summary>Retrieve the line number at which a particular marker is located. (Scintilla feature 2017)</summary>
        public int MarkerLineFromHandle(int markerHandle)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_MARKERLINEFROMHANDLE, (UIntPtr)markerHandle, Unused);
        }

        /// <summary>Delete a marker. (Scintilla feature 2018)</summary>
        public void MarkerDeleteHandle(int markerHandle)
        {
            SendMessage(_scintilla, SciMsg.SCI_MARKERDELETEHANDLE, (UIntPtr)markerHandle, Unused);
        }

        /// <summary>Is undo history being collected? (Scintilla feature 2019)</summary>
        public bool GetUndoCollection()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETUNDOCOLLECTION, UnusedW, Unused);
        }

        /// <summary>
        /// Are white space characters currently visible?
        /// Returns one of SCWS_* constants.
        /// (Scintilla feature 2020)
        /// </summary>
        public WhiteSpace GetViewWS()
        {
            return (WhiteSpace)SendMessage(_scintilla, SciMsg.SCI_GETVIEWWS, UnusedW, Unused);
        }

        /// <summary>Make white space characters invisible, always visible or visible outside indentation. (Scintilla feature 2021)</summary>
        public void SetViewWS(WhiteSpace viewWS)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETVIEWWS, (UIntPtr)viewWS, Unused);
        }

        /// <summary>
        /// Retrieve the current tab draw mode.
        /// Returns one of SCTD_* constants.
        /// (Scintilla feature 2698)
        /// </summary>
        public TabDrawMode GetTabDrawMode()
        {
            return (TabDrawMode)SendMessage(_scintilla, SciMsg.SCI_GETTABDRAWMODE, UnusedW, Unused);
        }

        /// <summary>Set how tabs are drawn when visible. (Scintilla feature 2699)</summary>
        public void SetTabDrawMode(TabDrawMode tabDrawMode)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETTABDRAWMODE, (UIntPtr)tabDrawMode, Unused);
        }

        /// <summary>Find the position from a point within the window. (Scintilla feature 2022)</summary>
        public int PositionFromPoint(int x, int y)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_POSITIONFROMPOINT, (UIntPtr)x, (IntPtr)y);
        }

        /// <summary>
        /// Find the position from a point within the window but return
        /// INVALID_POSITION if not close to text.
        /// (Scintilla feature 2023)
        /// </summary>
        public int PositionFromPointClose(int x, int y)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_POSITIONFROMPOINTCLOSE, (UIntPtr)x, (IntPtr)y);
        }

        /// <summary>Set caret to start of a line and ensure it is visible. (Scintilla feature 2024)</summary>
        public void GotoLine(int line)
        {
            SendMessage(_scintilla, SciMsg.SCI_GOTOLINE, (UIntPtr)line, Unused);
        }

        /// <summary>Set caret to a position and ensure it is visible. (Scintilla feature 2025)</summary>
        public void GotoPos(int caret)
        {
            SendMessage(_scintilla, SciMsg.SCI_GOTOPOS, (UIntPtr)caret, Unused);
        }

        /// <summary>
        /// Set the selection anchor to a position. The anchor is the opposite
        /// end of the selection from the caret.
        /// (Scintilla feature 2026)
        /// </summary>
        public void SetAnchor(int anchor)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETANCHOR, (UIntPtr)anchor, Unused);
        }

        /// <summary>
        /// Retrieve the text of the line containing the caret.
        /// Returns the index of the caret on the line.
        /// Result is NUL-terminated.
        /// (Scintilla feature 2027)
        /// </summary>
        public unsafe string GetCurLine()
        {
            return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_GETCURLINE);
        }

        /// <summary>Retrieve the position of the last correctly styled character. (Scintilla feature 2028)</summary>
        public int GetEndStyled()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETENDSTYLED, UnusedW, Unused);
        }

        /// <summary>Convert all line endings in the document to one mode. (Scintilla feature 2029)</summary>
        public void ConvertEOLs(EndOfLine eolMode)
        {
            SendMessage(_scintilla, SciMsg.SCI_CONVERTEOLS, (UIntPtr)eolMode, Unused);
        }

        /// <summary>Retrieve the current end of line mode - one of CRLF, CR, or LF. (Scintilla feature 2030)</summary>
        public EndOfLine GetEOLMode()
        {
            return (EndOfLine)SendMessage(_scintilla, SciMsg.SCI_GETEOLMODE, UnusedW, Unused);
        }

        /// <summary>Set the current end of line mode. (Scintilla feature 2031)</summary>
        public void SetEOLMode(EndOfLine eolMode)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETEOLMODE, (UIntPtr)eolMode, Unused);
        }

        /// <summary>
        /// Set the current styling position to start.
        /// The unused parameter is no longer used and should be set to 0.
        /// (Scintilla feature 2032)
        /// </summary>
        public void StartStyling(int start, int unused)
        {
            SendMessage(_scintilla, SciMsg.SCI_STARTSTYLING, (UIntPtr)start, Unused);
        }

        /// <summary>
        /// Change style from current styling position for length characters to a style
        /// and move the current styling position to after this newly styled segment.
        /// (Scintilla feature 2033)
        /// </summary>
        public void SetStyling(int length, int style)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETSTYLING, (UIntPtr)length, (IntPtr)style);
        }

        /// <summary>Is drawing done first into a buffer or direct to the screen? (Scintilla feature 2034)</summary>
        public bool GetBufferedDraw()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETBUFFEREDDRAW, UnusedW, Unused);
        }

        /// <summary>
        /// If drawing is buffered then each line of text is drawn into a bitmap buffer
        /// before drawing it to the screen to avoid flicker.
        /// (Scintilla feature 2035)
        /// </summary>
        public void SetBufferedDraw(bool buffered)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETBUFFEREDDRAW, new UIntPtr(buffered ? 1U : 0U), Unused);
        }

        /// <summary>Change the visible size of a tab to be a multiple of the width of a space character. (Scintilla feature 2036)</summary>
        public void SetTabWidth(int tabWidth)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETTABWIDTH, (UIntPtr)tabWidth, Unused);
        }

        /// <summary>Retrieve the visible size of a tab. (Scintilla feature 2121)</summary>
        public int GetTabWidth()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETTABWIDTH, UnusedW, Unused);
        }

        /// <summary>Clear explicit tabstops on a line. (Scintilla feature 2675)</summary>
        public void ClearTabStops(int line)
        {
            SendMessage(_scintilla, SciMsg.SCI_CLEARTABSTOPS, (UIntPtr)line, Unused);
        }

        /// <summary>Add an explicit tab stop for a line. (Scintilla feature 2676)</summary>
        public void AddTabStop(int line, int x)
        {
            SendMessage(_scintilla, SciMsg.SCI_ADDTABSTOP, (UIntPtr)line, (IntPtr)x);
        }

        /// <summary>Find the next explicit tab stop position on a line after a position. (Scintilla feature 2677)</summary>
        public int GetNextTabStop(int line, int x)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETNEXTTABSTOP, (UIntPtr)line, (IntPtr)x);
        }

        /// <summary>
        /// Set the code page used to interpret the bytes of the document as characters.
        /// The SC_CP_UTF8 value can be used to enter Unicode mode.
        /// (Scintilla feature 2037)
        /// </summary>
        public void SetCodePage(int codePage)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETCODEPAGE, (UIntPtr)codePage, Unused);
        }

        /// <summary>Is the IME displayed in a window or inline? (Scintilla feature 2678)</summary>
        public IMEInteraction GetIMEInteraction()
        {
            return (IMEInteraction)SendMessage(_scintilla, SciMsg.SCI_GETIMEINTERACTION, UnusedW, Unused);
        }

        /// <summary>Choose to display the the IME in a winow or inline. (Scintilla feature 2679)</summary>
        public void SetIMEInteraction(IMEInteraction imeInteraction)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETIMEINTERACTION, (UIntPtr)imeInteraction, Unused);
        }

        /// <summary>Set the symbol used for a particular marker number. (Scintilla feature 2040)</summary>
        public void MarkerDefine(int markerNumber, MarkerSymbol markerSymbol)
        {
            SendMessage(_scintilla, SciMsg.SCI_MARKERDEFINE, (UIntPtr)markerNumber, (IntPtr)markerSymbol);
        }

        /// <summary>Set the foreground colour used for a particular marker number. (Scintilla feature 2041)</summary>
        public void MarkerSetFore(int markerNumber, Colour fore)
        {
            SendMessage(_scintilla, SciMsg.SCI_MARKERSETFORE, (UIntPtr)markerNumber, fore.Value);
        }

        /// <summary>Set the background colour used for a particular marker number. (Scintilla feature 2042)</summary>
        public void MarkerSetBack(int markerNumber, Colour back)
        {
            SendMessage(_scintilla, SciMsg.SCI_MARKERSETBACK, (UIntPtr)markerNumber, back.Value);
        }

        /// <summary>Set the background colour used for a particular marker number when its folding block is selected. (Scintilla feature 2292)</summary>
        public void MarkerSetBackSelected(int markerNumber, Colour back)
        {
            SendMessage(_scintilla, SciMsg.SCI_MARKERSETBACKSELECTED, (UIntPtr)markerNumber, back.Value);
        }

        /// <summary>Enable/disable highlight for current folding bloc (smallest one that contains the caret) (Scintilla feature 2293)</summary>
        public void MarkerEnableHighlight(bool enabled)
        {
            SendMessage(_scintilla, SciMsg.SCI_MARKERENABLEHIGHLIGHT, new UIntPtr(enabled ? 1U : 0U), Unused);
        }

        /// <summary>Add a marker to a line, returning an ID which can be used to find or delete the marker. (Scintilla feature 2043)</summary>
        public int MarkerAdd(int line, int markerNumber)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_MARKERADD, (UIntPtr)line, (IntPtr)markerNumber);
        }

        /// <summary>Delete a marker from a line. (Scintilla feature 2044)</summary>
        public void MarkerDelete(int line, int markerNumber)
        {
            SendMessage(_scintilla, SciMsg.SCI_MARKERDELETE, (UIntPtr)line, (IntPtr)markerNumber);
        }

        /// <summary>Delete all markers with a particular number from all lines. (Scintilla feature 2045)</summary>
        public void MarkerDeleteAll(int markerNumber)
        {
            SendMessage(_scintilla, SciMsg.SCI_MARKERDELETEALL, (UIntPtr)markerNumber, Unused);
        }

        /// <summary>Get a bit mask of all the markers set on a line. (Scintilla feature 2046)</summary>
        public int MarkerGet(int line)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_MARKERGET, (UIntPtr)line, Unused);
        }

        /// <summary>
        /// Find the next line at or after lineStart that includes a marker in mask.
        /// Return -1 when no more lines.
        /// (Scintilla feature 2047)
        /// </summary>
        public int MarkerNext(int lineStart, int markerMask)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_MARKERNEXT, (UIntPtr)lineStart, (IntPtr)markerMask);
        }

        /// <summary>Find the previous line before lineStart that includes a marker in mask. (Scintilla feature 2048)</summary>
        public int MarkerPrevious(int lineStart, int markerMask)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_MARKERPREVIOUS, (UIntPtr)lineStart, (IntPtr)markerMask);
        }

        /// <summary>Define a marker from a pixmap. (Scintilla feature 2049)</summary>
        public unsafe void MarkerDefinePixmap(int markerNumber, string pixmap)
        {
            fixed (byte* pixmapPtr = Encoding.UTF8.GetBytes(pixmap))
            {
                SendMessage(_scintilla, SciMsg.SCI_MARKERDEFINEPIXMAP, (UIntPtr)markerNumber, (IntPtr)pixmapPtr);
            }
        }

        /// <summary>Add a set of markers to a line. (Scintilla feature 2466)</summary>
        public void MarkerAddSet(int line, int markerSet)
        {
            SendMessage(_scintilla, SciMsg.SCI_MARKERADDSET, (UIntPtr)line, (IntPtr)markerSet);
        }

        /// <summary>Set the alpha used for a marker that is drawn in the text area, not the margin. (Scintilla feature 2476)</summary>
        public void MarkerSetAlpha(int markerNumber, Alpha alpha)
        {
            SendMessage(_scintilla, SciMsg.SCI_MARKERSETALPHA, (UIntPtr)markerNumber, (IntPtr)alpha);
        }

        /// <summary>Set a margin to be either numeric or symbolic. (Scintilla feature 2240)</summary>
        public void SetMarginTypeN(int margin, MarginType marginType)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETMARGINTYPEN, (UIntPtr)margin, (IntPtr)marginType);
        }

        /// <summary>Retrieve the type of a margin. (Scintilla feature 2241)</summary>
        public MarginType GetMarginTypeN(int margin)
        {
            return (MarginType)SendMessage(_scintilla, SciMsg.SCI_GETMARGINTYPEN, (UIntPtr)margin, Unused);
        }

        /// <summary>Set the width of a margin to a width expressed in pixels. (Scintilla feature 2242)</summary>
        public void SetMarginWidthN(int margin, int pixelWidth)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETMARGINWIDTHN, (UIntPtr)margin, (IntPtr)pixelWidth);
        }

        /// <summary>Retrieve the width of a margin in pixels. (Scintilla feature 2243)</summary>
        public int GetMarginWidthN(int margin)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETMARGINWIDTHN, (UIntPtr)margin, Unused);
        }

        /// <summary>Set a mask that determines which markers are displayed in a margin. (Scintilla feature 2244)</summary>
        public void SetMarginMaskN(int margin, int mask)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETMARGINMASKN, (UIntPtr)margin, (IntPtr)mask);
        }

        /// <summary>Retrieve the marker mask of a margin. (Scintilla feature 2245)</summary>
        public int GetMarginMaskN(int margin)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETMARGINMASKN, (UIntPtr)margin, Unused);
        }

        /// <summary>Make a margin sensitive or insensitive to mouse clicks. (Scintilla feature 2246)</summary>
        public void SetMarginSensitiveN(int margin, bool sensitive)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETMARGINSENSITIVEN, (UIntPtr)margin, new IntPtr(sensitive ? 1 : 0));
        }

        /// <summary>Retrieve the mouse click sensitivity of a margin. (Scintilla feature 2247)</summary>
        public bool GetMarginSensitiveN(int margin)
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETMARGINSENSITIVEN, (UIntPtr)margin, Unused);
        }

        /// <summary>Set the cursor shown when the mouse is inside a margin. (Scintilla feature 2248)</summary>
        public void SetMarginCursorN(int margin, CursorShape cursor)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETMARGINCURSORN, (UIntPtr)margin, (IntPtr)cursor);
        }

        /// <summary>Retrieve the cursor shown in a margin. (Scintilla feature 2249)</summary>
        public CursorShape GetMarginCursorN(int margin)
        {
            return (CursorShape)SendMessage(_scintilla, SciMsg.SCI_GETMARGINCURSORN, (UIntPtr)margin, Unused);
        }

        /// <summary>Set the background colour of a margin. Only visible for SC_MARGIN_COLOUR. (Scintilla feature 2250)</summary>
        public void SetMarginBackN(int margin, Colour back)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETMARGINBACKN, (UIntPtr)margin, back.Value);
        }

        /// <summary>Retrieve the background colour of a margin (Scintilla feature 2251)</summary>
        public Colour GetMarginBackN(int margin)
        {
            return new Colour((int)SendMessage(_scintilla, SciMsg.SCI_GETMARGINBACKN, (UIntPtr)margin, Unused));
        }

        /// <summary>Allocate a non-standard number of margins. (Scintilla feature 2252)</summary>
        public void SetMargins(int margins)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETMARGINS, (UIntPtr)margins, Unused);
        }

        /// <summary>How many margins are there?. (Scintilla feature 2253)</summary>
        public int GetMargins()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETMARGINS, UnusedW, Unused);
        }

        /// <summary>Clear all the styles and make equivalent to the global default style. (Scintilla feature 2050)</summary>
        public void StyleClearAll()
        {
            SendMessage(_scintilla, SciMsg.SCI_STYLECLEARALL, UnusedW, Unused);
        }

        /// <summary>Set the foreground colour of a style. (Scintilla feature 2051)</summary>
        public void StyleSetFore(int style, Colour fore)
        {
            SendMessage(_scintilla, SciMsg.SCI_STYLESETFORE, (UIntPtr)style, fore.Value);
        }

        /// <summary>Set the background colour of a style. (Scintilla feature 2052)</summary>
        public void StyleSetBack(int style, Colour back)
        {
            SendMessage(_scintilla, SciMsg.SCI_STYLESETBACK, (UIntPtr)style, back.Value);
        }

        /// <summary>Set a style to be bold or not. (Scintilla feature 2053)</summary>
        public void StyleSetBold(int style, bool bold)
        {
            SendMessage(_scintilla, SciMsg.SCI_STYLESETBOLD, (UIntPtr)style, new IntPtr(bold ? 1 : 0));
        }

        /// <summary>Set a style to be italic or not. (Scintilla feature 2054)</summary>
        public void StyleSetItalic(int style, bool italic)
        {
            SendMessage(_scintilla, SciMsg.SCI_STYLESETITALIC, (UIntPtr)style, new IntPtr(italic ? 1 : 0));
        }

        /// <summary>Set the size of characters of a style. (Scintilla feature 2055)</summary>
        public void StyleSetSize(int style, int sizePoints)
        {
            SendMessage(_scintilla, SciMsg.SCI_STYLESETSIZE, (UIntPtr)style, (IntPtr)sizePoints);
        }

        /// <summary>Set the font of a style. (Scintilla feature 2056)</summary>
        public unsafe void StyleSetFont(int style, string fontName)
        {
            fixed (byte* fontNamePtr = Encoding.UTF8.GetBytes(fontName))
            {
                SendMessage(_scintilla, SciMsg.SCI_STYLESETFONT, (UIntPtr)style, (IntPtr)fontNamePtr);
            }
        }

        /// <summary>Set a style to have its end of line filled or not. (Scintilla feature 2057)</summary>
        public void StyleSetEOLFilled(int style, bool eolFilled)
        {
            SendMessage(_scintilla, SciMsg.SCI_STYLESETEOLFILLED, (UIntPtr)style, new IntPtr(eolFilled ? 1 : 0));
        }

        /// <summary>Reset the default style to its state at startup (Scintilla feature 2058)</summary>
        public void StyleResetDefault()
        {
            SendMessage(_scintilla, SciMsg.SCI_STYLERESETDEFAULT, UnusedW, Unused);
        }

        /// <summary>Set a style to be underlined or not. (Scintilla feature 2059)</summary>
        public void StyleSetUnderline(int style, bool underline)
        {
            SendMessage(_scintilla, SciMsg.SCI_STYLESETUNDERLINE, (UIntPtr)style, new IntPtr(underline ? 1 : 0));
        }

        /// <summary>Get the foreground colour of a style. (Scintilla feature 2481)</summary>
        public Colour StyleGetFore(int style)
        {
            return new Colour((int)SendMessage(_scintilla, SciMsg.SCI_STYLEGETFORE, (UIntPtr)style, Unused));
        }

        /// <summary>Get the background colour of a style. (Scintilla feature 2482)</summary>
        public Colour StyleGetBack(int style)
        {
            return new Colour((int)SendMessage(_scintilla, SciMsg.SCI_STYLEGETBACK, (UIntPtr)style, Unused));
        }

        /// <summary>Get is a style bold or not. (Scintilla feature 2483)</summary>
        public bool StyleGetBold(int style)
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_STYLEGETBOLD, (UIntPtr)style, Unused);
        }

        /// <summary>Get is a style italic or not. (Scintilla feature 2484)</summary>
        public bool StyleGetItalic(int style)
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_STYLEGETITALIC, (UIntPtr)style, Unused);
        }

        /// <summary>Get the size of characters of a style. (Scintilla feature 2485)</summary>
        public int StyleGetSize(int style)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_STYLEGETSIZE, (UIntPtr)style, Unused);
        }

        /// <summary>
        /// Get the font of a style.
        /// Returns the length of the fontName
        /// Result is NUL-terminated.
        /// (Scintilla feature 2486)
        /// </summary>
        public unsafe string StyleGetFont(int style)
        {
            return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_STYLEGETFONT, (UIntPtr)style);
        }

        /// <summary>Get is a style to have its end of line filled or not. (Scintilla feature 2487)</summary>
        public bool StyleGetEOLFilled(int style)
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_STYLEGETEOLFILLED, (UIntPtr)style, Unused);
        }

        /// <summary>Get is a style underlined or not. (Scintilla feature 2488)</summary>
        public bool StyleGetUnderline(int style)
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_STYLEGETUNDERLINE, (UIntPtr)style, Unused);
        }

        /// <summary>Get is a style mixed case, or to force upper or lower case. (Scintilla feature 2489)</summary>
        public CaseVisible StyleGetCase(int style)
        {
            return (CaseVisible)SendMessage(_scintilla, SciMsg.SCI_STYLEGETCASE, (UIntPtr)style, Unused);
        }

        /// <summary>Get the character get of the font in a style. (Scintilla feature 2490)</summary>
        public CharacterSet StyleGetCharacterSet(int style)
        {
            return (CharacterSet)SendMessage(_scintilla, SciMsg.SCI_STYLEGETCHARACTERSET, (UIntPtr)style, Unused);
        }

        /// <summary>Get is a style visible or not. (Scintilla feature 2491)</summary>
        public bool StyleGetVisible(int style)
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_STYLEGETVISIBLE, (UIntPtr)style, Unused);
        }

        /// <summary>
        /// Get is a style changeable or not (read only).
        /// Experimental feature, currently buggy.
        /// (Scintilla feature 2492)
        /// </summary>
        public bool StyleGetChangeable(int style)
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_STYLEGETCHANGEABLE, (UIntPtr)style, Unused);
        }

        /// <summary>Get is a style a hotspot or not. (Scintilla feature 2493)</summary>
        public bool StyleGetHotSpot(int style)
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_STYLEGETHOTSPOT, (UIntPtr)style, Unused);
        }

        /// <summary>Set a style to be mixed case, or to force upper or lower case. (Scintilla feature 2060)</summary>
        public void StyleSetCase(int style, CaseVisible caseVisible)
        {
            SendMessage(_scintilla, SciMsg.SCI_STYLESETCASE, (UIntPtr)style, (IntPtr)caseVisible);
        }

        /// <summary>Set the size of characters of a style. Size is in points multiplied by 100. (Scintilla feature 2061)</summary>
        public void StyleSetSizeFractional(int style, int sizeHundredthPoints)
        {
            SendMessage(_scintilla, SciMsg.SCI_STYLESETSIZEFRACTIONAL, (UIntPtr)style, (IntPtr)sizeHundredthPoints);
        }

        /// <summary>Get the size of characters of a style in points multiplied by 100 (Scintilla feature 2062)</summary>
        public int StyleGetSizeFractional(int style)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_STYLEGETSIZEFRACTIONAL, (UIntPtr)style, Unused);
        }

        /// <summary>Set the weight of characters of a style. (Scintilla feature 2063)</summary>
        public void StyleSetWeight(int style, FontWeight weight)
        {
            SendMessage(_scintilla, SciMsg.SCI_STYLESETWEIGHT, (UIntPtr)style, (IntPtr)weight);
        }

        /// <summary>Get the weight of characters of a style. (Scintilla feature 2064)</summary>
        public FontWeight StyleGetWeight(int style)
        {
            return (FontWeight)SendMessage(_scintilla, SciMsg.SCI_STYLEGETWEIGHT, (UIntPtr)style, Unused);
        }

        /// <summary>Set the character set of the font in a style. (Scintilla feature 2066)</summary>
        public void StyleSetCharacterSet(int style, CharacterSet characterSet)
        {
            SendMessage(_scintilla, SciMsg.SCI_STYLESETCHARACTERSET, (UIntPtr)style, (IntPtr)characterSet);
        }

        /// <summary>Set a style to be a hotspot or not. (Scintilla feature 2409)</summary>
        public void StyleSetHotSpot(int style, bool hotspot)
        {
            SendMessage(_scintilla, SciMsg.SCI_STYLESETHOTSPOT, (UIntPtr)style, new IntPtr(hotspot ? 1 : 0));
        }

        /// <inheritdoc cref="SciMsg.SCI_SETSELFORE"/>
        [Obsolete("Use the element colours APIs instead: https://www.scintilla.org/ScintillaDoc.html#ElementColours")]
        public void SetSelFore(bool useSetting, Colour fore)
        {
        }

        /// <inheritdoc cref="SciMsg.SCI_SETSELBACK"/>
        [Obsolete("Use the element colours APIs instead: https://www.scintilla.org/ScintillaDoc.html#ElementColours")]
        public void SetSelBack(bool useSetting, Colour back)
        {
        }

        /// <inheritdoc cref="SciMsg.SCI_GETSELALPHA"/>
        [Obsolete("Use the element colours APIs instead: https://www.scintilla.org/ScintillaDoc.html#ElementColours")]
        public Alpha GetSelAlpha() => default;

        /// <inheritdoc cref="SciMsg.SCI_SETSELALPHA"/>
        [Obsolete("Use the element colours APIs instead: https://www.scintilla.org/ScintillaDoc.html#ElementColours")]
        public void SetSelAlpha(Alpha alpha)
        {
        }

        /// <summary>Is the selection end of line filled? (Scintilla feature 2479)</summary>
        public bool GetSelEOLFilled()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETSELEOLFILLED, UnusedW, Unused);
        }

        /// <summary>Set the selection to have its end of line filled or not. (Scintilla feature 2480)</summary>
        public void SetSelEOLFilled(bool filled)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETSELEOLFILLED, new UIntPtr(filled ? 1U : 0U), Unused);
        }

        /// <inheritdoc cref="SciMsg.SCI_SETCARETFORE"/>
        [Obsolete("Use the element colours APIs instead: https://www.scintilla.org/ScintillaDoc.html#ElementColours")]
        public void SetCaretFore(Colour fore)
        {
        }

        /// <summary>When key+modifier combination keyDefinition is pressed perform sciCommand. (Scintilla feature 2070)</summary>
        public void AssignCmdKey(KeyModifier keyDefinition, int sciCommand)
        {
            SendMessage(_scintilla, SciMsg.SCI_ASSIGNCMDKEY, (UIntPtr)keyDefinition.Value, (IntPtr)sciCommand);
        }

        /// <summary>When key+modifier combination keyDefinition is pressed do nothing. (Scintilla feature 2071)</summary>
        public void ClearCmdKey(KeyModifier keyDefinition)
        {
            SendMessage(_scintilla, SciMsg.SCI_CLEARCMDKEY, (UIntPtr)keyDefinition.Value, Unused);
        }

        /// <summary>Drop all key mappings. (Scintilla feature 2072)</summary>
        public void ClearAllCmdKeys()
        {
            SendMessage(_scintilla, SciMsg.SCI_CLEARALLCMDKEYS, UnusedW, Unused);
        }

        /// <summary>Set the styles for a segment of the document. (Scintilla feature 2073)</summary>
        public unsafe void SetStylingEx(int length, string styles)
        {
            fixed (byte* stylesPtr = Encoding.UTF8.GetBytes(styles))
            {
                SendMessage(_scintilla, SciMsg.SCI_SETSTYLINGEX, (UIntPtr)length, (IntPtr)stylesPtr);
            }
        }

        /// <summary>Set a style to be visible or not. (Scintilla feature 2074)</summary>
        public void StyleSetVisible(int style, bool visible)
        {
            SendMessage(_scintilla, SciMsg.SCI_STYLESETVISIBLE, (UIntPtr)style, new IntPtr(visible ? 1 : 0));
        }

        /// <summary>Get the time in milliseconds that the caret is on and off. (Scintilla feature 2075)</summary>
        public int GetCaretPeriod()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETCARETPERIOD, UnusedW, Unused);
        }

        /// <summary>Get the time in milliseconds that the caret is on and off. 0 = steady on. (Scintilla feature 2076)</summary>
        public void SetCaretPeriod(int periodMilliseconds)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETCARETPERIOD, (UIntPtr)periodMilliseconds, Unused);
        }

        /// <summary>
        /// Set the set of characters making up words for when moving or selecting by word.
        /// First sets defaults like SetCharsDefault.
        /// (Scintilla feature 2077)
        /// </summary>
        public unsafe void SetWordChars(string characters)
        {
            fixed (byte* charactersPtr = Encoding.UTF8.GetBytes(characters))
            {
                SendMessage(_scintilla, SciMsg.SCI_SETWORDCHARS, UnusedW, (IntPtr)charactersPtr);
            }
        }

        /// <summary>
        /// Get the set of characters making up words for when moving or selecting by word.
        /// Returns the number of characters
        /// (Scintilla feature 2646)
        /// </summary>
        public unsafe string GetWordChars()
        {
            return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_GETWORDCHARS);
        }

        /// <summary>Set the number of characters to have directly indexed categories (Scintilla feature 2720)</summary>
        public void SetCharacterCategoryOptimization(int countCharacters)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETCHARACTERCATEGORYOPTIMIZATION, (UIntPtr)countCharacters, Unused);
        }

        /// <summary>Get the number of characters to have directly indexed categories (Scintilla feature 2721)</summary>
        public int GetCharacterCategoryOptimization()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETCHARACTERCATEGORYOPTIMIZATION, UnusedW, Unused);
        }

        /// <summary>
        /// Start a sequence of actions that is undone and redone as a unit.
        /// May be nested.
        /// (Scintilla feature 2078)
        /// </summary>
        public void BeginUndoAction()
        {
            SendMessage(_scintilla, SciMsg.SCI_BEGINUNDOACTION, UnusedW, Unused);
        }

        /// <summary>End a sequence of actions that is undone and redone as a unit. (Scintilla feature 2079)</summary>
        public void EndUndoAction()
        {
            SendMessage(_scintilla, SciMsg.SCI_ENDUNDOACTION, UnusedW, Unused);
        }

        /// <summary>Set an indicator to plain, squiggle or TT. (Scintilla feature 2080)</summary>
        public void IndicSetStyle(int indicator, IndicatorStyle indicatorStyle)
        {
            SendMessage(_scintilla, SciMsg.SCI_INDICSETSTYLE, (UIntPtr)indicator, (IntPtr)indicatorStyle);
        }

        /// <summary>Retrieve the style of an indicator. (Scintilla feature 2081)</summary>
        public IndicatorStyle IndicGetStyle(int indicator)
        {
            return (IndicatorStyle)SendMessage(_scintilla, SciMsg.SCI_INDICGETSTYLE, (UIntPtr)indicator, Unused);
        }

        /// <summary>Set the foreground colour of an indicator. (Scintilla feature 2082)</summary>
        public void IndicSetFore(int indicator, Colour fore)
        {
            SendMessage(_scintilla, SciMsg.SCI_INDICSETFORE, (UIntPtr)indicator, fore.Value);
        }

        /// <summary>Retrieve the foreground colour of an indicator. (Scintilla feature 2083)</summary>
        public Colour IndicGetFore(int indicator)
        {
            return new Colour((int)SendMessage(_scintilla, SciMsg.SCI_INDICGETFORE, (UIntPtr)indicator, Unused));
        }

        /// <summary>Set an indicator to draw under text or over(default). (Scintilla feature 2510)</summary>
        public void IndicSetUnder(int indicator, bool under)
        {
            SendMessage(_scintilla, SciMsg.SCI_INDICSETUNDER, (UIntPtr)indicator, new IntPtr(under ? 1 : 0));
        }

        /// <summary>Retrieve whether indicator drawn under or over text. (Scintilla feature 2511)</summary>
        public bool IndicGetUnder(int indicator)
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_INDICGETUNDER, (UIntPtr)indicator, Unused);
        }

        /// <summary>Set a hover indicator to plain, squiggle or TT. (Scintilla feature 2680)</summary>
        public void IndicSetHoverStyle(int indicator, IndicatorStyle indicatorStyle)
        {
            SendMessage(_scintilla, SciMsg.SCI_INDICSETHOVERSTYLE, (UIntPtr)indicator, (IntPtr)indicatorStyle);
        }

        /// <summary>Retrieve the hover style of an indicator. (Scintilla feature 2681)</summary>
        public IndicatorStyle IndicGetHoverStyle(int indicator)
        {
            return (IndicatorStyle)SendMessage(_scintilla, SciMsg.SCI_INDICGETHOVERSTYLE, (UIntPtr)indicator, Unused);
        }

        /// <summary>Set the foreground hover colour of an indicator. (Scintilla feature 2682)</summary>
        public void IndicSetHoverFore(int indicator, Colour fore)
        {
            SendMessage(_scintilla, SciMsg.SCI_INDICSETHOVERFORE, (UIntPtr)indicator, fore.Value);
        }

        /// <summary>Retrieve the foreground hover colour of an indicator. (Scintilla feature 2683)</summary>
        public Colour IndicGetHoverFore(int indicator)
        {
            return new Colour((int)SendMessage(_scintilla, SciMsg.SCI_INDICGETHOVERFORE, (UIntPtr)indicator, Unused));
        }

        /// <summary>Set the attributes of an indicator. (Scintilla feature 2684)</summary>
        public void IndicSetFlags(int indicator, IndicFlag flags)
        {
            SendMessage(_scintilla, SciMsg.SCI_INDICSETFLAGS, (UIntPtr)indicator, (IntPtr)flags);
        }

        /// <summary>Retrieve the attributes of an indicator. (Scintilla feature 2685)</summary>
        public IndicFlag IndicGetFlags(int indicator)
        {
            return (IndicFlag)SendMessage(_scintilla, SciMsg.SCI_INDICGETFLAGS, (UIntPtr)indicator, Unused);
        }

        /// <summary>Set the foreground colour of all whitespace and whether to use this setting. (Scintilla feature 2084)</summary>
        public void SetWhitespaceFore(bool useSetting, Colour fore)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETWHITESPACEFORE, new UIntPtr(useSetting ? 1U : 0U), fore.Value);
        }

        /// <summary>Set the background colour of all whitespace and whether to use this setting. (Scintilla feature 2085)</summary>
        public void SetWhitespaceBack(bool useSetting, Colour back)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETWHITESPACEBACK, new UIntPtr(useSetting ? 1U : 0U), back.Value);
        }

        /// <summary>Set the size of the dots used to mark space characters. (Scintilla feature 2086)</summary>
        public void SetWhitespaceSize(int size)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETWHITESPACESIZE, (UIntPtr)size, Unused);
        }

        /// <summary>Get the size of the dots used to mark space characters. (Scintilla feature 2087)</summary>
        public int GetWhitespaceSize()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETWHITESPACESIZE, UnusedW, Unused);
        }

        /// <summary>Used to hold extra styling information for each line. (Scintilla feature 2092)</summary>
        public void SetLineState(int line, int state)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETLINESTATE, (UIntPtr)line, (IntPtr)state);
        }

        /// <summary>Retrieve the extra styling information for a line. (Scintilla feature 2093)</summary>
        public int GetLineState(int line)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETLINESTATE, (UIntPtr)line, Unused);
        }

        /// <summary>Retrieve the last line number that has line state. (Scintilla feature 2094)</summary>
        public int GetMaxLineState()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETMAXLINESTATE, UnusedW, Unused);
        }

        /// <inheritdoc cref="SciMsg.SCI_GETCARETLINEVISIBLE"/>
        /// <returns><see langword="true"/></returns>
        [Obsolete("Use the element colours APIs instead: https://www.scintilla.org/ScintillaDoc.html#ElementColours")]
        public bool GetCaretLineVisible() => true;

        /// <inheritdoc cref="SciMsg.SCI_SETCARETLINEVISIBLE"/>
        [Obsolete("Use the element colours APIs instead: https://www.scintilla.org/ScintillaDoc.html#ElementColours")]
        public void SetCaretLineVisible(bool show)
        {
        }

        /// <inheritdoc cref="SciMsg.SCI_GETCARETLINEBACK"/>
        [Obsolete("Use the element colours APIs instead: https://www.scintilla.org/ScintillaDoc.html#ElementColours")]
        public Colour GetCaretLineBack() => new Colour(0xffffff);

        /// <inheritdoc cref="SciMsg.SCI_GETCARETLINEBACK"/>
        [Obsolete("Use the element colours APIs instead: https://www.scintilla.org/ScintillaDoc.html#ElementColours")]
        public void SetCaretLineBack(Colour back)
        {
        }

        /// <summary>
        /// Retrieve the caret line frame width.
        /// Width = 0 means this option is disabled.
        /// (Scintilla feature 2704)
        /// </summary>
        public int GetCaretLineFrame()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETCARETLINEFRAME, UnusedW, Unused);
        }

        /// <summary>
        /// Display the caret line framed.
        /// Set width != 0 to enable this option and width = 0 to disable it.
        /// (Scintilla feature 2705)
        /// </summary>
        public void SetCaretLineFrame(int width)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETCARETLINEFRAME, (UIntPtr)width, Unused);
        }

        /// <summary>
        /// Set a style to be changeable or not (read only).
        /// Experimental feature, currently buggy.
        /// (Scintilla feature 2099)
        /// </summary>
        public void StyleSetChangeable(int style, bool changeable)
        {
            SendMessage(_scintilla, SciMsg.SCI_STYLESETCHANGEABLE, (UIntPtr)style, new IntPtr(changeable ? 1 : 0));
        }

        /// <summary>
        /// Display a auto-completion list.
        /// The lengthEntered parameter indicates how many characters before
        /// the caret should be used to provide context.
        /// (Scintilla feature 2100)
        /// </summary>
        public unsafe void AutoCShow(int lengthEntered, string itemList)
        {
            fixed (byte* itemListPtr = Encoding.UTF8.GetBytes(itemList))
            {
                SendMessage(_scintilla, SciMsg.SCI_AUTOCSHOW, (UIntPtr)lengthEntered, (IntPtr)itemListPtr);
            }
        }

        /// <summary>Remove the auto-completion list from the screen. (Scintilla feature 2101)</summary>
        public void AutoCCancel()
        {
            SendMessage(_scintilla, SciMsg.SCI_AUTOCCANCEL, UnusedW, Unused);
        }

        /// <summary>Is there an auto-completion list visible? (Scintilla feature 2102)</summary>
        public bool AutoCActive()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_AUTOCACTIVE, UnusedW, Unused);
        }

        /// <summary>Retrieve the position of the caret when the auto-completion list was displayed. (Scintilla feature 2103)</summary>
        public int AutoCPosStart()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_AUTOCPOSSTART, UnusedW, Unused);
        }

        /// <summary>User has selected an item so remove the list and insert the selection. (Scintilla feature 2104)</summary>
        public void AutoCComplete()
        {
            SendMessage(_scintilla, SciMsg.SCI_AUTOCCOMPLETE, UnusedW, Unused);
        }

        /// <summary>Define a set of character that when typed cancel the auto-completion list. (Scintilla feature 2105)</summary>
        public unsafe void AutoCStops(string characterSet)
        {
            fixed (byte* characterSetPtr = Encoding.UTF8.GetBytes(characterSet))
            {
                SendMessage(_scintilla, SciMsg.SCI_AUTOCSTOPS, UnusedW, (IntPtr)characterSetPtr);
            }
        }

        /// <summary>
        /// Change the separator character in the string setting up an auto-completion list.
        /// Default is space but can be changed if items contain space.
        /// (Scintilla feature 2106)
        /// </summary>
        public void AutoCSetSeparator(int separatorCharacter)
        {
            SendMessage(_scintilla, SciMsg.SCI_AUTOCSETSEPARATOR, (UIntPtr)separatorCharacter, Unused);
        }

        /// <summary>Retrieve the auto-completion list separator character. (Scintilla feature 2107)</summary>
        public int AutoCGetSeparator()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_AUTOCGETSEPARATOR, UnusedW, Unused);
        }

        /// <summary>Select the item in the auto-completion list that starts with a string. (Scintilla feature 2108)</summary>
        public unsafe void AutoCSelect(string select)
        {
            fixed (byte* selectPtr = Encoding.UTF8.GetBytes(select))
            {
                SendMessage(_scintilla, SciMsg.SCI_AUTOCSELECT, UnusedW, (IntPtr)selectPtr);
            }
        }

        /// <summary>
        /// Should the auto-completion list be cancelled if the user backspaces to a
        /// position before where the box was created.
        /// (Scintilla feature 2110)
        /// </summary>
        public void AutoCSetCancelAtStart(bool cancel)
        {
            SendMessage(_scintilla, SciMsg.SCI_AUTOCSETCANCELATSTART, new UIntPtr(cancel ? 1U : 0U), Unused);
        }

        /// <summary>Retrieve whether auto-completion cancelled by backspacing before start. (Scintilla feature 2111)</summary>
        public bool AutoCGetCancelAtStart()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_AUTOCGETCANCELATSTART, UnusedW, Unused);
        }

        /// <summary>
        /// Define a set of characters that when typed will cause the autocompletion to
        /// choose the selected item.
        /// (Scintilla feature 2112)
        /// </summary>
        public unsafe void AutoCSetFillUps(string characterSet)
        {
            fixed (byte* characterSetPtr = Encoding.UTF8.GetBytes(characterSet))
            {
                SendMessage(_scintilla, SciMsg.SCI_AUTOCSETFILLUPS, UnusedW, (IntPtr)characterSetPtr);
            }
        }

        /// <summary>Should a single item auto-completion list automatically choose the item. (Scintilla feature 2113)</summary>
        public void AutoCSetChooseSingle(bool chooseSingle)
        {
            SendMessage(_scintilla, SciMsg.SCI_AUTOCSETCHOOSESINGLE, new UIntPtr(chooseSingle ? 1U : 0U), Unused);
        }

        /// <summary>Retrieve whether a single item auto-completion list automatically choose the item. (Scintilla feature 2114)</summary>
        public bool AutoCGetChooseSingle()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_AUTOCGETCHOOSESINGLE, UnusedW, Unused);
        }

        /// <summary>Set whether case is significant when performing auto-completion searches. (Scintilla feature 2115)</summary>
        public void AutoCSetIgnoreCase(bool ignoreCase)
        {
            SendMessage(_scintilla, SciMsg.SCI_AUTOCSETIGNORECASE, new UIntPtr(ignoreCase ? 1U : 0U), Unused);
        }

        /// <summary>Retrieve state of ignore case flag. (Scintilla feature 2116)</summary>
        public bool AutoCGetIgnoreCase()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_AUTOCGETIGNORECASE, UnusedW, Unused);
        }

        /// <summary>Display a list of strings and send notification when user chooses one. (Scintilla feature 2117)</summary>
        public unsafe void UserListShow(int listType, string itemList)
        {
            fixed (byte* itemListPtr = Encoding.UTF8.GetBytes(itemList))
            {
                SendMessage(_scintilla, SciMsg.SCI_USERLISTSHOW, (UIntPtr)listType, (IntPtr)itemListPtr);
            }
        }

        /// <summary>Set whether or not autocompletion is hidden automatically when nothing matches. (Scintilla feature 2118)</summary>
        public void AutoCSetAutoHide(bool autoHide)
        {
            SendMessage(_scintilla, SciMsg.SCI_AUTOCSETAUTOHIDE, new UIntPtr(autoHide ? 1U : 0U), Unused);
        }

        /// <summary>Retrieve whether or not autocompletion is hidden automatically when nothing matches. (Scintilla feature 2119)</summary>
        public bool AutoCGetAutoHide()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_AUTOCGETAUTOHIDE, UnusedW, Unused);
        }

        /// <summary>
        /// Set whether or not autocompletion deletes any word characters
        /// after the inserted text upon completion.
        /// (Scintilla feature 2270)
        /// </summary>
        public void AutoCSetDropRestOfWord(bool dropRestOfWord)
        {
            SendMessage(_scintilla, SciMsg.SCI_AUTOCSETDROPRESTOFWORD, new UIntPtr(dropRestOfWord ? 1U : 0U), Unused);
        }

        /// <summary>
        /// Retrieve whether or not autocompletion deletes any word characters
        /// after the inserted text upon completion.
        /// (Scintilla feature 2271)
        /// </summary>
        public bool AutoCGetDropRestOfWord()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_AUTOCGETDROPRESTOFWORD, UnusedW, Unused);
        }

        /// <summary>Register an XPM image for use in autocompletion lists. (Scintilla feature 2405)</summary>
        public unsafe void RegisterImage(int type, string xpmData)
        {
            fixed (byte* xpmDataPtr = Encoding.UTF8.GetBytes(xpmData))
            {
                SendMessage(_scintilla, SciMsg.SCI_REGISTERIMAGE, (UIntPtr)type, (IntPtr)xpmDataPtr);
            }
        }

        /// <summary>Clear all the registered XPM images. (Scintilla feature 2408)</summary>
        public void ClearRegisteredImages()
        {
            SendMessage(_scintilla, SciMsg.SCI_CLEARREGISTEREDIMAGES, UnusedW, Unused);
        }

        /// <summary>Retrieve the auto-completion list type-separator character. (Scintilla feature 2285)</summary>
        public int AutoCGetTypeSeparator()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_AUTOCGETTYPESEPARATOR, UnusedW, Unused);
        }

        /// <summary>
        /// Change the type-separator character in the string setting up an auto-completion list.
        /// Default is '?' but can be changed if items contain '?'.
        /// (Scintilla feature 2286)
        /// </summary>
        public void AutoCSetTypeSeparator(int separatorCharacter)
        {
            SendMessage(_scintilla, SciMsg.SCI_AUTOCSETTYPESEPARATOR, (UIntPtr)separatorCharacter, Unused);
        }

        /// <summary>
        /// Set the maximum width, in characters, of auto-completion and user lists.
        /// Set to 0 to autosize to fit longest item, which is the default.
        /// (Scintilla feature 2208)
        /// </summary>
        public void AutoCSetMaxWidth(int characterCount)
        {
            SendMessage(_scintilla, SciMsg.SCI_AUTOCSETMAXWIDTH, (UIntPtr)characterCount, Unused);
        }

        /// <summary>Get the maximum width, in characters, of auto-completion and user lists. (Scintilla feature 2209)</summary>
        public int AutoCGetMaxWidth()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_AUTOCGETMAXWIDTH, UnusedW, Unused);
        }

        /// <summary>
        /// Set the maximum height, in rows, of auto-completion and user lists.
        /// The default is 5 rows.
        /// (Scintilla feature 2210)
        /// </summary>
        public void AutoCSetMaxHeight(int rowCount)
        {
            SendMessage(_scintilla, SciMsg.SCI_AUTOCSETMAXHEIGHT, (UIntPtr)rowCount, Unused);
        }

        /// <summary>Set the maximum height, in rows, of auto-completion and user lists. (Scintilla feature 2211)</summary>
        public int AutoCGetMaxHeight()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_AUTOCGETMAXHEIGHT, UnusedW, Unused);
        }

        /// <summary>Set the number of spaces used for one level of indentation. (Scintilla feature 2122)</summary>
        public void SetIndent(int indentSize)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETINDENT, (UIntPtr)indentSize, Unused);
        }

        /// <summary>Retrieve indentation size. (Scintilla feature 2123)</summary>
        public int GetIndent()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETINDENT, UnusedW, Unused);
        }

        /// <summary>
        /// Indentation will only use space characters if useTabs is false, otherwise
        /// it will use a combination of tabs and spaces.
        /// (Scintilla feature 2124)
        /// </summary>
        public void SetUseTabs(bool useTabs)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETUSETABS, new UIntPtr(useTabs ? 1U : 0U), Unused);
        }

        /// <summary>Retrieve whether tabs will be used in indentation. (Scintilla feature 2125)</summary>
        public bool GetUseTabs()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETUSETABS, UnusedW, Unused);
        }

        /// <summary>Change the indentation of a line to a number of columns. (Scintilla feature 2126)</summary>
        public void SetLineIndentation(int line, int indentation)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETLINEINDENTATION, (UIntPtr)line, (IntPtr)indentation);
        }

        /// <summary>Retrieve the number of columns that a line is indented. (Scintilla feature 2127)</summary>
        public int GetLineIndentation(int line)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETLINEINDENTATION, (UIntPtr)line, Unused);
        }

        /// <summary>Retrieve the position before the first non indentation character on a line. (Scintilla feature 2128)</summary>
        public int GetLineIndentPosition(int line)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETLINEINDENTPOSITION, (UIntPtr)line, Unused);
        }

        /// <summary>Retrieve the column number of a position, taking tab width into account. (Scintilla feature 2129)</summary>
        public int GetColumn(int pos)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETCOLUMN, (UIntPtr)pos, Unused);
        }

        /// <summary>Count characters between two positions. (Scintilla feature 2633)</summary>
        public int CountCharacters(int start, int end)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_COUNTCHARACTERS, (UIntPtr)start, (IntPtr)end);
        }

        /// <summary>Count code units between two positions. (Scintilla feature 2715)</summary>
        public int CountCodeUnits(int start, int end)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_COUNTCODEUNITS, (UIntPtr)start, (IntPtr)end);
        }

        /// <summary>Show or hide the horizontal scroll bar. (Scintilla feature 2130)</summary>
        public void SetHScrollBar(bool visible)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETHSCROLLBAR, new UIntPtr(visible ? 1U : 0U), Unused);
        }

        /// <summary>Is the horizontal scroll bar visible? (Scintilla feature 2131)</summary>
        public bool GetHScrollBar()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETHSCROLLBAR, UnusedW, Unused);
        }

        /// <summary>Show or hide indentation guides. (Scintilla feature 2132)</summary>
        public void SetIndentationGuides(IndentView indentView)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETINDENTATIONGUIDES, (UIntPtr)indentView, Unused);
        }

        /// <summary>Are the indentation guides visible? (Scintilla feature 2133)</summary>
        public IndentView GetIndentationGuides()
        {
            return (IndentView)SendMessage(_scintilla, SciMsg.SCI_GETINDENTATIONGUIDES, UnusedW, Unused);
        }

        /// <summary>
        /// Set the highlighted indentation guide column.
        /// 0 = no highlighted guide.
        /// (Scintilla feature 2134)
        /// </summary>
        public void SetHighlightGuide(int column)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETHIGHLIGHTGUIDE, (UIntPtr)column, Unused);
        }

        /// <summary>Get the highlighted indentation guide column. (Scintilla feature 2135)</summary>
        public int GetHighlightGuide()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETHIGHLIGHTGUIDE, UnusedW, Unused);
        }

        /// <summary>Get the position after the last visible characters on a line. (Scintilla feature 2136)</summary>
        public int GetLineEndPosition(int line)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETLINEENDPOSITION, (UIntPtr)line, Unused);
        }

        /// <summary>Get the code page used to interpret the bytes of the document as characters. (Scintilla feature 2137)</summary>
        public int GetCodePage()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETCODEPAGE, UnusedW, Unused);
        }

        /// <inheritdoc cref="SciMsg.SCI_GETCARETFORE"/>
        [Obsolete("Use the element colours APIs instead: https://www.scintilla.org/ScintillaDoc.html#ElementColours")]
        public Colour GetCaretFore() => new Colour(0);

        /// <summary>In read-only mode? (Scintilla feature 2140)</summary>
        public bool GetReadOnly()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETREADONLY, UnusedW, Unused);
        }

        /// <summary>Sets the position of the caret. (Scintilla feature 2141)</summary>
        public void SetCurrentPos(int caret)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETCURRENTPOS, (UIntPtr)caret, Unused);
        }

        /// <summary>Sets the position that starts the selection - this becomes the anchor. (Scintilla feature 2142)</summary>
        public void SetSelectionStart(int anchor)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETSELECTIONSTART, (UIntPtr)anchor, Unused);
        }

        /// <summary>Returns the position at the start of the selection. (Scintilla feature 2143)</summary>
        public int GetSelectionStart()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETSELECTIONSTART, UnusedW, Unused);
        }

        /// <summary>Sets the position that ends the selection - this becomes the caret. (Scintilla feature 2144)</summary>
        public void SetSelectionEnd(int caret)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETSELECTIONEND, (UIntPtr)caret, Unused);
        }

        /// <summary>Returns the position at the end of the selection. (Scintilla feature 2145)</summary>
        public int GetSelectionEnd()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETSELECTIONEND, UnusedW, Unused);
        }

        /// <summary>Set caret to a position, while removing any existing selection. (Scintilla feature 2556)</summary>
        public void SetEmptySelection(int caret)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETEMPTYSELECTION, (UIntPtr)caret, Unused);
        }

        /// <summary>Sets the print magnification added to the point size of each style for printing. (Scintilla feature 2146)</summary>
        public void SetPrintMagnification(int magnification)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETPRINTMAGNIFICATION, (UIntPtr)magnification, Unused);
        }

        /// <summary>Returns the print magnification. (Scintilla feature 2147)</summary>
        public int GetPrintMagnification()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETPRINTMAGNIFICATION, UnusedW, Unused);
        }

        /// <summary>Modify colours when printing for clearer printed text. (Scintilla feature 2148)</summary>
        public void SetPrintColourMode(PrintOption mode)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETPRINTCOLOURMODE, (UIntPtr)mode, Unused);
        }

        /// <summary>Returns the print colour mode. (Scintilla feature 2149)</summary>
        public PrintOption GetPrintColourMode()
        {
            return (PrintOption)SendMessage(_scintilla, SciMsg.SCI_GETPRINTCOLOURMODE, UnusedW, Unused);
        }

        /// <summary>Find some text in the document. (Scintilla feature 2150)</summary>
        public int FindText(FindOption searchFlags, TextToFind ft)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_FINDTEXT, (UIntPtr)searchFlags, ft.NativePointer);
        }

        /// <summary>Retrieve the display line at the top of the display. (Scintilla feature 2152)</summary>
        public int GetFirstVisibleLine()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETFIRSTVISIBLELINE, UnusedW, Unused);
        }

        /// <summary>
        /// Retrieve the contents of a line.
        /// Returns the length of the line.
        /// (Scintilla feature 2153)
        /// </summary>
        public unsafe string GetLine(int line)
        {
            return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_GETLINE, (UIntPtr)line);
        }

        /// <summary>Returns the number of lines in the document. There is always at least one. (Scintilla feature 2154)</summary>
        public int GetLineCount()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETLINECOUNT, UnusedW, Unused);
        }

        /// <summary>Sets the size in pixels of the left margin. (Scintilla feature 2155)</summary>
        public void SetMarginLeft(int pixelWidth)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETMARGINLEFT, UnusedW, (IntPtr)pixelWidth);
        }

        /// <summary>Returns the size in pixels of the left margin. (Scintilla feature 2156)</summary>
        public int GetMarginLeft()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETMARGINLEFT, UnusedW, Unused);
        }

        /// <summary>Sets the size in pixels of the right margin. (Scintilla feature 2157)</summary>
        public void SetMarginRight(int pixelWidth)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETMARGINRIGHT, UnusedW, (IntPtr)pixelWidth);
        }

        /// <summary>Returns the size in pixels of the right margin. (Scintilla feature 2158)</summary>
        public int GetMarginRight()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETMARGINRIGHT, UnusedW, Unused);
        }

        /// <summary>Is the document different from when it was last saved? (Scintilla feature 2159)</summary>
        public bool GetModify()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETMODIFY, UnusedW, Unused);
        }

        /// <summary>Select a range of text. (Scintilla feature 2160)</summary>
        public void SetSel(int anchor, int caret)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETSEL, (UIntPtr)anchor, (IntPtr)caret);
        }

        /// <summary>
        /// Retrieve the selected text.
        /// Return the length of the text.
        /// Result is NUL-terminated.
        /// (Scintilla feature 2161)
        /// </summary>
        public unsafe string GetSelText()
        {
            return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_GETSELTEXT);
        }

        /// <summary>
        /// Retrieve a range of text.
        /// Return the length of the text.
        /// (Scintilla feature 2162)
        /// </summary>
        public int GetTextRange(TextRange tr)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETTEXTRANGE, UnusedW, tr.NativePointer);
        }

        /// <summary>Draw the selection either highlighted or in normal (non-highlighted) style. (Scintilla feature 2163)</summary>
        public void HideSelection(bool hide)
        {
            SendMessage(_scintilla, SciMsg.SCI_HIDESELECTION, new UIntPtr(hide ? 1U : 0U), Unused);
        }

        /// <summary>Retrieve the x value of the point in the window where a position is displayed. (Scintilla feature 2164)</summary>
        public int PointXFromPosition(int pos)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_POINTXFROMPOSITION, UnusedW, (IntPtr)pos);
        }

        /// <summary>Retrieve the y value of the point in the window where a position is displayed. (Scintilla feature 2165)</summary>
        public int PointYFromPosition(int pos)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_POINTYFROMPOSITION, UnusedW, (IntPtr)pos);
        }

        /// <summary>Retrieve the line containing a position. (Scintilla feature 2166)</summary>
        public int LineFromPosition(int pos)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_LINEFROMPOSITION, (UIntPtr)pos, Unused);
        }

        /// <summary>Retrieve the position at the start of a line. (Scintilla feature 2167)</summary>
        public int PositionFromLine(int line)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_POSITIONFROMLINE, (UIntPtr)line, Unused);
        }

        /// <summary>Scroll horizontally and vertically. (Scintilla feature 2168)</summary>
        public void LineScroll(int columns, int lines)
        {
            SendMessage(_scintilla, SciMsg.SCI_LINESCROLL, (UIntPtr)columns, (IntPtr)lines);
        }

        /// <summary>Ensure the caret is visible. (Scintilla feature 2169)</summary>
        public void ScrollCaret()
        {
            SendMessage(_scintilla, SciMsg.SCI_SCROLLCARET, UnusedW, Unused);
        }

        /// <summary>
        /// Scroll the argument positions and the range between them into view giving
        /// priority to the primary position then the secondary position.
        /// This may be used to make a search match visible.
        /// (Scintilla feature 2569)
        /// </summary>
        public void ScrollRange(int secondary, int primary)
        {
            SendMessage(_scintilla, SciMsg.SCI_SCROLLRANGE, (UIntPtr)secondary, (IntPtr)primary);
        }

        /// <summary>Replace the selected text with the argument text. (Scintilla feature 2170)</summary>
        public unsafe void ReplaceSel(string text)
        {
            fixed (byte* textPtr = Encoding.UTF8.GetBytes(text))
            {
                SendMessage(_scintilla, SciMsg.SCI_REPLACESEL, UnusedW, (IntPtr)textPtr);
            }
        }

        /// <summary>Set to read only or read write. (Scintilla feature 2171)</summary>
        public void SetReadOnly(bool readOnly)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETREADONLY, new UIntPtr(readOnly ? 1U : 0U), Unused);
        }

        /// <summary>Null operation. (Scintilla feature 2172)</summary>
        public void Null()
        {
            SendMessage(_scintilla, SciMsg.SCI_NULL, UnusedW, Unused);
        }

        /// <summary>Will a paste succeed? (Scintilla feature 2173)</summary>
        public bool CanPaste()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_CANPASTE, UnusedW, Unused);
        }

        /// <summary>Are there any undoable actions in the undo history? (Scintilla feature 2174)</summary>
        public bool CanUndo()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_CANUNDO, UnusedW, Unused);
        }

        /// <summary>Delete the undo history. (Scintilla feature 2175)</summary>
        public void EmptyUndoBuffer()
        {
            SendMessage(_scintilla, SciMsg.SCI_EMPTYUNDOBUFFER, UnusedW, Unused);
        }

        /// <summary>Undo one action in the undo history. (Scintilla feature 2176)</summary>
        public void Undo()
        {
            SendMessage(_scintilla, SciMsg.SCI_UNDO, UnusedW, Unused);
        }

        /// <summary>Cut the selection to the clipboard. (Scintilla feature 2177)</summary>
        public void Cut()
        {
            SendMessage(_scintilla, SciMsg.SCI_CUT, UnusedW, Unused);
        }

        /// <summary>Copy the selection to the clipboard. (Scintilla feature 2178)</summary>
        public void Copy()
        {
            SendMessage(_scintilla, SciMsg.SCI_COPY, UnusedW, Unused);
        }

        /// <summary>Paste the contents of the clipboard into the document replacing the selection. (Scintilla feature 2179)</summary>
        public void Paste()
        {
            SendMessage(_scintilla, SciMsg.SCI_PASTE, UnusedW, Unused);
        }

        /// <summary>Clear the selection. (Scintilla feature 2180)</summary>
        public void Clear()
        {
            SendMessage(_scintilla, SciMsg.SCI_CLEAR, UnusedW, Unused);
        }

        /// <summary>Replace the contents of the document with the argument text. (Scintilla feature 2181)</summary>
        public unsafe void SetText(string text)
        {
            fixed (byte* textPtr = Encoding.UTF8.GetBytes(text))
            {
                SendMessage(_scintilla, SciMsg.SCI_SETTEXT, UnusedW, (IntPtr)textPtr);
            }
        }

        /// <summary>
        /// Retrieve all the text in the document.
        /// Returns number of characters retrieved.
        /// Result is NUL-terminated.
        /// (Scintilla feature 2182)
        /// </summary>
        public unsafe string GetText(int length = -1)
        {
            if (length < 1)
                length = SendMessage(_scintilla, SciMsg.SCI_GETTEXT, (UIntPtr)length, Unused).ToInt32();
            byte[] textBuffer = new byte[length];
            fixed (byte* textPtr = textBuffer)
            {
                SendMessage(_scintilla, SciMsg.SCI_GETTEXT, (UIntPtr)length, (IntPtr)textPtr);
                return Utf8BytesToNullStrippedString(textBuffer);
            }
        }

        /// <summary>Retrieve the number of characters in the document. (Scintilla feature 2183)</summary>
        public int GetTextLength()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETTEXTLENGTH, UnusedW, Unused);
        }

        /// <summary>Retrieve a pointer to a function that processes messages for this Scintilla. (Scintilla feature 2184)</summary>
        public IntPtr GetDirectFunction()
        {
            return SendMessage(_scintilla, SciMsg.SCI_GETDIRECTFUNCTION, UnusedW, Unused);
        }

        /// <summary>
        /// Retrieve a pointer value to use as the first argument when calling
        /// the function returned by GetDirectFunction.
        /// (Scintilla feature 2185)
        /// </summary>
        public IntPtr GetDirectPointer()
        {
            return SendMessage(_scintilla, SciMsg.SCI_GETDIRECTPOINTER, UnusedW, Unused);
        }

        /// <summary>Set to overtype (true) or insert mode. (Scintilla feature 2186)</summary>
        public void SetOvertype(bool overType)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETOVERTYPE, new UIntPtr(overType ? 1U : 0U), Unused);
        }

        /// <summary>Returns true if overtype mode is active otherwise false is returned. (Scintilla feature 2187)</summary>
        public bool GetOvertype()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETOVERTYPE, UnusedW, Unused);
        }

        /// <summary>Set the width of the insert mode caret. (Scintilla feature 2188)</summary>
        public void SetCaretWidth(int pixelWidth)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETCARETWIDTH, (UIntPtr)pixelWidth, Unused);
        }

        /// <summary>Returns the width of the insert mode caret. (Scintilla feature 2189)</summary>
        public int GetCaretWidth()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETCARETWIDTH, UnusedW, Unused);
        }

        /// <summary>
        /// Sets the position that starts the target which is used for updating the
        /// document without affecting the scroll position.
        /// (Scintilla feature 2190)
        /// </summary>
        public void SetTargetStart(int start)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETTARGETSTART, (UIntPtr)start, Unused);
        }

        /// <summary>Get the position that starts the target. (Scintilla feature 2191)</summary>
        public int GetTargetStart()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETTARGETSTART, UnusedW, Unused);
        }

        /// <summary>
        /// Sets the position that ends the target which is used for updating the
        /// document without affecting the scroll position.
        /// (Scintilla feature 2192)
        /// </summary>
        public void SetTargetEnd(int end)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETTARGETEND, (UIntPtr)end, Unused);
        }

        /// <summary>Get the position that ends the target. (Scintilla feature 2193)</summary>
        public int GetTargetEnd()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETTARGETEND, UnusedW, Unused);
        }

        /// <summary>Sets both the start and end of the target in one call. (Scintilla feature 2686)</summary>
        public void SetTargetRange(int start, int end)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETTARGETRANGE, (UIntPtr)start, (IntPtr)end);
        }

        /// <summary>Retrieve the text in the target. (Scintilla feature 2687)</summary>
        public unsafe string GetTargetText()
        {
            return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_GETTARGETTEXT);
        }

        /// <summary>Make the target range start and end be the same as the selection range start and end. (Scintilla feature 2287)</summary>
        public void TargetFromSelection()
        {
            SendMessage(_scintilla, SciMsg.SCI_TARGETFROMSELECTION, UnusedW, Unused);
        }

        /// <summary>Sets the target to the whole document. (Scintilla feature 2690)</summary>
        public void TargetWholeDocument()
        {
            SendMessage(_scintilla, SciMsg.SCI_TARGETWHOLEDOCUMENT, UnusedW, Unused);
        }

        /// <summary>
        /// Replace the target text with the argument text.
        /// Text is counted so it can contain NULs.
        /// Returns the length of the replacement text.
        /// (Scintilla feature 2194)
        /// </summary>
        public unsafe int ReplaceTarget(int length, string text)
        {
            fixed (byte* textPtr = Encoding.UTF8.GetBytes(text))
            {
                return (int)SendMessage(_scintilla, SciMsg.SCI_REPLACETARGET, (UIntPtr)length, (IntPtr)textPtr);
            }
        }

        /// <summary>
        /// Replace the target text with the argument text after \d processing.
        /// Text is counted so it can contain NULs.
        /// Looks for \d where d is between 1 and 9 and replaces these with the strings
        /// matched in the last search operation which were surrounded by \( and \).
        /// Returns the length of the replacement text including any change
        /// caused by processing the \d patterns.
        /// (Scintilla feature 2195)
        /// </summary>
        public unsafe int ReplaceTargetRE(int length, string text)
        {
            fixed (byte* textPtr = Encoding.UTF8.GetBytes(text))
            {
                return (int)SendMessage(_scintilla, SciMsg.SCI_REPLACETARGETRE, (UIntPtr)length, (IntPtr)textPtr);
            }
        }

        /// <summary>
        /// Search for a counted string in the target and set the target to the found
        /// range. Text is counted so it can contain NULs.
        /// Returns start of found range or -1 for failure in which case target is not moved.
        /// (Scintilla feature 2197)
        /// </summary>
        public unsafe int SearchInTarget(int length, string text)
        {
            fixed (byte* textPtr = Encoding.UTF8.GetBytes(text))
            {
                return (int)SendMessage(_scintilla, SciMsg.SCI_SEARCHINTARGET, (UIntPtr)length, (IntPtr)textPtr);
            }
        }

        /// <summary>Set the search flags used by SearchInTarget. (Scintilla feature 2198)</summary>
        public void SetSearchFlags(FindOption searchFlags)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETSEARCHFLAGS, (UIntPtr)searchFlags, Unused);
        }

        /// <summary>Get the search flags used by SearchInTarget. (Scintilla feature 2199)</summary>
        public FindOption GetSearchFlags()
        {
            return (FindOption)SendMessage(_scintilla, SciMsg.SCI_GETSEARCHFLAGS, UnusedW, Unused);
        }

        /// <summary>Show a call tip containing a definition near position pos. (Scintilla feature 2200)</summary>
        public unsafe void CallTipShow(int pos, string definition)
        {
            fixed (byte* definitionPtr = Encoding.UTF8.GetBytes(definition))
            {
                SendMessage(_scintilla, SciMsg.SCI_CALLTIPSHOW, (UIntPtr)pos, (IntPtr)definitionPtr);
            }
        }

        /// <summary>Remove the call tip from the screen. (Scintilla feature 2201)</summary>
        public void CallTipCancel()
        {
            SendMessage(_scintilla, SciMsg.SCI_CALLTIPCANCEL, UnusedW, Unused);
        }

        /// <summary>Is there an active call tip? (Scintilla feature 2202)</summary>
        public bool CallTipActive()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_CALLTIPACTIVE, UnusedW, Unused);
        }

        /// <summary>Retrieve the position where the caret was before displaying the call tip. (Scintilla feature 2203)</summary>
        public int CallTipPosStart()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_CALLTIPPOSSTART, UnusedW, Unused);
        }

        /// <summary>Set the start position in order to change when backspacing removes the calltip. (Scintilla feature 2214)</summary>
        public void CallTipSetPosStart(int posStart)
        {
            SendMessage(_scintilla, SciMsg.SCI_CALLTIPSETPOSSTART, (UIntPtr)posStart, Unused);
        }

        /// <summary>Highlight a segment of the definition. (Scintilla feature 2204)</summary>
        public void CallTipSetHlt(int highlightStart, int highlightEnd)
        {
            SendMessage(_scintilla, SciMsg.SCI_CALLTIPSETHLT, (UIntPtr)highlightStart, (IntPtr)highlightEnd);
        }

        /// <summary>Set the background colour for the call tip. (Scintilla feature 2205)</summary>
        public void CallTipSetBack(Colour back)
        {
            SendMessage(_scintilla, SciMsg.SCI_CALLTIPSETBACK, (UIntPtr)back.Value, Unused);
        }

        /// <summary>Set the foreground colour for the call tip. (Scintilla feature 2206)</summary>
        public void CallTipSetFore(Colour fore)
        {
            SendMessage(_scintilla, SciMsg.SCI_CALLTIPSETFORE, (UIntPtr)fore.Value, Unused);
        }

        /// <summary>Set the foreground colour for the highlighted part of the call tip. (Scintilla feature 2207)</summary>
        public void CallTipSetForeHlt(Colour fore)
        {
            SendMessage(_scintilla, SciMsg.SCI_CALLTIPSETFOREHLT, (UIntPtr)fore.Value, Unused);
        }

        /// <summary>Enable use of STYLE_CALLTIP and set call tip tab size in pixels. (Scintilla feature 2212)</summary>
        public void CallTipUseStyle(int tabSize)
        {
            SendMessage(_scintilla, SciMsg.SCI_CALLTIPUSESTYLE, (UIntPtr)tabSize, Unused);
        }

        /// <summary>Set position of calltip, above or below text. (Scintilla feature 2213)</summary>
        public void CallTipSetPosition(bool above)
        {
            SendMessage(_scintilla, SciMsg.SCI_CALLTIPSETPOSITION, new UIntPtr(above ? 1U : 0U), Unused);
        }

        /// <summary>Find the display line of a document line taking hidden lines into account. (Scintilla feature 2220)</summary>
        public int VisibleFromDocLine(int docLine)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_VISIBLEFROMDOCLINE, (UIntPtr)docLine, Unused);
        }

        /// <summary>Find the document line of a display line taking hidden lines into account. (Scintilla feature 2221)</summary>
        public int DocLineFromVisible(int displayLine)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_DOCLINEFROMVISIBLE, (UIntPtr)displayLine, Unused);
        }

        /// <summary>The number of display lines needed to wrap a document line (Scintilla feature 2235)</summary>
        public int WrapCount(int docLine)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_WRAPCOUNT, (UIntPtr)docLine, Unused);
        }

        /// <summary>
        /// Set the fold level of a line.
        /// This encodes an integer level along with flags indicating whether the
        /// line is a header and whether it is effectively white space.
        /// (Scintilla feature 2222)
        /// </summary>
        public void SetFoldLevel(int line, FoldLevel level)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETFOLDLEVEL, (UIntPtr)line, (IntPtr)level);
        }

        /// <summary>Retrieve the fold level of a line. (Scintilla feature 2223)</summary>
        public FoldLevel GetFoldLevel(int line)
        {
            return (FoldLevel)SendMessage(_scintilla, SciMsg.SCI_GETFOLDLEVEL, (UIntPtr)line, Unused);
        }

        /// <summary>Find the last child line of a header line. (Scintilla feature 2224)</summary>
        public int GetLastChild(int line, FoldLevel level)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETLASTCHILD, (UIntPtr)line, (IntPtr)level);
        }

        /// <summary>Find the parent line of a child line. (Scintilla feature 2225)</summary>
        public int GetFoldParent(int line)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETFOLDPARENT, (UIntPtr)line, Unused);
        }

        /// <summary>Make a range of lines visible. (Scintilla feature 2226)</summary>
        public void ShowLines(int lineStart, int lineEnd)
        {
            SendMessage(_scintilla, SciMsg.SCI_SHOWLINES, (UIntPtr)lineStart, (IntPtr)lineEnd);
        }

        /// <summary>Make a range of lines invisible. (Scintilla feature 2227)</summary>
        public void HideLines(int lineStart, int lineEnd)
        {
            SendMessage(_scintilla, SciMsg.SCI_HIDELINES, (UIntPtr)lineStart, (IntPtr)lineEnd);
        }

        /// <summary>Is a line visible? (Scintilla feature 2228)</summary>
        public bool GetLineVisible(int line)
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETLINEVISIBLE, (UIntPtr)line, Unused);
        }

        /// <summary>Are all lines visible? (Scintilla feature 2236)</summary>
        public bool GetAllLinesVisible()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETALLLINESVISIBLE, UnusedW, Unused);
        }

        /// <summary>Show the children of a header line. (Scintilla feature 2229)</summary>
        public void SetFoldExpanded(int line, bool expanded)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETFOLDEXPANDED, (UIntPtr)line, new IntPtr(expanded ? 1 : 0));
        }

        /// <summary>Is a header line expanded? (Scintilla feature 2230)</summary>
        public bool GetFoldExpanded(int line)
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETFOLDEXPANDED, (UIntPtr)line, Unused);
        }

        /// <summary>Switch a header line between expanded and contracted. (Scintilla feature 2231)</summary>
        public void ToggleFold(int line)
        {
            SendMessage(_scintilla, SciMsg.SCI_TOGGLEFOLD, (UIntPtr)line, Unused);
        }

        /// <summary>Switch a header line between expanded and contracted and show some text after the line. (Scintilla feature 2700)</summary>
        public unsafe void ToggleFoldShowText(int line, string text)
        {
            fixed (byte* textPtr = Encoding.UTF8.GetBytes(text))
            {
                SendMessage(_scintilla, SciMsg.SCI_TOGGLEFOLDSHOWTEXT, (UIntPtr)line, (IntPtr)textPtr);
            }
        }

        /// <summary>Set the style of fold display text. (Scintilla feature 2701)</summary>
        public void FoldDisplayTextSetStyle(FoldDisplayTextStyle style)
        {
            SendMessage(_scintilla, SciMsg.SCI_FOLDDISPLAYTEXTSETSTYLE, (UIntPtr)style, Unused);
        }

        /// <summary>Get the style of fold display text. (Scintilla feature 2707)</summary>
        public FoldDisplayTextStyle FoldDisplayTextGetStyle()
        {
            return (FoldDisplayTextStyle)SendMessage(_scintilla, SciMsg.SCI_FOLDDISPLAYTEXTGETSTYLE, UnusedW, Unused);
        }

        /// <summary>Set the default fold display text. (Scintilla feature 2722)</summary>
        public unsafe void SetDefaultFoldDisplayText(string text)
        {
            fixed (byte* textPtr = Encoding.UTF8.GetBytes(text))
            {
                SendMessage(_scintilla, SciMsg.SCI_SETDEFAULTFOLDDISPLAYTEXT, UnusedW, (IntPtr)textPtr);
            }
        }

        /// <summary>Get the default fold display text. (Scintilla feature 2723)</summary>
        public unsafe string GetDefaultFoldDisplayText()
        {
            return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_GETDEFAULTFOLDDISPLAYTEXT);
        }

        /// <summary>Expand or contract a fold header. (Scintilla feature 2237)</summary>
        public void FoldLine(int line, FoldAction action)
        {
            SendMessage(_scintilla, SciMsg.SCI_FOLDLINE, (UIntPtr)line, (IntPtr)action);
        }

        /// <summary>Expand or contract a fold header and its children. (Scintilla feature 2238)</summary>
        public void FoldChildren(int line, FoldAction action)
        {
            SendMessage(_scintilla, SciMsg.SCI_FOLDCHILDREN, (UIntPtr)line, (IntPtr)action);
        }

        /// <summary>Expand a fold header and all children. Use the level argument instead of the line's current level. (Scintilla feature 2239)</summary>
        public void ExpandChildren(int line, FoldLevel level)
        {
            SendMessage(_scintilla, SciMsg.SCI_EXPANDCHILDREN, (UIntPtr)line, (IntPtr)level);
        }

        /// <summary>Expand or contract all fold headers. (Scintilla feature 2662)</summary>
        public void FoldAll(FoldAction action)
        {
            SendMessage(_scintilla, SciMsg.SCI_FOLDALL, (UIntPtr)action, Unused);
        }

        /// <summary>Ensure a particular line is visible by expanding any header line hiding it. (Scintilla feature 2232)</summary>
        public void EnsureVisible(int line)
        {
            SendMessage(_scintilla, SciMsg.SCI_ENSUREVISIBLE, (UIntPtr)line, Unused);
        }

        /// <summary>Set automatic folding behaviours. (Scintilla feature 2663)</summary>
        public void SetAutomaticFold(AutomaticFold automaticFold)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETAUTOMATICFOLD, (UIntPtr)automaticFold, Unused);
        }

        /// <summary>Get automatic folding behaviours. (Scintilla feature 2664)</summary>
        public AutomaticFold GetAutomaticFold()
        {
            return (AutomaticFold)SendMessage(_scintilla, SciMsg.SCI_GETAUTOMATICFOLD, UnusedW, Unused);
        }

        /// <summary>Set some style options for folding. (Scintilla feature 2233)</summary>
        public void SetFoldFlags(FoldFlag flags)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETFOLDFLAGS, (UIntPtr)flags, Unused);
        }

        /// <summary>
        /// Ensure a particular line is visible by expanding any header line hiding it.
        /// Use the currently set visibility policy to determine which range to display.
        /// (Scintilla feature 2234)
        /// </summary>
        public void EnsureVisibleEnforcePolicy(int line)
        {
            SendMessage(_scintilla, SciMsg.SCI_ENSUREVISIBLEENFORCEPOLICY, (UIntPtr)line, Unused);
        }

        /// <summary>Sets whether a tab pressed when caret is within indentation indents. (Scintilla feature 2260)</summary>
        public void SetTabIndents(bool tabIndents)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETTABINDENTS, new UIntPtr(tabIndents ? 1U : 0U), Unused);
        }

        /// <summary>Does a tab pressed when caret is within indentation indent? (Scintilla feature 2261)</summary>
        public bool GetTabIndents()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETTABINDENTS, UnusedW, Unused);
        }

        /// <summary>Sets whether a backspace pressed when caret is within indentation unindents. (Scintilla feature 2262)</summary>
        public void SetBackSpaceUnIndents(bool bsUnIndents)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETBACKSPACEUNINDENTS, new UIntPtr(bsUnIndents ? 1U : 0U), Unused);
        }

        /// <summary>Does a backspace pressed when caret is within indentation unindent? (Scintilla feature 2263)</summary>
        public bool GetBackSpaceUnIndents()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETBACKSPACEUNINDENTS, UnusedW, Unused);
        }

        /// <summary>Sets the time the mouse must sit still to generate a mouse dwell event. (Scintilla feature 2264)</summary>
        public void SetMouseDwellTime(int periodMilliseconds)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETMOUSEDWELLTIME, (UIntPtr)periodMilliseconds, Unused);
        }

        /// <summary>Retrieve the time the mouse must sit still to generate a mouse dwell event. (Scintilla feature 2265)</summary>
        public int GetMouseDwellTime()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETMOUSEDWELLTIME, UnusedW, Unused);
        }

        /// <summary>Get position of start of word. (Scintilla feature 2266)</summary>
        public int WordStartPosition(int pos, bool onlyWordCharacters)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_WORDSTARTPOSITION, (UIntPtr)pos, new IntPtr(onlyWordCharacters ? 1 : 0));
        }

        /// <summary>Get position of end of word. (Scintilla feature 2267)</summary>
        public int WordEndPosition(int pos, bool onlyWordCharacters)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_WORDENDPOSITION, (UIntPtr)pos, new IntPtr(onlyWordCharacters ? 1 : 0));
        }

        /// <summary>Is the range start..end considered a word? (Scintilla feature 2691)</summary>
        public bool IsRangeWord(int start, int end)
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_ISRANGEWORD, (UIntPtr)start, (IntPtr)end);
        }

        /// <summary>Sets limits to idle styling. (Scintilla feature 2692)</summary>
        public void SetIdleStyling(IdleStyling idleStyling)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETIDLESTYLING, (UIntPtr)idleStyling, Unused);
        }

        /// <summary>Retrieve the limits to idle styling. (Scintilla feature 2693)</summary>
        public IdleStyling GetIdleStyling()
        {
            return (IdleStyling)SendMessage(_scintilla, SciMsg.SCI_GETIDLESTYLING, UnusedW, Unused);
        }

        /// <summary>Sets whether text is word wrapped. (Scintilla feature 2268)</summary>
        public void SetWrapMode(Wrap wrapMode)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETWRAPMODE, (UIntPtr)wrapMode, Unused);
        }

        /// <summary>Retrieve whether text is word wrapped. (Scintilla feature 2269)</summary>
        public Wrap GetWrapMode()
        {
            return (Wrap)SendMessage(_scintilla, SciMsg.SCI_GETWRAPMODE, UnusedW, Unused);
        }

        /// <summary>Set the display mode of visual flags for wrapped lines. (Scintilla feature 2460)</summary>
        public void SetWrapVisualFlags(WrapVisualFlag wrapVisualFlags)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETWRAPVISUALFLAGS, (UIntPtr)wrapVisualFlags, Unused);
        }

        /// <summary>Retrive the display mode of visual flags for wrapped lines. (Scintilla feature 2461)</summary>
        public WrapVisualFlag GetWrapVisualFlags()
        {
            return (WrapVisualFlag)SendMessage(_scintilla, SciMsg.SCI_GETWRAPVISUALFLAGS, UnusedW, Unused);
        }

        /// <summary>Set the location of visual flags for wrapped lines. (Scintilla feature 2462)</summary>
        public void SetWrapVisualFlagsLocation(WrapVisualLocation wrapVisualFlagsLocation)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETWRAPVISUALFLAGSLOCATION, (UIntPtr)wrapVisualFlagsLocation, Unused);
        }

        /// <summary>Retrive the location of visual flags for wrapped lines. (Scintilla feature 2463)</summary>
        public WrapVisualLocation GetWrapVisualFlagsLocation()
        {
            return (WrapVisualLocation)SendMessage(_scintilla, SciMsg.SCI_GETWRAPVISUALFLAGSLOCATION, UnusedW, Unused);
        }

        /// <summary>Set the start indent for wrapped lines. (Scintilla feature 2464)</summary>
        public void SetWrapStartIndent(int indent)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETWRAPSTARTINDENT, (UIntPtr)indent, Unused);
        }

        /// <summary>Retrive the start indent for wrapped lines. (Scintilla feature 2465)</summary>
        public int GetWrapStartIndent()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETWRAPSTARTINDENT, UnusedW, Unused);
        }

        /// <summary>Sets how wrapped sublines are placed. Default is fixed. (Scintilla feature 2472)</summary>
        public void SetWrapIndentMode(WrapIndentMode wrapIndentMode)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETWRAPINDENTMODE, (UIntPtr)wrapIndentMode, Unused);
        }

        /// <summary>Retrieve how wrapped sublines are placed. Default is fixed. (Scintilla feature 2473)</summary>
        public WrapIndentMode GetWrapIndentMode()
        {
            return (WrapIndentMode)SendMessage(_scintilla, SciMsg.SCI_GETWRAPINDENTMODE, UnusedW, Unused);
        }

        /// <summary>Sets the degree of caching of layout information. (Scintilla feature 2272)</summary>
        public void SetLayoutCache(LineCache cacheMode)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETLAYOUTCACHE, (UIntPtr)cacheMode, Unused);
        }

        /// <summary>Retrieve the degree of caching of layout information. (Scintilla feature 2273)</summary>
        public LineCache GetLayoutCache()
        {
            return (LineCache)SendMessage(_scintilla, SciMsg.SCI_GETLAYOUTCACHE, UnusedW, Unused);
        }

        /// <summary>Sets the document width assumed for scrolling. (Scintilla feature 2274)</summary>
        public void SetScrollWidth(int pixelWidth)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETSCROLLWIDTH, (UIntPtr)pixelWidth, Unused);
        }

        /// <summary>Retrieve the document width assumed for scrolling. (Scintilla feature 2275)</summary>
        public int GetScrollWidth()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETSCROLLWIDTH, UnusedW, Unused);
        }

        /// <summary>Sets whether the maximum width line displayed is used to set scroll width. (Scintilla feature 2516)</summary>
        public void SetScrollWidthTracking(bool tracking)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETSCROLLWIDTHTRACKING, new UIntPtr(tracking ? 1U : 0U), Unused);
        }

        /// <summary>Retrieve whether the scroll width tracks wide lines. (Scintilla feature 2517)</summary>
        public bool GetScrollWidthTracking()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETSCROLLWIDTHTRACKING, UnusedW, Unused);
        }

        /// <summary>
        /// Measure the pixel width of some text in a particular style.
        /// NUL terminated text argument.
        /// Does not handle tab or control characters.
        /// (Scintilla feature 2276)
        /// </summary>
        public unsafe int TextWidth(int style, string text)
        {
            fixed (byte* textPtr = Encoding.UTF8.GetBytes(text))
            {
                return (int)SendMessage(_scintilla, SciMsg.SCI_TEXTWIDTH, (UIntPtr)style, (IntPtr)textPtr);
            }
        }

        /// <summary>
        /// Sets the scroll range so that maximum scroll position has
        /// the last line at the bottom of the view (default).
        /// Setting this to false allows scrolling one page below the last line.
        /// (Scintilla feature 2277)
        /// </summary>
        public void SetEndAtLastLine(bool endAtLastLine)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETENDATLASTLINE, new UIntPtr(endAtLastLine ? 1U : 0U), Unused);
        }

        /// <summary>
        /// Retrieve whether the maximum scroll position has the last
        /// line at the bottom of the view.
        /// (Scintilla feature 2278)
        /// </summary>
        public bool GetEndAtLastLine()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETENDATLASTLINE, UnusedW, Unused);
        }

        /// <summary>Retrieve the height of a particular line of text in pixels. (Scintilla feature 2279)</summary>
        public int TextHeight(int line)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_TEXTHEIGHT, (UIntPtr)line, Unused);
        }

        /// <summary>Show or hide the vertical scroll bar. (Scintilla feature 2280)</summary>
        public void SetVScrollBar(bool visible)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETVSCROLLBAR, new UIntPtr(visible ? 1U : 0U), Unused);
        }

        /// <summary>Is the vertical scroll bar visible? (Scintilla feature 2281)</summary>
        public bool GetVScrollBar()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETVSCROLLBAR, UnusedW, Unused);
        }

        /// <summary>Append a string to the end of the document without changing the selection. (Scintilla feature 2282)</summary>
        public unsafe void AppendText(int length, string text)
        {
            fixed (byte* textPtr = Encoding.UTF8.GetBytes(text))
            {
                SendMessage(_scintilla, SciMsg.SCI_APPENDTEXT, (UIntPtr)length, (IntPtr)textPtr);
            }
        }

        /// <summary>How many phases is drawing done in? (Scintilla feature 2673)</summary>
        public PhasesDraw GetPhasesDraw()
        {
            return (PhasesDraw)SendMessage(_scintilla, SciMsg.SCI_GETPHASESDRAW, UnusedW, Unused);
        }

        /// <summary>
        /// In one phase draw, text is drawn in a series of rectangular blocks with no overlap.
        /// In two phase draw, text is drawn in a series of lines allowing runs to overlap horizontally.
        /// In multiple phase draw, each element is drawn over the whole drawing area, allowing text
        /// to overlap from one line to the next.
        /// (Scintilla feature 2674)
        /// </summary>
        public void SetPhasesDraw(PhasesDraw phases)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETPHASESDRAW, (UIntPtr)phases, Unused);
        }

        /// <summary>Choose the quality level for text from the FontQuality enumeration. (Scintilla feature 2611)</summary>
        public void SetFontQuality(FontQuality fontQuality)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETFONTQUALITY, (UIntPtr)fontQuality, Unused);
        }

        /// <summary>Retrieve the quality level for text. (Scintilla feature 2612)</summary>
        public FontQuality GetFontQuality()
        {
            return (FontQuality)SendMessage(_scintilla, SciMsg.SCI_GETFONTQUALITY, UnusedW, Unused);
        }

        /// <summary>Scroll so that a display line is at the top of the display. (Scintilla feature 2613)</summary>
        public void SetFirstVisibleLine(int displayLine)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETFIRSTVISIBLELINE, (UIntPtr)displayLine, Unused);
        }

        /// <summary>Change the effect of pasting when there are multiple selections. (Scintilla feature 2614)</summary>
        public void SetMultiPaste(MultiPaste multiPaste)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETMULTIPASTE, (UIntPtr)multiPaste, Unused);
        }

        /// <summary>Retrieve the effect of pasting when there are multiple selections. (Scintilla feature 2615)</summary>
        public MultiPaste GetMultiPaste()
        {
            return (MultiPaste)SendMessage(_scintilla, SciMsg.SCI_GETMULTIPASTE, UnusedW, Unused);
        }

        /// <summary>
        /// Retrieve the value of a tag from a regular expression search.
        /// Result is NUL-terminated.
        /// (Scintilla feature 2616)
        /// </summary>
        public unsafe string GetTag(int tagNumber)
        {
            if (tagNumber < 0)
            {
                throw new ArgumentException("tagNumber must be non-negative integer");
            }
            return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_GETTAG, (UIntPtr)tagNumber);
        }

        /// <summary>Join the lines in the target. (Scintilla feature 2288)</summary>
        public void LinesJoin()
        {
            SendMessage(_scintilla, SciMsg.SCI_LINESJOIN, UnusedW, Unused);
        }

        /// <summary>
        /// Split the lines in the target into lines that are less wide than pixelWidth
        /// where possible.
        /// (Scintilla feature 2289)
        /// </summary>
        public void LinesSplit(int pixelWidth)
        {
            SendMessage(_scintilla, SciMsg.SCI_LINESSPLIT, (UIntPtr)pixelWidth, Unused);
        }

        /// <summary>Set one of the colours used as a chequerboard pattern in the fold margin (Scintilla feature 2290)</summary>
        public void SetFoldMarginColour(bool useSetting, Colour back)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETFOLDMARGINCOLOUR, new UIntPtr(useSetting ? 1U : 0U), back.Value);
        }

        /// <summary>Set the other colour used as a chequerboard pattern in the fold margin (Scintilla feature 2291)</summary>
        public void SetFoldMarginHiColour(bool useSetting, Colour fore)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETFOLDMARGINHICOLOUR, new UIntPtr(useSetting ? 1U : 0U), fore.Value);
        }

        /// <summary>Enable or disable accessibility. (Scintilla feature 2702)</summary>
        public void SetAccessibility(Accessibility accessibility)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETACCESSIBILITY, (UIntPtr)accessibility, Unused);
        }

        /// <summary>Report accessibility status. (Scintilla feature 2703)</summary>
        public Accessibility GetAccessibility()
        {
            return (Accessibility)SendMessage(_scintilla, SciMsg.SCI_GETACCESSIBILITY, UnusedW, Unused);
        }

        /// <summary>Move caret down one line. (Scintilla feature 2300)</summary>
        public void LineDown()
        {
            SendMessage(_scintilla, SciMsg.SCI_LINEDOWN, UnusedW, Unused);
        }

        /// <summary>Move caret down one line extending selection to new caret position. (Scintilla feature 2301)</summary>
        public void LineDownExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_LINEDOWNEXTEND, UnusedW, Unused);
        }

        /// <summary>Move caret up one line. (Scintilla feature 2302)</summary>
        public void LineUp()
        {
            SendMessage(_scintilla, SciMsg.SCI_LINEUP, UnusedW, Unused);
        }

        /// <summary>Move caret up one line extending selection to new caret position. (Scintilla feature 2303)</summary>
        public void LineUpExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_LINEUPEXTEND, UnusedW, Unused);
        }

        /// <summary>Move caret left one character. (Scintilla feature 2304)</summary>
        public void CharLeft()
        {
            SendMessage(_scintilla, SciMsg.SCI_CHARLEFT, UnusedW, Unused);
        }

        /// <summary>Move caret left one character extending selection to new caret position. (Scintilla feature 2305)</summary>
        public void CharLeftExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_CHARLEFTEXTEND, UnusedW, Unused);
        }

        /// <summary>Move caret right one character. (Scintilla feature 2306)</summary>
        public void CharRight()
        {
            SendMessage(_scintilla, SciMsg.SCI_CHARRIGHT, UnusedW, Unused);
        }

        /// <summary>Move caret right one character extending selection to new caret position. (Scintilla feature 2307)</summary>
        public void CharRightExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_CHARRIGHTEXTEND, UnusedW, Unused);
        }

        /// <summary>Move caret left one word. (Scintilla feature 2308)</summary>
        public void WordLeft()
        {
            SendMessage(_scintilla, SciMsg.SCI_WORDLEFT, UnusedW, Unused);
        }

        /// <summary>Move caret left one word extending selection to new caret position. (Scintilla feature 2309)</summary>
        public void WordLeftExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_WORDLEFTEXTEND, UnusedW, Unused);
        }

        /// <summary>Move caret right one word. (Scintilla feature 2310)</summary>
        public void WordRight()
        {
            SendMessage(_scintilla, SciMsg.SCI_WORDRIGHT, UnusedW, Unused);
        }

        /// <summary>Move caret right one word extending selection to new caret position. (Scintilla feature 2311)</summary>
        public void WordRightExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_WORDRIGHTEXTEND, UnusedW, Unused);
        }

        /// <summary>Move caret to first position on line. (Scintilla feature 2312)</summary>
        public void Home()
        {
            SendMessage(_scintilla, SciMsg.SCI_HOME, UnusedW, Unused);
        }

        /// <summary>Move caret to first position on line extending selection to new caret position. (Scintilla feature 2313)</summary>
        public void HomeExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_HOMEEXTEND, UnusedW, Unused);
        }

        /// <summary>Move caret to last position on line. (Scintilla feature 2314)</summary>
        public void LineEnd()
        {
            SendMessage(_scintilla, SciMsg.SCI_LINEEND, UnusedW, Unused);
        }

        /// <summary>Move caret to last position on line extending selection to new caret position. (Scintilla feature 2315)</summary>
        public void LineEndExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_LINEENDEXTEND, UnusedW, Unused);
        }

        /// <summary>Move caret to first position in document. (Scintilla feature 2316)</summary>
        public void DocumentStart()
        {
            SendMessage(_scintilla, SciMsg.SCI_DOCUMENTSTART, UnusedW, Unused);
        }

        /// <summary>Move caret to first position in document extending selection to new caret position. (Scintilla feature 2317)</summary>
        public void DocumentStartExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_DOCUMENTSTARTEXTEND, UnusedW, Unused);
        }

        /// <summary>Move caret to last position in document. (Scintilla feature 2318)</summary>
        public void DocumentEnd()
        {
            SendMessage(_scintilla, SciMsg.SCI_DOCUMENTEND, UnusedW, Unused);
        }

        /// <summary>Move caret to last position in document extending selection to new caret position. (Scintilla feature 2319)</summary>
        public void DocumentEndExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_DOCUMENTENDEXTEND, UnusedW, Unused);
        }

        /// <summary>Move caret one page up. (Scintilla feature 2320)</summary>
        public void PageUp()
        {
            SendMessage(_scintilla, SciMsg.SCI_PAGEUP, UnusedW, Unused);
        }

        /// <summary>Move caret one page up extending selection to new caret position. (Scintilla feature 2321)</summary>
        public void PageUpExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_PAGEUPEXTEND, UnusedW, Unused);
        }

        /// <summary>Move caret one page down. (Scintilla feature 2322)</summary>
        public void PageDown()
        {
            SendMessage(_scintilla, SciMsg.SCI_PAGEDOWN, UnusedW, Unused);
        }

        /// <summary>Move caret one page down extending selection to new caret position. (Scintilla feature 2323)</summary>
        public void PageDownExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_PAGEDOWNEXTEND, UnusedW, Unused);
        }

        /// <summary>Switch from insert to overtype mode or the reverse. (Scintilla feature 2324)</summary>
        public void EditToggleOvertype()
        {
            SendMessage(_scintilla, SciMsg.SCI_EDITTOGGLEOVERTYPE, UnusedW, Unused);
        }

        /// <summary>Cancel any modes such as call tip or auto-completion list display. (Scintilla feature 2325)</summary>
        public void Cancel()
        {
            SendMessage(_scintilla, SciMsg.SCI_CANCEL, UnusedW, Unused);
        }

        /// <summary>Delete the selection or if no selection, the character before the caret. (Scintilla feature 2326)</summary>
        public void DeleteBack()
        {
            SendMessage(_scintilla, SciMsg.SCI_DELETEBACK, UnusedW, Unused);
        }

        /// <summary>
        /// If selection is empty or all on one line replace the selection with a tab character.
        /// If more than one line selected, indent the lines.
        /// (Scintilla feature 2327)
        /// </summary>
        public void Tab()
        {
            SendMessage(_scintilla, SciMsg.SCI_TAB, UnusedW, Unused);
        }

        /// <summary>Dedent the selected lines. (Scintilla feature 2328)</summary>
        public void BackTab()
        {
            SendMessage(_scintilla, SciMsg.SCI_BACKTAB, UnusedW, Unused);
        }

        /// <summary>Insert a new line, may use a CRLF, CR or LF depending on EOL mode. (Scintilla feature 2329)</summary>
        public void NewLine()
        {
            SendMessage(_scintilla, SciMsg.SCI_NEWLINE, UnusedW, Unused);
        }

        /// <summary>Insert a Form Feed character. (Scintilla feature 2330)</summary>
        public void FormFeed()
        {
            SendMessage(_scintilla, SciMsg.SCI_FORMFEED, UnusedW, Unused);
        }

        /// <summary>
        /// Move caret to before first visible character on line.
        /// If already there move to first character on line.
        /// (Scintilla feature 2331)
        /// </summary>
        public void VCHome()
        {
            SendMessage(_scintilla, SciMsg.SCI_VCHOME, UnusedW, Unused);
        }

        /// <summary>Like VCHome but extending selection to new caret position. (Scintilla feature 2332)</summary>
        public void VCHomeExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_VCHOMEEXTEND, UnusedW, Unused);
        }

        /// <summary>Magnify the displayed text by increasing the sizes by 1 point. (Scintilla feature 2333)</summary>
        public void ZoomIn()
        {
            SendMessage(_scintilla, SciMsg.SCI_ZOOMIN, UnusedW, Unused);
        }

        /// <summary>Make the displayed text smaller by decreasing the sizes by 1 point. (Scintilla feature 2334)</summary>
        public void ZoomOut()
        {
            SendMessage(_scintilla, SciMsg.SCI_ZOOMOUT, UnusedW, Unused);
        }

        /// <summary>Delete the word to the left of the caret. (Scintilla feature 2335)</summary>
        public void DelWordLeft()
        {
            SendMessage(_scintilla, SciMsg.SCI_DELWORDLEFT, UnusedW, Unused);
        }

        /// <summary>Delete the word to the right of the caret. (Scintilla feature 2336)</summary>
        public void DelWordRight()
        {
            SendMessage(_scintilla, SciMsg.SCI_DELWORDRIGHT, UnusedW, Unused);
        }

        /// <summary>Delete the word to the right of the caret, but not the trailing non-word characters. (Scintilla feature 2518)</summary>
        public void DelWordRightEnd()
        {
            SendMessage(_scintilla, SciMsg.SCI_DELWORDRIGHTEND, UnusedW, Unused);
        }

        /// <summary>Cut the line containing the caret. (Scintilla feature 2337)</summary>
        public void LineCut()
        {
            SendMessage(_scintilla, SciMsg.SCI_LINECUT, UnusedW, Unused);
        }

        /// <summary>Delete the line containing the caret. (Scintilla feature 2338)</summary>
        public void LineDelete()
        {
            SendMessage(_scintilla, SciMsg.SCI_LINEDELETE, UnusedW, Unused);
        }

        /// <summary>Switch the current line with the previous. (Scintilla feature 2339)</summary>
        public void LineTranspose()
        {
            SendMessage(_scintilla, SciMsg.SCI_LINETRANSPOSE, UnusedW, Unused);
        }

        /// <summary>Reverse order of selected lines. (Scintilla feature 2354)</summary>
        public void LineReverse()
        {
            SendMessage(_scintilla, SciMsg.SCI_LINEREVERSE, UnusedW, Unused);
        }

        /// <summary>Duplicate the current line. (Scintilla feature 2404)</summary>
        public void LineDuplicate()
        {
            SendMessage(_scintilla, SciMsg.SCI_LINEDUPLICATE, UnusedW, Unused);
        }

        /// <summary>Transform the selection to lower case. (Scintilla feature 2340)</summary>
        public void LowerCase()
        {
            SendMessage(_scintilla, SciMsg.SCI_LOWERCASE, UnusedW, Unused);
        }

        /// <summary>Transform the selection to upper case. (Scintilla feature 2341)</summary>
        public void UpperCase()
        {
            SendMessage(_scintilla, SciMsg.SCI_UPPERCASE, UnusedW, Unused);
        }

        /// <summary>Scroll the document down, keeping the caret visible. (Scintilla feature 2342)</summary>
        public void LineScrollDown()
        {
            SendMessage(_scintilla, SciMsg.SCI_LINESCROLLDOWN, UnusedW, Unused);
        }

        /// <summary>Scroll the document up, keeping the caret visible. (Scintilla feature 2343)</summary>
        public void LineScrollUp()
        {
            SendMessage(_scintilla, SciMsg.SCI_LINESCROLLUP, UnusedW, Unused);
        }

        /// <summary>
        /// Delete the selection or if no selection, the character before the caret.
        /// Will not delete the character before at the start of a line.
        /// (Scintilla feature 2344)
        /// </summary>
        public void DeleteBackNotLine()
        {
            SendMessage(_scintilla, SciMsg.SCI_DELETEBACKNOTLINE, UnusedW, Unused);
        }

        /// <summary>Move caret to first position on display line. (Scintilla feature 2345)</summary>
        public void HomeDisplay()
        {
            SendMessage(_scintilla, SciMsg.SCI_HOMEDISPLAY, UnusedW, Unused);
        }

        /// <summary>
        /// Move caret to first position on display line extending selection to
        /// new caret position.
        /// (Scintilla feature 2346)
        /// </summary>
        public void HomeDisplayExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_HOMEDISPLAYEXTEND, UnusedW, Unused);
        }

        /// <summary>Move caret to last position on display line. (Scintilla feature 2347)</summary>
        public void LineEndDisplay()
        {
            SendMessage(_scintilla, SciMsg.SCI_LINEENDDISPLAY, UnusedW, Unused);
        }

        /// <summary>
        /// Move caret to last position on display line extending selection to new
        /// caret position.
        /// (Scintilla feature 2348)
        /// </summary>
        public void LineEndDisplayExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_LINEENDDISPLAYEXTEND, UnusedW, Unused);
        }

        /// <summary>
        /// Like Home but when word-wrap is enabled goes first to start of display line
        /// HomeDisplay, then to start of document line Home.
        /// (Scintilla feature 2349)
        /// </summary>
        public void HomeWrap()
        {
            SendMessage(_scintilla, SciMsg.SCI_HOMEWRAP, UnusedW, Unused);
        }

        /// <summary>
        /// Like HomeExtend but when word-wrap is enabled extends first to start of display line
        /// HomeDisplayExtend, then to start of document line HomeExtend.
        /// (Scintilla feature 2450)
        /// </summary>
        public void HomeWrapExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_HOMEWRAPEXTEND, UnusedW, Unused);
        }

        /// <summary>
        /// Like LineEnd but when word-wrap is enabled goes first to end of display line
        /// LineEndDisplay, then to start of document line LineEnd.
        /// (Scintilla feature 2451)
        /// </summary>
        public void LineEndWrap()
        {
            SendMessage(_scintilla, SciMsg.SCI_LINEENDWRAP, UnusedW, Unused);
        }

        /// <summary>
        /// Like LineEndExtend but when word-wrap is enabled extends first to end of display line
        /// LineEndDisplayExtend, then to start of document line LineEndExtend.
        /// (Scintilla feature 2452)
        /// </summary>
        public void LineEndWrapExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_LINEENDWRAPEXTEND, UnusedW, Unused);
        }

        /// <summary>
        /// Like VCHome but when word-wrap is enabled goes first to start of display line
        /// VCHomeDisplay, then behaves like VCHome.
        /// (Scintilla feature 2453)
        /// </summary>
        public void VCHomeWrap()
        {
            SendMessage(_scintilla, SciMsg.SCI_VCHOMEWRAP, UnusedW, Unused);
        }

        /// <summary>
        /// Like VCHomeExtend but when word-wrap is enabled extends first to start of display line
        /// VCHomeDisplayExtend, then behaves like VCHomeExtend.
        /// (Scintilla feature 2454)
        /// </summary>
        public void VCHomeWrapExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_VCHOMEWRAPEXTEND, UnusedW, Unused);
        }

        /// <summary>Copy the line containing the caret. (Scintilla feature 2455)</summary>
        public void LineCopy()
        {
            SendMessage(_scintilla, SciMsg.SCI_LINECOPY, UnusedW, Unused);
        }

        /// <summary>Move the caret inside current view if it's not there already. (Scintilla feature 2401)</summary>
        public void MoveCaretInsideView()
        {
            SendMessage(_scintilla, SciMsg.SCI_MOVECARETINSIDEVIEW, UnusedW, Unused);
        }

        /// <summary>How many characters are on a line, including end of line characters? (Scintilla feature 2350)</summary>
        public int LineLength(int line)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_LINELENGTH, (UIntPtr)line, Unused);
        }

        /// <summary>Highlight the characters at two positions. (Scintilla feature 2351)</summary>
        public void BraceHighlight(int posA, int posB)
        {
            SendMessage(_scintilla, SciMsg.SCI_BRACEHIGHLIGHT, (UIntPtr)posA, (IntPtr)posB);
        }

        /// <summary>Use specified indicator to highlight matching braces instead of changing their style. (Scintilla feature 2498)</summary>
        public void BraceHighlightIndicator(bool useSetting, int indicator)
        {
            SendMessage(_scintilla, SciMsg.SCI_BRACEHIGHLIGHTINDICATOR, new UIntPtr(useSetting ? 1U : 0U), (IntPtr)indicator);
        }

        /// <summary>Highlight the character at a position indicating there is no matching brace. (Scintilla feature 2352)</summary>
        public void BraceBadLight(int pos)
        {
            SendMessage(_scintilla, SciMsg.SCI_BRACEBADLIGHT, (UIntPtr)pos, Unused);
        }

        /// <summary>Use specified indicator to highlight non matching brace instead of changing its style. (Scintilla feature 2499)</summary>
        public void BraceBadLightIndicator(bool useSetting, int indicator)
        {
            SendMessage(_scintilla, SciMsg.SCI_BRACEBADLIGHTINDICATOR, new UIntPtr(useSetting ? 1U : 0U), (IntPtr)indicator);
        }

        /// <summary>
        /// Find the position of a matching brace or INVALID_POSITION if no match.
        /// The maxReStyle must be 0 for now. It may be defined in a future release.
        /// (Scintilla feature 2353)
        /// </summary>
        public int BraceMatch(int pos, int maxReStyle)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_BRACEMATCH, (UIntPtr)pos, (IntPtr)maxReStyle);
        }

        /// <summary>Are the end of line characters visible? (Scintilla feature 2355)</summary>
        public bool GetViewEOL()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETVIEWEOL, UnusedW, Unused);
        }

        /// <summary>Make the end of line characters visible or invisible. (Scintilla feature 2356)</summary>
        public void SetViewEOL(bool visible)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETVIEWEOL, new UIntPtr(visible ? 1U : 0U), Unused);
        }

        /// <summary>Retrieve a pointer to the document object. (Scintilla feature 2357)</summary>
        public IntPtr GetDocPointer()
        {
            return SendMessage(_scintilla, SciMsg.SCI_GETDOCPOINTER, UnusedW, Unused);
        }

        /// <summary>Change the document object used. (Scintilla feature 2358)</summary>
        public void SetDocPointer(IntPtr doc)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETDOCPOINTER, UnusedW, doc);
        }

        /// <summary>Set which document modification events are sent to the container. (Scintilla feature 2359)</summary>
        public void SetModEventMask(ModificationFlags eventMask)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETMODEVENTMASK, (UIntPtr)eventMask, Unused);
        }

        /// <summary>Retrieve the column number which text should be kept within. (Scintilla feature 2360)</summary>
        public int GetEdgeColumn()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETEDGECOLUMN, UnusedW, Unused);
        }

        /// <summary>
        /// Set the column number of the edge.
        /// If text goes past the edge then it is highlighted.
        /// (Scintilla feature 2361)
        /// </summary>
        public void SetEdgeColumn(int column)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETEDGECOLUMN, (UIntPtr)column, Unused);
        }

        /// <summary>Retrieve the edge highlight mode. (Scintilla feature 2362)</summary>
        public EdgeVisualStyle GetEdgeMode()
        {
            return (EdgeVisualStyle)SendMessage(_scintilla, SciMsg.SCI_GETEDGEMODE, UnusedW, Unused);
        }

        /// <summary>
        /// The edge may be displayed by a line (EDGE_LINE/EDGE_MULTILINE) or by highlighting text that
        /// goes beyond it (EDGE_BACKGROUND) or not displayed at all (EDGE_NONE).
        /// (Scintilla feature 2363)
        /// </summary>
        public void SetEdgeMode(EdgeVisualStyle edgeMode)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETEDGEMODE, (UIntPtr)edgeMode, Unused);
        }

        /// <summary>Retrieve the colour used in edge indication. (Scintilla feature 2364)</summary>
        public Colour GetEdgeColour()
        {
            return new Colour((int)SendMessage(_scintilla, SciMsg.SCI_GETEDGECOLOUR, UnusedW, Unused));
        }

        /// <summary>Change the colour used in edge indication. (Scintilla feature 2365)</summary>
        public void SetEdgeColour(Colour edgeColour)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETEDGECOLOUR, (UIntPtr)edgeColour.Value, Unused);
        }

        /// <summary>Add a new vertical edge to the view. (Scintilla feature 2694)</summary>
        public void MultiEdgeAddLine(int column, Colour edgeColour)
        {
            SendMessage(_scintilla, SciMsg.SCI_MULTIEDGEADDLINE, (UIntPtr)column, edgeColour.Value);
        }

        /// <summary>Clear all vertical edges. (Scintilla feature 2695)</summary>
        public void MultiEdgeClearAll()
        {
            SendMessage(_scintilla, SciMsg.SCI_MULTIEDGECLEARALL, UnusedW, Unused);
        }

        /// <summary>Sets the current caret position to be the search anchor. (Scintilla feature 2366)</summary>
        public void SearchAnchor()
        {
            SendMessage(_scintilla, SciMsg.SCI_SEARCHANCHOR, UnusedW, Unused);
        }

        /// <summary>
        /// Find some text starting at the search anchor.
        /// Does not ensure the selection is visible.
        /// (Scintilla feature 2367)
        /// </summary>
        public unsafe int SearchNext(FindOption searchFlags, string text)
        {
            fixed (byte* textPtr = Encoding.UTF8.GetBytes(text))
            {
                return (int)SendMessage(_scintilla, SciMsg.SCI_SEARCHNEXT, (UIntPtr)searchFlags, (IntPtr)textPtr);
            }
        }

        /// <summary>
        /// Find some text starting at the search anchor and moving backwards.
        /// Does not ensure the selection is visible.
        /// (Scintilla feature 2368)
        /// </summary>
        public unsafe int SearchPrev(FindOption searchFlags, string text)
        {
            fixed (byte* textPtr = Encoding.UTF8.GetBytes(text))
            {
                return (int)SendMessage(_scintilla, SciMsg.SCI_SEARCHPREV, (UIntPtr)searchFlags, (IntPtr)textPtr);
            }
        }

        /// <summary>Retrieves the number of lines completely visible. (Scintilla feature 2370)</summary>
        public int LinesOnScreen()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_LINESONSCREEN, UnusedW, Unused);
        }

        /// <summary>
        /// Set whether a pop up menu is displayed automatically when the user presses
        /// the wrong mouse button on certain areas.
        /// (Scintilla feature 2371)
        /// </summary>
        public void UsePopUp(PopUp popUpMode)
        {
            SendMessage(_scintilla, SciMsg.SCI_USEPOPUP, (UIntPtr)popUpMode, Unused);
        }

        /// <summary>Is the selection rectangular? The alternative is the more common stream selection. (Scintilla feature 2372)</summary>
        public bool SelectionIsRectangle()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_SELECTIONISRECTANGLE, UnusedW, Unused);
        }

        /// <summary>
        /// Set the zoom level. This number of points is added to the size of all fonts.
        /// It may be positive to magnify or negative to reduce.
        /// (Scintilla feature 2373)
        /// </summary>
        public void SetZoom(int zoomInPoints)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETZOOM, (UIntPtr)zoomInPoints, Unused);
        }

        /// <summary>Retrieve the zoom level. (Scintilla feature 2374)</summary>
        public int GetZoom()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETZOOM, UnusedW, Unused);
        }

        /// <summary>
        /// Create a new document object.
        /// Starts with reference count of 1 and not selected into editor.
        /// (Scintilla feature 2375)
        /// </summary>
        public IntPtr CreateDocument(int bytes, DocumentOption documentOptions)
        {
            return SendMessage(_scintilla, SciMsg.SCI_CREATEDOCUMENT, (UIntPtr)bytes, (IntPtr)documentOptions);
        }

        /// <summary>Extend life of document. (Scintilla feature 2376)</summary>
        public void AddRefDocument(IntPtr doc)
        {
            SendMessage(_scintilla, SciMsg.SCI_ADDREFDOCUMENT, UnusedW, doc);
        }

        /// <summary>Release a reference to the document, deleting document if it fades to black. (Scintilla feature 2377)</summary>
        public void ReleaseDocument(IntPtr doc)
        {
            SendMessage(_scintilla, SciMsg.SCI_RELEASEDOCUMENT, UnusedW, doc);
        }

        /// <summary>Get which document options are set. (Scintilla feature 2379)</summary>
        public DocumentOption GetDocumentOptions()
        {
            return (DocumentOption)SendMessage(_scintilla, SciMsg.SCI_GETDOCUMENTOPTIONS, UnusedW, Unused);
        }

        /// <summary>Get which document modification events are sent to the container. (Scintilla feature 2378)</summary>
        public ModificationFlags GetModEventMask()
        {
            return (ModificationFlags)SendMessage(_scintilla, SciMsg.SCI_GETMODEVENTMASK, UnusedW, Unused);
        }

        /// <summary>Set whether command events are sent to the container. (Scintilla feature 2717)</summary>
        public void SetCommandEvents(bool commandEvents)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETCOMMANDEVENTS, new UIntPtr(commandEvents ? 1U : 0U), Unused);
        }

        /// <summary>Get whether command events are sent to the container. (Scintilla feature 2718)</summary>
        public bool GetCommandEvents()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETCOMMANDEVENTS, UnusedW, Unused);
        }

        /// <summary>Change internal focus flag. (Scintilla feature 2380)</summary>
        public void SetFocus(bool focus)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETFOCUS, new UIntPtr(focus ? 1U : 0U), Unused);
        }

        /// <summary>Get internal focus flag. (Scintilla feature 2381)</summary>
        public bool GetFocus()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETFOCUS, UnusedW, Unused);
        }

        /// <summary>Change error status - 0 = OK. (Scintilla feature 2382)</summary>
        public void SetStatus(Status status)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETSTATUS, (UIntPtr)status, Unused);
        }

        /// <summary>Get error status. (Scintilla feature 2383)</summary>
        public Status GetStatus()
        {
            return (Status)SendMessage(_scintilla, SciMsg.SCI_GETSTATUS, UnusedW, Unused);
        }

        /// <summary>Set whether the mouse is captured when its button is pressed. (Scintilla feature 2384)</summary>
        public void SetMouseDownCaptures(bool captures)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETMOUSEDOWNCAPTURES, new UIntPtr(captures ? 1U : 0U), Unused);
        }

        /// <summary>Get whether mouse gets captured. (Scintilla feature 2385)</summary>
        public bool GetMouseDownCaptures()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETMOUSEDOWNCAPTURES, UnusedW, Unused);
        }

        /// <summary>Set whether the mouse wheel can be active outside the window. (Scintilla feature 2696)</summary>
        public void SetMouseWheelCaptures(bool captures)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETMOUSEWHEELCAPTURES, new UIntPtr(captures ? 1U : 0U), Unused);
        }

        /// <summary>Get whether mouse wheel can be active outside the window. (Scintilla feature 2697)</summary>
        public bool GetMouseWheelCaptures()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETMOUSEWHEELCAPTURES, UnusedW, Unused);
        }

        /// <summary>Sets the cursor to one of the SC_CURSOR* values. (Scintilla feature 2386)</summary>
        public void SetCursor(CursorShape cursorType)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETCURSOR, (UIntPtr)cursorType, Unused);
        }

        /// <summary>Get cursor type. (Scintilla feature 2387)</summary>
        public CursorShape GetCursor()
        {
            return (CursorShape)SendMessage(_scintilla, SciMsg.SCI_GETCURSOR, UnusedW, Unused);
        }

        /// <summary>
        /// Change the way control characters are displayed:
        /// If symbol is &lt; 32, keep the drawn way, else, use the given character.
        /// (Scintilla feature 2388)
        /// </summary>
        public void SetControlCharSymbol(int symbol)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETCONTROLCHARSYMBOL, (UIntPtr)symbol, Unused);
        }

        /// <summary>Get the way control characters are displayed. (Scintilla feature 2389)</summary>
        public int GetControlCharSymbol()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETCONTROLCHARSYMBOL, UnusedW, Unused);
        }

        /// <summary>Move to the previous change in capitalisation. (Scintilla feature 2390)</summary>
        public void WordPartLeft()
        {
            SendMessage(_scintilla, SciMsg.SCI_WORDPARTLEFT, UnusedW, Unused);
        }

        /// <summary>
        /// Move to the previous change in capitalisation extending selection
        /// to new caret position.
        /// (Scintilla feature 2391)
        /// </summary>
        public void WordPartLeftExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_WORDPARTLEFTEXTEND, UnusedW, Unused);
        }

        /// <summary>Move to the change next in capitalisation. (Scintilla feature 2392)</summary>
        public void WordPartRight()
        {
            SendMessage(_scintilla, SciMsg.SCI_WORDPARTRIGHT, UnusedW, Unused);
        }

        /// <summary>
        /// Move to the next change in capitalisation extending selection
        /// to new caret position.
        /// (Scintilla feature 2393)
        /// </summary>
        public void WordPartRightExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_WORDPARTRIGHTEXTEND, UnusedW, Unused);
        }

        /// <summary>
        /// Set the way the display area is determined when a particular line
        /// is to be moved to by Find, FindNext, GotoLine, etc.
        /// (Scintilla feature 2394)
        /// </summary>
        public void SetVisiblePolicy(VisiblePolicy visiblePolicy, int visibleSlop)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETVISIBLEPOLICY, (UIntPtr)visiblePolicy, (IntPtr)visibleSlop);
        }

        /// <summary>Delete back from the current position to the start of the line. (Scintilla feature 2395)</summary>
        public void DelLineLeft()
        {
            SendMessage(_scintilla, SciMsg.SCI_DELLINELEFT, UnusedW, Unused);
        }

        /// <summary>Delete forwards from the current position to the end of the line. (Scintilla feature 2396)</summary>
        public void DelLineRight()
        {
            SendMessage(_scintilla, SciMsg.SCI_DELLINERIGHT, UnusedW, Unused);
        }

        /// <summary>Set the xOffset (ie, horizontal scroll position). (Scintilla feature 2397)</summary>
        public void SetXOffset(int xOffset)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETXOFFSET, (UIntPtr)xOffset, Unused);
        }

        /// <summary>Get the xOffset (ie, horizontal scroll position). (Scintilla feature 2398)</summary>
        public int GetXOffset()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETXOFFSET, UnusedW, Unused);
        }

        /// <summary>Set the last x chosen value to be the caret x position. (Scintilla feature 2399)</summary>
        public void ChooseCaretX()
        {
            SendMessage(_scintilla, SciMsg.SCI_CHOOSECARETX, UnusedW, Unused);
        }

        /// <summary>Set the focus to this Scintilla widget. (Scintilla feature 2400)</summary>
        public void GrabFocus()
        {
            SendMessage(_scintilla, SciMsg.SCI_GRABFOCUS, UnusedW, Unused);
        }

        /// <summary>
        /// Set the way the caret is kept visible when going sideways.
        /// The exclusion zone is given in pixels.
        /// (Scintilla feature 2402)
        /// </summary>
        public void SetXCaretPolicy(CaretPolicy caretPolicy, int caretSlop)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETXCARETPOLICY, (UIntPtr)caretPolicy, (IntPtr)caretSlop);
        }

        /// <summary>
        /// Set the way the line the caret is on is kept visible.
        /// The exclusion zone is given in lines.
        /// (Scintilla feature 2403)
        /// </summary>
        public void SetYCaretPolicy(CaretPolicy caretPolicy, int caretSlop)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETYCARETPOLICY, (UIntPtr)caretPolicy, (IntPtr)caretSlop);
        }

        /// <summary>Set printing to line wrapped (SC_WRAP_WORD) or not line wrapped (SC_WRAP_NONE). (Scintilla feature 2406)</summary>
        public void SetPrintWrapMode(Wrap wrapMode)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETPRINTWRAPMODE, (UIntPtr)wrapMode, Unused);
        }

        /// <summary>Is printing line wrapped? (Scintilla feature 2407)</summary>
        public Wrap GetPrintWrapMode()
        {
            return (Wrap)SendMessage(_scintilla, SciMsg.SCI_GETPRINTWRAPMODE, UnusedW, Unused);
        }

        /// <inheritdoc cref="SciMsg.SCI_SETHOTSPOTACTIVEFORE"/>
        [Obsolete("Use the element colours APIs instead: https://www.scintilla.org/ScintillaDoc.html#ElementColours")]
        public void SetHotspotActiveFore(bool useSetting, Colour fore)
        {
        }

        /// <inheritdoc cref="SciMsg.SCI_GETHOTSPOTACTIVEFORE"/>
        [Obsolete("Use the element colours APIs instead: https://www.scintilla.org/ScintillaDoc.html#ElementColours")]
        public Colour GetHotspotActiveFore() => new Colour(0);

        /// <inheritdoc cref="SciMsg.SCI_SETHOTSPOTACTIVEBACK"/>
        [Obsolete("Use the element colours APIs instead: https://www.scintilla.org/ScintillaDoc.html#ElementColours")]
        public void SetHotspotActiveBack(bool useSetting, Colour back)
        {
        }

        /// <inheritdoc cref="SciMsg.SCI_GETHOTSPOTACTIVEBACK"/>
        [Obsolete("Use the element colours APIs instead: https://www.scintilla.org/ScintillaDoc.html#ElementColours")]
        public Colour GetHotspotActiveBack() => new Colour(0xffffff);

        /// <summary>Enable / Disable underlining active hotspots. (Scintilla feature 2412)</summary>
        public void SetHotspotActiveUnderline(bool underline)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETHOTSPOTACTIVEUNDERLINE, new UIntPtr(underline ? 1U : 0U), Unused);
        }

        /// <summary>Get whether underlining for active hotspots. (Scintilla feature 2496)</summary>
        public bool GetHotspotActiveUnderline()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETHOTSPOTACTIVEUNDERLINE, UnusedW, Unused);
        }

        /// <summary>Limit hotspots to single line so hotspots on two lines don't merge. (Scintilla feature 2421)</summary>
        public void SetHotspotSingleLine(bool singleLine)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETHOTSPOTSINGLELINE, new UIntPtr(singleLine ? 1U : 0U), Unused);
        }

        /// <summary>Get the HotspotSingleLine property (Scintilla feature 2497)</summary>
        public bool GetHotspotSingleLine()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETHOTSPOTSINGLELINE, UnusedW, Unused);
        }

        /// <summary>Move caret down one paragraph (delimited by empty lines). (Scintilla feature 2413)</summary>
        public void ParaDown()
        {
            SendMessage(_scintilla, SciMsg.SCI_PARADOWN, UnusedW, Unused);
        }

        /// <summary>Extend selection down one paragraph (delimited by empty lines). (Scintilla feature 2414)</summary>
        public void ParaDownExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_PARADOWNEXTEND, UnusedW, Unused);
        }

        /// <summary>Move caret up one paragraph (delimited by empty lines). (Scintilla feature 2415)</summary>
        public void ParaUp()
        {
            SendMessage(_scintilla, SciMsg.SCI_PARAUP, UnusedW, Unused);
        }

        /// <summary>Extend selection up one paragraph (delimited by empty lines). (Scintilla feature 2416)</summary>
        public void ParaUpExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_PARAUPEXTEND, UnusedW, Unused);
        }

        /// <summary>
        /// Given a valid document position, return the previous position taking code
        /// page into account. Returns 0 if passed 0.
        /// (Scintilla feature 2417)
        /// </summary>
        public int PositionBefore(int pos)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_POSITIONBEFORE, (UIntPtr)pos, Unused);
        }

        /// <summary>
        /// Given a valid document position, return the next position taking code
        /// page into account. Maximum value returned is the last position in the document.
        /// (Scintilla feature 2418)
        /// </summary>
        public int PositionAfter(int pos)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_POSITIONAFTER, (UIntPtr)pos, Unused);
        }

        /// <summary>
        /// Given a valid document position, return a position that differs in a number
        /// of characters. Returned value is always between 0 and last position in document.
        /// (Scintilla feature 2670)
        /// </summary>
        public int PositionRelative(int pos, int relative)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_POSITIONRELATIVE, (UIntPtr)pos, (IntPtr)relative);
        }

        /// <summary>
        /// Given a valid document position, return a position that differs in a number
        /// of UTF-16 code units. Returned value is always between 0 and last position in document.
        /// The result may point half way (2 bytes) inside a non-BMP character.
        /// (Scintilla feature 2716)
        /// </summary>
        public int PositionRelativeCodeUnits(int pos, int relative)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_POSITIONRELATIVECODEUNITS, (UIntPtr)pos, (IntPtr)relative);
        }

        /// <summary>Copy a range of text to the clipboard. Positions are clipped into the document. (Scintilla feature 2419)</summary>
        public void CopyRange(int start, int end)
        {
            SendMessage(_scintilla, SciMsg.SCI_COPYRANGE, (UIntPtr)start, (IntPtr)end);
        }

        /// <summary>Copy argument text to the clipboard. (Scintilla feature 2420)</summary>
        public unsafe void CopyText(int length, string text)
        {
            fixed (byte* textPtr = Encoding.UTF8.GetBytes(text))
            {
                SendMessage(_scintilla, SciMsg.SCI_COPYTEXT, (UIntPtr)length, (IntPtr)textPtr);
            }
        }

        /// <summary>
        /// Set the selection mode to stream (SC_SEL_STREAM) or rectangular (SC_SEL_RECTANGLE/SC_SEL_THIN) or
        /// by lines (SC_SEL_LINES).
        /// (Scintilla feature 2422)
        /// </summary>
        public void SetSelectionMode(SelectionMode selectionMode)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETSELECTIONMODE, (UIntPtr)selectionMode, Unused);
        }

        /// <summary>Get the mode of the current selection. (Scintilla feature 2423)</summary>
        public SelectionMode GetSelectionMode()
        {
            return (SelectionMode)SendMessage(_scintilla, SciMsg.SCI_GETSELECTIONMODE, UnusedW, Unused);
        }

        /// <summary>Get whether or not regular caret moves will extend or reduce the selection. (Scintilla feature 2706)</summary>
        public bool GetMoveExtendsSelection()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETMOVEEXTENDSSELECTION, UnusedW, Unused);
        }

        /// <summary>Retrieve the position of the start of the selection at the given line (INVALID_POSITION if no selection on this line). (Scintilla feature 2424)</summary>
        public int GetLineSelStartPosition(int line)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETLINESELSTARTPOSITION, (UIntPtr)line, Unused);
        }

        /// <summary>Retrieve the position of the end of the selection at the given line (INVALID_POSITION if no selection on this line). (Scintilla feature 2425)</summary>
        public int GetLineSelEndPosition(int line)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETLINESELENDPOSITION, (UIntPtr)line, Unused);
        }

        /// <summary>Move caret down one line, extending rectangular selection to new caret position. (Scintilla feature 2426)</summary>
        public void LineDownRectExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_LINEDOWNRECTEXTEND, UnusedW, Unused);
        }

        /// <summary>Move caret up one line, extending rectangular selection to new caret position. (Scintilla feature 2427)</summary>
        public void LineUpRectExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_LINEUPRECTEXTEND, UnusedW, Unused);
        }

        /// <summary>Move caret left one character, extending rectangular selection to new caret position. (Scintilla feature 2428)</summary>
        public void CharLeftRectExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_CHARLEFTRECTEXTEND, UnusedW, Unused);
        }

        /// <summary>Move caret right one character, extending rectangular selection to new caret position. (Scintilla feature 2429)</summary>
        public void CharRightRectExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_CHARRIGHTRECTEXTEND, UnusedW, Unused);
        }

        /// <summary>Move caret to first position on line, extending rectangular selection to new caret position. (Scintilla feature 2430)</summary>
        public void HomeRectExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_HOMERECTEXTEND, UnusedW, Unused);
        }

        /// <summary>
        /// Move caret to before first visible character on line.
        /// If already there move to first character on line.
        /// In either case, extend rectangular selection to new caret position.
        /// (Scintilla feature 2431)
        /// </summary>
        public void VCHomeRectExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_VCHOMERECTEXTEND, UnusedW, Unused);
        }

        /// <summary>Move caret to last position on line, extending rectangular selection to new caret position. (Scintilla feature 2432)</summary>
        public void LineEndRectExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_LINEENDRECTEXTEND, UnusedW, Unused);
        }

        /// <summary>Move caret one page up, extending rectangular selection to new caret position. (Scintilla feature 2433)</summary>
        public void PageUpRectExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_PAGEUPRECTEXTEND, UnusedW, Unused);
        }

        /// <summary>Move caret one page down, extending rectangular selection to new caret position. (Scintilla feature 2434)</summary>
        public void PageDownRectExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_PAGEDOWNRECTEXTEND, UnusedW, Unused);
        }

        /// <summary>Move caret to top of page, or one page up if already at top of page. (Scintilla feature 2435)</summary>
        public void StutteredPageUp()
        {
            SendMessage(_scintilla, SciMsg.SCI_STUTTEREDPAGEUP, UnusedW, Unused);
        }

        /// <summary>Move caret to top of page, or one page up if already at top of page, extending selection to new caret position. (Scintilla feature 2436)</summary>
        public void StutteredPageUpExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_STUTTEREDPAGEUPEXTEND, UnusedW, Unused);
        }

        /// <summary>Move caret to bottom of page, or one page down if already at bottom of page. (Scintilla feature 2437)</summary>
        public void StutteredPageDown()
        {
            SendMessage(_scintilla, SciMsg.SCI_STUTTEREDPAGEDOWN, UnusedW, Unused);
        }

        /// <summary>Move caret to bottom of page, or one page down if already at bottom of page, extending selection to new caret position. (Scintilla feature 2438)</summary>
        public void StutteredPageDownExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_STUTTEREDPAGEDOWNEXTEND, UnusedW, Unused);
        }

        /// <summary>Move caret left one word, position cursor at end of word. (Scintilla feature 2439)</summary>
        public void WordLeftEnd()
        {
            SendMessage(_scintilla, SciMsg.SCI_WORDLEFTEND, UnusedW, Unused);
        }

        /// <summary>Move caret left one word, position cursor at end of word, extending selection to new caret position. (Scintilla feature 2440)</summary>
        public void WordLeftEndExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_WORDLEFTENDEXTEND, UnusedW, Unused);
        }

        /// <summary>Move caret right one word, position cursor at end of word. (Scintilla feature 2441)</summary>
        public void WordRightEnd()
        {
            SendMessage(_scintilla, SciMsg.SCI_WORDRIGHTEND, UnusedW, Unused);
        }

        /// <summary>Move caret right one word, position cursor at end of word, extending selection to new caret position. (Scintilla feature 2442)</summary>
        public void WordRightEndExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_WORDRIGHTENDEXTEND, UnusedW, Unused);
        }

        /// <summary>
        /// Set the set of characters making up whitespace for when moving or selecting by word.
        /// Should be called after SetWordChars.
        /// (Scintilla feature 2443)
        /// </summary>
        public unsafe void SetWhitespaceChars(string characters)
        {
            fixed (byte* charactersPtr = Encoding.UTF8.GetBytes(characters))
            {
                SendMessage(_scintilla, SciMsg.SCI_SETWHITESPACECHARS, UnusedW, (IntPtr)charactersPtr);
            }
        }

        /// <summary>Get the set of characters making up whitespace for when moving or selecting by word. (Scintilla feature 2647)</summary>
        public unsafe string GetWhitespaceChars()
        {
            return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_GETWHITESPACECHARS);
        }

        /// <summary>
        /// Set the set of characters making up punctuation characters
        /// Should be called after SetWordChars.
        /// (Scintilla feature 2648)
        /// </summary>
        public unsafe void SetPunctuationChars(string characters)
        {
            fixed (byte* charactersPtr = Encoding.UTF8.GetBytes(characters))
            {
                SendMessage(_scintilla, SciMsg.SCI_SETPUNCTUATIONCHARS, UnusedW, (IntPtr)charactersPtr);
            }
        }

        /// <summary>Get the set of characters making up punctuation characters (Scintilla feature 2649)</summary>
        public unsafe string GetPunctuationChars()
        {
            return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_GETPUNCTUATIONCHARS);
        }

        /// <summary>Reset the set of characters for whitespace and word characters to the defaults. (Scintilla feature 2444)</summary>
        public void SetCharsDefault()
        {
            SendMessage(_scintilla, SciMsg.SCI_SETCHARSDEFAULT, UnusedW, Unused);
        }

        /// <summary>Get currently selected item position in the auto-completion list (Scintilla feature 2445)</summary>
        public int AutoCGetCurrent()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_AUTOCGETCURRENT, UnusedW, Unused);
        }

        /// <summary>
        /// Get currently selected item text in the auto-completion list
        /// Returns the length of the item text
        /// Result is NUL-terminated.
        /// (Scintilla feature 2610)
        /// </summary>
        public unsafe string AutoCGetCurrentText()
        {
            return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_AUTOCGETCURRENTTEXT);
        }

        /// <summary>Set auto-completion case insensitive behaviour to either prefer case-sensitive matches or have no preference. (Scintilla feature 2634)</summary>
        public void AutoCSetCaseInsensitiveBehaviour(CaseInsensitiveBehaviour behaviour)
        {
            SendMessage(_scintilla, SciMsg.SCI_AUTOCSETCASEINSENSITIVEBEHAVIOUR, (UIntPtr)behaviour, Unused);
        }

        /// <summary>Get auto-completion case insensitive behaviour. (Scintilla feature 2635)</summary>
        public CaseInsensitiveBehaviour AutoCGetCaseInsensitiveBehaviour()
        {
            return (CaseInsensitiveBehaviour)SendMessage(_scintilla, SciMsg.SCI_AUTOCGETCASEINSENSITIVEBEHAVIOUR, UnusedW, Unused);
        }

        /// <summary>Change the effect of autocompleting when there are multiple selections. (Scintilla feature 2636)</summary>
        public void AutoCSetMulti(MultiAutoComplete multi)
        {
            SendMessage(_scintilla, SciMsg.SCI_AUTOCSETMULTI, (UIntPtr)multi, Unused);
        }

        /// <summary>Retrieve the effect of autocompleting when there are multiple selections. (Scintilla feature 2637)</summary>
        public MultiAutoComplete AutoCGetMulti()
        {
            return (MultiAutoComplete)SendMessage(_scintilla, SciMsg.SCI_AUTOCGETMULTI, UnusedW, Unused);
        }

        /// <summary>Set the way autocompletion lists are ordered. (Scintilla feature 2660)</summary>
        public void AutoCSetOrder(Ordering order)
        {
            SendMessage(_scintilla, SciMsg.SCI_AUTOCSETORDER, (UIntPtr)order, Unused);
        }

        /// <summary>Get the way autocompletion lists are ordered. (Scintilla feature 2661)</summary>
        public Ordering AutoCGetOrder()
        {
            return (Ordering)SendMessage(_scintilla, SciMsg.SCI_AUTOCGETORDER, UnusedW, Unused);
        }

        /// <summary>Enlarge the document to a particular size of text bytes. (Scintilla feature 2446)</summary>
        public void Allocate(int bytes)
        {
            SendMessage(_scintilla, SciMsg.SCI_ALLOCATE, (UIntPtr)bytes, Unused);
        }

        /// <summary>
        /// Returns the target converted to UTF8.
        /// Return the length in bytes.
        /// (Scintilla feature 2447)
        /// </summary>
        public unsafe string TargetAsUTF8()
        {
            return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_TARGETASUTF8);
        }

        /// <summary>
        /// Set the length of the utf8 argument for calling EncodedFromUTF8.
        /// Set to -1 and the string will be measured to the first nul.
        /// (Scintilla feature 2448)
        /// </summary>
        public void SetLengthForEncode(int bytes)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETLENGTHFORENCODE, (UIntPtr)bytes, Unused);
        }

        /// <summary>
        /// Translates a UTF8 string into the document encoding.
        /// Return the length of the result in bytes.
        /// On error return 0.
        /// (Scintilla feature 2449)
        /// </summary>
        public unsafe string EncodedFromUTF8(string utf8)
        {
            fixed (byte* utf8Ptr = Encoding.UTF8.GetBytes(utf8))
            {
                return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_ENCODEDFROMUTF8, (UIntPtr)utf8Ptr);
            }
        }

        /// <summary>
        /// Find the position of a column on a line taking into account tabs and
        /// multi-byte characters. If beyond end of line, return line end position.
        /// (Scintilla feature 2456)
        /// </summary>
        public int FindColumn(int line, int column)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_FINDCOLUMN, (UIntPtr)line, (IntPtr)column);
        }

        /// <summary>Can the caret preferred x position only be changed by explicit movement commands? (Scintilla feature 2457)</summary>
        public CaretSticky GetCaretSticky()
        {
            return (CaretSticky)SendMessage(_scintilla, SciMsg.SCI_GETCARETSTICKY, UnusedW, Unused);
        }

        /// <summary>Stop the caret preferred x position changing when the user types. (Scintilla feature 2458)</summary>
        public void SetCaretSticky(CaretSticky useCaretStickyBehaviour)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETCARETSTICKY, (UIntPtr)useCaretStickyBehaviour, Unused);
        }

        /// <summary>Switch between sticky and non-sticky: meant to be bound to a key. (Scintilla feature 2459)</summary>
        public void ToggleCaretSticky()
        {
            SendMessage(_scintilla, SciMsg.SCI_TOGGLECARETSTICKY, UnusedW, Unused);
        }

        /// <summary>Enable/Disable convert-on-paste for line endings (Scintilla feature 2467)</summary>
        public void SetPasteConvertEndings(bool convert)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETPASTECONVERTENDINGS, new UIntPtr(convert ? 1U : 0U), Unused);
        }

        /// <summary>Get convert-on-paste setting (Scintilla feature 2468)</summary>
        public bool GetPasteConvertEndings()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETPASTECONVERTENDINGS, UnusedW, Unused);
        }

        /// <summary>Duplicate the selection. If selection empty duplicate the line containing the caret. (Scintilla feature 2469)</summary>
        public void SelectionDuplicate()
        {
            SendMessage(_scintilla, SciMsg.SCI_SELECTIONDUPLICATE, UnusedW, Unused);
        }

        /// <inheritdoc cref="SciMsg.SCI_SETCARETLINEBACKALPHA"/>
        [Obsolete("Use the element colours APIs instead: https://www.scintilla.org/ScintillaDoc.html#ElementColours")]
        public void SetCaretLineBackAlpha(Alpha alpha)
        {
        }

        /// <inheritdoc cref="SciMsg.SCI_GETCARETLINEBACKALPHA"/>
        [Obsolete("Use the element colours APIs instead: https://www.scintilla.org/ScintillaDoc.html#ElementColours")]
        public Alpha GetCaretLineBackAlpha() => default;

        /// <summary>Set the style of the caret to be drawn. (Scintilla feature 2512)</summary>
        public void SetCaretStyle(CaretStyle caretStyle)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETCARETSTYLE, (UIntPtr)caretStyle, Unused);
        }

        /// <summary>Returns the current style of the caret. (Scintilla feature 2513)</summary>
        public CaretStyle GetCaretStyle()
        {
            return (CaretStyle)SendMessage(_scintilla, SciMsg.SCI_GETCARETSTYLE, UnusedW, Unused);
        }

        /// <summary>Set the indicator used for IndicatorFillRange and IndicatorClearRange (Scintilla feature 2500)</summary>
        public void SetIndicatorCurrent(int indicator)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETINDICATORCURRENT, (UIntPtr)indicator, Unused);
        }

        /// <summary>Get the current indicator (Scintilla feature 2501)</summary>
        public int GetIndicatorCurrent()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETINDICATORCURRENT, UnusedW, Unused);
        }

        /// <summary>Set the value used for IndicatorFillRange (Scintilla feature 2502)</summary>
        public void SetIndicatorValue(int value)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETINDICATORVALUE, (UIntPtr)value, Unused);
        }

        /// <summary>Get the current indicator value (Scintilla feature 2503)</summary>
        public int GetIndicatorValue()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETINDICATORVALUE, UnusedW, Unused);
        }

        /// <summary>Turn a indicator on over a range. (Scintilla feature 2504)</summary>
        public void IndicatorFillRange(int start, int lengthFill)
        {
            SendMessage(_scintilla, SciMsg.SCI_INDICATORFILLRANGE, (UIntPtr)start, (IntPtr)lengthFill);
        }

        /// <summary>Turn a indicator off over a range. (Scintilla feature 2505)</summary>
        public void IndicatorClearRange(int start, int lengthClear)
        {
            SendMessage(_scintilla, SciMsg.SCI_INDICATORCLEARRANGE, (UIntPtr)start, (IntPtr)lengthClear);
        }

        /// <summary>Are any indicators present at pos? (Scintilla feature 2506)</summary>
        public int IndicatorAllOnFor(int pos)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_INDICATORALLONFOR, (UIntPtr)pos, Unused);
        }

        /// <summary>What value does a particular indicator have at a position? (Scintilla feature 2507)</summary>
        public int IndicatorValueAt(int indicator, int pos)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_INDICATORVALUEAT, (UIntPtr)indicator, (IntPtr)pos);
        }

        /// <summary>Where does a particular indicator start? (Scintilla feature 2508)</summary>
        public int IndicatorStart(int indicator, int pos)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_INDICATORSTART, (UIntPtr)indicator, (IntPtr)pos);
        }

        /// <summary>Where does a particular indicator end? (Scintilla feature 2509)</summary>
        public int IndicatorEnd(int indicator, int pos)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_INDICATOREND, (UIntPtr)indicator, (IntPtr)pos);
        }

        /// <summary>Set number of entries in position cache (Scintilla feature 2514)</summary>
        public void SetPositionCache(int size)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETPOSITIONCACHE, (UIntPtr)size, Unused);
        }

        /// <summary>How many entries are allocated to the position cache? (Scintilla feature 2515)</summary>
        public int GetPositionCache()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETPOSITIONCACHE, UnusedW, Unused);
        }

        /// <summary>Copy the selection, if selection empty copy the line with the caret (Scintilla feature 2519)</summary>
        public void CopyAllowLine()
        {
            SendMessage(_scintilla, SciMsg.SCI_COPYALLOWLINE, UnusedW, Unused);
        }

        /// <summary>
        /// Compact the document buffer and return a read-only pointer to the
        /// characters in the document.
        /// (Scintilla feature 2520)
        /// </summary>
        public IntPtr GetCharacterPointer()
        {
            return SendMessage(_scintilla, SciMsg.SCI_GETCHARACTERPOINTER, UnusedW, Unused);
        }

        /// <summary>
        /// Return a read-only pointer to a range of characters in the document.
        /// May move the gap so that the range is contiguous, but will only move up
        /// to lengthRange bytes.
        /// (Scintilla feature 2643)
        /// </summary>
        public IntPtr GetRangePointer(int start, int lengthRange)
        {
            return SendMessage(_scintilla, SciMsg.SCI_GETRANGEPOINTER, (UIntPtr)start, (IntPtr)lengthRange);
        }

        /// <summary>
        /// Return a position which, to avoid performance costs, should not be within
        /// the range of a call to GetRangePointer.
        /// (Scintilla feature 2644)
        /// </summary>
        public int GetGapPosition()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETGAPPOSITION, UnusedW, Unused);
        }

        /// <summary>Set the alpha fill colour of the given indicator. (Scintilla feature 2523)</summary>
        public void IndicSetAlpha(int indicator, Alpha alpha)
        {
            SendMessage(_scintilla, SciMsg.SCI_INDICSETALPHA, (UIntPtr)indicator, (IntPtr)alpha);
        }

        /// <summary>Get the alpha fill colour of the given indicator. (Scintilla feature 2524)</summary>
        public Alpha IndicGetAlpha(int indicator)
        {
            return (Alpha)SendMessage(_scintilla, SciMsg.SCI_INDICGETALPHA, (UIntPtr)indicator, Unused);
        }

        /// <summary>Set the alpha outline colour of the given indicator. (Scintilla feature 2558)</summary>
        public void IndicSetOutlineAlpha(int indicator, Alpha alpha)
        {
            SendMessage(_scintilla, SciMsg.SCI_INDICSETOUTLINEALPHA, (UIntPtr)indicator, (IntPtr)alpha);
        }

        /// <summary>Get the alpha outline colour of the given indicator. (Scintilla feature 2559)</summary>
        public Alpha IndicGetOutlineAlpha(int indicator)
        {
            return (Alpha)SendMessage(_scintilla, SciMsg.SCI_INDICGETOUTLINEALPHA, (UIntPtr)indicator, Unused);
        }

        /// <summary>Set extra ascent for each line (Scintilla feature 2525)</summary>
        public void SetExtraAscent(int extraAscent)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETEXTRAASCENT, (UIntPtr)extraAscent, Unused);
        }

        /// <summary>Get extra ascent for each line (Scintilla feature 2526)</summary>
        public int GetExtraAscent()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETEXTRAASCENT, UnusedW, Unused);
        }

        /// <summary>Set extra descent for each line (Scintilla feature 2527)</summary>
        public void SetExtraDescent(int extraDescent)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETEXTRADESCENT, (UIntPtr)extraDescent, Unused);
        }

        /// <summary>Get extra descent for each line (Scintilla feature 2528)</summary>
        public int GetExtraDescent()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETEXTRADESCENT, UnusedW, Unused);
        }

        /// <summary>Which symbol was defined for markerNumber with MarkerDefine (Scintilla feature 2529)</summary>
        public int MarkerSymbolDefined(int markerNumber)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_MARKERSYMBOLDEFINED, (UIntPtr)markerNumber, Unused);
        }

        /// <summary>Set the text in the text margin for a line (Scintilla feature 2530)</summary>
        public unsafe void MarginSetText(int line, string text)
        {
            fixed (byte* textPtr = Encoding.UTF8.GetBytes(text))
            {
                SendMessage(_scintilla, SciMsg.SCI_MARGINSETTEXT, (UIntPtr)line, (IntPtr)textPtr);
            }
        }

        /// <summary>Get the text in the text margin for a line (Scintilla feature 2531)</summary>
        public unsafe string MarginGetText(int line)
        {
            return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_MARGINGETTEXT, (UIntPtr)line);
        }

        /// <summary>Set the style number for the text margin for a line (Scintilla feature 2532)</summary>
        public void MarginSetStyle(int line, int style)
        {
            SendMessage(_scintilla, SciMsg.SCI_MARGINSETSTYLE, (UIntPtr)line, (IntPtr)style);
        }

        /// <summary>Get the style number for the text margin for a line (Scintilla feature 2533)</summary>
        public int MarginGetStyle(int line)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_MARGINGETSTYLE, (UIntPtr)line, Unused);
        }

        /// <summary>Set the style in the text margin for a line (Scintilla feature 2534)</summary>
        public unsafe void MarginSetStyles(int line, string styles)
        {
            fixed (byte* stylesPtr = Encoding.UTF8.GetBytes(styles))
            {
                SendMessage(_scintilla, SciMsg.SCI_MARGINSETSTYLES, (UIntPtr)line, (IntPtr)stylesPtr);
            }
        }

        /// <summary>Get the styles in the text margin for a line (Scintilla feature 2535)</summary>
        public unsafe string MarginGetStyles(int line)
        {
            return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_MARGINGETSTYLES, (UIntPtr)line);
        }

        /// <summary>Clear the margin text on all lines (Scintilla feature 2536)</summary>
        public void MarginTextClearAll()
        {
            SendMessage(_scintilla, SciMsg.SCI_MARGINTEXTCLEARALL, UnusedW, Unused);
        }

        /// <summary>Get the start of the range of style numbers used for margin text (Scintilla feature 2537)</summary>
        public void MarginSetStyleOffset(int style)
        {
            SendMessage(_scintilla, SciMsg.SCI_MARGINSETSTYLEOFFSET, (UIntPtr)style, Unused);
        }

        /// <summary>Get the start of the range of style numbers used for margin text (Scintilla feature 2538)</summary>
        public int MarginGetStyleOffset()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_MARGINGETSTYLEOFFSET, UnusedW, Unused);
        }

        /// <summary>Set the margin options. (Scintilla feature 2539)</summary>
        public void SetMarginOptions(MarginOption marginOptions)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETMARGINOPTIONS, (UIntPtr)marginOptions, Unused);
        }

        /// <summary>Get the margin options. (Scintilla feature 2557)</summary>
        public MarginOption GetMarginOptions()
        {
            return (MarginOption)SendMessage(_scintilla, SciMsg.SCI_GETMARGINOPTIONS, UnusedW, Unused);
        }

        /// <summary>Set the annotation text for a line (Scintilla feature 2540)</summary>
        public unsafe void AnnotationSetText(int line, string text)
        {
            fixed (byte* textPtr = Encoding.UTF8.GetBytes(text))
            {
                SendMessage(_scintilla, SciMsg.SCI_ANNOTATIONSETTEXT, (UIntPtr)line, (IntPtr)textPtr);
            }
        }

        /// <summary>Get the annotation text for a line (Scintilla feature 2541)</summary>
        public unsafe string AnnotationGetText(int line)
        {
            return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_ANNOTATIONGETTEXT, (UIntPtr)line);
        }

        /// <summary>Set the style number for the annotations for a line (Scintilla feature 2542)</summary>
        public void AnnotationSetStyle(int line, int style)
        {
            SendMessage(_scintilla, SciMsg.SCI_ANNOTATIONSETSTYLE, (UIntPtr)line, (IntPtr)style);
        }

        /// <summary>Get the style number for the annotations for a line (Scintilla feature 2543)</summary>
        public int AnnotationGetStyle(int line)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_ANNOTATIONGETSTYLE, (UIntPtr)line, Unused);
        }

        /// <summary>Set the annotation styles for a line (Scintilla feature 2544)</summary>
        public unsafe void AnnotationSetStyles(int line, string styles)
        {
            fixed (byte* stylesPtr = Encoding.UTF8.GetBytes(styles))
            {
                SendMessage(_scintilla, SciMsg.SCI_ANNOTATIONSETSTYLES, (UIntPtr)line, (IntPtr)stylesPtr);
            }
        }

        /// <summary>Get the annotation styles for a line (Scintilla feature 2545)</summary>
        public unsafe string AnnotationGetStyles(int line)
        {
            return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_ANNOTATIONGETSTYLES, (UIntPtr)line);
        }

        /// <summary>Get the number of annotation lines for a line (Scintilla feature 2546)</summary>
        public int AnnotationGetLines(int line)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_ANNOTATIONGETLINES, (UIntPtr)line, Unused);
        }

        /// <summary>Clear the annotations from all lines (Scintilla feature 2547)</summary>
        public void AnnotationClearAll()
        {
            SendMessage(_scintilla, SciMsg.SCI_ANNOTATIONCLEARALL, UnusedW, Unused);
        }

        /// <summary>Set the visibility for the annotations for a view (Scintilla feature 2548)</summary>
        public void AnnotationSetVisible(AnnotationVisible visible)
        {
            SendMessage(_scintilla, SciMsg.SCI_ANNOTATIONSETVISIBLE, (UIntPtr)visible, Unused);
        }

        /// <summary>Get the visibility for the annotations for a view (Scintilla feature 2549)</summary>
        public AnnotationVisible AnnotationGetVisible()
        {
            return (AnnotationVisible)SendMessage(_scintilla, SciMsg.SCI_ANNOTATIONGETVISIBLE, UnusedW, Unused);
        }

        /// <summary>Get the start of the range of style numbers used for annotations (Scintilla feature 2550)</summary>
        public void AnnotationSetStyleOffset(int style)
        {
            SendMessage(_scintilla, SciMsg.SCI_ANNOTATIONSETSTYLEOFFSET, (UIntPtr)style, Unused);
        }

        /// <summary>Get the start of the range of style numbers used for annotations (Scintilla feature 2551)</summary>
        public int AnnotationGetStyleOffset()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_ANNOTATIONGETSTYLEOFFSET, UnusedW, Unused);
        }

        /// <summary>Release all extended (>255) style numbers (Scintilla feature 2552)</summary>
        public void ReleaseAllExtendedStyles()
        {
            SendMessage(_scintilla, SciMsg.SCI_RELEASEALLEXTENDEDSTYLES, UnusedW, Unused);
        }

        /// <summary>Allocate some extended (>255) style numbers and return the start of the range (Scintilla feature 2553)</summary>
        public int AllocateExtendedStyles(int numberStyles)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_ALLOCATEEXTENDEDSTYLES, (UIntPtr)numberStyles, Unused);
        }

        /// <summary>Add a container action to the undo stack (Scintilla feature 2560)</summary>
        public void AddUndoAction(int token, UndoFlags flags)
        {
            SendMessage(_scintilla, SciMsg.SCI_ADDUNDOACTION, (UIntPtr)token, (IntPtr)flags);
        }

        /// <summary>Find the position of a character from a point within the window. (Scintilla feature 2561)</summary>
        public int CharPositionFromPoint(int x, int y)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_CHARPOSITIONFROMPOINT, (UIntPtr)x, (IntPtr)y);
        }

        /// <summary>
        /// Find the position of a character from a point within the window.
        /// Return INVALID_POSITION if not close to text.
        /// (Scintilla feature 2562)
        /// </summary>
        public int CharPositionFromPointClose(int x, int y)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_CHARPOSITIONFROMPOINTCLOSE, (UIntPtr)x, (IntPtr)y);
        }

        /// <summary>Set whether switching to rectangular mode while selecting with the mouse is allowed. (Scintilla feature 2668)</summary>
        public void SetMouseSelectionRectangularSwitch(bool mouseSelectionRectangularSwitch)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETMOUSESELECTIONRECTANGULARSWITCH, new UIntPtr(mouseSelectionRectangularSwitch ? 1U : 0U), Unused);
        }

        /// <summary>Whether switching to rectangular mode while selecting with the mouse is allowed. (Scintilla feature 2669)</summary>
        public bool GetMouseSelectionRectangularSwitch()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETMOUSESELECTIONRECTANGULARSWITCH, UnusedW, Unused);
        }

        /// <summary>Set whether multiple selections can be made (Scintilla feature 2563)</summary>
        public void SetMultipleSelection(bool multipleSelection)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETMULTIPLESELECTION, new UIntPtr(multipleSelection ? 1U : 0U), Unused);
        }

        /// <summary>Whether multiple selections can be made (Scintilla feature 2564)</summary>
        public bool GetMultipleSelection()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETMULTIPLESELECTION, UnusedW, Unused);
        }

        /// <summary>Set whether typing can be performed into multiple selections (Scintilla feature 2565)</summary>
        public void SetAdditionalSelectionTyping(bool additionalSelectionTyping)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETADDITIONALSELECTIONTYPING, new UIntPtr(additionalSelectionTyping ? 1U : 0U), Unused);
        }

        /// <summary>Whether typing can be performed into multiple selections (Scintilla feature 2566)</summary>
        public bool GetAdditionalSelectionTyping()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETADDITIONALSELECTIONTYPING, UnusedW, Unused);
        }

        /// <summary>Set whether additional carets will blink (Scintilla feature 2567)</summary>
        public void SetAdditionalCaretsBlink(bool additionalCaretsBlink)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETADDITIONALCARETSBLINK, new UIntPtr(additionalCaretsBlink ? 1U : 0U), Unused);
        }

        /// <summary>Whether additional carets will blink (Scintilla feature 2568)</summary>
        public bool GetAdditionalCaretsBlink()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETADDITIONALCARETSBLINK, UnusedW, Unused);
        }

        /// <summary>Set whether additional carets are visible (Scintilla feature 2608)</summary>
        public void SetAdditionalCaretsVisible(bool additionalCaretsVisible)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETADDITIONALCARETSVISIBLE, new UIntPtr(additionalCaretsVisible ? 1U : 0U), Unused);
        }

        /// <summary>Whether additional carets are visible (Scintilla feature 2609)</summary>
        public bool GetAdditionalCaretsVisible()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETADDITIONALCARETSVISIBLE, UnusedW, Unused);
        }

        /// <summary>How many selections are there? (Scintilla feature 2570)</summary>
        public int GetSelections()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETSELECTIONS, UnusedW, Unused);
        }

        /// <summary>Is every selected range empty? (Scintilla feature 2650)</summary>
        public bool GetSelectionEmpty()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETSELECTIONEMPTY, UnusedW, Unused);
        }

        /// <summary>Clear selections to a single empty stream selection (Scintilla feature 2571)</summary>
        public void ClearSelections()
        {
            SendMessage(_scintilla, SciMsg.SCI_CLEARSELECTIONS, UnusedW, Unused);
        }

        /// <summary>Set a simple selection (Scintilla feature 2572)</summary>
        public void SetSelection(int caret, int anchor)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETSELECTION, (UIntPtr)caret, (IntPtr)anchor);
        }

        /// <summary>Add a selection (Scintilla feature 2573)</summary>
        public void AddSelection(int caret, int anchor)
        {
            SendMessage(_scintilla, SciMsg.SCI_ADDSELECTION, (UIntPtr)caret, (IntPtr)anchor);
        }

        /// <summary>Drop one selection (Scintilla feature 2671)</summary>
        public void DropSelectionN(int selection)
        {
            SendMessage(_scintilla, SciMsg.SCI_DROPSELECTIONN, (UIntPtr)selection, Unused);
        }

        /// <summary>Set the main selection (Scintilla feature 2574)</summary>
        public void SetMainSelection(int selection)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETMAINSELECTION, (UIntPtr)selection, Unused);
        }

        /// <summary>Which selection is the main selection (Scintilla feature 2575)</summary>
        public int GetMainSelection()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETMAINSELECTION, UnusedW, Unused);
        }

        /// <summary>Set the caret position of the nth selection. (Scintilla feature 2576)</summary>
        public void SetSelectionNCaret(int selection, int caret)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETSELECTIONNCARET, (UIntPtr)selection, (IntPtr)caret);
        }

        /// <summary>Return the caret position of the nth selection. (Scintilla feature 2577)</summary>
        public int GetSelectionNCaret(int selection)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETSELECTIONNCARET, (UIntPtr)selection, Unused);
        }

        /// <summary>Set the anchor position of the nth selection. (Scintilla feature 2578)</summary>
        public void SetSelectionNAnchor(int selection, int anchor)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETSELECTIONNANCHOR, (UIntPtr)selection, (IntPtr)anchor);
        }

        /// <summary>Return the anchor position of the nth selection. (Scintilla feature 2579)</summary>
        public int GetSelectionNAnchor(int selection)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETSELECTIONNANCHOR, (UIntPtr)selection, Unused);
        }

        /// <summary>Set the virtual space of the caret of the nth selection. (Scintilla feature 2580)</summary>
        public void SetSelectionNCaretVirtualSpace(int selection, int space)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETSELECTIONNCARETVIRTUALSPACE, (UIntPtr)selection, (IntPtr)space);
        }

        /// <summary>Return the virtual space of the caret of the nth selection. (Scintilla feature 2581)</summary>
        public int GetSelectionNCaretVirtualSpace(int selection)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETSELECTIONNCARETVIRTUALSPACE, (UIntPtr)selection, Unused);
        }

        /// <summary>Set the virtual space of the anchor of the nth selection. (Scintilla feature 2582)</summary>
        public void SetSelectionNAnchorVirtualSpace(int selection, int space)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETSELECTIONNANCHORVIRTUALSPACE, (UIntPtr)selection, (IntPtr)space);
        }

        /// <summary>Return the virtual space of the anchor of the nth selection. (Scintilla feature 2583)</summary>
        public int GetSelectionNAnchorVirtualSpace(int selection)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETSELECTIONNANCHORVIRTUALSPACE, (UIntPtr)selection, Unused);
        }

        /// <summary>Sets the position that starts the selection - this becomes the anchor. (Scintilla feature 2584)</summary>
        public void SetSelectionNStart(int selection, int anchor)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETSELECTIONNSTART, (UIntPtr)selection, (IntPtr)anchor);
        }

        /// <summary>Returns the position at the start of the selection. (Scintilla feature 2585)</summary>
        public int GetSelectionNStart(int selection)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETSELECTIONNSTART, (UIntPtr)selection, Unused);
        }

        /// <summary>Sets the position that ends the selection - this becomes the currentPosition. (Scintilla feature 2586)</summary>
        public void SetSelectionNEnd(int selection, int caret)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETSELECTIONNEND, (UIntPtr)selection, (IntPtr)caret);
        }

        /// <summary>Returns the position at the end of the selection. (Scintilla feature 2587)</summary>
        public int GetSelectionNEnd(int selection)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETSELECTIONNEND, (UIntPtr)selection, Unused);
        }

        /// <summary>Set the caret position of the rectangular selection. (Scintilla feature 2588)</summary>
        public void SetRectangularSelectionCaret(int caret)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETRECTANGULARSELECTIONCARET, (UIntPtr)caret, Unused);
        }

        /// <summary>Return the caret position of the rectangular selection. (Scintilla feature 2589)</summary>
        public int GetRectangularSelectionCaret()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETRECTANGULARSELECTIONCARET, UnusedW, Unused);
        }

        /// <summary>Set the anchor position of the rectangular selection. (Scintilla feature 2590)</summary>
        public void SetRectangularSelectionAnchor(int anchor)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETRECTANGULARSELECTIONANCHOR, (UIntPtr)anchor, Unused);
        }

        /// <summary>Return the anchor position of the rectangular selection. (Scintilla feature 2591)</summary>
        public int GetRectangularSelectionAnchor()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETRECTANGULARSELECTIONANCHOR, UnusedW, Unused);
        }

        /// <summary>Set the virtual space of the caret of the rectangular selection. (Scintilla feature 2592)</summary>
        public void SetRectangularSelectionCaretVirtualSpace(int space)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETRECTANGULARSELECTIONCARETVIRTUALSPACE, (UIntPtr)space, Unused);
        }

        /// <summary>Return the virtual space of the caret of the rectangular selection. (Scintilla feature 2593)</summary>
        public int GetRectangularSelectionCaretVirtualSpace()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETRECTANGULARSELECTIONCARETVIRTUALSPACE, UnusedW, Unused);
        }

        /// <summary>Set the virtual space of the anchor of the rectangular selection. (Scintilla feature 2594)</summary>
        public void SetRectangularSelectionAnchorVirtualSpace(int space)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETRECTANGULARSELECTIONANCHORVIRTUALSPACE, (UIntPtr)space, Unused);
        }

        /// <summary>Return the virtual space of the anchor of the rectangular selection. (Scintilla feature 2595)</summary>
        public int GetRectangularSelectionAnchorVirtualSpace()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETRECTANGULARSELECTIONANCHORVIRTUALSPACE, UnusedW, Unused);
        }

        /// <summary>Set options for virtual space behaviour. (Scintilla feature 2596)</summary>
        public void SetVirtualSpaceOptions(VirtualSpace virtualSpaceOptions)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETVIRTUALSPACEOPTIONS, (UIntPtr)virtualSpaceOptions, Unused);
        }

        /// <summary>Return options for virtual space behaviour. (Scintilla feature 2597)</summary>
        public VirtualSpace GetVirtualSpaceOptions()
        {
            return (VirtualSpace)SendMessage(_scintilla, SciMsg.SCI_GETVIRTUALSPACEOPTIONS, UnusedW, Unused);
        }

        /// <summary>
        /// On GTK, allow selecting the modifier key to use for mouse-based
        /// rectangular selection. Often the window manager requires Alt+Mouse Drag
        /// for moving windows.
        /// Valid values are SCMOD_CTRL(default), SCMOD_ALT, or SCMOD_SUPER.
        /// (Scintilla feature 2598)
        /// </summary>
        public void SetRectangularSelectionModifier(int modifier)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETRECTANGULARSELECTIONMODIFIER, (UIntPtr)modifier, Unused);
        }

        /// <summary>Get the modifier key used for rectangular selection. (Scintilla feature 2599)</summary>
        public int GetRectangularSelectionModifier()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETRECTANGULARSELECTIONMODIFIER, UnusedW, Unused);
        }

        /// <summary>
        /// Set the foreground colour of additional selections.
        /// Must have previously called SetSelFore with non-zero first argument for this to have an effect.
        /// (Scintilla feature 2600)
        /// </summary>
        [Obsolete("Replaced in Scintilla v5 by SCI_SETELEMENTCOLOUR(SC_ELEMENT_SELECTION_ADDITIONAL_TEXT, colourAlpha)")]
        public void SetAdditionalSelFore(Colour fore)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETADDITIONALSELFORE, (UIntPtr)fore.Value, Unused);
        }

        /// <summary>
        /// Set the background colour of additional selections.
        /// Must have previously called SetSelBack with non-zero first argument for this to have an effect.
        /// (Scintilla feature 2601)
        /// </summary>
        [Obsolete("Replaced in Scintilla v5 by SCI_SETELEMENTCOLOUR(SC_ELEMENT_SELECTION_ADDITIONAL_BACK, colourAlpha)")]
        public void SetAdditionalSelBack(Colour back)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETADDITIONALSELBACK, (UIntPtr)back.Value, Unused);
        }

        /// <summary>Set the alpha of the selection. (Scintilla feature 2602)</summary>
        [Obsolete("Replaced in Scintilla v5 by SCI_SETELEMENTCOLOUR(SC_ELEMENT_SELECTION_ADDITIONAL_TEXT, colouralpha)")]
        public void SetAdditionalSelAlpha(Alpha alpha)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETADDITIONALSELALPHA, (UIntPtr)alpha, Unused);
        }

        /// <summary>Get the alpha of the selection. (Scintilla feature 2603)</summary>
        [Obsolete("Replaced in Scintilla v5 by SCI_GETELEMENTCOLOUR(SC_ELEMENT_SELECTION_ADDITIONAL_TEXT)")]
        public Alpha GetAdditionalSelAlpha()
        {
            return (Alpha)SendMessage(_scintilla, SciMsg.SCI_GETADDITIONALSELALPHA, UnusedW, Unused);
        }

        /// <summary>Set the foreground colour of additional carets. (Scintilla feature 2604)</summary>
        [Obsolete("Replaced in Scintilla v5 by SCI_SETELEMENTCOLOUR(SC_ELEMENT_CARET, colouralpha)")]
        public void SetAdditionalCaretFore(Colour fore)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETADDITIONALCARETFORE, (UIntPtr)fore.Value, Unused);
        }

        /// <summary>Get the foreground colour of additional carets. (Scintilla feature 2605)</summary>
        [Obsolete("Replaced in Scintilla v5 by SCI_GETELEMENTCOLOUR(SC_ELEMENT_CARET)")]
        public Colour GetAdditionalCaretFore()
        {
            return new Colour((int)SendMessage(_scintilla, SciMsg.SCI_GETADDITIONALCARETFORE, UnusedW, Unused));
        }

        /// <summary>Set the main selection to the next selection. (Scintilla feature 2606)</summary>
        public void RotateSelection()
        {
            SendMessage(_scintilla, SciMsg.SCI_ROTATESELECTION, UnusedW, Unused);
        }

        /// <summary>Swap that caret and anchor of the main selection. (Scintilla feature 2607)</summary>
        public void SwapMainAnchorCaret()
        {
            SendMessage(_scintilla, SciMsg.SCI_SWAPMAINANCHORCARET, UnusedW, Unused);
        }

        /// <summary>
        /// Add the next occurrence of the main selection to the set of selections as main.
        /// If the current selection is empty then select word around caret.
        /// (Scintilla feature 2688)
        /// </summary>
        public void MultipleSelectAddNext()
        {
            SendMessage(_scintilla, SciMsg.SCI_MULTIPLESELECTADDNEXT, UnusedW, Unused);
        }

        /// <summary>
        /// Add each occurrence of the main selection in the target to the set of selections.
        /// If the current selection is empty then select word around caret.
        /// (Scintilla feature 2689)
        /// </summary>
        public void MultipleSelectAddEach()
        {
            SendMessage(_scintilla, SciMsg.SCI_MULTIPLESELECTADDEACH, UnusedW, Unused);
        }

        /// <summary>
        /// Indicate that the internal state of a lexer has changed over a range and therefore
        /// there may be a need to redraw.
        /// (Scintilla feature 2617)
        /// </summary>
        public int ChangeLexerState(int start, int end)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_CHANGELEXERSTATE, (UIntPtr)start, (IntPtr)end);
        }

        /// <summary>
        /// Find the next line at or after lineStart that is a contracted fold header line.
        /// Return -1 when no more lines.
        /// (Scintilla feature 2618)
        /// </summary>
        public int ContractedFoldNext(int lineStart)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_CONTRACTEDFOLDNEXT, (UIntPtr)lineStart, Unused);
        }

        /// <summary>Centre current line in window. (Scintilla feature 2619)</summary>
        public void VerticalCentreCaret()
        {
            SendMessage(_scintilla, SciMsg.SCI_VERTICALCENTRECARET, UnusedW, Unused);
        }

        /// <summary>Move the selected lines up one line, shifting the line above after the selection (Scintilla feature 2620)</summary>
        public void MoveSelectedLinesUp()
        {
            SendMessage(_scintilla, SciMsg.SCI_MOVESELECTEDLINESUP, UnusedW, Unused);
        }

        /// <summary>Move the selected lines down one line, shifting the line below before the selection (Scintilla feature 2621)</summary>
        public void MoveSelectedLinesDown()
        {
            SendMessage(_scintilla, SciMsg.SCI_MOVESELECTEDLINESDOWN, UnusedW, Unused);
        }

        /// <summary>Set the identifier reported as idFrom in notification messages. (Scintilla feature 2622)</summary>
        public void SetIdentifier(int identifier)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETIDENTIFIER, (UIntPtr)identifier, Unused);
        }

        /// <summary>Get the identifier. (Scintilla feature 2623)</summary>
        public int GetIdentifier()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETIDENTIFIER, UnusedW, Unused);
        }

        /// <summary>Set the width for future RGBA image data. (Scintilla feature 2624)</summary>
        public void RGBAImageSetWidth(int width)
        {
            SendMessage(_scintilla, SciMsg.SCI_RGBAIMAGESETWIDTH, (UIntPtr)width, Unused);
        }

        /// <summary>Set the height for future RGBA image data. (Scintilla feature 2625)</summary>
        public void RGBAImageSetHeight(int height)
        {
            SendMessage(_scintilla, SciMsg.SCI_RGBAIMAGESETHEIGHT, (UIntPtr)height, Unused);
        }

        /// <summary>Set the scale factor in percent for future RGBA image data. (Scintilla feature 2651)</summary>
        public void RGBAImageSetScale(int scalePercent)
        {
            SendMessage(_scintilla, SciMsg.SCI_RGBAIMAGESETSCALE, (UIntPtr)scalePercent, Unused);
        }

        /// <summary>
        /// Define a marker from RGBA data.
        /// It has the width and height from RGBAImageSetWidth/Height
        /// (Scintilla feature 2626)
        /// </summary>
        public unsafe void MarkerDefineRGBAImage(int markerNumber, string pixels)
        {
            fixed (byte* pixelsPtr = Encoding.UTF8.GetBytes(pixels))
            {
                SendMessage(_scintilla, SciMsg.SCI_MARKERDEFINERGBAIMAGE, (UIntPtr)markerNumber, (IntPtr)pixelsPtr);
            }
        }

        /// <summary>
        /// Register an RGBA image for use in autocompletion lists.
        /// It has the width and height from RGBAImageSetWidth/Height
        /// (Scintilla feature 2627)
        /// </summary>
        public unsafe void RegisterRGBAImage(int type, string pixels)
        {
            fixed (byte* pixelsPtr = Encoding.UTF8.GetBytes(pixels))
            {
                SendMessage(_scintilla, SciMsg.SCI_REGISTERRGBAIMAGE, (UIntPtr)type, (IntPtr)pixelsPtr);
            }
        }

        /// <summary>Scroll to start of document. (Scintilla feature 2628)</summary>
        public void ScrollToStart()
        {
            SendMessage(_scintilla, SciMsg.SCI_SCROLLTOSTART, UnusedW, Unused);
        }

        /// <summary>Scroll to end of document. (Scintilla feature 2629)</summary>
        public void ScrollToEnd()
        {
            SendMessage(_scintilla, SciMsg.SCI_SCROLLTOEND, UnusedW, Unused);
        }

        /// <summary>Set the technology used. (Scintilla feature 2630)</summary>
        public void SetTechnology(Technology technology)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETTECHNOLOGY, (UIntPtr)technology, Unused);
        }

        /// <summary>Get the tech. (Scintilla feature 2631)</summary>
        public Technology GetTechnology()
        {
            return (Technology)SendMessage(_scintilla, SciMsg.SCI_GETTECHNOLOGY, UnusedW, Unused);
        }

        /// <summary>Create an ILoader*. (Scintilla feature 2632)</summary>
        public IntPtr CreateLoader(int bytes, DocumentOption documentOptions)
        {
            return SendMessage(_scintilla, SciMsg.SCI_CREATELOADER, (UIntPtr)bytes, (IntPtr)documentOptions);
        }

        /// <summary>On OS X, show a find indicator. (Scintilla feature 2640)</summary>
        public void FindIndicatorShow(int start, int end)
        {
            SendMessage(_scintilla, SciMsg.SCI_FINDINDICATORSHOW, (UIntPtr)start, (IntPtr)end);
        }

        /// <summary>On OS X, flash a find indicator, then fade out. (Scintilla feature 2641)</summary>
        public void FindIndicatorFlash(int start, int end)
        {
            SendMessage(_scintilla, SciMsg.SCI_FINDINDICATORFLASH, (UIntPtr)start, (IntPtr)end);
        }

        /// <summary>On OS X, hide the find indicator. (Scintilla feature 2642)</summary>
        public void FindIndicatorHide()
        {
            SendMessage(_scintilla, SciMsg.SCI_FINDINDICATORHIDE, UnusedW, Unused);
        }

        /// <summary>
        /// Move caret to before first visible character on display line.
        /// If already there move to first character on display line.
        /// (Scintilla feature 2652)
        /// </summary>
        public void VCHomeDisplay()
        {
            SendMessage(_scintilla, SciMsg.SCI_VCHOMEDISPLAY, UnusedW, Unused);
        }

        /// <summary>Like VCHomeDisplay but extending selection to new caret position. (Scintilla feature 2653)</summary>
        public void VCHomeDisplayExtend()
        {
            SendMessage(_scintilla, SciMsg.SCI_VCHOMEDISPLAYEXTEND, UnusedW, Unused);
        }

        /// <summary>Is the caret line always visible? (Scintilla feature 2654)</summary>
        public bool GetCaretLineVisibleAlways()
        {
            return 1 == (int)SendMessage(_scintilla, SciMsg.SCI_GETCARETLINEVISIBLEALWAYS, UnusedW, Unused);
        }

        /// <summary>Sets the caret line to always visible. (Scintilla feature 2655)</summary>
        public void SetCaretLineVisibleAlways(bool alwaysVisible)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETCARETLINEVISIBLEALWAYS, new UIntPtr(alwaysVisible ? 1U : 0U), Unused);
        }

        /// <summary>Set the line end types that the application wants to use. May not be used if incompatible with lexer or encoding. (Scintilla feature 2656)</summary>
        public void SetLineEndTypesAllowed(LineEndType lineEndBitSet)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETLINEENDTYPESALLOWED, (UIntPtr)lineEndBitSet, Unused);
        }

        /// <summary>Get the line end types currently allowed. (Scintilla feature 2657)</summary>
        public LineEndType GetLineEndTypesAllowed()
        {
            return (LineEndType)SendMessage(_scintilla, SciMsg.SCI_GETLINEENDTYPESALLOWED, UnusedW, Unused);
        }

        /// <summary>Get the line end types currently recognised. May be a subset of the allowed types due to lexer limitation. (Scintilla feature 2658)</summary>
        public LineEndType GetLineEndTypesActive()
        {
            return (LineEndType)SendMessage(_scintilla, SciMsg.SCI_GETLINEENDTYPESACTIVE, UnusedW, Unused);
        }

        /// <summary>Set the way a character is drawn. (Scintilla feature 2665)</summary>
        public unsafe void SetRepresentation(string encodedCharacter, string representation)
        {
            fixed (byte* encodedCharacterPtr = Encoding.UTF8.GetBytes(encodedCharacter))
            {
                fixed (byte* representationPtr = Encoding.UTF8.GetBytes(representation))
                {
                    SendMessage(_scintilla, SciMsg.SCI_SETREPRESENTATION, (UIntPtr)encodedCharacterPtr, (IntPtr)representationPtr);
                }
            }
        }

        /// <summary>
        /// Set the way a character is drawn.
        /// Result is NUL-terminated.
        /// (Scintilla feature 2666)
        /// </summary>
        public unsafe string GetRepresentation(string encodedCharacter)
        {
            fixed (byte* encodedCharacterPtr = Encoding.UTF8.GetBytes(encodedCharacter))
            {
                return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_GETREPRESENTATION, (UIntPtr)encodedCharacterPtr);
            }
        }

        /// <summary>Remove a character representation. (Scintilla feature 2667)</summary>
        public unsafe void ClearRepresentation(string encodedCharacter)
        {
            fixed (byte* encodedCharacterPtr = Encoding.UTF8.GetBytes(encodedCharacter))
            {
                SendMessage(_scintilla, SciMsg.SCI_CLEARREPRESENTATION, (UIntPtr)encodedCharacterPtr, Unused);
            }
        }

        /// <summary>Start notifying the container of all key presses and commands. (Scintilla feature 3001)</summary>
        public void StartRecord()
        {
            SendMessage(_scintilla, SciMsg.SCI_STARTRECORD, UnusedW, Unused);
        }

        /// <summary>Stop notifying the container of all key presses and commands. (Scintilla feature 3002)</summary>
        public void StopRecord()
        {
            SendMessage(_scintilla, SciMsg.SCI_STOPRECORD, UnusedW, Unused);
        }

        [Obsolete("Replaced in Scintilla v5 by SCI_SETILEXER(0, void* ilexer)")]
        public void SetLexer(int lexer) { }

        /// <summary>Retrieve the lexing language of the document. (Scintilla feature 4002)</summary>
        public int GetLexer()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETLEXER, UnusedW, Unused);
        }

        /// <summary>Colourise a segment of the document using the current lexing language. (Scintilla feature 4003)</summary>
        public void Colourise(int start, int end)
        {
            SendMessage(_scintilla, SciMsg.SCI_COLOURISE, (UIntPtr)start, (IntPtr)end);
        }

        /// <summary>Set up a value that may be used by a lexer for some optional feature. (Scintilla feature 4004)</summary>
        public unsafe void SetProperty(string key, string value)
        {
            fixed (byte* keyPtr = Encoding.UTF8.GetBytes(key))
            {
                fixed (byte* valuePtr = Encoding.UTF8.GetBytes(value))
                {
                    SendMessage(_scintilla, SciMsg.SCI_SETPROPERTY, (UIntPtr)keyPtr, (IntPtr)valuePtr);
                }
            }
        }

        /// <summary>Set up the key words used by the lexer. (Scintilla feature 4005)</summary>
        public unsafe void SetKeyWords(int keyWordSet, string keyWords)
        {
            fixed (byte* keyWordsPtr = Encoding.UTF8.GetBytes(keyWords))
            {
                SendMessage(_scintilla, SciMsg.SCI_SETKEYWORDS, (UIntPtr)keyWordSet, (IntPtr)keyWordsPtr);
            }
        }

        /// <summary>Set the lexing language of the document based on string name. (Scintilla feature 4006)</summary>
        [Obsolete("Use SCI_SETILEXER instead: https://www.scintilla.org/ScintillaDoc.html#SCI_SETILEXER", true)]
        public unsafe void SetLexerLanguage(string language)
        {
        }

        /// <summary>Load a lexer library (dll / so). (Scintilla feature 4007)</summary>
        [Obsolete("SCI_LOADLEXERLIBRARY was removed in Scintilla 5.0: https://www.scintilla.org/ScintillaDoc.html#SCI_CREATELOADER", true)]
        public unsafe void LoadLexerLibrary(string path)
        {
        }

        /// <summary>
        /// Retrieve a "property" value previously set with SetProperty.
        /// Result is NUL-terminated.
        /// (Scintilla feature 4008)
        /// </summary>
        public unsafe string GetProperty(string key)
        {
            fixed (byte* keyPtr = Encoding.UTF8.GetBytes(key))
            {
                return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_GETPROPERTY, (UIntPtr)keyPtr);
            }
        }

        /// <summary>
        /// Retrieve a "property" value previously set with SetProperty,
        /// with "$()" variable replacement on returned buffer.
        /// Result is NUL-terminated.
        /// (Scintilla feature 4009)
        /// </summary>
        public unsafe string GetPropertyExpanded(string key)
        {
            fixed (byte* keyPtr = Encoding.UTF8.GetBytes(key))
            {
                return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_GETPROPERTYEXPANDED, (UIntPtr)keyPtr);
            }
        }

        /// <summary>
        /// Retrieve a "property" value previously set with SetProperty,
        /// interpreted as an int AFTER any "$()" variable replacement.
        /// (Scintilla feature 4010)
        /// </summary>
        public unsafe int GetPropertyInt(string key, int defaultValue)
        {
            fixed (byte* keyPtr = Encoding.UTF8.GetBytes(key))
            {
                return (int)SendMessage(_scintilla, SciMsg.SCI_GETPROPERTYINT, (UIntPtr)keyPtr, (IntPtr)defaultValue);
            }
        }

        /// <summary>
        /// Retrieve the name of the lexer.
        /// Return the length of the text.
        /// Result is NUL-terminated.
        /// (Scintilla feature 4012)
        /// </summary>
        public unsafe string GetLexerLanguage()
        {
            return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_GETLEXERLANGUAGE);
        }

        /// <summary>For private communication between an application and a known lexer. (Scintilla feature 4013)</summary>
        public IntPtr PrivateLexerCall(int operation, IntPtr pointer)
        {
            return SendMessage(_scintilla, SciMsg.SCI_PRIVATELEXERCALL, (UIntPtr)operation, (IntPtr)pointer);
        }

        /// <summary>
        /// Retrieve a '\n' separated list of properties understood by the current lexer.
        /// Result is NUL-terminated.
        /// (Scintilla feature 4014)
        /// </summary>
        public unsafe string PropertyNames()
        {
            return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_PROPERTYNAMES);
        }

        /// <summary>Retrieve the type of a property. (Scintilla feature 4015)</summary>
        public unsafe TypeProperty PropertyType(string name)
        {
            fixed (byte* namePtr = Encoding.UTF8.GetBytes(name))
            {
                return (TypeProperty)SendMessage(_scintilla, SciMsg.SCI_PROPERTYTYPE, (UIntPtr)namePtr, Unused);
            }
        }

        /// <summary>
        /// Describe a property.
        /// Result is NUL-terminated.
        /// (Scintilla feature 4016)
        /// </summary>
        public unsafe string DescribeProperty(string name)
        {
            fixed (byte* namePtr = Encoding.UTF8.GetBytes(name))
            {
                return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_DESCRIBEPROPERTY, (UIntPtr)namePtr);
            }
        }

        /// <summary>
        /// Retrieve a '\n' separated list of descriptions of the keyword sets understood by the current lexer.
        /// Result is NUL-terminated.
        /// (Scintilla feature 4017)
        /// </summary>
        public unsafe string DescribeKeyWordSets()
        {
            return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_DESCRIBEKEYWORDSETS);
        }

        /// <summary>
        /// Bit set of LineEndType enumertion for which line ends beyond the standard
        /// LF, CR, and CRLF are supported by the lexer.
        /// (Scintilla feature 4018)
        /// </summary>
        public int GetLineEndTypesSupported()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETLINEENDTYPESSUPPORTED, UnusedW, Unused);
        }

        /// <summary>Allocate a set of sub styles for a particular base style, returning start of range (Scintilla feature 4020)</summary>
        public int AllocateSubStyles(int styleBase, int numberStyles)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_ALLOCATESUBSTYLES, (UIntPtr)styleBase, (IntPtr)numberStyles);
        }

        /// <summary>The starting style number for the sub styles associated with a base style (Scintilla feature 4021)</summary>
        public int GetSubStylesStart(int styleBase)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETSUBSTYLESSTART, (UIntPtr)styleBase, Unused);
        }

        /// <summary>The number of sub styles associated with a base style (Scintilla feature 4022)</summary>
        public int GetSubStylesLength(int styleBase)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETSUBSTYLESLENGTH, (UIntPtr)styleBase, Unused);
        }

        /// <summary>For a sub style, return the base style, else return the argument. (Scintilla feature 4027)</summary>
        public int GetStyleFromSubStyle(int subStyle)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETSTYLEFROMSUBSTYLE, (UIntPtr)subStyle, Unused);
        }

        /// <summary>For a secondary style, return the primary style, else return the argument. (Scintilla feature 4028)</summary>
        public int GetPrimaryStyleFromStyle(int style)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETPRIMARYSTYLEFROMSTYLE, (UIntPtr)style, Unused);
        }

        /// <summary>Free allocated sub styles (Scintilla feature 4023)</summary>
        public void FreeSubStyles()
        {
            SendMessage(_scintilla, SciMsg.SCI_FREESUBSTYLES, UnusedW, Unused);
        }

        /// <summary>Set the identifiers that are shown in a particular style (Scintilla feature 4024)</summary>
        public unsafe void SetIdentifiers(int style, string identifiers)
        {
            fixed (byte* identifiersPtr = Encoding.UTF8.GetBytes(identifiers))
            {
                SendMessage(_scintilla, SciMsg.SCI_SETIDENTIFIERS, (UIntPtr)style, (IntPtr)identifiersPtr);
            }
        }

        /// <summary>
        /// Where styles are duplicated by a feature such as active/inactive code
        /// return the distance between the two types.
        /// (Scintilla feature 4025)
        /// </summary>
        public int DistanceToSecondaryStyles()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_DISTANCETOSECONDARYSTYLES, UnusedW, Unused);
        }

        /// <summary>
        /// Get the set of base styles that can be extended with sub styles
        /// Result is NUL-terminated.
        /// (Scintilla feature 4026)
        /// </summary>
        public unsafe string GetSubStyleBases()
        {
            return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_GETSUBSTYLEBASES);
        }

        /// <summary>Retrieve the number of named styles for the lexer. (Scintilla feature 4029)</summary>
        public int GetNamedStyles()
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_GETNAMEDSTYLES, UnusedW, Unused);
        }

        /// <summary>
        /// Retrieve the name of a style.
        /// Result is NUL-terminated.
        /// (Scintilla feature 4030)
        /// </summary>
        public unsafe string NameOfStyle(int style)
        {
            return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_NAMEOFSTYLE, (UIntPtr)style);
        }

        /// <summary>
        /// Retrieve a ' ' separated list of style tags like "literal quoted string".
        /// Result is NUL-terminated.
        /// (Scintilla feature 4031)
        /// </summary>
        public unsafe string TagsOfStyle(int style)
        {
            return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_TAGSOFSTYLE, (UIntPtr)style);
        }

        /// <summary>
        /// Retrieve a description of a style.
        /// Result is NUL-terminated.
        /// (Scintilla feature 4032)
        /// </summary>
        public unsafe string DescriptionOfStyle(int style)
        {
            return GetNullStrippedStringFromMessageThatReturnsLength(SciMsg.SCI_DESCRIPTIONOFSTYLE, (UIntPtr)style);
        }

#if !SCI_DISABLE_PROVISIONAL
        /// <summary>Retrieve bidirectional text display state. (Scintilla feature 2708)</summary>
        public Bidirectional GetBidirectional()
        {
            return (Bidirectional)SendMessage(_scintilla, SciMsg.SCI_GETBIDIRECTIONAL, UnusedW, Unused);
        }

        /// <summary>Set bidirectional text display state. (Scintilla feature 2709)</summary>
        public void SetBidirectional(Bidirectional bidirectional)
        {
            SendMessage(_scintilla, SciMsg.SCI_SETBIDIRECTIONAL, (UIntPtr)bidirectional, Unused);
        }
#endif

        /// <summary>Retrieve line character index state. (Scintilla feature 2710)</summary>
        public LineCharacterIndexType GetLineCharacterIndex()
        {
            return (LineCharacterIndexType)SendMessage(_scintilla, SciMsg.SCI_GETLINECHARACTERINDEX, UnusedW, Unused);
        }

        /// <summary>Request line character index be created or its use count increased. (Scintilla feature 2711)</summary>
        public void AllocateLineCharacterIndex(LineCharacterIndexType lineCharacterIndex)
        {
            SendMessage(_scintilla, SciMsg.SCI_ALLOCATELINECHARACTERINDEX, (UIntPtr)lineCharacterIndex, Unused);
        }

        /// <summary>Decrease use count of line character index and remove if 0. (Scintilla feature 2712)</summary>
        public void ReleaseLineCharacterIndex(LineCharacterIndexType lineCharacterIndex)
        {
            SendMessage(_scintilla, SciMsg.SCI_RELEASELINECHARACTERINDEX, (UIntPtr)lineCharacterIndex, Unused);
        }

        /// <summary>Retrieve the document line containing a position measured in index units. (Scintilla feature 2713)</summary>
        public int LineFromIndexPosition(int pos, LineCharacterIndexType lineCharacterIndex)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_LINEFROMINDEXPOSITION, (UIntPtr)pos, (IntPtr)lineCharacterIndex);
        }

        /// <summary>Retrieve the position measured in index units at the start of a document line. (Scintilla feature 2714)</summary>
        public int IndexPositionFromLine(int line, LineCharacterIndexType lineCharacterIndex)
        {
            return (int)SendMessage(_scintilla, SciMsg.SCI_INDEXPOSITIONFROMLINE, (UIntPtr)line, (IntPtr)lineCharacterIndex);
        }

        /// <summary>
        /// Scintilla no longer supports style byte indicators. The last version to support style byte indicators was 3.4.2.
        /// Any use of these symbols should be removed and replaced with <a href="https://www.scintilla.org/ScintillaDoc.html#Indicators">standard indicators</a>.
        /// </summary>
        [Obsolete("See https://www.scintilla.org/ScintillaDoc.html#SCI_SETSTYLEBITS")]
        public void SetStyleBits(int _) { }

        /// <summary>Always returns <c>8</c>, indicating that 8 bits are used for styling and there are 256 styles.</summary>
        [Obsolete("See https://www.scintilla.org/ScintillaDoc.html#SCI_GETSTYLEBITS")]
        public int GetStyleBits() => 8;

        /// <inheritdoc cref="GetStyleBits" />
        [Obsolete("See https://www.scintilla.org/ScintillaDoc.html#SCI_GETSTYLEBITSNEEDED")]
        public int GetStyleBitsNeeded() => GetStyleBits();

        /// <summary>
        /// On Windows, Scintilla no longer supports narrow character windows so input is always treated as Unicode.
        /// </summary>
        [Obsolete("See https://www.scintilla.org/ScintillaDoc.html#SCI_SETKEYSUNICODE")]
        public void SetKeysUnicode(bool keysUnicode)
        {
        }

        /// <inheritdoc cref="SetKeysUnicode" />
        /// <returns><see langword="true"/></returns>
        [Obsolete("See https://www.scintilla.org/ScintillaDoc.html#SCI_GETKEYSUNICODE")]
        public bool GetKeysUnicode() => true;

        /// <summary>
        /// Single phase drawing SC_PHASES_ONE, is deprecated and should be replaced with 2-phase SC_PHASES_TWO or multi-phase SC_PHASES_MULTIPLE drawing.
        /// </summary>
        /// <returns><see langword="true"/></returns>
        [Obsolete("See https://www.scintilla.org/ScintillaDoc.html#SCI_GETTWOPHASEDRAW")]
        public bool GetTwoPhaseDraw() => true;

        /// <inheritdoc cref="GetTwoPhaseDraw" />
        [Obsolete("See https://www.scintilla.org/ScintillaDoc.html#SCI_SETTWOPHASEDRAW")]
        public void SetTwoPhaseDraw(bool twoPhase)
        {
        }

        /* --Autogenerated -- end of section automatically generated from Scintilla.iface */
    }
}
