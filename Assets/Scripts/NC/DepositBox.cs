using System;
using UnityEngine;

public class DepositBox : MonoBehaviour, IBookwormParent
{
    public event EventHandler OnBookwormDeposited;
    
    [SerializeField] private Transform bookwormHoldPoint;

    //---------MARI-------------------------
    //Points per bookworm
    [SerializeField] private int pointsPerBookworm = 10;
    //--------------------------------------

    private Bookworm _bookworm;

    //---------MARI-------------------------
    //Warning for missing ScoreSystem
    private bool _hasShownMissingScoreSystemWarning;
    //--------------------------------------


    public Transform GetBookwormTransform()
    {
        return bookwormHoldPoint;
    }

    public void SetBookworm(Bookworm bookworm)
    {
        _bookworm = bookworm;

        if (_bookworm != null)
        {

            //---------MARI-------------------------
            //Add points to score system
            ScoreSystem scoreSystem = ScoreSystem.Instance != null ? ScoreSystem.Instance : FindObjectOfType<ScoreSystem>();

            if (scoreSystem == null && !_hasShownMissingScoreSystemWarning)
            {
                Debug.LogWarning("ScoreSystem not found in scene. Bookworms will not award points.");
                _hasShownMissingScoreSystemWarning = true;
            }

            scoreSystem?.AddPoints(pointsPerBookworm);
            //--------------------------------------
    
            OnBookwormDeposited?.Invoke(this, EventArgs.Empty);

            //---------MARI-------------------------
            //Destroy bookworm once deposited
            _bookworm.DestroySelf();
            //--------------------------------------
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
