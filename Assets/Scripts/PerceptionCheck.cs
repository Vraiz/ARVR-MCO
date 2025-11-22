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
    public DiceRoll diceRoll;
    
    [Header("Perception Check Settings")]
    public int difficultyClass = 15;
    [TextArea(2, 4)]
    public string passText = "SUCCESS! You notice the portal hums with ancient magic and reveals hidden runes.";
    [TextArea(2, 4)]
    public string failText = "FAILURE! The portal remains mysterious, its secrets hidden from your sight.";
    public float interactionUIDisplayTime = 1f;
    public float resultDisplayTime = 3f;
    
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
        
        // UI should already be disabled in inspector, but ensure it's off
        if (interactionUI != null && interactionUI.activeInHierarchy)
            interactionUI.SetActive(false);
    }

    void Update()
    {
        // Touch input for AR
        if (!isInteracting && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            CheckForPerceptionObjectTouch(Input.GetTouch(0).position);
        }
    }

    void CheckForPerceptionObjectTouch(Vector2 touchPosition)
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

    // Visual feedback when object is being looked at
    public void OnObjectGazed()
    {
        if (!isInteracting && uiElementsSet)
        {
            rend.material = onMaterial;
            if (playerText != null)
                playerText.text = "Tap for Perception Check (DC: " + difficultyClass + ")";
        }
    }

    public void OnObjectUngazed()
    {
        if (!isInteracting && uiElementsSet)
        {
            rend.material = offMaterial;
            if (playerText != null)
                playerText.text = "";
        }
    }

    // ADD THIS METHOD TO FIX THE ERROR
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
        
        if (diceRollResult >= difficultyClass)
        {
            if (resultText != null)
            {
                resultText.text = "Roll: " + diceRollResult + " (DC: " + difficultyClass + ")\n\n" + passText;
                resultText.color = Color.green;
            }
        }
        else
        {
            if (resultText != null)
            {
                resultText.text = "Roll: " + diceRollResult + " (DC: " + difficultyClass + ")\n\n" + failText;
                resultText.color = Color.red;
            }
        }
        
        yield return new WaitForSeconds(interactionUIDisplayTime);
        
        // Hide interaction UI but keep result text visible
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
            playerText.text = "";
            resultText.text = "";
        }
    }

    void OnDestroy()
{
    // Unregister from UIManager when object is destroyed
    if (UIManager.Instance != null)
    {
        UIManager.Instance.UnregisterPerceptionCheck(this);
    }
}
}