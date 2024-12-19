using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AIVtuberApp.Core.Api;
using AIVtuberApp.Core.Models;
using AIVtuberApp.Services;
using Cysharp.Threading.Tasks;

namespace AIVtuberApp.Services
{
    public class ChatService : MonoBehaviour
    {
        private DifyClient _difyClient;
        private VrmAnimationService _vrmAnimationService;
        private List<ChatLog> _conversationHistory;

        private void Awake()
        {
            _conversationHistory = new List<ChatLog>();
        }

        public void Initialize(
            DifyClient difyClient, 
            VrmAnimationService vrmAnimationService)
        {
            _difyClient = difyClient ?? throw new ArgumentNullException(nameof(difyClient));
            _vrmAnimationService = vrmAnimationService ?? throw new ArgumentNullException(nameof(vrmAnimationService));
        }

        /// <summary>
        /// ユーザーメッセージを処理し、LLMと対話する
        /// </summary>
        /// <param name="userMessage">ユーザーからのメッセージ</param>
        /// <returns>AIの応答</returns>
        public async UniTask<string> ProcessMessageAsync(string userMessage)
        {
            try
            {
                // ... existing code ...
            }
            catch (Exception ex)
            {
                Debug.LogError($"メッセージ処理中にエラーが発生しました: {ex}");
                throw;
            }
        }
    }
}
