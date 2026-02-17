using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class MonoBehaviourExtensions
{
    public static bool IsOffScreen(this MonoBehaviour behaviour)
    {
        if (Camera.main == null) return false;
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(behaviour.transform.position);
        return viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1;
    }

    private static string GetScriptFileName(string path)
    {
        return string.IsNullOrEmpty(path) ? string.Empty : System.IO.Path.GetFileName(path);
    }

    public static bool TryInitComponent<T>(this MonoBehaviour behaviour, ref T component, bool isOptional = false, [CallerFilePath] string callerScriptPath = null)
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
}