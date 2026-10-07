using UnityEngine;

public class CrossyRoadCompleteCamera : MonoBehaviour
{
    public Transform target;        
    public Vector3 offset;           
    public float autoMoveSpeed = 1.5f; 
    public float smoothSpeedX = 5f;    
    public float smoothSpeedZ = 5f;   

    void Start()
    {
        if (offset == Vector3.zero && target != null)
        {
            offset = transform.position - target.position;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;
        float targetX = target.position.x + offset.x;
        float nextX = Mathf.Lerp(transform.position.x, targetX, smoothSpeedX * Time.deltaTime);
        float nextZ = transform.position.z + (autoMoveSpeed * Time.deltaTime);
        float targetZ = target.position.z + offset.z;
        if (targetZ > nextZ)
        {
            nextZ = Mathf.Lerp(nextZ, targetZ, smoothSpeedZ * Time.deltaTime);
        }
        transform.position = new Vector3(nextX, target.position.y + offset.y, nextZ);
    }
}
