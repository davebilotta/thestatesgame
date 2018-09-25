using UnityEngine;
using System.Collections;

public class CapitalsGameController : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		DataController dataController = FindObjectOfType<DataController>();
		dataController.LoadGameDataForScene("capitals");

		GameController gc = FindObjectOfType<GameController>();
		gc.SetGameMode("capitals");

		gc.StartRound();
	}

	// Update is called once per frame
	void Update () {

	}
}
