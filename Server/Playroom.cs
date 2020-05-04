using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Server
{
    public class Playroom : IPlayroom
    {
        public Playroom(ITextToType textToType)
        {
            Players = new List<Player>();
            CompetitionText = textToType.GetData();
            TimeToWaitForOpponents = DateTime.UtcNow.AddSeconds(20);
        }

        private bool GameHasStarted => GameStartingTime != DateTime.MinValue;
        public List<Player> Players { get; set; }
        public DateTime GameStartingTime { get; set; }
        public DateTime GameEndingTime { get; set; }
        public DateTime TimeToWaitForOpponents { get; set; }
        public int Place { get; set; } = 1;
        public string CompetitionText { get; set; }

        public bool Join(Player player)
        {
            if (!GameHasStarted && Players.Count != 3 && !IsInPlayroom(player.Name))
            {
                Players.Add(player);
                player.Playroom = this;
                StartCommunication(player);
                return true;
            }
            return false;
        }

        private void StartCommunication(Player player)
        {
            player.BeginReadMessage(ReadSuccessCallback, FailedCallback);

            void ReadSuccessCallback(IMessage receivedMessage)
            {
                var message = (ReceivedMessage)receivedMessage;

                var receivedData = message.Data;

                if (!string.IsNullOrEmpty(receivedData))
                {
                    IMessage toSend = ProcessReceivedResults(receivedData, player);
                    if (toSend != null)
                    {
                        player.BeginWriteMessage(toSend, WriteSuccessCallback, FailedCallback);
                    }

                }
                else
                {
                    player.NetworkClient.Dispose();
                }
            }

            void WriteSuccessCallback()
            {
                player.BeginReadMessage(ReadSuccessCallback, FailedCallback);
            }

            void FailedCallback(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                player.NetworkClient.Dispose();
            }
        }


        private IMessage ProcessReceivedResults(string receivedData, Player player)
        {
            Console.WriteLine("Data received: " + receivedData);
            UpdateInfo(player, receivedData);

            if (!CheckIfPlayerLeft(player))
            {
                if (player.FirstTimeConnecting || CheckIfPlayerLeft(player))
                {
                    TrySetStartingTime();
                    player.FirstTimeConnecting = false;
                    return GameMessage();
                }
                else
                {
                    TrySetStartingTime();
                    TrySetRank(player);
                    return GetGameStatus(player);
                }
            }

            return null;
        }

        public bool CheckIfPlayerLeft(Player player)
        {
            if (player.Name.Contains("_removed"))
            {
                Console.WriteLine("Player removed: " + player.Name);
                Leave(player.Name);
                return true;
            }
            return false;
        }

        public void TrySetRank(Player player)
        {
            if (player.CompletedTextPercentage == 100 && !player.Finnished)
            {
                player.Finnished = true;
                player.Place = Place++;
            }
        }

        public bool CheckIfTriesToRestart(Player player)
        {
            return player.Name.Contains("_restart");
        }

        private bool IsInPlayroom(string playerName)
        {
            return Players.Any(x => x.Name.Equals(playerName));
        }

        public Player GetPlayer(string name)
        {
            return Players.Find(x => x.Name.Equals(name));
        }

        public bool Leave(string playerName)
        {
            if (IsInPlayroom(playerName))
            {
                Players.Remove(Players.Find(x => x.Name.Equals(playerName)));
                return true;
            }
            if (Players.Count == 0)
            {
                Reset();
            }

            Console.WriteLine("REMOVED: " + playerName);
            Console.WriteLine("Playroom size: " + Players.Count);
            return false;
        }

        private void Reset()
        {
            TimeToWaitForOpponents = DateTime.UtcNow.AddSeconds(20);
        }

        public void TrySetStartingTime()
        {
            if (!GameHasStarted)
            {
                if (Players.Count == 3 || (TimeToWaitForOpponents - DateTime.UtcNow.AddSeconds(2) <= TimeSpan.Zero && Players.Count == 2))
                {
                    GameStartingTime = DateTime.UtcNow.AddSeconds(10);
                    GameEndingTime = GameStartingTime.AddSeconds(90);
                }

                if ((Players.Count == 1) && TimeToWaitForOpponents - DateTime.UtcNow.AddSeconds(2) <= TimeSpan.Zero)
                {
                    Reset();
                }
            }
        }

        public void UpdateInfo(Player player, string dataReceived)
        {
            //received the name, if first time connected and progress infos
            var nameAndInfo = dataReceived.Split('$');
            var infos = nameAndInfo.FirstOrDefault()?.Split('&');
            player.Name = nameAndInfo.LastOrDefault();
            player.FirstTimeConnecting = Convert.ToBoolean(infos[2]);
            var wpmProgress = int.Parse(infos[0]);
            var completedTextPercentage = int.Parse(infos[1]);
            player.UpdateProgress(wpmProgress, completedTextPercentage);

            Console.WriteLine("Name: " + player.Name + ", First time connecting: " + player.FirstTimeConnecting + ", WPMprogress: " + player.WPMProgress + ", completed text: " + completedTextPercentage);
        }
        private IMessage GetGameStatus(Player player)
        {
            return new OpponentsMessage(Players, GameStartingTime, GameEndingTime, player.Name, player.Finnished, player.Place);
        }

        private IMessage GameMessage()
        {
            return new GameMessage(CompetitionText, TimeToWaitForOpponents, GameStartingTime, GameEndingTime);
        }
    }
}