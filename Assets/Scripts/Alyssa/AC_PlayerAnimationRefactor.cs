using UnityEngine;
using UnityEngine.InputSystem;

public class AC_PlayerAnimationRefactor : MonoBehaviour
{
    [SerializeField] private PlayerRefactor playerRefactor;
    [SerializeField] private Animator animator;
    private string currentAnim;
    private string currentBaseAnim;

    private string FACING_FRONT = "FacingFront";
    private string FACING_BACK = "FacingBack";
    private string FACING_LEFT = "FacingLeft";
    private string FACING_RIGHT = "FacingRight";
    private string WALKING_LEFT = "WalkingLeft";
    private string WALKING_RIGHT = "WalkingRight";
    private string DASH_LEFT = "DashLeft";
     private string DASH_RIGHT = "DashRight";
    private string JUMP_FRONT = "JumpFront";
    private string JUMP_BACK = "JumpBack";
    private string JUMP_LEFT = "JumpLeft";
    private string JUMP_RIGHT = "JumpRight";
    private string FALL_FRONT = "FallFront";
    private string FALL_BACK = "FallBack";
    private string FALL_LEFT = "FallLeft";
    private string FALL_RIGHT = "FallRight";
    private string CLIMB_BACK = "Climbing";


    private void Start()
    {
        //base animations are necessary so that when an action stops (i.e. walking), it goes to it's matching idle rather than a mismatch one

        animator = GetComponent<Animator>();
        currentAnim = FACING_FRONT;
        currentBaseAnim = FACING_FRONT;
    }
    

    private void Update()
    {
        Debug.Log(playerRefactor.GetState());
        
        if (playerRefactor.GetState() == PlayerRefactor.State.Dashing)
        {
            if (0 != playerRefactor.GetMovementX())
            {
                if (0 > playerRefactor.GetMovementX())
                    SwitchAnimation(DASH_LEFT, FACING_LEFT);
                
                else if (0 < playerRefactor.GetMovementX())
                    SwitchAnimation(DASH_RIGHT, FACING_RIGHT);
            }
            
        }
        
        
        if(playerRefactor.GetState() == PlayerRefactor.State.Moving)
        {    
            if (0 > playerRefactor.GetMovementX())
                SwitchAnimation(WALKING_LEFT, FACING_LEFT);
            
            else if (0 < playerRefactor.GetMovementX())
                SwitchAnimation(WALKING_RIGHT, FACING_RIGHT);
        }


        if(playerRefactor.GetState() == PlayerRefactor.State.SingleJump || playerRefactor.GetState() == PlayerRefactor.State.DoubleJump)
        {
            if(GetBaseAnim() == FACING_LEFT || 0 > playerRefactor.GetMovementX())
            {
                SwitchAnimation(JUMP_LEFT, FACING_LEFT);
                AnimateGrounded(FACING_LEFT, FACING_LEFT);
            }
                
            if(GetBaseAnim() == FACING_RIGHT || 0 < playerRefactor.GetMovementX())
            {
                SwitchAnimation(JUMP_RIGHT, FACING_RIGHT);
                AnimateGrounded(FACING_RIGHT, FACING_RIGHT);
            }
                
            if(GetBaseAnim() == FACING_FRONT)
            {
                SwitchAnimation(JUMP_FRONT, FACING_FRONT);
                AnimateGrounded(FACING_FRONT, FACING_FRONT);
            }

            if (GetBaseAnim() == FACING_BACK)
            {
                SwitchAnimation(JUMP_BACK, FACING_BACK);
                AnimateGrounded(FACING_BACK, FACING_BACK);
            }
        }


        if (playerRefactor.GetState() == PlayerRefactor.State.Climbing)
        {
            if (0 != playerRefactor.GetMovementY())
                SwitchAnimation(CLIMB_BACK, FACING_BACK);
        }


        if(playerRefactor.GetState() == PlayerRefactor.State.Falling)
        {
            if(GetBaseAnim() == FACING_LEFT || 0 > playerRefactor.GetMovementX())
            {
                SwitchAnimation(FALL_LEFT, FACING_LEFT);
                AnimateGrounded(FACING_LEFT, FACING_LEFT);
            }
                
            if(GetBaseAnim() == FACING_RIGHT || 0 < playerRefactor.GetMovementX())
            {
                SwitchAnimation(FALL_RIGHT, FACING_RIGHT);
                AnimateGrounded(FACING_RIGHT, FACING_RIGHT);
            }
                
            if(GetBaseAnim() == FACING_FRONT)
            {
                SwitchAnimation(FALL_FRONT, FACING_FRONT);
                AnimateGrounded(FACING_FRONT, FACING_FRONT);
            }

            if (GetBaseAnim() == FACING_BACK)
            {
                SwitchAnimation(FALL_BACK, FACING_BACK);
                AnimateGrounded(FACING_BACK, FACING_BACK);
            }
        }


        if (playerRefactor.GetState() == PlayerRefactor.State.Idle && playerRefactor.GetIsGrounded())
        {
            SwitchAnimation(currentBaseAnim, currentBaseAnim);

            if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame )
                SwitchAnimation(FACING_BACK, FACING_BACK);
                    
            if (Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame )
                SwitchAnimation(FACING_FRONT, FACING_FRONT);
                
            if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame )
                SwitchAnimation(FACING_LEFT, FACING_LEFT);
                
            if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame )
                SwitchAnimation(FACING_RIGHT, FACING_RIGHT); 
        }


       
        
    }


    private void SwitchAnimation(string newAnim, string newBaseAnim)
    {
        //when animation changes, set current animations to false and set it to the new ones
        //also changes it's base animation to fall back on when an active (walk, dash) animation is done 
        if (newAnim != currentAnim)
        {
            animator.SetBool(currentAnim, false);
            animator.SetBool(currentBaseAnim, false);

            animator.SetBool(newBaseAnim, true);
            animator.SetBool(newAnim, true);

            currentAnim = newAnim;
            currentBaseAnim = newBaseAnim;
        }
    }


    private string GetBaseAnim()
    {
        return currentBaseAnim;
    }


    private void AnimateGrounded(string newAnim, string newBaseAnim)
    {
        if(playerRefactor.GetIsGrounded())
            SwitchAnimation(newAnim, newBaseAnim);
    }

}

