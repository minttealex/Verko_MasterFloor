using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Verko_MasterFloor
{
    public partial class PartnerHistoryPage : Page
    {
        private Partners _currentPartner;

        public string PartnerInfo => _currentPartner != null
            ? $"История реализации продукции: {_currentPartner.PartnerCompanyName}"
            : "История реализации продукции";

        public PartnerHistoryPage(Partners partner)
        {
            InitializeComponent();
            _currentPartner = partner;
            DataContext = this;
            LoadPartnerHistory();
        }

        private void LoadPartnerHistory()
        {
            if (_currentPartner != null)
            {
                var history = Verko_MasterFloorEntities.GetContext().PartnerProducts
                    .Where(pp => pp.PartnerID == _currentPartner.PartnerID)
                    .OrderByDescending(pp => pp.SellDate)
                    .ToList();

                HistoryListView.ItemsSource = history;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.GoBack();
        }
    }
}