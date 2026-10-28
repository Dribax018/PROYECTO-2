using UnityEngine;

public class PuntoDeAparicion : MonoBehaviour
{
    public GameObject player;
    public Transform puntoAparicion;
    public bool aparecido = false;
    void Start()
    {
        Aparecer();
    }

    public void Aparecer()
    {
        aparecido = true;
       
        if (aparecido == true)
        {
            Instantiate(player, puntoAparicion.position, puntoAparicion.rotation);
            aparecido =false;
        }
       
        
    }
}
