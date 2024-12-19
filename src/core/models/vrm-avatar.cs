using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VRM;
using UniGLTF;
using VRMShaders;

namespace AIVtuberChat.Core.Models
{
    /// <summary>
    /// VRMアバターのモデルを表現するクラス
    /// </summary>
    public class VrmAvatar : IDisposable
    {
        // アバターのユニークID
        public string Id { get; private set; }

        // アバターのメタデータ
        public VrmAvatarMetadata Metadata { get; private set; }

        // VRMモデルのGameObject
        private GameObject avatarGameObject;

        // VRMインスタンス
        private RuntimeGltfInstance vrmInstance;

        // 表情管理
        private Dictionary<string, BlendShapeKey> expressionKeys;

        // 物理挙動管理
        private VRMSpringBoneColliderGroup[] springBoneColliders;

        /// <summary>
        /// コンストラクター
        /// </summary>
        public VrmAvatar()
        {
            Id = Guid.NewGuid().ToString();
            expressionKeys = new Dictionary<string, BlendShapeKey>();
        }

        /// <summary>
        /// VRMアバターを非同期でロードする
        /// </summary>
        /// <param name="vrmFilePath">VRMファイルのパス</param>
        public async Task LoadAvatarAsync(string vrmFilePath)
        {
            try
            {
                // VRMファイルを非同期でロード
                var bytes = await System.IO.File.ReadAllBytesAsync(vrmFilePath);
                using (var gltfData = new GltifeData(bytes))
                {
                    using (var loader = new VRMImporterContext(gltfData))
                    {
                        // メタデータの読み込み
                        var meta = await loader.ReadMetaAsync();
                        Metadata = new VrmAvatarMetadata
                        {
                            Name = meta.Name,
                            Author = meta.Author,
                            Version = meta.Version
                        };

                        // モデルの読み込みと実体化
                        RuntimeGltfInstance instance = await loader.LoadAsync(new RuntimeOnlyAwaitCaller());
                        vrmInstance = instance;
                        avatarGameObject = instance.Root;

                        // 表情とボーンの初期化
                        InitializeExpressions();
                        InitializeSpringBones();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"VRMアバターのロードに失敗: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 表情を初期化
        /// </summary>
        private void InitializeExpressions()
        {
            var proxy = avatarGameObject.GetComponent<VRMBlendShapeProxy>();
            if (proxy != null)
            {
                // プリセット表情を登録
                expressionKeys.Add("Neutral", BlendShapeKey.CreateFromPreset(BlendShapePreset.Neutral));
                expressionKeys.Add("Joy", BlendShapeKey.CreateFromPreset(BlendShapePreset.Joy));
                expressionKeys.Add("Angry", BlendShapeKey.CreateFromPreset(BlendShapePreset.Angry));
                expressionKeys.Add("Sorrow", BlendShapeKey.CreateFromPreset(BlendShapePreset.Sorrow));
                expressionKeys.Add("Fun", BlendShapeKey.CreateFromPreset(BlendShapePreset.Fun));
            }
        }

        /// <summary>
        /// スプリングボーンを初期化
        /// </summary>
        private void InitializeSpringBones()
        {
            springBoneColliders = avatarGameObject.GetComponentsInChildren<VRMSpringBoneColliderGroup>();
        }

        /// <summary>
        /// 特定の表情を設定
        /// </summary>
        /// <param name="expressionName">表情名</param>
        /// <param name="weight">表情の強さ (0.0f ~ 1.0f)</param>
        public void SetExpression(string expressionName, float weight = 1.0f)
        {
            if (expressionKeys.TryGetValue(expressionName, out BlendShapeKey key))
            {
                var proxy = avatarGameObject.GetComponent<VRMBlendShapeProxy>();
                if (proxy != null)
                {
                    proxy.SetValue(key, Mathf.Clamp01(weight));
                }
            }
        }

        /// <summary>
        /// リソースの解放
        /// </summary>
        public void Dispose()
        {
            if (vrmInstance != null)
            {
                vrmInstance.Dispose();
                vrmInstance = null;
            }

            if (avatarGameObject != null)
            {
                GameObject.Destroy(avatarGameObject);
                avatarGameObject = null;
            }

            expressionKeys.Clear();
        }
    }
}
