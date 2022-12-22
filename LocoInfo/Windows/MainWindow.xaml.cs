using LocoInfo.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using LocoInfo.Classes;
using LocoInfo.EF;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.Win32;
using ExcelDataReader;

namespace LocoInfo
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(Locomotive loco)
        {
            InitializeComponent();
            lvEdging.ItemsSource = Classes.AppData.Context.Locomotive.ToList();


        }
        public MainWindow()
        {
            InitializeComponent();
            lvEdging.ItemsSource = Classes.AppData.Context.Locomotive.ToList();

            
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            if (lvEdging.SelectedItem is EF.Locomotive)
            {
                var loco = lvEdging.SelectedItem as EF.Locomotive;
                HistoryWindow hw = new HistoryWindow(loco);
                hw.Show();
                
            }
            else
            {
                MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchWindow sw = new SearchWindow();
            sw.ShowDialog();
            this.Close();
        }






        //SORT
        private ListSortDirection _sortDirection;
        private GridViewColumnHeader _sortColumn;
        private void lvEdging_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = e.OriginalSource as GridViewColumnHeader;
            if (column == null || column.Column == null)
            {
                return;
            }

            if (_sortColumn == column)
            {
                // Toggle sorting direction 
                _sortDirection = _sortDirection == ListSortDirection.Ascending ?
                                                   ListSortDirection.Descending :
                                                   ListSortDirection.Ascending;
            }
            else
            {
                // Remove arrow from previously sorted header 
                if (_sortColumn != null && _sortColumn.Column != null)
                {
                    _sortColumn.Column.HeaderTemplate = null;
                    _sortColumn.Column.Width = _sortColumn.ActualWidth - 20;
                }

                _sortColumn = column;
                _sortDirection = ListSortDirection.Ascending;
                column.Column.Width = column.ActualWidth + 20;
            }

            if (_sortDirection == ListSortDirection.Ascending)
            {
                column.Column.HeaderTemplate = Resources["ArrowUp"] as DataTemplate;
            }
            else
            {
                column.Column.HeaderTemplate = Resources["ArrowDown"] as DataTemplate;
            }

            string header = string.Empty;

            // if binding is used and property name doesn't match header content 
            Binding b = _sortColumn.Column.DisplayMemberBinding as Binding;
            if (b != null)
            {
                header = b.Path.Path;
            }

            ICollectionView resultDataView = CollectionViewSource.GetDefaultView((sender as ListView).ItemsSource);
            resultDataView.SortDescriptions.Clear();
            resultDataView.SortDescriptions.Add(new SortDescription(header, _sortDirection));
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            lvEdging.ItemsSource = Classes.AppData.Context.Locomotive.ToList();
        }

        private void lvEdging_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvEdging.SelectedItem is EF.Locomotive)
            {
                var loco = lvEdging.SelectedItem as EF.Locomotive;
                HistoryWindow hw = new HistoryWindow(loco);
                hw.Show();

            }
        }
        DataTableCollection tableCollection;
        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excell|*.xls;*.xlsx;";
            //ofd.ShowDialog();
            if (ofd.ShowDialog() != true)
            {
                MessageBox.Show("ofd close");
                return;
            } 
            else
            {
                MessageBox.Show("Файл выбран");
                using(var stream = File.Open(ofd.FileName, FileMode.Open, FileAccess.Read))
                {
                    using(IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                        });
                        tableCollection = result.Tables;

                        DataTable dt = tableCollection[0];
                        List<Locomotive> loco_list = new List<Locomotive>();
                        for (int i = 0 ; i< dt.Rows.Count;i++)
                        {
                            Locomotive loco = new Locomotive();
                            loco.DirectorateNumber = Convert.ToInt32(dt.Rows[i]["Номер дирекции"]);
                            loco.DepotDirectorate = dt.Rows[i]["Дирекция"].ToString();
                            loco.TypeOfTraction = dt.Rows[i]["Депо"].ToString();
                            loco.TypeOfMovement = dt.Rows[i]["Тип тяги"].ToString();
                            loco.LocomotiveSeries = dt.Rows[i]["Вид движения"].ToString();
                            loco.LocomotiveSectionNumber = dt.Rows[i]["Серия локомотива"].ToString();
                            loco.SectionNumber = dt.Rows[i]["Номер секции локомотива"].ToString();
                            loco.TurningDate = Convert.ToDateTime(dt.Rows[i]["Дата обточки"].ToString());
                            loco.GroupOfReasons = dt.Rows[i]["Группа причин"].ToString();
                            loco.Reason = dt.Rows[i]["Причина"].ToString();
                            loco.KP = dt.Rows[i]["КП"].ToString();
                            loco.SeatNumber = Convert.ToInt32(dt.Rows[i]["Номер места"]);
                            loco.MonthOfTurning = Convert.ToInt32(dt.Rows[i]["Месяц обточки"]);
                            loco.TheCompanyThatProducedTheTurning = dt.Rows[i]["Предприятие, производившее обточку"].ToString();
                            loco.DirectorateOfTheCompanyThatProducedTheTurning = dt.Rows[i]["Дирекция предприятия, производившего обточку"].ToString();

                        }

                    }
                }
            }
        }
    }
}
