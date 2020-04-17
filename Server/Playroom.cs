using Common;
using System;
using System.Collections.Generic;
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

        public bool Join(Player currentPlayer, IRecievedInformationManager informationManager)
        {
            if (GameHasStarted || Players.Count == 3)
            {
                return false;
            }

            if (!IsInPlayroom(currentPlayer.Name))
            {
                Players.Add(currentPlayer);
                currentPlayer.Playroom = this;
                informationManager.StartCommunication();

                return true;
            }

            return false;
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
    }
}