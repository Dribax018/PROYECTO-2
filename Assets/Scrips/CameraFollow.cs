using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Jugador")]
    [SerializeField] private Transform jugador;

    [Header("Camara")]
    [SerializeField] private Vector3 offset = new Vector3(6f, 8f, -6f);
    [SerializeField] private float suavizado = 0.15f;

    [Header("Seguimiento por cubos")]
    [SerializeField] private bool soloAvanza = true;
    private Vector3 velocidad;
    private float zMaxima;
    private float alturaInicialJugador;

    private void Start()
    {
        if (jugador == null)
        {
            Debug.LogError("No has asignado el jugador a CameraFollow.");
            return;
        }

        zMaxima = jugador.position.z;
        alturaInicialJugador = jugador.position.y;
    }

    private void LateUpdate()
    {
        if (jugador == null)
            return;

        float posicionZ = jugador.position.z;
        if (soloAvanza)
        {
            zMaxima = Mathf.Max(zMaxima, jugador.position.z);
            posicionZ = zMaxima;
        }

        Vector3 posicionJugador = new Vector3(
            jugador.position.x,
            alturaInicialJugador,
            posicionZ
        );

        Vector3 posicionDeseada = posicionJugador + offset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            posicionDeseada,
            ref velocidad,
            suavizado
        );
    }
}