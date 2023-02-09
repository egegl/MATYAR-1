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
    private RectTransform cbRectTransform;
    private Vector2 cbStartLocalScale;
    private TMP_InputField input;
    private GameObject tickButton;
    private Image tbImage;
    private RectTransform tbRectTransform;
    private Button cbButton;
    private Image cbImage;
    private GameObject confetti;

    public static LevelManager instance;
    public TextMeshProUGUI firstNumText;
    public TextMeshProUGUI secondNumText;
    public GameObject checkButton;
    public GameObject endGame;
    public GameObject winPanel;

    private void Awake()
    {
        // singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        firstSlot = GameObject.FindWithTag("First").GetComponent<ItemSlot>();
        secondSlot = GameObject.FindWithTag("Second").GetComponent<ItemSlot>();
        cbRectTransform = checkButton.GetComponent<RectTransform>();
        cbImage = checkButton.GetComponent<Image>();
        cbButton = checkButton.GetComponent<Button>();
        cbStartLocalScale = cbRectTransform.localScale;
        input = endGame.transform.GetChild(0).GetComponent<TMP_InputField>();
        tickButton = endGame.transform.GetChild(1).gameObject;
        confetti = endGame.transform.GetChild(2).gameObject;
        tbImage = tickButton.GetComponent<Image>();
        tbRectTransform = tickButton.GetComponent<RectTransform>();

        RandomizeNumbers();
    }

    // reset level state
    public void ResetLevel()
    {
        // reset circles
        foreach(DragDrop circle in FindObjectsOfType<DragDrop>())
        {
            circle.ResetCircle();
        }

        // reset slots
        foreach (ItemSlot slot in FindObjectsOfType<ItemSlot>())
        {
            slot.numCircles = 0;
        }

        // reset check button
        cbButton.interactable = true;
        cbImage.color = Color.white;
        //cbRectTransform.localScale = cbStartLocalScale;

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
        firstNum = (int)Random.Range(1, 11);
        secondNum = (int)Random.Range(1, 11);

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
            StartCoroutine(ColorChange(cbImage, Color.white, Color.red, .25f, 0, true));
            StartCoroutine(Shake(cbRectTransform, 4, .5f));
        }
        else
        {
            // correct answer visuals
            StartCoroutine(ColorChange(cbImage, Color.white, Color.green, .5f, 1, false));
            
            // disable check button
            cbButton.interactable = false;

            // remove check button
            //cbRectTransform.LeanScale(Vector3.zero, .5f).setEaseInOutQuart();

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
            if(x == 330)
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
                StartCoroutine(ColorChange(cImage, Color.green, Color.red, .3f, 0, false));
            }
            x += 60;
        }

        if (x == 330)
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
            else if(x == 150)
            {
                i--;
            }
            if (i % 2 == 0)
            {
                StartCoroutine(ColorChange(cImage, Color.red, Color.green, .3f, 0, false));
            }
            x += 60;
        }
    }

    // Check endgame answer
    public void CheckEndgame()
    {
        if(int.TryParse(input.text, out int result) && result == (firstNum + secondNum))
        {
            // correct answer visuals
            StartCoroutine(ColorChange(tbImage, Color.white, Color.green, .25f, 0, false));

            // win game
            WinGame();
        }
        else
        {
            // wrong answer visuals
            StartCoroutine(ColorChange(tbImage, Color.white, Color.red, .25f, 0, true));
            StartCoroutine(Shake(tbRectTransform, 4, .5f));
        }
    }

    private void WinGame()
    {
        winPanel.transform.SetAsLastSibling();
        winPanel.SetActive(true);
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
    public IEnumerator ColorChange(Image image, Color from, Color to, float duration, float waitBetween, bool again)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            image.color = Color.Lerp(from, to, (t / duration));
            yield return null;
        }
        if(again)
        {
            t = 0;
            yield return new WaitForSeconds(waitBetween);
            while (t < duration)
            {
                t += Time.deltaTime;
                image.color = Color.Lerp(to, from, (t / duration));
                yield return null;
            }
        }
    }
}
