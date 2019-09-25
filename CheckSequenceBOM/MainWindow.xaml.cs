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


namespace CheckSequenceBOM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private DbOperations ProdTracing = new DbOperations();
 
        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            this.ProdTracing.closeConnection();
            this.Close();

        }

        private void MenuConnect_Click(object sender, RoutedEventArgs e)
        {
            this.ProdTracing.openConnection();
            this.RefreshStatus();
        }

        private void MenuDisconnect_Click(object sender, RoutedEventArgs e)
        {
            this.ProdTracing.closeConnection();
            this.RefreshStatus();
        }



        private void RefreshStatus()
        {

            // InfoText3.Text = System.DateTime.Now.ToString("yyyy.MM.dd");
                       
            if (ProdTracing.ConnectionState() == "OK")
            {
                ConnectionInfoText.Text = "connected";
                ConnectionInfoColor.Background = Brushes.Green;
                MenuConnect.IsEnabled = false;
                MenuDisconnect.IsEnabled = true;
                UserPcInfo.Text = "User: " + System.Environment.UserName + " // PC: " + System.Environment.MachineName + " // ATraQ line: " + ProdTracing.getATraQLineID();
                MainContent.IsEnabled = true;
                MenuFilters.IsEnabled = true;

            }
            else
            {

                ConnectionInfoText.Text = "not connected";
                ConnectionInfoColor.Background = Brushes.Red;
                MenuConnect.IsEnabled = true;
                MenuDisconnect.IsEnabled = false;
                MainContent.IsEnabled = false;
                UserPcInfo.Text = "User: " + System.Environment.UserName + " // PC: " + System.Environment.MachineName + " // ATraQ line: xxxx";
                MenuFilters.IsEnabled = false;
               // Application.Current.Windows.OfType<WindowFilters>().First().Close();  // not needed anymore :)
            }                
        }

        private void MenuAbout_Click(object sender, RoutedEventArgs e)
        {

            string aboutText = "Author: Marcin Maka \r\n marcin.maka@autoliv.int \r\n +48 783 440 550";
            string aboutHeader = "Version 0.1 ... probably :)";
            MessageBox.Show(aboutText, aboutHeader);
            
        }

               
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string ExitInfo = "Do you really want to exit?";
            MessageBoxResult result;
            result = MessageBox.Show(ExitInfo, "", MessageBoxButton.YesNo);

            if (result==MessageBoxResult.No)
            {
                e.Cancel = true;
            }           
        }

        private void FamilyList_DropDownOpened(object sender, EventArgs e)
        {
            FamilyList.ItemsSource = ProdTracing.getSequenceFamilies();
        }

        private void SequenceList_DropDownOpened(object sender, EventArgs e)
        {
            SequenceList.ItemsSource = ProdTracing.getSequencesToBuild();
            
        }

        public void MenuFilters_Click(object sender, RoutedEventArgs e)
        {
            if (!Application.Current.Windows.OfType<WindowFilters>().Any()) // Check if Filters window is not opened, Open it when needed.
            {
                var Filters = new WindowFilters();
                Filters.ShowInTaskbar = false;
                Filters.Owner = Application.Current.MainWindow;
                Filters.Show();
            }
            else Application.Current.Windows.OfType<WindowFilters>().First().Activate();
                        
        }

        private void FamilyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {                 
            FamiliyBOM.ItemsSource = ProdTracing.getBillOfMaterial(FamilyList.SelectedValue.ToString());
        }

        private void SequenceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AMOTEQBOM.ItemsSource = ProdTracing.getSeqBillOfMaterial(SequenceList.SelectedValue.ToString().Substring(0,12));
        }
    }
}
