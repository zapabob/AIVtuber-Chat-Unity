using System;
using System.IO;
using System.Threading.Tasks;
using NAudio.Wave;
using UnityEngine;

namespace AIVtuberApp.Services
{
    public class AudioManager : MonoBehaviour, IDisposable
    {
        private WaveOutEvent _outputDevice;
        private AudioFileReader _audioFileReader;
        private VoicevoxClient _voicevoxClient;

        private void Awake()
        {
            _voicevoxClient = new VoicevoxClient();
        }

        /// <summary>
        /// 音声ファイルを再生する
        /// </summary>
        /// <param name="filePath">音声ファイルのパス</param>
        public async Task PlayAudioAsync(string filePath)
        {
            try
            {
                // 既存の再生を停止
                StopAudio();

                // 音声ファイルを読み込み
                _audioFileReader = new AudioFileReader(filePath);
                _outputDevice = new WaveOutEvent();
                _outputDevice.Init(_audioFileReader);
                _outputDevice.Play();

                // 再生が完了するまで待機
                while (_outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                // UnityのDebugログを使用
                Debug.LogError($"音声再生エラー: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 音声再生を停止する
        /// </summary>
        public void StopAudio()
        {
            if (_outputDevice != null)
            {
                _outputDevice.Stop();
                _outputDevice.Dispose();
                _outputDevice = null;
            }

            if (_audioFileReader != null)
            {
                _audioFileReader.Dispose();
                _audioFileReader = null;
            }
        }

        /// <summary>
        /// 音声合成を行う（Voicevox APIと連携）
        /// </summary>
        /// <param name="text">合成する文字列</param>
        /// <param name="voiceId">話者ID</param>
        /// <returns>生成された音声ファイルのパス</returns>
        public async Task<string> SynthesizeVoiceAsync(string text, int voiceId)
        {
            try
            {
                string audioFilePath = await _voicevoxClient.GenerateSpeechAsync(text, voiceId);
                return audioFilePath;
            }
            catch (Exception ex)
            {
                Debug.LogError($"音声合成エラー: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 音声の音量を調整する
        /// </summary>
        /// <param name="volume">音量（0.0 - 1.0）</param>
        public void SetVolume(float volume)
        {
            if (_outputDevice != null)
            {
                _outputDevice.Volume = Mathf.Clamp01(volume);
            }
        }

        /// <summary>
        /// リソースを解放する
        /// </summary>
        public void Dispose()
        {
            StopAudio();
            _voicevoxClient?.Dispose();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}