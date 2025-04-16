/*
 * SPDX-FileCopyrightText: 2016 Kasper B. Graversen <https://github.com/kbilsted>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Npp.DotNet.Plugin.Extensions;

namespace Npp.DotNet.Plugin
{
    public class ClikeStringArray : IDisposable
    {
        internal IntPtr _nativeArray;
        internal List<IntPtr> _nativeItems;
        bool _disposed = false;

        /// <summary>
        /// Creates a new <see cref="ClikeStringArray"/> with capacity for <paramref name="num"/> strings.
        /// </summary>
        /// <param name="num">The number of strings in this <see cref="ClikeStringArray"/>.</param>
        /// <param name="stringLength">
        /// The number of <em><b>wide</b></em> characters in each string, not counting the NULL terminator.
        /// The memory size of each string in bytes will be equal to <c>stringLength x sizeof(wchar_t)</c>,
        /// or simply <c>stringLength x 2</c> on Unicode-aware versions of Windows.
        /// </param>
        /// <remarks>
        /// <paramref name="stringLength"/> should be no greater than <see cref="Win32.MAX_PATH"/>, less 1 for the NULL terminator.
        /// The constructor will truncate any larger values.
        /// </remarks>
        public ClikeStringArray(int num, int stringLength)
        {
            _nativeArray = Marshal.AllocHGlobal(UncheckedMath.Increment(num) * IntPtr.Size);
            _nativeItems = new List<IntPtr>();
            for (int i = 0; i < num; i++)
            {
                int cbSize = Math.Min(UncheckedMath.Increment(stringLength), Win32.MAX_PATH - 1) * Marshal.SystemDefaultCharSize;
                IntPtr item = Marshal.AllocHGlobal(cbSize);
                Marshal.WriteIntPtr(_nativeArray + (i * IntPtr.Size), item);
                _nativeItems.Add(item);
            }
            Marshal.WriteIntPtr(_nativeArray + (num * IntPtr.Size), IntPtr.Zero);
        }

        /// <summary>
        /// Creates a new <see cref="ClikeStringArray"/> from the given <paramref name="stringList"/>.
        /// </summary>
        /// <param name="stringList">A <see cref="List{T}"/> of managed .NET strings.</param>
        /// <remarks>
        /// Each string in <paramref name="stringList"/> should be no greater than <see cref="Win32.MAX_PATH"/>, less 1 for the NULL terminator.
        /// </remarks>
        public ClikeStringArray(List<string> stringList) : this(stringList.Count, Win32.MAX_PATH - 1)
        {
            for (int i = 0; i < stringList.Count; i++)
            {
                byte[] bytes = Encoding.Unicode.GetBytes(stringList[i]);
                Marshal.Copy(bytes, 0, _nativeItems[i], bytes.Length);
            }
        }

        public IntPtr NativePointer { get { return _nativeArray; } }
        public List<string> ManagedStringsAnsi { get { return _getManagedItems(false); } }
        public List<string> ManagedStringsUnicode { get { return _getManagedItems(true); } }
        List<string> _getManagedItems(bool unicode)
        {
            List<string> _managedItems = new List<string>();
            for (int i = 0; i < _nativeItems.Count; i++)
            {
                if (unicode) _managedItems.Add(Marshal.PtrToStringUni(_nativeItems[i]));
                else _managedItems.Add(Marshal.PtrToStringAnsi(_nativeItems[i]));
            }
            return _managedItems;
        }

        public void Dispose()
        {
            try
            {
                if (!_disposed)
                {
                    for (int i = 0; i < _nativeItems.Count; i++)
                        if (_nativeItems[i] != IntPtr.Zero) Marshal.FreeHGlobal(_nativeItems[i]);
                    if (_nativeArray != IntPtr.Zero) Marshal.FreeHGlobal(_nativeArray);
                    GC.SuppressFinalize(this);
                    _disposed = true;
                }
            }
            catch (Exception e)
            {
                _ = Win32.MsgBoxDialog(IntPtr.Zero, $"{nameof(Dispose)}:{e.Message}", GetType().Name, (uint)Win32.MsgBox.ICONERROR);
            }
        }
        ~ClikeStringArray()
        {
            Dispose();
        }
    }
}
