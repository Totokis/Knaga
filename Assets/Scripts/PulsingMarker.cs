using System;
using UnityEngine;

public class PulsingMarker : MonoBehaviour
{
    private Vector3 _startScale;
    [SerializeField] private float _pulseFactor;

    private void Awake()
    {
        _startScale = transform.localScale;
    }

    private void Start()
       {
           Pulse();
       }

    public void Pulse()
       {
           LeanTween.scale(gameObject, _startScale * _pulseFactor, 0.5f)
               .setEaseInOutSine()
               .setLoopPingPong();
       }

       public void StopPulsing()
       {
              LeanTween.cancel(gameObject);
              transform.localScale = _startScale;
       }
}
