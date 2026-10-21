using System;
using UnityEngine;

/// <summary>
/// Sustituye a VehicleMovement. Avanza en línea recta hacia delante (eje Z local)
/// y avisa al carril cuando llega al punto de salida para que lo devuelva al pool.
/// El eje Z (azul) del prefab del coche debe apuntar hacia donde mira el coche.
/// </summary>
public class LaneVehicle : MonoBehaviour
{
    private float speed;
    private Vector3 endPosition;
    private Action<LaneVehicle> onFinished;

    public void Init(float speed, Vector3 endPosition, Action<LaneVehicle> onFinished)
    {
        this.speed = speed;
        this.endPosition = endPosition;
        this.onFinished = onFinished;
    }

    private void Update()
    {
        transform.position += transform.forward * (speed * Time.deltaTime);
        if (Vector3.Dot(endPosition - transform.position, transform.forward) <= 0f)
        {
            onFinished?.Invoke(this);
        }
    }
}