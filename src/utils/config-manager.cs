using UnityEngine;
using System;
using System.IO;

namespace AIVtuberChat.Utils
{
    public class ConfigManager : MonoBehaviour
    {
        private static ConfigManager _instance;
        public static ConfigManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("ConfigManager");
                    _instance = go.AddComponent<ConfigManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private ConfigData _config;
        private readonly string CONFIG_PATH = Path.Combine(Application.persistentDataPath, "config.json");

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                if (File.Exists(CONFIG_PATH))
                {
                    string json = File.ReadAllText(CONFIG_PATH);
                    _config = JsonUtility.FromJson<ConfigData>(json);
                }
                else
                {
                    _config = new ConfigData();
                    SaveConfig();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"設定ファイルの読み込みに失敗しました: {ex.Message}");
                _config = new ConfigData();
            }
        }

        public void SaveConfig()
        {
            try
            {
                string json = JsonUtility.ToJson(_config, true);
                File.WriteAllText(CONFIG_PATH, json);
                Debug.Log("設定を保存しました");
            }
            catch (Exception ex)
            {
                Debug.LogError($"設定の保存に失敗しました: {ex.Message}");
            }
        }

        public string GetApiKey(string service)
        {
            return _config.GetApiKey(service);
        }

        public void SetApiKey(string service, string apiKey)
        {
            _config.SetApiKey(service, apiKey);
            SaveConfig();
        }
    }

    [Serializable]
    public class ConfigData
    {
        public string OpenAIApiKey = "";
        public string VoiceVoxEndpoint = "http://localhost:50021";
        public string DiscordToken = "";

        public string GetApiKey(string service)
        {
            switch (service.ToLower())
            {
                case "openai":
                    return OpenAIApiKey;
                case "discord":
                    return DiscordToken;
                default:
                    return "";
            }
        }

        public void SetApiKey(string service, string apiKey)
        {
            switch (service.ToLower())
            {
                case "openai":
                    OpenAIApiKey = apiKey;
                    break;
                case "discord":
                    DiscordToken = apiKey;
                    break;
            }
        }
    }
}