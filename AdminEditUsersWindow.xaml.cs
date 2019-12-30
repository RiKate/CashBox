﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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


namespace MVVMCashbox
{
    /// <summary>
    /// Логика взаимодействия для AdminEditUsersWindow.xaml
    /// </summary>
    public partial class AdminEditUsersWindow : Window
    {
        AdminEditUsersViewModel model;
        readonly IJsonFileInterface<User> jsonData = new JsonFileService<User>();

        public AdminEditUsersWindow()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
            Closing += Window_Closing;

            model = new AdminEditUsersViewModel();

        }


        // Загрузка данных по событию загруженного окна
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            model.Users = jsonData.Load(@"..\..\data/users.json");
            DataContext = model;
        }

        // Сохранение данных по событию закрытия окна
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (model != null)
            {
                model.CheckUsers();
                jsonData.Save(@"..\..\data/users.json", model.Users);
            }
        }


    }
}
