using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Threading.Tasks;
using System.Text;

namespace src.core.api
{
    public class VoicevoxClient : MonoBehaviour
    {
        private const string BaseUrl = "https://api.voicevox.io/v1";
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        /// <summary>
        /// Voicevox APIを使用して音声合成リクエストを送信
        /// </summary>
        /// <param name="text">音声に変換するテキスト</param>
        /// <param name="speakerId">話者ID</param>
        public async Task<AudioClip> SynthesizeSpeechAsync(string text, int speakerId = 1)
        {
            try
            {
                // 音声合成のクエリ作成リクエスト
                var queryResponse = await CreateSynthesisQueryAsync(text, speakerId);
                
                // 音声合成リクエスト
                var audioClip = await GenerateSpeechAsync(queryResponse, speakerId);

                // 音声の再生
                PlayAudio(audioClip);

                return audioClip;
            }
            catch (Exception ex)
            {
                Debug.LogError($"音声合成エラー: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 音声合成のクエリを作成
        /// </summary>
        private async Task<string> CreateSynthesisQueryAsync(string text, int speakerId)
        {
            var queryUrl = $"{BaseUrl}/audio_query?text={UnityWebRequest.EscapeURL(text)}&speaker={speakerId}";
            using var request = UnityWebRequest.Post(queryUrl, "");
            
            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"Query creation failed: {request.error}");
            }

            return request.downloadHandler.text;
        }

        /// <summary>
        /// 音声データを生成
        /// </summary>
        private async Task<AudioClip> GenerateSpeechAsync(string query, int speakerId)
        {
            var synthesisUrl = $"{BaseUrl}/synthesis?speaker={speakerId}";
            using var request = new UnityWebRequest(synthesisUrl, "POST");
            
            byte[] bodyRaw = Encoding.UTF8.GetBytes(query);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerAudioClip(synthesisUrl, AudioType.WAV);
            request.SetRequestHeader("Content-Type", "application/json");

            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"Speech synthesis failed: {request.error}");
            }

            return DownloadHandlerAudioClip.GetContent(request);
        }

        /// <summary>
        /// 利用可能な話者のリストを取得
        /// </summary>
        public async Task<string[]> GetAvailableSpeakersAsync()
        {
            var speakersUrl = $"{BaseUrl}/speakers";
            using var request = UnityWebRequest.Get(speakersUrl);
            
            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"Failed to get speakers: {request.error}");
            }

            return JsonUtility.FromJson<string[]>(request.downloadHandler.text);
        }

        /// <summary>
        /// 音声を再生
        /// </summary>
        private void PlayAudio(AudioClip audioClip)
        {
            if (_audioSource != null && audioClip != null)
            {
                _audioSource.clip = audioClip;
                _audioSource.Play();
            }
        }
    }
}