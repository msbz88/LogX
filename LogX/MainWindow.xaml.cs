using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using LogX.Models;
using LogX.Views;

namespace LogX {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        BatchJobGroupView BatchJobGroupView { get; set; }
        DBConnectionView DBConnectionView { get; set; }
        NavigationView NavigationView { get; set; }
        ReportsView ReportsView { get; set; }
        DashboardsView DashboardsView { get; set; }
        System.Windows.Threading.DispatcherTimer DispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        public MainWindow() {
            InitializeComponent();
            DBConnectionView = new DBConnectionView();
            DBConnectionView.DBConnected += OnDBConnected;
            RenderViews();
        }

        private void OnDBConnected(object sender, EventArgs e) {
            RenderViews();
            WriteMessage("Connected to databases.", new SolidColorBrush(Colors.Green));
        }

        private void OnError(object sender, EventArgs e) {
            var message = sender.ToString();
            WriteMessage(message, new SolidColorBrush(Colors.Red));
        }

        private void OnMessage(object sender, EventArgs e) {
            var message = sender.ToString();
            WriteMessage(message, new SolidColorBrush(Colors.Green));
        }

        private void CreateViews() {
            BatchJobGroupView = new BatchJobGroupView();
            BatchJobGroupView.Error += OnError;
            BatchJobGroupView.Message += OnMessage;
            NavigationView = new NavigationView();
            NavigationView.ShowBJGView += OnShowBJGView;
            NavigationView.ShowReportsView += OnReportsView;
            NavigationView.ShowDashboardsView += OnDashboardsView;
        }

        private void OnShowBJGView(object sender, EventArgs e) {
            WorkArea.Content = BatchJobGroupView;
        }

        private void OnReportsView(object sender, EventArgs e) {
            if(ReportsView == null) {
                ReportsView = new ReportsView();
            }
            WorkArea.Content = ReportsView;           
        }

        private void OnDashboardsView(object sender, EventArgs e) {
            if(DashboardsView == null) {
                DashboardsView = new DashboardsView();
            }
            WorkArea.Content = DashboardsView;
        }

        private void RenderViews() {
            if (DBConnectionView.IsConnectionDataNeeded()) {
                AllArea.Content = DBConnectionView;
            } else {
                CreateViews();
                NavArea.Content = NavigationView;
                WorkArea.Content = BatchJobGroupView;
            }
        }

        private void StartClearTimer() {
            DispatcherTimer.Tick += new EventHandler(DispatcherTimerTick);
            DispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            DispatcherTimer.Start();
        }

        private void WriteMessage(string message, SolidColorBrush color) {
            StatusBarContent.Text = message;
            StatusBarContent.Foreground = color;
            StartClearTimer();
        }

        private void DispatcherTimerTick(object sender, EventArgs e) {
            StatusBarContent.Text = "";
            DispatcherTimer.Stop();
        }

    }
}
