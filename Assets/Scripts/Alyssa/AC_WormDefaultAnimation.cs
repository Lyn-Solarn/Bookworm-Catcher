using UnityEngine;

public class AC_WormDefaultAnimation : MonoBehaviour
{
    [SerializeField] private WormPatrol wormDefault;
    [SerializeField] private Animator animator;
    private string IS_CAUGHT = "isCaught";


    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool(IS_CAUGHT, false);
    }


    private void Update()
    {
        // Debug.Log("Worm State: " + wormDefault.GetState());
        
        //changes to caught animation when worm is caught 
        if (wormDefault.GetState() == WormPatrol.StateMachine.Caught)
        {
            animator.SetBool(IS_CAUGHT, true);
        }
    }

}
