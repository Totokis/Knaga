using System;
using TMPro;
using UnityEngine;

public class Location : MonoBehaviour
{
    [SerializeField] private string locationName;
    [SerializeField] private TMP_Text text;
    [SerializeField] private PulsingMarker marker;

    [SerializeField] private LineRenderer lineRenderer;
    
    
    private float _initialScaleX;
    private float _initialPositionX;

    private void Awake()
    {
        _initialScaleX = text.transform.localScale.x;
        _initialPositionX = text.transform.position.x;
        text.text = locationName;
        text.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<GórnikKontroller>() != null)
            {
                text.gameObject.SetActive(true);
                LeanTween.scaleX(text.gameObject,_initialScaleX, 0.5f).setFrom(0f).setEase(LeanTweenType.easeOutCubic);
                LeanTween.moveLocalX(text.gameObject, _initialPositionX, 0.5f).setFrom(_initialPositionX-1f).setEase(LeanTweenType.easeOutCubic);
                marker.StopPulsing();
            }
        }
    
    private void OnTriggerExit2D(Collider2D other)
        {
            if (other.GetComponent<GórnikKontroller>() != null)
            {
                LeanTween.scaleX(text.gameObject, 0f, 0.5f).setFrom(_initialScaleX).setEase(LeanTweenType.easeInCubic)
                    .setOnComplete(() => text.gameObject.SetActive(false));
                LeanTween.moveLocalX(text.gameObject, _initialPositionX-1f, 0.5f)
                    .setFrom(text.transform.localPosition.x).setEase(LeanTweenType.easeInCubic);
                marker.Pulse();
            }
        }
}
