using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Location : MonoBehaviour
{
    [SerializeField] private string locationName;
    [SerializeField] private GameObject sign;
    [SerializeField] private PulsingMarker marker;

    [SerializeField] private LineRenderer lineRenderer;
    
    
    private float _initialScaleX;
    private float _initialPositionX;
    private bool _in;

    private void Awake()
    {
        InputSystem_Actions input = new InputSystem_Actions();
        input.Enable();
        
        input.Player.Jump.performed += ctx =>
        {
            if (_in)
            {
                LoadNextScene();
            }
        };
        
        input.Player.Interact.performed += ctx =>
        {
          RestartGame();
        };
       
        _initialScaleX = sign.transform.localScale.x;
        _initialPositionX = sign.transform.localPosition.x; // poprawka: używaj localPosition
        
        sign.gameObject.SetActive(false);
    }
    
    public void RestartGame()
    {
        int currentIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentIndex);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<GórnikKontroller>() != null)
        {
            sign.gameObject.SetActive(true);
            LeanTween.scaleX(sign.gameObject,_initialScaleX, 0.5f).setFrom(0f).setEase(LeanTweenType.easeOutCubic);
            LeanTween.moveLocalX(sign.gameObject, _initialPositionX, 0.5f).setFrom(_initialPositionX-1f).setEase(LeanTweenType.easeOutCubic); // większa różnica, by efekt był widoczny, ale zawsze wokół _initialPositionX
            marker.StopPulsing();
            _in = true;
        }
    }
    
    public void LoadNextScene()
    {
        int currentIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;
        if (nextIndex < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.Log("[Location] Brak kolejnej sceny do załadowania.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<GórnikKontroller>() != null)
        {
            LeanTween.scaleX(sign.gameObject, 0f, 0.5f).setFrom(_initialScaleX).setEase(LeanTweenType.easeInCubic)
                .setOnComplete(() => sign.gameObject.SetActive(false));
            LeanTween.moveLocalX(sign.gameObject, _initialPositionX-1f, 0.5f)
                .setFrom(_initialPositionX).setEase(LeanTweenType.easeInCubic); // poprawka: zawsze wokół _initialPositionX
            marker.Pulse();
        }
    }
}
