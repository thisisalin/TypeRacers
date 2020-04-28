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
        //todo: change name to server
        private TcpListener server;
        private Rooms playrooms;
        private TcpClient client;

        // Thread signal.
        public static ManualResetEvent allDone = new ManualResetEvent(false);

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
            StartCommunication();
        }

        // Accept one client connection asynchronously.
        public void StartCommunication()
        {
            while (true)
            {
                // Set the event to nonsignaled state.
                allDone.Reset();

                // Start an asynchronous socket to listen for connections.
                Console.WriteLine("Waiting for a connection...");

                // Start to listen for connections from a client.
                server.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClientCallback), server);

                // Wait until a connection is made before continuing.
                allDone.WaitOne();
            }
        }

        // Process the client connection.
        public void AcceptTcpClientCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();

            TcpListener listener = (TcpListener)ar.AsyncState;
            client = listener.EndAcceptTcpClient(ar);
            Console.WriteLine("Accepted new client...");
            Player newConnectedClient = new Player(new NetworkClient(client));
            playrooms.AllocatePlayroom(newConnectedClient);
        }
    }
}