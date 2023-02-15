using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;

public class Level2Manager : MonoBehaviour
{
    private int _i;
    private int _givenHr;
    private int _givenMin;
    private int _winTries;
    private int _atdTries;
    private Image _wbImage;
    private RectTransform _wbRectTransform;

    [SerializeField] private TextMeshProUGUI givenDigitalText;
    [SerializeField] private TextMeshProUGUI givenDurumText;
    [SerializeField] private TextMeshProUGUI analogToDigital;
    [SerializeField] private TextMeshProUGUI amPmText;
    [SerializeField] private TextMeshProUGUI durumText;
    [SerializeField] private CanvasGroup endgame;
    [SerializeField] private Transform hearts;
    [SerializeField] private GameObject winButton;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject lossPanel;

    public static Level2Manager Instance { get; private set; }

    private void Awake()
    {
        // singleton
        Instance = this;

        _wbImage = winButton.GetComponent<Image>();
        _wbRectTransform = winButton.GetComponent<RectTransform>();

        ResetLevel();
    }

    // reset level state
    public void ResetLevel()
    {
        // reset analog clock
        Yelkovan.Instance.Reset();
        Akrep.Instance.Reset();

        // hide endgame
        SHEndgame(0);

        // randomize given time
        RandomizeTime();

        // reset atd tries and win tries
        _atdTries = 0;
        _winTries = 0;

        // reset win button
        _wbImage.color = Color.white;

        // reset win panel
        winPanel.SetActive(false);
    }

    private void RandomizeTime()
    {
        // assign random values to given hours and minute
        _givenHr = Random.Range(0, 24);
        _givenMin = Random.Range(0, 60);

        // change the text of given digital clock
        givenDigitalText.text = FixClockHr(_givenHr) + ":" + FixClockMin(_givenMin);

        // update given durum text
        UpdateDurum(_givenHr, givenDurumText);
    }

    // convert analog clock to digital
    public void AnalogToDigital()
    {
        // update minutes
        analogToDigital.text = analogToDigital.text.Substring(0, 3) + FixClockMin(Yelkovan.Instance.Min);

        // update hours
        if (amPmText.text.Equals("Öğleden Önce")) Akrep.Instance.Hr += 12;
        analogToDigital.text = FixClockHr(Akrep.Instance.Hr) + analogToDigital.text.Substring(2);

        // update durum text
        UpdateDurum(Akrep.Instance.Hr, durumText);
    }

    private void ResetHearts()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject heart = hearts.GetChild(i).gameObject;
            heart.LeanScale(Vector3.one, .3f).setEaseOutBack();
        }
    }

    public void ButtonAnalogToDigital()
    {
        if (_atdTries == 0) ResetHearts();

        // analog to digital
        AnalogToDigital();

        // show endgame
        SHEndgame(1);

        // increment atd tries
        _atdTries++;

        ButtonHandler.Instance.AfterPress();
    }

    public void ButtonCheckWin()
    {
        // check win
        if(givenDigitalText.text.Equals(analogToDigital.text))
        {
            // correct answer visuals
            StartCoroutine(GameManager.Instance.ColorChange(_wbImage, Color.white, Color.green, .25f));

            // win game
            WinGame();
        }
        else
        {
            // wrong answer visuals
            StartCoroutine(GameManager.Instance.ColorChange(_wbImage, Color.white, Color.red, .25f, 0f));
            StartCoroutine(GameManager.Instance.Shake(_wbRectTransform, 4, .5f));

            // get heart object and destroy it
            GameObject heart = hearts.GetChild(_winTries).gameObject;
            heart.LeanScale(Vector3.zero, .4f).setEaseOutBack();

            // increment win tries
            _winTries++;

            // lose level if there are no hearts left
            if (_winTries == 3) LoseGame();
        }
        ButtonHandler.Instance.AfterPress();
    }

    // activate win panel
    private void WinGame()
    {
        winPanel.transform.SetAsLastSibling();
        winPanel.SetActive(true);

        GameManager.Instance.LevelWon();
    }

    private void LoseGame()
    {
        lossPanel.transform.SetAsLastSibling();
        lossPanel.SetActive(true);
    }

    private void SHEndgame(int enable)
    {
        bool interactable = false;
        if (enable == 1) interactable = true;

        // make endgame un/interactable
        endgame.interactable = interactable;

        // show/hide endgame animation
        endgame.LeanAlpha(enable, .4f).setEaseOutQuad();
    }

    // update durum text
    private void UpdateDurum(int hr, TextMeshProUGUI durumText)
    {
        if (hr < 6) durumText.text = "Gece"; // 0, 1, 2, 3, 4, 5
        else if (hr < 7) durumText.text = "Gün Doğumu"; // 6
        else if (hr < 11) durumText.text = "Sabah"; // 7, 8, 9, 10
        else if (hr < 13) durumText.text = "Öğlen"; // 11, 12
        else if (hr < 17) durumText.text = "Öğleden Sonra"; // 13, 14, 15, 16
        else if (hr < 19) durumText.text = "Akşamüstü"; // 17, 18
        else if (hr < 20) durumText.text = "Gün Batımı"; // 19
        else if (hr < 22) durumText.text = "Akşam"; // 20, 21
        else durumText.text = "Gece"; // 22, 23
    }

    // convert 0-9 numbers to 00-09 and 60-inf to 00-60
    private string FixClockMin(int min)
    {
        string minStr = min.ToString();
        if (min > 69)
        {
            minStr = (min - 60).ToString();
        }
        else if (min > 59)
        {
            minStr = "0" + (min - 60).ToString();
        }
        else if (min < 10)
        {
            minStr = "0" + minStr;
        }
        return minStr;
    }

    // convert 0-9 numbers to 00-09
    private string FixClockHr(int hr)
    {
        string hrStr = hr.ToString();
        if (hr < 10)
        {
            hrStr = "0" + hrStr;
        }
        return hrStr;
    }

    // AM/PM selection of the 12h analog clock
    public void ButtonAmPm()
    {
        int numChange = 12;
        if (_i % 2 != 0)
        {
            numChange *= -1;
            amPmText.text = "Öğleden Sonra";
        }
        else amPmText.text = "Öğleden Önce";
        Akrep.Instance.Hr += numChange;
        UpdateHour(numChange);
        
        _i++;

        ButtonHandler.Instance.AfterPress();
    }

    private void UpdateHour(int numChange)
    {
        // update durum text
        UpdateDurum(Akrep.Instance.Hr, durumText);

        int firstNum = int.Parse(analogToDigital.text.Substring(0, 2)) + numChange;

        string FirstText = FixClockString(firstNum);
        if (FirstText.Length < 2) FirstText = "0" + FirstText;

        analogToDigital.text = FirstText + analogToDigital.text.Substring(2);
    }

    private string FixClockString(int num)
    {
        string str = num.ToString();
        if (num < 0) str = (num + 12).ToString();
        else if (num < 10) str = "0" + str;
        else if (num > 60) str = (num - 60).ToString();
        return str;
    }
}