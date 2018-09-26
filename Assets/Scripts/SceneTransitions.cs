using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitions : MonoBehaviour {
    public Animator animator;

    private string sceneToLoad;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LoadScene("MenuScreenWithTransitions2");
        }
    }

    public void LoadScene(string sceneName)
    {
        sceneToLoad = sceneName;
        animator.SetTrigger("FadeOut");
    }
   
    public void OnFadeComplete()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
