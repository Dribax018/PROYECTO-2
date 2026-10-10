using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Vista isométrica")]
    [SerializeField] private float pitch = 30f;
    [SerializeField] private float yaw = 45f;
    [SerializeField] private float distance = 20f;
    [Header("Seguimiento")]
    [SerializeField] private float smoothTime = 0.25f;
    [SerializeField] private bool followSideways = true;
    [Header("Avance infinito")]
    [SerializeField] private float forwardSpeed= 5f;

    private Vector3 focusPoint;
    private Vector3 velocity;
    private float maxForwardZ;

    private void Awake()
    {
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private void OnEnable()
    {
        PlayerMovement.OnPlayerMoved += HandlePlayerMoved;
    }
    private void OnDisable()
    {
        PlayerMovement.OnPlayerMoved -= HandlePlayerMoved;
    }
    private void Start()
    {
        var player = FindFirstObjectByType<PlayerMovement>();
        if (player != null)
        {
            focusPoint = player.transform.position;
            maxForwardZ = focusPoint.z;
            transform.position = focusPoint - transform.forward * distance;
        }
    }

    private void HandlePlayerMoved(Vector3 newPosition)
    {
        maxForwardZ = Mathf.Max(maxForwardZ, newPosition.z);
        focusPoint = new Vector3
        (
            followSideways ? newPosition.x : focusPoint.x,
            0f,
            maxForwardZ
        );
    }

    private void LateUpdate()
    {
        maxForwardZ += forwardSpeed * Time.deltaTime;
        focusPoint.z = maxForwardZ;
        Vector3 targetPos = focusPoint - transform.forward * distance;
        transform.position = Vector3.SmoothDamp
        (
            transform.position,
            targetPos,
            ref velocity,
            smoothTime
        );
    }
}