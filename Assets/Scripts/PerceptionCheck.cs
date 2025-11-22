using UnityEngine;
using TMPro;
using System.Collections;

public class PerceptionCheck : MonoBehaviour
{
    [Header("UI References - Assign in Inspector")]
    public TMP_Text playerText;
    public Material onMaterial;
    public Material offMaterial;
    public GameObject interactionUI;
    public TMP_Text resultText;
    public TMP_Text notification;
    public TMP_Text diceText;
    public DiceRoll diceRoll;
    
    [Header("Perception Check Settings")]
    public int difficultyClass = 15;
    [TextArea(2, 4)]
    public string passText = "SUCCESS! You notice the portal hums with ancient magic and reveals hidden runes.";
    [TextArea(2, 4)]
    public string failText = "FAILURE! The portal remains mysterious, its secrets hidden from your sight.";
    public float interactionUIDisplayTime = 1f;
    public float resultDisplayTime = 3f;
    public string[] clue = new string[5];
    
    [Header("AR Settings")]
    public bool useGazeDetection = true;
    
    private Renderer rend;
    private bool isInteracting = false;
    private bool waitingForRoll = false;
    private Camera arCamera;
    private bool uiElementsSet = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        arCamera = Camera.main;
        
        // Register with UIManager to get UI references
        if (UIManager.Instance != null)
        {
            UIManager.Instance.RegisterPerceptionCheck(this);
        }
        
        interactionUI?.SetActive(false);
    }

    void Update()
    {
        // AR Touch input
        if (!isInteracting && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            CheckForARTouch(Input.GetTouch(0).position);
        }
    }

    void CheckForARTouch(Vector2 touchPosition)
    {
        Ray ray = arCamera.ScreenPointToRay(touchPosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                StartInteraction();
            }
        }
    }

    // AR Gaze methods
    public void OnGazeEnter()
    {
        if (!isInteracting && uiElementsSet)
        {
            rend.material = onMaterial;
            if (playerText != null)
                playerText.text = "Tap for Perception Check (DC: " + difficultyClass + ")";
        }
    }

    public void OnGazeExit()
    {
        if (!isInteracting && uiElementsSet)
        {
            rend.material = offMaterial;
            if (playerText != null)
                playerText.text = "";
        }
    }

    // ADD THIS METHOD TO FIX THE UIMANAGER ERROR
    public void SetUIReferences(TMP_Text newPlayerText, GameObject newInteractionUI, TMP_Text newResultText, DiceRoll newDiceRoll)
    {
        playerText = newPlayerText;
        interactionUI = newInteractionUI;
        resultText = newResultText;
        diceRoll = newDiceRoll;
        uiElementsSet = (playerText != null && interactionUI != null && resultText != null && diceRoll != null);
        
        if (uiElementsSet)
        {
            Debug.Log("UI references set successfully for PerceptionCheck");
        }
        else
        {
            Debug.LogWarning("Some UI references are null in PerceptionCheck");
        }
    }

    void StartInteraction()
    {
        if (!uiElementsSet) return;

        isInteracting = true;
        waitingForRoll = true;
        
        // Show UI via UIManager
        if (UIManager.Instance != null)
            UIManager.Instance.ShowInteractionUI();
        
        // Reset result text and wait for dice roll
        if (resultText != null)
        {
            resultText.text = "Tap the button to roll the dice!";
            resultText.color = Color.white;
        }
        if (diceText != null)
        {
            diceText.text = "";
        } 
    }

    public void ProcessDiceRoll()
    {
        if (!waitingForRoll || !uiElementsSet) return;
        
        int rolledValue = GetDiceRollValue();
        StartCoroutine(DisplayResults(rolledValue));
    }

    int GetDiceRollValue()
    {
        if (diceRoll != null && diceRoll.displayText != null)
        {
            if (int.TryParse(diceRoll.displayText.text, out int result))
            {
                return result;
            }
        }
        
        return Random.Range(1, 21);
    }

    IEnumerator DisplayResults(int diceRollResult)
    {
        waitingForRoll = false;
        
        if (diceRollResult == 20)
        {
            resultText.text = "Roll: " + diceRollResult + " (DC: " + difficultyClass + ")\n\n" + passText + "\n\nCritical Success!";
            if (notification != null) notification.text = "Password is Blue -> Yellow -> Red -> Green -> White";
            resultText.color = Color.yellow;
        }
        else if (diceRollResult >= difficultyClass)
        {
            int index = Random.Range(0, clue.Length);
            resultText.text = "Roll: " + diceRollResult + " (DC: " + difficultyClass + ")\n\n" + passText;
            if (notification != null) notification.text = clue[index];
            resultText.color = Color.green;
        }
        else if(diceRollResult == 1)
        {
            resultText.text = "Roll: " + diceRollResult + " (DC: " + difficultyClass + ")\n\n" + failText + "\n\nCritical Failure!";
            if (notification != null) notification.text = "The trophy laughs at you";
            resultText.color = Color.red;
        }
        else
        {
            resultText.text = "Roll: " + diceRollResult + " (DC: " + difficultyClass + ")\n\n" + failText;
            if (notification != null) notification.text = "";
            resultText.color = Color.black;
        }
        
        yield return new WaitForSeconds(interactionUIDisplayTime);
        
        if (interactionUI != null)
            interactionUI.SetActive(false);
        
        yield return new WaitForSeconds(resultDisplayTime - interactionUIDisplayTime);
        
        EndInteraction();
    }

    public void EndInteraction()
    {
        isInteracting = false;
        waitingForRoll = false;
        
        // Hide UI via UIManager
        if (UIManager.Instance != null)
            UIManager.Instance.HideInteractionUI();
        
        if (uiElementsSet)
        {
            rend.material = offMaterial;
            if (playerText != null) playerText.text = "";
            if (resultText != null) resultText.text = "";
        }
    }
}