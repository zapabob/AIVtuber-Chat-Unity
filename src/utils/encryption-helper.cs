using UnityEngine;
using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace AIVtuberChat.Utils
{
    /// <summary>
    /// APIキーや機密情報の暗号化・復号化を行うヘルパークラス
    /// </summary>
    public class EncryptionHelper : MonoBehaviour
    
    {
        private const int KeySize = 256;
        private const int BlockSize = 128;
        
        // 暗号化に使用する鍵とIVを実行時に生成（初回のみ）
        private static byte[] _key = null;
        private static byte[] _iv = null;

        private static void InitializeKeyAndIV()
        {
            if (_key == null || _iv == null)
            {
                // 保存された鍵とIVの取得を試みる
                string savedKey = PlayerPrefs.GetString("EncryptionKey", "");
                string savedIV = PlayerPrefs.GetString("EncryptionIV", "");

                if (string.IsNullOrEmpty(savedKey) || string.IsNullOrEmpty(savedIV))
                {
                    // 新しい鍵とIVを生成
                    using (var aes = Aes.Create())
                    {
                        aes.KeySize = KeySize;
                        aes.BlockSize = BlockSize;
                        aes.GenerateKey();
                        aes.GenerateIV();
                        _key = aes.Key;
                        _iv = aes.IV;

                        // 生成した鍵とIVを保存
                        PlayerPrefs.SetString("EncryptionKey", Convert.ToBase64String(_key));
                        PlayerPrefs.SetString("EncryptionIV", Convert.ToBase64String(_iv));
                        PlayerPrefs.Save();
                    }
                }
                else
                {
                    // 保存された鍵とIVを復元
                    _key = Convert.FromBase64String(savedKey);
                    _iv = Convert.FromBase64String(savedIV);
                }
            }
        }

        /// <summary>
        /// 文字列を暗号化する
        /// </summary>
        /// <param name="plainText">暗号化する平文</param>
        /// <returns>暗号化された文字列</returns>
        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            InitializeKeyAndIV();

            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.KeySize = KeySize;
                    aes.BlockSize = BlockSize;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = _key;
                    aes.IV = _iv;

                    using (var encryptor = aes.CreateEncryptor())
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }

                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"暗号化中にエラーが発生: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 暗号化された文字列を復号化する
        /// </summary>
        /// <param name="encryptedText">暗号化された文字列</param>
        /// <returns>復号化された平文</returns>
        public static string Decrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
                return string.Empty;

            InitializeKeyAndIV();

            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.KeySize = KeySize;
                    aes.BlockSize = BlockSize;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = _key;
                    aes.IV = _iv;

                    using (var decryptor = aes.CreateDecryptor())
                    {
                        byte[] cipherBytes = Convert.FromBase64String(encryptedText);
                        using (var msDecrypt = new MemoryStream(cipherBytes))
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"復号化中にエラーが発生: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// APIキーを安全に保存する
        /// </summary>
        /// <param name="apiKey">暗号化して保存するAPIキー</param>
        public static void SaveApiKey(string apiKey)
        {
            try
            {
                string encryptedApiKey = Encrypt(apiKey);
                PlayerPrefs.SetString("EncryptedApiKey", encryptedApiKey);
                PlayerPrefs.Save();
            }
            catch (Exception ex)
            {
                Debug.LogError($"APIキーの保存中にエラーが発生: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 保存されたAPIキーを取得する
        /// </summary>
        /// <returns>復号化されたAPIキー</returns>
        public static string GetApiKey()
        {
            try
            {
                string encryptedApiKey = PlayerPrefs.GetString("EncryptedApiKey", "");
                return string.IsNullOrEmpty(encryptedApiKey) ? "" : Decrypt(encryptedApiKey);
            }
            catch (Exception ex)
            {
                Debug.LogError($"APIキーの取得中にエラーが発生: {ex.Message}");
                throw;
            }
        }
    }
}
