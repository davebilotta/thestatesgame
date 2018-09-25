using UnityEngine;
using System.Collections;

// This attribute is needed so you can edit and display values in editor
[System.Serializable]
public class AnswerData  {

	public string answerText; 
	public bool	isCorrect;

	public AnswerData(string answerText) {
		this.answerText = answerText;
	}
}
