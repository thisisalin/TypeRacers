using System;

namespace Common
{
    public interface INetworkClient : IDisposable
    {
        void Write(IMessage message);
        string Read(byte[] buffer, int offset, int size);
        void BeginRead(byte[] buffer, int offset, int count, Action<int> succesCallback, Action<Exception> failedCallback);
        void BeginWrite(IMessage message, Action succesCallback, Action<Exception> failedCallback);

    }
}