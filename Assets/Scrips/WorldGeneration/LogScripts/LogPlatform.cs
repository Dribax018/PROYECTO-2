

using UnityEngine;

public class LogPlatform : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") &&
            other.transform.parent == transform)
        {
            other.transform.SetParent(null, true);
        }
    }
    private void OnDisable()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);

            if (child.CompareTag("Player"))
            {
                child.SetParent(null, true);
            }
        }
    }
}
