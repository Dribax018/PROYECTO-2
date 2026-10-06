using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerInputReader inputReader;

    [Header("Movimiento")]
    [SerializeField] private float distanciaPaso = 1f;
    [SerializeField] private float duracionMovimiento = 0.15f;
    [SerializeField] private float alturaSalto = 0.35f;

    private bool moviendose;

    private void OnEnable()
    {
        inputReader.MovePressed += HandleMove;
    }

    private void OnDisable()
    {
        inputReader.MovePressed -= HandleMove;
    }

    private void HandleMove(Vector2 input)
    {
        if (moviendose)
            return;

        Vector3 direccion = new Vector3(input.x, 0, input.y);

        if (direccion != Vector3.zero)
        {
            StartCoroutine(Mover(direccion));
        }
    }

    private IEnumerator Mover(Vector3 direccion)
    {
        moviendose = true;

        Vector3 posicionInicial = transform.position;

        Vector3 posicionFinal = posicionInicial + direccion.normalized * distanciaPaso;

        // Giramos hacia la dirección de movimiento
        transform.rotation = Quaternion.LookRotation(direccion);

        float tiempo = 0f;

        while (tiempo < duracionMovimiento)
        {
            tiempo += Time.deltaTime;

            float porcentaje = tiempo / duracionMovimiento;

            Vector3 posicion = Vector3.Lerp(posicionInicial, posicionFinal, porcentaje
                );

            // Salto estilo Crossy Road
            float salto = Mathf.Sin(porcentaje * Mathf.PI) * alturaSalto;

            posicion.y += salto;

            transform.position = posicion;

            yield return null;
        }

        transform.position = posicionFinal;

        moviendose = false;
    }
}