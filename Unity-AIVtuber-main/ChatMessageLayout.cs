using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace AIVTuber
{
    public class ChatMessageLayout : MonoBehaviour
    {
        [Header("UI Components")]
        public TMP_Text messageText;
        public Image background;
        public RectTransform messageRect;

        [Header("Colors")]
        public Color userMessageColor = new Color(0.8f, 0.9f, 1f);
        public Color aiMessageColor = new Color(0.9f, 0.9f, 0.9f);
        public Color errorMessageColor = new Color(1f, 0.8f, 0.8f);

        [Header("Layout")]
        public float maxWidth = 800f;
        public float padding = 20f;

        public void SetMessage(string text, MessageType type)
        {
            messageText.text = text;

            // メッセージタイプに応じて色を設定
            switch (type)
            {
                case MessageType.User:
                    background.color = userMessageColor;
                    messageRect.anchorMin = new Vector2(1f, 0f);
                    messageRect.anchorMax = new Vector2(1f, 0f);
                    messageRect.pivot = new Vector2(1f, 0f);
                    break;
                case MessageType.AI:
                    background.color = aiMessageColor;
                    messageRect.anchorMin = new Vector2(0f, 0f);
                    messageRect.anchorMax = new Vector2(0f, 0f);
                    messageRect.pivot = new Vector2(0f, 0f);
                    break;
                case MessageType.Error:
                    background.color = errorMessageColor;
                    messageRect.anchorMin = new Vector2(0.5f, 0f);
                    messageRect.anchorMax = new Vector2(0.5f, 0f);
                    messageRect.pivot = new Vector2(0.5f, 0f);
                    break;
            }

            // テキストのサイズに合わせてレイアウトを調整
            LayoutRebuilder.ForceRebuildLayoutImmediate(messageRect);
            float width = Mathf.Min(messageText.preferredWidth + padding * 2, maxWidth);
            messageRect.sizeDelta = new Vector2(width, messageText.preferredHeight + padding * 2);
        }
    }
}
