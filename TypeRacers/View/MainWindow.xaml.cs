﻿using System.Windows;
using System.Windows.Controls;
using TypeRacers.ViewModel;

namespace TypeRacers.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainPage mainPage;
        private MainViewModel mainViewModel;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        public void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            mainPage = new MainPage();
            mainViewModel = (MainViewModel)mainPage.Resources["MainVM"];
            Frame.NavigationService.Navigate(mainPage);
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            if (mainViewModel.Model != null)
            {
                mainViewModel.Model.GetPlayer().Removed = true;
            }
        }
    }
}