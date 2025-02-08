/*
 * SPDX-FileCopyrightText: 2024 Mark Johnston Olson <https://github.com/molsonkiko>
 *
 * SPDX-License-Identifier: Apache-2.0
 */

using System.Drawing;
using System.Windows.Forms;

namespace Npp.DotNet.Plugin.Winforms
{
    /// <summary>
    /// Provides readonly access to Notepad++'s GUI connector, with some additional helper methods.
    /// </summary>
    public static class DialogUtils
    {
        /// <summary>
        /// Connector to Notepad++'s GUI.
        /// </summary>
        public static PluginDialogBase NotepadGUI { get; } = new PluginDialogBase();

        /// <summary>
        /// Gets a translucent <see cref="Color"/> from the RGB values of <paramref name="rgb"/> and the given <paramref name="alpha"/> value.
        /// </summary>
        /// <param name="rgb">A <see cref="Colour"/> representing a 24-bit opaque color.</param>
        /// <param name="alpha">The alpha component of the returned <see cref="Color"/>.</param>
        public static Color GetColorAlpha(Colour rgb, int alpha = 0xff)
        {
            return Color.FromArgb(alpha, rgb.Red, rgb.Green, rgb.Blue);
        }

        /// <summary>
        /// Gets a translucent <see cref="Color"/> from the alpha component and RGB values of <paramref name="value"/>.
        /// </summary>
        /// <param name="value">A 32-bit color value in ABGR format.</param>
        public static Color GetColorAlpha(uint value)
        {
            return Color.FromArgb(GetByte(value, 24), GetByte(value), GetByte(value, 8), GetByte(value, 16));

#if NETCOREAPP3_0_OR_GREATER
            static
#endif
            int GetByte(uint n, int shift = 0) => unchecked(System.Convert.ToInt32(n >> shift)) & 0xFF;
        }

        /// <summary>
        /// Trying to copy an empty string or null to the clipboard raises an error.<br></br>
        /// This shows a message box if the user tries to do that.
        /// </summary>
        /// <param name="text"></param>
        public static void TryCopyToClipboard(string text)
        {
            if (text == null || text.Length == 0)
            {
                MessageBox.Show("Couldn't find anything to copy to the clipboard",
                    "Nothing to copy to clipboard",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }
            NppUtils.Editor.CopyText(text);
        }

        /// <summary>
        /// Based on the value of askUser, do one of three things:<br></br>
        /// DONT_DO_DONT_ASK: return false (don't do the thing)<br></br>
        /// ASK_BEFORE_DOING:<br></br>
        /// 1. show a Yes/No message box with text messageBoxText and caption messageBoxCaption.<br></br>
        /// 2. if and only if the user clicks Yes, return true.<br></br>
        /// DO_WITHOUT_ASKING: return true (do the thing without asking)
        /// </summary>
        /// <param name="askUser">whether to ask user</param>
        /// <param name="messageBoxText">text of message box (if and only if askUser = ASK_BEFORE_DOING</param>
        /// <param name="messageBoxCaption">caption of message box (if and only if askUser = ASK_BEFORE_DOING</param>
        /// <returns></returns>
        public static bool AskBeforeDoingSomething(AskUserWhetherToDoThing askUser, string messageBoxText, string messageBoxCaption)
        {
            switch (askUser)
            {
                case AskUserWhetherToDoThing.DONT_DO_DONT_ASK:
                    return false;
                case AskUserWhetherToDoThing.ASK_BEFORE_DOING:
                    return MessageBox.Show(messageBoxText,
                        messageBoxCaption,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question
                        ) != DialogResult.No;
                case AskUserWhetherToDoThing.DO_WITHOUT_ASKING:
                default:
                    break;
            }
            return true;
        }
    }

    public enum AskUserWhetherToDoThing
    {
        /// <summary>
        /// don't do the thing, and don't prompt the user either
        /// </summary>
        DONT_DO_DONT_ASK,
        /// <summary>
        /// prompt the user to ask whether to do it
        /// </summary>
        ASK_BEFORE_DOING,
        /// <summary>
        /// do it without prompting the user
        /// </summary>
        DO_WITHOUT_ASKING,
    }
}
