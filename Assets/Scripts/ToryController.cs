using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class ToryController : MonoBehaviour
{
    [SerializeField] private Tory currentTory;
    [SerializeField] private Tory toryPrefab;
    [SerializeField] private float toryLength = 14f;
    
    [ContextMenu("Tools/Place New Tory")]
    public void PlaceNewTory()
    {
        Tory newTory = Instantiate(toryPrefab, currentTory.transform.position+new Vector3(toryLength,0,0),Quaternion.identity, currentTory.transform.parent);
        currentTory.DestroyPreview();
        currentTory = newTory;
    }

    [ContextMenu("Show tory preview")]
    public void ShowToryPreview()
    {
        currentTory.SetActiveToryPreview(true);
    }
    
    [ContextMenu("Hide tory preview")]
    public void HidePreview()
    {
        currentTory.SetActiveToryPreview(false);
    }
}
