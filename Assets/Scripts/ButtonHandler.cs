using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHandler : MonoBehaviour
{
    public static ButtonHandler Instance { get; private set; }

    private void Awake()
    {
        //singleton
        Instance = this;
    }

    public void ResetLevel1()
    {
        Level1Manager.Instance.ResetLevel();
        AfterPress();
    }

    public void ResetLevel2()
    {
        Level2Manager.Instance.ResetLevel();
        AfterPress();
    }

    public void ResetLevel3()
    {
        Level3Manager.Instance.ResetLevel();
        AfterPress();
    }

    public void LoadScene(int i)
    {
        SceneLoader.Instance.LoadScene(i);
        AfterPress();
    }

    public void AfterPress()
    {
        // play click sound
        AudioManager.Instance.Play("button");

        // unselect UI element
        EventSystem.current.SetSelectedGameObject(null);
    }
}
