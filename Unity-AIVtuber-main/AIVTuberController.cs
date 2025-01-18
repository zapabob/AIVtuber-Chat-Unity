using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Text;
using System.IO;
using UniVRM10;
using System.Runtime.CompilerServices;

namespace AIVTuber
{
    public static class UnityWebRequestExtensions
    {
        public static TaskAwaiter<UnityWebRequest> GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
        {
            var tcs = new TaskCompletionSource<UnityWebRequest>();
            asyncOp.completed += obj => { tcs.SetResult(((UnityWebRequestAsyncOperation)obj).webRequest); };
            return tcs.Task.GetAwaiter();
        }
    }

    // LLMサービスファクトリ
    public static class LLMServiceFactory
    {
        public static ILLMService Create(LLMServiceType serviceType, AIVTuberConfig config, AIVTuberSettings settings)
        {
            switch (serviceType)
            {
                case LLMServiceType.Dify:
                    return new DifyService(config.DifyEndpoint, config.DifyApiKey);
                case LLMServiceType.LocalLLM:
                    return new LocalLLMService(config.LocalLLMEndpoint, config.LocalLLMModel);
                case LLMServiceType.Maria:
                    return new MariaService(settings);
                default:
                    throw new AIVTuberException($"Unsupported LLM service type: {serviceType}", ErrorType.Configuration);
            }
        }
    }

    // LLMサービスの実装
    public class DifyService : ILLMService
    {
        private readonly string endpoint;
        private readonly string apiKey;
        public bool IsInitialized => !string.IsNullOrEmpty(apiKey);

        public DifyService(string endpoint, string apiKey)
        {
            this.endpoint = endpoint;
            this.apiKey = apiKey;
        }

        public Task<string> GetResponseAsync(string input, string systemPrompt)
        {
            // Dify APIの実装
            throw new NotImplementedException();
        }

        public Task<string> GetResponseAsync(string input)
        {
            return GetResponseAsync(input, "");
        }
    }

    public class LocalLLMService : ILLMService
    {
        private readonly string endpoint;
        private readonly string model;
        private readonly string modelPath;
        private readonly AIVTuberConfig config;

        public bool IsInitialized => !string.IsNullOrEmpty(endpoint) && File.Exists(modelPath);

        public LocalLLMService(string endpoint, string model)
        {
            this.endpoint = endpoint;
            this.model = model;
            this.modelPath = Path.Combine(Application.streamingAssetsPath, "Models", $"{model}.gguf");
            this.config = new AIVTuberConfig(); // デフォルト設定を使用
        }

        private UnityWebRequest currentRequest;

        public async Task<string> GetResponseAsync(string input, string systemPrompt)
        {
            try
            {
                var requestData = new KoboldRequest
                {
                    prompt = FormatPrompt(input, systemPrompt),
                    max_context_length = config.MaxContextLength,
                    max_length = config.MaxLength,
                    temperature = config.Temperature,
                    top_p = config.TopP,
                    top_k = config.TopK,
                    rep_pen = config.RepetitionPenalty,
                    rep_pen_range = config.RepetitionPenaltyRange,
                    rep_pen_slope = config.RepetitionPenaltySlope,
                    tfs = 1,
                    stop_sequence = config.StopSequences,
                    min_p = config.MinP,
                    mirostat = config.Mirostat,
                    mirostat_tau = config.MirostatTau,
                    mirostat_eta = config.MirostatEta,
                    sampler_order = new int[] { 6, 0, 1, 3, 4, 2, 5 }
                };

                var jsonData = JsonUtility.ToJson(requestData);
                using (currentRequest = new UnityWebRequest($"{endpoint}/api/v1/generate", "POST"))
                {
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                    currentRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    currentRequest.downloadHandler = new DownloadHandlerBuffer();
                    currentRequest.SetRequestHeader("Content-Type", "application/json");

                    var response = await currentRequest.SendWebRequest();

                    if (response.result != UnityWebRequest.Result.Success)
                    {
                        throw new AIVTuberException($"Kobold API error: {response.error}", ErrorType.AIResponse);
                    }

                    var responseJson = JsonUtility.FromJson<KoboldResponse>(response.downloadHandler.text);
                    return CleanResponse(responseJson.results[0].text.Trim());
                }
            }
            catch (Exception ex)
            {
                throw new AIVTuberException($"Kobold API error: {ex.Message}", ex, ErrorType.AIResponse);
            }
            finally
            {
                currentRequest = null;
            }
        }

        private string FormatPrompt(string input, string systemPrompt)
        {
            if (string.IsNullOrEmpty(systemPrompt))
            {
                systemPrompt = config.SystemPrompt;
            }
            
            return $"{systemPrompt}\n\nユーザー：{input}\nアシスタント：";
        }

        private string CleanResponse(string response)
        {
            // 不要な応答の終了マーカーを削除
            foreach (var stopSequence in config.StopSequences)
            {
                if (response.Contains(stopSequence))
                {
                    response = response.Substring(0, response.IndexOf(stopSequence));
                }
            }
            return response.Trim();
        }

        public Task<string> GetResponseAsync(string input)
        {
            return GetResponseAsync(input, "");
        }

        public void CancelRequest()
        {
            if (currentRequest != null)
            {
                currentRequest.Abort();
                currentRequest = null;
            }
        }
    }

    [Serializable]
    public class KoboldRequest
    {
        public string prompt;
        public int max_context_length;
        public int max_length;
        public float temperature;
        public float top_p;
        public int top_k;
        public float rep_pen;
        public int rep_pen_range;
        public float rep_pen_slope;
        public int tfs;
        public string[] stop_sequence;
        public float min_p;
        public int mirostat;
        public float mirostat_tau;
        public float mirostat_eta;
        public int[] sampler_order;
    }

    [Serializable]
    public class KoboldResponse
    {
        [Serializable]
        public class Result
        {
            public string text;
        }
        public Result[] results;
    }

    [Serializable]
    public class MariaResponse
    {
        [Serializable]
        public class Choice
        {
            [Serializable]
            public class Message
            {
                public string content;
            }
            public Message message;
        }
        public Choice[] choices;
    }

    [RequireComponent(typeof(VRMModelLoader))]
    public class AIVTuberController : MonoBehaviour
    {
        [Header("VRM Settings")]
        public GameObject vrmModel;
        private Vrm10Instance vrmInstance;
        private Animator animator;
        private VRMModelLoader vrmLoader;
        private bool IsLoading = false;
        private GameObject ModelObject;

        [Header("Error Handling")]
        public ErrorNotification errorDisplay;

        [Header("Voice Settings")]
        private AudioSource audioSource;

        private ILLMService llmService;
        private AIVTuberConfig config;
        public AIVTuberSettings settings;

        // 感情表現のマッピング
        private Dictionary<string, ExpressionKey> emotionToExpression = new Dictionary<string, ExpressionKey>()
        {
            {"neutral", ExpressionKey.Neutral},
            {"happy", ExpressionKey.Happy},
            {"angry", ExpressionKey.Angry},
            {"sad", ExpressionKey.Sad},
            {"relaxed", ExpressionKey.Neutral}
        };

        private void Start()
        {
            InitializeComponents();
            InitializeLLMService();
            StartCoroutine(AutoBlink());
        }

        private void InitializeComponents()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            vrmLoader = GetComponent<VRMModelLoader>();
            
            if (vrmModel != null)
            {
                vrmInstance = vrmModel.GetComponent<Vrm10Instance>();
                if (vrmInstance == null)
                {
                    vrmInstance = vrmLoader.CurrentModel;
                }
                animator = vrmModel.GetComponent<Animator>();
            }
            else if (vrmLoader.CurrentModel != null)
            {
                vrmInstance = vrmLoader.CurrentModel;
                vrmModel = vrmInstance.gameObject;
                animator = vrmModel.GetComponent<Animator>();
            }

            if (vrmInstance == null)
            {
                throw new AIVTuberException("VRM instance not found!", ErrorType.VRMModel);
            }

            if (animator == null)
            {
                throw new AIVTuberException("Animator not found!", ErrorType.VRMModel);
            }
        }

        private void InitializeLLMService()
        {
            if (settings == null)
            {
                settings = new AIVTuberSettings(); // デフォルト設定を使用
            }

            config = settings.ToConfig();

            llmService = LLMServiceFactory.Create(settings.LLMServiceType, config, settings);

            if (!llmService.IsInitialized)
            {
                throw new AIVTuberException("LLM service is not properly initialized", ErrorType.Configuration);
            }
        }

        public async Task SetVRMModel(GameObject model)
        {
            vrmModel = model;
            await Task.Yield();
            InitializeComponents();
        }

        private async Task LoadModel()
        {
            if (vrmLoader == null)
            {
                vrmLoader = GetComponent<VRMModelLoader>();
            }

            try
            {
                IsLoading = true;
                var path = Path.Combine(Application.streamingAssetsPath, "AliciaSolid_vrm10.vrm");
                
                if (path.Contains("://") || path.Contains(":///"))
                {
                    using (UnityWebRequest www = UnityWebRequest.Get(path))
                    {
                        await www.SendWebRequest();
                        if (www.result != UnityWebRequest.Result.Success)
                        {
                            throw new AIVTuberException($"Failed to download VRM: {www.error}", ErrorType.VRMModel);
                        }
                        var tempPath = Path.Combine(Application.temporaryCachePath, "temp.vrm");
                        File.WriteAllBytes(tempPath, www.downloadHandler.data);
                        ModelObject = await vrmLoader.LoadVRMModel(tempPath);
                        File.Delete(tempPath);
                    }
                }
                else
                {
                    ModelObject = await vrmLoader.LoadVRMModel(path);
                }
            }
            catch (AIVTuberException e)
            {
                Debug.LogError(e);
                errorDisplay.ShowError(e.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task UnloadModel()
        {
            if (ModelObject != null)
            {
                Destroy(ModelObject);
                ModelObject = null;
            }
        }

        public async Task<string> HandleMessage(string userMessage)
        {
            Debug.Log($"HandleMessage called with userMessage: {userMessage}");
            if (IsLoading)
            {
                Debug.Log("Model is currently loading. Ignoring message.");
                return null;
            }
            try
            {
                var response = await ErrorHandler.RetryOnFailureAsync(async () =>
                {
                    var aiResponse = await GetAIResponse(userMessage);
                    string emotion = ExtractEmotion(aiResponse);
                    string text = RemoveEmotionTag(aiResponse);

                    ExpressEmotion(emotion);
                    await SynthesizeAndPlayVoice(text);
                    return aiResponse;
                }, settings.MaxRetryCount, settings.RetryDelay);

                // Send response back to the server
                if (!string.IsNullOrEmpty(response))
                {
                    await SendResponseToServer(response);
                }
                return response;
            }
            catch (AIVTuberException ex)
            {
                ErrorHandler.LogError(ex);
                ErrorNotification.Instance.ShowError(
                    ErrorHandler.GetUserFriendlyMessage(ex.Type));
                return null;
            }
            catch (Exception ex)
            {
                ErrorHandler.LogError(ex, "HandleMessage");
                ErrorNotification.Instance.ShowError(
                    ErrorHandler.GetUserFriendlyMessage(ErrorType.General));
                return null;
            }
        }

        public async void ReceiveMessageFromExternal(string message)
        {
            await HandleMessage(message);
        }

        private async Task SendResponseToServer(string response)
        {
            try
            {
                using (UnityWebRequest request = new UnityWebRequest("http://localhost:3000/response", "POST"))
                {
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(new { response = response }));
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.SetRequestHeader("Content-Type", "application/json");
                    await request.SendWebRequest();

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError($"Failed to send response to server: {request.error}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error sending response to server: {ex.Message}");
            }
        }

        public async Task<string> GetAIResponse(string userMessage)
        {
            Debug.Log($"GetAIResponse called with userMessage: {userMessage}");
            if (llmService == null)
            {
                InitializeLLMService();
            }

            try
            {
                var response = await llmService.GetResponseAsync(userMessage);
                Debug.Log($"GetAIResponse received response: {response}");
                return ProcessResponse(response);
            }
            catch (Exception ex)
            {
                Debug.LogError($"GetAIResponse exception: {ex.Message}");
                throw new AIVTuberException($"Failed to get AI response: {ex.Message}", ex, ErrorType.AIResponse);
            }
        }

        private string ProcessResponse(string response)
        {
            // 感情タグの処理を強化
            string emotion = ExtractEmotion(response);
            string text = RemoveEmotionTag(response);

            // 感情に基づいて声のパラメータを調整
            AdjustVoiceParameters(emotion);

            return text;
        }

        private void AdjustVoiceParameters(string emotion)
        {
            switch (emotion.ToLower())
            {
                case "happy":
                    config.VoiceSpeed = 1.2f;
                    config.VoicePitch = 0.2f;
                    break;
                case "sad":
                    config.VoiceSpeed = 0.8f;
                    config.VoicePitch = -0.2f;
                    break;
                case "angry":
                    config.VoiceSpeed = 1.3f;
                    config.VoicePitch = 0.3f;
                    break;
                default:
                    config.VoiceSpeed = 1.0f;
                    config.VoicePitch = 0.0f;
                    break;
            }
        }

        private string ExtractEmotion(string response)
        {
            int start = response.IndexOf('[');
            int end = response.IndexOf(']');
            if (start >= 0 && end > start)
            {
                return response.Substring(start + 1, end - start - 1);
            }
            return "neutral";
        }

        private string RemoveEmotionTag(string response)
        {
            int end = response.IndexOf(']');
            if (end >= 0)
            {
                return response.Substring(end + 1).Trim();
            }
            return response;
        }

        private async Task ExpressEmotion(string emotion)
        {
            if (vrmInstance != null && emotionToExpression.ContainsKey(emotion))
            {
                // 現在の表情をリセット
                foreach (var expression in emotionToExpression.Values)
                {
                    vrmInstance.Runtime.Expression.SetWeight(expression, 0);
                    await Task.Yield();
                }
                
                // 新しい表情を設定
                vrmInstance.Runtime.Expression.SetWeight(emotionToExpression[emotion], 1);
            }
        }

        private async Task SynthesizeAndPlayVoice(string text)
        {
            try
            {
                switch (settings.TTSEngineType)
                {
                    case TTSEngineType.VoiceVox:
                        // 1. VOICEVOX Audio Query
                        var queryUrl = $"{config.VoicevoxEndpoint}/audio_query?text={UnityWebRequest.EscapeURL(text)}&speaker={config.SpeakerId}";
                        string audioQuery = await GetAudioQuery(queryUrl);

                        // クエリパラメータの調整
                        var queryData = JsonUtility.FromJson<VoicevoxQuery>(audioQuery);
                        queryData.speedScale = config.VoiceSpeed;
                        queryData.pitchScale = config.VoicePitch;
                        queryData.intonationScale = config.VoiceIntonation;
                        queryData.volumeScale = config.VoiceVolume;
                        var adjustedQuery = JsonUtility.ToJson(queryData);

                        // 2. 音声合成
                        var synthesisUrl = $"{config.VoicevoxEndpoint}/synthesis?speaker={config.SpeakerId}";
                        byte[] audioData = await SynthesizeVoice(synthesisUrl, adjustedQuery);

                        // 3. 音声再生
                        await PlayVoice(audioData);
                        break;
                    case TTSEngineType.Nijivoice:
                        // 1. Nijivoice Audio Query
                        var nijivoiceQueryUrl = $"{config.NijivoiceEndpoint}/audio_query?text={UnityWebRequest.EscapeURL(text)}&speaker={config.NijivoiceSpeaker}";
                        string nijivoiceAudioQuery = await GetAudioQuery(nijivoiceQueryUrl);

                        // 2. 音声合成
                        var nijivoiceSynthesisUrl = $"{config.NijivoiceEndpoint}/synthesis?speaker={config.NijivoiceSpeaker}";
                        byte[] nijivoiceAudioData = await SynthesizeVoice(nijivoiceSynthesisUrl, nijivoiceAudioQuery);

                        // 3. 音声再生
                        await PlayVoice(nijivoiceAudioData);
                        break;
                    case TTSEngineType.StyleBertVits2:
                        // 1. Style-Bert-VITS2 Audio Synthesis
                        var styleBertVits2Url = $"{config.StyleBertVits2Endpoint}/synthesis?text={UnityWebRequest.EscapeURL(text)}&model={config.StyleBertVits2Model}";
                        byte[] styleBertVits2AudioData = await SynthesizeVoice(styleBertVits2Url, "");

                        // 2. 音声再生
                        await PlayVoice(styleBertVits2AudioData);
                        break;
                    case TTSEngineType.Yukkuri:
                        // 1. Yukkuri Audio Synthesis
                        var yukkuriUrl = $"{config.YukkuriEndpoint}/synthesis?text={UnityWebRequest.EscapeURL(text)}&voice={config.YukkuriVoice}";
                        byte[] yukkuriAudioData = await SynthesizeVoice(yukkuriUrl, "");

                        // 2. 音声再生
                        await PlayVoice(yukkuriAudioData);
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Voice synthesis error: {ex.Message}");
                throw new AIVTuberException($"Voice synthesis error: {ex.Message}", ex, ErrorType.VoiceSynthesis);
            }
        }

        private async Task<string> GetAudioQuery(string url)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                var response = await request.SendWebRequest();
                if (response.result != UnityWebRequest.Result.Success)
                {
                    throw new AIVTuberException($"Audio query failed: {response.error}", ErrorType.VoiceSynthesis);
                }
                return response.downloadHandler.text;
            }
        }

        private async Task<byte[]> SynthesizeVoice(string url, string audioQuery)
        {
            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(audioQuery);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                var response = await request.SendWebRequest();
                if (response.result != UnityWebRequest.Result.Success)
                {
                    throw new AIVTuberException($"Voice synthesis failed: {response.error}", ErrorType.VoiceSynthesis);
                }

                return response.downloadHandler.data;
            }
        }

        private async Task PlayVoice(byte[] audioData)
        {
            var clip = await CreateAudioClip(audioData);
            if (clip != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }

        private Task<AudioClip> CreateAudioClip(byte[] audioData)
        {
            try
            {
                // WAVヘッダーをパースしてAudioClipを作成
                const int headerSize = 44; // WAVヘッダーのサイズ
                const int sampleRate = 24000; // VOICEVOXのサンプルレート

                var samples = new float[(audioData.Length - headerSize) / 2];
                for (int i = 0; i < samples.Length; i++)
                {
                    short wave = BitConverter.ToInt16(audioData, headerSize + i * 2);
                    samples[i] = wave / 32768f;
                }

                AudioClip clip = AudioClip.Create("VoicevoxAudio", samples.Length, 1, sampleRate, false);
                clip.SetData(samples, 0);
                return Task.FromResult(clip);
            }
            catch (Exception ex)
            {
                throw new AIVTuberException($"Failed to create AudioClip: {ex.Message}", ex, ErrorType.VoiceSynthesis);
            }
        }

        private IEnumerator AutoBlink()
        {
            while (true)
            {
                // 通常時の瞬き
                yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 6f));
                if (vrmInstance != null)
                {
                    vrmInstance.Runtime.Expression.SetWeight(ExpressionKey.Blink, 1);
                    yield return new WaitForSeconds(0.15f);
                    vrmInstance.Runtime.Expression.SetWeight(ExpressionKey.Blink, 0);
                }
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            if (audioSource != null && audioSource.clip != null)
            {
                Destroy(audioSource.clip);
            }
        }

        // LLMConfigからAIVTuberSettingsへの変換メソッドを追加

        // 非同期メソッドの修正
        private async Task<string> ProcessMessageAsync(string message)
        {
            try
            {
                var response = await GetAIResponse(message);
                string emotion = ExtractEmotion(response);
                string text = RemoveEmotionTag(response);

                ExpressEmotion(emotion);
                await SynthesizeAndPlayVoice(text);
                return response;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error processing message: {ex.Message}");
                return string.Empty;
            }
        }
    }

    [Serializable]
    public class VoicevoxQuery
    {
        public float speedScale = 1.0f;
        public float pitchScale = 0.0f;
        public float intonationScale = 1.0f;
        public float volumeScale = 1.0f;
        public float prePhonemeLength = 0.1f;
        public float postPhonemeLength = 0.1f;
        public float outputSamplingRate = 24000;
        public bool outputStereo = false;
    }

    public interface ILLMService
    {
        bool IsInitialized { get; }
        Task<string> GetResponseAsync(string input);
        Task<string> GetResponseAsync(string input, string systemPrompt);
    }

    // LLMサービスの実装
    public class MariaService : ILLMService
    {
        private readonly AIVTuberSettings settings;
        private UnityWebRequest currentRequest;
        public bool IsInitialized => !string.IsNullOrEmpty(settings?.Endpoint);

        public MariaService(AIVTuberSettings settings)
        {
            this.settings = settings;
        }

        public async Task<string> GetResponseAsync(string input, string systemPrompt = "")
        {
            try
            {
                var requestData = new
                {
                    prompt = string.IsNullOrEmpty(systemPrompt) ? settings.SystemPrompt : systemPrompt,
                    user_input = input,
                    temperature = settings.Temperature,
                    max_tokens = settings.MaxTokens
                };

                var jsonData = JsonUtility.ToJson(requestData);
                using (currentRequest = new UnityWebRequest(settings.Endpoint + "/v1/chat/completions", "POST"))
                {
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                    currentRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    currentRequest.downloadHandler = new DownloadHandlerBuffer();
                    currentRequest.SetRequestHeader("Content-Type", "application/json");

                    if (!string.IsNullOrEmpty(settings.ApiKey))
                    {
                        currentRequest.SetRequestHeader("Authorization", $"Bearer {settings.ApiKey}");
                    }

                    var response = await currentRequest.SendWebRequest();
                    if (response.result != UnityWebRequest.Result.Success)
                    {
                        throw new AIVTuberException($"Maria API error: {response.error}", ErrorType.AIResponse);
                    }

                    var responseJson = JsonUtility.FromJson<MariaResponse>(response.downloadHandler.text);
                    return responseJson.choices[0].message.content;
                }
            }
            catch (Exception ex)
            {
                throw new AIVTuberException($"Maria API error: {ex.Message}", ex, ErrorType.AIResponse);
            }
            finally
            {
                currentRequest = null;
            }
        }

        public Task<string> GetResponseAsync(string input)
        {
            return GetResponseAsync(input, settings.SystemPrompt);
        }

        public void CancelRequest()
        {
            if (currentRequest != null)
            {
                currentRequest.Abort();
                currentRequest = null;
            }
        }
    }
}
