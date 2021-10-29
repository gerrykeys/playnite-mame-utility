using Playnite.SDK;
using System.Threading;
using System.Windows.Forms;

namespace MAMEUtility.Services.UI
{
    class UIService
    {
        //////////////////////////////////////////////////////////////////
        public static GlobalProgressResult showProgress(string text, bool cancelable, bool indeterminate, System.Action<GlobalProgressActionArgs> f)
        {
            GlobalProgressOptions options = new GlobalProgressOptions(text, cancelable);
            options.IsIndeterminate = indeterminate;

            return MAMEUtilityPlugin.playniteAPI.Dialogs.ActivateGlobalProgress(f, options);
        }

        //////////////////////////////////////////////////////////////////
        public static void showMessage(string text)
        {
            MAMEUtilityPlugin.playniteAPI.Dialogs.ShowMessage(text);
        }

        //////////////////////////////////////////////////////////////////
        public static void showError(string title, string text)
        {
            MAMEUtilityPlugin.playniteAPI.Dialogs.ShowErrorMessage(text, title);
        }

        //////////////////////////////////////////////////////////////////
        public static DialogResult openAskDialog(string title, string text)
        {
            return MessageBox.Show(text, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }

        //////////////////////////////////////////////////////////////////
        public static string openFileDialogChooser(string extensionFiltersType)
        {
            return MAMEUtilityPlugin.playniteAPI.Dialogs.SelectFile(extensionFiltersType);
        }

        //////////////////////////////////////////////////////////////////
        public static string openDirectoryDialogChooser()
        {
            return MAMEUtilityPlugin.playniteAPI.Dialogs.SelectFolder();
        }
    }
}
