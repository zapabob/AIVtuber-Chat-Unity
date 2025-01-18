using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

namespace AIVTuber
{
    public class ChatUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_InputField chatInput;
        [SerializeField] private Button sendButton;
        [SerializeField] private ScrollRect chatScrollRect;
        [SerializeField] private Transform messageContainer;
        [SerializeField] private GameObject messagePrefab;
        
        [Header("Components")]
        [SerializeField] private AIVTuberController aiVTuberController;

        private List<ChatMessage> chatHistory = new List<ChatMessage>();
        private const int MaxMessages = 100;

        private void Start()
        {
            if (!aiVTuberController)
            {
                aiVTuberController = FindObjectOfType<AIVTuberController>();
            }

            InitializeUI();
        }

        private void InitializeUI()
        {
            sendButton.onClick.AddListener(SendMessage);
            chatInput.onSubmit.AddListener(_ => SendMessage());

            // エンターキーでメッセージを送信
            chatInput.onEndEdit.AddListener(text =>
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    SendMessage();
                }
            });
        }

        private async void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(chatInput.text)) return;

            string message = chatInput.text;
            chatInput.text = "";
            
            // ユーザーメッセージを表示
            AddMessage(message, true);

            try
            {
                // AIVTuberからの応答を取得
                string response = await aiVTuberController.HandleMessage(message);
                if (!string.IsNullOrEmpty(response))
                {
                    // AIの応答を表示
                    AddMessage(response, false);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error getting AI response: {ex.Message}");
                AddMessage("申し訳ありません。エラーが発生しました。", false);
            }
        }

        private void AddMessage(string text, bool isUser)
        {
            if (chatHistory.Count >= MaxMessages)
            {
                Destroy(chatHistory[0].gameObject);
                chatHistory.RemoveAt(0);
            }

            GameObject messageObj = Instantiate(messagePrefab, messageContainer);
            var messageComponent = messageObj.GetComponent<ChatMessage>();
            if (messageComponent != null)
            {
                messageComponent.SetMessage(text, isUser);
                chatHistory.Add(messageComponent);
            }

            // 自動スクロール
            Canvas.ForceUpdateCanvases();
            chatScrollRect.normalizedPosition = new Vector2(0, 0);
        }
    }

    [Serializable]
    public class ChatMessage : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Image background;
        [SerializeField] private Color userColor = new Color(0.8f, 0.8f, 1f);
        [SerializeField] private Color aiColor = new Color(1f, 0.8f, 0.8f);

        public void SetMessage(string text, bool isUser)
        {
            messageText.text = text;
            background.color = isUser ? userColor : aiColor;
        }
    }
}
