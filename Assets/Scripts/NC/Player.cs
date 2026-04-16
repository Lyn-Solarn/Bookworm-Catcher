using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour
{
    
    [SerializeField] private GameInput gameInput;
    [SerializeField] private float baseMoveSpeed = 7f;
    [SerializeField] private float apexHeight = .5f;
    [SerializeField] private float apexTime = .05f;

    private bool _isGrounded;
    private bool _canJump;
    private bool _canDoubleJump;
    private float _dashTimer;
    private bool _dashActive = false;
    private float _jumpVelocity = 0f;
    
    void Start()
    {
        gameInput.OnJump += GameInput_OnJump;
        gameInput.OnDash += GameInput_OnDash;
    }

    private void GameInput_OnDash(object sender, EventArgs e)
    {
        _dashActive = true;
        _dashTimer = 0f;
    }

    private void GameInput_OnJump(object sender, EventArgs e)
    {
        if (_canJump)
        {
            Debug.Log("Player_Jump");
            _jumpVelocity = 2f * apexHeight / apexTime;
            _canJump = false;
            _canDoubleJump = true;
        }
        else if (_canDoubleJump)
        {
            Debug.Log("Player_DoubleJump");
            _jumpVelocity = 2f * apexHeight / apexTime;
            _canJump = false;
            _canDoubleJump = false;
        }
    }

    // Update is called once per frame
    void Update()
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
        
        _isGrounded = Physics2D.Raycast(transform.position, Vector3.down, .005f);
        _canJump = _isGrounded;
        
        //x movement things
        float moveDistance = currentMoveSpeed * Time.deltaTime;
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        float deltaX = inputVector.x * moveDistance;
        
        //y movement things
        float gravity = 2f * apexHeight / (apexTime * apexTime);
        if (!_isGrounded)
        {
            _jumpVelocity -= gravity * Time.deltaTime;
        }
        else if (_jumpVelocity <= -.1f)
        {
            _jumpVelocity = 0f;
        }
        
        float deltaY = _jumpVelocity * Time.deltaTime;
        
        transform.position += new Vector3(deltaX, deltaY, 0);
        
    }
    
}
