using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Common
{
    public class ReceivedMessage : IMessage
    {

        public ReceivedMessage(string data)
        {
            Data = data;
        }

        public string Data { get; set; }
        public byte[] ToByteArray()
        {
            return Encoding.ASCII.GetBytes(Data);
        }
    }
}