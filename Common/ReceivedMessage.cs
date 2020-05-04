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