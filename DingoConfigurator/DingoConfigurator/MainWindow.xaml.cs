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

namespace DingoConfigurator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly MainViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();
            _vm = new MainViewModel();
            DataContext = _vm;

            Closing += MainWindow_Closing;

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
                _vm.OpenFile(args[1]);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _vm.WindowClosing();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _vm.TreeView_SelectionChanged(sender, e);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _vm.Cans_SelectionChanged(sender, e);
        }

        private void ToggleKeypad1_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is CanDevices.DingoPdm.DingoPdmCan pdm)
            {
                if (pdm.Keypads.Count > 0)
                {
                    pdm.Keypads[0].Visible = menuItem.IsChecked;
                }
            }
        }

        private void ToggleKeypad2_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is CanDevices.DingoPdm.DingoPdmCan pdm)
            {
                if (pdm.Keypads.Count > 1)
                {
                    pdm.Keypads[1].Visible = menuItem.IsChecked;
                }
            }
        }
    }
}
