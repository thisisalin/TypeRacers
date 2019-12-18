﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TypeRacers.Server
{
    public class TypeRacersServer
    {
        static void Main()
        {
            //starting the server
            ServerSetup();
        }

        public static void ServerSetup()
        {
            IPAddress ip = Dns.GetHostEntry("localhost").AddressList[0];
            TcpListener server = new TcpListener(ip, 80);

            try
            {
                server.Start();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }

            Console.WriteLine("Server started");
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("New client accepted");
                try
                {
                    NetworkStream networkStream = client.GetStream();
                   
                    byte[] buffer = new byte[client.ReceiveBufferSize];

                    //---read incoming stream---
                    int bytesRead = networkStream.Read(buffer, 0, client.ReceiveBufferSize);

                    //recieving the progress
                    string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Client said: " + dataReceived);
                    client.Close();
                }
                catch (Exception)
                {
                    Console.WriteLine("Client disconnected");
                }
            }
        }
    }
}