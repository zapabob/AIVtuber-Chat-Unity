using System;
using System.Collections.Generic;

namespace AIVtuberChat.Core.Models
{
    /// <summary>
    /// 大規模言語モデル(LLM)へのリクエストを表現するクラス
    /// </summary>
    public class LlmRequest
    {
        /// <summary>
        /// リクエストの一意の識別子
        /// </summary>
        public Guid RequestId { get; set; }

        /// <summary>
        /// ユーザーからのメッセージ
        /// </summary>
        public string UserMessage { get; set; }

        /// <summary>
        /// 使用する言語モデルの設定
        /// </summary>
        public ModelSettings ModelConfig { get; set; }

        /// <summary>
        /// 追加のリクエストパラメータ
        /// </summary>
        public Dictionary<string, object> AdditionalParameters { get; set; }

        /// <summary>
        /// リクエストの言語
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// コンストラクター
        /// </summary>
        public LlmRequest()
        {
            RequestId = Guid.NewGuid();
            AdditionalParameters = new Dictionary<string, object>();
        }

        /// <summary>
        /// モデル設定を表現する内部クラス
        /// </summary>
        public class ModelSettings
        {
            /// <summary>
            /// モデル名
            /// </summary>
            public string ModelName { get; set; }

            /// <summary>
            /// 最大トークン数
            /// </summary>
            public int MaxTokens { get; set; }

            /// <summary>
            /// 温度パラメータ（創造性の制御）
            /// </summary>
            public double Temperature { get; set; }

            /// <summary>
            /// トップP サンプリング
            /// </summary>
            public double TopP { get; set; }
        }

        /// <summary>
        /// リクエストにパラメータを追加するメソッド
        /// </summary>
        /// <param name="key">パラメータキー</param>
        /// <param name="value">パラメータ値</param>
        public void AddParameter(string key, object value)
        {
            AdditionalParameters[key] = value;
        }

        /// <summary>
        /// リクエストの検証を行うメソッド
        /// </summary>
        /// <returns>検証結果</returns>
        public bool Validate()
        {
            // 基本的な検証ロジック
            return !string.IsNullOrWhiteSpace(UserMessage) 
                   && ModelConfig != null 
                   && !string.IsNullOrEmpty(ModelConfig.ModelName);
        }
    }
};