using UnityEngine;
using System;
using System.IO;
using System.Threading.Tasks;
using UniVRM10;


namespace AIVTuber
{
    public class VRMModelLoader : MonoBehaviour
    {
        public Vrm10Instance CurrentModel { get; private set; }

        public async Task<GameObject> LoadVRMModel(string path)
        {
            Vrm10Instance loader = null;
            try
            {
                // 既存のモデルがあれば破棄
                if (CurrentModel != null)
                {
                    var oldGameObject = CurrentModel.gameObject;
                    CurrentModel = null;
                    Destroy(oldGameObject);
                }

                loader = await Vrm10.LoadPathAsync(path);
                if (loader == null)
                {
                    throw new AIVTuberException("Failed to load VRM file", ErrorType.VRMModel);
                }

                CurrentModel = loader;

                // モデルの初期設定
                var go = loader.gameObject;
                go.transform.SetParent(this.transform, false);

                return go;
            }
            catch (Exception ex)
            {
                throw new AIVTuberException($"Failed to load VRM model: {ex.Message}", ex, ErrorType.VRMModel);
            }
        }

        private void OnDestroy()
        {
            if (CurrentModel != null)
            {
                var go = CurrentModel.gameObject;
                CurrentModel = null;
                Destroy(go);
            }
        }
    }
}
