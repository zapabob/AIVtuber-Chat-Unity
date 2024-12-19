using System;
using System.Collections.Generic;
using UnityEngine;
using UniVRM10;
using VRM10;

namespace AIVtuberApp.Services
{
    public class VrmAnimationService : MonoBehaviour
    {
        private Vrm10Instance _currentAvatar;
        private Dictionary<string, AnimationClip> _animationClips;
        private Animator _animator;
        private Vrm10RuntimeExpression _expression;

        public VrmAnimationService()
        {
            _animationClips = new Dictionary<string, AnimationClip>();
            InitializeAnimationClips();
        }

        private void InitializeAnimationClips()
        {
            // デフォルトのアニメーションクリップをロード
            _animationClips["idle"] = Resources.Load<AnimationClip>("Animations/Idle");
            _animationClips["talk"] = Resources.Load<AnimationClip>("Animations/Talk");
            _animationClips["smile"] = Resources.Load<AnimationClip>("Animations/Smile");
            _animationClips["surprise"] = Resources.Load<AnimationClip>("Animations/Surprise");
        }

        public void LoadAvatar(Vrm10Instance avatar)
        {
            _currentAvatar = avatar;
            if (_currentAvatar != null)
            {
                _animator = _currentAvatar.GetComponent<Animator>();
                _expression = _currentAvatar.Runtime.Expression;
            }
        }

        public void PlayAnimation(string animationName)
        {
            if (_currentAvatar == null || _animator == null)
            {
                Debug.LogWarning("No avatar or animator component loaded.");
                return;
            }

            if (_animationClips.TryGetValue(animationName, out AnimationClip clip))
            {
                _animator.Play(clip.name, 0, 0f);
            }
            else
            {
                Debug.LogWarning($"Animation '{animationName}' not found.");
            }
        }

        public void SetExpression(string expressionName)
        {
            if (_currentAvatar == null || _expression == null)
            {
                Debug.LogWarning("No avatar or expression component loaded.");
                return;
            }

            // 前の表情をリセット
            _expression.ResetAllExpressions();

            switch (expressionName.ToLower())
            {
                case "neutral":
                    _expression.SetWeight(ExpressionKey.Neutral, 1.0f);
                    break;
                case "happy":
                    _expression.SetWeight(ExpressionKey.Happy, 1.0f);
                    break;
                case "angry":
                    _expression.SetWeight(ExpressionKey.Angry, 1.0f);
                    break;
                case "sad":
                    _expression.SetWeight(ExpressionKey.Sad, 1.0f);
                    break;
                default:
                    Debug.LogWarning($"Unknown expression: {expressionName}");
                    break;
            }
        }

        public void LipSync(float[] audioLevels)
        {
            if (_currentAvatar == null || _expression == null)
            {
                Debug.LogWarning("No avatar or expression component loaded.");
                return;
            }

            float mouthOpenLevel = CalculateMouthOpenLevel(audioLevels);
            _expression.SetWeight(ExpressionKey.Aa, mouthOpenLevel);
        }

        private float CalculateMouthOpenLevel(float[] audioLevels)
        {
            if (audioLevels == null || audioLevels.Length == 0)
            {
                return 0f;
            }

            float averageLevel = 0f;
            foreach (float level in audioLevels)
            {
                averageLevel += level;
            }
            averageLevel /= audioLevels.Length;

            return Mathf.Clamp01(averageLevel * 2f);
        }
    }
}