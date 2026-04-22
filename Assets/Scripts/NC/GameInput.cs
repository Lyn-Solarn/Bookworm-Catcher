using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public event EventHandler OnJump;
    public event EventHandler OnDash;
    public event EventHandler OnDrop;
    
    private PlayerInputActions _playerInputActions;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Enable();

        _playerInputActions.Player.Jump.performed += Jump_performed;
        _playerInputActions.Player.Dash.performed += Dash_performed;
        _playerInputActions.Player.Drop.performed += Drop_performed;
    }

    private void Drop_performed(InputAction.CallbackContext obj)
    {
        OnDrop?.Invoke(this, EventArgs.Empty);
    }

    private void Dash_performed(InputAction.CallbackContext obj)
    {
        OnDash?.Invoke(this, EventArgs.Empty);
    }

    private void Jump_performed(InputAction.CallbackContext obj)
    {
        OnJump?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy()
    {
        _playerInputActions.Dispose();
    }


    public Vector2 GetMovementVectorNormalized()
    {
        return _playerInputActions.Player.Move.ReadValue<Vector2>().normalized;
    }
}
