using UnityEngine;

public class DestruirAlChocar : MonoBehaviour
{
   
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("barrera"))
        {
            
            Destroy(gameObject);
        }
    }
}