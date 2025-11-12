using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class AddEditPage : Page
    {
        private Partners _currentPartner = new Partners();

        public AddEditPage(Partners SelectedPartner)
        {
            InitializeComponent();

            if (SelectedPartner != null)
                _currentPartner = SelectedPartner;
            else
                _currentPartner = new Partners(); 

            DataContext = _currentPartner;
            LoadTypes();
        }

        private void LoadTypes()
        {
            var Types = Verko_MasterFloorEntities.GetContext().PartnerTypes.ToList();
            ComboTypes.ItemsSource = Types;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (ComboTypes.SelectedItem == null)
                errors.AppendLine("Укажите тип партнера");

            if (string.IsNullOrWhiteSpace(_currentPartner.PartnerCompanyName))
                errors.AppendLine("Укажите наименование компании");
            else if (_currentPartner.PartnerCompanyName.Length > 100)
                errors.AppendLine("Наименование компании не может быть длиннее 100 символов");

            if (string.IsNullOrWhiteSpace(_currentPartner.PartnerDirectorSurname))
                errors.AppendLine("Укажите фамилию директора партнера");
            else if (_currentPartner.PartnerDirectorSurname.Length > 50)
                errors.AppendLine("Фамилия не может быть длиннее 50 символов");
            else if (!Regex.IsMatch(_currentPartner.PartnerDirectorSurname, @"^[а-яА-ЯёЁa-zA-Z\s-]+$"))
                errors.AppendLine("Фамилия может содержать только буквы, пробелы и дефисы");

            if (string.IsNullOrWhiteSpace(_currentPartner.PartnerDirectorName))
                errors.AppendLine("Укажите имя директора партнера");
            else if (_currentPartner.PartnerDirectorName.Length > 50)
                errors.AppendLine("Имя не может быть длиннее 50 символов");
            else if (!Regex.IsMatch(_currentPartner.PartnerDirectorName, @"^[а-яА-ЯёЁa-zA-Z\s-]+$"))
                errors.AppendLine("Имя может содержать только буквы, пробелы и дефисы");

            if (!string.IsNullOrWhiteSpace(_currentPartner.PartnerDirectorPatronymic) && _currentPartner.PartnerDirectorPatronymic.Length > 50)
                errors.AppendLine("Отчество не может быть длиннее 50 символов");
            else if (!string.IsNullOrWhiteSpace(_currentPartner.PartnerDirectorPatronymic) && !Regex.IsMatch(_currentPartner.PartnerDirectorPatronymic, @"^[а-яА-ЯёЁa-zA-Z\s-]+$"))
                errors.AppendLine("Отчество может содержать только буквы, пробелы и дефисы");

            if (string.IsNullOrWhiteSpace(_currentPartner.PartnerEmail))
            {
                errors.AppendLine("Укажите email партнера");
            }
            else if (!IsValidEmail(_currentPartner.PartnerEmail))
            {
                errors.AppendLine("Укажите корректный email адрес");
            }

            if (string.IsNullOrWhiteSpace(_currentPartner.PartnerPhone))
            {
                errors.AppendLine("Укажите номер телефона партнера");
            }
            else
            {
                string validationResult = ValidatePhoneNumber(_currentPartner.PartnerPhone);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    errors.AppendLine(validationResult);
                }
            }

            if (string.IsNullOrWhiteSpace(_currentPartner.PartnerAddress))
                errors.AppendLine("Укажите юридический адрес партнера");
            else if (_currentPartner.PartnerAddress.Length > 200)
                errors.AppendLine("Адрес не может быть длиннее 200 символов");

            if (string.IsNullOrWhiteSpace(_currentPartner.PartnerINN))
                errors.AppendLine("Укажите ИНН партнера");
            else
            {
                string inn = _currentPartner.PartnerINN.Replace(" ", "").Replace("-", "");
                if (inn.Length != 10)
                    errors.AppendLine("ИНН должен содержать 10 цифр");
                else if (!Regex.IsMatch(inn, @"^\d+$"))
                    errors.AppendLine("ИНН может содержать только цифры");
            }

            if (_currentPartner.PartnerRating == null)
                errors.AppendLine("Укажите рейтинг партнера");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            _currentPartner.PartnerPhone = FormatPhoneNumber(_currentPartner.PartnerPhone);

            if (_currentPartner.PartnerID == 0)
            {
                Verko_MasterFloorEntities.GetContext().Partners.Add(_currentPartner);
            }

            try
            {
                Verko_MasterFloorEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.Navigate(new PartnerPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private string ValidatePhoneNumber(string phone)
        {
            string digitsOnly = Regex.Replace(phone, @"[^\d+]", "");

            string cleanPhone = digitsOnly.Replace("+", "");
            if (cleanPhone.StartsWith("8") || cleanPhone.StartsWith("7"))
            {
                cleanPhone = cleanPhone.Substring(1);
            }

            if (cleanPhone.Length != 10)
            {
                return "Номер телефона должен содержать 10 цифр (после кода страны)";
            }

            if (!Regex.IsMatch(cleanPhone, @"^\d+$"))
            {
                return "Номер телефона может содержать только цифры, пробелы, скобки, дефисы и знак +";
            }

                return null;
        }

        private string FormatPhoneNumber(string phone)
        {
            string digitsOnly = Regex.Replace(phone, @"[^\d+]", "");

            string cleanPhone = digitsOnly.Replace("+", "");
            if (cleanPhone.StartsWith("8") || cleanPhone.StartsWith("7"))
            {
                cleanPhone = cleanPhone.Substring(1);
            }

            if (cleanPhone.Length == 10)
            {
                return $"+7 ({cleanPhone.Substring(0, 3)}) {cleanPhone.Substring(3, 3)}-{cleanPhone.Substring(6, 2)}-{cleanPhone.Substring(8, 2)}";
            }

            return phone; 
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var currentPartner = (sender as Button).DataContext as Partners;

            var currentPartnerProducts = Verko_MasterFloorEntities.GetContext().PartnerProducts.Where(pp => pp.PartnerID == currentPartner.PartnerID).ToList();

            if (currentPartnerProducts.Count != 0)
                MessageBox.Show("Невозможно выполнить удаление, так как у партнера есть история продаж");
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Verko_MasterFloorEntities.GetContext().Partners.Remove(currentPartner);
                        Verko_MasterFloorEntities.GetContext().SaveChanges();
                        Manager.MainFrame.Navigate(new PartnerPage());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }
    }
}