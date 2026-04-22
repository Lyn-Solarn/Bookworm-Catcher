using UnityEngine;

public class Bookworm : MonoBehaviour
{
    private IBookwormParent _parent;

    private bool _parentIsPlayer;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Bookworm_CollisionEnter2D");
        if (collision.gameObject.GetComponent<Player>() && !_parentIsPlayer)
        {
            SetObjectParent(collision.gameObject.GetComponent<Player>());
            _parentIsPlayer = true;
        }
        
        if (collision.gameObject.GetComponent<DepositBox>() && _parentIsPlayer)
        {
            SetObjectParent(collision.gameObject.GetComponent<DepositBox>());
            
        }
    }
    
    public void SetObjectParent(IBookwormParent parent)
    {
        //clear previous parent
        if (_parent != null)
        {
            _parent.ClearBookworm();
        }
        
        //set new parent
        _parent = parent;
        
        _parent.SetBookworm(this);
        
        transform.parent = _parent.GetBookwormTransform();
        transform.localPosition = Vector3.zero;
    }

    public IBookwormParent GetBookwormParent()
    {
        return _parent;
    }

    public void DestroySelf()
    {
        _parent.ClearBookworm();
        Destroy(gameObject);
    }
    
}
