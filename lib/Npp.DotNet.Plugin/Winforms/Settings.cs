/*
 * SPDX-FileCopyrightText: 2024 Mark Johnston Olson <https://github.com/molsonkiko>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

namespace Npp.DotNet.Plugin.Winforms
{
    /// <summary>
    /// Manages application settings
    /// </summary>
    public class Settings : SettingsDialog
    {
        /// <inheritdoc />
        public override void ChangeMenuItemState(FuncItem itemToCheck)
        {
            base.ChangeMenuItemState(itemToCheck);
        }
    }
}
