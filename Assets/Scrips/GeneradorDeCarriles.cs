using UnityEngine;
using UnityEngine.InputSystem;

public class GeneradorDeCarriles : MonoBehaviour,ICarril
{
    [SerializeField] private GameObject[] terreno;
    [SerializeField] private Transform spawn;

    public void inicializar()
    {
       
    }

    public void r()
    {
       
    }

    private void SpawnTerr()
    {
        int index = Random.Range(0, terreno.Length);
        Instantiate(terreno[index], spawn.position, spawn.rotation);
    }


}
