using System;
using UnityEngine;

public class WormAttackLadder : MonoBehaviour
{
    /*
    This bookworm variant finds ladders and fires projectiles down the ladders to attack players because it is evil and has only malice in its cold heart
    */

    //--------- Alyssa--------------
    public delegate void OnFireAttackAnimFunc(SpriteRenderer spriteRender);
    public static event OnFireAttackAnimFunc OnFireAttackAnim;
    //------------------------------
    
    [SerializeField] private float speed = 5;
    private float obstacleRaycastDistance = 2f;

    [SerializeField] private bool movingRight = true; // This is a serialize field so the starting direction can be changed from the editor.
    
    [SerializeField] private Transform groundDetection;
    [SerializeField] private Transform bookProjectileSpawn;
    [SerializeField] private Transform circleCastOrigin;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask ladderLayerMask;
    [SerializeField] private LayerMask playerLayerMask;

    [SerializeField] private GameObject bookProjectilePrefab;
    [SerializeField] private GameObject slimePrefab;

    [SerializeField] private float attackDelay = 5f;
    private float attackTimer = 5f;
    [SerializeField] private float retreatTimerMax = 3f;
    [SerializeField] private float retreatSpeedMultiplier = 2f;
    private float retreatTimer;

    public enum StateMachine
    {
        Search,
        Attack,
        Retreat,
        Caught
    }

    public StateMachine currentState;

    private void Start()
    {
        currentState = StateMachine.Search;

        retreatTimer = retreatTimerMax;

        // Start worm facing left if movingRight is disabled in the editor when the game starts
        if (!movingRight)
        {
            transform.eulerAngles = new Vector3(0, -180, 0);
        }
    }

    private void Update()
    {
        switch (currentState)
        {
            case StateMachine.Search:
                Search();
                break;
            case StateMachine.Attack:
                Attack();
                break;
            case StateMachine.Retreat:
                Retreat();
                break;
            case StateMachine.Caught:
                // Do nothing
                break;
        }
    }

    private void Search()
    {
        /*
        Search for a ladder to attack from
        */

        retreatTimer = retreatTimerMax;

        // Moves worm
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        // Checks for when ground ends
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, obstacleRaycastDistance, groundLayerMask);
        RaycastHit2D pathAheadInfo = Physics2D.Raycast(groundDetection.position, Vector2.right, obstacleRaycastDistance, groundLayerMask);
        RaycastHit2D ladderInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, obstacleRaycastDistance, ladderLayerMask);

        if (ladderInfo.collider != false)
        {
            attackTimer = attackDelay; // Reset attack timer
            currentState = StateMachine.Attack;
        }

        // Changes direction when ground ends
        if (groundInfo.collider == false && ladderInfo.collider == false)
        {
            ChangeDirectionLeftRight();
        } else if (pathAheadInfo.collider != false)
        {
            ChangeDirectionLeftRight();
        }

        DetectPlayer();
    }

    private void Attack()
    {
        /*
        Fire projectiles down ladder until player is detected, then retreat
        */

        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        } else
        {
            //------- Alyssa ------
            SpriteRenderer spriteRender = gameObject.GetComponentInChildren<SpriteRenderer>();
            OnFireAttackAnim?.Invoke(spriteRender);
            //---------------------
            
            // Instantiate book projectile at spawn point
            Vector3 projectileSpawnPoint = bookProjectileSpawn.position;
            Instantiate(bookProjectilePrefab, projectileSpawnPoint, Quaternion.identity);
            attackTimer = attackDelay; // Reset attack timer
        }

        DetectPlayer();

    }

    private void Retreat()
    {
        /*
        Run away from at increased speed for a limited time
        */

        if (retreatTimer > 0f)
        {
            retreatTimer -= Time.deltaTime;

            // Moves worm
            transform.Translate(Vector2.right * (retreatSpeedMultiplier * speed) * Time.deltaTime);

            // Checks for when ground ends
            RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, obstacleRaycastDistance, groundLayerMask);
            RaycastHit2D pathAheadInfo = Physics2D.Raycast(groundDetection.position, Vector2.right, obstacleRaycastDistance, groundLayerMask);

            // Changes direction when ground ends
            if (groundInfo.collider == false)
            {
                ChangeDirectionLeftRight();
            } else if (pathAheadInfo.collider != false)
            {
                ChangeDirectionLeftRight();
            }
        } else
        {
            currentState = StateMachine.Search;
        }   
    }

    private void DetectPlayer()
    {
        Vector3 circleCastOriginCoords = circleCastOrigin.position;
        float playerDetectionRadius = 1f;
        RaycastHit2D detectPlayer = Physics2D.CircleCast(circleCastOriginCoords, playerDetectionRadius, Vector2.right, obstacleRaycastDistance / 2f, playerLayerMask);

        if (detectPlayer.collider != null)
        {
            Instantiate(slimePrefab, circleCastOriginCoords, Quaternion.identity);
            ChangeDirectionLeftRight();
            currentState = StateMachine.Retreat;
        }
    }

    public void BookwormCaught()
    {
        currentState = StateMachine.Caught;
    }

    private void ChangeDirectionLeftRight()
    {
        // Debug.Log("Floor not detected");
        if (movingRight)
        {
            transform.eulerAngles = new Vector3(0, -180, 0);
            movingRight = false;
        } else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            movingRight = true;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {   
        if (collision.collider.CompareTag("Ladder"))
        {
            // Ignore collisions with ladders
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider, true);
        }

        if (collision.collider.CompareTag("Slime"))
        {
            // Ignore collisions with slime
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider, true);
        }
        
        if (collision.collider.CompareTag("Bookworm"))
        {
            // Ignore collisions with other worms
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider, true);
        }
        
    }

    // Visualize the CircleCast in Scene view
    void OnDrawGizmos()
    {
        Vector2 origin = transform.position;
        Vector2 direction = transform.right;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origin, 1f);
        
        Gizmos.color = Color.yellow;
        Vector2 endPosition = origin + direction * (obstacleRaycastDistance / 2f);
        Gizmos.DrawWireSphere(endPosition, 1f);
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(origin, endPosition);
    }


     //------Alyssa-------
    public StateMachine GetState()
    {
        return currentState;
    }

    public float GetSpeedModifier()
    {
        return retreatSpeedMultiplier;
    }
    //--------------------
}
