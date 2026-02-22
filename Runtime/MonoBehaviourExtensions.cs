using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RobsonRocha.UnityCommon
{
    /// <summary>
    /// Provides common initialization and visibility helpers for <see cref="MonoBehaviour"/> components.
    /// </summary>
    public static class MonoBehaviourExtensions
    {
        /// <summary>
        /// Determines whether the behaviour position is outside of the main camera viewport.
        /// </summary>
        /// <param name="behaviour">The behaviour to evaluate.</param>
        /// <returns><see langword="true"/> when outside the viewport; otherwise <see langword="false"/>.</returns>
        public static bool IsOffScreen(this MonoBehaviour behaviour)
        {
            if (Camera.main == null) return false;
            Vector3 viewportPos = Camera.main.WorldToViewportPoint(behaviour.transform.position);
            return viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1;
        }

        // Extracts the script file name from a full caller path.
        private static string GetScriptFileName(string path)
        {
            return string.IsNullOrEmpty(path) ? string.Empty : System.IO.Path.GetFileName(path);
        }

        /// <summary>
        /// Tries to initialize a required component from the same game object.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <param name="behaviour">The owner behaviour.</param>
        /// <param name="component">The component reference to populate.</param>
        /// <param name="isOptional">Whether missing component should be tolerated.</param>
        /// <param name="callerScriptPath">Caller script path used for diagnostics.</param>
        /// <returns><see langword="true"/> when the component is found; otherwise <see langword="false"/>.</returns>
        public static bool TryInitComponent<T>(
            this MonoBehaviour behaviour,
            [NotNullWhen(true)] ref T component, 
            bool isOptional = false, 
            [CallerFilePath] string callerScriptPath = null)
        {
            if (!behaviour.TryGetComponent(out component))
            {
                if (!isOptional)
                {
                    Debug.LogError($"The {GetScriptFileName(callerScriptPath)} script in {behaviour.gameObject.name} gameObject requires a {typeof(T).Name} component. Disabling the gameObject.");
                    behaviour.enabled = false;
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// Tries to initialize a component matching a predicate from the same game object.
        /// </summary>
        /// <typeparam name="T">The component type.</typeparam>
        /// <param name="behaviour">The owner behaviour.</param>
        /// <param name="component">The component reference to populate.</param>
        /// <param name="predicate">Predicate used to select one component instance.</param>
        /// <param name="isOptional">Whether missing component should be tolerated.</param>
        /// <param name="callerScriptPath">Caller script path used for diagnostics.</param>
        /// <returns><see langword="true"/> when a matching component is found; otherwise <see langword="false"/>.</returns>
        public static bool TryInitComponent<T>(this MonoBehaviour behaviour, ref T component, Func<T, bool> predicate, bool isOptional = false, [CallerFilePath] string callerScriptPath = null)
            where T : class
        {
            T[] components = behaviour.GetComponents<T>();
            if (components.Length == 0)
            {
                if (!isOptional)
                {
                    Debug.LogError($"The {GetScriptFileName(callerScriptPath)} script in {behaviour.gameObject.name} gameObject requires at least one {typeof(T).Name} component. Disabling the gameObject.");
                    behaviour.enabled = false;
                }
                return false;
            }
            component = components.FirstOrDefault(predicate);
            if (component == null)
            {
                if (!isOptional)
                {
                    Debug.LogError($"The {GetScriptFileName(callerScriptPath)} script in {behaviour.gameObject.name} gameObject requires at least one {typeof(T).Name} component that matches the predicate. Disabling the gameObject.");
                    behaviour.enabled = false;
                }
                return false;
            }
            return true;
        }

        public static void FadeAndDestroy(this MonoBehaviour behaviour, float intervalBeforeFadeAway)
        {
            if (behaviour.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
            {
                behaviour.StartCoroutine(FadeAndDestroyRoutine(spriteRenderer, intervalBeforeFadeAway));
            }
        }

        private static IEnumerator FadeAndDestroyRoutine(SpriteRenderer spriteRenderer, float intervalBeforeFadeAway)
        {
            yield return new WaitForSeconds(intervalBeforeFadeAway);
            float fadeDuration = 0.25f;
            float elapsed = 0f;
            Color startColor = spriteRenderer.color;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }
            UnityEngine.Object.Destroy(spriteRenderer.gameObject);
        }

    }
}
