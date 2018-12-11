using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class DisplayController : MonoBehaviour {
	private GameController gc;

	public Text questionText;
	public Image questionImage;
	public Text scoreText;
    public Text roundText;
	public Text timeRemainingText;

    public Text scoreAnimationText;
    public GameObject scoreAnimationDisplay;
    private bool scoreAnimationActive = false;
    private float scoreAnimationFadeDelay = 0.025f;

    public GameObject pauseDisplay;

    public SceneTransitions sceneTransitions;
	public SimpleObjectPool answerButtonObjectPool;
	public Transform answerButtonParent;
	private List<GameObject> answerButtonGameObjects = new List<GameObject>();
    

	void Awake() {
		//Logger.Log("DISPLAY CONTROLLER AWAKE");
		gc = FindObjectOfType<GameController>();
        gc.displayController = this;

        sceneTransitions = FindObjectOfType<SceneTransitions>();
     //   Debug.Log("ST " + sceneTransitions.ToString());

        // TODO: Should this get moved elsewhere?
        //SceneManager.LoadScene("MenuScreenLandscape");
        //SceneManager.LoadScene("MenuScreenWithTransitions");
       // LoadScene("MenuScreenLandscape");
       
    }

    void Start () {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // TODO: This throws a single error at start, need to figure this out (tried adding && gc.displayController != null but it didn't work)
        if (gc.roundActive)
        {
            UpdateTimeRemainingDisplay();
            UpdateScoreAnimation();
        }
    }

    public void UpdateScoreText()
    {
        scoreText.text = gc.playerScore.ToString();

    }

    public void UpdateRoundText()
    {
        roundText.text = "Round " + (gc.roundNumber + 1) + " of " + gc.dataController.roundData.Count;
    }


    private void UpdateTimeRemainingDisplay()
    {
        // timeRemainingText.text = Mathf.Round(gc.timeRemaining).ToString();

        // TODO: why can't I just reference timeRemainingText alone? (Throws an error)
        gc.displayController.timeRemainingText.text = Mathf.Round(gc.timeRemaining).ToString();
    }

	public void ShowQuestion() {

		// Clear out any old ones
		RemoveAllAnswerButtons();    

		// Get what question we're going to show 
		Question currentQuestion = gc.currentQuestion;

		string qText;
		Sprite questionSprite;

		// TODO: This should all be done when the level loads

		// Build string for question and image depending on mode
		if (gc.GetGameMode() == "states") {
			qText = "Which state is this? ";

			// TODO: 1) need to fix names with spaces
			// TODO: Need to figure out a way to pull this from the json and Preload it 
			questionSprite = Resources.Load<Sprite>(getImagePath(currentQuestion));

		}
		else {
			qText = "What is the capital of " + currentQuestion.name + "?"; 
			questionSprite = Resources.Load<Sprite>(getImagePath(currentQuestion));

		}

		questionText.text = qText;
		questionImage.sprite = questionSprite;

		questionImage.GetComponent<Image>().sprite = questionSprite;

		// now find 4 other questions TODO make these into buttons
		List<Question> answers = gc.answers;
		answers.Add(currentQuestion);
		answers = gc.Shuffle(answers);

		//Logger.Log("The parent is " + answerButtonParent.ToString());

		// Show as many buttons as the question has answers
		for (int i = 0; i < answers.Count; i++) {
			// get me another spawned button that isn't being used
			GameObject answerButtonGameObject = answerButtonObjectPool.GetObject(); 

			// Parent the buttons to the panel - will fall into vertical 
			// layout group and be arranged correctly
			answerButtonGameObject.transform.SetParent(answerButtonParent);
           

			// add to the list of objects
			answerButtonGameObjects.Add(answerButtonGameObject);

			// Get the AnswerButton script attached
			AnswerButton answerButton = answerButtonGameObject.GetComponent<AnswerButton>();
			answerButton.question = answers[i];

			if (gc.GetGameMode() == "states") {
				answerButton.Setup(new AnswerData(answers[i].name));
			}
			else {
				answerButton.Setup(new AnswerData(answers[i].capital));
			}
		}

		//roundActive = true;
	}

	private void RemoveAllAnswerButtons() {
		// Go through and return ALL answerButtons to object pool that are in use

		while (answerButtonGameObjects.Count > 0) {
			// Return to the pool and remove from List 
			answerButtonObjectPool.ReturnObject(answerButtonGameObjects[0]);
			answerButtonGameObjects.RemoveAt(0);
		}
	}

	public void OnCorrectAnswer(int score) {
		UpdateScoreText();
        ActivateScoreAnimation(score); 
	}
       
    public void UpdateScoreAnimation()
    {
        if (scoreAnimationActive)
        {
            Color c = scoreAnimationText.color;
            //Color c = scoreAnimationText.color;

            if (c.a > 0f)
            {
                c.a -= scoreAnimationFadeDelay;
                scoreAnimationText.color = c;
            }
            else
            {
                InactivateScoreAnimation();                
            }           
        }

    }

    public void ActivateScoreAnimation(int score)
    {
        scoreAnimationText.text = "+" + score.ToString();
        Color c = scoreAnimationText.color;
        c.a = 1.0f;
        scoreAnimationText.color = c;
        
        // Check this in case user keeps answering quickly while animation is active. 
        // Probably overkill but just leave it 
        if (!scoreAnimationActive)
        {
            scoreAnimationActive = true;
            scoreAnimationDisplay.SetActive(true);
        }
    }

    public void InactivateScoreAnimation()
    {
        if (scoreAnimationActive)
        {
            scoreAnimationActive = false;
            scoreAnimationDisplay.SetActive(false);
        }
    }

    public void OnIncorrectAnswer(AnswerButton button) {
		//ActivateScoreAnimation();
		RemoveAnswerButton(button);
	}

	public int GetNumAnswersRemaining() 
	{ 
		return answerButtonGameObjects.Count;
	}

	private void RemoveAnswerButton(AnswerButton button) {
		// At this point we've determined the answer is not correct, so remove it from the list of buttons on screen

		bool done = false;
		int cnt = 0;

		// can't do a foreach and modify list in the process so use a while
		while (!done) {
			GameObject b = answerButtonGameObjects[cnt];
			if (b.GetComponent<AnswerButton>().question.abbreviation == button.question.abbreviation) {
				int pos = answerButtonGameObjects.IndexOf(b);
				answerButtonObjectPool.ReturnObject(b);
				answerButtonGameObjects.RemoveAt(pos);
				done = true;
			}
			cnt++;
		}
	}

	public void ActivateCorrectAnimation() {
       
    }

	public void ActivateIncorrectAnimation() {

	}

	public void GamePause(bool gamePaused) {
		pauseDisplay.SetActive(gamePaused);
	}	

	private string getImagePath(Question q) {
		string imagePath = "";
		if (gc.GetGameMode() == "states") {
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
	}

    // TODO: this can eventually go away
    /*public void Announce()
    {
        //Logger.Log("Announcing DisplayController");
    } */

    public void LoadScene(string sceneName)
    {
        sceneTransitions.LoadScene(sceneName);
    }

    public void EndRound()
    {
        scoreAnimationActive = false;
    }

    /*public List<Question> FindOtherQuestions(Question q,int n) {
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
	}*/
}
