using System.Collections;
using TMPro;
using UnityEngine;

public class StarPanel : MonoBehaviour
{
    private int _totalStars;
    private int _numPages;
    private int _starsLastPage;
    private int _thisPage;
    private int _currStars;
    [SerializeField] private GameObject nextPageButton;
    [SerializeField] private GameObject prevPageButton;
    [SerializeField] private Transform allStars;
    [SerializeField] private TextMeshProUGUI starsText;

    public static StarPanel Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        for (int i = 0; i < 30; i++)
        {
            allStars.GetChild(i).localScale = Vector3.zero;
        }

        _totalStars = PlayerPrefs.GetInt("CurrStars");
        _starsLastPage = _totalStars;

        // reset this page and #pages
        _thisPage = 0;
        _numPages = 0;

        // calculate #pages
        while (_starsLastPage > 30)
        {
            _starsLastPage -= 30;
            _numPages++;
        }

        // determine current stars
        if (_numPages > 0) _currStars = 30;
        else _currStars = _starsLastPage;

        // show stars
        ShowStars();
    }

    public void ShowStars()
    {
        // hide buttons
        nextPageButton.SetActive(false);
        prevPageButton.SetActive(false);

        // update stars text
        UpdateText();

        // show stars one by one, then manage button states
        StartCoroutine(Appear());
    }

    private void UpdateText()
    {
        int start = 30 * _thisPage;
        starsText.text = "Gösterilen Yıldızlar: " + start + "-" + (start + _currStars);
    }

    // show/hide next and previous page buttons
    private void ShowHideButtons()
    {
        if (_numPages > 0 && _numPages > _thisPage)
        {
            nextPageButton.SetActive(true);
        }
        else nextPageButton.SetActive(false);
        if (_thisPage > 0)
        {
            prevPageButton.SetActive(true);
        }
        else prevPageButton.SetActive(false);
    }

    private IEnumerator Appear()
    {
        for (int i = 0; i < _currStars; i++)
        {
            allStars.GetChild(i).LeanScale(Vector3.one, .2f).setEaseOutBack();
            yield return new WaitForSeconds(.1f);
        }
        ShowHideButtons();
    }

    public void ButtonNextPage()
    {
        HideCurrStars();

        // update current stars
        int starsLeft = _totalStars - (_thisPage * 30 + _currStars);
        if (starsLeft > 30) _currStars = 30;
        else _currStars = starsLeft;

        // increment this page
        _thisPage++;

        // load next page
        ShowStars();
    
        ButtonHandler.Instance.AfterPress();
    }

    public void ButtonPrevPage()
    {
        HideCurrStars();

        // update current stars
        _currStars = 30;

        // decrement this page
        _thisPage--;      

        // load previous page
        ShowStars();

        ButtonHandler.Instance.AfterPress();
    }

    public void ResetPanel()
    {
        HideCurrStars();

        // deactivate self
        gameObject.SetActive(false);
    }

    private void HideCurrStars()
    {
        StopCoroutine(Appear());
        for (int i = 0; i < _currStars; i++)
        {
            allStars.GetChild(i).localScale = Vector3.zero;
        }
    }
}
