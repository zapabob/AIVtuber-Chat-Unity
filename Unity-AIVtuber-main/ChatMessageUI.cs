using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color userMessageColor = new Color(0.2f, 0.6f, 1f);
    [SerializeField] private Color aiMessageColor = new Color(0.9f, 0.9f, 0.9f);
    [SerializeField] private RectTransform rectTransform;

    public void SetMessage(ChatMessage message)
    {
        messageText.text = message.text;
        backgroundImage.color = message.isUserMessage ? userMessageColor : aiMessageColor;
        
        // メッセージの配置を設定
        if (message.isUserMessage)
        {
            rectTransform.anchorMin = new Vector2(1, 0);
            rectTransform.anchorMax = new Vector2(1, 0);
            rectTransform.pivot = new Vector2(1, 0);
        }
        else
        {
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.pivot = new Vector2(0, 0);
        }
    }
}