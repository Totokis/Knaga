using System.Collections.Generic;
using UnityEngine;

public class PodrygiwanieController : MonoBehaviour
{
    [SerializeField] private List<Podrygiwanie> podrygiwania;


    public void StopPodrygiwanie()
    {
        foreach (var podrygiwanie in podrygiwania)
        {
            if (podrygiwanie != null)
            {
                LeanTween.cancel(podrygiwanie.gameObject);
            }
        }
    } 
    
    public void StartPodrygiwanie()
    {
        foreach (var podrygiwanie in podrygiwania)
        {
            if (podrygiwanie != null)
            {
                podrygiwanie.Podryguj();
            }
        }
    }
        
}
