using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Level1Manager : MonoBehaviour
{
    private int _firstNum;
    private int _secondNum;
    private TMP_InputField _input;
    private Image _cbImage;
    private RectTransform _cbRectTransform;
    private GameObject _tickButton;
    private Image _tbImage;
    private RectTransform _tbRectTransform;

    [SerializeField] private ItemSlot firstSlot;
    [SerializeField] private ItemSlot secondSlot;
    [SerializeField] private TextMeshProUGUI firstNumText;
    [SerializeField] private TextMeshProUGUI secondNumText;
    [SerializeField] private GameObject checkButton;
    [SerializeField] private GameObject endgame;
    [SerializeField] private GameObject winPanel;

    private static Level1Manager _instance;
    
    private void Awake()
    {
        // singleton
        _instance = this;

        _cbRectTransform = checkButton.GetComponent<RectTransform>();
        _cbImage = checkButton.GetComponent<Image>();
        _input = endgame.transform.GetChild(0).GetComponent<TMP_InputField>();
        _tickButton = endgame.transform.GetChild(1).gameObject;
        _tbImage = _tickButton.GetComponent<Image>();
        _tbRectTransform = _tickButton.GetComponent<RectTransform>();

        RandomizeNumbers();
    }
    
    // randomizes numbers   
    private void RandomizeNumbers()
    {
        // assign random integer value (min, max) to firstNum and secondNum - inclusive, exclusive
        _firstNum = Random.Range(1, 11);
        _secondNum = Random.Range(1, 11);

        // assign numbers to firstNumText and secondNumText
        firstNumText.text = _firstNum.ToString();
        secondNumText.text = _secondNum.ToString();
    }
    
    // reset level state
    public void ResetLevel(bool menu)
    {
        if (menu)
        {
            SceneLoader.Instance.LoadScene(0);
            return;
        }

        // reset all circles in the scene
        foreach (DragDrop dragDrop in FindObjectsOfType<DragDrop>())
        {
            dragDrop.ResetCircle();
        }

        // reset slots
        firstSlot.NumCircles = 0;
        secondSlot.NumCircles = 0;

        // reset check button
        _cbImage.color = Color.white;

        // reset endgame
        endgame.SetActive(false);
        _input.text = "";
        _tbImage.color = Color.white;

        // reset win panel
        winPanel.SetActive(false);
        
        // show spawn circle
        DragDrop.SpawnCircle.GetComponent<CanvasGroup>().alpha = 1;
        
        // randomize numbers
        RandomizeNumbers();
    }

    // checks if the number of circles in slots are correct
    public void CheckCircles()
    {
        if ((firstSlot.NumCircles + secondSlot.NumCircles) != (_firstNum + _secondNum))
        {
            // wrong answer visuals
            StartCoroutine(GameManager.Instance.ColorChange(_cbImage, Color.white, Color.red, .25f, 0f));
            StartCoroutine(GameManager.Instance.Shake(_cbRectTransform, 4, .5f));
        }
        else
        {
            // correct answer visuals
            StartCoroutine(GameManager.Instance.ColorChange(_cbImage, Color.white, Color.green, .5f));

            // enable endgame
            endgame.transform.SetAsLastSibling();
            endgame.SetActive(true);
            MoveCirclesToEndgame();
            
            // hide spawn circle
            DragDrop.SpawnCircle.GetComponent<CanvasGroup>().LeanAlpha(0f, .2f);
        }
    }

    // check endgame answer
    public void CheckEndgame()
    {
        if (int.TryParse(_input.text, out int result) && result == (_firstNum + _secondNum))
        {
            // correct answer visuals
            StartCoroutine(GameManager.Instance.ColorChange(_tbImage, Color.white, Color.green, .25f));

            // win game
            WinGame();
        }
        else
        {
            // wrong answer visuals
            StartCoroutine(GameManager.Instance.ColorChange(_tbImage, Color.white, Color.red, .25f, 0f));
            StartCoroutine(GameManager.Instance.Shake(_tbRectTransform, 4, .5f));
        }
    }
    
    // move all circles in slots to the final slot
    private void MoveCirclesToEndgame()
    {
        int x = 150;
        int y = 160;

        for (int j = 1; j < 3; j++)
        {
            foreach (GameObject circle in DragDrop.CircDict[j])
            {
                RectTransform cRectTransform = circle.GetComponent<RectTransform>();
                Image cImage = circle.GetComponent<Image>();

                cRectTransform.LeanMoveLocal(new Vector2(x, y), .5f);
                
                if (x == 330)
                {
                    y -= 60;
                    x -= 240;
                }
                x += 60;
            }
            if (x == 390)
            {
                x -= 180;
                y -= 60;
            }
        }
    }

    // activate win panel
    private void WinGame()
    {
        winPanel.transform.SetAsLastSibling();
        winPanel.SetActive(true);
        winPanel.GetComponent<CanvasGroup>().LeanAlpha(1f, 1f);
        
        GameManager.Instance.LevelWon();
    }
}
