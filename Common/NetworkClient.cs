using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Common
{
    public class NetworkClient : INetworkClient
    {
        // The real implementation
        private NetworkStream networkStream;

        private readonly TcpClient realTcpClient;

        public NetworkClient()
        {
            realTcpClient = new TcpClient();
        }

        public NetworkClient(TcpClient client)
        {
            realTcpClient = client;
        }

        public void Dispose()
        {
            networkStream.Dispose();
            realTcpClient.Close();
        }

        //synchronous read
        public string Read(byte[] buffer, int offset, int size)
        {
            var dataReceived = string.Empty;

            try
            {
                networkStream = realTcpClient.GetStream();
                int bytesRead = networkStream.Read(buffer, offset, size);
                dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                while (!dataReceived.Contains("#"))
                {
                    dataReceived += Encoding.ASCII.GetString(buffer, dataReceived.Length, bytesRead);
                }

                return dataReceived;
            }
            catch (IOException)
            {
                Dispose();
            }

            return dataReceived;
        }

        //synchronous write
        public void Write(IMessage message)
        {
            try
            {
                networkStream = realTcpClient.GetStream();
                var toSend = message.ToByteArray();
                networkStream.Write(toSend, 0, toSend.Length);
            }
            catch (IOException)
            {
                Dispose();
            }
        }

        //asynchronous read
        public void BeginRead(byte[] buffer, int offset, int size, Action<int> succesCallback, Action<Exception> failedCallback)
        {
            networkStream = realTcpClient.GetStream();
            networkStream.BeginRead(buffer, offset, size, ReadCallback, networkStream);
            void ReadCallback(IAsyncResult ar)
            {
                NetworkStream networkStream = ar.AsyncState as NetworkStream;

                try
                {
                    int bytesRead = networkStream.EndRead(ar);
                    succesCallback(bytesRead);
                }
                catch (IOException x)
                {
                    failedCallback(x);
                }
            }
        }

        //asynchronous write
        public void BeginWrite(IMessage message, Action successCallback, Action<Exception> failedCallback)
        {
            var toSend = message.ToByteArray();
            networkStream = realTcpClient.GetStream();
            networkStream.BeginWrite(toSend, 0, toSend.Length, WriteCallback, networkStream);

            void WriteCallback(IAsyncResult ar)
            {
                try
                {
                    networkStream.EndWrite(ar);

                    successCallback();
                }
                catch (IOException x)
                {
                    failedCallback(x);
                }
            }
        }
    }
}