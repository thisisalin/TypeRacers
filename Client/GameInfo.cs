using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace TypeRacers.Client
{
    public class GameInfo : IPlayroom
    {
        public delegate void OpponentsChangedEventHandler(List<Player> updatedOpponents);

        public event OpponentsChangedEventHandler OpponentsChanged;

        public string CompetitionText { get; set; }
        public List<Player> Players { get; set; } = new List<Player>();
        public DateTime GameStartingTime { get; set; }
        public DateTime GameEndingTime { get; set; }
        public DateTime TimeToWaitForOpponents { get; set; }
        public int Place { get; set; }
        public bool GameInfoIsSet { get; set; }
        public bool ConnectionLost { get; set; }

        public Player GetPlayer(string name)
        {
            return Players.Find(p => p.Name.Equals(name));
        }

        public bool Join(Player player, IRecievedInformationManager informationManager)
        {
            Players.Add(player);
            return true;
        }

        public void SetGameInfo(string data)
        {
            CompetitionText = data.Substring(0, data.IndexOf('$'));
            var times = data.Substring(data.IndexOf('%') + 1);
            var gameTimers = times.Split('*');
            TimeToWaitForOpponents = DateTime.Parse(gameTimers.FirstOrDefault());
            var startAndEndTimes = gameTimers.LastOrDefault().Split('+');
            GameStartingTime = DateTime.Parse(startAndEndTimes.FirstOrDefault());
            GameEndingTime = DateTime.Parse(startAndEndTimes.LastOrDefault());
            GameInfoIsSet = true;
        }

        public void SetOpponentsAndTimers(List<string> data)
        {
            foreach (var v in data)
            {
                if (v.FirstOrDefault().Equals('*'))
                {
                    var times = v.Substring(1).Split('+');
                    GameStartingTime = DateTime.Parse(times.FirstOrDefault());
                    GameEndingTime = DateTime.Parse(times.LastOrDefault());
                }
                else
                {
                    var nameAndInfos = v.Split(':');
                    SetOpponents(nameAndInfos);
                }
            }
        }

        public void SubscribeToSearchingOpponents(Action<List<Player>> updateOpponents)
        {
            OpponentsChanged = new OpponentsChangedEventHandler(updateOpponents);
            OpponentsChanged += new OpponentsChangedEventHandler(OpponentsChanged);
        }

        private void SetOpponents(string[] recievedData)
        {
            var receivedName = recievedData.FirstOrDefault();
            var playerInfo = recievedData.LastOrDefault()?.Split('&');

            if (string.IsNullOrEmpty(receivedName))
            {
                return;
            }
            var player = GetPlayer(receivedName);

            if (player == default)
            {
                var tcpClient = new TcpClient();
                player = new Player(new TypeRacersNetworkClient(tcpClient))
                {
                    Name = receivedName
                };
                var informationManager = new ClientReceivedInformationManager(player, this);
                Join(player, informationManager);
            }
            else
            {
                var wpmProgress = int.Parse(playerInfo[0]);
                var completedTextPercentage = int.Parse(playerInfo[1]);

                player.UpdateProgress(wpmProgress, completedTextPercentage);
                player.Finnished = Convert.ToBoolean(playerInfo[2]);
                player.Place = int.Parse(playerInfo[3]);
            }
        }

        public void OnOpponentsChanged(List<Player> opponents)
        {
            if (opponents != null && OpponentsChanged != null)
            {
                OpponentsChanged(opponents);
            }
        }

        public bool Leave(string name)
        {
            throw new NotImplementedException();
        }

        public void TrySetStartingTime()
        {
            throw new NotImplementedException();
        }
    }
}