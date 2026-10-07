using UnityEngine;

public class VehicleMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;
    public Vector3 direccion = Vector3.right;
    [Header("Puntos")]
    public Transform spawnPoint;
    public Transform finPoint;
    [Header("Modelo 3D")]
    public GameObject modelo3D;
    void Start()
    {
        transform.position = spawnPoint.position;
    }
    void Update()
    {
        transform.Translate
        (
            direccion.normalized * velocidad * Time.deltaTime,
            Space.World
        );


        if (Vector3.Distance(transform.position, finPoint.position) < 0.5f)
        {
            ReiniciarCoche();
        }
    }
    void ReiniciarCoche()
    {
        modelo3D.SetActive(false);
        transform.position = spawnPoint.position;
        modelo3D.SetActive(true);
    }
}
