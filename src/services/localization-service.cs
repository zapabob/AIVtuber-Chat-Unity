using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Threading;
using UnityEngine;

namespace AIVtuberChat.Services
{
    public class LocalizationService : MonoBehaviour
    {
        private ResourceManager resourceManager;
        private CultureInfo currentCulture;

        // サポート言語リスト
        private static readonly Dictionary<string, string> SupportedLanguages = new Dictionary<string, string>
        {
            { "en", "English" },
            { "ja", "日本語" },
            { "zh", "中文" },
            { "ko", "한국어" },
            { "es", "Español" }
        };

        public LocalizationService(string resourcePath)
        {
            try
            {
                // Unity向けにリソースマネージャーの初期化を修正
                resourceManager = new ResourceManager(resourcePath, typeof(LocalizationService).Assembly);
                SetLanguage(SystemLanguage.English.ToString().ToLower());
            }
            catch (Exception ex)
            {
                Debug.LogError($"Localization initialization failed: {ex.Message}");
            }
        }

        /// <summary>
        /// 言語を設定する
        /// </summary>
        /// <param name="languageCode">言語コード（例：en, ja, zh）</param>
        public void SetLanguage(string languageCode)
        {
            if (SupportedLanguages.ContainsKey(languageCode))
            {
                try
                {
                    currentCulture = new CultureInfo(languageCode);
                    // Unity向けにスレッド処理を修正
                    #if !UNITY_WEBGL
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                    Thread.CurrentThread.CurrentUICulture = currentCulture;
                    #endif
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Language setting failed: {ex.Message}");
                }
            }
            else
            {
                Debug.LogWarning($"Unsupported language code: {languageCode}");
            }
        }

        /// <summary>
        /// 指定されたキーの翻訳を取得
        /// </summary>
        /// <param name="key">翻訳キー</param>
        /// <returns>翻訳された文字列</returns>
        public string Translate(string key)
        {
            return resourceManager.GetString(key, currentCulture) ?? key;
        }

        /// <summary>
        /// 指定されたキーの翻訳を取得（フォーマット付き）
        /// </summary>
        /// <param name="key">翻訳キー</param>
        /// <param name="args">フォーマット引数</param>
        /// <returns>フォーマットされた翻訳文字列</returns>
        public string Translate(string key, params object[] args)
        {
            string translation = resourceManager.GetString(key, currentCulture) ?? key;
            return string.Format(translation, args);
        }

        /// <summary>
        /// サポートされている言語のリストを取得
        /// </summary>
        /// <returns>サポート言語の辞書</returns>
        public Dictionary<string, string> GetSupportedLanguages()
        {
            return new Dictionary<string, string>(SupportedLanguages);
        }

        /// <summary>
        /// 現在の言語コードを取得
        /// </summary>
        /// <returns>現在の言語コード</returns>
        public string GetCurrentLanguageCode()
        {
            return currentCulture.TwoLetterISOLanguageName;
        }
    }
}

// LocalizationServiceの初期化
var localizationService = new LocalizationService("path.to.resource.file");

// 言語設定
localizationService.SetLanguage("ja");

// 翻訳の取得
string greeting = localizationService.Translate("GREETING_KEY");

// フォーマット付き翻訳
string welcomeMessage = localizationService.Translate("WELCOME_MESSAGE", "ユーザー名");

// サポート言語の取得
var supportedLanguages = localizationService.GetSupportedLanguages();