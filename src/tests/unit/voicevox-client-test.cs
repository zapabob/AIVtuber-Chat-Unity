using System;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using Moq;
using AIVtuberChat.Core.Api;
using AIVtuberChat.Core.Services;

namespace AIVtuberChat.Tests.PlayMode
{
    public class VoicevoxClientTest : MonoBehaviour
    {
        private Mock<AudioManager> _mockAudioManager;
        private VoicevoxClient _voicevoxClient;

        [SetUp]
        public void Setup()
        {
            // モックの初期化
            _mockAudioManager = new Mock<AudioManager>();
            _voicevoxClient = new VoicevoxClient(_mockAudioManager.Object);
        }

        [UnityTest]
        public IEnumerator SynthesizeSpeech_ValidInput_ReturnsAudioData()
        {
            // Arrange
            string testText = "こんにちは、これはテストです。";
            int testSpeakerId = 1;
            byte[] expectedAudioData = new byte[] { 1, 2, 3, 4, 5 }; // ダミーの音声データ

            // モックの設定
            _mockAudioManager
                .Setup(am => am.GenerateSpeech(testText, testSpeakerId))
                .Returns(expectedAudioData);

            // Act
            byte[] resultAudioData = _voicevoxClient.SynthesizeSpeech(testText, testSpeakerId);

            // Assert
            Assert.AreEqual(expectedAudioData, resultAudioData);
            _mockAudioManager.Verify(am => am.GenerateSpeech(testText, testSpeakerId), Times.Once);
            
            yield return null;
        }

        [UnityTest]
        public IEnumerator SynthesizeSpeech_EmptyText_ThrowsArgumentException()
        {
            // Arrange
            string emptyText = "";
            int testSpeakerId = 1;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                _voicevoxClient.SynthesizeSpeech(emptyText, testSpeakerId)
            );
            
            yield return null;
        }

        [UnityTest]
        public IEnumerator SynthesizeSpeech_InvalidSpeakerId_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            string testText = "テストメッセージ";
            int invalidSpeakerId = -1;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                _voicevoxClient.SynthesizeSpeech(testText, invalidSpeakerId)
            );
            
            yield return null;
        }

        [UnityTest]
        public IEnumerator GetAvailableSpeakers_ReturnsNonEmptyList()
        {
            // Act
            var speakers = _voicevoxClient.GetAvailableSpeakers();

            // Assert
            Assert.IsNotNull(speakers);
            Assert.IsTrue(speakers.Count > 0);
            
            yield return null;
        }

        [UnityTest]
        public IEnumerator GetSpeakerInfo_ValidSpeakerId_ReturnsSpeakerDetails()
        {
            // Arrange
            int testSpeakerId = 1;

            // Act
            var speakerInfo = _voicevoxClient.GetSpeakerInfo(testSpeakerId);

            // Assert
            Assert.IsNotNull(speakerInfo);
            Assert.AreEqual(testSpeakerId, speakerInfo.Id);
            
            yield return null;
        }
    }
}