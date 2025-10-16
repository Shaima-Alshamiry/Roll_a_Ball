using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject[] collectibles;
    public GameObject[] tutorialCollectibles;
    private int currentIndex = 0;
    private int tutorialIndex = 0;
    public GameObject startStudyButton;
    public bool isTutorial = true;
    public TextMeshProUGUI tutorialText;

    void Start()
    {
        // Deactivate all collectibles initially
        foreach (GameObject c in tutorialCollectibles)
        {
            c.SetActive(false);
        }
        foreach (GameObject c in collectibles)
        {
            c.SetActive(false);
        }

        // Activate the first collectible
        if (collectibles.Length > 0)
            collectibles[0].SetActive(true);
    }

    public void ActivateNextCollectible()
    {
        if (isTutorial)
        {
            // Deactivate current tutorial collectible
            if (tutorialIndex < tutorialCollectibles.Length)
                tutorialCollectibles[tutorialIndex].SetActive(false);

            tutorialIndex++;

            if (tutorialIndex < tutorialCollectibles.Length)
            {
                // Activate next tutorial collectible
                tutorialCollectibles[tutorialIndex].SetActive(true);
            }
            else
            {
                // Tutorial finished → show Start Study button
                startStudyButton.SetActive(true);
            }
        }
        else
        {
            // Main study sequence (12 collectibles) – same as before
            if (currentIndex < collectibles.Length)
                collectibles[currentIndex].SetActive(false);

            currentIndex++;

            if (currentIndex < collectibles.Length)
                collectibles[currentIndex].SetActive(true);
            else
                Debug.Log("All collectibles collected!");
        }
    }

    public void StartStudy()
    {
        isTutorial = false;
        startStudyButton.SetActive(false);  // Hide button

        if (tutorialText != null)
            tutorialText.gameObject.SetActive(false);

        // Activate first main collectible
        if (collectibles.Length > 0)
            collectibles[0].SetActive(true);

        playerController pc = FindAnyObjectByType<playerController>();
        if (pc != null) { 
            pc.ResetScore();
            pc.elapsedTime = 0.0f;
            pc.isTimerRunning = true;
            if (pc.timeText != null)
                pc.timeText.text = "Time Taken: 0.0 s";
        }
    }
}