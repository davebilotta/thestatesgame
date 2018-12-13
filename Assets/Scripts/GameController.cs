using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public DataController dataController;

	public DisplayController displayController;

	public Text questionText;
	public Image questionImage;
	public Text scoreText;

	public GameObject pauseDisplay;              // this is the component that gets hidden/revealed as game is paused/unpaused

	private Question[] currentRoundData;
	private Question[] questionPool;

	public bool gamePaused;
	public bool roundActive;                     // Has the round started?
	public bool gameActive;                      // Has the game started?
	public float timeRemaining;                   // How much time is left
	private int questionIndex = 0;                // what number question are we on
	public int playerScore;                       // This is the cumulative player score (what displays at the top)
	public int roundScore;                        // This is the score for the round 
    public int roundStartScore;                   // This is the score at the start of the round (playerScore at the end of the previous round with all bonuses added)
	public int totalRoundScore = 0;               // This is the total score for all rounds (used at end of the game)
	public Question currentQuestion;              // The Question we are on
	public List<Question> answers;                // These are the other Answers
	//private List<GameObject> answerButtonGameObjects = new List<GameObject>();
	 
	public enum MyGameMode {States,Capitals};
	public MyGameMode mode;

	public int roundNumber = -1;                 // which round out of (1-N) are we on
	private int roundTimeLimit = 90;              // how long will each round last, in seconds
	private int pointsPerSecondRemaining = 1;     // how many points does each remaining second get you 
	private int pointsPerButtonRemaining = 10;    // how many points does each correct answer get you
	public bool perfectRound;                     // gets initialized in StartRound
	public bool perfectGame;                      // this only gets initialized here and gets cleared on an incorrect answer
	public int secondsRemainingBonus = 0;         // How many bonus points do you get for remaining seconds
	public int perfectRoundBonus = 100;           // How many bonus points do you get for a perfect round
	public int perfectGameBonus = 250;            // How many bonus points do you get for a perfect game

    public bool newHighScore;

	void Awake () 
	{
		DontDestroyOnLoad(gameObject); 
	}

	void Start() 
	{
		Logger.Log("***** GAMECONTROLLER START *****");
		dataController = FindObjectOfType<DataController>();

		// TODO: Figure this out 
		//questionPool = currentRoundData; // store questions we're going to be asking

	}
	public void StartGame() {
		Logger.Log("GAME IS NOW ACTIVE");
		gameActive = true;
		playerScore = 0;
		roundNumber = -1;
		perfectGame = true;
        newHighScore = false;
	}

	public void StartRound() 
	{
		roundActive = true;
		roundScore = 0;
		roundScore = 0;
        roundStartScore = playerScore;
		perfectRound = true;

		roundNumber++;
		Logger.Log("Starting round " + roundNumber + ", Perfect Game =" + perfectGame.ToString());
		
		// TODO Need to rename so it doesn't say "new"
		currentRoundData = dataController.getCurrentRoundDataNew(roundNumber);

        displayController.UpdateRoundText();
        displayController.UpdateScoreText();

		questionPool = currentRoundData; // store questions we're going to be asking

		questionIndex = 0;
		timeRemaining = roundTimeLimit;

        Logger.Log("timeRemaining: " + timeRemaining.ToString() + ", roundTimeLimit" + roundTimeLimit.ToString());

        NextQuestion();

		gamePaused = false;
	}

	private void NextQuestion() 
	{
		// TODO: Do we increment here? 

		// Get question 
		currentQuestion = questionPool[questionIndex]; 

		// Get the other answers 
		answers = FindOtherQuestions(currentQuestion,dataController.numAnswers - 1);
		
		// Now call logic to show it
		displayController.ShowQuestion();
	}
    
	public List<Question> Shuffle(List<Question> items) {

		// shuffles a round and returns the value 
		List<Question> tempList = items;
		List<Question> finalList = new List<Question>();

		while (tempList.Count > 0) {
			int r = Random.Range(0,tempList.Count);
			finalList.Add(tempList[r]);
			tempList.Remove(tempList[r]);
		}

		return finalList;
	}
		
	public void OnAnswerButtonClick(AnswerButton button) 
	{
		// ***** CORRECT ANSWER *****
		if (button.question == currentQuestion) {
			//Logger.Log("CORRECT!");

			// Score for each button left (if you get it on the first try it's 5, if there were 2 incorrect guesses it's 3, etc.)
			int score = displayController.GetNumAnswersRemaining() * pointsPerButtonRemaining;
            roundScore += score;
            playerScore += score;

			displayController.OnCorrectAnswer(score);

			// Do we still have questions left?
			if (questionPool.Length > questionIndex + 1) {
				Logger.Log("Still have more questions " + questionPool.Length + " " + questionIndex);
				questionIndex++;
				NextQuestion();
			}
			else {
				Logger.Log("Checking round number " + roundNumber + " " + dataController.roundData.Count);
				if (roundNumber >= (dataController.roundData.Count-1)) {
					EndGame();
				}
				else {
					EndRound();
				}
			} 
		}

		// ***** INCORRECT ANSWER *****
		else 
		{
			perfectRound = false;
			perfectGame = false;
			displayController.OnIncorrectAnswer(button);
		}
	}
		
	public void AddRoundScore() {
		// We're done with the round, so player score is base score of round plus any bonuses 
		
		// Add bonus for each second remaining * bonus per second 
		secondsRemainingBonus = (Mathf.CeilToInt(timeRemaining) * pointsPerSecondRemaining);
		playerScore += secondsRemainingBonus;

		// Add bonus for perfect round  
		if (perfectRound) {
			playerScore += perfectRoundBonus;
		}
	}

	public bool AddGameScore() {
		totalRoundScore = playerScore;

		if (perfectGame) {
			Logger.Log("Adding perfect game bonus!");
			playerScore += perfectGameBonus;
		}
        return dataController.SubmitNewPlayerScore(playerScore);
    }

	public void EndRound() {
		roundActive = false;
        displayController.EndRound();
        
        AddRoundScore();
		SceneManager.LoadScene("RoundOverScene");
	}

	public void EndGame() {
		Logger.Log("GAME OVER");
		roundActive = false;
		gameActive = false;
        Logger.Log("Perfect Game = " + perfectGame);
        displayController.EndRound();

        AddRoundScore();
		newHighScore = AddGameScore();
		SceneManager.LoadScene("GameOverScene");
	}

	public void ReturnToMenu() {
        // TODO: This needs changes to use landscape mode
		SceneManager.LoadScene("MenuScreenLandscape");
	}

	public void GamePause() {
		Logger.Log("PAUSING GAME!");
		displayController.GamePause(true);
		gamePaused = true;
	}

	public void GameUnpause() {
		Logger.Log("UNPAUSING GAME!");
		displayController.GamePause(false);
		gamePaused = false;
	}

	/*private void UpdateTimeRemainingDisplay() {
		timeRemainingText.text = "Time Remaining: " + Mathf.Round(timeRemaining).ToString();
	} */

	public List<Question> FindOtherQuestions(Question q,int n) {
		// Given a question, find n other Questions that do not match

		List<Question> fullQuestionList = dataController.questionData;
		List<Question> questionList = new List<Question>();
		Question tempQuestion = new Question();

		for (int i = 0; i < n; i ++) {
			bool found = false;
			while (!found) {
				// get a random one 

				tempQuestion = fullQuestionList[Random.Range(0,fullQuestionList.Count)];
				// if doesn't match q and is not already in list, set found to True 
				// otherwise do nothing
				if (tempQuestion.abbreviation != q.abbreviation && !questionList.Contains(tempQuestion)) {
					found = true;
				}
			}

			questionList.Add(tempQuestion);

		}

		return questionList;
	}

	void Update () {
		if (!gamePaused) {
			if (roundActive) {
				// subtract time it took to render the last frame
				timeRemaining -= Time.deltaTime;

               if (timeRemaining <= 0f) {
					perfectRound = false;
					perfectGame = false;
					if (roundNumber >= (dataController.roundData.Count-1)) {
					    EndGame();
					}	
					else {
						EndRound();
					}
				}
			}
		}
	}

	public void SetGameMode(string startMode) {
		if (startMode.ToLower() == "states") {
			mode = MyGameMode.States;
			}
		else {
			mode = MyGameMode.Capitals;
			}
	}

	public string GetGameMode() { 
		if (mode == MyGameMode.States) {
			return "states";
		}
		else {
			return "capitals";
		}
	}

	public List<PlayerResult> GetHighScores() {
		// TODO: Eventually want to pull this from the high scores file, sort, and return the 10 highest 
		
		List<PlayerResult> highScores = new List<PlayerResult>();
	
		highScores.Add(new PlayerResult("Dave",1000,"01/02/2018"));
		highScores.Add(new PlayerResult("Leana",2000,"02/16/2018"));
		highScores.Add(new PlayerResult("Lukas",1920,"03/01/2018"));
		highScores.Add(new PlayerResult("Jonah",1670,"04/18/2018"));
		highScores.Add(new PlayerResult("Dave",6547,"05/09/2018"));
		highScores.Add(new PlayerResult("Sammy",915,"06/14/2018"));
		highScores.Add(new PlayerResult("Julia",57,"07/04/2018"));
		highScores.Add(new PlayerResult("Lukas",999,"08/29/2018"));
		highScores.Add(new PlayerResult("Leana",32,"09/22/2018"));
		highScores.Add(new PlayerResult("Jonah",876,"10/12/2018"));
		
		return highScores;
	}

    // TODO: Put this into a generic class

    /*private static List<T> Shuffle<T> (List<T> items) {

		// shuffles a round and returns the value 
		List<Object> tempList = items;
		List<Object> finalList = new List<Object>();

		while (tempList.Count > 0) {
			int r = Random.Range(0,tempList.Count);
			finalList.Add(tempList[r]);
			tempList.Remove(tempList[r]);
		}

		return finalList;
	}*/

    /*private void RemoveAllAnswerButtons() {
    // Go through and return ALL answerButtons to object pool that are in use

    while (answerButtonGameObjects.Count > 0) {
        // Return to the pool and remove from List 
        answerButtonObjectPool.ReturnObject(answerButtonGameObjects[0]);
        answerButtonGameObjects.RemoveAt(0);
    }
}*/

    /*private void RemoveAnswerButton(AnswerButton button) {
		// At this point we've determined the answer is not correct, so remove it from the list of buttons on screen

		bool done = false;
		int cnt = 0;

		// can't do a foreach and modify list in the process so use a while
		while (!done) {
			GameObject b = answerButtonGameObjects[cnt];
			//Logger.Log("Evaluating for removal " + b.GetComponent<AnswerButton>().question.name);
			if (b.GetComponent<AnswerButton>().question.abbreviation == button.question.abbreviation) {
				int pos = answerButtonGameObjects.IndexOf(b);
				answerButtonObjectPool.ReturnObject(b);
				answerButtonGameObjects.RemoveAt(pos);
				done = true;
			}
			cnt++;
		}
	} */

}
