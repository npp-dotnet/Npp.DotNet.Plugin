/*
 * SPDX-FileCopyrightText: 2025 Robert Di Pardo <https://github.com/rdipardo>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System.Runtime.InteropServices;

namespace Npp.DotNet.Plugin.Tests.Buffers
{
    [TestClass]
    public partial class BufferTests : Harness
    {

        [LibraryImport("Unmanaged", EntryPoint = "GetFileCount")]
        internal static partial int GetFileCount();

        [LibraryImport("Unmanaged", EntryPoint = "GetFileNames", StringMarshalling = StringMarshalling.Utf16)]
        internal static partial void GetFileNames(int nbFileNames, IntPtr fileNames);

        private static readonly int MaxBufferLength = Win32.MAX_PATH - 1;

        /// <summary>
        /// Verifies that <see cref="ClikeStringArray(int, int)"/> initializes
        /// each unmanaged buffer with capacity for MAX_PATH - 1 wide characters.
        /// </summary>
        [TestMethod]
        public void CanBeAllocatedFormAnUnmanagedBuffer()
        {
            TryExecute(AllocateWithoutAList);
        }

        /// <summary>
        /// Verifies that <see cref="ClikeStringArray(List{string})"/> initializes
        /// each unmanaged buffer with capacity for MAX_PATH - 1 wide characters.
        /// </summary>
        [TestMethod]
        public void CanBeAllocatedFormAManagedList()
        {
            TryExecute(AllocateWithAnInitializedList);
        }

        // --------------------------------------------------------------------------------------------------
        // Test method wrappers
        // --------------------------------------------------------------------------------------------------
        private void AllocateWithoutAList()
        {
            int nbFiles = GetFileCount();
            using ClikeStringArray cStrArray = new(nbFiles, int.MaxValue - 1);

            GetFileNames(nbFiles, cStrArray.NativePointer);
            Assert.AreEqual(nbFiles, cStrArray.ManagedStringsUnicode.Count);

            foreach (string path in cStrArray.ManagedStringsUnicode)
                Assert.IsTrue(path.Length <= MaxBufferLength, $"{path.Length} > {MaxBufferLength} wide chars");
        }

        private void AllocateWithAnInitializedList()
        {
            int nbFiles = GetFileCount();
            var stringList = new List<string>();
            for (int i = 0; i < nbFiles; i++)
                stringList.Add(new string('\0', Win32.MAX_PATH));

            using ClikeStringArray cStrArray = new(stringList);

            GetFileNames(nbFiles, cStrArray.NativePointer);
            Assert.AreEqual(nbFiles, cStrArray.ManagedStringsUnicode.Count);

            foreach (string path in cStrArray.ManagedStringsUnicode)
                Assert.IsTrue(path.Length <= MaxBufferLength, $"{path.Length} > {MaxBufferLength} wide chars");
        }
    }
}
