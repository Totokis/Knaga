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
            // IMPORTANT: Camera should ALWAYS follow the Player, NOT the Kombajn!
            // DO NOT CHANGE THIS - The player needs to see where they're going
            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                target = player.transform;
                Debug.Log("Camera following Player (CORRECT - DO NOT CHANGE)");
            }
            else
            {
                Debug.LogWarning("Player not found! Camera has no target.");
            }
            
            // NEVER make camera follow Kombajn - it moves automatically
            // and player needs to control their own character
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
    
    // WARNING: This camera script should ALWAYS follow the PLAYER
    // The Kombajn moves on its own and following it would make the game unplayable
    // DO NOT CHANGE the target to Kombajn or KombajnMain!
}