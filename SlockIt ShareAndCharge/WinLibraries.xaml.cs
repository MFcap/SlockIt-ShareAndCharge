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

namespace SlockIt_ShareAndCharge
{
    /// <summary>
    /// Interaktionslogik für WinLibraries.xaml
    /// </summary>
    public partial class WinLibraries : Window
    {
        public WinLibraries()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            System.Windows.Data.CollectionViewSource libManagerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("libManagerViewSource")));
            // Laden Sie Daten durch Festlegen der CollectionViewSource.Source-Eigenschaft:
            List<LibManager> lst = new List<LibManager>(){ ContractFactory.Instance.LibManager };
            libManagerViewSource.Source = lst;
        }

        private void bttOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
