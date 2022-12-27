using LocoInfo.Classes;
using LocoInfo.EF;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Логика взаимодействия для SearchWindow.xaml
    /// </summary>
    public partial class SearchWindow : Window
    {
        private List<Locomotive> locos = new List<Locomotive>();
        private List<Locomotive> regions = new List<Locomotive>();
        private List<Locomotive> slds = new List<Locomotive>();
        private List<Locomotive> directions = new List<Locomotive>();
        public SearchWindow()
        {
            InitializeComponent();
            locos = Classes.AppData.Context.Locomotive.Distinct().ToList();          
            string[] mass_locos = locos.AsEnumerable().Select(s => s.SectionNumber).Distinct().OrderBy(x=>x).ToArray<string>();
            cmbLoc.Items.Insert(0, "--Все локомотивы--");
            for (int i = 1; i <= mass_locos.Length; i++)
            {
                cmbLoc.Items.Insert(i, mass_locos[i - 1]);
            }

            regions = Classes.AppData.Context.Locomotive.Distinct().ToList();
            string[] regs = regions.AsEnumerable().Select(s => s.DepotDirectorate).Distinct().OrderBy(x => x).ToArray<string>();
            cmbRegion.Items.Insert(0, "--Все регионы--");
            for (int i = 1; i <= regs.Length; i++)
            {
                cmbRegion.Items.Insert(i, regs[i - 1]);
            }

            slds = Classes.AppData.Context.Locomotive.Distinct().ToList();
            string[] mass_slds = slds.AsEnumerable().Select(s => s.TheCompanyThatProducedTheTurning).Distinct().OrderBy(x => x).ToArray<string>();
            cmbSLD.Items.Insert(0, "--Все СЛД--");
            for (int i = 1; i <= mass_slds.Length; i++)
            {
                cmbSLD.Items.Insert(i, mass_slds[i - 1]);
            }

           

        }
        List<Locomotive> quer = new List<Locomotive>();
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            //Check for selection
            if(cmbData.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите данные", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if(cmbLoc.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите локомотив", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (cmbPeriod.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите дату", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (cmbRegion.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите регион", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (cmbSLD.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите СЛД", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if(cmbPeriod.SelectedIndex == 1 && dp1.SelectedDate == null && dp2.SelectedDate == null
                ||  cmbPeriod.SelectedIndex == 1 && dp1.SelectedDate == null || cmbPeriod.SelectedIndex == 1 && dp2.SelectedDate == null)
            {
                MessageBox.Show("Выберите промежуток времени", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            //Dates check
            if(dp1.SelectedDate > dp2.SelectedDate)
            {
                MessageBox.Show("Проверьте корректность промежутка", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if(cmbData.SelectedIndex == 0 && cmbLoc.SelectedIndex == 0 && cmbPeriod.SelectedIndex == 0 && cmbRegion.SelectedIndex == 0 && cmbSLD.SelectedIndex == 0)
            {
                MainWindow mw = new MainWindow(quer = AppData.Context.Locomotive.ToList());
                mw.Show();
                this.Close();
            }
            int data_ind = cmbData.SelectedIndex;
            int loc_ind = cmbLoc.SelectedIndex;
            int per_ind = cmbPeriod.SelectedIndex;
            int reg_ind = cmbRegion.SelectedIndex;
            int sld_ind = cmbSLD.SelectedIndex;


            if (data_ind != -1 && loc_ind != 0 && per_ind == 0 && reg_ind == 0 && sld_ind == 0)
            {
                MainWindow mw = new MainWindow(quer = AppData.Context.Locomotive
                    .Where(x => x.SectionNumber.Equals(cmbLoc.Text)).ToList()); //поиск строк только по локомотиву
                mw.Show();
                this.Close();
            }
            if (data_ind != -1 && loc_ind != 0 && per_ind != 0 && reg_ind == 0 && sld_ind == 0)
            {
                DateTime d1 = dp1.DisplayDate;
                DateTime d2 = dp2.DisplayDate;
                MainWindow mw = new MainWindow(quer = AppData.Context.Locomotive
                    .Where(x => x.SectionNumber.Equals(cmbLoc.Text) && x.TurningDate >= d1 && x.TurningDate <= d2).ToList()); //поиск строк по локомотиву и периоду
                mw.Show();
                this.Close();
            }

            if (data_ind != -1 && loc_ind != 0 && per_ind != 0 && reg_ind != 0 && sld_ind == 0)
            {
                DateTime d1 = dp1.DisplayDate;
                DateTime d2 = dp2.DisplayDate;
                MainWindow mw = new MainWindow(quer = AppData.Context.Locomotive
                    .Where(x => x.SectionNumber.Equals(cmbLoc.Text) && x.DepotDirectorate.Equals(cmbRegion.Text)
                    && x.TurningDate >= d1 && x.TurningDate <= d2).ToList()); //поиск строк по локомотиву, периоду, дирекции
                mw.Show();
                this.Close();
            }

            if (data_ind != -1 && loc_ind != 0 && per_ind != 0 && reg_ind != 0 && sld_ind != 0)
            {
                DateTime d1 = dp1.DisplayDate;
                DateTime d2 = dp2.DisplayDate;
                MainWindow mw = new MainWindow(quer = AppData.Context.Locomotive
                    .Where(x => x.SectionNumber.Equals(cmbLoc.Text) && x.DepotDirectorate.Equals(cmbRegion.Text)
                    && x.TurningDate >= d1 && x.TurningDate <= d2 && x.TheCompanyThatProducedTheTurning.Equals(cmbSLD.Text)).ToList()); //поиск строк по всему, кроме data
                mw.Show();
                this.Close();
            }



            if (data_ind != -1 && loc_ind != 0 && per_ind == 0 && reg_ind == 0 && sld_ind != 0)
            {
                MainWindow mw = new MainWindow(quer = AppData.Context.Locomotive
                    .Where(x => x.SectionNumber.Equals(cmbLoc.Text) && x.TheCompanyThatProducedTheTurning.Equals(cmbSLD.Text)).ToList()); 
                //поиск строк по локомотиву и СЛД
                mw.Show();
                this.Close();
            }
            if (data_ind != -1 && loc_ind != 0 && per_ind == 0 && reg_ind != 0 && sld_ind == 0)
            {
                MainWindow mw = new MainWindow(quer = AppData.Context.Locomotive
                    .Where(x => x.SectionNumber.Equals(cmbLoc.Text) && x.DepotDirectorate.Equals(cmbRegion.Text)).ToList());
                //поиск строк по локомотиву и дирекции
                mw.Show();
                this.Close();
            }
            if (data_ind != -1 && loc_ind != 0 && per_ind == 0 && reg_ind != 0 && sld_ind != 0)
            {
                MainWindow mw = new MainWindow(quer = AppData.Context.Locomotive
                    .Where(x => x.SectionNumber.Equals(cmbLoc.Text) && x.DepotDirectorate.Equals(cmbRegion.Text)
                    && x.TheCompanyThatProducedTheTurning.Equals(cmbSLD.Text)).ToList());
                //поиск строк по локомотиву, дирекции, СЛД
                mw.Show();
                this.Close();
            }

            if (data_ind != -1 && loc_ind == 0 && per_ind != 0 && reg_ind == 0 && sld_ind == 0)
            {
                DateTime d1 = dp1.DisplayDate;
                DateTime d2 = dp2.DisplayDate;
                MainWindow mw = new MainWindow(quer = AppData.Context.Locomotive
                    .Where(x => x.TurningDate >= d1 && x.TurningDate <= d2).ToList());
                //поиск строк по периоду
                mw.Show();
                this.Close();
            }
            if (data_ind != -1 && loc_ind == 0 && per_ind != 0 && reg_ind != 0 && sld_ind == 0)
            {
                DateTime d1 = dp1.DisplayDate;
                DateTime d2 = dp2.DisplayDate;
                MainWindow mw = new MainWindow(quer = AppData.Context.Locomotive
                    .Where(x => x.TurningDate >= d1 && x.TurningDate <= d2 && x.DepotDirectorate.Equals(cmbRegion.Text)).ToList());
                //поиск строк по периоду и дирекции
                mw.Show();
                this.Close();
            }
            if (data_ind != -1 && loc_ind == 0 && per_ind != 0 && reg_ind != 0 && sld_ind != 0)
            {
                DateTime d1 = dp1.DisplayDate;
                DateTime d2 = dp2.DisplayDate;
                MainWindow mw = new MainWindow(quer = AppData.Context.Locomotive
                    .Where(x => x.TurningDate >= d1 && x.TurningDate <= d2 && x.DepotDirectorate.Equals(cmbRegion.Text)
                    && x.TheCompanyThatProducedTheTurning.Equals(cmbSLD.Text)).ToList());
                //поиск строк по периоду, дирекции, слд
                mw.Show();
                this.Close();
            }
            if (data_ind != -1 && loc_ind == 0 && per_ind != 0 && reg_ind == 0 && sld_ind != 0)
            {
                DateTime d1 = dp1.DisplayDate;
                DateTime d2 = dp2.DisplayDate;
                MainWindow mw = new MainWindow(quer = AppData.Context.Locomotive
                    .Where(x => x.TurningDate >= d1 && x.TurningDate <= d2
                    && x.TheCompanyThatProducedTheTurning.Equals(cmbSLD.Text)).ToList());
                //поиск строк по периоду, слд
                mw.Show();
                this.Close();
            }
            if (data_ind != -1 && loc_ind != 0 && per_ind != 0 && reg_ind == 0 && sld_ind != 0)
            {
                DateTime d1 = dp1.DisplayDate;
                DateTime d2 = dp2.DisplayDate;
                MainWindow mw = new MainWindow(quer = AppData.Context.Locomotive
                    .Where(x => x.SectionNumber.Equals(cmbLoc.Text) && x.TurningDate >= d1 && x.TurningDate <= d2
                    && x.TheCompanyThatProducedTheTurning.Equals(cmbSLD.Text)).ToList());
                //поиск строк по периоду, слд, локомотиву
                mw.Show();
                this.Close();
            }
            if (data_ind != -1 && loc_ind == 0 && per_ind == 0 && reg_ind != 0 && sld_ind == 0)
            {
               
                MainWindow mw = new MainWindow(quer = AppData.Context.Locomotive
                    .Where(x => x.DepotDirectorate.Equals(cmbRegion.Text)).ToList());
                //поиск строк по дирекции
                mw.Show();
                this.Close();
            }
            if (data_ind != -1 && loc_ind == 0 && per_ind == 0 && reg_ind != 0 && sld_ind != 0)
            {

                MainWindow mw = new MainWindow(quer = AppData.Context.Locomotive
                    .Where(x => x.DepotDirectorate.Equals(cmbRegion.Text) && x.TheCompanyThatProducedTheTurning.Equals(cmbSLD.Text)).ToList());
                //поиск строк по дирекции, sld
                mw.Show();
                this.Close(); 
            }
            if (data_ind != -1 && loc_ind == 0 && per_ind == 0 && reg_ind == 0 && sld_ind != 0)
            {

                MainWindow mw = new MainWindow(quer = AppData.Context.Locomotive
                    .Where(x => x.TheCompanyThatProducedTheTurning.Equals(cmbSLD.Text)).ToList());
                //поиск строк по sld
                mw.Show();
                this.Close();
            }
            //////////





        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }

        private void cmbPeriod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cmbPeriod.SelectedIndex == 1)
            {
                tb1.Opacity = 1;
                tb2.Opacity = 1;
                dp1.IsEnabled = true;
                dp2.IsEnabled = true;
            }
            else
            {
                tb1.Opacity = 0;
                tb2.Opacity = 0;
                dp1.SelectedDate = null;
                dp2.SelectedDate = null;
                dp1.IsEnabled = false;
                dp2.IsEnabled = false;
            }
        }
    }
}
