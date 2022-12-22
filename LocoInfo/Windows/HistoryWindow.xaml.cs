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
using System.Windows.Shapes;

namespace LocoInfo.Windows
{
    /// <summary>
    /// Логика взаимодействия для HistoryWindow.xaml
    /// </summary>
    public partial class HistoryWindow : Window
    {
        public HistoryWindow(EF.Locomotive loco)
        {
            InitializeComponent();
            lvHistory.ItemsSource = Classes.AppData.Context.Locomotive.Where(x=> x.SectionNumber == loco.SectionNumber).ToList();
            locoLabel.Text = "Локомотив " + loco.SectionNumber;
        }

       

        private void lvHistory_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvHistory.SelectedItem is EF.Locomotive)
            {
                var pair = lvHistory.SelectedItem as EF.Locomotive;
                lvPair.ItemsSource = Classes.AppData.Context.Locomotive.Where(x=> x.KP == pair.KP).ToList();
                pairLabel.Text = pair.KP.ToString();

            }
        }

        //SORT
        private ListSortDirection _sortDirection;
        private GridViewColumnHeader _sortColumn;
        private void lvHistory_Click(object sender, RoutedEventArgs e)
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

        private void btnPair_Click(object sender, RoutedEventArgs e)
        {
            if (lvHistory.SelectedItem is EF.Locomotive)
            {
                var pair = lvHistory.SelectedItem as EF.Locomotive;
                lvPair.ItemsSource = Classes.AppData.Context.Locomotive.Where(x => x.KP == pair.KP).ToList();
                pairLabel.Text = pair.KP.ToString();

            }
            else
            {
                MessageBox.Show("Выберите запись", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
