using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class RoundOverController : MonoBehaviour {

    public Text roundStartScore;
    public Text roundScore;
	public Text secondsRemainingBonusText;
	public Text perfectRoundBonusText;
	public Text totalScoreText;
	public GameObject mainDisplay;
	public GameObject exitConfirmation;

    public Text roundOverText;
    public Button endGameButton;
    public Button nextRoundButton;

	private bool exitConfirmationDialogDisplayed;

private GameController gc;

	// Use this for initialization
	void Awake () {
		
		gc = FindObjectOfType<GameController>();

		roundStartScore.text = gc.roundStartScore.ToString();
        roundScore.text = gc.roundScore.ToString();
        secondsRemainingBonusText.text = gc.secondsRemainingBonus.ToString();
		if (gc.perfectRound) {
			perfectRoundBonusText.text = gc.perfectRoundBonus.ToString() + "";
		}
		else {
			perfectRoundBonusText.text = "0";
		}
			
		totalScoreText.text = gc.playerScore.ToString();
	}

	// Update is called once per frame
	void Update () {}

	public void OnEndGameButtonClick() {
		if (!exitConfirmationDialogDisplayed) {
			ExitConfirmationToggle(true);
		}
	}

	public void NextRound() {
		if (!exitConfirmationDialogDisplayed) {
			SceneManager.LoadScene("StatesGameScene");
		//gc.StartRound();
		}
	}

	public void OnNoButtonClick() {
		//mainDisplay.SetActive(true);
		ExitConfirmationToggle(false);
	}

	public void OnYesButtonClick() {
		//mainDisplay.SetActive(true);
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
		SceneManager.LoadScene("MenuScreen");
	}
}
