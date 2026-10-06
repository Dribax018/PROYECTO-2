using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputReader : MonoBehaviour
{
    public Vector2 MoveValue { get; private set; }

    public event Action<Vector2> MovePressed;

    private PlayerInput playerInput;

    private const string MoveAction = "move";

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        playerInput.onActionTriggered += HandleAction;
    }

    private void OnDisable()
    {
        playerInput.onActionTriggered -= HandleAction;
        MoveValue = Vector2.zero;
    }

    private void HandleAction(InputAction.CallbackContext ctx)
    {
        if (ctx.action.name != MoveAction)
            return;

        MoveValue = ctx.ReadValue<Vector2>();

        if (ctx.performed)
        {
            MovePressed?.Invoke(MoveValue);
        }
    }
}