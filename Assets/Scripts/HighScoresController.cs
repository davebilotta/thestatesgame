using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class HighScoresController : MonoBehaviour {

	private GameController gc;
	private List<PlayerResult> highScores;
	public Transform highScoresDisplay;
	public GameObject playerResultPrefab;
	
	void Start () {
		gc = FindObjectOfType<GameController>();

		highScores = gc.GetHighScores();

		foreach (PlayerResult res in highScores) {

			GameObject go = (GameObject)Instantiate(playerResultPrefab).gameObject;
			Text[] textItems = go.GetComponentsInChildren<Text>();
			textItems[0].text = res.playerName;
			textItems[1].text = res.playerScore.ToString();
			textItems[2].text = res.resultDate;
			
			go.transform.SetParent(highScoresDisplay);

		}		
	}
	
	public void OnHomeButtonClick() { 
		SceneManager.LoadScene("MenuScreen");
	}
}
