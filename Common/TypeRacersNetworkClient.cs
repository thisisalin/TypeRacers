using System;
using System.IO;
using System.Net.Sockets;

namespace Common
{
    public class TypeRacersNetworkClient : INetworkClient
    {
        // The real implementation
        private NetworkStream networkStream;

        private readonly TcpClient realTcpClient;

        public TypeRacersNetworkClient()
        {
            realTcpClient = new TcpClient();
        }

        public TypeRacersNetworkClient(TcpClient client)
        {
            realTcpClient = client;
        }

        public void Dispose()
        {
            networkStream.Dispose();
            realTcpClient.Close();
        }

        public bool IsConnected()
        {
            return realTcpClient.Connected;
        }

        public void Write(IMessage message, AsyncCallback callback)
        {
            try
            {
                networkStream = realTcpClient.GetStream();
                var toSend = message.ToByteArray();
                networkStream.BeginWrite(toSend, 0, toSend.Length, callback, networkStream);
            }
            catch (IOException)
            {
                Dispose();
            }
            catch (InvalidOperationException)
            {
                Dispose();
            }
        }

        public void Read(AsyncCallback callback, byte[] buffer)
        {
            try
            {
                networkStream = realTcpClient.GetStream();
                networkStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(callback), networkStream);
            }
            catch (IOException)
            {
                Dispose();
            }
            catch (InvalidOperationException)
            {
                Dispose();
            }
        }
    }
}