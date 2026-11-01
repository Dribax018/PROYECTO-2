using UnityEngine;

public class Exit : MonoBehaviour
{
    public GameObject popup_Salir;
    void Start()
    {
        
    }
    public void ShowPopUpSalir()
    {
        popup_Salir.SetActive(true);
    }
    public void HidePopUpSalir()
    {
        popup_Salir.SetActive(false);
    }
    public void SalirApp()
    {
        Application.Quit();
        Debug.Log("ha salido de la app");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
