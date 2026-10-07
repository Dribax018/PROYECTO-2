using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    public event Action<Vector2> MovePressed;
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
    }
}
