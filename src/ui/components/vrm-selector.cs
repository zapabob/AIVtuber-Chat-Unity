using System;
using System.Collections.Generic;
using UnityEngine;
using VRM;
using UniGLTF;
using System.Threading.Tasks;
using UnityEngine.UI;
using System.IO;

namespace AIVtuberApp.UI.Components
{
    public class VRMSelector : MonoBehaviour
    {
        [SerializeField] private Button selectButton;
        [SerializeField] private Text modelPathText;
        
        private string currentVRMPath;
        private GameObject currentVRMModel;
        
        private void Start()
        {
            if (selectButton != null)
            {
                selectButton.onClick.AddListener(OnSelectButtonClick);
            }
        }

        private async void OnSelectButtonClick()
        {
            var path = OpenFileDialog();
            if (string.IsNullOrEmpty(path)) return;

            await LoadVRMModel(path);
        }

        private string OpenFileDialog()
        {
            var extensions = new[] { "vrm", "glb" };
            var path = UnityEditor.EditorUtility.OpenFilePanelWithFilters(
                "Select VRM Model",
                "",
                new[] { "VRM Files", "vrm,glb", "All Files", "*" }
            );
            
            return path;
        }

        private async Task LoadVRMModel(string path)
        {
            try
            {
                // 既存のモデルを破棄
                if (currentVRMModel != null)
                {
                    Destroy(currentVRMModel);
                }

                // VRMファイルを読み込む
                var bytes = File.ReadAllBytes(path);
                var context = new VRMImporterContext();
                
                // GLBとしてパース
                await context.ParseGlbAsync(bytes);
                
                // メタデータを取得
                var meta = context.ReadMeta(true);
                
                // モデルをロード
                await context.LoadAsync();
                
                currentVRMModel = context.Root;
                currentVRMPath = path;
                
                // UIを更新
                if (modelPathText != null)
                {
                    modelPathText.text = Path.GetFileName(path);
                }
                
                // モデルを適切な位置に配置
                currentVRMModel.transform.position = Vector3.zero;
                currentVRMModel.transform.rotation = Quaternion.identity;
                currentVRMModel.transform.localScale = Vector3.one;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load VRM model: {ex.Message}");
                if (modelPathText != null)
                {
                    modelPathText.text = "Error: Failed to load model";
                }
            }
        }

        private void OnDestroy()
        {
            if (selectButton != null)
            {
                selectButton.onClick.RemoveListener(OnSelectButtonClick);
            }
            
            if (currentVRMModel != null)
            {
                Destroy(currentVRMModel);
            }
        }
    }
}