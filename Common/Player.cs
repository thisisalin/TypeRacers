using System;
using System.Linq;

namespace Common
{
    public class Player
    {
        public Player(INetworkClient tcpClient)
        {
            NetworkClient = tcpClient;
        }

        public bool FirstTimeConnecting { get; set; } = true;
        public string Name { get; set; }
        public int Place { get; set; }
        public bool Restarting { get; set; }
        public bool Removed { get; set; }
        public bool Finnished { get; set; }
        public int WPMProgress { get; set; }
        public int CompletedTextPercentage { get; set; }
        public IPlayroom Playroom { get; set; }
        public INetworkClient NetworkClient { get; }

        public void SetPlayroom(IPlayroom playroom)
        {
            Playroom = playroom;
        }

        public void UpdateProgress(int wpmProgress, int completedTextPercentage)
        {
            WPMProgress = wpmProgress;
            CompletedTextPercentage = completedTextPercentage;
        }

        public void TrySetRank()
        {
            if (CompletedTextPercentage == 100 && !Finnished)
            {
                Finnished = true;
                Place = Playroom.Place++;
            }
        }

        public void UpdateInfo(string dataReceived)
        {
            //received the name, if first time connected and progress infos
            var nameAndInfo = dataReceived.Split('$');
            var infos = nameAndInfo.FirstOrDefault()?.Split('&');
            Name = nameAndInfo.LastOrDefault();
            FirstTimeConnecting = Convert.ToBoolean(infos[2]);
            var wpmProgress = int.Parse(infos[0]);
            var completedTextPercentage = int.Parse(infos[1]);
            UpdateProgress(wpmProgress, completedTextPercentage);
            Console.WriteLine("Name: " + Name + ", First time connecting: " + FirstTimeConnecting + ", WPMprogress: " + WPMProgress + ", completed text: " + completedTextPercentage);
        }

        public bool CheckIfTriesToRestart()
        {
            return Name.Contains("_restart");
        }

        public bool CheckIfLeft()
        {
            if (Name?.Contains("_removed") == true)
            {
                Playroom.Leave(Name);
                NetworkClient.Dispose();
                return true;
            }

            return false;
        }
    }
}