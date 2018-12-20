using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CapitalsGameController : MonoBehaviour
{
    GameController gameController;
    private DisplayController displayController;

    public GameObject mainDisplay;
    public GameObject exitConfirmation;

    public Text pauseText;
    public Button endGameButton;
    public Button resumeButton;

    private bool exitConfirmationDialogDisplayed;

    // Use this for initialization
    void Awake()
    {
        //Logger.Log("***** STATESGAME CONTROLLER AWAKE *****");
        DataController dataController = FindObjectOfType<DataController>();
        dataController.LoadGameDataForScene("capitals");

        gameController = FindObjectOfType<GameController>();
        gameController.SetGameMode("capitals");

    }

    void Start()
    {
        //Logger.Log("***** STATESGAME CONTROLLER START *****");

        displayController = GetComponent<DisplayController>();

        gameController.displayController = displayController;
        gameController.StartRound();

    }

    public void OnEndGameButtonClick()
    {
        if (!exitConfirmationDialogDisplayed)
        {
            ExitConfirmationToggle(true);
        }
    }

    public void OnNoButtonClick()
    {
        ExitConfirmationToggle(false);
    }

    public void OnYesButtonClick()
    {
        ExitConfirmationToggle(false);
        ReturnToMenu();
    }


    private void ExitConfirmationToggle(bool displayed)
    {
        exitConfirmationDialogDisplayed = displayed;
        exitConfirmation.SetActive(displayed);
        endGameButton.interactable = !displayed;
        resumeButton.interactable = !displayed;

        float alpha;
        if (displayed)
        {
            alpha = 0.25f;
        }
        else
        {
            alpha = 1.0f;
        }
        Color c = pauseText.color;
        c.a = alpha;
        pauseText.color = c;

    }

    public void OnPauseButtonClick()
    {
        if (gameController.gamePaused)
        {
            gameController.GameUnpause();
        }
        else
        {
            gameController.GamePause();
        }
    }

    public void ReturnToMenu()
    {
        gameController.roundActive = false;
        gameController.gameActive = false;
        SceneManager.LoadScene("MenuScreen2Landscape");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
