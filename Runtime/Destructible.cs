using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public delegate bool BeforeDestroyHandler(Destructible destructible);
public delegate bool BeforeTakeDamageHandler(Destructible destructible, float damage, ref float newRemainingHealth);

[RequireComponent(typeof(Renderer))]
[SuppressMessage("Major Code Smell", "S3264:Events should be invoked", Justification = "Events called by extensions")]
public class Destructible : MonoBehaviour, IPoolable, IManagedStateContributor
{
    public float HealthCurrent = 10f;
    public float HealthMax = 10f;

    public string ExplosionPrefabName;

    private string _myPoolName;
    private bool _isDestroyed;
    private FlashEffect _flashEffect;
    private FadeOutEffect _fadeOutEffect;
    private FragmenterEffect _fragmenterEffect;
    private KnockbackMovement _knockbackMovement;
    private ManagedState _managedState;

    public bool IsDestroyed => _isDestroyed;
    public bool CanTakeDamage { get; set; } = true;

    public event BeforeDestroyHandler OnBeforeDestroy;
    public event BeforeTakeDamageHandler OnBeforeTakeDamage;

    public void OnPoolableCreate(string poolName)
    {
        _myPoolName = poolName;
        this.TryInitComponent(ref _flashEffect, isOptional: true);
        this.TryInitComponent(ref _fadeOutEffect, isOptional: true);
        this.TryInitComponent(ref _fragmenterEffect, isOptional: true);
        this.TryInitComponent(ref _knockbackMovement, isOptional: true);
        this.TryInitComponent(ref _managedState, isOptional: true);
    }

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

    void StartFlash()
    {
        if (_flashEffect != null)
        {
            _flashEffect.StartFlash();
        }
    }

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

    private void DoSelfDestruct()
    {
        PoolManager.Instance.Recycle(_myPoolName, gameObject);
    }

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
