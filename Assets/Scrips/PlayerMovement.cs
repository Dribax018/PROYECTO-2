using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static event System.Action<Vector3> OnPlayerMoved;
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private float distancia = 1f;
    [SerializeField] private float duracion = 0.15f;
    [SerializeField] private float salto = 0.3f;
    [SerializeField] private LayerMask obstaculos;

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
        if (Physics.Raycast(transform.position, direccion, distancia, obstaculos))
            return;
        StartCoroutine(Mover(direccion));
    }
    private IEnumerator Mover(Vector3 direccion)
    {
        moviendose = true;
        Vector3 inicio = transform.position;
        Vector3 final = inicio + direccion * distancia;
        OnPlayerMoved?.Invoke(final);
        transform.rotation = Quaternion.LookRotation(direccion);
        float tiempo = 0;
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float porcentaje = tiempo / duracion;
            Vector3 posicion = Vector3.Lerp(inicio, final, porcentaje);
            posicion.y += Mathf.Sin(porcentaje * Mathf.PI) * salto;
            transform.position = posicion;
            yield return null;
        }
        transform.position = final;
        moviendose = false;
    }
}