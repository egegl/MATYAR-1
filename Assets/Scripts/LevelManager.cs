using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class LevelManager : MonoBehaviour
{
    private ItemSlot firstSlot;
    private ItemSlot secondSlot;
    private int firstNum;
    private int secondNum;
    private TMP_InputField input;
    private Image cbImage;
    private RectTransform cbRectTransform;
    private GameObject tickButton;
    private Image tbImage;
    private RectTransform tbRectTransform;

    public static LevelManager Instance;
    public Vector3 StartLocalPos { get; private set; } = new(-185f, 40f, 0f);
    public Vector3 StartLocalScale { get; private set; } = new(1.1f, 1.1f, 1.1f);

    public TextMeshProUGUI firstNumText;
    public TextMeshProUGUI secondNumText;
    public GameObject checkButton;
    public GameObject endGame;
    public GameObject winPanel;
    

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

        firstSlot = GameObject.FindWithTag("First").GetComponent<ItemSlot>();
        secondSlot = GameObject.FindWithTag("Second").GetComponent<ItemSlot>();
        cbRectTransform = checkButton.GetComponent<RectTransform>();
        cbImage = checkButton.GetComponent<Image>();
        input = endGame.transform.GetChild(0).GetComponent<TMP_InputField>();
        tickButton = endGame.transform.GetChild(1).gameObject;
        tbImage = tickButton.GetComponent<Image>();
        tbRectTransform = tickButton.GetComponent<RectTransform>();

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
        foreach (ItemSlot slot in FindObjectsOfType<ItemSlot>())
        {
            slot.numCircles = 0;
        }

        // reset check button
        cbImage.color = Color.white;

        // reset endgame
        endGame.SetActive(false);
        input.text = "";
        tbImage.color = Color.white;

        // reset win panel
        winPanel.SetActive(false);

        RandomizeNumbers();
    }

    // randomizes numbers   
    public void RandomizeNumbers()
    {
        // assign random integer value (min, max) to firstNum and secondNum - inclusive, exclusive
        firstNum = Random.Range(1, 11);
        secondNum = Random.Range(1, 11);

        // assign numbers to firstNumText and secondNumText
        firstNumText.text = firstNum.ToString();
        secondNumText.text = secondNum.ToString();
    }

    // checks if the number of circles in slots are correct
    public void CheckCircles()
    {
        if (firstSlot.numCircles != firstNum || secondSlot.numCircles != secondNum)
        {
            // wrong answer visuals
            StartCoroutine(ColorChange(cbImage, Color.white, Color.red, .25f, 0f));
            StartCoroutine(Shake(cbRectTransform, 4, .5f));
        }
        else
        {
            // correct answer visuals
            StartCoroutine(ColorChange(cbImage, Color.white, Color.green, .5f));

            // enable endgame
            endGame.transform.SetAsLastSibling();
            endGame.SetActive(true);
            GameObject.FindGameObjectWithTag("Circ").SetActive(false);
            MoveCirclesToEndgame();
        }
    }

    // move all circles in slots to the final slot
    public void MoveCirclesToEndgame()
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
            if (x == 330)
            {
                y -= 60;
                x -= 240;
            }
            else if (x == 150)
            {
                i--;
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
            if (x == 330)
            {
                y -= 60;
                x -= 240;
            }
            else if (x == 150)
            {
                i--;
            }
            if (i % 2 == 0)
            {
                StartCoroutine(ColorChange(cImage, Color.red, Color.green, .3f));
            }
            x += 60;
        }
    }

    // Check endgame answer
    public void CheckEndgame()
    {
        if (int.TryParse(input.text, out int result) && result == (firstNum + secondNum))
        {
            // correct answer visuals
            StartCoroutine(ColorChange(tbImage, Color.white, Color.green, .25f));

            // win game
            WinGame();
        }
        else
        {
            // wrong answer visuals
            StartCoroutine(ColorChange(tbImage, Color.white, Color.red, .25f, 0f));
            StartCoroutine(Shake(tbRectTransform, 4, .5f));
        }
    }

    private void WinGame()
    {
        winPanel.transform.SetAsLastSibling();
        winPanel.SetActive(true);
        StartCoroutine(AlphaChange(winPanel.GetComponent<CanvasGroup>(), 0f, 1f, 1f));
    }

    // shakes UI element
    private IEnumerator Shake(RectTransform rectTransform, float shakeAmount, float duration)
    {
        float t = 0;
        Vector3 initialPos = rectTransform.localPosition;
        while (t < duration)
        {
            t += Time.deltaTime;
            rectTransform.localPosition = Random.insideUnitSphere * shakeAmount + initialPos;
            yield return null;
        }
        rectTransform.localPosition = initialPos;

        // unselect button
        EventSystem.current.SetSelectedGameObject(null);
    }

    // color change of UI element
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

    // alpha change of UI element
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
