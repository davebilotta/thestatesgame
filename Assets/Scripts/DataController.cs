using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

public class DataController : MonoBehaviour {
    private int roundSize = 3;           // This is how many questions per round
	public int numAnswers = 5;           // This is how many answers we present to user

	private int maxQuestions = 6;  // This is just for testing purposes to get to end of round/game quicker

	private StatesGameData[] statesData;
	//private List<StatesGameData> roundData = new List<StatesGameData>();

	public List<Question> questionData = new List<Question>();    // this is a list of all questions that is built from file

	public List<RoundData> roundData = new List<RoundData>();     // this is a list of all rounds (items above divided by roundSize) ... each item in list is a list of Questions

	private PlayerProgress playerProgress;

	private string statesGameDataFileName = "statesdata.json";
	//private string capitalsGameDataFileName = "capitalsdata.json";

	void Start () {
		DontDestroyOnLoad(gameObject);            // We want this to persist when we load new scenes

		LoadGameData();
		LoadPlayerProgress();

        // TODO: Should this get moved elsewhere?
        SceneManager.LoadScene("MenuScreenLandscape");
        //SceneManager.LoadScene("MenuScreenWithTransitions");

    }
		
	public StatesGameData getCurrentRoundData() {
		// for now we'll only have one item 
		return statesData[0];
	}

	//public List<Question> getCurrentRoundDataNew(int roundNumber) {
	public Question[] getCurrentRoundDataNew(int roundNumber) {
			// TODO: Check index out of range 
		//return roundData[roundNumber];
		// TODO: This needs to return an array of questions 
		List<Question> qList = roundData[roundNumber].questionList;
		Question[] questionList = new Question[qList.Count];
		int cnt = 0;
		foreach (Question q in qList) {
			questionList[cnt] = q;
			cnt++;
		}
		return questionList;
	} 

	private void LoadPlayerProgress() {
		playerProgress = new PlayerProgress();

		// Check if we've already stored a highest score
		if (PlayerPrefs.HasKey("highestScore")) {
			playerProgress.highestScore = PlayerPrefs.GetInt("highestScore");
		}
	}

	private void SavePlayerProgress() {
		// Save the player's progress
		PlayerPrefs.SetInt("highestScore",playerProgress.highestScore);
	}

	public bool SubmitNewPlayerScore(int newScore) {
		// Every time a round end, game controller submits at end 
		// Check if it's higher than what we have stored 
		if (newScore > playerProgress.highestScore) {
            Logger.Log("FOUND A NEW HIGH SCORE OF " + newScore);
			playerProgress.highestScore = newScore;
			SavePlayerProgress();
            return true;
		}
        Logger.Log("Score of " + newScore + " is not higher than " + playerProgress.highestScore);
        return false;
	}
		
	public int GetHighestPlayerScore() {
		return playerProgress.highestScore;
	}

	private void LoadGameData() {
		
		string filePath = Path.Combine(Application.streamingAssetsPath,statesGameDataFileName);

		if (File.Exists(filePath)) {
			// read all text from file at path 
			string dataAsJson = File.ReadAllText(filePath);

			GameData loadedData = JsonUtility.FromJson<GameData>(dataAsJson);

			// Load all of the states 
			statesData = loadedData.statesData;
			StatesGameData[] data = statesData;

			int questionCount = 0;
			// Build up Question objects in questionData
			foreach (StatesGameData d in data) {
				foreach (Question q in d.questions) {
					if (questionCount < maxQuestions) {
						questionData.Add(q);
						questionCount++;
					}
				}
			} 

			ShuffleQuestions();

			// Now go through and divide up all of these questions into rounds
			List<Question> questionList = new List<Question>();
			foreach (Question q in questionData) {
				questionList.Add(q);

				if (questionList.Count >= roundSize) {
					roundData.Add(new RoundData(questionList));
					questionList = new List<Question>();
						//.Clear();
				}
			}

			// Handle leftover for cases where the full file doesn't have X * roundSize items 
			if (questionList.Count > 0) {
				roundData.Add(new RoundData(questionList));
			}

			//ShowRoundData();

		}

		else {
			Logger.LogError("Cannot load game data");	
		}
	}

	public void ShowRoundData() {
		int cnt = 0;
		foreach (RoundData r in roundData) {
			Logger.Log("Round " + cnt );
			foreach (Question q in r.questionList) {
				Logger.Log(q.abbreviation + " " + q.name + " " + q.capital);
			}	
			cnt++;
		}
	}

	public void ShuffleQuestions() {
		
		// shuffles a round and returns the value 
		List<Question> tempQuestionData = questionData;
		List<Question> finalQuestionData = new List<Question>();

		while (tempQuestionData.Count > 0) {
			int r = Random.Range(0,tempQuestionData.Count);
			finalQuestionData.Add(tempQuestionData[r]);
			tempQuestionData.Remove(tempQuestionData[r]);
		}

		questionData = finalQuestionData;
	}

	public void LoadGameDataForScene(string gameMode) {
		Logger.Log("Loading data for " + gameMode);
	}

	void Update () {

	}

	private void LoadHighScoresData() {

	}

	private void SaveHighScoresData() {

	}
		
}
