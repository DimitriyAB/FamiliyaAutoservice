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

namespace FamiliyaAutoservice_1
{
    /// <summary>
    /// Логика взаимодействия для SingUpPage.xaml
    /// </summary>
    public partial class SingUpPage : Page
    {
        private Service _currentService = new Service();
        public SingUpPage(Service SelectedService)
        {
            InitializeComponent();
            if(SelectedService != null)
                this._currentService = SelectedService;

            DataContext = _currentService;

            var _currentClient = Baranov_AutoserviceEntities4.GetContext().Client.ToList();

            ComboClient.ItemsSource = _currentClient;
        }
        private ClientService _currentClientService = new ClientService();
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (ComboClient.SelectedItem == null)
                errors.AppendLine("Укажите ФИО клиента");
            if (StartDate.Text == "")
                errors.AppendLine("Укажите дату услуги");
            Manager.MainFrame.Navigate(new SingUpPage((sender as Button).DataContext as Service));
            if (TBStart.Text == "")
                errors.AppendLine("Укажите время начала услуги");

            if(errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            _currentClientService.ClientID = ComboClient.SelectedIndex + 1;
            _currentClientService.ServiceID = _currentService.ID;
            _currentClientService.StartTime = Convert.ToDateTime(StartDate.Text + " " + TBStart.Text);
            if (_currentClientService.ID == 0)
                Baranov_AutoserviceEntities4.GetContext().ClientService.Add(_currentClientService);

            try
            {
                Baranov_AutoserviceEntities4.GetContext().SaveChanges();
                MessageBox.Show("информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void TBStart_TextChanged(object sender, TextChangedEventArgs e)
        {
            string s = TBStart.Text;

            if(s.Length < 4 || !s.Contains(':')) 
                TBEnd.Text = "";
            else
            {
                string[] start = s.Split(new char[] {':'});
                int startHour = Convert.ToInt32(start[0].ToString()) * 60;
                int startMin = Convert.ToInt32(start[1].ToString());
                

                int sum = startHour + startMin + Convert.ToInt32(_currentService.DurationInSeconds);

                int EndHour = sum / 60;
                if (EndHour > 23)
                {
                    EndHour -= 24;
                }
                int EndMin = sum % 60;
                s = EndHour.ToString() + ":" + EndMin.ToString();
                TBEnd.Text = s;

            }
        }

        private void TBStart_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
                return;
            }
            //текущее значение текста в TextBox
            string currentValue = ((TextBox)sender).Text;
            if (currentValue.Length == 0)
            {
                int hour1 = Convert.ToInt32(e.Text);
                TBStart.Clear();
                if (hour1 > 2)
                {
                    currentValue = "";
                    currentValue = "0" + (hour1).ToString();
                    TBStart.Text = "";

                    TBStart.Text = currentValue;
                    e.Handled = true;
                }
            }
            if (currentValue.Length == 1)
            {
                if (currentValue[0] == '2')
                {
                    int hours2 = Convert.ToInt32(e.Text);
                    Console.WriteLine(hours2);
                    if (hours2 > 3)
                    {
                        e.Handled = true; //игнорируем ввод
                        return;
                    }
                }
            }

            if (currentValue.Length == 3)
            {
                int minute = Convert.ToInt32(e.Text);
                if (minute > 5)
                {
                    e.Handled = true; //игнорируем ввод
                    return;
                }


            }

            //Если введено 2 цифры и след символ не ":", добавляем ":"
            if (currentValue.Length == 2 && e.Text != ":")
            {
                currentValue += ":";
            }

            // Если введено 5 символов (формат "hh:mm"), то не даем вводить больше
            if (currentValue.Length > 4)
            {
                e.Handled = true;
                return;
            }

            // Обновляем значение текста в TextBox
            ((TextBox)sender).Text = currentValue;
            ((TextBox)sender).SelectionStart = ((TextBox)sender).Text.Length;
        }
    }
}
