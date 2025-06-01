using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemMonoBehaviour : MonoBehaviour
{
    public Item item;
    private Vector3 offset;
    private float zCoord;
    
    private InputSystem_Actions _input;
    private Vector3 curScreenPos;
    private bool isDragging;
    [SerializeField] private bool FuseReady;
    private bool _fuseTime;

    public bool isClickedOn
    {
        get
        {
         Ray ray = Camera.main.ScreenPointToRay(curScreenPos);
         RaycastHit hit;
         if (Physics.Raycast(ray, out hit))
         {
             return hit.transform == transform;
         }

         return false;
        }
        
    }

    private Vector3 WorldPos
    {
        get
        {
            var z = Camera.main.WorldToScreenPoint(transform.position).z;
            return Camera.main.ScreenToWorldPoint(curScreenPos + new Vector3(0,0,z));
        }
    }

    private void Awake()
    {
        _input = new InputSystem_Actions();
        _input.Enable();
        _input.Mouse.ScreenPos.performed += ctx => 
        {
          curScreenPos = ctx.ReadValue<Vector2>();
        };

        _input.Mouse.Press.performed += _ => {
            if (isClickedOn)
            {
                StartCoroutine(Drag());    
            }

             
        };
        
        _input.Mouse.Press.canceled += _ => {
            isDragging = false;
        };
        
    }

    private IEnumerator Drag()
    {
        isDragging = true;
        var offset = transform.position - WorldPos;

        while (isDragging)
        {
            transform.position = WorldPos + offset;
            yield return null;
        }
        
    }

    public void SetItem(Item newItem)
    {
        item = newItem;
        if (item != null)
        {
            // Ustaw nazwę GameObject
            gameObject.name = item.itemName;
        
            // Ustaw sprite, jeśli istnieje komponent SpriteRenderer
            var sr = GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                sr = gameObject.AddComponent<SpriteRenderer>();
                sr.sortingOrder = 80; // Ustawienie porządku sortowania, jeśli potrzebne
                
            }

            
            
            if (sr != null && item.icon != null)
            {
                sr.sprite = item.icon;
                sr.color = item.color;
            }
            
            gameObject.AddComponent<SphereCollider>();
            
        }
    }

  


    public Item GetItem()
    {
        return item;
    }

    public void MarkReadyToFuse(bool b)
    {
        FuseReady = b;
        if (FuseReady)
        {
           LeanTween.rotateZ(gameObject, 10f, 0.15f)
               .setEase(LeanTweenType.easeShake)
               .setLoopPingPong()
               .setOnComplete(() => LeanTween.rotateZ(gameObject, 0f, 0.1f));
        }
        else
        {
            if (_fuseTime)
            {
                return; 
            }
            LeanTween.cancel(gameObject);
        }
    }

    public void FuseTime()
    {
        _fuseTime = true;
    }
}

