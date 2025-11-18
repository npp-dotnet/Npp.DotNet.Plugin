/*
 * SPDX-FileCopyrightText: 2024 Robert Di Pardo <https://github.com/rdipardo>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

namespace Npp.DotNet.Plugin.Tests.Memory
{
    [TestClass]
    public class LayoutTests : Harness
    {
        private static PEImage? _dll;
        private static ModuleDefinition? _module;
        private static bool _is64bit = true;

        /// <summary>
        /// Initializes the static fields of the test class.
        /// </summary>
        /// <param name="_">Unused</param>
        /// <exception cref="InvalidOperationException">Thrown if the module under test can't be found.</exception>
        [ClassInitialize]
        public static void Setup(TestContext _)
        {
            try
            {
                string libPath = Path.Combine(ModulePath, $"{ModuleName}.dll");
                string dllPath = Path.Combine(ModulePath, $"{ModuleName}.Demo.dll");
                _module = ModuleDefinition.FromFile(libPath);
                _dll = PEImage.FromFile(dllPath);
                _is64bit = !$"{_module.MachineType}".StartsWith("I386");
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Aborting tests", e);
            }
        }

        /// <summary>
        /// Verifies that the plugin module exports the 6 essential plugin API functions.
        /// </summary>
        [TestMethod]
        public void HasFunctionExports()
        {
            TryExecute(TestExports);
        }

        /// <summary>
        /// Verifies that every field of a marshalled <c>ShortcutKey</c> struct is one byte.
        /// </summary>
        [TestMethod]
        public void HasProperlyAlignedHotKeyStruct()
        {
            TryExecute(TestLayoutOfHotkeyStruct);
        }

        /// <summary>
        /// Verifies that <c>ScNotification</c> structs have the following memory layout, i.e.:<br/>
        /// <example>
        /// <code>
        /// --------------------------------------------------------------------------------------------------
        /// |                               |              size              |             offset            |
        /// |            field              |---------------|----------------|---------------|---------------|
        /// |                               |      x86      |     x86_64     |      x86      |     x86_64    |
        /// |-------------------------------|---------------|----------------|---------------|---------------|
        /// | header {hwndFrom,idFrom,code} | {4,4,4} == 12 | {8,8,4*} == 20 |       0       |       0       |
        /// | position                      |       4       |       8        |      12       |      24       |
        /// | character                     |       4       |       4        |      16       |      32       |
        /// | modifiers                     |       4       |       4        |      20       |      36       |
        /// | modificationType              |       4       |       4*       |      24       |      40       |
        /// | textPointer                   |       4       |       8        |      28       |      48       |
        /// | length                        |       4       |       8        |      32       |      56       |
        /// | linesAdded                    |       4       |       8        |      36       |      64       |
        /// | message                       |       4       |       4*       |      40       |      72       |
        /// | wParam                        |       4       |       8        |      44       |      80       |
        /// | lParam                        |       4       |       8        |      48       |      88       |
        /// | line                          |       4       |       8        |      52       |      96       |
        /// | foldLevelNow                  |       4       |       4        |      56       |     104       |
        /// | foldLevelPrev                 |       4       |       4        |      60       |     108       |
        /// | margin                        |       4       |       4        |      64       |     112       |
        /// | listType                      |       4       |       4        |      68       |     116       |
        /// | x                             |       4       |       4        |      72       |     120       |
        /// | y                             |       4       |       4        |      76       |     124       |
        /// | token                         |       4       |       4*       |      80       |     128       |
        /// | annotationLinesAdded          |       4       |       8        |      84       |     136       |
        /// | updated                       |       4       |       4        |      88       |     144       |
        /// | listCompletionMethod          |       4       |       4        |      92       |     148       |
        /// | characterSource               |       4       |       4*       |      96       |     152       |
        /// |-------------------------------|---------------|----------------|---------------|---------------|
        /// |                            =  |      100      |      160       |     100       |     160       |
        /// --------------------------------------------------------------------------------------------------
        ///                                                             [*] padded for 8-byte alignment
        /// </code>
        /// </example>
        /// </summary>
        [TestMethod]
        public void HasProperlyAlignedScNotificationStruct()
        {
            TryExecute(TestLayoutOfNotificationStruct);
        }

        // --------------------------------------------------------------------------------------------------
        // Test method wrappers
        // --------------------------------------------------------------------------------------------------
#pragma warning disable MSTEST0017
        private void TestExports()
        {
            Assert.IsNotNull(_dll?.Exports, $"{ModuleName} has no export table");
            int exports = (_dll?.Exports?.Entries?.Count).GetValueOrDefault();
            Assert.IsGreaterThanOrEqualTo(6, exports, $"{ModuleName} exports only {exports} function(s)");
        }

        private void TestLayoutOfNotificationStruct()
        {
            uint expected = _is64bit ? 144U : 88U;
            int token = GetMetdataTokenForType("Npp.DotNet.Plugin.ScNotification");

            if (_module!.TryLookupMember(token, out IMetadataMember? member))
            {
                var typedef = (TypeDefinition)member!;
                var layout = typedef.GetImpliedMemoryLayout(!_is64bit);
                var updated = typedef.Fields.FirstOrDefault(f => $"{f.Name}".Equals("Updated", StringComparison.OrdinalIgnoreCase));
                Assert.IsNotNull(updated, "The 'ScNotification' type does not declare the 'Updated' field");
                uint actual = layout[updated].Offset;
                Assert.AreEqual(actual, expected, $"Expected to find 'ScNotification.Updated' after {expected} bytes, not {actual}");
            }
            else
            {
                throw new AssertFailedException($"{ModuleName} does not define the 'ScNotification' type");
            }
        }

        private void TestLayoutOfHotkeyStruct()
        {
            int token = GetMetdataTokenForType("Npp.DotNet.Plugin.ShortcutKey");
            if (_module!.TryLookupMember(token, out IMetadataMember? member))
            {
                var typedef = (TypeDefinition)member;
                var layout = typedef.GetImpliedMemoryLayout(!_is64bit);
                for (int i = 0; i < typedef.Fields.Count; i++)
                {
                    var field = typedef.Fields[i];
                    var structField = layout[field];
                    uint actual = structField.Offset;
                    Assert.AreEqual(actual, i * (uint)sizeof(byte), $"Expected to find '{field.Name}' after {i * sizeof(byte)} bytes, not {actual})");
                }
            }
            else
            {
                throw new AssertFailedException($"{ModuleName} does not define the 'ShortcutKey' type");
            }
        }
#pragma warning restore MSTEST0017

        private static int GetMetdataTokenForType(string typeName)
        {
            var memberType =
                MemberDiscoverer.DiscoverMembersInModule(_module!, MemberDiscoveryFlags.None).Types
                .FirstOrDefault(t => $"{t.FullName}".Equals(typeName, StringComparison.OrdinalIgnoreCase));
            return (memberType?.MetadataToken.ToInt32()).GetValueOrDefault();
        }
    }
}
