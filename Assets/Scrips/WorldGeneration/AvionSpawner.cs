
using System.Collections;
using UnityEngine;

public class AvionSpawner : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject prefabAvion;
    [SerializeField] private Transform jugador;

    [Header("Tiempo")]
    [SerializeField] private float intervalo = 15f;

    [Header("Movimiento")]
    [SerializeField] private float velocidad = 10f;
    [SerializeField] private float altura = 6f;
    [SerializeField] private float distanciaDetras = 15f;
    [SerializeField] private float distanciaRecorrido = 50f;
    [SerializeField] private Vector3 rotacionAvion;

    private void Start()
    {
        StartCoroutine(GenerarAviones());
    }

    private IEnumerator GenerarAviones()
    {
        while (true)
        {
            yield return new WaitForSeconds(Mathf.Max(0.1f, intervalo));

            if (GameSession.Instance != null &&GameSession.Instance.IsGameOver)yield break;

            if (prefabAvion == null) continue;

            if (jugador == null)
            {
                GameObject objeto = GameObject.FindGameObjectWithTag("Player");

                if (objeto != null) jugador = objeto.transform;
            }

            if (jugador != null)
                CrearAvion();
        }
    }

    private void CrearAvion()
    {
        Vector3 posicion = new Vector3(jugador.position.x,jugador.position.y + altura,jugador.position.z - distanciaDetras);

        GameObject avion = Instantiate(prefabAvion,posicion,Quaternion.Euler(rotacionAvion));

        StartCoroutine(MoverAvion(avion));
    }

    private IEnumerator MoverAvion(GameObject avion)
    {
        float destinoZ = avion.transform.position.z + distanciaRecorrido;

        while (avion != null && avion.transform.position.z < destinoZ)
        {
            if (GameSession.Instance != null &&GameSession.Instance.IsGameOver) break;

            avion.transform.position +=Vector3.forward * velocidad * Time.deltaTime;

            yield return null;
        }

        if (avion != null) Destroy(avion);
    }
}
