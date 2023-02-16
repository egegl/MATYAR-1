using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject star0;
    [SerializeField] private GameObject star1;
    [SerializeField] private GameObject star2;
    [SerializeField] private GameObject star3;
    [SerializeField] private GameObject starPanel;
    private GameObject[] stars = new GameObject[4];

    public static MenuManager Instance { get; private set; }

    private void Awake()
    {
        // singleton
        Instance = this;

        stars = new[] {star0, star1, star2, star3};

        ScoresToStars();
    }

    private void ScoresToStars()
    {
        for (int i = 0; i < 4; i++)
        {
            int gameScore = PlayerPrefs.GetInt(i.ToString());
            if (gameScore > 0) stars[i].SetActive(true);
            else stars[i].SetActive(false);
            stars[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = gameScore.ToString();
        }
    }

    public void ButtonStarPanel(string game)
    {
        PlayerPrefs.SetInt("CurrStars", PlayerPrefs.GetInt(game));
        starPanel.SetActive(true);
    }

    public void ButtonResetPrefs()
    {
        PlayerPrefs.DeleteAll();
        ScoresToStars();
    }

    public void ButtonSetPrefs(int set)
    {
        PlayerPrefs.SetInt("2", set);
        ScoresToStars();
    }
}
