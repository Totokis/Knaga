using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Podrygiwanie : MonoBehaviour
{
    private void Start()
    {
        Podryguj();
    }

    public void Podryguj()
    {
        float randomOffset = Random.Range(0f, 0.08f); // mniejsza amplituda
        float randomDelay = Random.Range(0f, 0.2f);  // losowe opóźnienie
        float randomDelay2 = Random.Range(0f, 0.2f);  // losowe opóźnienie
        float targetY = transform.localPosition.y + randomOffset;
        LeanTween.moveLocalY(gameObject, targetY, 0.4f)
            .setEase(LeanTweenType.easeShake)
            .setLoopPingPong()
            .setDelay(randomDelay);

        float randomRot = Random.Range(-1.2f, 1.2f); // jeszcze mniejszy zakres obrotu
        LeanTween.rotateZ(gameObject, randomRot, 0.4f)
            .setEase(LeanTweenType.easeShake)
            .setLoopPingPong()
            .setDelay(randomDelay2);
    }
}

