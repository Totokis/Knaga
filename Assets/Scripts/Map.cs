using System;
using System.Linq;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private GameObject mapElements;
    [SerializeField] private Animator scrollAnimation;
    [SerializeField] private ParticleSystem pSystem;
    [SerializeField] private Pocztowka pocztowka;
    
    private Vector3 _initialScale;


    private void Awake()
    {
        _initialScale = transform.localScale;
    }

    private void Start()
    {
        LeanTween.rotateZ(gameObject,0f,0.5f).setFrom(30f).setEase(LeanTweenType.easeOutCubic);
        LeanTween.scale(gameObject, _initialScale, 0.5f)
            .setFrom(Vector3.one)
            .setEase(LeanTweenType.easeOutBounce).setOnComplete(() =>
            {
                mapElements.SetActive(true);
                LeanTween.color(mapElements, Color.black, 0.5f).setFromColor(Color.clear)
                    .setDelay(scrollAnimation.runtimeAnimatorController.animationClips.First().length+0.5f).setOnComplete((
                        () =>
                        {
                            pocztowka.ShowPocztowka();
                        } ));
              
                pSystem.Play();
            });
        
        
    }
}