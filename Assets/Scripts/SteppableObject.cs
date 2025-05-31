using System;
using UnityEngine;

public class SteppableObject : MonoBehaviour
{
    [SerializeField] private GameObject normalObject;
    [SerializeField] private GameObject steppedObject;
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<GórnikKontroller>() != null)
        {
            normalObject.SetActive(false);
            steppedObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<GórnikKontroller>() != null)
        {
            normalObject.SetActive(true);
            steppedObject.SetActive(false);
        }
    }
}
