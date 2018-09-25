using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuScreenController : MonoBehaviour {
			
	// TODO: This should probably get moved to DisplayController class?
	public Animator logoAnimator;

	void Awake() { 
		logoAnimator.SetBool("IsOpen",true);
	//	Logger.Log("Open is now true");
	}

	void Start() {
		//logoAnimator.SetBool("IsOpen",false);
	//	Logger.Log("Open is now false");
	}

	public void LoadNextMenuScene() {
		SceneManager.LoadScene("MenuScreen2");
	}

	public void LoadHighScores() {
		SceneManager.LoadScene("HighScoresScene");
	}

	public void LoadSettings() {
		Logger.Log("This will eventually load Settings Scene");
	}

	public void PlayStatesGame() {
		GameController gameController = FindObjectOfType<GameController>();
		gameController.StartGame();
		SceneManager.LoadScene("StatesGameScene");
		}

	public void PlayCapitalsGame() {
		GameController gameController = FindObjectOfType<GameController>();
		gameController.StartGame();
		SceneManager.LoadScene("CapitalsGameScene");
	}
}
