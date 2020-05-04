using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Common
{
    public class Player
    {
        private string receivedData = string.Empty;
        private readonly byte[] buffer = new byte[1024];

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
            if (receivedData.Contains("#"))
            {
                receivedData = receivedData.Substring(0, receivedData.IndexOf('#'));
                successCallback(new ReceivedMessage(receivedData));
                //clearing the string
                receivedData = string.Empty;
                return;
            }
            NetworkClient.BeginRead(buffer, 0, buffer.Length, ReadCallback, failedCallback);
            void ReadCallback(int bytesRead)
            {
                if (bytesRead > 0)
                {
                    receivedData += Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    if (receivedData.Contains("#"))
                    {
                        receivedData = receivedData.Substring(0, receivedData.IndexOf('#'));
                        successCallback(new ReceivedMessage(receivedData));
                        receivedData = string.Empty;
                    }
                    else
                    {
                        // Not all data received. Get more.
                        NetworkClient.BeginRead(buffer, 0, buffer.Length, ReadCallback, failedCallback);
                    }
                }
                else
                {
                    failedCallback(new IOException());
                    NetworkClient.Dispose();
                }
            }
        }

        public void UpdateProgress(int wpmProgress, int completedTextPercentage)
        {
            WPMProgress = wpmProgress;
            CompletedTextPercentage = completedTextPercentage;
        }
    }
}