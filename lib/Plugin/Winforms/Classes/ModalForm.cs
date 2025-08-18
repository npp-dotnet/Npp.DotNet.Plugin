/*
 * SPDX-FileCopyrightText: 2025 Robert Di Pardo <https://github.com/rdipardo>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

namespace Npp.DotNet.Plugin.Winforms.Classes
{
    /// <summary>
    /// A modal plugin dialog.
    /// </summary>
    public class ModalForm : FormBase
    {
        /// <summary>
        /// Creates a new <see cref="ModalForm"/>.
        /// </summary>
        public ModalForm() : base(DialogKind.Modal) { }
    }
}
