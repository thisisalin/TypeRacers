using Common;
using Server;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TypeRacers.Server
{
    internal class ServerSetup
    {
        private TcpListener server;
        private Rooms playrooms;
        private TcpClient client;
        // Thread signal.
        public static ManualResetEvent tcpClientConnected = new ManualResetEvent(false);
        public void Setup()
        {
            server = new TcpListener(IPAddress.IPv6Any, 80);
            server.Server.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
            playrooms = new Rooms();

            try
            {
                server.Start();
            }
            catch (Exception)
            {
                throw;
            }

            Console.WriteLine("Server started");
            CommunicationSetup();
        }

        private void CommunicationSetup()
        {
            while (true)
            {
                DoBeginAcceptTcpClient();
            }
        }

        // Accept one client connection asynchronously.
        public void DoBeginAcceptTcpClient()
        {
            // Start to listen for connections from a client.
            server.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), server);
        }

        // Process the client connection.
        public void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;

            try
            {
                client = listener.EndAcceptTcpClient(ar);
                Console.WriteLine("Accepted new client...");
                Player newConnectedClient = new Player(new TypeRacersNetworkClient(client));
                playrooms.AllocatePlayroom(newConnectedClient);
            }

            catch (SocketException ex)
            {
                Console.WriteLine("Error accepting TCP connection: {0}", ex.Message);
                return;
            }
            //keep listening
            listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);
        }
    }
}