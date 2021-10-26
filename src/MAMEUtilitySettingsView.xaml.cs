using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MAMEUtility
{
    public partial class MAMEUtilitySettingsView : UserControl
    {
        //////////////////////////////////////////////////////////
        private readonly MAMEUtilityPlugin plugin;

        //////////////////////////////////////////////////////////
        public MAMEUtilitySettingsView(MAMEUtilityPlugin plugin)
        {
            InitializeComponent();

            this.plugin = plugin;
        }

        ////////////////////////////////////////////////////////////////////
        public MAMEUtilitySettingsViewModel getSettings()
        {
            if (this.plugin == null) return null;

            return this.plugin.GetSettings(false) as MAMEUtilitySettingsViewModel;
        }

        //////////////////////////////////////////////////////////
        private void Button_SelectMameExecutable(object sender, RoutedEventArgs e)
        {
            if (this.plugin == null) return;

            string mameExecutableFilePath = Services.UI.UIService.openFileDialogChooser("Executable (*.exe)|*.exe");
            if (!string.IsNullOrEmpty(mameExecutableFilePath))
            {
                MAMEUtilitySettingsViewModel settings = getSettings();
                settings.Settings.MameExecutableFilePath = mameExecutableFilePath;

            }
        }

        private void Button_SelectMameGamelistXmlFilePath(object sender, RoutedEventArgs e)
        {
            if (this.plugin == null) return;
            
            string mameListFilePath = Services.UI.UIService.openFileDialogChooser("Gamelist XMl (*.xml)|*.xml");
            if (!string.IsNullOrEmpty(mameListFilePath))
            {
                MAMEUtilitySettingsViewModel settings = getSettings();
                settings.Settings.MameListFilePath = mameListFilePath;

            }
        }

        private void RadioButton_UseMameExecutable_Checked(object sender, RoutedEventArgs e)
        {
            if (this.plugin == null) return;
            
            getSettings().Settings.UseMameExecutable = true;
            getSettings().Settings.UseMameListFile    = false;
        }

        private void RadioButton_UseMameFile_Checked(object sender, RoutedEventArgs e)
        {
            if (this.plugin == null) return;
            
            getSettings().Settings.UseMameExecutable = false;
            getSettings().Settings.UseMameListFile    = true;
        }
    }
}