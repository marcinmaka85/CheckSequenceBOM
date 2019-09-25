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
using System.Windows.Shapes;

namespace CheckSequenceBOM
{
    /// <summary>
    /// Interaction logic for WindowFilters.xaml
    /// </summary>
    public partial class WindowFilters : Window
    {
        public WindowFilters()
        {
            InitializeComponent();
            CreateControlList();
        }

        public bool SequenceStatus10;
        public bool SequenceStatus19;
        public bool SequenceStatus20;
        public bool SequenceStatus21;
        public bool SequenceStatus30;
        public bool SequenceStatusOther;
        public bool SequenceStatusAll;

        private List<Control> checkboxlist= new List<Control>();


        private void CreateControlList()
        {
            checkboxlist.Add(CheckboxStatus10);
            checkboxlist.Add(CheckboxStatus19);
            checkboxlist.Add(CheckboxStatus20);
            checkboxlist.Add(CheckboxStatus21);
            checkboxlist.Add(CheckboxStatusOther);
        }

        private void Window_Activated(object sender, EventArgs e)
        {

        }

        private void RackList_Loaded(object sender, RoutedEventArgs e)
        {
            DbOperations ProdTracing = new DbOperations();
            ProdTracing.openConnection();
            RackList.ItemsSource = ProdTracing.getSequnecesRacks();
            ProdTracing.closeConnection();
        }

        private void CheckboxAllRacks_Checked(object sender, RoutedEventArgs e)
        {
            RackList.SelectAll();
        }

        private void CheckboxAllRacks_Unchecked(object sender, RoutedEventArgs e)
        {
            RackList.UnselectAll();
        }

        private void CheckboxStatusAll_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var ctrl in checkboxlist)
            {
                if (ctrl.GetType() == typeof(CheckBox))
                {
                    ((CheckBox)ctrl).IsChecked = true;
                }
            }
        }

        private void CheckboxStatusAll_Unchecked(object sender, RoutedEventArgs e)
        {
            if (CheckboxStatusAll.IsFocused)
            {
                foreach (var ctrl in checkboxlist)
                {
                    if (ctrl.GetType() == typeof(CheckBox))
                    {
                        ((CheckBox)ctrl).IsChecked = false;
                    }
                }
            }



        }

        private void CheckboxList_OneUnchecked(object sender, RoutedEventArgs e)
        {

            CheckboxStatusAll.IsChecked = false;
         
        }
    }
}
