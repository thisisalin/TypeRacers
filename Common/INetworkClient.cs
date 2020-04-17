using System;

namespace Common
{
    public interface INetworkClient : IDisposable
    {
        void Write(IMessage message, AsyncCallback callback);

        void Read(AsyncCallback callback, byte[] buffer);

        bool IsConnected();
    }
}