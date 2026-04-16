using UnityEngine;
using UnityEngine.InputSystem;

public class AC_PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private string currentAnim;


    void Start()
    {
        animator = GetComponent<Animator>();
        currentAnim = "FacingFront";
    }
    

    void Update()
    {
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            SwitchAnimation("FacingBack");
        }
            
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            SwitchAnimation("FacingFront");
        }
            
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            SwitchAnimation("FacingLeft");
        }
            
        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            SwitchAnimation("FacingRight");
        }
            
    }

    private void SwitchAnimation(string newAnim)
    {
        animator.SetBool(currentAnim, false);
        animator.SetBool(newAnim, true);
        currentAnim = newAnim;
        // Debug.Log(currentAnim);
    }

}
