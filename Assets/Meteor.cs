using UnityEngine;

public class Meteor : MonoBehaviour
{
    [SerializeField]
    private float _calibratingTime = 0.8f;
    [SerializeField]
    private float _clearTime = 16f;

    private float _counter = 0f;

    private Transform _player;
    private Animator _animator;

    private bool _clearing;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _animator = GetComponent<Animator>();

        _clearing = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.parent == null || collision.transform.parent.parent == null)
            return;

        if(collision.transform.parent.parent.TryGetComponent(out Meteor meteor))
            Destroy(meteor.gameObject);
    }

    private void Update()
    {
        _counter += Time.deltaTime;
        if (_calibratingTime > _counter)
            transform.position = new Vector3(_player.position.x ,transform.position.y, _player.position.z);

        if (_clearTime > _counter)
            return;

        if (!_clearing)
        {
            _animator?.SetTrigger("Clear");
            _clearing = true;
        }
        if (_clearTime + 2f > _counter)
            return;

        Destroy(gameObject);
    }
}