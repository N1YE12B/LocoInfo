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

using LiveCharts;
using LiveCharts.Configurations;

using LiveCharts.Helpers;

using LocoInfo.EF;
using LocoInfo.Classes;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Reflection;
using LiveCharts.Maps;

namespace LocoInfo.Pages
{
    /// <summary>
    /// Логика взаимодействия для TurningReasonPage.xaml
    /// </summary>
    public partial class TurningReasonPage : Page
    {
        private List<Locomotive> series = new List<Locomotive>();
        private List<Locomotive> slds = new List<Locomotive>();
        
        public object Mapper { get; set; }

        public ChartValues<Locomotive> Results { get; set; }
        public ObservableCollection<string> Labels { get; set; }
       


        public TurningReasonPage()
        {
            InitializeComponent();
            series = Classes.AppData.Context.Locomotive.Distinct().ToList();
            string[] mass_series = series.AsEnumerable().Select(s => s.LocomotiveSectionNumber).Distinct().OrderBy(x => x).ToArray<string>();
            cmbSeries.Items.Insert(0, "--Весь парк--");
            for (int i = 1; i <= mass_series.Length; i++)
            {
                cmbSeries.Items.Insert(i, mass_series[i - 1]);
            }
            cmbSeries.SelectedIndex = 0;

            slds = Classes.AppData.Context.Locomotive.Distinct().ToList();
            string[] mass_slds = slds.AsEnumerable().Select(s => s.TheCompanyThatProducedTheTurning).Distinct().OrderBy(x => x).ToArray<string>();
            cmbSLD.Items.Insert(0, "--Все СЛД--");
            for (int i = 1; i <= mass_slds.Length; i++)
            {
                cmbSLD.Items.Insert(i, mass_slds[i - 1]);
            }
            cmbSLD.SelectedIndex = 0;






            //Настройка графика
          
            Mapper = Mappers.Xy<Locomotive>().X(((Locomotive, index) => index)).Y(Locomotive => Locomotive.Reason.Distinct().Count());

            var records = AppData.Context.Locomotive.ToArray().Take(56);
            Results = records.AsChartValues();      
            //Достаем причины обточки     
            Labels = new ObservableCollection<string>(AppData.Context.Locomotive.AsEnumerable().Select(s => s.Reason).Distinct().OrderBy(x => x).ToArray<string>());
            DataContext = this;


            //AppData.Context.Locomotive.AsEnumerable().Select(s => s.Reason).Distinct().OrderBy(x => x).ToArray<string>());
        }

        private void cmbSeries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbSLD_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
