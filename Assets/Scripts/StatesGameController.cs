using UnityEngine;
using System.Collections;

public class StatesGameController : MonoBehaviour {

	GameController gameController;
	private DisplayController displayController;

	// Use this for initialization
	void Awake () {
		//Logger.Log("***** STATESGAME CONTROLLER AWAKE *****");
		DataController dataController = FindObjectOfType<DataController>();
		dataController.LoadGameDataForScene("states");

		gameController = FindObjectOfType<GameController>();
		gameController.SetGameMode("states");

	}

	void Start() {
		//Logger.Log("***** STATESGAME CONTROLLER START *****");

		displayController = GetComponent<DisplayController>();
		
		gameController.displayController = displayController;
		gameController.StartRound();

	}

	public void OnPauseButtonClick() {
		if (gameController.gamePaused) {
			gameController.GameUnpause();
		}
		else {
			gameController.GamePause();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
