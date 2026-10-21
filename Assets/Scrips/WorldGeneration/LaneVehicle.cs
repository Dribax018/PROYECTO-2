using System;
using UnityEngine;
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