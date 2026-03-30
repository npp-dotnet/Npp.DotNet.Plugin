/*
 * SPDX-FileCopyrightText: 2025 Robert Di Pardo <https://github.com/rdipardo>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System.Reflection;

namespace Npp.DotNet.Plugin.Tests
{
    public class Harness
    {
        public static readonly string ModuleName = "Npp.DotNet.Plugin";
        public static readonly string ModulePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

        protected static void TryExecute(Action testCase)
        {
            try
            {
                testCase();
            }
            catch (Exception e)
            {
                Assert.Fail($"{e.GetType().Name}: {e.Message}");
            }
        }
    }
}
