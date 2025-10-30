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

namespace Verko_MasterFloor
{
    /// <summary>
    /// Логика взаимодействия для PartnerPage.xaml
    /// </summary>
    public partial class PartnerPage : Page
    {
        public PartnerPage()
        {
            InitializeComponent();

            var currentPartners = Verko_MasterFloorEntities.GetContext().Partners.ToList();

            PartnerListView.ItemsSource = currentPartners;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage());
        }
    }
}
