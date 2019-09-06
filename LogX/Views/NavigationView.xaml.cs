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

namespace LogX.Views {
    /// <summary>
    /// Interaction logic for NavigationView.xaml
    /// </summary>
    public partial class NavigationView : Page {
        public EventHandler ShowBJGView { get; set; }
        public EventHandler ShowReportsView { get; set; }
        public EventHandler ShowDashboardsView { get; set; }

        public NavigationView() {
            InitializeComponent();
        }

        private void ButtonBJGClick(object sender, RoutedEventArgs e) {
            ShowBJGView?.Invoke(null, null);
        }

        private void ButtonReportsClick(object sender, RoutedEventArgs e) {
            ShowReportsView?.Invoke(null, null);
        }

        private void ButtonDashboardsClick(object sender, RoutedEventArgs e) {
            ShowDashboardsView?.Invoke(null, null);
        }

        private void ChangeButtonColor(Button button, SolidColorBrush color) {
            Style editingStyle = new Style(typeof(Button));
            editingStyle.BasedOn = (Style)FindResource("NavButtton");
            Rectangle body = FindVisualChild<Rectangle>(button);
            body.Fill = color;
            button.Style = editingStyle;
        }

        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++) {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

    }
}
