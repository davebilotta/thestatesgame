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

    public Text answerAnimationText;
    public GameObject answerAnimationDisplay;
    private bool answerAnimationActive = false;
    private float answerAnimationFadeDelay = 0.025f;

    public GameObject pauseDisplay;

    public SceneTransitions sceneTransitions;
    public SimpleObjectPool answerButtonObjectPool;
    public Transform answerButtonParent;
    private List<GameObject> answerButtonGameObjects = new List<GameObject>();
    private List<Sprite> RoundImageList;



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

    void Start() {
        //   DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // TODO: This throws a single error at start, need to figure this out (tried adding && gc.displayController != null but it didn't work)
        if (gc.roundActive)
        {
            UpdateTimeRemainingDisplay();
            UpdateScoreAnimation();
            UpdateAnswerAnimation();
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

    public void UpdateScoreAnimation()
    {
        if (scoreAnimationActive)
        {
            Color c = scoreAnimationText.color;

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

    public void UpdateAnswerAnimation()
    {
        if (answerAnimationActive)
        {
            Color c = answerAnimationText.color;

            if (c.a > 0f)
            {
                c.a -= answerAnimationFadeDelay;
                answerAnimationText.color = c;
            }
            else
            {
                InactivateAnswerAnimation();
            }
        }
    }
    public Sprite GetQuestionImage(Question q)
    {
        return q.image;
    }

    public void ShowQuestion() {

        // Clear out any old ones
        RemoveAllAnswerButtons();

        // Get what question we're going to show 
        Question currentQuestion = gc.currentQuestion;

        string qText;

        // Build string for question depending on mode
        if (gc.GetGameMode() == "states") {
            qText = "Which state is this? ";
        }
        else {
            qText = "What is the capital of " + currentQuestion.name + "?";
        }

        Sprite questionSprite = currentQuestion.image;

        questionText.text = qText;
        questionImage.sprite = questionSprite;

        questionImage.GetComponent<Image>().sprite = questionSprite;

        // now find 4 other questions TODO make these into buttons
        List<Question> answers = gc.answers;
        answers.Add(currentQuestion);
        answers = gc.Shuffle(answers);

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

    private void RemoveAllAnswersButCorrect()
    {
        Logger.Log("REMOVING ALL BUT CORRECT");

        int cnt = 0;
        int max = answerButtonGameObjects.Count;

        // can't do a foreach and modify list in the process so use a while
        while (cnt < max)
        {
            GameObject b = answerButtonGameObjects[cnt];
            if (b.GetComponent<AnswerButton>().question.abbreviation != gc.currentQuestion.abbreviation)
            {
                int pos = answerButtonGameObjects.IndexOf(b);
                answerButtonObjectPool.ReturnObject(b);
                answerButtonGameObjects.RemoveAt(pos);

            }
            cnt++;
        }
    }

    public void OnCorrectAnswer(int score)
    {
        ActivateAnswerAnimation();
        UpdateScoreText();
        ActivateScoreAnimation(score);
    }

    // Used for kicking off correct answer animation
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

    public void ActivateAnswerAnimation()
    {
        if (gc.GetGameMode() == "capitals") {
               answerAnimationText.text = gc.currentQuestion.capital.ToString();
        }
        else {
            answerAnimationText.text = gc.currentQuestion.name.ToString();
        }

        Color c = answerAnimationText.color;
        c.a = 1.0f;
        answerAnimationText.color = c;

        // Check this in case user keeps answering quickly while animation is active. 
        // Probably overkill but just leave it 
        if (!answerAnimationActive)
        {
            answerAnimationActive = true;
            answerAnimationDisplay.SetActive(true);
        }
    }

    public void InactivateAnswerAnimation()
    {
        if (answerAnimationActive)
        {
            answerAnimationActive = false;
            answerAnimationDisplay.SetActive(false);
        }
    }

    public void OnIncorrectAnswer(AnswerButton button) {
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
	
	public void GamePause(bool gamePaused) {
		pauseDisplay.SetActive(gamePaused);
	}	

    public void LoadScene(string sceneName)
    {
        sceneTransitions.LoadScene(sceneName);
    }

    public void EndRound()
    {
        scoreAnimationActive = false;
        UnloadRoundImages();
    }
    
    public string getImagePath(Question q)
    {
        string imagePath = "";
        if (gc.GetGameMode() == "states")
        {
            imagePath = "raw/state_images/";
        }
        else
        {
            imagePath = "raw/state_images_capital/";
        }
        string[] n = q.name.ToLower().Split();
        foreach (string s in n)
        {
            imagePath += s;
        }
        return imagePath;
    }

    public void LoadRoundImages(Question[] questionList)
    {
        RoundImageList = new List<Sprite>();
        foreach (Question q in questionList)
        {

            string path = getImagePath(q);
            Logger.Log("Loading image at path " + path);

            q.image = Resources.Load<Sprite>(getImagePath(q));
            RoundImageList.Add(q.image);
        }
    }

    public void UnloadRoundImages()
    {
        foreach (Sprite s in RoundImageList)
        {
            Logger.Log("Unloading asset " + s);
            Resources.UnloadAsset(s);
        }
    }

}
