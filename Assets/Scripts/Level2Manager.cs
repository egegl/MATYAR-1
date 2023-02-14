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

        // reset hour and minute
        Akrep.Instance.Hr = 0;
        Yelkovan.Instance.Min = 0;

        // reset digital clock
        analogToDigital.text = "00:00";

        // reset durum text accordingly
        UpdateDurum();       
    }

    // convert analog clock to digital
    public void AnalogToDigital()
    {
        // update minutes
        analogToDigital.text = analogToDigital.text.Substring(0, 3) + FixClockMin();

        // update hours
        if (amPmText.text.Equals("Öğleden Önce")) Akrep.Instance.Hr += 12;
        analogToDigital.text = FixClockHr() + analogToDigital.text.Substring(2);

        // update durum text
        UpdateDurum();
    }

    // update durum text
    private void UpdateDurum()
    {
        int hr = Akrep.Instance.Hr;

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
    private string FixClockMin()
    {
        int min = Yelkovan.Instance.Min;

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
    private string FixClockHr()
    {
        int hr = Akrep.Instance.Hr;

        string hrStr = hr.ToString();
        if (hr < 10)
        {
            hrStr = "0" + hrStr;
        }
        return hrStr;
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
        Akrep.Instance.Hr += numChange;
        UpdateHour(numChange);
        
        i++;
    }

    private void UpdateHour(int numChange)
    {
        // update durum text
        UpdateDurum();

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