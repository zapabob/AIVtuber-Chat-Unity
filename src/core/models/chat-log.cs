using UnityEngine;
using System;
using System.Collections.Generic;

namespace AIVtuberApp.Core.Models
{
    /// <summary>
    /// チャットログを表現するモデルクラス
    /// メッセージ履歴の管理とメタデータの保持を行う
    /// </summary>
    [System.Serializable]
    public class ChatLog
    {
        /// <summary>
        /// ログのユニークな識別子
        /// </summary>
        [SerializeField]
        private string logId;
        public string LogId 
        {
            get => logId;
            private set => logId = value;
        }

        /// <summary>
        /// チャットメッセージのコレクション
        /// </summary>
        [SerializeField]
        private List<ChatMessage> messages;
        public List<ChatMessage> Messages 
        {
            get => messages;
            private set => messages = value;
        }

        /// <summary>
        /// チャットセッションの開始時刻
        /// </summary>
        [SerializeField]
        private string startTime;
        public DateTime StartTime 
        {
            get => DateTime.Parse(startTime);
            private set => startTime = value.ToString("o");
        }

        /// <summary>
        /// チャットセッションの終了時刻
        /// </summary>
        [SerializeField]
        private string endTime;
        public DateTime? EndTime 
        {
            get => string.IsNullOrEmpty(endTime) ? null : (DateTime?)DateTime.Parse(endTime);
            private set => endTime = value?.ToString("o");
        }

        /// <summary>
        /// チャットセッションの参加者情報
        /// </summary>
        [SerializeField]
        private List<Participant> participants;
        public List<Participant> Participants 
        {
            get => participants;
            private set => participants = value;
        }

        /// <summary>
        /// チャットセッションのメタデータ
        /// </summary>
        [SerializeField]
        private SerializableDictionary metadata;
        public Dictionary<string, string> Metadata 
        {
            get => metadata.ToDictionary();
            private set => metadata = new SerializableDictionary(value);
        }

        /// <summary>
        /// コンストラクター
        /// </summary>
        public ChatLog()
        {
            logId = Guid.NewGuid().ToString();
            messages = new List<ChatMessage>();
            StartTime = DateTime.UtcNow;
            participants = new List<Participant>();
            metadata = new SerializableDictionary();
        }

        /// <summary>
        /// メッセージを追加するメソッド
        /// </summary>
        /// <param name="message">追加するメッセージ</param>
        public void AddMessage(ChatMessage message)
        {
            Messages.Add(message);
        }

        /// <summary>
        /// チャットセッションを終了するメソッド
        /// </summary>
        public void EndSession()
        {
            EndTime = DateTime.UtcNow;
        }

        /// <summary>
        /// メタデータを追加するメソッド
        /// </summary>
        /// <param name="key">メタデータのキー</param>
        /// <param name="value">メタデータの値</param>
        public void AddMetadata(string key, string value)
        {
            Metadata[key] = value;
        }
    }

    /// <summary>
    /// チャットメッセージを表現するクラス
    /// </summary>
    [System.Serializable]
    public class ChatMessage
    {
        /// <summary>
        /// メッセージのユニークな識別子
        /// </summary>
        [SerializeField]
        private string messageId;
        public string MessageId 
        {
            get => messageId;
            private set => messageId = value;
        }

        /// <summary>
        /// メッセージの送信者
        /// </summary>
        [SerializeField]
        private Participant sender;
        public Participant Sender 
        {
            get => sender;
            set => sender = value;
        }

        /// <summary>
        /// メッセージの内容
        /// </summary>
        [SerializeField]
        private string content;
        public string Content 
        {
            get => content;
            set => content = value;
        }

        /// <summary>
        /// メッセージの送信時刻
        /// </summary>
        [SerializeField]
        private string timestamp;
        public DateTime Timestamp 
        {
            get => DateTime.Parse(timestamp);
            private set => timestamp = value.ToString("o");
        }

        /// <summary>
        /// メッセージの種類（テキスト、音声など）
        /// </summary>
        [SerializeField]
        private MessageType type;
        public MessageType Type 
        {
            get => type;
            set => type = value;
        }

        /// <summary>
        /// コンストラクター
        /// </summary>
        public ChatMessage()
        {
            messageId = Guid.NewGuid().ToString();
            Timestamp = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// チャット参加者を表現するクラス
    /// </summary>
    [System.Serializable]
    public class Participant
    {
        /// <summary>
        /// 参加者のユニークな識別子
        /// </summary>
        [SerializeField]
        private string participantId;
        public string ParticipantId 
        {
            get => participantId;
            private set => participantId = value;
        }

        /// <summary>
        /// 参加者の名前
        /// </summary>
        [SerializeField]
        private string name;
        public string Name 
        {
            get => name;
            set => name = value;
        }

        /// <summary>
        /// 参加者の種類（ユーザー、AIなど）
        /// </summary>
        [SerializeField]
        private ParticipantType type;
        public ParticipantType Type 
        {
            get => type;
            set => type = value;
        }

        public Participant()
        {
            participantId = Guid.NewGuid().ToString();
        }
    }

    /// <summary>
    /// メッセージの種類を定義する列挙型
    /// </summary>
    public enum MessageType
    {
        Text,
        Voice,
        Image,
        System
    }

    /// <summary>
    /// 参加者の種類を定義する列挙型
    /// </summary>
    public enum ParticipantType
    {
        User,
        AIVtuber,
        System
    }

    /// <summary>
    /// Unity用のシリアライズ可能なディクショナリ
    /// </summary>
    [System.Serializable]
    public class SerializableDictionary
    {
        [SerializeField]
        private List<string> keys = new List<string>();
        [SerializeField]
        private List<string> values = new List<string>();

        public SerializableDictionary()
        {
        }

        public SerializableDictionary(Dictionary<string, string> dict)
        {
            foreach (var kvp in dict)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
        }

        public Dictionary<string, string> ToDictionary()
        {
            var dict = new Dictionary<string, string>();
            for (int i = 0; i < keys.Count; i++)
            {
                dict[keys[i]] = values[i];
            }
            return dict;
        }
    }
}