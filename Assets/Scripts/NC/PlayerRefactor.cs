using System;
using Unity.VisualScripting;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerRefactor : MonoBehaviour, IBookwormParent
{
    public event EventHandler OnCaughtBookworm;
    public static PlayerRefactor Instance { get; private set; }  //property for singleton pattern
    
    [SerializeField] private GameInput gameInput;
    [SerializeField] private Transform bookwormHoldPoint;
    //movement default numbers
    [SerializeField] private float baseMoveSpeed = 7f;
    [SerializeField] private float slowedMoveSpeed = 5f;
    [SerializeField] private float ladderMoveSpeed = 5f;
    [SerializeField] private float slowedLadderMoveSpeed = 2f;
    [SerializeField] private float dropMoveSpeed = 1f;
    [SerializeField] private float apexHeight = .5f;
    [SerializeField] private float apexTime = .05f;
    //boundaries for player
    [SerializeField] private float groundLevel;
    [SerializeField] private float ceilingLevel;
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
    public event EventHandler MovingActivated;

    private Vector2 previousPosition;
    private State currentState;
    private State previousState;
    //------------------------

    //------Alyssa--------
    private float getMovementX;
    private float getMovementY;
    //--------------------

    private bool _isGrounded;
    private bool _canJump;
    private bool _canDoubleJump;

    //---------MARI-------------------------
    [SerializeField] private float dashDuration = 2f;
    [SerializeField] private float dashCooldownDuration = 5f;
    //--------------------------------------

    private float _dashTimer;
    private bool _dashActive;
    private float _dashCooldownTimer;
    private bool _onLadder;
    private bool _dropping;
    private bool _slowed;
    private bool _slowedLadder;
    private float _slowedLadderTimer = 3f;
    
    private float _jumpVelocity;
    private float _verticalVelocity;
    
    private Bookworm _bookworm;
    private Rigidbody2D _rb;
    private GameObject _player;
    
    private void Awake()
    {
        
        if (Instance != null)
        {
            Debug.LogError("There are multiple instances of the player");
        }
        Instance = this;
        
        _rb = GetComponent<Rigidbody2D>();

        //---------MARI-------------------------
        _dashCooldownTimer = dashCooldownDuration;
        //--------------------------------------
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
    
    
    void FixedUpdate()
    {
        float currentMoveSpeed = baseMoveSpeed;
        //check for slow, then assign move speed
        if (_slowed)
        {
            currentMoveSpeed = slowedMoveSpeed;
        }
        
        //---------MARI-------------------------
        //handle dash timer and cooldown timer
        if (_dashActive)
        {
            _dashTimer += Time.deltaTime;
            currentMoveSpeed *= 1.5f; //move faster during dash
            if (_dashTimer > dashDuration)
            {
                _dashTimer = 0f;
                _dashActive = false;
                _dashCooldownTimer = 0f;
            }
        }
        else if (_dashCooldownTimer < dashCooldownDuration)
        {
            _dashCooldownTimer += Time.deltaTime;
            if (_dashCooldownTimer > dashCooldownDuration)
            {
                _dashCooldownTimer = dashCooldownDuration;
            }
        }
        //--------------------------------------
        //check for on ladder
        _onLadder = Physics2D.CircleCast(transform.position, .05f, Vector2.down, .1f, LayerMask.GetMask("Ladder"));
        //check for on ground/jump capability
        _isGrounded = Physics2D.Raycast(transform.position, Vector3.down, 0.1f, LayerMask.GetMask("GroundLayer"));
        Debug.DrawRay(transform.position, Vector2.down * 1.01f, Color.red);
        _canJump = _isGrounded || _onLadder;
        
        //x movement things
        float moveDistance = currentMoveSpeed * Time.deltaTime;
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        float deltaX = inputVector.x * moveDistance;
        
        //y movement
        float climbSpeed = ladderMoveSpeed;
        if (_slowedLadder)
        {
            climbSpeed = slowedLadderMoveSpeed;
            _slowedLadderTimer -= Time.deltaTime;
            if (_slowedLadderTimer <= 0)
            {
                _slowedLadderTimer = 3f;
                _slowedLadder = false;
            }
        }
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
            _verticalVelocity = inputVector.y * climbSpeed;
        }
        else
        {
            _verticalVelocity = 0f;
        }
        
        float yVelocity = _jumpVelocity + _verticalVelocity;
        float deltaY = yVelocity * Time.deltaTime;
        
        //Debug.Log("Dropping: " + _dropping);
        if (_dropping)
        {
            float dropVelocity = -dropMoveSpeed * gravity * Time.deltaTime;
            deltaY = dropVelocity;
            if (_isGrounded && !Mathf.Approximately(deltaY, dropVelocity))
            {
                _dropping = false;
            }
        }
        
        Vector3 deltaPosition = new Vector3(deltaX, deltaY, 0);
        _rb.MovePosition(transform.position + deltaPosition);

        //Debug.Log(_isGrounded);
        ClampPosition();

        //---------Moises---------
        UpdatePlayerState();
        previousPosition = transform.position;
        previousState = currentState;
        //------------------------

        //------Alyssa-----
        UpdateDeltaMovement(deltaX, deltaY);
        //-----------------
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            _onLadder = true;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GameObject().GetComponent<Bookworm>() && _bookworm == null)
        {
            Bookworm tempWorm =  collision.gameObject.GetComponent<Bookworm>();
            tempWorm.SetObjectParent(this);
            SetBookworm(tempWorm);
        }

        if (collision.GameObject().GetComponent<DepositBox>() && _bookworm != null)
        {
            _bookworm.SetObjectParent(collision.GameObject().GetComponent<DepositBox>());
            ClearBookworm();
        }
        
        //slow speed if entering a hazard
        if (collision.gameObject.layer == LayerMask.NameToLayer("Hazard"))
        {
            _slowed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //go back to normal speed if not in a hazard
        if (collision.gameObject.layer == LayerMask.NameToLayer("Hazard"))
        {
            _slowed = false;
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Bookworm>() && _bookworm == null)
        {
            Bookworm tempWorm =  collision.gameObject.GetComponent<Bookworm>();
            tempWorm.SetObjectParent(this);
            SetBookworm(tempWorm);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            _slowedLadder = true;
        }
    }

    private void GameInput_OnDrop(object sender, EventArgs e)
    {
        //Debug.Log("PlayerRefactor_Dropping");
        _dropping =  true;
    }

    private void GameInput_OnDash(object sender, EventArgs e)
    {
        //---------MARI-------------------------
        if (_dashActive || _dashCooldownTimer < dashCooldownDuration)
        {
            return;
        }
        //--------------------------------------

        _dashActive = true;
        _dashTimer = 0f;
    }

    private void GameInput_OnJump(object sender, EventArgs e)
    {
        if (_canJump)
        {
            //Debug.Log("Player_Jump");
            _jumpVelocity = 2f * apexHeight / apexTime;
            //Moises---------
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
        else if (transform.position.y > ceilingLevel)
        {
            clampedPosition.y = ceilingLevel;
        }

        if (transform.position.x < leftWall)
        {
            clampedPosition.x = leftWall;
        }
        else if (transform.position.x > rightWall)
        {
            clampedPosition.x = rightWall;
        }
        
        transform.position = clampedPosition;
    }

    public bool IsDropping()
    {
        return _dropping;
    }

    public void SetIsDropping(bool value)
    {
        _dropping = value;
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

            /*
            Nick's code -----------------------------------------------------------------------
            */
            // Stop bookworms from moving once they are caught
            WormPatrol wormPatrol = _bookworm.GetComponent<WormPatrol>();
            WormAttackLadder wormAttackLadder = _bookworm.GetComponent<WormAttackLadder>();

            // Check which bookworm type was caught to call the proper method
            if (wormPatrol != null)
            {
                wormPatrol.BookwormCaught();
            } else if (wormAttackLadder != null)
            {
                wormAttackLadder.BookwormCaught();
            }
            /*
            -----------------------------------------------------------------------------------
            */
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
            if(_onLadder && !_isGrounded)
            {
                currentState = State.Climbing;
            }
            //If player is moving downwards, but not on ladder or on ground
            else if (!_isGrounded && !_onLadder && (transform.position.y < previousPosition.y))
            {
                currentState = State.Falling;
            }
            else if(_canJump == false && _canDoubleJump == true)
            {
                currentState = State.SingleJump;
            }
            else if(_canJump == false && _canDoubleJump == false && !_isGrounded)
            {
                currentState = State.DoubleJump;
            }
            //If player is moving left or right, without dashing
            else if ((transform.position.x != previousPosition.x) && Mathf.Approximately(transform.position.y,previousPosition.y))
            {
                //If going from falling to walking
                if (previousState == State.Falling)
                {
                    LandingActivated?.Invoke(this, EventArgs.Empty);
                }
                if (_isGrounded)
                {
                    //if dashing
                    if(_dashActive == true)
                    {
                        currentState = State.Dashing;
                    }
                    //if grounded, changing position, and not dashing
                    else
                    {
                        MovingActivated?.Invoke(this, EventArgs.Empty);
                        currentState = State.Moving;
                    }
                }
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

    public State GetState()
    {
        return currentState;
    }

    //---------MARI-------------------------
    public bool IsDashActive()
    {
        return _dashActive;
    }

    public bool IsDashReady()
    {
        return !_dashActive && _dashCooldownTimer >= dashCooldownDuration;
    }

    public float GetDashDuration()
    {
        return dashDuration;
    }

    public float GetDashTimeRemaining()
    {
        if (!_dashActive)
        {
            return 0f;
        }

        return Mathf.Max(0f, dashDuration - _dashTimer);
    }

    public float GetDashCooldownDuration()
    {
        return dashCooldownDuration;
    }

    public float GetDashCooldownTimeRemaining()
    {
        if (_dashActive)
        {
            return dashCooldownDuration;
        }

        return Mathf.Max(0f, dashCooldownDuration - _dashCooldownTimer);
    }
    //--------------------------------------
    //-------------------------


    //--------Alyssa----------
    private void UpdateDeltaMovement(float deltaX, float deltaY)
    {
        getMovementX = deltaX;
        getMovementY = deltaY;
    }

    public float GetMovementX()
    {
        return getMovementX;
    }

    public float GetMovementY()
    {
        return getMovementY;
    }

    public bool GetIsGrounded()
    {
        return _isGrounded;
    }
    //------------------------
    
}