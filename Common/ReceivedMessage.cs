using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Common
{
    public class ReceivedMessage : IMessage
    {
        private readonly TcpClient tcpClient;
        private byte[] buffer = new byte[1024];

        public ReceivedMessage(string data)
        {
            Data = data;
        }

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
            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                Data = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                if (!Data.Contains("#"))
                {
                    Data += Encoding.ASCII.GetString(buffer, Data.Length, bytesRead);
                }
            }
            catch (IOException)
            {
                tcpClient.Close();
            }
        }

        public void BeginReadMessage()
        {
            var stream = tcpClient.GetStream();
            try
            {
                stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(ReadCallback), stream);
            }
            catch (IOException)
            {
                tcpClient.Close();
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            NetworkStream networkStream = (NetworkStream)ar.AsyncState;
            int bytesRead = networkStream.EndRead(ar);

            if (bytesRead > 0)
            {
                Data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                if (Data != null && !Data.Contains("#"))
                {
                    Data += Encoding.ASCII.GetString(buffer, Data.Length, bytesRead);
                    networkStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(ReadCallback), networkStream);
                }
            }
        }

        public string GetData()
        {
            if (!string.IsNullOrEmpty(Data))
            {
                var data = Data?.Substring(0, Data.IndexOf('#')) ?? string.Empty;
                Data = Data?.Remove(0, data.Length + 1);
                return data;
            }

            return string.Empty;
        }
    }
}