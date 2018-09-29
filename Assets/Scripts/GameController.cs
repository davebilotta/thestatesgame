using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	private DataController dataController;

	public DisplayController displayController;

	public Text questionText;
	public Image questionImage;
	public Text scoreText;

	public GameObject pauseDisplay;              // this is the component that gets hidden/revealed as game is paused/unpaused

	private Question[] currentRoundData;
	private Question[] questionPool;

	public bool gamePaused;
	private bool roundActive;                     // Has the round started?
	private bool gameActive;                      // Has the game started?
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
	private int roundTimeLimit = 60;              // how long will each round last, in seconds
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
        roundStartScore = playerScore;
		perfectRound = true;
		displayController.UpdateScoreText();

		// TODO: These two lines can be removed 
		//Logger.Log("MY Display Controller is " + displayController.ToString());
		//displayController.Announce();

		roundNumber++;
		Logger.Log("Starting round " + roundNumber);
		
		// TODO Need to rename so it doesn't say "new"
		currentRoundData = dataController.getCurrentRoundDataNew(roundNumber);

        displayController.UpdateRoundText();

		questionPool = currentRoundData; // store questions we're going to be asking

		questionIndex = 0;
		timeRemaining = roundTimeLimit;
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

	/*private void ShowQuestion() {
		// Stop timer at beginning 
		//roundActive = false;

		// Clear out any old ones
		RemoveAllAnswerButtons();    

		// Get what question we're on from the pool and store it 
		Logger.Log("idx" + questionIndex);

		currentQuestion = questionPool[questionIndex]; 
		string qText;
		Sprite questionSprite;

		// TODO: This should probably all be done up front? 
	
		// Build string for question and image depending on mode
		if (GetGameMode() == "states") {
			qText = "Which state is this? ";
			//answer = currentQuestion.abbreviation;

			// TODO: 1) need to fix names with spaces
			// TODO: Need to figure out a way to pull this from the json and Preload it 
			questionSprite = Resources.Load<Sprite>(getImagePath(currentQuestion));

		}
		else {
			qText = "What is the capital of " + currentQuestion.name + "?"; 
			//answer = currentQuestion.capital;
			questionSprite = Resources.Load<Sprite>(getImagePath(currentQuestion));

		}

		questionText.text = qText;
		questionImage.sprite = questionSprite;

		questionImage.GetComponent<Image>().sprite = questionSprite;

		// now find 4 other questions TODO make these into buttons
		List<Question> questionAnswers = FindOtherQuestions(currentQuestion,dataController.numAnswers - 1);

		questionAnswers.Add(currentQuestion);
		questionAnswers = Shuffle(questionAnswers);

		// Show as many buttons as the question has answers
			for (int i = 0; i < questionAnswers.Count; i++) {
				// get me another spawned button that isn't being used
				GameObject answerButtonGameObject = answerButtonObjectPool.GetObject(); 

				// Parent the buttons to the panel - will fall into vertical 
				// layout group and be arranged correctly
				answerButtonGameObject.transform.SetParent(answerButtonParent);

				// add to the list of objects
				answerButtonGameObjects.Add(answerButtonGameObject);

				// Get the AnswerButton script attached
				AnswerButton answerButton = answerButtonGameObject.GetComponent<AnswerButton>();
				answerButton.question = questionAnswers[i];

			if (GetGameMode() == "states") {
				answerButton.Setup(new AnswerData(questionAnswers[i].name));
			}
			else {
				answerButton.Setup(new AnswerData(questionAnswers[i].capital));
			}
		}

		//roundActive = true;
	} */
		
	/*private string getImagePath(Question q) {
		string imagePath = "";
		if (GetGameMode() == "states") {
			imagePath = "raw/state_images/";
		}
		else {
			imagePath = "raw/state_images_capital/";
		}
		string[] n = q.name.ToLower().Split();
		foreach (string s in n) {
			imagePath += s; 		
		}
		return imagePath;
	} */

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

			displayController.OnCorrectAnswer();

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
		AddRoundScore();
		SceneManager.LoadScene("RoundOverScene");
	}

	public void EndGame() {
		Logger.Log("GAME OVER");
		roundActive = false;
		gameActive = false;
		AddRoundScore();
		newHighScore = AddGameScore();
		SceneManager.LoadScene("GameOverScene");
	}

	public void ReturnToMenu() {
		SceneManager.LoadScene("MenuScreen");
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
