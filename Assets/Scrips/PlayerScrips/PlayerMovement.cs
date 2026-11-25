
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public static event System.Action<Vector3> OnPlayerMoved;

    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private float distancia = 1f;
    [SerializeField] private float duracion = 0.15f;
    [SerializeField] private float salto = 0.3f;
    [SerializeField] private LayerMask obstaculos;

    [SerializeField] public int puntos = 0;

    [Header("Agacharse")]
    [SerializeField] private BoxCollider colliderJugador;
    [SerializeField] private float alturaAgachado = 0.5f;

    private Vector3 sizeOriginal;
    private Vector3 centerOriginal;
    private bool agachado;
    private List<Vector2Int> pos = new List<Vector2Int>();

    private bool moviendose;

    private void Awake()
    {
        if (colliderJugador == null)
            colliderJugador = GetComponent<BoxCollider>();

        sizeOriginal = colliderJugador.size;
        centerOriginal = colliderJugador.center;
    }

    private void OnEnable()
    {
        inputReader.MovePressed += MoverJugador;
        inputReader.CrouchChanged += CambiarAgachado;
    }

    private void OnDisable()
    {
        inputReader.MovePressed -= MoverJugador;
        inputReader.CrouchChanged -= CambiarAgachado;
    }

    private void CambiarAgachado(bool agachar)
    {
        agachado = agachar;

        if (agachar)
        {
            float diferencia = sizeOriginal.y - alturaAgachado;

            colliderJugador.size = new Vector3(sizeOriginal.x, alturaAgachado, sizeOriginal.z);
            colliderJugador.center = new Vector3(centerOriginal.x,centerOriginal.y - diferencia / 2f,centerOriginal.z);
        }
        else
        {
            colliderJugador.size = sizeOriginal;
            colliderJugador.center = centerOriginal;
        }
    }

    private void MoverJugador(Vector2 input)
    {
        if (moviendose)
            return;

        Vector3 direccion;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            direccion = new Vector3(Mathf.Sign(input.x), 0, 0);
        }
        else
        {
            direccion = new Vector3(0, 0, Mathf.Sign(input.y));
        }

        if (Physics.Raycast(transform.position, direccion,
            distancia, obstaculos))
            return;
        StartCoroutine(Mover(direccion));
    }

    private IEnumerator Mover(Vector3 direccion)
    {
        moviendose = true;

        Vector3 inicio = transform.position;
        Vector3 final = inicio + direccion * distancia;
        transform.rotation = Quaternion.LookRotation(direccion);

        float tiempo = 0;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float porcentaje = tiempo / duracion;

            Vector3 posicion = Vector3.Lerp(
                inicio, final, porcentaje);

            posicion.y += Mathf.Sin(porcentaje * Mathf.PI) * salto;

            transform.position = posicion;

            yield return null;
        }

        transform.position = final;
        moviendose = false;
    }
}