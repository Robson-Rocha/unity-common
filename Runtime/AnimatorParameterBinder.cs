using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RobsonRocha.UnityCommon
{
    /// <summary>
    /// Binds animator parameters to value factories and updates them only when values change.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimatorParameterBinder : MonoBehaviour
    {
        public enum BoolTriggerBehavior
        {
            ManualTrigger,
            TriggerWhenTrue,
            TriggerWhenChanged
        }

        private class BoolTrigger
        {
            public string ParameterName;
            public string StateName;
            public Func<bool> ValueFactory;
            public BoolTriggerBehavior Behavior;
            public bool ShouldRestartWhenRetrigger;
            public BoolTrigger(string parameterName, string stateName, Func<bool> valueFactory, BoolTriggerBehavior behavior, bool shouldRestartWhenRetrigger)
            {
                ParameterName = parameterName;
                StateName = stateName;
                ValueFactory = valueFactory;
                Behavior = behavior;
                ShouldRestartWhenRetrigger = shouldRestartWhenRetrigger;
            }
        }

        private Animator _animator; // Caches the animator component.

        private readonly Dictionary<string, Func<bool>> _boolBinds = new(); // Stores bound boolean parameter factories.
        private readonly Dictionary<string, Func<int>> _integerBinds = new(); // Stores bound integer parameter factories.
        private readonly Dictionary<string, Func<float>> _floatBinds = new(); // Stores bound float parameter factories.

        private readonly Dictionary<string, bool> _previousBoolValues = new(); // Stores the last applied boolean value per parameter.
        private readonly Dictionary<string, int> _previousIntegerValues = new(); // Stores the last applied integer value per parameter.
        private readonly Dictionary<string, float> _previousFloatValues = new(); // Stores the last applied float value per parameter.

        private readonly Dictionary<string, BoolTrigger> _triggerBinds = new(); // Stores bound boolean parameter factories that trigger when true, with options for retriggering behavior.

        // Initializes component references.
        private void Awake()
        {
            this.TryInitComponent(ref _animator);
        }

        /// <summary>
        /// Binds a parameter name to a value factory for supported animator parameter types.
        /// </summary>
        /// <typeparam name="T">The parameter type. Supported types are <see cref="bool"/>, <see cref="int"/>, and <see cref="float"/>.</typeparam>
        /// <param name="parameterName">The animator parameter name.</param>
        /// <param name="valueFactory">Factory that returns the current value for the parameter.</param>
        public void Bind<T>(string parameterName, Func<T> valueFactory)
        {
            if (!IsParameterNameUnused(parameterName))
            {
                Debug.LogError($"Parameter name '{parameterName}' is already bound to a value factory.");
                return;
            }
            if (valueFactory == null)
            {
                Debug.LogError("Value factory cannot be null.");
                return;
            }
            if (typeof(T) == typeof(bool))
            {
                _boolBinds[parameterName] = () => (bool)(object)valueFactory();
            }
            else if (typeof(T) == typeof(int))
            {
                _integerBinds[parameterName] = () => (int)(object)valueFactory();
            }
            else if (typeof(T) == typeof(float))
            {
                _floatBinds[parameterName] = () => (float)(object)valueFactory();
            }
            else
            {
                Debug.LogError($"Unsupported type: {typeof(T)}");
            }
        }

        private bool IsParameterNameUnused(string parameterName) =>
            !_boolBinds.ContainsKey(parameterName) &&
            !_integerBinds.ContainsKey(parameterName) &&
            !_floatBinds.ContainsKey(parameterName) &&
            !_triggerBinds.ContainsKey(parameterName);

        public void BindTrigger(
            string parameterName,
            BoolTriggerBehavior behavior = BoolTriggerBehavior.ManualTrigger,
            Func<bool> valueFactory = null, 
            bool shouldRestartWhenRetrigger = false,
            string stateName = null)
        {
            if (!IsParameterNameUnused(parameterName))
            {
                Debug.LogError($"Parameter name '{parameterName}' is already bound to a value factory.");
                return;
            }
            if (behavior != BoolTriggerBehavior.ManualTrigger && valueFactory == null)
            {
                Debug.LogError("Value factory must be provided for non-manual trigger behaviors.");
                return;
            }

            if (shouldRestartWhenRetrigger && stateName == null)
            {
                Debug.LogError("State name must be provided when shouldRestartWhenRetrigger is true.");
                return;
            }

            _triggerBinds[parameterName] =
                new BoolTrigger(parameterName, stateName, valueFactory, behavior, shouldRestartWhenRetrigger);
        }

        // Applies changed bound values to animator parameters.
        private void IterateDictionary<T>(Dictionary<string, Func<T>> valueBinds, Dictionary<string, T> previousValues)
            where T : IEquatable<T>
        {
            foreach (var bind in valueBinds)
            {
                T newValue = bind.Value();
                if (!previousValues.TryGetValue(bind.Key, out T previousValue) ||
                    !previousValue.Equals(newValue))
                {
                    switch (typeof(T))
                    {
                        case Type t when t == typeof(int):
                            _animator.SetInteger(bind.Key, Convert.ToInt32(newValue));
                            break;
                        case Type t when t == typeof(float):
                            _animator.SetFloat(bind.Key, Convert.ToSingle(newValue));
                            break;
                        case Type t when t == typeof(bool):
                            _animator.SetBool(bind.Key, Convert.ToBoolean(newValue));
                            break;
                        default:
                            throw new InvalidOperationException($"Unsupported type: {typeof(T)}");
                    }
                    previousValues[bind.Key] = newValue;
                }
            }
        }

        void IterateTriggers()
        {
            foreach (BoolTrigger trigger in _triggerBinds.Values.Where(trigger => trigger.Behavior != BoolTriggerBehavior.ManualTrigger))
            {
                bool currentValue = trigger.ValueFactory();
                bool previousValue = _previousBoolValues.TryGetValue(trigger.ParameterName, out previousValue) && previousValue;
                bool shouldTrigger = (trigger.Behavior == BoolTriggerBehavior.TriggerWhenTrue && currentValue != previousValue && currentValue) ||
                                     (trigger.Behavior == BoolTriggerBehavior.TriggerWhenChanged && currentValue != previousValue);
                if (shouldTrigger)
                {
                    FireTrigger(trigger);
                }
                _previousBoolValues[trigger.ParameterName] = currentValue;
            }
        }

        public void FireTrigger(string key)
        {
            if (_triggerBinds.TryGetValue(key, out BoolTrigger trigger))
            {
                FireTrigger(trigger);
            }
            else
            {
                Debug.LogWarning($"No trigger bound for key: {key}");
            }
        }

        void FireTrigger(BoolTrigger trigger)
        {
            if (trigger.ShouldRestartWhenRetrigger)
            {
                _animator.Play(trigger.StateName, 0, 0f);
            }
            _animator.SetTrigger(trigger.ParameterName);
        }

        // Updates all bound animator parameters each frame.
        void LateUpdate()
        {
            if (_animator == null)
            {
                return;
            }
            IterateDictionary(_boolBinds, _previousBoolValues);
            IterateDictionary(_integerBinds, _previousIntegerValues);
            IterateDictionary(_floatBinds, _previousFloatValues);
            IterateTriggers();
        }
    }
}