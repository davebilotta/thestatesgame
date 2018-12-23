using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

public class DataController : MonoBehaviour {
    private bool testingMode = false;

    private int roundSize;             // This is how many questions per round
	public int numAnswers;             // This is how many answers we present to user
    private int maxQuestions;          // This is just for testing purposes to get to end of round/game quicker

	private StatesGameData[] statesData;

	public List<Question> questionData = new List<Question>();    // this is a list of all questions that is built from file

	public List<RoundData> roundData;     // this is a list of all rounds (items above divided by roundSize) ... each item in list is a list of Questions

	private PlayerProgress playerProgress;

	private string statesGameDataFileName = "statesdata.json";

    void Start () {
        DontDestroyOnLoad(gameObject);            // We want this to persist when we load new scenes

        if (testingMode)
        {
            roundSize = 2;
            maxQuestions = 6;
        }
        else
        {
            roundSize = 10;
            maxQuestions = 50;
        }
        numAnswers = Mathf.Min(5, maxQuestions);
		LoadGameData();
		LoadPlayerProgress();

        // TODO: Once the main screen has more than just Play active, change this back to MenuScreenLandscape 
        SceneManager.LoadScene("MenuScreen2Landscape");
 
    }

    public Question[] getCurrentRoundData(int roundNumber) {

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

    private GameData LoadFile ()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, statesGameDataFileName);

        Logger.Log("Will load file from " + filePath);

        string dataAsJson = "";

        if (Application.platform == RuntimePlatform.Android)
        {
            Logger.Log("Reading for Android");
            WWW reader = new WWW(filePath);
            
            Logger.Log("Done with create");
            
            dataAsJson = reader.text;

        }
        else
        {
            Logger.Log("Reading for Other");
            if (File.Exists(filePath))
            {
                dataAsJson = File.ReadAllText(filePath);
            }
        }

        Logger.Log("Data is " + dataAsJson.ToString());
        return JsonUtility.FromJson<GameData>(dataAsJson); 
    }

	private void LoadGameData() {
        Logger.Log("In LoadGameData()");

        GameData loadedData = LoadFile();
        
        if (loadedData != null) {
          
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

            // TODO: Remove this later 
            //BuildRoundData();
			
		}

		else {
			Logger.LogError("Cannot load game data");	
		}
	}

    public void BuildRoundData()
    {
        ShuffleQuestions();


        // Now go through and divide up all of these questions into rounds
        List<Question> questionList = new List<Question>();
        roundData = new List<RoundData>();

        foreach (Question q in questionData)
        {
            questionList.Add(q);

            if (questionList.Count >= roundSize)
            {
                roundData.Add(new RoundData(questionList));
                questionList = new List<Question>();
                //.Clear();
            }
        }

        // Handle leftover for cases where the full file doesn't have X * roundSize items 
        if (questionList.Count > 0)
        {
            roundData.Add(new RoundData(questionList));
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
