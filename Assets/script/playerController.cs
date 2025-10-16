using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Leap;

public class playerController : MonoBehaviour
{
    // Existing variables for player and game state
    public float speed;
    private Rigidbody rb;
    private int count;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI winText;
    public GameUIManager uiManager;
    private string controlMode = "Hands";

    // Public variable for the Leap Provider
    public LeapProvider leapProvider;

    // Variables for the "grab and throw" functionality
    public float grabDistance = 1.0f;
    public float grabThreshold = 0.5f;
    public float throwForceMultiplier = 2.0f;
    public float releaseThreshold = 0.5f;
    private bool isHoldingPlayer = false;
    private Hand currentlyTrackedHand = null;
    public float elapsedTime = 0.0f;
    public TextMeshProUGUI timeText;
    public bool isTimerRunning = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
        winText.text = "";
    }

    void FixedUpdate()
    {
        if (controlMode == "Keyboard" && !isHoldingPlayer)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
            rb.AddForce(movement * speed);
        }
        
        if(isTimerRunning && timeText != null)
        {
            elapsedTime += Time.fixedDeltaTime;
            timeText.text = "Time Taken: " + elapsedTime.ToString("F2") + " s";
        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("pickup"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();

            FindAnyObjectByType<GameManager>().ActivateNextCollectible();
        }
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        uiManager.UpdateScore(count);
        if (count >= 12)
        {
            winText.text = "You Win!";
            isTimerRunning = false;
        }
    }

    private void Awake()
    {
        if (leapProvider == null)
        {
            leapProvider = FindObjectOfType<LeapProvider>();
            if (leapProvider == null)
            {
                Debug.Log("HandTracker: No LeapProvider found in the scene");
            }
        }
    }

    private void OnEnable()
    {
        if (leapProvider != null)
            leapProvider.OnUpdateFrame += HandleFrame;
    }

    private void OnDisable()
    {
        if (leapProvider != null)
            leapProvider.OnUpdateFrame -= HandleFrame;
    }

    private void HandleFrame(Frame frame)
    {
        if (frame == null) return;
        if (controlMode != "Hands") return;

        Hand activeHand = frame.Hands.Find(h => h.IsLeft || h.IsRight);

        if (isHoldingPlayer)
        {
            HandlePlayerHeld(activeHand);
        }
        else
        {
            HandlePlayerFree(activeHand);
        }
    }

    private void HandlePlayerFree(Hand hand)
    {
        if (hand == null) return;

        Vector3 handPalmPosition = new Vector3(hand.PalmPosition.x, hand.PalmPosition.y, hand.PalmPosition.z);

        // ADD THIS LINE
        Debug.Log("Distance: " + Vector3.Distance(transform.position, handPalmPosition) + ", Grab Strength: " + hand.GrabStrength);

        float distanceToPlayer = Vector3.Distance(transform.position, handPalmPosition);
        if (distanceToPlayer < grabDistance && hand.GrabStrength > grabThreshold)
        {
            GrabPlayer(hand);
        }
    }
    private void HandlePlayerHeld(Hand hand)
    {
        if (hand == null)
        {
            ThrowPlayer(null);
            return;
        }

        rb.transform.position = new Vector3(hand.PalmPosition.x, hand.PalmPosition.y, hand.PalmPosition.z);

        if (hand.GrabStrength < releaseThreshold)
        {
            ThrowPlayer(hand);
        }
    }

    private void GrabPlayer(Hand hand)
    {
        isHoldingPlayer = true;
        currentlyTrackedHand = hand;

        if (rb != null)
        {
            rb.isKinematic = true;
        }
        Debug.Log("Player Grabbed!");
    }

    private void ThrowPlayer(Hand hand)
    {
        isHoldingPlayer = false;

        if (rb != null)
        {
            rb.isKinematic = false;

            if (hand != null)
            {
                rb.velocity = new Vector3(hand.PalmVelocity.x, hand.PalmVelocity.y, hand.PalmVelocity.z) * throwForceMultiplier;
            }
        }

        currentlyTrackedHand = null;
        Debug.Log("Player Thrown!");
    }
    public void EnableControlMode(string mode) 
    {
        controlMode = mode;
    }
    public void ResetScore()
    {
        count = 0;
        SetCountText();
    }
    public void ResetTime()
    {
        elapsedTime = 0.0f;
    }
}