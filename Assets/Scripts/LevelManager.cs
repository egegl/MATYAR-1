using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class LevelManager : MonoBehaviour
{
    private ItemSlot _firstSlot;
    private ItemSlot _secondSlot;
    private int _firstNum;
    private int _secondNum;
    private TMP_InputField _input;
    private Image _cbImage;
    private RectTransform _cbRectTransform;
    private GameObject _tickButton;
    private Image _tbImage;
    private RectTransform _tbRectTransform;
    
    [SerializeField] private TextMeshProUGUI firstNumText;
    [SerializeField] private TextMeshProUGUI secondNumText;
    [SerializeField] private GameObject checkButton;
    [SerializeField] private GameObject endGame;
    [SerializeField] private GameObject winPanel;
    
    public static LevelManager Instance;
    public Vector3 StartLocalPos { get; } = new(-185f, 40f, 0f);
    public Vector3 StartLocalScale { get; } = new(1.1f, 1.1f, 1.1f);

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

        _firstSlot = GameObject.FindWithTag("First").GetComponent<ItemSlot>();
        _secondSlot = GameObject.FindWithTag("Second").GetComponent<ItemSlot>();
        _cbRectTransform = checkButton.GetComponent<RectTransform>();
        _cbImage = checkButton.GetComponent<Image>();
        _input = endGame.transform.GetChild(0).GetComponent<TMP_InputField>();
        _tickButton = endGame.transform.GetChild(1).gameObject;
        _tbImage = _tickButton.GetComponent<Image>();
        _tbRectTransform = _tickButton.GetComponent<RectTransform>();

        RandomizeNumbers();
    }

    // reset level state
    public void ResetLevel()
    {
        // reset circles
        foreach (DragDrop circle in FindObjectsOfType<DragDrop>())
        {
            circle.ResetCircle();
        }

        // reset slots
        foreach (ItemSlot itemSlot in FindObjectsOfType<ItemSlot>())
        {
            itemSlot.NumCircles = 0;
        }

        // reset check button
        _cbImage.color = Color.white;

        // reset endgame
        endGame.SetActive(false);
        _input.text = "";
        _tbImage.color = Color.white;

        // reset win panel
        winPanel.SetActive(false);

        // randomize numbers
        RandomizeNumbers();

        // spawn circle if no circle is left
        if (GameObject.FindGameObjectWithTag("Circ") == null)
        {
            _firstSlot.SpawnCircle(StartLocalPos, StartLocalScale);
        }
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
        if (_firstSlot.NumCircles != _firstNum || _secondSlot.NumCircles != _secondNum)
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
            endGame.transform.SetAsLastSibling();
            endGame.SetActive(true);
            GameObject.FindGameObjectWithTag("Circ").SetActive(false);
            MoveCirclesToEndgame();
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

        // move circles in the first slot
        foreach (GameObject circle in GameObject.FindGameObjectsWithTag("CircFirst"))
        {
            i++;
            RectTransform cRectTransform = circle.GetComponent<RectTransform>();
            Image cImage = circle.GetComponent<Image>();
            cRectTransform.LeanMoveLocal(new Vector2(x, y), .5f);
            switch (x)
            {
                case 150:
                    i--;
                    break;
                case 330:
                    y -= 60;
                    x -= 240;
                    break;
            }
            if (i % 2 == 0)
            {
                StartCoroutine(ColorChange(cImage, Color.green, Color.red, .3f));
            }
            x += 60;
        }

        if (x > 330)
        {
            x -= 180;
            y -= 60;
        }

        i--;

        //move circles in the second slot
        foreach (GameObject circle in GameObject.FindGameObjectsWithTag("CircSecond"))
        {
            i++;
            RectTransform cRectTransform = circle.GetComponent<RectTransform>();
            Image cImage = circle.GetComponent<Image>();
            cRectTransform.LeanMoveLocal(new Vector2(x, y), .5f);
            switch (x)
            {
                case 150:
                    i--;
                    break;
                case 330:
                    y -= 60;
                    x -= 240;
                    break;
            }
            if (i % 2 == 0)
            {
                StartCoroutine(ColorChange(cImage, Color.red, Color.green, .3f));
            }
            x += 60;
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
