using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverController : MonoBehaviour {

	public Text gameScoreText;
	public Text perfectGameBonusText;
	public Text totalScoreText;
    public GameObject highScoreDisplay;

private GameController gc;

	void Awake () {
		
		gc = FindObjectOfType<GameController>();

		gameScoreText.text = gc.totalRoundScore.ToString();
		if (gc.perfectGame) {
			perfectGameBonusText.text = gc.perfectGameBonus.ToString() + "";
		}
		else {
			perfectGameBonusText.text = "0";
		}
			
		totalScoreText.text = gc.playerScore.ToString();

        if (gc.newHighScore)
        {
            highScoreDisplay.active = true;
        }
	}

	void Update () {
	}

	public void ReturnToMenu() {
		SceneManager.LoadScene("MenuScreen");
	}

	public void LoadHighScores() {
		SceneManager.LoadScene("HighScoresScene");
	}
	public void LoadAchievements() {
		SceneManager.LoadScene("AchievementsScene");
	}
}
