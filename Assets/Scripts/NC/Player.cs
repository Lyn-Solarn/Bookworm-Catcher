using System;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour, IBookwormParent
{
    public event EventHandler OnCaughtBookworm;
    public static Player Instance { get; private set; }  //property for singleton pattern
    
    [SerializeField] private GameInput gameInput;
    [SerializeField] private Transform bookwormHoldPoint;
    //movement default numbers
    [SerializeField] private float baseMoveSpeed = 7f;
    [SerializeField] private float ladderMoveSpeed = 2f;
    [SerializeField] private float dropMoveSpeed = 2f;
    [SerializeField] private float apexHeight = .5f;
    [SerializeField] private float apexTime = .05f;
    //boundaries for player
    [SerializeField] private float groundLevel;
    [SerializeField] private float leftWall = -9f;
    [SerializeField] private float rightWall = 9f;
    
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float footOffset = 0.5f;   // Center to feet
    [SerializeField] private float skin = 0.02f;        // Small landing buffer

    //---------Moises---------
    public enum State
    {
        Idle,
        Moving,
        Climbing,
        Falling,
        SingleJump,
        DoubleJump,
        Dashing
    }

    public event EventHandler SingleJumpActivated;
    public event EventHandler DoubleJumpActivated;
    public event EventHandler LandingActivated;
    //public event EventHandler WalkingActivated;

    private Vector2 previousPosition;
    private State currentState;
    private State previousState;
    //------------------------

    private bool _isGrounded;
    private bool _canJump;
    private bool _canDoubleJump;
    private float _dashTimer;
    private bool _dashActive;
    private bool _onLadder;
    private bool _dropping;
    
    private float _jumpVelocity;
    private float _verticalVelocity;
    
    private Bookworm _bookworm;
    
    private void Awake()
    {
        
        if (Instance != null)
        {
            Debug.LogError("There are multiple instances of the player");
        }
        Instance = this;
        
    }
    
    void Start()
    {
        gameInput.OnJump += GameInput_OnJump;
        gameInput.OnDash += GameInput_OnDash;
        gameInput.OnDrop += GameInput_OnDrop;

        //Moises---------
        previousPosition = transform.position;
        previousState = State.Idle;
        currentState = State.Idle;
        //---------------
    }

    // Update is called once per frame
    
    /*
     * Notes:
     * While falling too fast you sink into the platform
     * After sinking, the y-level that you sink to becomes the new y-level for future jumps on that platform
     * 
     * Proposed fix:
     * Reset the y-position of the player to be ON level with the platform
     * once it reaches below the y-threshold of the platform
     *
     * Temp changes / fixes for level parser implimentation:
     * I had to change the Raycast to be longer to scale with the larger size of the player sprite
     * I added a colored Raycast for debugging, so we know the length of the Physics2D.Raycast
     * 
    */

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        float currentMoveSpeed = baseMoveSpeed;

        //handle dash timer
        if (_dashActive)
        {
            _dashTimer += dt;
            currentMoveSpeed = 1.5f * baseMoveSpeed; //move faster during dash
            if (_dashTimer > 2f)
            {
                _dashTimer = 0f;
                _dashActive = false;
            }
        }

        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector2 startPosition = transform.position;
        Vector2 feetStart = startPosition + Vector2.down * footOffset;

        //check for on ladder
        _onLadder = Physics2D.Raycast(feetStart, Vector2.down, 0.1f, LayerMask.GetMask("Ladder"));

        //x movement things
        float moveDistance = currentMoveSpeed * dt;
        float deltaX = inputVector.x * moveDistance;

        //falling if not on ground or on ladder
        float gravity = 2f * apexHeight / (apexTime * apexTime);
        if (!_isGrounded && !_onLadder)
        {
            _jumpVelocity -= gravity * dt;
        }
        else if (_jumpVelocity < 0f)
        {
            _jumpVelocity = 0f;
        }

        if (_onLadder)
        {
            _verticalVelocity = inputVector.y * ladderMoveSpeed;
        }
        else
        {
            _verticalVelocity = 0f;
        }

        float yVelocity = _jumpVelocity + _verticalVelocity;
        float deltaY = yVelocity * dt;

        if (_dropping)
        {
            deltaY = -dropMoveSpeed * dt;
            _dropping = false;
        }

        // Start by assuming previous grounded state may continue
        bool landedThisFrame = false;

        // Only try to land on platforms when moving downward and not on ladder
        if (!_onLadder && deltaY <= 0f)
        {
            float castDistance = Mathf.Abs(deltaY) + skin + 0.05f;

            RaycastHit2D hit = Physics2D.Raycast(
                feetStart + Vector2.up * 0.05f,
                Vector2.down,
                castDistance,
                groundMask
            );

            Debug.DrawRay(
                feetStart + Vector2.up * 0.05f,
                Vector2.down * castDistance,
                hit.collider != null ? Color.green : Color.red
            );

            if (hit.collider != null && hit.normal.y > 0.5f)
            {
                float platformTop = hit.collider.bounds.max.y;
                float feetEndY = feetStart.y + deltaY;


                // If the player's feet cross the platform top during this physics step, snap the player onto the platform and reset downward velocity.
                if (feetStart.y >= platformTop - skin && feetEndY <= platformTop + skin)
                {
                    Vector2 snappedPosition = startPosition;
                    snappedPosition.x += deltaX;
                    snappedPosition.y = platformTop + footOffset + skin;

                    transform.position = snappedPosition;

                    _isGrounded = true;
                    _canJump = true;
                    _canDoubleJump = false;
                    _jumpVelocity = 0f;
                    _verticalVelocity = 0f;
                    landedThisFrame = true;

                    ClampPosition();

                    //---------Moises---------
                    UpdatePlayerState();
                    previousPosition = transform.position;
                    previousState = currentState;
                    //------------------------

                    return;
                }
            }
        }

        // No landing this frame, move normally
        transform.position = startPosition + new Vector2(deltaX, deltaY);

        // Short grounded probe after movement so standing still still counts as grounded
        Vector2 feetNow = (Vector2)transform.position + Vector2.down * footOffset;

        RaycastHit2D groundedHit = Physics2D.Raycast(
            feetNow + Vector2.up * 0.1f,
            Vector2.down,
            0.2f,
            groundMask
        );

        _isGrounded = groundedHit.collider != null;

        Debug.DrawRay(
            feetNow + Vector2.up * 0.1f,
            Vector2.down * 0.2f,
            _isGrounded ? Color.green : Color.red
        );

        _canJump = _isGrounded || _onLadder;

        if (_isGrounded && !landedThisFrame && _jumpVelocity < 0f)
        {
            _jumpVelocity = 0f;
        }

        ClampPosition();

        //---------Moises---------
        UpdatePlayerState();
        previousPosition = transform.position;
        previousState = currentState;
        //------------------------
    }
    
    
    private void GameInput_OnDrop(object sender, EventArgs e)
    {
        _dropping =  true;
    }

    private void GameInput_OnDash(object sender, EventArgs e)
    {
        _dashActive = true;
        _dashTimer = 0f;

        //Moises---------
        currentState = State.Dashing;
        //---------------
    }

    private void GameInput_OnJump(object sender, EventArgs e)
    {
        Debug.Log("Jump");
        if (_canJump)
        {
            //Debug.Log("Player_Jump");
            _jumpVelocity = 2f * apexHeight / apexTime;
            //Moises---------
            currentState = State.SingleJump;
            SingleJumpActivated?.Invoke(this, EventArgs.Empty);
            //---------------
            _canJump = false;
            _canDoubleJump = true;
        }
        else if (_canDoubleJump)
        {
            //Debug.Log("Player_DoubleJump");
            _jumpVelocity = 2f * apexHeight / apexTime;
            //Moises---------
            currentState = State.DoubleJump;
            DoubleJumpActivated?.Invoke(this, EventArgs.Empty);
            //---------------
            _canJump = false;
            _canDoubleJump = false;
        }
    }

    private void ClampPosition()
    {
        //check for position and make sure it doesn't fall below ground level or side walls
        Vector3 clampedPosition = transform.position;
        if (transform.position.y < groundLevel)
        {
            clampedPosition.y = groundLevel;
        }

        if (transform.position.x < leftWall)
        {
            clampedPosition.x = leftWall;
        }

        if (transform.position.x > rightWall)
        {
            clampedPosition.x = rightWall;
        }
        
        transform.position = clampedPosition;
    }
    
    
    
    
    //Bookworm Holding information
    public Transform GetBookwormTransform()
    {
        return bookwormHoldPoint;
    }

    public void SetBookworm(Bookworm bookworm)
    {
        _bookworm = bookworm;

        if (_bookworm != null)
        {
            //TODO: Finish Player SetBookworm
            OnCaughtBookworm?.Invoke(this, EventArgs.Empty);
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

    //---------Moises---------
    private void UpdatePlayerState()
    {
        //If player is changing direction
        if((Vector2)transform.position != previousPosition)
        {
            //If Player is moving along a ladder
            if(_onLadder)
            {
                currentState = State.Climbing;
            }
            //If player is moving downwards, but not on ladder or on ground
            else if (!_isGrounded && !_onLadder && (transform.position.y < previousPosition.y))
            {
                currentState = State.Falling;
            }
            //If player is moving left or right, without dashing
            else if ((transform.position.x != previousPosition.x) && (transform.position.y == previousPosition.y) && !_dashActive)
            {
                //If going from falling to walking
                if (previousState == State.Falling)
                {
                    LandingActivated?.Invoke(this, EventArgs.Empty);
                }
                //WalkingActivated?.Invoke(this, EventArgs.Empty);
                currentState = State.Moving;
            }
        }
        //If player is not changing direction
        else
        {
            //If going from falling to Idle
            if (previousState == State.Falling)
            {
                LandingActivated?.Invoke(this, EventArgs.Empty);
            }
            currentState = State.Idle;
        }

        //Debug.Log("Current State: " + currentState);
    }

    public bool isWalking()
    {
        return currentState == State.Moving;
    }
    //-------------------------
    
}
