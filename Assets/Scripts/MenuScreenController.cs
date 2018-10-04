using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuScreenController : MonoBehaviour {
			
	public void LoadNextMenuScene() {
        SceneManager.LoadScene("MenuScreen2Landscape");
        //SceneManager.LoadScene("MenuScreenWithTransitions2");
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
        //SceneManager.LoadScene("StatesGameScene");
        SceneManager.LoadScene("StatesGameSceneLandscape");
    }

	public void PlayCapitalsGame() {
		GameController gameController = FindObjectOfType<GameController>();
		gameController.StartGame();
		SceneManager.LoadScene("CapitalsGameScene");
	}
}
