using UnityEngine;
using TMPro;

public class Level2Manager : MonoBehaviour
{
    private int i;
    [SerializeField] private TextMeshProUGUI analogToDigital;
    [SerializeField] private TextMeshProUGUI amPmText;
    [SerializeField] private TextMeshProUGUI durumText;

    public static Level2Manager Instance { get; private set; }

    private void Awake()
    {
        // singleton
        Instance = this;
    }

    // reset level state
    public void ResetLevel()
    {
        // reset analog clock
        Yelkovan.Instance.Reset();
        Akrep.Instance.Reset();

        // reset digital clock
        analogToDigital.text = "00:00";
    }

    // convert analog clock to digital
    public void AnalogToDigital()
    {
        // get minutes and hours
        int min = Yelkovan.Instance.Min;
        int hr = Akrep.Instance.Hr;

        // update minutes
        analogToDigital.text = analogToDigital.text.Substring(0, 3) + FixClockNum(min);

        // update hours
        if (amPmText.text.Equals("Öğleden Önce")) hr += 12;
        analogToDigital.text = FixClockNum(hr) + analogToDigital.text.Substring(2);

        // update durum text
        UpdateDurum(hr);
    }

    // update durum text
    private void UpdateDurum(int hr)
    {
        if (hr < 5) durumText.text = "Gece";
        else if (hr < 12) durumText.text = "Sabah";
        else if (hr < 18) durumText.text = "Öğle";
        else durumText.text = "Akşam";
    }

    // convert 0-9 numbers to 00-09
    private string FixClockNum(int num)
    {
        string numStr = num.ToString();
        if (num < 10)
        {
            numStr = "0" + numStr;
        }
        return numStr;
    }

    // AM/PM selection of the 12h analog clock
    public void AmPm()
    {
        int numChange = 12;
        if (i % 2 != 0)
        {
            numChange *= -1;
            amPmText.text = "Öğleden Sonra";
        }
        else amPmText.text = "Öğleden Önce";

        UpdateHour(numChange);
        
        i++;
    }

    private void UpdateHour(int numChange)
    {
        // update durum text
        UpdateDurum(numChange);

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