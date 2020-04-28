using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Common
{
    public class Player
    {
        private string receivedData = string.Empty;
        readonly byte[] buffer = new byte[1024];

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


        //public void TrySetRank()
        //{
        //    if (CompletedTextPercentage == 100 && !Finnished)
        //    {
        //        Finnished = true;
        //        Place = Playroom.Place++;
        //    }
        //}
        //public bool CheckIfTriesToRestart()
        //{
        //    return Name.Contains("_restart");
        //}

        //public bool CheckIfLeft()
        //{
        //    if (Name?.Contains("_removed") == true)
        //    {
        //        Playroom.Leave(Name);
        //        NetworkClient.Dispose();
        //        return true;
        //    }

        //    return false;
        //}

        //synchronous write   
        public void Write(IMessage message)
        {
            NetworkClient.Write(message);
        }

        //synchronous read
        public string Read()
        {
            return NetworkClient.Read(buffer, 0, buffer.Length);
        }

        //asynchronous write
        public void BeginWriteMessage(IMessage message, Action successCallback, Action<Exception> failedCallback)
        {
            NetworkClient.BeginWrite(message, successCallback, failedCallback);
        }
        //asynchronous read
        public void BeginReadMessage(Action<IMessage> successCallback, Action<Exception> failedCallback)
        {
            NetworkClient.BeginRead(buffer, 0, buffer.Length, ReadCallback, failedCallback);

            void ReadCallback(int bytesRead)
            {
                if (bytesRead > 0)
                {
                    receivedData += Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    if (receivedData.Contains("#"))
                    {
                        // All the data has been read from the stream
                        receivedData = receivedData.Remove(receivedData.Length - 1);

                        //receivedMessage implements IMessage
                        ReceivedMessage receivedMessage = new ReceivedMessage(receivedData);

                        successCallback(receivedMessage);
                    }
                    else
                    {
                        // Not all data received. Get more.
                        NetworkClient.BeginRead(buffer, 0, buffer.Length, ReadCallback, failedCallback);
                    }
                }
                else
                {
                    NetworkClient.Dispose();
                }

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

    }
}