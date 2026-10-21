
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

    //crear la lista
    private List<Vector2Int> pos = new List<Vector2Int>();
    private int posicionesGuardadas = 0;

    private bool moviendose;

    private void OnEnable()
    {
        inputReader.MovePressed += MoverJugador;
    }

    private void OnDisable()
    {
        inputReader.MovePressed -= MoverJugador;
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

       
        if (direccion.z > 0)
        {
            ComprobarPosicion(final);
        }

        OnPlayerMoved?.Invoke(final);

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

    private void ComprobarPosicion(Vector3 destino)
    {
        Vector2Int nuevaPosicion = new Vector2Int(
            Mathf.RoundToInt(destino.x),
            Mathf.RoundToInt(destino.z)
        );

        // Comprobar si la posición ya está en la lista.
        if (pos.Contains(nuevaPosicion))
        {
            Debug.Log("Posición ya visitada. No se suman puntos.");
            return;
        }

        // Añadir la nueva posición a la lista.
        pos.Add(nuevaPosicion);

        // Sumar un punto.
        puntos++;
        // puntuacion.text = puntos.ToString();
        GameManager.instance.UpdateScore(puntos);
        Debug.Log("Nueva posición: " + nuevaPosicion);
        Debug.Log("Puntos: " + puntos);
  
    }
}
