
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;
using TypeRacers.View;

namespace TypeRacers.ViewModel
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private VersusPage race;
        private PracticePage practice;

        public MainViewModel()
        {
            RetryCommand = new CommandHandler(NavigateContest, () => true);
            ContestCommand = new CommandHandler(NavigateContest, () => true);
            PracticeCommand = new CommandHandler(NavigatePractice, () => true);
        }
        public bool UsernameEntered { get; set; }
        public Model.Model Model { get; set; }
        public bool EnterUsernameMessage { get; set; }
        public CommandHandler RetryCommand { get; }
        public CommandHandler ContestCommand { get; }
        public NavigationService ContestNavigation { get; set; }
        public CommandHandler PracticeCommand { get; }
        public NavigationService PracticeNavigation { get; set; }

        public string Username
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    UsernameEntered = false;
                    TriggerPropertyChanged(nameof(UsernameEntered));
                    return;
                }

                UsernameEntered = true;
                TriggerPropertyChanged(nameof(UsernameEntered));
                Name = value;
            }
        }

        public static string Name { get; private set; }
        public bool EnableConnectingAnimation { get; set; }
        public bool EnableRetry { get; set; }

        private void NavigateContest()
        {
            if (UsernameEntered)
            {
                race = new VersusPage();

                Model = new Model.Model();
                Thread connect = new Thread(() =>
                {
                    try
                    {
                        bool connected = Model.StartCommunication();
                        if (!connected)
                        {
                            throw new SocketException();

                        }
                        var gameInfo = Model.GetGameInfo();
                        var player = Model.GetPlayer();
                        while (!gameInfo.GameInfoIsSet)
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                EnableConnectingAnimation = true;
                                TriggerPropertyChanged(nameof(EnableConnectingAnimation));
                            });
                        }
                        Application.Current.Dispatcher.Invoke(() => race.GameInfo = gameInfo);
                        Application.Current.Dispatcher.Invoke(() => race.Player = player);
                        Application.Current.Dispatcher.Invoke(() => ContestNavigation.Navigate(race));
                    }
                    catch (SocketException)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            EnableRetry = true;
                            TriggerPropertyChanged(nameof(EnableRetry));
                            EnableConnectingAnimation = false;
                            TriggerPropertyChanged(nameof(EnableConnectingAnimation));
                        });
                    }
                });
                connect.Start();
            }
            else
            {
                EnterUsernameMessage = true;
                TriggerPropertyChanged(nameof(EnterUsernameMessage));
            }
        }

        private void NavigatePractice()
        {
            if (UsernameEntered)
            {
                Model = new Model.Model();
                practice = new PracticePage
                {
                    TextToType = Model.GetGeneratedTextToTypeLocally()
                };

                PracticeNavigation.Navigate(practice);
            }
            else
            {
                EnterUsernameMessage = true;
                TriggerPropertyChanged(nameof(EnterUsernameMessage));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void TriggerPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}