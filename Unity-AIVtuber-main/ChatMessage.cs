using UnityEngine;
using System;

[Serializable]
public class ChatMessage
{
    public string text;
    public bool isUserMessage;
    public DateTime timestamp;

    public ChatMessage(string text, bool isUserMessage)
    {
        this.text = text;
        this.isUserMessage = isUserMessage;
        this.timestamp = DateTime.Now;
    }
}