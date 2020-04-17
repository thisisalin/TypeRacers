using Common;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class ServerReceivedInformationManager : IRecievedInformationManager
    {
        private string receivedData;
        private byte[] buffer = new byte[1024];
        private INetworkClient networkClient;

        public ServerReceivedInformationManager(Player player, IPlayroom playroom)
        {
            Player = player;
            Playroom = playroom;
            networkClient = Player.NetworkClient;
        }

        public Player Player { get; set; }
        public IPlayroom Playroom { get; set; }

        public void StartCommunication()
        {
            if (networkClient.IsConnected())
            {
                //first we read data from clients
                Player.Read(ReadCallback, buffer);
            }
            else
            {
                throw new IOException();
            }
        }

        private void WriteCallback(IAsyncResult ar)
        {
            //when the write is done we reach here
            try
            {
                // Retrieve the stream from the state object.
                NetworkStream networkStream = (NetworkStream)ar.AsyncState;
                // Complete sending the data to the remote device.
                networkStream.EndWrite(ar);
                //after writing we read again, Player.Read contains networkclient.BeginRead()
                Player.Read(ReadCallback, buffer);
            }
            catch (IOException)
            {
                networkClient.Dispose();
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            //when the read is done we reach here
            try
            {
                NetworkStream networkStream = (NetworkStream)ar.AsyncState;
                int bytesRead = networkStream.EndRead(ar);

                if (bytesRead > 0)
                {
                    receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    if (receivedData.Contains("#"))
                    {
                        // All the data has been read from the stream
                        receivedData = receivedData.Remove(receivedData.Length - 1);
                        ProcessResults(receivedData);
                    }
                    else
                    {
                        // Not all data received. Get more.
                        networkStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(ReadCallback), networkStream);
                    }
                }
            }
            catch (IOException)
            {
                networkClient.Dispose();
            }
        }

        private void ProcessResults(string data)
        {
            Player.UpdateInfo(data);

            if (Player.CheckIfLeft())
            {
                return;
            }

            if (Player.FirstTimeConnecting || Player.CheckIfTriesToRestart())
            {
                SendGameInfo();
                Player.FirstTimeConnecting = false;
            }
            else
            {
                SendGamestatus();
            }
        }

        private void SendGamestatus()
        {
            Playroom.TrySetStartingTime();
            Player.TrySetRank();
            Player.Write(GetGameStatus(), WriteCallback);
            Console.WriteLine("sending opponents");
        }

        private void SendGameInfo()
        {
            Playroom.TrySetStartingTime();
            Player.Write(GameMessage(), WriteCallback);
            Console.WriteLine("sending game info");
        }

        private IMessage GetGameStatus()
        {
            return new OpponentsMessage(Playroom.Players, Playroom.GameStartingTime, Playroom.GameEndingTime, Player.Name, Player.Finnished, Player.Place);
        }

        private IMessage GameMessage()
        {
            return new GameMessage(Playroom.CompetitionText, Playroom.TimeToWaitForOpponents, Playroom.GameStartingTime, Playroom.GameEndingTime);
        }
    }
}