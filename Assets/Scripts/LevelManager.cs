using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class LevelManager : MonoBehaviour
{
    
    private int _firstNum;
    private int _secondNum;
    private TMP_InputField _input;
    private Image _cbImage;
    private RectTransform _cbRectTransform;
    private GameObject _tickButton;
    private Image _tbImage;
    private RectTransform _tbRectTransform;
    private List<DragDrop> _circlesToHandle = new List<DragDrop>();

    [SerializeField] private ItemSlot firstSlot;
    [SerializeField] private ItemSlot secondSlot;
    [SerializeField] private TextMeshProUGUI firstNumText;
    [SerializeField] private TextMeshProUGUI secondNumText;
    [SerializeField] private GameObject checkButton;
    [SerializeField] private GameObject endgame;
    [SerializeField] private GameObject winPanel;
    
    public static LevelManager Instance;
    
    private void Awake()
    {
        // singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _cbRectTransform = checkButton.GetComponent<RectTransform>();
        _cbImage = checkButton.GetComponent<Image>();
        _input = endgame.transform.GetChild(0).GetComponent<TMP_InputField>();
        _tickButton = endgame.transform.GetChild(1).gameObject;
        _tbImage = _tickButton.GetComponent<Image>();
        _tbRectTransform = _tickButton.GetComponent<RectTransform>();

        RandomizeNumbers();
    }

    // reset level state
    public void ResetLevel()
    {
        // get circles to reset
        for (int j = 1; j < 3; j++)
        {
            foreach (GameObject circle in DragDrop.CircDict[j])
            {
                _circlesToHandle.Add(circle.GetComponent<DragDrop>());
            }
        }
        
        // reset circles
        foreach (DragDrop dragDrop in _circlesToHandle)
        {
            dragDrop.ResetCircle();
        }
        
        // reset circle list
        _circlesToHandle.Clear();

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

        // randomize numbers
        RandomizeNumbers();
    }

    // randomizes numbers   
    public void RandomizeNumbers()
    {
        // assign random integer value (min, max) to firstNum and secondNum - inclusive, exclusive
        _firstNum = Random.Range(1, 11);
        _secondNum = Random.Range(1, 11);

        // assign numbers to firstNumText and secondNumText
        firstNumText.text = _firstNum.ToString();
        secondNumText.text = _secondNum.ToString();
    }

    // checks if the number of circles in slots are correct
    public void CheckCircles()
    {
        if ((firstSlot.NumCircles + secondSlot.NumCircles) != (_firstNum + _secondNum))
        {
            // wrong answer visuals
            StartCoroutine(ColorChange(_cbImage, Color.white, Color.red, .25f, 0f));
            StartCoroutine(Shake(_cbRectTransform, 4, .5f));
        }
        else
        {
            // correct answer visuals
            StartCoroutine(ColorChange(_cbImage, Color.white, Color.green, .5f));

            // enable endgame
            endgame.transform.SetAsLastSibling();
            endgame.SetActive(true);
            MoveCirclesToEndgame();
            
            // hide the circle on spawn
            
        }
    }

    // check endgame answer
    public void CheckEndgame()
    {
        if (int.TryParse(_input.text, out int result) && result == (_firstNum + _secondNum))
        {
            // correct answer visuals
            StartCoroutine(ColorChange(_tbImage, Color.white, Color.green, .25f));

            // win game
            WinGame();
        }
        else
        {
            // wrong answer visuals
            StartCoroutine(ColorChange(_tbImage, Color.white, Color.red, .25f, 0f));
            StartCoroutine(Shake(_tbRectTransform, 4, .5f));
        }
    }
    
    // move all circles in slots to the final slot
    private void MoveCirclesToEndgame()
    {
        int i = 0;
        int x = 150;
        int y = 160;

        for (int j = 1; j < 3; j++)
        {
            foreach (GameObject circle in DragDrop.CircDict[j])
            {
                RectTransform cRectTransform = circle.GetComponent<RectTransform>();
                Image cImage = circle.GetComponent<Image>();

                cRectTransform.LeanMoveLocal(new Vector2(x, y), .5f);

                if (x == 150) i--;
                else if (x == 330)
                {
                    y -= 60;
                    x -= 240;
                }
                i++;
                
                if (i % 2 == 0) StartCoroutine(ColorChange(cImage, Color.green, Color.red, .3f));
                else StartCoroutine(ColorChange(cImage, Color.red, Color.green, .3f));
                
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
        StartCoroutine(AlphaChange(winPanel.GetComponent<CanvasGroup>(), 0f, 1f, 1f));
    }

    /**
    ALL FUNCTIONS BELOW WILL BE MOVED TO A GameManager SCRIPT!
    **/

    // shake UI element
    private IEnumerator Shake(RectTransform rectTransform, float shakeAmount, float duration)
    {
        float t = 0;
        
        Vector3 startLocalPos = rectTransform.localPosition;
        while (t < duration)
        {
            t += Time.deltaTime;
            rectTransform.localPosition = Random.insideUnitSphere * shakeAmount + startLocalPos;
            yield return null;
        }
        rectTransform.localPosition = startLocalPos;

        // unselect UI element
        EventSystem.current.SetSelectedGameObject(null);
    }

    // change color of UI element
    public IEnumerator ColorChange(Image image, Color from, Color to, float duration, float waitBetween)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            image.color = Color.Lerp(from, to, (t / duration));
            yield return null;
        }
        t = 0;
        yield return new WaitForSeconds(waitBetween);
        while (t < duration)
        {
            t += Time.deltaTime;
            image.color = Color.Lerp(to, from, (t / duration));
            yield return null;
        }
    }

    // override ColorChange if we don't want the color to change back
    public IEnumerator ColorChange(Image image, Color from, Color to, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            image.color = Color.Lerp(from, to, (t / duration));
            yield return null;
        }
    }

    // change alpha of UI element
    public IEnumerator AlphaChange(CanvasGroup canvasGroup, float from, float to, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
    }
}
