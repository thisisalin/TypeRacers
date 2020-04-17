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

        public void UpdateProgress(int wpmProgress, int completedText)
        {
            WPMProgress = wpmProgress;
            CompletedTextPercentage = completedText;
        }

        public void TrySetRank()
        {
            if (CompletedTextPercentage == 100 && !Finnished)
            {
                Finnished = true;
                Place = Playroom.Place++;
            }
        }

        public void Read(AsyncCallback callback, byte[] buffer)
        {
            NetworkClient.Read(callback, buffer);
        }

        public void Write(IMessage message, AsyncCallback callback)
        {
            NetworkClient.Write(message, callback);
        }

        public void UpdateInfo(string data)
        {
            var nameAndInfo = data.Split('$');
            var infos = nameAndInfo.FirstOrDefault()?.Split('&');
            Name = nameAndInfo.LastOrDefault();

            Console.WriteLine(data);

            FirstTimeConnecting = Convert.ToBoolean(infos[2]);
            UpdateProgress(int.Parse(infos[0]), int.Parse(infos[1]));
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