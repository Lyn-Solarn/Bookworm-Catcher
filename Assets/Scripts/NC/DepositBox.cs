using System;
using UnityEngine;

public class DepositBox : MonoBehaviour, IBookwormParent
{
    public event EventHandler OnBookwormDeposited;
    
    [SerializeField] private Transform bookwormHoldPoint;
    private Bookworm _bookworm;
    public Transform GetBookwormTransform()
    {
        return bookwormHoldPoint;
    }

    public void SetBookworm(Bookworm bookworm)
    {
        _bookworm = bookworm;

        if (_bookworm != null)
        {
            OnBookwormDeposited?.Invoke(this, EventArgs.Empty);
        }
    }

    public Bookworm GetBookworm()
    {
        return _bookworm;
    }

    public void ClearBookworm()
    {
        _bookworm = null;
    }

    public bool HasBookworm()
    {
        return _bookworm != null;
    }
}
