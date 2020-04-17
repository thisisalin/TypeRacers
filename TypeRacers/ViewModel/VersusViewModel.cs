using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using TypeRacers.Client;

namespace TypeRacers.ViewModel
{
    public class VersusViewModel : ITextToType, INotifyPropertyChanged
    {
        private string typedText;
        private InputCharacterValidation userInputValidator;
        private bool startReporting;
        private Player player;
        private GameInfo gameInfo;

        public VersusViewModel()
        {
            UpdateShownPlayers();
            EnableSearchingAnimation = true;
            ExitProgramCommand = new CommandHandler(ExitProgram, () => true);
            RemovePlayer = new CommandHandler(RemovePlayerFromPlayroom, () => true);
            RestartSearchingOpponentsCommand = new CommandHandler(RestartSearchingOpponents, () => true);
            OKbuttonCommand = new CommandHandler(OnOKButtonPressed, () => true);
        }

        public Player Player
        {
            get => player;
            set
            {
                player = value;
                TriggerPropertyChanged(nameof(Player));
            }
        }

        public GameInfo GameInfo
        {
            get => gameInfo;
            set
            {
                gameInfo = value;
                TriggerPropertyChanged(nameof(GameInfo));
                TriggerPropertyChanged(nameof(TextToType));
                TriggerPropertyChanged(nameof(TextToTypeStyles));
                TriggerPropertyChanged(nameof(SliderProgress));
                TriggerPropertyChanged(nameof(UserInputValidator));
                GameInfo.SubscribeToSearchingOpponents(UpdateOpponents);
                TriggerPropertyChanged(nameof(Opponents));
                TriggerPropertyChanged(nameof(WaitingTime));
                TriggerPropertyChanged(nameof(StartTime));
                TriggerPropertyChanged(nameof(EndTime));
                TriggerPropertyChanged(nameof(ShowRanking));
                TriggerPropertyChanged(nameof(RankingPlace));
                UpdateShownPlayers();
            }
        }

        public CommandHandler RemovePlayer { get; }
        public CommandHandler RestartSearchingOpponentsCommand { get; }
        public CommandHandler ExitProgramCommand { get; }
        public CommandHandler OKbuttonCommand { get; }
        public IEnumerable<Inline> TextToTypeStyles => UserInputValidator.TextToTypeStyles;

        public IEnumerable<Player> Opponents => GameInfo?.Players ?? new List<Player>();

        public Visibility ShowFirstOpponent { get; set; }

        public Visibility ShowSecondOpponent { get; set; }

        public DateTime WaitingTime => GameInfo.TimeToWaitForOpponents;
        public DateTime TimeToStart => DateTime.UtcNow.AddSeconds(WaitingTime.Subtract(DateTime.UtcNow).Seconds);
        public int OpponentsCount { get; set; }

        private InputCharacterValidation UserInputValidator { get => userInputValidator ?? new InputCharacterValidation(TextToType); set => userInputValidator = value; }

        public int SliderProgress
        {
            get
            {
                if (UserInputValidator.AllTextTyped || TextToType.Length == 0)
                {
                    return 100;
                }

                return UserInputValidator.SpaceIndex * 100 / TextToType.Length;
            }
        }

        public int WPMProgress
        {
            get
            {
                if (UserInputValidator.CurrentWordIndex == 0)
                {
                    return 0;
                }

                var wordperminut = (UserInputValidator.NumberOfCharactersTyped / 5) * 60;
                var secondsInGame = (int)(DateTime.UtcNow - StartTime).TotalSeconds;
                return wordperminut / secondsInGame;
            }
        }

        public bool AllTextTyped => UserInputValidator.AllTextTyped;

        //determines if a popup alert should apear, binded in open property of popup xaml
        public bool TypingAlert => UserInputValidator.TypingAlert;

        public string InputBackgroundColor => UserInputValidator.InputBackgroundColor;

        public bool StartReportingProgress
        {
            get => startReporting;

            set
            {
                startReporting = value;
                TriggerPropertyChanged(nameof(StartReportingProgress));
                ReportProgress();
            }
        }

        public string TextToType => GameInfo?.CompetitionText ?? string.Empty;
        public bool EnableGetReadyAlert { get; set; }
        public bool EnableRestartOrExitAlert { get; set; }
        public bool EnableDisconnectedAlert { get; set; }
        public string SecondsToGetReady { get; set; }
        public bool EnableSearchingAnimation { get; private set; }
        public DateTime StartTime { get; set; }
        public bool ShowRanking => Player?.Finnished ?? false;
        public string RankingPlace => Player?.Place.ToString() ?? string.Empty;
        public int Accuracy => UserInputValidator?.Accuracy ?? 0;
        public bool OpenFinishPopup => UserInputValidator?.OpenFinishPopup ?? false;
        public DateTime EndTime { get; private set; }

        private void ReportProgress()
        {
            if (StartReportingProgress)
            {
                TriggerPropertyChanged(nameof(Opponents));
                Player.UpdateProgress(WPMProgress, SliderProgress);
            }
        }

        public string CurrentInputText
        {
            get => typedText;
            set
            {
                if (typedText == value)
                    return;

                if (userInputValidator == null)
                {
                    UserInputValidator = new InputCharacterValidation(TextToType);
                    TriggerPropertyChanged(nameof(UserInputValidator));
                }

                UserInputValidator.ValidateInput(value);

                TriggerPropertyChanged(nameof(TextToTypeStyles));
                TriggerPropertyChanged(nameof(CurrentInputText));
                TriggerPropertyChanged(nameof(Accuracy));
                TriggerPropertyChanged(nameof(OpenFinishPopup));
                TriggerPropertyChanged(nameof(TypingAlert));
                TriggerPropertyChanged(nameof(SliderProgress));
                TriggerPropertyChanged(nameof(WPMProgress));
                TriggerPropertyChanged(nameof(InputBackgroundColor));
                TriggerPropertyChanged(nameof(AllTextTyped));

                typedText = UserInputValidator.CurrentInputText;
            }
        }

        private void OnOKButtonPressed()
        {
            EnableDisconnectedAlert = false;
            TriggerPropertyChanged(nameof(EnableDisconnectedAlert));
        }

        private void RestartSearchingOpponents()
        {
            EnableRestartOrExitAlert = false;
            TriggerPropertyChanged(nameof(EnableRestartOrExitAlert));

            Player.Restarting = true;
            //getting the waiting time again
            TriggerPropertyChanged(nameof(WaitingTime));
            TriggerPropertyChanged(nameof(TimeToStart));
            EnableSearchingAnimation = true;
            TriggerPropertyChanged(nameof(EnableSearchingAnimation));
        }

        private void ExitProgram()
        {
            Player.Removed = true;
            Application.Current.Shutdown();
        }

        private void RemovePlayerFromPlayroom()
        {
            Player.Removed = true;
        }

        private void UpdateOpponents(List<Player> uppdateOpponents)
        {
            TriggerPropertyChanged(nameof(Opponents));
            OpponentsCount = Opponents.Count();
            TriggerPropertyChanged(nameof(OpponentsCount));
            TriggerPropertyChanged(nameof(RankingPlace));
            TriggerPropertyChanged(nameof(ShowRanking));
            CheckConnectionStatus();

            UpdateShownPlayers();

            CheckIfStartTimeWasSet();

            CheckIfWaitingTimeHasPassed();
        }

        private void CheckConnectionStatus()
        {
            EnableDisconnectedAlert = GameInfo.ConnectionLost;
            TriggerPropertyChanged(nameof(EnableDisconnectedAlert));
        }

        private void CheckIfStartTimeWasSet()
        {
            if (GameInfo.GameStartingTime != DateTime.MinValue)
            {
                EnableSearchingAnimation = false;
                TriggerPropertyChanged(nameof(EnableSearchingAnimation));

                StartTime = GameInfo.GameStartingTime;
                TriggerPropertyChanged(nameof(StartTime));

                EndTime = GameInfo.GameEndingTime;
                TriggerPropertyChanged(nameof(EndTime));

                SecondsToGetReady = (StartTime - DateTime.UtcNow).Seconds.ToString();
                TriggerPropertyChanged(nameof(SecondsToGetReady));

                EnableGetReadyAlert = true;
                TriggerPropertyChanged(nameof(EnableGetReadyAlert));
            }
        }

        private void CheckIfWaitingTimeHasPassed()
        {
            if (TimeToStart.Subtract(DateTime.UtcNow) <= TimeSpan.Zero && GameInfo.GameStartingTime == DateTime.MinValue && !EnableGetReadyAlert)
            {
                if (Opponents.Any())
                {
                    CheckIfStartTimeWasSet();
                }
                else
                {
                    if (!Player.Restarting)
                    {
                        EnableSearchingAnimation = false;
                        TriggerPropertyChanged(nameof(EnableSearchingAnimation));
                        EnableRestartOrExitAlert = true;
                        TriggerPropertyChanged(nameof(EnableRestartOrExitAlert));
                    }
                }
            }
        }

        private void UpdateShownPlayers()
        {
            if (Opponents.Count() == 0)
            {
                ShowFirstOpponent = Visibility.Hidden;
                ShowSecondOpponent = Visibility.Hidden;
                return;
            }
            if (Opponents.Count() == 1)
            {
                ShowFirstOpponent = Visibility.Visible;
                ShowSecondOpponent = Visibility.Hidden;
            }

            if (Opponents.Count() == 2)
            {
                ShowFirstOpponent = Visibility.Visible;
                ShowSecondOpponent = Visibility.Visible;
            }

            TriggerPropertyChanged(nameof(ShowFirstOpponent));
            TriggerPropertyChanged(nameof(ShowSecondOpponent));
        }

        //INotifyPropertyChanged code - basic
        public event PropertyChangedEventHandler PropertyChanged;

        public void TriggerPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}