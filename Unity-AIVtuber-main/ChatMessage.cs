using System;

namespace AIVTuber
{
    public enum MessageType
    {
        User,
        AI,
        Error
    }

    [Serializable]
    public class ChatMessage
    {
        public string Text;
        public MessageType Type;
        public DateTime Timestamp;

        public ChatMessage()
        {
            Timestamp = DateTime.Now;
        }
    }
}
