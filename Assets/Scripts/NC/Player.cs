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
        float currentMoveSpeed = baseMoveSpeed;
        //handle dash timer
        if (_dashActive)
        {
            _dashTimer += Time.deltaTime;
            currentMoveSpeed = 1.5f*baseMoveSpeed; //move twice as fast during dash
            if (_dashTimer > 2f)
            {
                _dashTimer = 0f;
                _dashActive = false;
            }
        }

        //check for on ladder
        _onLadder = Physics2D.Raycast(transform.position, Vector2.down, .05f, LayerMask.GetMask("Ladder"));
        //check for on ground/jump capability
        _isGrounded = Physics2D.Raycast(transform.position, Vector3.down, 0.1f, LayerMask.GetMask("GroundLayer"));
        Debug.DrawRay(transform.position, Vector2.down * 1.01f, Color.red);
        _canJump = _isGrounded || _onLadder;
        
        //x movement things
        float moveDistance = currentMoveSpeed * Time.deltaTime;
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        float deltaX = inputVector.x * moveDistance;
        
        //falling if not on ground or onLadder
        float gravity = 2f * apexHeight / (apexTime * apexTime);
        if (!_isGrounded && !_onLadder)
        {
            _jumpVelocity -= gravity * Time.deltaTime;
        }
        else if (_jumpVelocity <= -.1f)
        {
            _jumpVelocity = 0f;
        }

        if (_onLadder)
        {
            //Debug.Log("Player_OnLadder");
            _verticalVelocity = inputVector.y * ladderMoveSpeed;
        }
        else
        {
            _verticalVelocity = 0f;
        }
        
        float yVelocity = _jumpVelocity + _verticalVelocity;
        float deltaY = yVelocity * Time.deltaTime;

        if (_dropping)
        {
            deltaY = -dropMoveSpeed * gravity * Time.deltaTime;
            _dropping = false;
        }
        
        transform.position += new Vector3(deltaX, deltaY, 0);

        Debug.Log(_isGrounded);
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
