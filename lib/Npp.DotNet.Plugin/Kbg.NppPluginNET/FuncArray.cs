/*
 * SPDX-FileCopyrightText: 2016 Kasper B. Graversen <https://github.com/kbilsted>
 *                         2024 Robert Di Pardo <dipardo.r@gmail.com>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Npp.DotNet.Plugin
{
    /// <summary>
    /// Encapsulates a plugin's functions as a managed collection of <see cref="FuncItem"/> items.<br/>
    /// Extracted and adapted for .NET SDK from <c>NppPluginNETHelper.cs</c>, part of
    /// <a href="https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net/">NotepadPlusPlusPluginPack.Net</a>.
    /// </summary>
    public class PluginFuncArray : IDisposable
    {
        [DllImport("kernel32")]
        internal static extern void RtlMoveMemory(IntPtr Destination, IntPtr Source, int Length);

        private static readonly int CbFuncItem = Marshal.SizeOf<FuncItem>();
        private static readonly int CbSKey = Marshal.SizeOf<ShortcutKey>();
        private static readonly int CbFuncName = Constants.MENU_TITLE_LENGTH * Marshal.SizeOf<char>();
        private static readonly int CbPtr = IntPtr.Size;
        private static readonly int CbInt32 = Marshal.SizeOf<int>();
        private readonly List<FuncItem> _funcItems = new List<FuncItem>();
        private readonly List<IntPtr> _hotKeys = new List<IntPtr>();

        public void Add(FuncItem funcItem)
        {
            int oldSize = _funcItems.Count * CbFuncItem;
            _funcItems.Add(funcItem);
            int newSize = _funcItems.Count * CbFuncItem;
            IntPtr newPointer = Marshal.AllocHGlobal(newSize);

            if (NativePointer != IntPtr.Zero)
            {
                RtlMoveMemory(newPointer, NativePointer, oldSize);
                Marshal.FreeHGlobal(NativePointer);
            }
            checked
            {
                IntPtr ptrPosNewItem = newPointer + oldSize;
                byte[] pszName = Encoding.Unicode.GetBytes($"{funcItem.ItemName}\0");
                Marshal.Copy(pszName, 0, ptrPosNewItem, pszName.Length);
                ptrPosNewItem += CbFuncName;
                IntPtr fnPtr = (funcItem.PFunc != null) ? Marshal.GetFunctionPointerForDelegate(funcItem.PFunc) : IntPtr.Zero;
                Marshal.WriteIntPtr(ptrPosNewItem, fnPtr);
                ptrPosNewItem += CbPtr;
                Marshal.WriteInt32(ptrPosNewItem, funcItem.CmdID);
                ptrPosNewItem += CbInt32;
                Marshal.WriteInt32(ptrPosNewItem, Convert.ToInt32(funcItem.Init2Check));
                ptrPosNewItem += CbInt32;

                if (funcItem.PShKey.Key != 0)
                {
                    IntPtr newShortCutKey = Marshal.AllocHGlobal(CbSKey);
                    Marshal.StructureToPtr(funcItem.PShKey, newShortCutKey, false);
                    Marshal.WriteIntPtr(ptrPosNewItem, newShortCutKey);
                }
                else Marshal.WriteIntPtr(ptrPosNewItem, IntPtr.Zero);

            }
            NativePointer = newPointer;
        }

        public void RefreshItems()
        {
            IntPtr ptrPosItem = NativePointer;
            for (int i = 0; i < _funcItems.Count; i++)
            {
                checked
                {
                    var updatedItem = new FuncItem()
                    {
                        ItemName = _funcItems[i].ItemName,
                        PFunc = _funcItems[i].PFunc,
                        CmdID = Marshal.ReadInt32(ptrPosItem += CbFuncName + CbPtr),
                        Init2Check = _funcItems[i].Init2Check,
                        PShKey = _funcItems[i].PShKey
                    };
                    ptrPosItem += CbPtr + CbInt32 * 2;
                    _funcItems[i] = updatedItem;
                }
            }
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                foreach (IntPtr ptr in _hotKeys) Marshal.FreeHGlobal(ptr);
                if (NativePointer != IntPtr.Zero) Marshal.FreeHGlobal(NativePointer);
                GC.SuppressFinalize(this);
                Disposed = true;
            }
        }

        ~PluginFuncArray()
        {
            Dispose();
        }

        public List<FuncItem> Items { get => _funcItems; }
        public IntPtr NativePointer { get; private set; }
        public bool Disposed { get; private set; }
    }
}
