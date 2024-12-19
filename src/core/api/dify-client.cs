using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Threading.Tasks;
using System.Collections;
using src.core.models;

namespace src.core.api
{
    [Serializable]
    public class DifyRequestBody
    {
        public string inputs;
        public string query;
        public string response_mode;
        public string user;
    }

    [Serializable]
    public class DifyResponseBody
    {
        public string id;
        public string answer;
        public string conversation_id;
    }

    public class DifyClient : MonoBehaviour
    {
        private readonly string _apiKey;
        private readonly string _apiBaseUrl;

        public DifyClient(string apiKey, string apiBaseUrl = "https://api.dify.ai/v1")
        {
            _apiKey = apiKey;
            _apiBaseUrl = apiBaseUrl;
        }

        public async Task<LlmResponse> SendChatRequestAsync(LlmRequest request)
        {
            try 
            {
                var requestBody = new DifyRequestBody
                {
                    inputs = request.UserMessage,
                    query = request.UserMessage,
                    response_mode = "streaming",
                    user = request.UserId
                };

                var jsonRequest = JsonUtility.ToJson(requestBody);
                
                using (var webRequest = new UnityWebRequest($"{_apiBaseUrl}/chat-messages", "POST"))
                {
                    byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonRequest);
                    webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
                    webRequest.downloadHandler = new DownloadHandlerBuffer();
                    webRequest.SetRequestHeader("Content-Type", "application/json");
                    webRequest.SetRequestHeader("Authorization", $"Bearer {_apiKey}");

                    await webRequest.SendWebRequest();

                    if (webRequest.result == UnityWebRequest.Result.Success)
                    {
                        var responseBody = webRequest.downloadHandler.text;
                        return ParseDifyResponse(responseBody);
                    }
                    else
                    {
                        return new LlmResponse 
                        {
                            ResponseId = Guid.NewGuid().ToString(),
                            GeneratedText = $"API Error: {webRequest.error}",
                            ReliabilityScore = 0.0
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new LlmResponse 
                {
                    ResponseId = Guid.NewGuid().ToString(),
                    GeneratedText = $"Error: {ex.Message}",
                    ReliabilityScore = 0.0
                };
            }
        }

        private LlmResponse ParseDifyResponse(string responseBody)
        {
            try 
            {
                var response = JsonUtility.FromJson<DifyResponseBody>(responseBody);

                return new LlmResponse 
                {
                    ResponseId = response.id,
                    GeneratedText = response.answer,
                    ReliabilityScore = CalculateReliabilityScore(),
                    Metadata = new LlmMetadata 
                    {
                        ConversationId = response.conversation_id
                    }
                };
            }
            catch (Exception ex)
            {
                return new LlmResponse 
                {
                    ResponseId = Guid.NewGuid().ToString(),
                    GeneratedText = $"Parse Error: {ex.Message}",
                    ReliabilityScore = 0.0
                };
            }
        }

        private double CalculateReliabilityScore()
        {
            return 0.8;
        }

        public async Task<bool> ValidateApiConnectionAsync()
        {
            try 
            {
                using (var webRequest = UnityWebRequest.Get($"{_apiBaseUrl}/health"))
                {
                    await webRequest.SendWebRequest();
                    return webRequest.result == UnityWebRequest.Result.Success;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}