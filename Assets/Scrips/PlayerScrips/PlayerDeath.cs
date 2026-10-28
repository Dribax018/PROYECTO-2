using System;
using UnityEngine;
/// <summary>
/// Va en el jugador. Detecta el choque con un vehículo (LaneVehicle), bloquea el movimiento
/// y avisa a GameSession.
///
/// Requisitos de física (para que Unity lance los eventos):
/// - El jugador y los vehículos necesitan Collider.
/// - Al menos uno de los dos necesita Rigidbody (vale Is Kinematic).
/// - Si el collider del coche es Trigger (o el del jugador), se usa OnTriggerEnter;
///   si ninguno lo es, se usa OnCollisionEnter. Ambos están cubiertos.
/// </summary>
public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private CameraFollow cameraFollow;

    public bool IsDead { get; private set; }
    public event Action OnDied;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<LaneVehicle>() != null) Die();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponentInParent<LaneVehicle>() != null) Die();
    }

    public void Die()
    {
        if (IsDead) return;
        IsDead = true;
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;

        Debug.Log("PlayerDeath: el jugador ha muerto");
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }
        if (cameraFollow == null && Camera.main != null)
        {
            cameraFollow = Camera.main.GetComponent<CameraFollow>();
        }
        if (cameraFollow != null) cameraFollow.enabled = false;

        OnDied?.Invoke();

        if (GameSession.Instance != null) GameSession.Instance.GameOver();
    }
}