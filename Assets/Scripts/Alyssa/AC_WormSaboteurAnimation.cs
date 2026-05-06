using NUnit.Framework;
using UnityEngine;

public class AC_WormSaboteurAnimation : MonoBehaviour
{
    [SerializeField] private WormAttackLadder wormSaboteur;
    [SerializeField] private Animator animator;
    private string IS_CAUGHT = "isCaught";
    private string IS_ATTACK = "isAttacking";
    private string ATTACK_ACTIVE = "AttackActive";


    private void Awake()
    {
        WormAttackLadder.OnFireAttackAnim += OnFireAttackAnim;
    }
    
    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool(IS_CAUGHT, false);
        animator.SetBool(IS_ATTACK, false);
    }


    private void Update()
    {
        // Debug.Log("Worm State: " + wormDefault.GetState());
        
        //defualt wiggle state of the worm 
        if (wormSaboteur.GetState() == WormAttackLadder.StateMachine.Search)
            animator.SetBool(IS_ATTACK, false);
        
        if (wormSaboteur.GetState() == WormAttackLadder.StateMachine.Attack)
            animator.SetBool(IS_ATTACK, true);
        
        //changes to caught animation when worm is caught
        if (wormSaboteur.GetState() == WormAttackLadder.StateMachine.Caught)
        {
            animator.SetBool(IS_CAUGHT, true);
            //de-signs up for event notif before worm is destroyed 
            WormAttackLadder.OnFireAttackAnim -= OnFireAttackAnim;
        }
            

        //change animation speed when worm gets faster when retreating, resets speed afterwards 
        if (wormSaboteur.GetState() == WormAttackLadder.StateMachine.Retreat)
        {
            animator.SetBool(IS_ATTACK, false);
            animator.speed = wormSaboteur.GetSpeedModifier();
        }
        else
            animator.speed = 1f;
        
    }


    public void OnFireAttackAnim(SpriteRenderer spriteRender)
    {
        //only fires animation for that specific worm 
        if (spriteRender != this.gameObject.GetComponent<SpriteRenderer>())
            return;
        
        //plays attack animation once, returns to attack waiting after 
        animator.SetTrigger(ATTACK_ACTIVE);
    }

}
