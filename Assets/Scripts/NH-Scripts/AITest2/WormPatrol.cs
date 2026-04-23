using System;
using UnityEngine;

public class WormPatrol : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    private float obstacleRaycastDistance = 2f;

    [SerializeField] private bool movingRight = true; // This is a serialize field so the starting direction can be changed from the editor.

    [SerializeField] private Transform groundDetection;
    // [SerializeField] private Transform ladderDetection;
    // [SerializeField] private Transform ladderUpDetection;
    // [SerializeField] private Transform exitLadderDetection;

    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask ladderLayerMask;

    [SerializeField] private GameObject playerGameObject;
    private Rigidbody2D rb;
    private bool touchingPlayer = false;
    private bool climbing = false;
    // private bool detectedDownLadder = false; // True whenever raycast connects with down ladder, prevents multiple positives for one ladder
    // private bool detectedUpLadder = false; // True whenever raycast connects with up ladder, prevents multiple positives for one ladder


    public enum StateMachine
    {
        Patrol,
        ClimbUp,
        ClimbDown,
        Caught
    }

    public StateMachine currentState;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    private void Start()
    {
        currentState = StateMachine.Patrol;

        // Start worm facing left if movingRight is disabled in the editor when the game starts
        if (!movingRight)
        {
            transform.eulerAngles = new Vector3(0, -180, 0);
        }

        // Subscribe to OnCaughtBookworm event from Player
        Player player = playerGameObject.GetComponent<Player>();
        player.OnCaughtBookworm += BookwormCaught;
    }

    private void Update()
    {
        switch (currentState)
        {
            case StateMachine.Patrol:
                Patrol();
                break;
            case StateMachine.ClimbUp:
                ClimbUp();
                break;
            case StateMachine.ClimbDown:
                ClimbDown();
                break;
            case StateMachine.Caught:
                // Do nothing
                break;
        }
    }

    private void Patrol()
    {
        // Moves worm
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        // Checks for when ground ends
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, obstacleRaycastDistance, groundLayerMask);
        RaycastHit2D pathAheadInfo = Physics2D.Raycast(groundDetection.position, Vector2.right, obstacleRaycastDistance, groundLayerMask);
        // RaycastHit2D ladderDownInfo = Physics2D.Raycast(ladderDetection.position, Vector2.down, obstacleRaycastDistance, ladderLayerMask);
        // RaycastHit2D ladderUpInfo = Physics2D.Raycast(groundDetection.position, Vector2.right, obstacleRaycastDistance / 2f, ladderLayerMask);

        // Changes direction when ground ends
        if (groundInfo.collider == false)
        {
            ChangeDirectionLeftRight();
        } else if (pathAheadInfo.collider != false)
        {
            ChangeDirectionLeftRight();
        }

        // if (ladderUpInfo != false)
        // {
        //     if (!detectedUpLadder) {
        //         // int ladderChance = UnityEngine.Random.Range(0, 5);
        //         int ladderChance = 1;
        //         if (ladderChance == 1)
        //         {
        //             Debug.Log("Ladder up");
        //             climbing = true;
        //             rb.gravityScale = 0f;
        //             rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        //             transform.eulerAngles = new Vector3(0, 0, 90);
                    
        //             currentState = StateMachine.ClimbUp;
        //         }
        //         detectedUpLadder = true;
        //     }
        // } else if (ladderDownInfo == false)
        // {
        //     detectedDownLadder = false;
        // }

        // if (ladderDownInfo != false && ladderUpInfo == false)
        // {
        //     if (!detectedDownLadder) {
        //         int ladderChance = UnityEngine.Random.Range(0, 5);
        //         // int ladderChance = 1;
        //         if (ladderChance == 1)
        //         {
        //             Debug.Log("Ladder down");
        //             climbing = true;
        //             rb.gravityScale = 0f;
        //             rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        //             transform.eulerAngles = new Vector3(0, 0, -90);
                    
        //             currentState = StateMachine.ClimbDown;
        //         }
        //         detectedDownLadder = true;
        //     }
        // } else if (ladderDownInfo == false)
        // {
        //     detectedDownLadder = false;
        // }
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

    private void ClimbUp()
    {
        // // Moves worm
        // transform.Translate(Vector2.right * speed * Time.deltaTime);

        // RaycastHit2D upLadderInfo = Physics2D.Raycast(exitLadderDetection.position, Vector2.down, obstacleRaycastDistance / 2.2f, ladderLayerMask);
        // RaycastHit2D floorInfo = Physics2D.Raycast(exitLadderDetection.position, Vector2.down, obstacleRaycastDistance / 2.2f, groundLayerMask);
        // if (upLadderInfo == false && floorInfo == false)
        // {
        //     // Occurs when raycast from middle of bookworm detects no ladder and no floor
        //     Debug.Log("Returning to patrol");
        //     climbing = false;
        //     rb.gravityScale = 1f;
        //     rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
        //     rb.constraints &= ~RigidbodyConstraints2D.FreezeRotation;
        //     transform.eulerAngles = new Vector3(0, 0, 0);

        //     currentState = StateMachine.Patrol;
        // }
    }

    private void ClimbDown()
    {
        // // Moves worm
        // transform.Translate(Vector2.right * speed * Time.deltaTime);

        // RaycastHit2D pathAheadInfo = Physics2D.Raycast(groundDetection.position, Vector2.right, obstacleRaycastDistance, groundLayerMask);
        // if (pathAheadInfo.collider != false)
        // {
        //     Debug.Log("Returning to patrol");
        //     climbing = false;
        //     rb.gravityScale = 1f;
        //     rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
        //     rb.constraints &= ~RigidbodyConstraints2D.FreezeRotation;
        //     transform.eulerAngles = new Vector3(0, 0, 0);

        //     currentState = StateMachine.Patrol;
        // }
    }

    private void BookwormCaught(object sender, EventArgs e)
    {
        // If the bookworm is touching the player when the event is triggered, assume this is the bookworm that was caught
        if (touchingPlayer)
        {
            currentState = StateMachine.Caught;
            Debug.Log("Bookworm caught!");
            Player player = playerGameObject.GetComponent<Player>();
            player.OnCaughtBookworm -= BookwormCaught;
            rb.gravityScale = 0f;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            touchingPlayer = true;
        }
        
        if (collision.collider.CompareTag("Ladder"))
        {
            // Ignore collisions with ladders
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider, true);
        }
        
        if (collision.collider.CompareTag("Bookworm"))
        {
            // Ignore collisions with other worms
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider, true);
        }
        
        if (climbing && !collision.collider.CompareTag("Player"))
        {
            // Ignore all collisions except for collisions with the player when climbing
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider, true);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            touchingPlayer = false;
        }
    }
}
