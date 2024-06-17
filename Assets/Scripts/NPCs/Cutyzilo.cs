using UnityEngine;

public class Cutyzilo : MonoBehaviour
{
    public const string IS_ATTACKING = "CtzloIsAtking";
    private bool _isAttacking;
    private Animator _animator;

    private void Start() 
        => _animator = transform.Find("Display").GetComponent<Animator>();
    
    private void Update()
    {
        var data = DataBridge.TryGetData<bool>(IS_ATTACKING);
        if (data.WasUpdated)
        {
            _isAttacking = data.EncodedData;
            DataBridge.MarkUpdateProcessed<bool>(IS_ATTACKING);
        }

        _animator.SetBool("IsAttacking", _isAttacking);
    }
}