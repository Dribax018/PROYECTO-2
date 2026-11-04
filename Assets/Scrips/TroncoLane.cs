using UnityEngine;

public class MoverTronco : MonoBehaviour
{
    public float velocidad = 3f;
    public int direccion = 1; 

    void Update()
    {
        transform.Translate(Vector3.right * direccion * velocidad * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }
}
