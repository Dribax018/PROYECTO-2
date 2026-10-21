using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CambiarEscena : MonoBehaviour
{
    [Header("Nombre de la escena a cargar")]
    public string nombreEscena;

    private Button boton;

    private void Awake()
    {
        // Obtener el componente Button del mismo GameObject
        boton = GetComponent<Button>();

        if (boton == null)
        {
            Debug.LogError("No se encontró un componente Button en este GameObject.");
            return;
        }

        // Asignar el evento de click
        boton.onClick.AddListener(Cambiar);
    }

    private void Cambiar()
    {
        Debug.Log("Botón presionado, cargando escena: " + nombreEscena);

        if (!string.IsNullOrEmpty(nombreEscena))
        {
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            Debug.LogWarning("No se ha asignado ninguna escena en el inspector.");
        }
    }
}
