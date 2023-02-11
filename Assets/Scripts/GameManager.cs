using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

/**
 * NOTES:
 * add fancy scene transition later (A4 paper transition?)
 * lines for it are commented out
 */

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {   
        // singleton
        Instance = this;
    }
    
    // What happens when the player wins? (add a reward system later)
    public void LevelWon()
    {
        Debug.Log("Level won!");
    }

    // shake UI element
    public IEnumerator Shake(RectTransform rectTransform, float shakeAmount, float duration)
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
