using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * NOTES:
 * add fancy scene transition later (A4 paper transition?)
 * lines for it are commented out
 */

public class SceneLoader : MonoBehaviour
{
    //private Animator _animator;
    
    [SerializeField] private float transitionDuration;
    
    public static SceneLoader Instance;

    private void Awake()
    {
        // singleton
        Instance = this;

        //_animator = transform.GetChild(0).GetComponent<Animator>();
    }
    
    // wrapper function for the Load coroutine
    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(Load(sceneIndex));
    }
    
    // load scene given the index and duration of the animation
    private IEnumerator Load(int sceneIndex)
    {
        //m_animator.SetTrigger("Start");
        yield return new WaitForSeconds(transitionDuration);
        SceneManager.LoadScene(sceneIndex);
    }
}