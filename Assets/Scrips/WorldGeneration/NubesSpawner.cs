
using System.Collections;
using UnityEngine;

public class NubesSpawner : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject prefabNube;
    [SerializeField] private Transform jugador;

    [Header("Tiempo de aparición")]
    [SerializeField] private float intervalo = 5f;

    [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;
    [SerializeField] private float distanciaLateral = 12f;
    [SerializeField] private float alturaMinima = 7f;
    [SerializeField] private float alturaMaxima = 10f;
    [SerializeField] private float distanciaDelante = 8f;
    [SerializeField] private float variacionZ = 10f;

    private void Start()
    {
        StartCoroutine(GenerarNubes());
    }

    private IEnumerator GenerarNubes()
    {
        while (true)
        {
            yield return new WaitForSeconds(Mathf.Max(0.1f, intervalo));

            if (prefabNube == null) continue;

            if (jugador == null)
            {
                GameObject objeto = GameObject.FindGameObjectWithTag("Player");

                if (objeto != null)
                    jugador = objeto.transform;
            }

            if (jugador != null)
                CrearNube();
        }
    }

    private void CrearNube()
    {
        
        bool desdeIzquierda = Random.value < 0.5f;

        float direccion = desdeIzquierda ? 1f : -1f;

        float x = jugador.position.x + (desdeIzquierda ? -distanciaLateral : distanciaLateral);

        float y = jugador.position.y + Random.Range(alturaMinima, alturaMaxima);

        float z = jugador.position.z + distanciaDelante + Random.Range(-variacionZ, variacionZ);

        Vector3 posicion = new Vector3(x, y, z);

        GameObject nube = Instantiate(prefabNube,posicion,prefabNube.transform.rotation);

        StartCoroutine(MoverNube(nube, direccion));
    }

    private IEnumerator MoverNube(GameObject nube, float direccion)
    {
        float recorrido = distanciaLateral * 2f;

        float distanciaMovida = 0f;

        while (nube != null && distanciaMovida < recorrido)
        {
            float movimiento = velocidad * Time.deltaTime;

            nube.transform.position += Vector3.right * direccion * movimiento;

            distanciaMovida += movimiento;

            yield return null;
        }

        if (nube != null)
            Destroy(nube);
    }
}
