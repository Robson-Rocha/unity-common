using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

/// <summary>
/// Delegate executed when a valid collision is entering before ignore logic is applied.
/// </summary>
/// <param name="ignoreCollisionsFromDirection">The component handling ignore rules.</param>
/// <param name="direction">The detected incoming direction.</param>
/// <param name="collidingGameObject">The colliding game object.</param>
/// <returns><see langword="true"/> to continue processing; otherwise <see langword="false"/>.</returns>
public delegate bool CollisionEntering(
    IgnoreCollisionsFromDirection ignoreCollisionsFromDirection, Direction direction, GameObject collidingGameObject);

/// <summary>
/// Delegate executed before adding temporary incoming directions to ignore.
/// </summary>
/// <param name="ignoreCollisionsFromDirection">The component handling ignore rules.</param>
/// <param name="direction">The direction requested for temporary ignoring.</param>
/// <param name="requesterGameObject">The requester game object.</param>
/// <returns><see langword="true"/> to allow adding; otherwise <see langword="false"/>.</returns>
public delegate bool AddingTempIncomingDirectionToIgnore(
    IgnoreCollisionsFromDirection ignoreCollisionsFromDirection, Direction direction, GameObject requesterGameObject);

[SuppressMessage("Major Code Smell", "S3264:Events should be invoked", Justification = "Events called by extensions")]
/// <summary>
/// Temporarily disables 2D collisions based on incoming direction and layer rules.
/// </summary>
public class IgnoreCollisionsFromDirection : MonoBehaviour, IPoolable
{
    /// <summary>
    /// Layers eligible for collision ignoring.
    /// </summary>
    public LayerMask LayersToIgnore;
    /// <summary>
    /// Incoming directions that should trigger collision ignore.
    /// </summary>
    public Direction IncomingDirectionsToIgnore = Direction.Up;
    /// <summary>
    /// Name used to locate the collision checker component.
    /// </summary>
    public string IgnoreCollisionsCheckerName = "IgnoreCollisions";

    private Collider2D _collider;
    private BaseCollisionChecker _collisionChecker;
    private bool _collisionsDisabled = false;
    private Direction _tempDirections = Direction.None;

    /// <summary>
    /// Invoked when collision entering is detected and before ignore logic is applied.
    /// </summary>
    public event CollisionEntering OnCollisionEntering;
    /// <summary>
    /// Invoked before adding temporary incoming directions to ignore.
    /// </summary>
    public event AddingTempIncomingDirectionToIgnore OnAddingTempIncomingDirectionToIgnore;

    /// <summary>
    /// Initializes required pooled references.
    /// </summary>
    /// <param name="poolName">The source pool name.</param>
    public void OnPoolableInit(string poolName)
    {
        this.TryInitComponent(ref _collider);
        this.TryInitComponent(ref _collisionChecker, c => c.Name == IgnoreCollisionsCheckerName);
    }

    /// <summary>
    /// Adds temporary directions that should be ignored until cleared.
    /// </summary>
    /// <param name="direction">The direction flags to add.</param>
    /// <param name="requester">The requesting game object.</param>
    /// <returns><see langword="true"/> if directions were added; otherwise <see langword="false"/>.</returns>
    public bool AddTempIncomingDirectionToIgnore(Direction direction, GameObject requester)
    {
        if (OnAddingTempIncomingDirectionToIgnore?.Invoke(this, direction, requester) ?? true)
        {
            IncomingDirectionsToIgnore |= direction;
            _tempDirections |= direction;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Removes all temporary incoming directions currently applied.
    /// </summary>
    public void ClearTempIncomingDirectionsToIgnore()
    {
        IncomingDirectionsToIgnore &= ~_tempDirections;
        _tempDirections = Direction.None;
    }

    /// <summary>
    /// Detects collisions and temporarily disables collisions when configured conditions are met.
    /// </summary>
    void Update()
    {
        if (_collisionsDisabled || _collisionChecker == null || !_collisionChecker.IsColliding)
            return;

        Collider2D hittingCollider = _collisionChecker.Hit;
        GameObject hitObject = hittingCollider.gameObject;
        if (!IsInLayerMask(hitObject.layer, LayersToIgnore))
            return;

        if (hitObject.TryGetComponent<Rigidbody2D>(out var collidingRigidbody))
        {
            // Use the velocity to determine the direction the object is coming from
            Direction incomingDirection = collidingRigidbody.linearVelocity.normalized.ToIncomingDirection();

            bool entering = OnCollisionEntering.InvokeMany<CollisionEntering, bool>(
                true, this, incomingDirection, hitObject).All();
            bool shouldIgnore = (incomingDirection & IncomingDirectionsToIgnore) != 0;
            if (!shouldIgnore ||
                !entering)
                return;
        }
        Physics2D.IgnoreCollision(_collider, hittingCollider, true);
        _collisionsDisabled = true;
        StartCoroutine(ReenableWhenSeparated(hittingCollider));
    }

    /// <summary>
    /// Checks whether a layer is included in a <see cref="LayerMask"/>.
    /// </summary>
    /// <param name="layer">The layer index.</param>
    /// <param name="mask">The layer mask to check against.</param>
    /// <returns><see langword="true"/> if the layer is in the mask; otherwise <see langword="false"/>.</returns>
    private static bool IsInLayerMask(int layer, LayerMask mask)
        => (mask.value & (1 << layer)) != 0;

    /// <summary>
    /// Re-enables collisions after the colliders are separated.
    /// </summary>
    /// <param name="other">The collider whose collision with this object was ignored.</param>
    /// <returns>An enumerator used by Unity coroutines.</returns>
    private IEnumerator ReenableWhenSeparated(Collider2D other)
    {
        yield return new WaitForEndOfFrame();
        while (true)
        {
            ColliderDistance2D collisionDistance = Physics2D.Distance(_collider, other);

            if (!_collisionChecker.IsColliding && !collisionDistance.isOverlapped)
                break;
            yield return null;
        }
        if (_collider != null && other != null)
            Physics2D.IgnoreCollision(_collider, other, false);
        _collisionsDisabled = false;
    }
}
