using UnityEngine;
using UnityEngine.UIElements;

public class Level3Manager : MonoBehaviour
{
    [SerializeField] private GameObject winPanel;

    public static Level3Manager Instance { get; private set; }

    private void Awake()
    {
        // singleton
        Instance = this;
    }

    public void ResetLevel()
    {
        return;
    }

    // activate win panel, increment level 3 score
    private void WinGame()
    {
        winPanel.transform.SetAsLastSibling();
        winPanel.SetActive(true);
        PlayerPrefs.SetInt("2", PlayerPrefs.GetInt("2") + 1);
    }
}
