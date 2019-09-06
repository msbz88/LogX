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
using System.Windows.Threading;
using LogX.Models;
using LogX.ViewModels;

namespace LogX.Views {
    /// <summary>
    /// Interaction logic for SearchBatchJobGroupView.xaml
    /// </summary>
    public partial class BatchJobGroupView : Page {
        BatchJobGropViewModel BJGViewModel { get; set; }
        public EventHandler ShowBLGLogs { get; set; }
        public EventHandler Error { get; set; }
        public EventHandler Message { get; set; }
        string prevVal = "";

        public BatchJobGroupView() {
            InitializeComponent();
            BJGViewModel = new BatchJobGropViewModel();
            BJGViewModel.LogsExtracted += OnLogsExtracted;
            BJGViewModel.Error += OnError;
            this.DataContext = BJGViewModel;
        }

       private void PreviewKeyDownHandler(object sender, KeyEventArgs e) {
            var grid = (DataGrid)sender;
            if (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control) {
                int index = grid.SelectedIndex;
                grid.ItemsSource = null;
                BJGViewModel.InsertFromClipBoard(index);
                grid.ItemsSource = BJGViewModel.BatchJobGroups;                         
            }
        }

        private void ResetGrid() {
            DataGridSearch.ItemsSource = null;
            DataGridSearch.ItemsSource = BJGViewModel.BatchJobGroups;
        }

        private void DataGridSearchCellEditEnding(object sender, DataGridCellEditEndingEventArgs e) {
            var textBox = e.EditingElement as TextBox;
            if (textBox != null && prevVal != textBox.Text.ToUpper()) {
                var text = textBox.Text.ToUpper();
                if (BJGViewModel.BatchJobGroups.Any(item => item.Name == text)) {
                    ResetGrid();
                    Error?.Invoke("Duplicate entry deleted.", null);
                } else if (text == "") {
                    ResetGrid();
                    Error?.Invoke("Empty values are not allowed.", null);
                }
            }
            if (Keyboard.IsKeyDown(Key.RightCtrl) && Keyboard.IsKeyDown(Key.Enter)) {
                BJGViewModel.GetFomDatabase();
            }
        }

        private void OnLogsExtracted(object sender, EventArgs e) {
            ShowBLGLogs?.Invoke(sender, null);
        }

        private void OnError(object sender, EventArgs e) {
            Error?.Invoke(sender, null);
        }

        private void ButtonLoadClick(object sender, RoutedEventArgs e) {
            BJGViewModel.GetFomDatabase();
        }

        private void ButtonViewLogClick(object sender, RoutedEventArgs e) {

        }

        private void ButtonSaveAsScopeClick(object sender, RoutedEventArgs e) {
            var errorMessage = BJGViewModel.SaveAsScope();
            if (errorMessage == "") {
                Message.Invoke("Saved to scope file.", null);
            }else {
                Error.Invoke(errorMessage, null);
            }
        }

        private void ButtonGetFromScopeClick(object sender, RoutedEventArgs e) {
            var errorMessage = BJGViewModel.LoadFromScope();
            if (errorMessage == "") {
                Message.Invoke("Loaded from scope file.", null);
            }else {
                Error.Invoke(errorMessage, null);
            }          
        }
        
        private void DataGridSearchBeginningEdit(object sender, DataGridBeginningEditEventArgs e) {
            var textBlock = e.EditingEventArgs.Source as TextBlock;
            if(textBlock != null) {
                prevVal = textBlock.Text;
            }else {
                prevVal = "";
            }
        }
    }
}
