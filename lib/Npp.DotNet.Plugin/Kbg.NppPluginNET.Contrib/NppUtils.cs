/*
 * SPDX-FileCopyrightText: 2024 Mark Johnston Olson <https://github.com/molsonkiko>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

// Miscellaneous useful things like a connector to Notepad++
namespace Npp.DotNet.Plugin
{
    /// <summary>
    /// contains connectors to Scintilla (editor) and Notepad++ (notepad)
    /// </summary>
    public class NppUtils
    {
        private static IScintillaGateway _editor = new ScintillaGateway(Utils.GetCurrentScintilla());
        private static INotepadPPGateway _notepad = new NotepadPPGateway();

        /// <summary>
        /// Connector to Scintilla. Can be reset after initialization.
        /// </summary>
        public static IScintillaGateway Editor { get => _editor; set { _editor = _editor ?? value; } }

        /// <summary>
        /// Connector to Notepad++.
        /// </summary>
        public static INotepadPPGateway Notepad { get => _notepad; set { _notepad = _notepad ?? value; } }

        /// <summary>
        /// this should only be instantiated once in your entire project
        /// </summary>
        public static readonly Random Random = new Random();

        public static readonly (int, int, int) NppVersion = Notepad.GetNppVersion();

        public static readonly string NppVersionStr = NppVersionString(true);

        public static readonly bool NppVersionAtLeast8 = NppVersion.Item1 >= 8;

        public static string ConfigDirectory => Notepad.GetConfigDirectory();

        public static string AssemblyVersionString
        {
            get
            {
                string version = Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString();
                while (version.EndsWith(".0"))
                    version = version.Substring(0, version.Length - 2);
#if DEBUG
                return $"{version} Debug";
#else
                return version;
#endif // DEBUG
            }
        }

        /// <summary>
        /// append text to current doc, then append newline and move cursor
        /// </summary>
        /// <param name="inp"></param>
        public static void AddLine(string inp)
        {
            Editor.AppendText(Encoding.UTF8.GetByteCount(inp), inp);
            Editor.AppendText(Environment.NewLine.Length, Environment.NewLine);
        }

        public enum PathType
        {
            FULL_CURRENT_PATH,
            FILE_NAME,
            DIRECTORY
        }

        /// <summary>
        /// input is one of 'p', 'd', 'f'<br></br>
        /// if 'p', get full path to current file (default)<br></br>
        /// if 'd', get directory of current file<br></br>
        /// if 'f', get filename of current file
        /// </summary>
        /// <param name="which"></param>
        /// <returns></returns>
        public static string GetCurrentPath(PathType which = PathType.FULL_CURRENT_PATH)
        {
            NppMsg msg = NppMsg.NPPM_GETFULLCURRENTPATH;
            switch (which)
            {
                case PathType.FULL_CURRENT_PATH: break;
                case PathType.DIRECTORY: msg = NppMsg.NPPM_GETCURRENTDIRECTORY; break;
                case PathType.FILE_NAME: msg = NppMsg.NPPM_GETFILENAME; break;
                default: throw new ArgumentException("GetCurrentPath argument must be member of PathType enum");
            }

            StringBuilder path = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginData.NppData.NppHandle, (uint)msg, (uint)path.Capacity, path);

            return path.ToString();
        }

        /// <summary>
        /// Get the file type for a file path (no period)<br></br>
        /// Default path is the currently open file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string FileExtension(string path = null)
        {
            path = (path != null) ? path : GetCurrentPath(PathType.FILE_NAME);
            StringBuilder sb = new StringBuilder();
            for (int ii = path.Length - 1; ii >= 0; ii--)
            {
                char c = path[ii];
                if (c == '.') break;
                sb.Append(c);
            }
            // the chars were added in the wrong direction, so reverse them
            return string.Join("", sb.ToString().ToCharArray().Reverse());
        }

        public static void CreateConfigSubDirectoryIfNotExists()
        {
            var ConfigDirInfo = new DirectoryInfo(Notepad.GetConfigDirectory());
            if (!ConfigDirInfo.Exists)
                ConfigDirInfo.Create();
        }

        /// <summary>
        /// get all text starting at position start in the current document
        /// and ending at position end in the current document
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static string GetSlice(int start, int end)
        {
            int len = end - start;
            IntPtr rangePtr = Editor.GetRangePointer(start, len);
            string ansi = Marshal.PtrToStringAnsi(rangePtr, len);
            // TODO: figure out a way to do this that involves less memcopy for non-ASCII
            if (ansi.Any(c => c >= 128))
                return Encoding.UTF8.GetString(Encoding.Default.GetBytes(ansi));
            return ansi;
        }

        private static readonly string[] newlines = new string[] { "\r\n", "\r", "\n" };

        /// <summary>0: CRLF, 1: CR, 2: LF<br></br>
        /// Anything less than 0 or greater than 2: LF</summary>
        public static string GetEndOfLineString(int eolType)
        {
            if (eolType < 0 || eolType >= 3)
                return "\n";
            return newlines[eolType];
        }

        private static string NppVersionString(bool include32bitVs64bit)
        {
            (int major, int minor, int revision) = Notepad.GetNppVersion();
            string nppVerStr = $"{major}.{minor}.{revision}";
            return include32bitVs64bit ? $"{nppVerStr} {IntPtr.Size * 8}bit" : nppVerStr;
        }

        /// <summary>
        /// appends the JSON representation of char c to a StringBuilder.<br></br>
        /// for most characters, this just means appending the character itself, but for example '\n' would become "\\n", '\t' would become "\\t",<br></br>
        /// and most other chars less than 32 would be appended as "\\u00{char value in hex}" (e.g., '\x14' becomes "\\u0014")
        /// </summary>
        public static void CharToSb(StringBuilder sb, char c)
        {
            switch (c)
            {
                case '\\': sb.Append("\\\\"); break;
                case '"': sb.Append("\\\""); break;
                case '\x01': sb.Append("\\u0001"); break;
                case '\x02': sb.Append("\\u0002"); break;
                case '\x03': sb.Append("\\u0003"); break;
                case '\x04': sb.Append("\\u0004"); break;
                case '\x05': sb.Append("\\u0005"); break;
                case '\x06': sb.Append("\\u0006"); break;
                case '\x07': sb.Append("\\u0007"); break;
                case '\x08': sb.Append("\\b"); break;
                case '\x09': sb.Append("\\t"); break;
                case '\x0A': sb.Append("\\n"); break;
                case '\x0B': sb.Append("\\v"); break;
                case '\x0C': sb.Append("\\f"); break;
                case '\x0D': sb.Append("\\r"); break;
                case '\x0E': sb.Append("\\u000E"); break;
                case '\x0F': sb.Append("\\u000F"); break;
                case '\x10': sb.Append("\\u0010"); break;
                case '\x11': sb.Append("\\u0011"); break;
                case '\x12': sb.Append("\\u0012"); break;
                case '\x13': sb.Append("\\u0013"); break;
                case '\x14': sb.Append("\\u0014"); break;
                case '\x15': sb.Append("\\u0015"); break;
                case '\x16': sb.Append("\\u0016"); break;
                case '\x17': sb.Append("\\u0017"); break;
                case '\x18': sb.Append("\\u0018"); break;
                case '\x19': sb.Append("\\u0019"); break;
                case '\x1A': sb.Append("\\u001A"); break;
                case '\x1B': sb.Append("\\u001B"); break;
                case '\x1C': sb.Append("\\u001C"); break;
                case '\x1D': sb.Append("\\u001D"); break;
                case '\x1E': sb.Append("\\u001E"); break;
                case '\x1F': sb.Append("\\u001F"); break;
                default: sb.Append(c); break;
            }
        }

        /// <summary>
        /// the string representation of a JSON string
        /// if not quoted, this will not have the enclosing quotes a JSON string normally has
        /// </summary>
        public static string StrToString(string s, bool quoted)
        {
            int slen = s.Length;
            int ii = 0;
            for (; ii < slen; ii++)
            {
                char c = s[ii];
                if (c < 32 || c == '\\' || c == '"')
                    break;
            }
            if (ii == slen)
                return quoted ? $"\"{s}\"" : s;
            var sb = new StringBuilder();
            if (quoted)
                sb.Append('"');
            if (ii > 0)
            {
                ii--;
                sb.Append(s, 0, ii);
            }
            for (; ii < slen; ii++)
                CharToSb(sb, s[ii]);
            if (quoted)
                sb.Append('"');
            return sb.ToString();
        }
    }
}
