using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public delegate bool CollisionEntering(
    IgnoreCollisionsFromDirection ignoreCollisionsFromDirection, Direction direction, GameObject collidingGameObject);

public delegate bool AddingTempIncomingDirectionToIgnore(
    IgnoreCollisionsFromDirection ignoreCollisionsFromDirection, Direction direction, GameObject requesterGameObject);

[SuppressMessage("Major Code Smell", "S3264:Events should be invoked", Justification = "Events called by extensions")]
public class IgnoreCollisionsFromDirection : MonoBehaviour, IPoolable
{
    public LayerMask LayersToIgnore;
    public Direction IncomingDirectionsToIgnore = Direction.Up;
    public string IgnoreCollisionsCheckerName = "IgnoreCollisions";

    private Collider2D _collider;
    private BaseCollisionChecker _collisionChecker;
    private bool _collisionsDisabled = false;
    private Direction _tempDirections = Direction.None;

    public event CollisionEntering OnCollisionEntering;
    public event AddingTempIncomingDirectionToIgnore OnAddingTempIncomingDirectionToIgnore;

    public void OnPoolableInit(string poolName)
    {
        this.TryInitComponent(ref _collider);
        this.TryInitComponent(ref _collisionChecker, c => c.Name == IgnoreCollisionsCheckerName);
    }

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

    public void ClearTempIncomingDirectionsToIgnore()
    {
        IncomingDirectionsToIgnore &= ~_tempDirections;
        _tempDirections = Direction.None;
    }

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

    private static bool IsInLayerMask(int layer, LayerMask mask)
        => (mask.value & (1 << layer)) != 0;

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
