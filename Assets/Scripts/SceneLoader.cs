using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private GameObject _loaderCanvas;

    public static SceneLoader Instance;

    private void Awake()
    {
        // singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _loaderCanvas = transform.GetChild(0).gameObject;
    }

    // load scene given the index and duration of the animation
    public IEnumerator LoadScene(int sceneIndex, float duration)
    {
        //m_animator.SetTrigger("Start");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        
        yield return new WaitForSeconds(duration);
        
    }
}