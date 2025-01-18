using System;
using UnityEngine;

namespace AIVTuber
{
    [Serializable]
    public class AIVTuberConfig
    {
        // LLM設定
        public float Temperature = 0.7f;
        public int MaxTokens = 150;
        public string SystemPrompt = "あなたはフレンドリーなAIアシスタントです。";
        
        // Dify設定
        public string DifyEndpoint = "https://api.dify.ai/v1";
        public string DifyApiKey = "";

        // LocalLLM設定
        public string LocalLLMEndpoint = "http://localhost:5001";
        public string LocalLLMModel = "japanese-Chat-Evolve-TEST-7B-NSFW_iMat_Ch200_IQ4_XS";
        
        // Kobold API設定
        public int MaxContextLength = 2048;
        public int MaxLength = 100;
        public float TopP = 0.9f;
        public int TopK = 40;
        public float RepetitionPenalty = 1.1f;
        public int RepetitionPenaltyRange = 512;
        public float RepetitionPenaltySlope = 0.7f;
        public float MinP = 0.05f;
        public int Mirostat = 0;
        public float MirostatTau = 5f;
        public float MirostatEta = 0.1f;
        public string[] StopSequences = new string[] { "ユーザー：", "\n" };

        // VOICEVOX設定
        public string VoicevoxEndpoint = "http://localhost:50021";
        public int SpeakerId = 1;
        public float VoiceSpeed = 1.0f;
        public float VoicePitch = 0.0f;
        public float VoiceIntonation = 1.0f;
        public float VoiceVolume = 1.0f;

        // Nijivoice設定
        public string NijivoiceEndpoint = "";
        public string NijivoiceSpeaker = "";

        // Style-Bert-VITS2設定
        public string StyleBertVits2Endpoint = "";
        public string StyleBertVits2Model = "";

        // Yukkuri設定
        public string YukkuriEndpoint = "";
        public string YukkuriVoice = "";

        // エラーハンドリング設定
        public int MaxRetryCount = 3;
        public float RetryDelay = 1.0f;

        // 環境変数キー
        public const string ENV_VOICEVOX_ENDPOINT = "VOICEVOX_ENDPOINT";
        public const string ENV_DIFY_ENDPOINT = "DIFY_ENDPOINT";
        public const string ENV_DIFY_API_KEY = "DIFY_API_KEY";
        public const string ENV_LOCAL_LLM_ENDPOINT = "LOCAL_LLM_ENDPOINT";
        public const string ENV_LOCAL_LLM_MODEL = "LOCAL_LLM_MODEL";
        public const string ENV_SYSTEM_PROMPT = "SYSTEM_PROMPT";
        public const string ENV_NIJIVOICE_ENDPOINT = "NIJIVOICE_ENDPOINT";
        public const string ENV_NIJIVOICE_SPEAKER = "NIJIVOICE_SPEAKER";
        public const string ENV_STYLE_BERT_VITS2_ENDPOINT = "STYLE_BERT_VITS2_ENDPOINT";
        public const string ENV_STYLE_BERT_VITS2_MODEL = "STYLE_BERT_VITS2_MODEL";
        public const string ENV_YUKKURI_ENDPOINT = "YUKKURI_ENDPOINT";
        public const string ENV_YUKKURI_VOICE = "YUKKURI_VOICE";
    }
}
