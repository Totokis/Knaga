using System;
using UnityEngine;

public class GórnikKontroller : MonoBehaviour
{
    private InputSystem_Actions _playerInput;

    private void Awake()
    {
        _playerInput = new InputSystem_Actions();
        _playerInput.Enable();
    }


    private void Update()
    {
        var directions = _playerInput.Player.Move.ReadValue<Vector2>();
        transform.position += (Vector3)directions * Time.deltaTime * 5f; // 5f to przykładowa prędkość
    }
}
