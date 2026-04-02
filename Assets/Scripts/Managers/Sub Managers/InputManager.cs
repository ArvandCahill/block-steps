using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance { get; private set; }

    public PlayerInputAction inputAction;
    public InputActionMap playerMap;
    public InputActionMap cameraMap;

    private void Awake()
    {
        inputAction = new PlayerInputAction();

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        playerMap = inputAction.Player;
        cameraMap = inputAction.Camera;
    }

    public void EnableAllMap()
    {
        playerMap.Enable();
        cameraMap.Enable();
    }

    public void DisableAllMap()
    {
        inputAction.Player.Disable();
        inputAction.Camera.Disable();
        Debug.Log("All input maps disabled");
    }
}
