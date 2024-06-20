using UnityEngine;

public class MeteorBreakable : MonoBehaviour
{
    [SerializeField]
    private float _clearTime = 1.8f;

    private float _counter = 0f;

    private Animator _animator;

    private bool _clearing;

    [Space(10)]
    [SerializeField]
    private Vector2 _xBoundaries;
    [SerializeField]
    private Vector2 _zBoundaries;

    private void Start()
    {
        transform.position = new Vector3
        (
            Random.Range(_xBoundaries.x, _xBoundaries.y),
            transform.position.y, 
            Random.Range(_zBoundaries.x, _zBoundaries.y)
        );
        _animator = GetComponent<Animator>();

        _clearing = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.parent == null ||
            collision.transform.parent.parent == null)
            return;

        if (collision.transform.parent.parent.TryGetComponent(out Meteor meteor))
            Destroy(meteor.gameObject);

        AudioManager.Instance.PlaySound("BossAttack");
    }

    private void Update()
    {
        _counter += Time.deltaTime;

        if (_clearTime > _counter)
            return;

        if (!_clearing)
        {
            _animator?.SetTrigger("Clear");
            _clearing = true;
        }
        if (_clearTime + 2f > _counter)
            return;

        Destroy(transform.parent.gameObject);
    }
}