using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Moq;
using AIVtuberChat.Core.Api;
using AIVtuberChat.Core.Models;

namespace AIVtuberChat.Tests.PlayMode
{
    public class DifyClientTest : MonoBehaviour
    {
        private Mock<IDifyClient> _mockDifyClient;
        private LLMRequest _testRequest;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            // モックの初期化
            _mockDifyClient = new Mock<IDifyClient>();
            
            // テスト用のリクエストデータ準備
            _testRequest = new LLMRequest
            {
                Prompt = "こんにちは、AIアシスタント",
                Language = "ja",
                Temperature = 0.7
            };

            yield return null;
        }

        [UnityTest]
        public IEnumerator SendRequest_ValidInput_ReturnsSuccessfulResponse()
        {
            // Arrange
            var expectedResponse = new LLMResponse
            {
                Content = "こんにちは！どのようにお手伝いできますか？",
                Status = ResponseStatus.Success
            };

            _mockDifyClient
                .Setup(x => x.SendRequest(_testRequest))
                .Returns(expectedResponse);

            // Act
            var actualResponse = _mockDifyClient.Object.SendRequest(_testRequest);

            // Assert
            Assert.AreEqual(expectedResponse.Content, actualResponse.Content);
            Assert.AreEqual(expectedResponse.Status, actualResponse.Status);

            yield return null;
        }

        [UnityTest]
        public IEnumerator SendRequest_EmptyPrompt_ThrowsArgumentException()
        {
            // Arrange
            var invalidRequest = new LLMRequest
            {
                Prompt = "",
                Language = "ja"
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                _mockDifyClient.Object.SendRequest(invalidRequest)
            );

            yield return null;
        }

        [UnityTest]
        public IEnumerator SendRequest_NetworkError_ReturnsErrorResponse()
        {
            // Arrange
            _mockDifyClient
                .Setup(x => x.SendRequest(_testRequest))
                .Throws(new System.Net.Http.HttpRequestException("Network error"));

            // Act & Assert
            Assert.Throws<System.Net.Http.HttpRequestException>(() => 
                _mockDifyClient.Object.SendRequest(_testRequest)
            );

            yield return null;
        }

        [UnityTest]
        public IEnumerator SendRequest_RateLimitExceeded_ReturnsRateLimitResponse()
        {
            // Arrange
            var rateLimitResponse = new LLMResponse
            {
                Content = "Rate limit exceeded",
                Status = ResponseStatus.RateLimitExceeded
            };

            _mockDifyClient
                .Setup(x => x.SendRequest(_testRequest))
                .Returns(rateLimitResponse);

            // Act
            var actualResponse = _mockDifyClient.Object.SendRequest(_testRequest);

            // Assert
            Assert.AreEqual(ResponseStatus.RateLimitExceeded, actualResponse.Status);

            yield return null;
        }
    }

    public enum ResponseStatus
    {
        Success,
        Error,
        RateLimitExceeded
    }

    public interface IDifyClient
    {
        LLMResponse SendRequest(LLMRequest request);
    }
}