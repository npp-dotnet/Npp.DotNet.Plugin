/*
 * SPDX-FileCopyrightText: 2016 Kasper B. Graversen <https://github.com/kbilsted>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

namespace Npp.DotNet.Plugin.Extensions
{
    /// <summary>
    /// This class holds helpers for sending messages defined in the Resource_h.cs file. It is at the moment
    /// incomplete. Please help fill in the blanks.
    /// </summary>
    static class NppResource
    {
        public static void ClearIndicator()
        {
            Win32.SendMessage(PluginData.NppData.NppHandle, (uint)Resource.NPPM_INTERNAL_CLEARINDICATOR);
        }
    }
}
