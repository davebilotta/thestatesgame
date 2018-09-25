using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class AnswerButton : MonoBehaviour {

	public Text answerText;         // store a reference to the button text
	private AnswerData answerData;  // 
	private GameController gameController;
	public Question question;

	void Start () {
		gameController = FindObjectOfType<GameController>();
	}

	public void Setup(AnswerData data) {
		answerData = data;
		answerText.text = answerData.answerText;
	}

	public void OnClick() {
		gameController.OnAnswerButtonClick(this);
	}
}
