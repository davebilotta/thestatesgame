using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitions : MonoBehaviour {
    public Animator animator;

    private string sceneToLoad;

    void Update()
    {
       // Debug.Log("SceneTransitions Update");
    }

    public void LoadScene(string sceneName)
    {
        //sceneToLoad = sceneName;
        //animator.SetTrigger("FadeOut");

        SceneManager.LoadScene(sceneName);
    }
   
    public void OnFadeComplete()
    {
        //animator.enabled = false;
        SceneManager.LoadScene(sceneToLoad);
    }
}
