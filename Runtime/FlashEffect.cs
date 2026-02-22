using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobsonRocha.UnityCommon
{
    [RequireComponent(typeof(Renderer))]
    public class FlashEffect : MonoBehaviour
    {
        [SerializeField] private float FlashDuration = 0.125f;
        [SerializeField] private AnimationCurve FlashCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
        [SerializeField] private Color FlashColor = Color.white;

        [SerializeField] private float BlinkDuration = 1f;
        [SerializeField] private float BlinkFrequency = 0.25f;
        [SerializeField] private AnimationCurve BlinkCurve = new(
            new Keyframe(0f, 0f, 0f, 0f),
            new Keyframe(0.5f, 0f, 0f, 0f),
            new Keyframe(0.5f, 1f, 0f, 0f),
            new Keyframe(1f, 1f, 0f, 0f));
        [SerializeField] private Color BlinkColor = Color.white;

        private struct TintSource
        {
            public Color Color;
            public float Amount;

            public TintSource(Color color, float amount)
            {
                Color = color;
                Amount = amount;
            }
        }

        private const string FlashKey = "flash";
        private const string BlinkKey = "blink";

        private SpriteRenderer _renderer;
        private Material _tintMaterial;
        private float _flashTimer;
        private Material _originalMaterial;
        private Coroutine _flashCoroutine;
        private Coroutine _blinkCoroutine;
        private readonly Dictionary<string, TintSource> _tintSources = new();

        public bool IsFlashing => _flashCoroutine != null;
        public bool IsBlinking => _blinkCoroutine != null;

        void Awake()
        {
            this.TryInitComponent(ref _renderer);
            _originalMaterial = _renderer.material;

            _tintMaterial = new Material(Resources.Load<Material>(ColorTintShaderRefs.MaterialName));
            _tintMaterial.SetColor(ColorTintShaderRefs.TintColor, Color.white);
            _tintMaterial.SetFloat(ColorTintShaderRefs.TintAmount, 0f);

            // Copy the sprite texture to the tint material
            if (_renderer.sprite != null)
            {
                _tintMaterial.mainTexture = _renderer.sprite.texture;
            }

            // Warm up the shader by briefly applying the material
            _renderer.material = _tintMaterial;
            _renderer.material = _originalMaterial;
        }

        void OnDisable()
        {
            StopFlash();
            StopBlinking();
            ResetTint();
        }

        public void StartFlash(float? duration = null, AnimationCurve curve = null, Color? color = null)
        {
            StopFlash();
            float usedDuration = duration ?? FlashDuration;
            AnimationCurve usedCurve = curve ?? FlashCurve;
            Color usedColor = color ?? FlashColor;

            if (usedDuration.IsAboveNearZero())
            {
                _flashCoroutine = StartCoroutine(FlashCoroutine(usedDuration, usedCurve, usedColor));
            }
        }

        public void StartBlinking(float? duration = null, float? frequency = null, Color? color = null, AnimationCurve curve = null)
        {
            StopBlinking();
            float usedDuration = duration ?? BlinkDuration;
            float usedFrequency = Mathf.Max(frequency ?? BlinkFrequency, 0.0001f);
            Color usedColor = color ?? BlinkColor;
            AnimationCurve usedCurve = curve ?? BlinkCurve;

            if (usedDuration.IsAboveNearZero())
            {
                _blinkCoroutine = StartCoroutine(BlinkCoroutine(usedDuration, usedFrequency, usedColor, usedCurve));
            }
        }

        public void StopFlash()
        {
            if (_flashCoroutine != null)
            {
                StopCoroutine(_flashCoroutine);
                _flashCoroutine = null;
            }

            _tintSources.Remove(FlashKey);
            ApplyTint();
        }

        public void StopBlinking()
        {
            if (_blinkCoroutine != null)
            {
                StopCoroutine(_blinkCoroutine);
                _blinkCoroutine = null;
            }

            _tintSources.Remove(BlinkKey);
            ApplyTint();
        }

        protected IEnumerator FlashCoroutine(float duration, AnimationCurve curve, Color color)
        {
            _flashTimer = duration;

            while (_flashTimer.IsAboveNearZero())
            {
                _flashTimer = _flashTimer.DecrementTimer();
                float normalizedTime = 1f - Mathf.Clamp01(_flashTimer / duration);
                float flashAmount = Mathf.Clamp01(curve.Evaluate(normalizedTime));

                _tintSources[FlashKey] = new TintSource(color, flashAmount);
                ApplyTint();

                yield return null;
            }

            _tintSources.Remove(FlashKey);
            ApplyTint();
            _flashCoroutine = null;
        }

        private IEnumerator BlinkCoroutine(float duration, float frequency, Color color, AnimationCurve curve)
        {
            float elapsed = 0f;
            float cycleTimer = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                cycleTimer += Time.deltaTime;

                float normalized = Mathf.Clamp01(cycleTimer / frequency);
                float blinkAmount = Mathf.Clamp01(curve.Evaluate(normalized));

                _tintSources[BlinkKey] = new TintSource(color, blinkAmount);
                ApplyTint();

                if (cycleTimer >= frequency)
                {
                    cycleTimer = 0f;
                }

                yield return null;
            }

            _tintSources.Remove(BlinkKey);
            ApplyTint();
            _blinkCoroutine = null;
        }

        private void ApplyTint()
        {
            if (_tintSources.Count == 0)
            {
                ResetTint();
                return;
            }

            if (_renderer.material != _tintMaterial)
            {
                _renderer.material = _tintMaterial;
            }

            // Sync texture with current animation frame
            if (_renderer.sprite != null)
            {
                _tintMaterial.mainTexture = _renderer.sprite.texture;
            }

            float totalWeight = 0f;
            float maxAmount = 0f;
            Color accum = Color.black;

            foreach (var source in _tintSources.Values)
            {
                float amount = Mathf.Clamp01(source.Amount);
                accum += source.Color * amount;
                totalWeight += amount;
                if (amount > maxAmount)
                    maxAmount = amount;
            }

            Color blendedColor = totalWeight > 0f ? accum / totalWeight : Color.black;

            _tintMaterial.SetColor(ColorTintShaderRefs.TintColor, blendedColor);
            _tintMaterial.SetFloat(ColorTintShaderRefs.TintAmount, maxAmount);
        }

        private void ResetTint()
        {
            _tintSources.Clear();

            if (_renderer != null && _originalMaterial != null)
            {
                _renderer.material = _originalMaterial;
            }

            if (_tintMaterial != null)
            {
                _tintMaterial.SetFloat(ColorTintShaderRefs.TintAmount, 0f);
            }
        }
    }
}
