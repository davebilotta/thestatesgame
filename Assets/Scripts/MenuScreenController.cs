using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuScreenController : MonoBehaviour {

    private DisplayController displayController;
    private GameController gc;

    public void Awake()
    {
        gc = FindObjectOfType<GameController>();
        displayController = gc.displayController;
        
    }

    public void LoadNextMenuScene() {
        displayController.LoadScene("MenuScreen2Landscape");
    }

	public void LoadHighScores() {
        //SceneManager.LoadScene("HighScoresScene");
        displayController.LoadScene("HighScoresScene");
	}

	public void LoadSettings() {
		Logger.Log("This will eventually load Settings Scene");
	}
	public void PlayStatesGame() {
		GameController gameController = FindObjectOfType<GameController>();
		gameController.StartGame();
        //SceneManager.LoadScene("StatesGameScene");
        //SceneManager.LoadScene("StatesGameSceneLandscape");
        displayController.LoadScene("StatesGameSceneLandscape");
    }

	public void PlayCapitalsGame() {
		GameController gameController = FindObjectOfType<GameController>();
		gameController.StartGame();
        //SceneManager.LoadScene("CapitalsGameScene");
        displayController.LoadScene("CapitalsGameScene");

    }
}
