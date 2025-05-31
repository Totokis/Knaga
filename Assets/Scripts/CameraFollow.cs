using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 2, -10);
    
    void Start()
    {
        if (target == null)
        {
            // First try to find KombajnMain
            GameObject kombajn = GameObject.Find("KombajnMain");
            if (kombajn != null)
            {
                target = kombajn.transform;
                Debug.Log("Camera following KombajnMain");
            }
            else
            {
                // Fallback to player
                GameObject player = GameObject.Find("Player");
                if (player != null)
                {
                    target = player.transform;
                    Debug.Log("Camera following Player");
                }
            }
        }
    }
    
    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}