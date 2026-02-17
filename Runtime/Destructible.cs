using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

/// <summary>
/// Delegate executed before this object is destroyed.
/// Return <see langword="true"/> to continue destruction.
/// </summary>
/// <param name="destructible">The destructible being processed.</param>
/// <returns><see langword="true"/> to allow destruction; otherwise <see langword="false"/>.</returns>
public delegate bool BeforeDestroyHandler(Destructible destructible);
/// <summary>
/// Delegate executed before damage is applied.
/// Return <see langword="true"/> to continue and optionally adjust remaining health.
/// </summary>
/// <param name="destructible">The destructible receiving damage.</param>
/// <param name="damage">The incoming damage value.</param>
/// <param name="newRemainingHealth">The computed remaining health, which can be modified.</param>
/// <returns><see langword="true"/> to apply damage; otherwise <see langword="false"/>.</returns>
public delegate bool BeforeTakeDamageHandler(Destructible destructible, float damage, ref float newRemainingHealth);

[RequireComponent(typeof(Renderer))]
[SuppressMessage("Major Code Smell", "S3264:Events should be invoked", Justification = "Events called by extensions")]
/// <summary>
/// Represents a pooled object that can take damage and self-destruct with optional effects.
/// </summary>
public class Destructible : MonoBehaviour, IPoolable, IManagedStateContributor
{
    /// <summary>
    /// The current health value.
    /// </summary>
    public float HealthCurrent = 10f;
    /// <summary>
    /// The maximum health value.
    /// </summary>
    public float HealthMax = 10f;

    /// <summary>
    /// Optional pool name for an explosion prefab spawned on destruction.
    /// </summary>
    public string ExplosionPrefabName;

    private string _myPoolName;
    private bool _isDestroyed;
    private FlashEffect _flashEffect;
    private FadeOutEffect _fadeOutEffect;
    private FragmenterEffect _fragmenterEffect;
    private KnockbackMovement _knockbackMovement;
    private ManagedState _managedState;

    /// <summary>
    /// Indicates whether this object has been destroyed.
    /// </summary>
    public bool IsDestroyed => _isDestroyed;
    /// <summary>
    /// Controls whether this object can currently receive damage.
    /// </summary>
    public bool CanTakeDamage { get; set; } = true;

    /// <summary>
    /// Invoked before the object starts its destruction flow.
    /// </summary>
    public event BeforeDestroyHandler OnBeforeDestroy;
    /// <summary>
    /// Invoked before damage is applied.
    /// </summary>
    public event BeforeTakeDamageHandler OnBeforeTakeDamage;

    /// <summary>
    /// Initializes pooled references when the object is created by the pool.
    /// </summary>
    /// <param name="poolName">The pool name used by this instance.</param>
    public void OnPoolableCreate(string poolName)
    {
        _myPoolName = poolName;
        this.TryInitComponent(ref _flashEffect, isOptional: true);
        this.TryInitComponent(ref _fadeOutEffect, isOptional: true);
        this.TryInitComponent(ref _fragmenterEffect, isOptional: true);
        this.TryInitComponent(ref _knockbackMovement, isOptional: true);
        this.TryInitComponent(ref _managedState, isOptional: true);
    }

    /// <summary>
    /// Restores persisted state values when a loading cycle occurs.
    /// </summary>
    public void OnLoadingState()
    {
        if (_managedState != null)
        {
            StateManager.Instance.TryGetStateValue(_managedState.Id, nameof(IsDestroyed), ref _isDestroyed);
            StateManager.Instance.TryGetStateValue(_managedState.Id, nameof(HealthCurrent), ref HealthCurrent);
            StateManager.Instance.TryGetStateValue(_managedState.Id, nameof(HealthMax), ref HealthMax);
            if (_isDestroyed)
            {
                DoSelfDestruct();
            }
            else
            {
                if (_fadeOutEffect != null)
                {
                    _fadeOutEffect.ResetFade();
                }
                if (_fragmenterEffect != null)
                {
                    _fragmenterEffect.ResetFragmenter();
                }
            }
        }
    }

    /// <summary>
    /// Applies damage to this object and triggers related effects.
    /// </summary>
    /// <param name="damage">The amount of damage to apply.</param>
    /// <param name="damageDirection">The direction used for knockback feedback.</param>
    /// <returns><see langword="true"/> when damage is applied; otherwise <see langword="false"/>.</returns>
    public bool TakeDamage(float damage, Vector2 damageDirection)
    {
        if (_isDestroyed || !CanTakeDamage)
        {
            return false;
        }

        float remainingHealth = Mathf.Max(HealthCurrent - damage, 0f);
        if (!OnBeforeTakeDamage.InvokeMany(
            true,
            (d) => d.Invoke(this, damage, ref remainingHealth)).All())
        {
            return false;
        }
        HealthCurrent = remainingHealth;

        if (HealthCurrent.IsNearZero())
        {
            _isDestroyed = true;
            if (OnBeforeDestroy.InvokeMany<BeforeDestroyHandler, bool>(true, this).All())
            {
                StartCoroutine(SelfDestruct());
            }
        }
        else
        {
            if (_knockbackMovement != null)
            {
                _knockbackMovement.StartKnockback(damageDirection);
            }
            StartFlash();
        }
        return true;
    }

    /// <summary>
    /// Starts the flash effect, when available.
    /// </summary>
    void StartFlash()
    {
        if (_flashEffect != null)
        {
            _flashEffect.StartFlash();
        }
    }

    /// <summary>
    /// Executes the destruction sequence and waits for visual effects to complete.
    /// </summary>
    /// <returns>An enumerator used by Unity coroutines.</returns>
    private IEnumerator SelfDestruct()
    {
        if (!string.IsNullOrEmpty(ExplosionPrefabName))
        {
            PoolManager.Instance.Get(ExplosionPrefabName, transform.position, Quaternion.identity);
        }
        if (_fragmenterEffect != null)
        {
            _fragmenterEffect.BreakApart();
            yield return new WaitUntil(() => _isDestroyed &&
                ((_fragmenterEffect == null) || !_fragmenterEffect.IsFragmenting));
        }
        else
        {
            StartFlash();
            if (_fadeOutEffect != null)
            {
                _fadeOutEffect.StartFadeOut();
            }

            yield return new WaitUntil(() => _isDestroyed &&
                ((_flashEffect == null) || !_flashEffect.IsFlashing) &&
                ((_fadeOutEffect == null) || !_fadeOutEffect.IsFading));
        }

        DoSelfDestruct();
    }

    /// <summary>
    /// Recycles this object back to its pool.
    /// </summary>
    private void DoSelfDestruct()
    {
        PoolManager.Instance.Recycle(_myPoolName, gameObject);
    }

    /// <summary>
    /// Persists relevant runtime state values.
    /// </summary>
    public void OnSavingState()
    {
        if (_managedState != null)
        {
            StateManager.Instance.SetStateValue(_managedState.Id, nameof(IsDestroyed), _isDestroyed);
            StateManager.Instance.SetStateValue(_managedState.Id, nameof(HealthCurrent), HealthCurrent);
            StateManager.Instance.SetStateValue(_managedState.Id, nameof(HealthMax), HealthMax);
        }
    }
}
