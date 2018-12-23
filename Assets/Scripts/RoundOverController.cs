using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class RoundOverController : MonoBehaviour {

    // These are the Text components 
    public Text roundStartScore;
    public Text roundScore;
	public Text secondsRemainingBonusText;
	public Text perfectRoundBonusText;
	public Text totalScoreText;

    // These are the Int equivalents for counting up 
    private double roundStartScoreNum = 0;
    private int roundStartScoreNumMax;
    private double roundStartScoreNumInc;
    private double roundScoreNum = 0;
    private int roundScoreNumMax;
    private double roundScoreNumInc;
    private double secondsRemainingBonusNum = 0;
    private int secondsRemainingBonusNumMax;
    private double secondsRemainingBonusNumInc;
    private double perfectRoundBonusNum = 0;
    private int perfectRoundBonusNumMax;
    private double perfectRoundBonusNumInc;
    private double totalScoreNum = 0;
    private int totalScoreNumMax;
    private double totalScoreNumInc;

	public GameObject mainDisplay;
	public GameObject exitConfirmation;

    public Text roundOverText;
    public Button endGameButton;
    public Button nextRoundButton;

	private bool exitConfirmationDialogDisplayed;
    private bool scoreCountUpComplete;            // Flag that when we're done with the round
    private float scoreCountUpTick = 0f;          // Where we're currently at in the counter
    private float scoreCountUpLength = 1.5f;      // How long will it take to count up our score?

    private int framesRequired;
    private int frameCount = 0;

private GameController gc;

    // Use this for initialization
    void Awake()
    {
        gc = FindObjectOfType<GameController>();
        scoreCountUpComplete = false;

        framesRequired = 60;
        
        // Round Start Score 
        roundStartScoreNumMax = gc.roundStartScore;
        roundStartScoreNumInc = ((double)roundStartScoreNumMax / framesRequired);

        // Round Score 
        roundScoreNumMax = gc.roundScore;
        roundScoreNumInc = ((double)roundScoreNumMax / framesRequired);

        // Seconds Remaining
        secondsRemainingBonusNumMax = gc.secondsRemainingBonus;
        secondsRemainingBonusNumInc = ((double)secondsRemainingBonusNumMax / framesRequired);

        // Perfect Round
        if (gc.perfectRound)
        {
            perfectRoundBonusNumMax = gc.perfectRoundBonus;
            perfectRoundBonusNumInc = ((double)perfectRoundBonusNumMax / framesRequired);

        }
        else
        {
            perfectRoundBonusNumMax = 0;
            perfectRoundBonusNumInc = 0;

        }

        // Total Score 
        totalScoreNumMax = gc.playerScore;
        totalScoreNumInc = ((double)totalScoreNumMax / framesRequired);

    }

	// Update is called once per frame
	void Update () {
        if (!scoreCountUpComplete)
        {
            if (frameCount < framesRequired) 
            {
                scoreCountUpTick += Time.deltaTime;
                frameCount++;

                // Increment the numbers
                roundStartScoreNum += roundStartScoreNumInc;
                roundScoreNum += roundScoreNumInc;
                secondsRemainingBonusNum += secondsRemainingBonusNumInc;
                perfectRoundBonusNum += perfectRoundBonusNumInc;
                totalScoreNum += totalScoreNumInc;

                // Update the text 
                roundStartScore.text = ((int)roundStartScoreNum).ToString();
                roundScore.text = ((int)roundScoreNum).ToString();
                secondsRemainingBonusText.text = ((int)secondsRemainingBonusNum).ToString();
                perfectRoundBonusText.text = ((int)perfectRoundBonusNum).ToString();
                totalScoreText.text = ((int)totalScoreNum).ToString();
            }
            else
            {
                scoreCountUpComplete = true;

                // Set these to their final values
                roundStartScore.text = roundStartScoreNumMax.ToString();
                roundScore.text = roundScoreNumMax.ToString();
                secondsRemainingBonusText.text = secondsRemainingBonusNumMax.ToString();
                perfectRoundBonusText.text = perfectRoundBonusNumMax.ToString();
                totalScoreText.text = totalScoreNumMax.ToString();
            }
        }
    }

	public void OnEndGameButtonClick() {
		if (!exitConfirmationDialogDisplayed) {
			ExitConfirmationToggle(true);
		}
	}

	public void NextRound() {
		if (!exitConfirmationDialogDisplayed) {

            if (gc.GetGameMode() == "capitals")
            {
                SceneManager.LoadScene("CapitalsGameSceneLandscape");
            }
            else
            {
                SceneManager.LoadScene("StatesGameSceneLandscape");
            }
        }
	}

	public void OnNoButtonClick() {
		ExitConfirmationToggle(false);
	}

	public void OnYesButtonClick() {
		ExitConfirmationToggle(false);
		ReturnToMenu();
	}

	private void ExitConfirmationToggle(bool displayed) {
		exitConfirmationDialogDisplayed = displayed;
		exitConfirmation.SetActive(displayed);
        endGameButton.interactable = !displayed;
        nextRoundButton.interactable = !displayed;

        float alpha;
        if (displayed)
        {
            alpha = 0.25f;
        }
        else
        {
            alpha = 1.0f;
        }
        Color c = roundOverText.color;
        c.a = alpha;
        roundOverText.color = c;
        
	}

	public void ReturnToMenu() {
        gc.roundActive = false;
        gc.gameActive = false;
		SceneManager.LoadScene("MenuScreen2Landscape");
	}
}
