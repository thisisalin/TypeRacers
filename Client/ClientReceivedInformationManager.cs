using Common;
using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TypeRacers.Client
{
    public class ClientReceivedInformationManager : IRecievedInformationManager
    {
        private string receivedData;
        private byte[] buffer = new byte[1024];
        private INetworkClient networkClient;

        public ClientReceivedInformationManager(Player player, IPlayroom playroom)
        {
            Player = player;
            Playroom = playroom;
            GameInfo = (GameInfo)playroom;
            networkClient = Player.NetworkClient;
        }

        public Player Player { get; set; }
        public IPlayroom Playroom { get; set; }
        private GameInfo GameInfo { get; }

        public void StartCommunication()
        {
            if (networkClient.IsConnected() && !Player.Removed)
            {
                //first we send some info to the server
                SendInfoToServer();
            }
        }

        public void SendInfoToServer()
        {
            //player.Write contains networkclient.BeginWrite()
            Player.Write(new PlayerMessage(Player.WPMProgress, Player.CompletedTextPercentage, Player.Name, Player.FirstTimeConnecting, Player.Restarting, Player.Removed), WriteCallback);
            GameInfo.OnOpponentsChanged(GameInfo.Players);

            Thread.Sleep(1000);

            if (Player.Removed)
            {
                Player.Write(new PlayerMessage(Player.WPMProgress, Player.CompletedTextPercentage, Player.Name, Player.FirstTimeConnecting, Player.Restarting, Player.Removed), WriteCallback);
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
                networkStream?.EndWrite(ar);
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
                        //received all data
                        receivedData = receivedData.Remove(receivedData.Length - 1);
                        ProcessResults(receivedData);
                    }
                    else
                    {
                        // Not all data received. Get more.
                        networkStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(ReadCallback), networkStream);
                    }
                }
                //finally write to server again
                SendInfoToServer();
            }
            catch (IOException)
            {
                networkClient.Dispose();
            }
        }

        private void ProcessResults(string data)
        {
            if (Player.FirstTimeConnecting || Player.Restarting)
            {
                GameInfo.SetGameInfo(data);
                Player.FirstTimeConnecting = false;
                Player.Restarting = false;
            }
            else
            {
                SetGameStatus(data);
            }
        }

        private void SetGameStatus(string data)
        {
            var infos = data.Split('%').ToList();

            infos.Remove("#");
            foreach (var i in infos)
            {
                if (i.StartsWith("!"))
                {
                    var rank = i.Split('/');
                    Player.Finnished = Convert.ToBoolean(rank.FirstOrDefault().Substring(1));
                    Player.Place = int.Parse(rank.LastOrDefault());
                    infos.Remove(i);
                    break;
                }
            }
            GameInfo.SetOpponentsAndTimers(infos);
        }
    }
}