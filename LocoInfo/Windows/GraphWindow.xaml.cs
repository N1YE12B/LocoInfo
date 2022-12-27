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

using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Helpers;
using LocoInfo.Classes;

namespace LocoInfo.Windows
{
    /// <summary>
    /// Логика взаимодействия для GraphWindow.xaml
    /// </summary>
    public partial class GraphWindow : Window
    {
        public GraphWindow()
        {
            InitializeComponent();
            MainFrame.frame = mainFrame;

        }

        private void btnReason_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.frame.Navigate(new Pages.TurningReasonPage());
        }
    }
}
