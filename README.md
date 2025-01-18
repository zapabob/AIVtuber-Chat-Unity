# AIVTuber Controller

Unity用のAIVTuberコントローラーシステムです。VRMモデルとLLM（大規模言語モデル）を組み合わせて、インタラクティブなバーチャルYouTuberを作成できます。

## 主な機能

- 複数のLLMサービス対応（Dify、ローカルLLM、Maria）
- VOICEVOXを使用した音声合成
- VRMモデルの表情制御
- 自動まばたき機能
- エラーハンドリングとリトライ機能

## 必要要件

- Unity 2021.3以降
- UniVRM 10.0以降
- VOICEVOX（音声合成用）

## セットアップ

1. VRMモデルを`StreamingAssets`フォルダに配置
2. AIVTuberControllerコンポーネントをシーンに追加
3. 必要な設定（LLMサービス、VOICEVOX）を構成

## 設定項目

### LLMサービス設定
- エンドポイントURL
- APIキー
- システムプロンプト
- 各種パラメータ（温度、最大トークン数など）

### 音声設定
- VOICEVOXエンドポイント
- 話者ID
- 音声パラメータ（速度、ピッチ、イントネーション）

### VRMモデル設定
- モデルパス
- 表情マッピング

## エラーハンドリング

システムは以下のタイプのエラーを処理します：
- 設定エラー
- AIレスポンスエラー
- VRMモデルエラー
- 音声合成エラー

## ライセンス

MITライセンス

## 注意事項

- VOICEVOXは別途インストールが必要です
- LLMサービスは別途APIキーの取得が必要な場合があります
- VRMモデルの利用規約を確認してください

## 使用例

### 基本的な使用方法
```csharp
// AIVTuberControllerの初期化
var controller = gameObject.AddComponent<AIVTuberController>();
controller.settings = new AIVTuberSettings {
    LLMServiceType = LLMServiceType.LocalLLM,
    SystemPrompt = "あなたはフレンドリーなバーチャルYouTuberです。"
};

// メッセージの処理
await controller.HandleMessage("こんにちは！");
```

### カスタム感情表現の追加
```csharp
// 感情表現のマッピングをカスタマイズ
emotionToExpression.Add("excited", ExpressionKey.Happy);
emotionToExpression.Add("thoughtful", ExpressionKey.Neutral);
```

### 音声パラメータの調整
```csharp
// 音声パラメータのカスタマイズ
config.VoiceSpeed = 1.2f;      // 話速
config.VoicePitch = 0.2f;      // ピッチ
config.VoiceIntonation = 1.1f; // イントネーション
```

## 詳細な使用方法ガイド

### 1. プロジェクトのセットアップ

#### 1.1 Unity環境の準備
1. Unity Hub で新規プロジェクトを作成
   - Unity 2021.3以降を選択
   - 3Dテンプレートを使用
   - プロジェクト名を設定

2. 必要なパッケージのインストール
   ```
   - UniVRM 10.0（Package Managerから）
   - UniTask（Package Managerから）
   - JSON.NET（Package Managerから）
   ```

#### 1.2 フォルダ構造の設定
```
Assets/
  ├── StreamingAssets/
  │   ├── Models/          # VRMモデルファイル
  │   └── LLMModels/      # ローカルLLMモデル
  ├── Scripts/
  │   ├── Core/           # コア機能
  │   ├── Services/       # LLMサービス
  │   ├── UI/            # UI関連
  │   └── Utils/         # ユーティリティ
  └── Scenes/
      └── Main.scene     # メインシーン
```

#### 1.3 外部サービスの準備
1. VOICEVOXのセットアップ
   ```bash
   # VOICEVOXのインストール
   1. https://voicevox.hiroshiba.jp/ からダウンロード
   2. インストーラーを実行
   3. デフォルトポート(50021)で起動
   ```

2. LLMサービスの準備
   ```bash
   # Difyの場合
   1. https://dify.ai/ でアカウント作成
   2. APIキーを取得
   
   # ローカルLLMの場合
   1. モデルをダウンロード
   2. KoboldCPP/llama.cppを設定
   3. APIサーバーを起動
   ```

### 2. シーンの設定

#### 2.1 基本シーン構成
```
Scene
├── Main Camera
├── Directional Light
├── AIVTuberController
│   ├── VRMModelLoader
│   ├── ErrorNotification
│   └── ChatUI
└── EventSystem
```

#### 2.2 コンポーネントの設定手順
1. AIVTuberControllerの設定
   ```csharp
   // インスペクターでの設定
   - VRM Model: VRMモデルのプレハブをアタッチ
   - Settings: AIVTuberSettingsアセットを作成してアタッチ
   - Error Display: ErrorNotificationをアタッチ
   ```

2. カメラの設定
   ```
   Position: (0, 1.5, -2)
   Rotation: (0, 180, 0)
   Field of View: 60
   ```

### 3. 運用シナリオ別の設定例

#### 3.1 ゲーム実況配信の場合
```csharp
// システムプロンプトの設定
settings.SystemPrompt = @"あなたはゲーム実況が得意なバーチャルYouTuberです。
以下の点に注意して応答してください：
- ゲームの状況を分かりやすく実況
- プレイヤーの行動に対して適切なリアクション
- 視聴者からの質問に丁寧に回答
- ゲームの攻略情報や裏技の共有
- 盛り上がりポイントでの感情表現を豊かに

感情表現のガイドライン：
[happy] - 良いプレイや成功時
[excited] - 重要なイベントや驚きの展開
[thoughtful] - 戦略を考える場面
[sad] - 失敗やゲームオーバー時
[angry] - 悔しい場面や激しい戦闘時";

// 音声設定
config.VoiceSettings = new VoiceSettings {
    BaseSpeed = 1.2f,  // 実況向けにやや早め
    ExcitementSpeedMultiplier = 1.3f,  // 盛り上がり時は更に早く
    PitchRange = new Vector2(-0.2f, 0.4f),  // 感情表現の幅を広く
    VolumeRange = new Vector2(0.8f, 1.2f)   // メリハリのある音量
};
```

#### 3.2 雑談配信の場合
```csharp
// システムプロンプトの設定
settings.SystemPrompt = @"あなたは視聴者と楽しく雑談するバーチャルYouTuberです。
以下の特徴を持って応答してください：
- 親しみやすい口調で会話
- 視聴者のコメントに共感的な反応
- 時事ネタやトレンドについて話題提供
- 適度な冗談や軽い話題も取り入れる
- 個人情報や機密情報には触れない

感情表現のガイドライン：
[happy] - 楽しい話題や共感時
[surprised] - 意外な情報や驚きの展開
[thoughtful] - 深い話題や考察時
[sad] - 共感や励まし
[relaxed] - 通常の会話時";

// 音声設定
config.VoiceSettings = new VoiceSettings {
    BaseSpeed = 1.0f,  // 標準的な速度
    EmotionSpeedMultiplier = 1.1f,  // 感情による変化を抑えめに
    PitchRange = new Vector2(-0.1f, 0.2f),  // 自然な抑揚
    VolumeRange = new Vector2(0.9f, 1.1f)   // 安定した音量
};
```

### 4. 高度な設定とカスタマイズ

#### 4.1 表情制御の詳細設定
```csharp
// 表情ブレンド設定
public class ExpressionBlendSettings
{
    public float BlendDuration = 0.3f;    // ブレンド時間
    public float HoldDuration = 2.0f;     // 表情維持時間
    public float ReturnDuration = 0.5f;   // 戻り時間
    
    // 表情の強さの設定
    public Dictionary<string, float> ExpressionStrength = new Dictionary<string, float>
    {
        ["happy"] = 1.0f,
        ["sad"] = 0.7f,
        ["angry"] = 0.8f,
        ["surprised"] = 1.0f,
        ["relaxed"] = 0.5f
    };
}

// 実装例
public async Task ApplyExpressionWithBlend(string emotion)
{
    var settings = new ExpressionBlendSettings();
    float strength = settings.ExpressionStrength[emotion];
    
    // 現在の表情からブレンド
    float time = 0;
    while (time < settings.BlendDuration)
    {
        float t = time / settings.BlendDuration;
        float weight = Mathf.Lerp(0, strength, t);
        vrmInstance.Runtime.Expression.SetWeight(emotionToExpression[emotion], weight);
        time += Time.deltaTime;
        await Task.Yield();
    }
    
    // 表情を維持
    await Task.Delay((int)(settings.HoldDuration * 1000));
    
    // 徐々に戻す
    time = 0;
    while (time < settings.ReturnDuration)
    {
        float t = time / settings.ReturnDuration;
        float weight = Mathf.Lerp(strength, 0, t);
        vrmInstance.Runtime.Expression.SetWeight(emotionToExpression[emotion], weight);
        time += Time.deltaTime;
        await Task.Yield();
    }
}
```

#### 4.2 音声合成の詳細設定
```csharp
// 音声パラメータの動的調整
public class DynamicVoiceSettings
{
    // 基本パラメータ
    public float BaseSpeed = 1.0f;
    public float BasePitch = 0.0f;
    public float BaseIntonation = 1.0f;
    
    // 文脈による調整
    public class ContextualAdjustment
    {
        public float SpeedMultiplier = 1.0f;
        public float PitchOffset = 0.0f;
        public float IntonationMultiplier = 1.0f;
    }
    
    // 文脈別の設定
    public Dictionary<string, ContextualAdjustment> ContextSettings = new Dictionary<string, ContextualAdjustment>
    {
        ["question"] = new ContextualAdjustment 
        { 
            SpeedMultiplier = 0.9f,
            PitchOffset = 0.1f,
            IntonationMultiplier = 1.2f
        },
        ["excitement"] = new ContextualAdjustment
        {
            SpeedMultiplier = 1.2f,
            PitchOffset = 0.2f,
            IntonationMultiplier = 1.3f
        }
    };
}

// 実装例
public void AdjustVoiceForContext(string context)
{
    var settings = new DynamicVoiceSettings();
    if (settings.ContextSettings.TryGetValue(context, out var adjustment))
    {
        config.VoiceSpeed = settings.BaseSpeed * adjustment.SpeedMultiplier;
        config.VoicePitch = settings.BasePitch + adjustment.PitchOffset;
        config.VoiceIntonation = settings.BaseIntonation * adjustment.IntonationMultiplier;
    }
}
```

### 5. パフォーマンスチューニング

#### 5.1 メモリ使用量の最適化
```csharp
public class MemoryOptimizationSettings
{
    // キャッシュ設定
    public int MaxVoiceCacheSize = 100;    // 音声キャッシュの最大数
    public int MaxResponseCacheSize = 50;   // 応答キャッシュの最大数
    public float CacheCleanupInterval = 300f; // クリーンアップ間隔（秒）
    
    // リソース解放のしきい値
    public float UnusedResourceTimeout = 600f;  // 未使用リソースの保持時間
    public float MemoryThreshold = 0.8f;        // メモリ使用率のしきい値
}

// 実装例
public class ResourceManager
{
    private MemoryOptimizationSettings settings;
    private Dictionary<string, CachedResource> resourceCache;
    
    public void Initialize()
    {
        settings = new MemoryOptimizationSettings();
        StartCoroutine(PeriodicCleanup());
    }
    
    private IEnumerator PeriodicCleanup()
    {
        while (true)
        {
            CleanupUnusedResources();
            yield return new WaitForSeconds(settings.CacheCleanupInterval);
        }
    }
    
    private void CleanupUnusedResources()
    {
        var currentTime = Time.time;
        foreach (var resource in resourceCache.Values)
        {
            if (currentTime - resource.LastAccessTime > settings.UnusedResourceTimeout)
            {
                resource.Dispose();
            }
        }
    }
}
```

### 6. トラブルシューティングガイド

#### 6.1 一般的な問題の診断と解決
```csharp
public class DiagnosticTools
{
    // システム状態の診断
    public static SystemDiagnostics GetSystemStatus()
    {
        return new SystemDiagnostics
        {
            MemoryUsage = SystemInfo.systemMemorySize,
            CpuUsage = GetCPUUsage(),
            DiskSpace = GetAvailableDiskSpace(),
            NetworkLatency = CheckNetworkLatency()
        };
    }
    
    // 接続テスト
    public static async Task<bool> TestConnections()
    {
        bool voicevoxOk = await TestVoicevoxConnection();
        bool llmOk = await TestLLMConnection();
        return voicevoxOk && llmOk;
    }
    
    // ログ収集
    public static void CollectLogs(string outputPath)
    {
        // システムログ
        // パフォーマンスデータ
        // エラーレポート
        // の収集と保存
    }
}
```

#### 6.2 エラーコードと対処方法
```
エラーコード一覧：

VRM001: VRMモデルのロードエラー
- ファイルパスの確認
- モデルの互換性確認
- ファイルの破損チェック

LLM001: LLMサービス接続エラー
- APIキーの確認
- ネットワーク接続の確認
- エンドポイントURLの確認

VOX001: VOICEVOX接続エラー
- サービスの起動確認
- ポート番号の確認
- ファイアウォール設定の確認

MEM001: メモリ不足エラー
- 未使用リソースの解放
- キャッシュのクリア
- システムリソースの確認
```

## 開発者向け情報

### アーキテクチャ概要

- `AIVTuberController`: メインのコントローラークラス
- `ILLMService`: LLMサービスのインターフェース
- `ErrorHandling`: エラー処理システム
- `VRMModelLoader`: VRMモデル読み込み管理

### 拡張ポイント

1. 新しいLLMサービスの追加
```csharp
public class NewLLMService : ILLMService
{
    public bool IsInitialized => true;
    
    public async Task<string> GetResponseAsync(string input)
    {
        // サービス固有の実装
        return await Task.FromResult("応答");
    }
}
```

2. カスタム表情の追加
```csharp
public void AddCustomExpression(string emotionName, ExpressionKey expressionKey)
{
    emotionToExpression[emotionName] = expressionKey;
}
```

### パフォーマンス最適化

- 音声合成のキャッシュ
- 非同期処理の適切な使用
- メモリ管理の注意点

## 貢献ガイドライン

### プルリクエスト

1. 新機能の追加
   - 機能の説明と目的
   - 実装の詳細
   - テストケース

2. バグ修正
   - バグの再現手順
   - 修正内容の説明
   - テスト結果

### コーディング規約

- C#コーディング規約に従う
- XML文書コメントを使用
- 単体テストを作成

### 開発フロー

1. Issueの作成
2. ブランチの作成（feature/xxxまたはfix/xxx）
3. 実装
4. テスト
5. プルリクエスト
6. コードレビュー
7. マージ

## サポート

問題が解決しない場合は、以下の手順で報告してください：

1. Issueを作成
2. 問題の詳細な説明
3. 再現手順
4. 環境情報
   - Unityバージョン
   - UniVRMバージョン
   - OS情報
   - 使用しているLLMサービス

## 更新履歴

### v1.0.0
- 初期リリース
- 基本機能の実装
- VOICEVOXとの連携
- 3種類のLLMサービス対応

### 7. UI/UXの設定とカスタマイズ

#### 7.1 基本的なCanvas階層構造
```
Canvas (Screen Space - Overlay)
├── SafeArea
│   ├── Header
│   │   ├── Title
│   │   └── StatusIndicator
│   ├── ChatArea
│   │   ├── ScrollView
│   │   │   └── Content
│   │   │       ├── MessageContainer
│   │   │       │   ├── UserMessage
│   │   │       │   └── AIResponse
│   │   │       └── MessageTemplate
│   │   └── ScrollBar
│   ├── InputArea
│   │   ├── InputField
│   │   └── SendButton
│   └── EmotePanel
│       ├── EmoteGrid
│       └── EmoteButtons
└── OverlayEffects
    ├── LoadingIndicator
    └── NotificationPanel
```

#### 7.2 UI要素の詳細設定
```csharp
// Canvas設定
Canvas mainCanvas = gameObject.AddComponent<Canvas>();
mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
mainCanvas.sortingOrder = 100;

// スケーリング設定
CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
scaler.referenceResolution = new Vector2(1920, 1080);
scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
scaler.matchWidthOrHeight = 1.0f;

// セーフエリアの設定
public class SafeAreaHandler : MonoBehaviour
{
    private void Awake()
    {
        ApplySafeArea();
    }

    private void ApplySafeArea()
    {
        var safeArea = Screen.safeArea;
        var rectTransform = GetComponent<RectTransform>();
        
        var anchorMin = safeArea.position;
        var anchorMax = anchorMin + safeArea.size;
        
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }
}
```

#### 7.3 チャットメッセージのレイアウト
```csharp
public class ChatMessageLayout : MonoBehaviour
{
    [SerializeField] private float maxWidth = 800f;
    [SerializeField] private float padding = 20f;
    [SerializeField] private float spacing = 10f;
    
    // メッセージプレハブの設定
    public class MessageSettings
    {
        public Color userMessageColor = new Color(0.2f, 0.6f, 1f);
        public Color aiMessageColor = new Color(0.8f, 0.8f, 0.8f);
        public float messageFadeInDuration = 0.3f;
        public float messageSpacing = 15f;
        public float cornerRadius = 10f;
    }
    
    // メッセージの追加
    public void AddMessage(string text, bool isUser)
    {
        var settings = new MessageSettings();
        var messageObj = Instantiate(messagePrefab, contentTransform);
        var messageUI = messageObj.GetComponent<ChatMessage>();
        
        messageUI.Initialize(text, isUser, settings);
        StartCoroutine(AnimateMessage(messageUI));
    }
    
    // メッセージのアニメーション
    private IEnumerator AnimateMessage(ChatMessage message)
    {
        message.SetAlpha(0f);
        float time = 0;
        
        while (time < message.settings.messageFadeInDuration)
        {
            float alpha = time / message.settings.messageFadeInDuration;
            message.SetAlpha(alpha);
            time += Time.deltaTime;
            yield return null;
        }
        
        message.SetAlpha(1f);
        ScrollToBottom();
    }
}
```

#### 7.4 インタラクティブ要素のデザイン
```csharp
public class ChatInputHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button sendButton;
    [SerializeField] private float inputDebounceTime = 0.1f;
    
    // 入力フィールドのスタイル設定
    public void ConfigureInputField()
    {
        inputField.textComponent.fontSize = 16;
        inputField.textComponent.color = new Color(0.2f, 0.2f, 0.2f);
        inputField.placeholder.GetComponent<TextMeshProUGUI>().text = "メッセージを入力...";
        
        // カスタムプレースホルダーアニメーション
        var placeholderAnimator = inputField.placeholder.gameObject.AddComponent<PlaceholderAnimator>();
        placeholderAnimator.fadeSpeed = 0.5f;
    }
    
    // 送信ボタンのスタイル設定
    public void ConfigureSendButton()
    {
        var buttonColors = sendButton.colors;
        buttonColors.normalColor = new Color(0.2f, 0.6f, 1f);
        buttonColors.highlightedColor = new Color(0.3f, 0.7f, 1f);
        buttonColors.pressedColor = new Color(0.1f, 0.5f, 0.9f);
        sendButton.colors = buttonColors;
        
        // ボタンアニメーション
        var buttonAnimator = sendButton.gameObject.AddComponent<ButtonAnimator>();
        buttonAnimator.scaleOnPress = 0.95f;
    }
}
```

#### 7.5 アクセシビリティとレスポンシブデザイン
```csharp
public class UIAccessibilityManager : MonoBehaviour
{
    // フォントサイズの動的調整
    public void AdjustFontSizes()
    {
        float screenWidth = Screen.width;
        float baseSize = 16f;
        float scaleFactor = Mathf.Clamp(screenWidth / 1920f, 0.8f, 1.2f);
        
        foreach (var text in FindObjectsOfType<TextMeshProUGUI>())
        {
            text.fontSize = baseSize * scaleFactor;
        }
    }
    
    // コントラスト比の確保
    public void EnsureAccessibleColors()
    {
        // WCAG 2.0 AAガイドラインに準拠
        var backgroundColor = new Color(0.98f, 0.98f, 0.98f);
        var textColor = new Color(0.1f, 0.1f, 0.1f);
        var accentColor = new Color(0.2f, 0.6f, 1f);
    }
}
```

#### 7.6 アニメーションとトランジション
```csharp
public class UIAnimationController : MonoBehaviour
{
    // メッセージ送信アニメーション
    public IEnumerator AnimateSendMessage(RectTransform messageRect)
    {
        messageRect.localScale = Vector3.zero;
        
        float time = 0;
        float duration = 0.3f;
        
        while (time < duration)
        {
            float t = time / duration;
            float scale = Mathf.Sin(t * Mathf.PI * 0.5f);
            messageRect.localScale = Vector3.one * scale;
            time += Time.deltaTime;
            yield return null;
        }
        
        messageRect.localScale = Vector3.one;
    }
    
    // 感情表現エフェクト
    public void PlayEmoteEffect(string emotion)
    {
        var effectPrefab = GetEmoteEffectPrefab(emotion);
        var effect = Instantiate(effectPrefab, effectsContainer);
        
        var sequence = DOTween.Sequence();
        sequence.Append(effect.DOScale(1.2f, 0.2f));
        sequence.Append(effect.DOScale(1f, 0.1f));
        sequence.AppendInterval(1f);
        sequence.Append(effect.DOFade(0f, 0.3f));
        sequence.OnComplete(() => Destroy(effect.gameObject));
    }
}
```

#### 7.7 パフォーマンス最適化
```csharp
public class ChatUIOptimizer : MonoBehaviour
{
    [SerializeField] private int maxVisibleMessages = 50;
    [SerializeField] private int recycleThreshold = 20;
    
    // メッセージプール
    private Queue<ChatMessage> messagePool = new Queue<ChatMessage>();
    private List<ChatMessage> activeMessages = new List<ChatMessage>();
    
    // メッセージのリサイクル
    private void RecycleMessages()
    {
        if (activeMessages.Count > maxVisibleMessages + recycleThreshold)
        {
            int removeCount = activeMessages.Count - maxVisibleMessages;
            for (int i = 0; i < removeCount; i++)
            {
                var message = activeMessages[i];
                message.gameObject.SetActive(false);
                messagePool.Enqueue(message);
            }
            activeMessages.RemoveRange(0, removeCount);
        }
    }
    
    // レイアウトの最適化
    private void OptimizeLayout()
    {
        // レイアウトグループの更新を制御
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
        Canvas.ForceUpdateCanvases();
    }
}
```
