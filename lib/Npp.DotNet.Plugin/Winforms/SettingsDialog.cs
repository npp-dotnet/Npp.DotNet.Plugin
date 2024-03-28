/*
 * SPDX-FileCopyrightText: 2024 Mark Johnston Olson <https://github.com/molsonkiko>
 *                         2017-2020 Joakim Wennergren <joakim.wennergren@neovici.se>
 *
 * SPDX-License-Identifier: Apache-2.0 OR LicenseRef-CsvQuery
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace Npp.DotNet.Plugin.Winforms
{
    /// <summary>
    /// Extends <see cref="Npp.DotNet.Plugin.SettingsBase"/> with forms for modifying plugin settings.
    /// </summary>
    public class SettingsDialog : SettingsBase
    {
        private const int DEFAULT_WIDTH = 430;
        private const int DEFAULT_HEIGHT = 300;

        /// <summary> Delegate for update events </summary>
        public delegate void RepositoryEventHandler(object sender, SettingsChangedEventArgs e);

        /// <summary> Raised before settings has been changed, allowing listeners to cancel the change </summary>
        public event RepositoryEventHandler ValidateChanges;

        /// <summary> Raised after a setting has been changed </summary>
        public event RepositoryEventHandler SettingsChanged;

        /// <summary> Overridable event logic </summary>
        protected virtual void OnSettingsChanged(object sender, SettingsChangedEventArgs e)
        {
            SettingsChanged?.Invoke(sender, e);
        }

        /// <summary> Overridable event logic </summary>
        protected virtual bool OnValidateChanges(object sender, SettingsChangedEventArgs e)
        {
            ValidateChanges?.Invoke(sender, e);
            return !e.Cancel;
        }

        /// <summary>
        /// things to do whenever the settings are changed by using the dialog<br></br>
        /// The main thing to always do is save settings to the {PluginName}.ini file.<br></br>
        /// override this in your Settings.cs as desired,
        /// but make sure to call base.OnSettingsChanged() in the override.
        /// </summary>
        public void OnSettingsChanged()
        {
            SaveToIniFile();
        }

        /// <summary>
        /// Register the changed state of a plugin menu item. Overridable.
        /// </summary>
        /// <param name="itemToCheck">menu item that has changed state</param>
        public virtual void ChangeMenuItemState(FuncItem itemToCheck)
        {
            OnSettingsChanged();
            // (un)check the menu item
            Utils.CheckMenuItem(itemToCheck.CmdID, !Convert.ToBoolean(itemToCheck.Init2Check));
        }

        /// <summary>
        /// Opens a window that edits all settings
        /// </summary>
        public void ShowDialog(bool debug = false)
        {
            // We bind a copy of this object and only apply it after they click "Ok"
            var copy = (Settings)MemberwiseClone();
#if DEBUG
            // check the current settings
            var settings_sb = new System.Text.StringBuilder();
            foreach (System.Reflection.PropertyInfo p in GetType().GetProperties())
            {
                settings_sb.Append(p.ToString());
                settings_sb.Append($": {p.GetValue(this)}");
                settings_sb.Append(", ");
            }
            MessageBox.Show(settings_sb.ToString());
#endif
            var dialog = new Form()
            {
                Text = $"Settings - {_pluginName} plug-in",
                ClientSize = new Size(DEFAULT_WIDTH, DEFAULT_HEIGHT),
                MinimumSize = new Size(250, 250),
                ShowIcon = false,
                AutoScaleMode = AutoScaleMode.Font,
                AutoScaleDimensions = new SizeF(6F, 13F),
                ShowInTaskbar = false,
                StartPosition = FormStartPosition.CenterParent,
                Controls =
                {
                    new Button
                    {
                        Name = "Cancel",
                        Text = "&Cancel",
                        Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                        Size = new Size(75, 23),
                        Location = new Point(DEFAULT_WIDTH - 115, DEFAULT_HEIGHT - 36),
                        UseVisualStyleBackColor = true
                    },
                    new Button
                    {
                        Name = "Reset",
                        Text = "&Reset",
                        Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                        Size = new Size(75, 23),
                        Location = new Point(DEFAULT_WIDTH - 212, DEFAULT_HEIGHT - 36),
                        UseVisualStyleBackColor = true
                    },
                    new Button
                    {
                        Name = "Ok",
                        Text = "&Ok",
                        Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                        Size = new Size(75, 23),
                        Location = new Point(DEFAULT_WIDTH - 310, DEFAULT_HEIGHT - 36),
                        UseVisualStyleBackColor = true
                    },
                    new PropertyGrid
                    {
                        Name = "Grid",
                        Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                        Location = new Point(13, 13),
                        Size = new Size(DEFAULT_WIDTH - 13 - 13, DEFAULT_HEIGHT - 55),
                        AutoScaleMode = AutoScaleMode.Font,
                        AutoScaleDimensions = new SizeF(6F,13F),
                        SelectedObject = copy
                    },
                }
            };

            dialog.Controls["Cancel"].Click += (a, b) => dialog.Close();
            dialog.Controls["Ok"].Click += (a, b) =>
            {
                // change the settings to whatever the user selected
                var changesEventArgs = new SettingsChangedEventArgs((Settings)this, copy);
                if (!changesEventArgs.Changed.Any())
                {
                    dialog?.Close();
                    return;
                }
                foreach (var propertyInfo in GetType().GetProperties())
                {
                    var oldValue = propertyInfo.GetValue(this, null);
                    var newValue = propertyInfo.GetValue(copy, null);
                    if (oldValue != null && !oldValue.Equals(newValue))
                        propertyInfo.SetValue(this, newValue, null);
                }
                OnSettingsChanged();
                dialog.Close();
            };

            dialog.Controls["Reset"].Click += (a, b) =>
            {
                // reset the settings to defaults
                foreach (var propertyInfo in GetType().GetProperties())
                {
                    if (propertyInfo.GetCustomAttributes(typeof(DefaultValueAttribute), false).FirstOrDefault() is DefaultValueAttribute def)
                    {
                        propertyInfo.SetValue(this, def.Value, null);
                    }
                }

                OnSettingsChanged();
                dialog.Close();
            };

            dialog.ShowDialog();
        }
    }

    public class SettingsChangedEventArgs : CancelEventArgs
    {
        public HashSet<string> Changed { get; }
        public Settings OldSettings { get; }
        public Settings NewSettings { get; }

        public SettingsChangedEventArgs(Settings oldSettings, Settings newSettings)
        {
            OldSettings = oldSettings;
            NewSettings = newSettings;
            Changed = new HashSet<string>();
            foreach (var propertyInfo in typeof(Settings).GetProperties())
            {
                var oldValue = propertyInfo.GetValue(oldSettings, null);
                var newValue = propertyInfo.GetValue(newSettings, null);
                if (oldValue != null && !oldValue.Equals(newValue))
                {
                    Trace.TraceInformation($"Setting {propertyInfo.Name} has changed");
                    Changed.Add(propertyInfo.Name);
                }
            }
        }
    }
}
