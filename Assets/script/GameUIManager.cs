using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject hudPanel;
    public Button keyboardButton;
    public Button handsButton;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI modeText;
    public playerController playerController;
    public TextMeshProUGUI tutorialText;

    private string currentMode = "";

    void Start()
    {
        // Make sure only menu is visible at start
        menuPanel.SetActive(true);
        hudPanel.SetActive(false);

        // Disable player movement initially
        playerController.enabled = false;

        // Button listeners
        keyboardButton.onClick.AddListener(() => StartGame("Keyboard"));
        handsButton.onClick.AddListener(() => StartGame("Hands"));
    }

    public void StartGame(string mode)
    {
        currentMode = mode;
        modeText.text = "Mode: " + mode;

        menuPanel.SetActive(false);
        hudPanel.SetActive(true);

        // Tell player which control mode to use
        playerController.EnableControlMode(mode);

        playerController.enabled = true;
    }

    // Called from playerController whenever score changes
    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score;
    }
}