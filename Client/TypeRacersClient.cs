using Common;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace TypeRacers.Client
{
    public class TypeRacersClient
    {
        private readonly GameInfo gameInfo;
        private readonly Player player;

        public TypeRacersClient(Player player)
        {
            this.player = player;
            gameInfo = new GameInfo();
            player.SetPlayroom(gameInfo);
        }

        public void StartCommunication()
        {
            //player.Write contains networkclient.BeginWrite()
            IMessage toSend = new PlayerMessage(player.WPMProgress, player.CompletedTextPercentage, player.Name, player.FirstTimeConnecting, player.Restarting, player.Removed);
            player.BeginWriteMessage(toSend, WriteSuccessCallback, FailedCallback);

            void ReadSuccessCallback(IMessage receivedMessage)
            {
                var message = (ReceivedMessage)receivedMessage;

                var receivedData = message.Data;

                if (!string.IsNullOrEmpty(receivedData))
                {
                    IMessage toSend = ProcessReceivedResults(receivedData, player);
                    player.BeginWriteMessage(toSend, WriteSuccessCallback, FailedCallback);
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

        //private void ReadSuccessCallback(IMessage receivedMessage)
        //{
        //    var message = (ReceivedMessage)receivedMessage;
        //    var receivedData = message.Data;

        //    if (!string.IsNullOrEmpty(receivedData))
        //    {
        //        Debug.WriteLine("Received data: " + receivedData);
        //        ProcessReceivedResults(receivedData);
        //    }
        //    else
        //    {
        //        Debug.WriteLine("Nothing received");

        //    }

        //    SendInfoToServer();
        //}
        //private void WriteSuccessCallback()
        //{
        //    player.BeginReadMessage(ReadSuccessCallback, FailedCallback);

        //}
        //private void FailedCallback(Exception ex)
        //{
        //    Debug.WriteLine(ex.Message);
        //}
        //public void SendInfoToServer()
        //{
        //    ////player.Write contains networkclient.BeginWrite()
        //    //var toSend = new PlayerMessage(player.WPMProgress, player.CompletedTextPercentage, player.Name, player.FirstTimeConnecting, player.Restarting, player.Removed);
        //    player.BeginWriteMessage(toSend, WriteSuccessCallback, FailedCallback);
        //    gameInfo.OnOpponentsChanged(gameInfo.Players);
        //    Thread.Sleep(1000);
        //}
        private IMessage ProcessReceivedResults(string receivedData, Player player)
        {
            if (player.FirstTimeConnecting || player.Restarting)
            {
                gameInfo.SetGameInfo(receivedData);
                player.FirstTimeConnecting = false;
                player.Restarting = false;
            }
            else
            {
                SetGameStatus(receivedData);
            }

            //player.Write contains networkclient.BeginWrite()
            IMessage toSend = new PlayerMessage(player.WPMProgress, player.CompletedTextPercentage, player.Name, player.FirstTimeConnecting, player.Restarting, player.Removed);
            gameInfo.OnOpponentsChanged(gameInfo.Players);
            Thread.Sleep(1000);
            return toSend;
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
                    player.Finnished = Convert.ToBoolean(rank.FirstOrDefault()?.Substring(1));
                    player.Place = int.Parse(rank.LastOrDefault());
                    infos.Remove(i);
                    break;
                }
            }

            gameInfo.SetOpponentsAndTimers(infos);
        }
    }
}