using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class LevelManager : MonoBehaviour
{
    private ItemSlot m_firstSlot;
    private ItemSlot m_secondSlot;
    private int m_firstNum;
    private int m_secondNum;
    private TMP_InputField m_input;
    private Image m_cbImage;
    private RectTransform m_cbRectTransform;
    private GameObject m_tickButton;
    private Image m_tbImage;
    private RectTransform m_tbRectTransform;
    
    [SerializeField] private TextMeshProUGUI m_firstNumText;
    [SerializeField] private TextMeshProUGUI m_secondNumText;
    [SerializeField] private GameObject m_checkButton;
    [SerializeField] private GameObject m_endGame;
    [SerializeField] private GameObject m_winPanel;
    
    public static LevelManager Instance;
    public Vector3 StartLocalPos { get; private set; } = new(-185f, 40f, 0f);
    public Vector3 StartLocalScale { get; private set; } = new(1.1f, 1.1f, 1.1f);

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

        m_firstSlot = GameObject.FindWithTag("First").GetComponent<ItemSlot>();
        m_secondSlot = GameObject.FindWithTag("Second").GetComponent<ItemSlot>();
        m_cbRectTransform = m_checkButton.GetComponent<RectTransform>();
        m_cbImage = m_checkButton.GetComponent<Image>();
        m_input = m_endGame.transform.GetChild(0).GetComponent<TMP_InputField>();
        m_tickButton = m_endGame.transform.GetChild(1).gameObject;
        m_tbImage = m_tickButton.GetComponent<Image>();
        m_tbRectTransform = m_tickButton.GetComponent<RectTransform>();

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
        m_cbImage.color = Color.white;

        // reset endgame
        m_endGame.SetActive(false);
        m_input.text = "";
        m_tbImage.color = Color.white;

        // reset win panel
        m_winPanel.SetActive(false);

        // randomize numbers
        RandomizeNumbers();

        // spawn circle if no circle is left
        if (GameObject.FindGameObjectWithTag("Circ") == null)
        {
            m_firstSlot.SpawnCircle(StartLocalPos, StartLocalScale);
        }
    }

    // randomizes numbers   
    public void RandomizeNumbers()
    {
        // assign random integer value (min, max) to firstNum and secondNum - inclusive, exclusive
        m_firstNum = Random.Range(1, 11);
        m_secondNum = Random.Range(1, 11);

        // assign numbers to firstNumText and secondNumText
        m_firstNumText.text = m_firstNum.ToString();
        m_secondNumText.text = m_secondNum.ToString();
    }

    // checks if the number of circles in slots are correct
    public void CheckCircles()
    {
        if (m_firstSlot.NumCircles != m_firstNum || m_secondSlot.NumCircles != m_secondNum)
        {
            // wrong answer visuals
            StartCoroutine(ColorChange(m_cbImage, Color.white, Color.red, .25f, 0f));
            StartCoroutine(Shake(m_cbRectTransform, 4, .5f));
        }
        else
        {
            // correct answer visuals
            StartCoroutine(ColorChange(m_cbImage, Color.white, Color.green, .5f));

            // enable endgame
            m_endGame.transform.SetAsLastSibling();
            m_endGame.SetActive(true);
            GameObject.FindGameObjectWithTag("Circ").SetActive(false);
            MoveCirclesToEndgame();
        }
    }

    // check endgame answer
    public void CheckEndgame()
    {
        if (int.TryParse(m_input.text, out int result) && result == (m_firstNum + m_secondNum))
        {
            // correct answer visuals
            StartCoroutine(ColorChange(m_tbImage, Color.white, Color.green, .25f));

            // win game
            WinGame();
        }
        else
        {
            // wrong answer visuals
            StartCoroutine(ColorChange(m_tbImage, Color.white, Color.red, .25f, 0f));
            StartCoroutine(Shake(m_tbRectTransform, 4, .5f));
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
            if (x == 150)
            {
                i--;
            }
            else if (x == 330)
            {
                y -= 60;
                x -= 240;
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
            RectTransform m_cRectTransform = circle.GetComponent<RectTransform>();
            Image m_cImage = circle.GetComponent<Image>();
            m_cRectTransform.LeanMoveLocal(new Vector2(x, y), .5f);
            if (x == 150)
            {
                i--;
            }
            else if (x == 330)
            {
                y -= 60;
                x -= 240;
            }
            if (i % 2 == 0)
            {
                StartCoroutine(ColorChange(m_cImage, Color.red, Color.green, .3f));
            }
            x += 60;
        }
    }

    // activate win panel
    private void WinGame()
    {
        m_winPanel.transform.SetAsLastSibling();
        m_winPanel.SetActive(true);
        StartCoroutine(AlphaChange(m_winPanel.GetComponent<CanvasGroup>(), 0f, 1f, 1f));
    }

    // shake UI element
    private IEnumerator Shake(RectTransform rectTransform, float shakeAmount, float duration)
    {
        float t = 0;
        Vector3 m_StartLocalPos = rectTransform.localPosition;
        while (t < duration)
        {
            t += Time.deltaTime;
            rectTransform.localPosition = Random.insideUnitSphere * shakeAmount + m_StartLocalPos;
            yield return null;
        }
        rectTransform.localPosition = m_StartLocalPos;

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
