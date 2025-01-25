using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ChatUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button sendButton;
    [SerializeField] private Transform messageContainer;
    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private AIVTuberController aiVTuberController;

    [Header("Settings")]
    [SerializeField] private int maxMessages = 100;
    private List<GameObject> messageObjects = new List<GameObject>();

    private void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        // Sendボタンのイベントリスナーをセットアップ
        sendButton.onClick.AddListener(SendMessage);

        // Input Fieldのイベントリスナーをセットアップ
        inputField.onSubmit.AddListener(_ => SendMessage());

        // エンターキーでの送信を設定
        inputField.onEndEdit.AddListener(text => {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                SendMessage();
            }
        });
    }

    private async void SendMessage()
    {
        if (string.IsNullOrWhiteSpace(inputField.text)) return;

        string messageText = inputField.text;
        inputField.text = "";

        // ユーザーメッセージを表示
        AddMessage(new ChatMessage(messageText, true));

        if (aiVTuberController != null)
        {
            // AIの応答を取得
            string response = await aiVTuberController.HandleMessage(messageText);
            if (!string.IsNullOrEmpty(response))
            {
                // AIの応答を表示
                AddMessage(new ChatMessage(response, false));
            }
        }
    }

    private void AddMessage(ChatMessage message)
    {
        // メッセージ数の制限をチェック
        if (messageObjects.Count >= maxMessages)
        {
            Destroy(messageObjects[0]);
            messageObjects.RemoveAt(0);
        }

        // メッセージUIを生成
        GameObject messageObj = Instantiate(messagePrefab, messageContainer);
        var messageUI = messageObj.GetComponent<ChatMessageUI>();
        if (messageUI != null)
        {
            messageUI.SetMessage(message);
            messageObjects.Add(messageObj);
        }

        // スクロールを一番下に移動
        Canvas.ForceUpdateCanvases();
        scrollRect.normalizedPosition = new Vector2(0, 0);
    }

    private void OnDestroy()
    {
        // イベントリスナーをクリーンアップ
        sendButton.onClick.RemoveAllListeners();
        inputField.onSubmit.RemoveAllListeners();
        inputField.onEndEdit.RemoveAllListeners();
    }
}

