using UnityEngine;

public class Tory: MonoBehaviour
{
     [SerializeField] private GameObject toryPreview;

     public void SetActiveToryPreview(bool b)
     {
          if (toryPreview != null)
          {
               toryPreview.SetActive(b);
          }
     }

     public void DestroyPreview()
     {
          if (toryPreview != null)
          {
               Destroy(toryPreview);
          }
     }
}