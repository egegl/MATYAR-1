using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{
    private CanvasGroup _akrep;
    private CanvasGroup _yelkovan;
    
    public Clock Instance { get; private set; }

    public void Awake()
    {
        Instance = this;

        _akrep = transform.GetChild(0).gameObject.GetComponent<CanvasGroup>();
        _yelkovan = transform.GetChild(1).gameObject.GetComponent<CanvasGroup>();
    }

    public void Dragged(int index)
    {
        if (index == 0)
        {
            _akrep.alpha = .6f;
            _yelkovan.interactable = false;
        }
        else if (index == 1)
        {
            _yelkovan.alpha = .6f;
            _yelkovan.blocksRaycasts = false;
        }
    }
}
