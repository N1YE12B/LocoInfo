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
using Z.Dapper.Plus;
using Microsoft.Office.Interop.Excel;

namespace LocoInfo
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow(List<Locomotive> loco)
        {
            InitializeComponent();
            lvEdging.ItemsSource =loco.ToList();


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
                //MessageBox.Show("ofd close");
                return;
            } 
            else
            {
                //MessageBox.Show("Файл выбран");

                try
                {
                    List<Locomotive> loco_list = new List<Locomotive>();
                    using (var stream = File.Open(ofd.FileName, FileMode.Open, FileAccess.Read))
                    {
                        using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                            });
                            tableCollection = result.Tables;

                            System.Data.DataTable dt = tableCollection[0];
                           
                            for (int i = 0; i < dt.Rows.Count; i++)
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
                                loco.MileagePerYear = Convert.ToInt32(dt.Rows[i]["Пробег в год, км"]);
                                
                                loco_list.Add(loco);
                            }
                        }
                    }

                    foreach(var lc in loco_list)
                    {
                        AppData.Context.Locomotive.Add(new Locomotive {
                            DirectorateNumber = lc.DirectorateNumber,
                            DepotDirectorate = lc.DepotDirectorate,
                            TypeOfTraction = lc.TypeOfTraction,
                            TypeOfMovement = lc.TypeOfMovement,
                            LocomotiveSeries = lc.LocomotiveSeries,
                            LocomotiveSectionNumber = lc.LocomotiveSectionNumber,
                            SectionNumber = lc.SectionNumber,
                            TurningDate = lc.TurningDate,
                            GroupOfReasons = lc.GroupOfReasons,
                            Reason = lc.Reason,
                            KP = lc.KP,
                            SeatNumber = lc.SeatNumber,
                            MonthOfTurning = lc.MonthOfTurning,
                            TheCompanyThatProducedTheTurning = lc.TheCompanyThatProducedTheTurning,
                            DirectorateOfTheCompanyThatProducedTheTurning = lc.DirectorateOfTheCompanyThatProducedTheTurning,
                            MileagePerYear = lc.MileagePerYear
                        });
                        AppData.Context.SaveChanges();
                        
                    }
                    
                    
                    MessageBox.Show("Импорт выполнен", "Завершение операции", MessageBoxButton.OK, MessageBoxImage.Information);
                    lvEdging.ItemsSource = Classes.AppData.Context.Locomotive.ToList();

                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message,"Message",MessageBoxButton.OK, MessageBoxImage.Error);
                }





            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.ValidateNames= true;
            //sfd.Filter = "Excel Workbook|*.xlsx";
            sfd.Filter = "Excell|*.xlsx;";
            if (sfd.ShowDialog() == true) 
            {
                Microsoft.Office.Interop.Excel.Application excl_app = new Microsoft.Office.Interop.Excel.Application();
                Workbook wb = excl_app.Workbooks.Add(XlSheetType.xlWorksheet);
                Worksheet ws = (Worksheet)excl_app.ActiveSheet;
                excl_app.Visible = false;
                ws.Cells[1, 1] = "Номер дирекции";
                ws.Cells[1, 2] = "Дирекция";
                ws.Cells[1, 3] = "Депо";
                ws.Cells[1, 4] = "Тип тяги";
                ws.Cells[1, 5] = "Вид движения";
                ws.Cells[1, 6] = "Серия локомотива";
                ws.Cells[1, 7] = "Номер секции локомотива";
                ws.Cells[1, 8] = "Дата обточки";
                ws.Cells[1, 9] = "Группа причин";
                ws.Cells[1, 10] = "Причина";
                ws.Cells[1, 11] = "КП";
                ws.Cells[1, 12] = "Номер места";
                ws.Cells[1, 13] = "Месяц обточки";
                ws.Cells[1, 14] = "Предприятие, производившее обточку";
                ws.Cells[1, 15] = "Дирекция предприятия, производившего обточку";
                ws.Cells[1, 16] = "Пробег в год, км";

                int i = 2;
                foreach(EF.Locomotive item in lvEdging.Items)
                {

                    ws.Cells[i, 1] = item.DirectorateNumber.ToString();
                    ws.Cells[i, 2] = item.DepotDirectorate.ToString();
                    ws.Cells[i, 3] = item.TypeOfTraction.ToString();
                    ws.Cells[i, 4] = item.TypeOfMovement.ToString();
                    ws.Cells[i, 5] = item.LocomotiveSeries.ToString();
                    ws.Cells[i, 6] = item.LocomotiveSectionNumber.ToString();
                    ws.Cells[i, 7] = item.SectionNumber.ToString();
                    ws.Cells[i, 8] = item.TurningDate.ToString();
                    ws.Cells[i, 9] = item.GroupOfReasons.ToString();
                    ws.Cells[i, 10] = item.Reason.ToString();
                    ws.Cells[i, 11] = item.KP.ToString();
                    ws.Cells[i, 12] = item.SeatNumber.ToString();
                    ws.Cells[i, 13] = item.MonthOfTurning.ToString();
                    ws.Cells[i, 14] = item.TheCompanyThatProducedTheTurning.ToString();
                    ws.Cells[i, 15] = item.DirectorateOfTheCompanyThatProducedTheTurning.ToString();
                    ws.Cells[i, 16] = item.MileagePerYear.ToString();
                    i++;
                }

                wb.SaveAs(sfd.FileName, XlFileFormat.xlWorkbookDefault,Type.Missing,Type.Missing,true,false,XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing);
                excl_app.Quit();
                MessageBox.Show("Экспорт завершён", "Завершение операции", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                return;
            }


        }

        private void btnGraph_Click(object sender, RoutedEventArgs e)
        {
            GraphWindow gw = new GraphWindow();
            gw.Show();
        }
    }
}
