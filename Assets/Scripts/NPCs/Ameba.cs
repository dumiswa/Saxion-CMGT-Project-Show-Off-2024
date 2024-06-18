using Monoliths;
using Monoliths.Mechanisms;
using Monoliths.Player;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Ameba : Actuator
{
    [SerializeField]
    private Vector3 A;
    [SerializeField]
    private Vector3 B;

    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _seatingOffsetX;
    [SerializeField]
    private float _seatingOffsetY;


    private bool _isMoving;
    private bool _isB;

    private Transform _parentBuffer;
    private Transform _passenger;

    private bool _nextFrameUnlock;

    private void Update()
    {
        if (_nextFrameUnlock)
        {
            MonolithMaster.Instance.Monoliths[typeof(PlayerMovement)]?.SetActive(true);
            _nextFrameUnlock = false;
        }
        if (!_isMoving)
            return;

        if (_isB)
        {
            var direction = A - B;
            transform.position += _speed * Time.deltaTime * direction.normalized * Mathf.Clamp01(Vector3.Distance(transform.position, A));
            if (Vector3.Distance(transform.position, A) < 0.5f)
            {
                _isMoving = false;
                _isB = !_isB;

                _passenger.position += _seatingOffsetX * (_isB ? B - A : A - B).normalized;
                _passenger.position -= Vector3.up * _seatingOffsetY;
                _passenger.SetParent(_parentBuffer);

                _passenger.TryGetComponent(out Collider collider);
                if (collider)
                    collider.enabled = true;

                _passenger.TryGetComponent(out Rigidbody rb);
                if (rb)
                {
                    rb.WakeUp();
                    rb.position = transform.position;
                }

                _nextFrameUnlock = true;
                Locked = false;
                return;
            }
        }
        else
        {
            var direction = B - A;
            transform.position += _speed * Time.deltaTime * direction.normalized * Mathf.Clamp01(Vector3.Distance(transform.position, B));
            if (Vector3.Distance(transform.position, B) < 0.5f)
            {
                _isMoving = false;
                _isB = !_isB;

                _passenger.position += _seatingOffsetX * (_isB ? B - A : A - B).normalized;
                _passenger.position -= Vector3.up * _seatingOffsetY;
                _passenger.SetParent(_parentBuffer);

                _passenger.TryGetComponent(out Collider collider);
                if (collider)
                    collider.enabled = true;

                _passenger.TryGetComponent(out Rigidbody rb);
                if (rb)
                {
                    rb.WakeUp();
                    rb.position = transform.position;
                }
                _nextFrameUnlock = true;
                Locked = false;
                return;
            }
        }
        _passenger.position = new Vector3(transform.position.x, transform.position.y + _seatingOffsetY, transform.position.z);
    }

    public override void Invoke()
    {
        Locked = true;

        _passenger.TryGetComponent(out Collider collider);
        if(collider)
            collider.enabled = false;

        MonolithMaster.Instance.Monoliths[typeof(PlayerMovement)]?.SetActive(false);

        _passenger.TryGetComponent(out Rigidbody rb);
        if (rb)
        {
            rb.Sleep();
        }

        _parentBuffer = _passenger.parent;
        _passenger.SetParent(transform);
        _passenger.position -= _seatingOffsetX * (_isB ? B - A : A - B).normalized;
        _passenger.position += Vector3.up * _seatingOffsetY;

        _isMoving = true;
    }

    public override void Interact(GameObject caller)
    {
        if (Locked)
            return;
        _passenger = caller.transform;
        base.Interact(caller);
        Invoke();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(A, 0.25f);
        Gizmos.DrawSphere(B, 0.25f);
        Gizmos.DrawLine(A, B);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position + new Vector3(0,0.15f,0), transform.position + _seatingOffsetX * (_isB ? B - A : A - B).normalized + new Vector3(0, 0.15f, 0));
    }
}