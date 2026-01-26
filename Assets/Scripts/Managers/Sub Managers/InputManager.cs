using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance { get; private set; }

    public PlayerInputAction inputAction;
    private InputActionMap playerMap;
    private InputActionMap cameraMap;

    private void OnEnable()
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

    private void Awake()
    {
       
    }
}
