using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    public void ResetLevel1()
    {
        Level1Manager.Instance.ResetLevel();
    }

    public void ResetLevel2()
    {
        Level2Manager.Instance.ResetLevel();
    }

    public void LoadScene(int i)
    {
        SceneLoader.Instance.LoadScene(i);
    }

    public void ClickSound()
    {
        AudioManager.Instance.Play("ButtonClick"); // not implemented
    }
}
