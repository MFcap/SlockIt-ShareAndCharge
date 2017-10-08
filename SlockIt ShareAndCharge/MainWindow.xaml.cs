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

namespace SlockIt_ShareAndCharge
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            System.Windows.Data.CollectionViewSource logServiceViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("logServiceViewSource")));
            // Laden Sie Daten durch Festlegen der CollectionViewSource.Source-Eigenschaft:
            logServiceViewSource.Source = LogService.Instance.Messages;
            messagesDataGrid.DataContext = logServiceViewSource;
            LogService.Instance.Log("Application startet...");
            LogService.Instance.OutputConsole = true;
            LogService.Instance.PropertyChanged += Instance_PropertyChanged;

            txtUpdWeb3.Text = RequestService.Instance.Web3URI;

            ListCollectionView lst = (messagesDataGrid.ItemsSource as ListCollectionView);
            lst.SortDescriptions.Clear();
            lst.SortDescriptions.Add(new System.ComponentModel.SortDescription("Date", System.ComponentModel.ListSortDirection.Descending));
        }

        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            System.Windows.Data.CollectionViewSource logServiceViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("logServiceViewSource")));
            try
            {
                this.Dispatcher.Invoke((Action)(() => {
                    logServiceViewSource.View.Refresh();
                }));                
            } catch (Exception ex)
            {
                LogService.Instance.Log("Fehler beim Udate der LogServiceViewSource. Komponent update: " + e.PropertyName + "   Error: " + ex.Message);
            }
        }


        private async void bttLibUpdates_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await RequestService.Instance.ReqLibUpdate();
            }
            catch (Exception ex)
            {
                LogService.Instance.Log("Fehler bei ReqLibUpdates." + ex.Message);
            }
        }

        private async void bttReqCurrentBlock_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string res = await RequestService.Instance.ReqCurrentBlockNumber();
                labMaxBlock.Content = res + "   " + DateTime.Now.ToString("(HH:mm:ss dd.MM.yyyy)");      
            } catch (Exception ex)
            {
                LogService.Instance.Log("Fehler bei ReqCurrentBlock." + ex.Message);
            }
        }

        private async void bttReqSyncingStatus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 await RequestService.Instance.ReqSyncingStatus();
            }
            catch (Exception ex)
            {
                LogService.Instance.Log("Fehler bei SyncingStatus." + ex.Message);
            }
            
        }

        private async void bttReqContract_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await RequestService.Instance.ReqContractTest();
            }
            catch (Exception ex)
            {
                LogService.Instance.Log("Fehler bei ContractTest." + ex.Message);
            }

        }








        private async void bttReqLibAddress_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await RequestService.Instance.ReqLibraries();
            }
            catch (Exception ex)
            {
                LogService.Instance.Log("Fehler bei ContractTest." + ex.Message);
            }

        }

        private async void bttReqLogRented_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await RequestService.Instance.ReqLogRented();
            }
            catch (Exception ex)
            {
                LogService.Instance.Log("Fehler bei SyncingStatus." + ex.Message);
            }

        }

        private async void bttReqLogReturned_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await RequestService.Instance.ReqLogReturned();
            }
            catch (Exception ex)
            {
                LogService.Instance.Log("Fehler bei ContractTest." + ex.Message);
            }

        }

        private void bttUpdWeb3_Click(object sender, RoutedEventArgs e)
        {
            RequestService.Instance.Web3URI = txtUpdWeb3.Text;
            txtUpdWeb3.Text = RequestService.Instance.Web3URI;
        }

        private void bttLibAddressWindows_Click(object sender, RoutedEventArgs e)
        {
            WinLibraries win = new WinLibraries();
            win.Show();
        }
    }
}
