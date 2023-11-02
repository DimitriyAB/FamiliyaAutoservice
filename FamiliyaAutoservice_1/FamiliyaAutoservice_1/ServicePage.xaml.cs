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

    public partial class ServicePage : Page
    {
        int CounteRecords;
        int CountPage;
        int CurrentPage = 0;

        List<Service> CurrentPagelist = new List<Service>();
        List<Service> TableList;
        public ServicePage()
        {
            InitializeComponent();
            var currentServices = Baranov_AutoserviceEntities3.GetContext().Service.ToList();
            ServiceListView.ItemsSource = currentServices;
            ComboType.SelectedIndex = 0;
            UpdateService();

        }
        private void UpdateService()
        {
            var currentServices = Baranov_AutoserviceEntities3.GetContext().Service.ToList();

            if (ComboType.SelectedIndex == 0)
            {
                currentServices = currentServices.Where(p => (p.Discount >= 0 && p.Discount <= 100)).ToList();
            }
            if (ComboType.SelectedIndex == 1)
            {
                currentServices = currentServices.Where(p => (p.Discount >= 0 && p.Discount < 5)).ToList();
            }
            if (ComboType.SelectedIndex == 2)
            {
                currentServices = currentServices.Where(p => (p.Discount >= 5 && p.Discount < 15)).ToList();
            }
            if (ComboType.SelectedIndex == 3)
            {
                currentServices = currentServices.Where(p => (p.Discount >= 15 && p.Discount < 30)).ToList();
            }
            if (ComboType.SelectedIndex == 4)
            {
                currentServices = currentServices.Where(p => (p.Discount >= 30 && p.Discount < 70)).ToList();
            }
            if (ComboType.SelectedIndex == 5)
            {
                currentServices = currentServices.Where(p => (p.Discount >= 70 && p.Discount < 100)).ToList();
            }


            currentServices = currentServices.Where(p => p.Title.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            if (RButtonDown.IsChecked.Value)
            {
                currentServices = currentServices.OrderByDescending(p => p.Cost).ToList();
            }
            if (RButtonUp.IsChecked.Value)
            {
                currentServices = currentServices.OrderBy(p => p.Cost).ToList();
            }

            ServiceListView.ItemsSource = currentServices;

            TableList = currentServices;

            ChangePage(0, 0);


        }
        private void TBoxSearch_TextChanged(object sender, EventArgs e)
        {
            UpdateService();
        }

        private void RButtonUp_Checked_1(object sender, RoutedEventArgs e)
        {
            UpdateService();
        }

        private void RButtonDown_Checked_1(object sender, RoutedEventArgs e)
        {
            UpdateService();
        }

        private void ComboType_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            UpdateService();
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Baranov_AutoserviceEntities3.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                ServiceListView.ItemsSource = Baranov_AutoserviceEntities3.GetContext().Service.ToList();
                UpdateService();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Service));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var currentService = (sender as Button).DataContext as Service;

            var currentClientServices = Baranov_AutoserviceEntities3.GetContext().ClientService.ToList();
            currentClientServices = currentClientServices.Where(p => p.ServiceID == currentService.ID).ToList();

            if (currentClientServices.Count != 0)
                MessageBox.Show("Невозможно выполнить удаление, так как существуют записи на эту услугу");
            else
            {

                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Baranov_AutoserviceEntities3.GetContext().Service.Remove(currentService);
                        Baranov_AutoserviceEntities3.GetContext().SaveChanges();
                        ServiceListView.ItemsSource = Baranov_AutoserviceEntities3.GetContext().Service.ToList();
                        UpdateService();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }

        private void ChangePage(object direction, int? selectedPage)
        {
            CurrentPagelist.Clear();
            CounteRecords = TableList.Count;

            if (CounteRecords % 10 > 0)
            {
                CountPage = CounteRecords / 10 + 1;
            }
            else
            {
                CountPage = CounteRecords / 10;
            }

            Boolean Ifupdate = true;

            int min;

            if (selectedPage.HasValue)
            {
                if (selectedPage >= 0 && selectedPage <= CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * 10 + 10 < CounteRecords ? CurrentPage * 10 + 10 : CounteRecords;
                    for (int i = CurrentPage * 10; i < min; i++)
                    {
                        CurrentPagelist.Add(TableList[i]);
                    }
                }
            }
            else
            {
                switch (direction)
                {
                    case 1:
                        if(CurrentPage > 0)
                        {
                            CurrentPage--;
                            min = CurrentPage*10+10< CounteRecords ? CurrentPage*10+10 : CounteRecords;
                            for(int i = CurrentPage*10; i < min; i++)
                            {
                                CurrentPagelist.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;
                    case 2:
                        if(CurrentPage < CountPage -1) 
                        {
                            CurrentPage++;
                            min = CurrentPage*10+10 < CounteRecords ? CurrentPage*10+10 : CounteRecords;
                            for(int i = CurrentPage*10;i < min; i++)
                            {
                                CurrentPagelist.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            Ifupdate = false;
                        }
                        break;
                }
            }

            if (Ifupdate)
            {
                PageListBox.Items.Clear();

                for(int i =1;i<=CountPage;i++)
                {
                    PageListBox.Items.Add(i);
                }
                PageListBox.SelectedIndex = CurrentPage;

                min = CurrentPage*10+10 < CounteRecords ? CurrentPage *10+10 : CounteRecords;
                TBCount.Text = min.ToString();
                TBAllRecords.Text = "из" + CounteRecords.ToString();

                ServiceListView.ItemsSource = CurrentPagelist;
                ServiceListView.Items.Refresh();
            }

        }
        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);

        }
    }
}
