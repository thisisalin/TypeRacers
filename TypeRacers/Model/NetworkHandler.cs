using Common;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using TypeRacers.Client;

namespace TypeRacers
{
    //a class that handles the messages to and from the network
    public class NetworkHandler
    {
        private readonly Player player;
        private readonly TcpClient tcpClient;
        private readonly TypeRacersClient typeRacersClient;

        // ManualResetEvent instances signal completion.
        private static readonly ManualResetEvent connectDone = new ManualResetEvent(false);

        public NetworkHandler(string userName)
        {
            tcpClient = new TcpClient();
            player = new Player(new TypeRacersNetworkClient(tcpClient))
            {
                Name = userName
            };
            typeRacersClient = new TypeRacersClient(player);
        }

        internal GameInfo GameModel()
        {
            return (GameInfo)player.Playroom;
        }

        internal Player PlayerModel()
        {
            return player;
        }

        internal void StartCommunication()
        {
            try
            {
                // Connect to the remote endpoint.
                tcpClient.BeginConnect("localhost", 80, new AsyncCallback(ConnectCallback), tcpClient);
                connectDone.WaitOne();
                typeRacersClient.StartCommunication();
            }
            catch (SocketException ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                TcpClient client = (TcpClient)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                // Signal that the connection has been made.
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}