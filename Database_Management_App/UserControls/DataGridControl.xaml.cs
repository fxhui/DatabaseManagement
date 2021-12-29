using Database_Management_App.ViewModels;
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

namespace Database_Management_App.UserControls
{
    /// <summary>
    /// DataGridControl.xaml 的交互逻辑
    /// </summary>
    public partial class DataGridControl : UserControl
    {
        public DataGridControl()
        {
            InitializeComponent();
            dataGrid.Visibility = Visibility.Collapsed;
            dataGrid.ColumnWidth = DataGridLength.Auto;
        }


        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(DataGridControl), new PropertyMetadata(-1, SelectedIndexChangedCallback));

        private static void SelectedIndexChangedCallback(
           DependencyObject dependencyObject,
           DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = dependencyObject as DataGridControl;
            var index = Convert.ToInt32(dependencyPropertyChangedEventArgs.NewValue);

            var model = control.DataContext as DataGridViewModel;

            control.dataGrid.Visibility = model.TabIndex == index ? Visibility.Visible : Visibility.Collapsed;

        }

        private void dataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
    }
}
