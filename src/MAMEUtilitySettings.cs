using Playnite.SDK;
using Playnite.SDK.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MAMEUtility
{
    public class MAMEUtilitySettings : ObservableObject
    {
        ////////////////////////////////
        public MAMEUtilitySettings()
        {
            RomsetSourceFormat = new[] { "MAME", "FBNeo" };
        }

        //////////////////////////////////////////
        //// Use MAME Source executable path
        //////////////////////////////////////////
        private bool _useMameExecutablePath = true;
        public bool UseMameExecutable
        {
            get => _useMameExecutablePath;
            set
            {
                _useMameExecutablePath = value;
                OnPropertyChanged();
            }
        }

        //////////////////////////////////////////
        //// MAME Source executable path
        //////////////////////////////////////////
        private string _mameExecutableFilePath;
        public string MameExecutableFilePath
        {
            get => _mameExecutableFilePath;
            set
            {
                _mameExecutableFilePath = value;
                OnPropertyChanged();
            }
        }

        //////////////////////////////////////////
        //// USE XML/DAT Source file
        //////////////////////////////////////////
        private bool _useSourcelistFilePath = false;
        public bool UseSourceListFile
        {
            get => _useSourcelistFilePath;
            set
            {
                _useSourcelistFilePath = value;
                OnPropertyChanged();
            }
        }

        //////////////////////////////////////////
        //// Romset Source Format
        //////////////////////////////////////////
        public IEnumerable<string> RomsetSourceFormat { get; private set; }
        private string _selectedRomsetSourceFormat;
        public string SelectedRomsetSourceFormat
        {
            get => !string.IsNullOrEmpty(_selectedRomsetSourceFormat) ? _selectedRomsetSourceFormat : "MAME";
            set
            {
                _selectedRomsetSourceFormat = value;
                OnPropertyChanged();
            }
        }

        //////////////////////////////////////////
        //// Source list file path
        //////////////////////////////////////////
        private string _sourceListFilePath;
        public string SourceListFilePath
        {
            get => _sourceListFilePath;
            set
            {
                _sourceListFilePath = value;
                OnPropertyChanged();
            }
        }


        // Playnite serializes settings object to a JSON object and saves it as text file.
        // If you want to exclude some property from being saved then use `JsonDontSerialize` ignore attribute.
        //[DontSerialize]
        //public bool OptionThatWontBeSaved { get => optionThatWontBeSaved; set => SetValue(ref optionThatWontBeSaved, value); }
    }

    public class MAMEUtilitySettingsViewModel : ObservableObject, ISettings
    {
        private readonly MAMEUtilityPlugin plugin;
        private MAMEUtilitySettings editingClone { get; set; }

        private MAMEUtilitySettings settings;
        public MAMEUtilitySettings Settings
        {
            get => settings;
            set
            {
                settings = value;
                OnPropertyChanged();
            }
        }

        public MAMEUtilitySettingsViewModel(MAMEUtilityPlugin plugin)
        {
            // Injecting your plugin instance is required for Save/Load method because Playnite saves data to a location based on what plugin requested the operation.
            this.plugin = plugin;

            // Load saved settings.
            var savedSettings = plugin.LoadPluginSettings<MAMEUtilitySettings>();

            // LoadPluginSettings returns null if not saved data is available.
            if (savedSettings != null)
            {
                Settings = savedSettings;
            }
            else
            {
                Settings = new MAMEUtilitySettings();
            }
        }

        public void BeginEdit()
        {
            // Code executed when settings view is opened and user starts editing values.
            editingClone = Serialization.GetClone(Settings);
        }

        public void CancelEdit()
        {
            // Code executed when user decides to cancel any changes made since BeginEdit was called.
            // This method should revert any changes made to Option1 and Option2.
            Settings = editingClone;
        }

        public void EndEdit()
        {
            // Code executed when user decides to confirm changes made since BeginEdit was called.
            // This method should save settings made to Option1 and Option2.
            plugin.SavePluginSettings(Settings);
        }

        public bool VerifySettings(out List<string> errors)
        {
            // Code execute when user decides to confirm changes made since BeginEdit was called.
            // Executed before EndEdit is called and EndEdit is not called if false is returned.
            // List of errors is presented to user if verification fails.
            errors = new List<string>();
            return true;
        }
    }
}