using System;
using UnityEngine;

public class SteppableObject : MonoBehaviour
{
    [SerializeField] private GameObject steppedObject;
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<GórnikKontroller>() != null)
        {
            
            steppedObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<GórnikKontroller>() != null)
        {
            steppedObject.SetActive(false);
        }
    }
}
