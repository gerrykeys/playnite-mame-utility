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
            return this.plugin.GetSettings(false) as MAMEUtilitySettingsViewModel;
        }

        //////////////////////////////////////////////////////////
        private void Button_SelectMameExecutable(object sender, RoutedEventArgs e)
        {
            string mameExecutableFilePath = Services.UI.UIService.openFileDialogChooser("Executable (*.exe)|*.exe");
            if (!string.IsNullOrEmpty(mameExecutableFilePath))
            {
                MAMEUtilitySettingsViewModel settings = getSettings();
                settings.Settings.MameExecutableFilePath = mameExecutableFilePath;

            }
        }
    }
}