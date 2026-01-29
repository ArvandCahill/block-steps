using UnityEngine;

public class ButtonUI : MonoBehaviour
{
    [SerializeField] private ButtonType buttonType;

    private bool isPopupActive = false;

    public void OnClick(string name)
    {
        switch (buttonType)
        {
            case ButtonType.LoadScene:
                GameManager.instance.SceneLoader.LoadScene(name);
                break;
            case ButtonType.UIAction:
                ShowPopup(name);
                break;
            case ButtonType.Restart:
                GameManager.instance.SceneLoader.RestartScene();
                break;
        }
    }

    public void ShowPopup(string name)
    {
        if (!isPopupActive)
        {
            isPopupActive = true;
            GameManager.instance.uiManager.ShowPopup(name, true);
        }

        else GameManager.instance.uiManager.HidePopup(name);
    }
}
