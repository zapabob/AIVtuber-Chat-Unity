using System;
using UnityEngine;

namespace AIVTuber
{
    public enum TTSEngineType
    {
        VoiceVox,
        Nijivoice,
        StyleBertVits2,
        Yukkuri
    }

    [Serializable]
    public class AIVTuberSettings
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

        // Maria設定
        public string Endpoint = "http://localhost:8000";
        public string ApiKey = "";
        
        // VOICEVOX設定
        public string VoicevoxEndpoint = "http://localhost:50021";
        public int SpeakerId = 1;
        public float VoiceSpeed = 1.0f;
        public float VoicePitch = 0.0f;
        public float VoiceIntonation = 1.0f;
        public float VoiceVolume = 1.0f;

        // エラーハンドリング設定
        public int MaxRetryCount = 3;
        public float RetryDelay = 1.0f;

        // LLMサービスタイプ
        public LLMServiceType LLMServiceType = LLMServiceType.LocalLLM;

        // TTSエンジンタイプ
        public TTSEngineType TTSEngineType = TTSEngineType.VoiceVox;

        // 設定をAIVTuberConfigに変換するメソッド
        public AIVTuberConfig ToConfig()
        {
            return new AIVTuberConfig
            {
                Temperature = this.Temperature,
                MaxTokens = this.MaxTokens,
                SystemPrompt = this.SystemPrompt,
                DifyEndpoint = this.DifyEndpoint,
                DifyApiKey = this.DifyApiKey,
                LocalLLMEndpoint = this.LocalLLMEndpoint,
                LocalLLMModel = this.LocalLLMModel,
                VoicevoxEndpoint = this.VoicevoxEndpoint,
                SpeakerId = this.SpeakerId,
                VoiceSpeed = this.VoiceSpeed,
                VoicePitch = this.VoicePitch,
                VoiceIntonation = this.VoiceIntonation,
                VoiceVolume = this.VoiceVolume,
                MaxRetryCount = this.MaxRetryCount,
                RetryDelay = this.RetryDelay,
                MaxContextLength = 2048,
                MaxLength = 100,
                TopP = 0.9f,
                TopK = 40,
                RepetitionPenalty = 1.1f,
                RepetitionPenaltyRange = 512,
                RepetitionPenaltySlope = 0.7f,
                MinP = 0.05f,
                Mirostat = 0,
                MirostatTau = 5f,
                MirostatEta = 0.1f,
                StopSequences = new string[] { "ユーザー：", "\n" }
            };
        }
    }
}
