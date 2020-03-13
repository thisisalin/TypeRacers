﻿using System.ComponentModel;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace Common
{
    public class ReceivedMessage : IMessage, INotifyPropertyChanged
    {
        private readonly TcpClient tcpClient;

        public ReceivedMessage(string data)
        {
            Data = data;
        }

        public bool ClientDisconnected { get; private set; }

        public ReceivedMessage(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
        }

        private string Data { get; set; }

        public byte[] ToByteArray()
        {
            return default;
        }

        public void ReadMessage()
        {
            var stream = tcpClient.GetStream();
            byte[] buffer = new byte[1024];        
            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                Data = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                while (!Data.Contains("#"))
                {
                    bytesRead = stream.Read(buffer, 0, 1024);
                    Data += Encoding.ASCII.GetString(buffer, Data.Length, bytesRead);
                }
            }
            catch (System.IO.IOException ex)
            {
                tcpClient.Close();
                ClientDisconnected = true;
                TriggerPropertyChanged(nameof(ClientDisconnected));
                Debug.WriteLine(ex.Message);
            }
        }

        public string GetData()
        {
            ReadMessage();
            var data = Data?.Substring(0, Data.IndexOf('#')) ?? string.Empty;
            Data = Data?.Remove(0, data.Length + 1);
            return data ?? string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void TriggerPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}