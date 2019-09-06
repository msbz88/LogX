using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using LogX.ViewModels;

namespace LogX.Views {
    /// <summary>
    /// Interaction logic for DBConnectionPage.xaml
    /// </summary>
    public partial class DBConnectionView : Page {
        public EventHandler DBConnected { get; set; }
        DBConnectionViewModel MasterDbViewModel { get; set; }
        DBConnectionViewModel TestDbViewModel { get; set; }

        public DBConnectionView() {
            InitializeComponent();
            MasterDbViewModel = new DBConnectionViewModel("Master");
            TestDbViewModel = new DBConnectionViewModel("Test");
        }

        private void ButtonConnectAndSave(object sender, RoutedEventArgs e) {
            if (CheckIfAnyTextBoxEmpty()) {
                WriteError("All available TextBoxes must be filled");
            } else {
                string errorMessage = "";
                errorMessage = MasterDbViewModel.StartSession(TextBoxMasterHost.Text, TextBoxMasterPort.Text, TextBoxMasterUserName.Text, TextBoxMasterPassword.Text, TextBoxMasterDBName.Text, TextBoxMasterSid.Text);
                if (errorMessage != "") {
                    WriteError(errorMessage);
                    return;
                }
                MasterDbViewModel.Save();
                TestDbViewModel.StartSession(TextBoxTestHost.Text, TextBoxTestPort.Text, TextBoxTestUserName.Text, TextBoxTestPassword.Text, TextBoxTestDBName.Text, TextBoxTestSid.Text);
                if (errorMessage != "") {
                    WriteError(errorMessage);
                    return;
                } else {
                    TestDbViewModel.Save();
                    DBConnected?.Invoke(null, null);
                }
            }
        }

        private void WriteError(string errorMessage) {
            LabelMessage.Foreground = new SolidColorBrush(Colors.Red);
            LabelMessage.Content = errorMessage;
        }

        private void TextBoxMasterPortPreviewTextInput(object sender, TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TextBoxTestDBNameLostFocus(object sender, RoutedEventArgs e) {
            if (TextBoxTestDBName.Text != "") {
                TextBoxTestSid.Visibility = Visibility.Collapsed;
            } else {
                TextBoxTestSid.Visibility = Visibility.Visible;
            }
        }

        private void TextBoxTestSidLostFocus(object sender, RoutedEventArgs e) {
            if (TextBoxTestSid.Text != "") {
                TextBoxTestDBName.Visibility = Visibility.Collapsed;
            } else {
                TextBoxTestDBName.Visibility = Visibility.Visible;
            }
        }

        private void TextBoxMasterDBNameLostFocus(object sender, RoutedEventArgs e) {
            if (TextBoxMasterDBName.Text != "") {
                TextBoxMasterSid.Visibility = Visibility.Collapsed;
            } else {
                TextBoxMasterSid.Visibility = Visibility.Visible;
            }
        }

        private void TextBoxMasterSidLostFocus(object sender, RoutedEventArgs e) {
            if (TextBoxMasterSid.Text != "") {
                TextBoxMasterDBName.Visibility = Visibility.Collapsed;
            } else {
                TextBoxMasterDBName.Visibility = Visibility.Visible;
            }
        }

        private bool CheckIfAnyTextBoxEmpty() {
            foreach (var textBox in FindVisualChildren<TextBox>(this)) {
                if (textBox.Visibility == Visibility.Visible && textBox.Text == "") {
                    return true;
                }
            }
            return false;
        }

        private IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject {
            if (depObj != null) {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++) {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T) {
                        yield return (T)child;
                    }
                    foreach (T childOfChild in FindVisualChildren<T>(child)) {
                        yield return childOfChild;
                    }
                }
            }
        }

        public bool IsConnectionDataNeeded() {
            if (MasterDbViewModel.IsConnectionFileExists() && TestDbViewModel.IsConnectionFileExists()) {
                return false;
            }else {
                return true;
            }
        }


    }
}
