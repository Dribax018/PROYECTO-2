
using UnityEngine;

public class LogMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;
    [SerializeField] private Transform puntoInicio;
    [SerializeField] private Transform puntoFinal;

    private bool moviendo = true;

    private void Update()
    {
        if (!moviendo) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            puntoFinal.position,
            velocidad * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, puntoFinal.position) < 0.05f)
        {
            transform.position = puntoInicio.position;
        }
    }
}
