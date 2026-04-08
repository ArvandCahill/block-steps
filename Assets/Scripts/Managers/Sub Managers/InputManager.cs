using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance { get; private set; }

    public PlayerInputAction inputAction;
    public InputActionReference InputActionReference;
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

    public void DisablePlayerMap()
    {
        playerMap.Disable();
    }

    public void EnableCameraMap(bool enable = true)
    {
        if(enable) InputActionReference.action.actionMap.Enable();
        else InputActionReference.action.actionMap.Disable();
    }

    public void DisableAllMap()
    {
        inputAction.Player.Disable();
        InputActionReference.action.actionMap.Disable();
    }
}
