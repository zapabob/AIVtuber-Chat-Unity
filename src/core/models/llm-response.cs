using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AIVtuberChat.Core.Models
{
    /// <summary>
    /// 大規模言語モデル(LLM)からのレスポンスを表現するモデルクラス
    /// </summary>
    [System.Serializable]
    public class LlmResponse
    {
        /// <summary>
        /// レスポンスの一意の識別子
        /// </summary>
        [SerializeField]
        [JsonPropertyName("response_id")]
        private string responseId;
        public string ResponseId { get => responseId; set => responseId = value; }

        /// <summary>
        /// LLMによって生成されたテキスト
        /// </summary>
        [SerializeField]
        [JsonPropertyName("generated_text")]
        private string generatedText;
        public string GeneratedText { get => generatedText; set => generatedText = value; }

        /// <summary>
        /// レスポンスの信頼性スコア（0.0 - 1.0）
        /// </summary>
        [SerializeField]
        [JsonPropertyName("confidence_score")]
        private double confidenceScore;
        public double ConfidenceScore { get => confidenceScore; set => confidenceScore = value; }

        /// <summary>
        /// レスポンスに関連するメタデータ
        /// </summary>
        [SerializeField]
        [JsonPropertyName("metadata")]
        private ResponseMetadata metadata;
        public ResponseMetadata Metadata { get => metadata; set => metadata = value; }

        /// <summary>
        /// レスポンスが生成された日時
        /// </summary>
        [SerializeField]
        [JsonPropertyName("timestamp")]
        private string timestamp;
        public DateTime Timestamp 
        { 
            get => DateTime.Parse(timestamp);
            set => timestamp = value.ToString("o");
        }

        /// <summary>
        /// レスポンスの言語
        /// </summary>
        [SerializeField]
        [JsonPropertyName("language")]
        private string language;
        public string Language { get => language; set => language = value; }
    }

    /// <summary>
    /// LLMレスポンスに関連するメタデータクラス
    /// </summary>
    [System.Serializable]
    public class ResponseMetadata
    {
        /// <summary>
        /// 使用されたLLMモデル名
        /// </summary>
        [SerializeField]
        [JsonPropertyName("model_name")]
        private string modelName;
        public string ModelName { get => modelName; set => modelName = value; }

        /// <summary>
        /// モデルのバージョン
        /// </summary>
        [SerializeField]
        [JsonPropertyName("model_version")]
        private string modelVersion;
        public string ModelVersion { get => modelVersion; set => modelVersion = value; }

        /// <summary>
        /// トークン使用量
        /// </summary>
        [SerializeField]
        [JsonPropertyName("tokens_used")]
        private int tokensUsed;
        public int TokensUsed { get => tokensUsed; set => tokensUsed = value; }

        /// <summary>
        /// 追加のカスタムメタデータ
        /// </summary>
        [SerializeField]
        [JsonPropertyName("custom_properties")]
        private Dictionary<string, string> customProperties;
        public Dictionary<string, string> CustomProperties { get => customProperties; set => customProperties = value; }
    }
};