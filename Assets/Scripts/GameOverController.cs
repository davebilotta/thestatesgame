using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverController : MonoBehaviour {

	public Text gameScoreText;
	public Text perfectGameBonusText;
	public Text totalScoreText;
    public GameObject highScoreDisplay;

    // These are the Int equivalents for counting up 
    private double gameScoreNum = 0;
    private int gameScoreNumMax;
    private double gameScoreNumInc;
    private double perfectGameBonusNum = 0;
    private int perfectGameBonusNumMax;
    private double perfectGameBonusNumInc;
    private double totalScoreNum = 0;
    private int totalScoreNumMax;
    private double totalScoreNumInc;

    private bool scoreCountUpComplete;            // Flag that when we're done with the round
    private float scoreCountUpTick = 0f;          // Where we're currently at in the counter
    private float scoreCountUpLength = 1.5f;      // How long will it take to count up our score?

    private int framesRequired;
    private int frameCount = 0;

    private GameController gc;

	void Awake () {
		
		gc = FindObjectOfType<GameController>();
        scoreCountUpComplete = false;

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
            highScoreDisplay.SetActive(true);
        }

        framesRequired = 90;

        // Game Score
        gameScoreNumMax = gc.totalRoundScore;
        gameScoreNumInc = ((double)gameScoreNumMax / framesRequired);

        // Perfect Game
        if (gc.perfectRound)
        {
            perfectGameBonusNumMax = gc.perfectGameBonus;
            perfectGameBonusNumInc = ((double)perfectGameBonusNumMax / framesRequired);
        }
        else
        {
            perfectGameBonusNumMax = 0;
            perfectGameBonusNumInc = 0;
        }

        // Total Score 
        totalScoreNumMax = gc.playerScore;
        totalScoreNumInc = ((double)totalScoreNumMax / framesRequired);
    }

	void Update () {
        if (!scoreCountUpComplete)
        {
            //if (scoreCountUpTick < scoreCountUpLength)
            if (frameCount < framesRequired)
            {
                scoreCountUpTick += Time.deltaTime;
                frameCount++;

                // Increment the numbers
                gameScoreNum += gameScoreNumInc;
                perfectGameBonusNum += perfectGameBonusNumInc;
                totalScoreNum += totalScoreNumInc;

                // Update the text 
                gameScoreText.text = ((int)gameScoreNum).ToString();
                perfectGameBonusText.text = ((int)perfectGameBonusNum).ToString();
                totalScoreText.text = ((int)totalScoreNum).ToString();
            }
            else
            {
                scoreCountUpComplete = true;

                // Set these to their final values
                gameScoreText.text = gameScoreNumMax.ToString();
                perfectGameBonusText.text = perfectGameBonusNumMax.ToString();
                totalScoreText.text = totalScoreNumMax.ToString();
            }
        }
    }

	public void ReturnToMenu() {
        
		SceneManager.LoadScene("MenuScreen2Landscape");
	}

	public void LoadHighScores() {
		SceneManager.LoadScene("HighScoresScene");
	}
	public void LoadAchievements() {
		SceneManager.LoadScene("AchievementsScene");
	}
}
