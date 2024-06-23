using Monoliths.Mechanisms;
using System.Collections;
using UnityEngine;

public class Meteor : Actuator
{
    private Vector3 _sender = new(0, 0, 10f);

    [SerializeField]
    private float _calibratingTime = 0.8f;
    [SerializeField]
    private float _clearTime = 16f;

    private float _counter = 0f;

    private Transform _player;
    private Animator _animator;

    private bool _clearing;
    private bool _isInAction;
    private bool _acted;
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _animator = GetComponent<Animator>();

        _clearing = false;
        _acted = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.parent == null || 
            collision.transform.parent.parent == null)
            return;

        if(collision.transform.parent.parent.TryGetComponent(out Meteor meteor))
            Destroy(meteor.gameObject);
    }

    private void Update()
    {

        if (_isInAction)
            return;

        var data = DataBridge.TryGetData<bool>(Cutyzilo.IS_ATTACKING);
        if (!data.IsEmpty) Locked = data.EncodedData && !_acted;

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

    public override void Interact(GameObject caller)
    {
        if (Locked)
            return;

        base.Interact(caller);
        Invoke();
    }

    public override void Invoke() 
        => StartCoroutine(ReturnToSender());

    private IEnumerator ReturnToSender()
    {
        Locked = false;
        _acted = true;
        DataBridge.UpdateData<bool>(Cutyzilo.IS_ATTACKING, true);
        _isInAction = true;
        _animator?.SetTrigger("Act");
        float acceleration = 0.1f;
        float counter = 0f;
        yield return new WaitForSeconds(1);
        while (true)
        {
            if (counter > 2f || Vector3.Distance(transform.position, _sender) < 1.2f)
                break;

            var direction = _sender - transform.position;
            var movement = acceleration * Time.deltaTime * direction.normalized;
            transform.position += movement;

            acceleration *= 2f;
            acceleration = Mathf.Clamp(acceleration, 0f, 32f);
            yield return new WaitForEndOfFrame();
            counter += Time.deltaTime;
        }
        Boss.Instance?.DealDamage();
        _animator?.SetTrigger("Clear2");
        DataBridge.UpdateData<bool>(Cutyzilo.IS_ATTACKING, false);
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
