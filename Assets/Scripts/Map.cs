using System.Linq;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private GameObject mapElements;
    [SerializeField] private Animator scrollAnimation;
    
    
    private void Start()
    {
        LeanTween.rotateZ(gameObject,0f,0.5f).setFrom(30f).setEase(LeanTweenType.easeOutCubic);
        LeanTween.scale(gameObject, Vector3.one * 0.35f, 0.5f)
            .setFrom(Vector3.one)
            .setEase(LeanTweenType.easeOutBounce).setOnComplete(() =>
            {
                mapElements.SetActive(true);
                LeanTween.color(mapElements, Color.black, 0.5f).setFromColor(Color.clear)
                    .setDelay(scrollAnimation.runtimeAnimatorController.animationClips.First().length+0.5f);
            });
    }
}