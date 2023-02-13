using UnityEngine;
using TMPro;

public class Level2Manager : MonoBehaviour
{
    private int i;
    [SerializeField] private TextMeshProUGUI analogToDigital;
    [SerializeField] private TextMeshProUGUI amPmText; 
    public static Level2Manager Instance { get; private set; }

    private void Awake()
    {
        // singleton
        Instance = this;
    }

    // reset level state
    public void ResetLevel()
    {
        Debug.Log("Reset Level 2");
    }

    // update the digital clock based on the analog input
    public void UpdateMins(int min)
    {
        analogToDigital.text = analogToDigital.text.Substring(0, 3) + FixClockNum(min);
    }

    public void UpdateHours(int hr)
    {
        // if the button is set to PM, add 12 to the hour
        if (amPmText.text.Equals("Öğleden Önce")) hr += 12;

        analogToDigital.text = FixClockNum(hr) + analogToDigital.text.Substring(2);
    }

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