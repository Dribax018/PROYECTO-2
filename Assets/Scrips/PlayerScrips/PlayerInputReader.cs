using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    public event Action<Vector2> MovePressed;
    public event Action<bool> CrouchChanged;  

    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        playerInput.onActionTriggered += LeerInput;
    }

    private void OnDisable()
    {
        playerInput.onActionTriggered -= LeerInput;
    }

    private void LeerInput(InputAction.CallbackContext context)
    {
        if (context.action.name == "move" && context.performed)
        {
            Vector2 direccion = context.ReadValue<Vector2>();
            MovePressed?.Invoke(direccion);
        }
        else if (context.action.name == "crouch")
        {
            if (context.started) CrouchChanged?.Invoke(true);
            else if (context.canceled) CrouchChanged?.Invoke(false);
        }
    }
}