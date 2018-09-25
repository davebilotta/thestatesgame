using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class RoundOverController : MonoBehaviour {

	public Text roundScoreText;
	public Text secondsRemainingBonusText;
	public Text perfectRoundBonusText;
	public Text totalScoreText;
	public GameObject mainDisplay;
	public GameObject exitConfirmation;

	private bool exitConfirmationDialogDisplayed;

private GameController gc;

	// Use this for initialization
	void Awake () {
		
		gc = FindObjectOfType<GameController>();

		roundScoreText.text = gc.roundScore.ToString();
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
	}
	public void ReturnToMenu() {
		SceneManager.LoadScene("MenuScreen");
	}
}
