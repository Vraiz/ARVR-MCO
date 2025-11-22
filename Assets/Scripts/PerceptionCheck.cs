using UnityEngine;
using TMPro;
using System.Collections;

public class PerceptionCheck : MonoBehaviour
{
    [Header("UI References - Will be found automatically")]
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
    
    // UI GameObject names to search for
    private const string PLAYER_TEXT_NAME = "PerceptionCheck";
    private const string INTERACTION_UI_NAME = "InteractionUI"; // Adjust if different
    private const string RESULT_TEXT_NAME = "ResultTextTMP";
    private const string DICE_ROLL_NAME = "DiceRoll";
    
    private Renderer rend;
    private bool isInteracting = false;
    private bool waitingForRoll = false;
    private Camera arCamera;
    private bool uiElementsFound = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        arCamera = Camera.main;
        
        // Find UI elements automatically
        FindUIElements();
        
        if (interactionUI != null)
            interactionUI.SetActive(false);
    }

    void FindUIElements()
    {
        // Find Player Text (TMP)
        GameObject playerTextObj = GameObject.Find(PLAYER_TEXT_NAME);
        if (playerTextObj != null)
        {
            playerText = playerTextObj.GetComponent<TMP_Text>();
            if (playerText == null)
            {
                Debug.LogWarning("Found " + PLAYER_TEXT_NAME + " but it has no TMP_Text component");
            }
        }
        else
        {
            Debug.LogWarning("Could not find Player Text object: " + PLAYER_TEXT_NAME);
        }

        // Find Interaction UI
        interactionUI = GameObject.Find(INTERACTION_UI_NAME);
        if (interactionUI == null)
        {
            Debug.LogWarning("Could not find Interaction UI: " + INTERACTION_UI_NAME);
        }

        // Find Result Text (TMP)
        GameObject resultTextObj = GameObject.Find(RESULT_TEXT_NAME);
        if (resultTextObj != null)
        {
            resultText = resultTextObj.GetComponent<TMP_Text>();
            if (resultText == null)
            {
                Debug.LogWarning("Found " + RESULT_TEXT_NAME + " but it has no TMP_Text component");
            }
        }
        else
        {
            Debug.LogWarning("Could not find Result Text object: " + RESULT_TEXT_NAME);
        }

        // Find Dice Roll component
        GameObject diceRollObj = GameObject.Find(DICE_ROLL_NAME);
        if (diceRollObj != null)
        {
            diceRoll = diceRollObj.GetComponent<DiceRoll>();
            if (diceRoll == null)
            {
                Debug.LogWarning("Found " + DICE_ROLL_NAME + " but it has no DiceRoll component");
            }
        }
        else
        {
            Debug.LogWarning("Could not find Dice Roll object: " + DICE_ROLL_NAME);
        }

        // Check if all essential elements were found
        uiElementsFound = (playerText != null && interactionUI != null && resultText != null && diceRoll != null);
        
        if (!uiElementsFound)
        {
            Debug.LogError("Some UI elements are missing! Perception check may not work properly.");
        }
        else
        {
            Debug.Log("All UI elements found successfully for PerceptionCheck");
        }
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
        if (!uiElementsFound)
        {
            Debug.LogWarning("UI elements not found, cannot start interaction");
            return;
        }

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
        if (!isInteracting && uiElementsFound)
        {
            rend.material = onMaterial;
            playerText.text = "Tap for Perception Check (DC: " + difficultyClass + ")";
        }
    }

    public void OnObjectUngazed()
    {
        if (!isInteracting && uiElementsFound)
        {
            rend.material = offMaterial;
            playerText.text = "";
        }
    }

    void StartInteraction()
    {
        if (!uiElementsFound) return;

        isInteracting = true;
        waitingForRoll = true;
        
        // Show UI
        if (interactionUI != null)
            interactionUI.SetActive(true);
        
        // Reset result text and wait for dice roll
        if (resultText != null)
        {
            resultText.text = "Tap the button to roll the dice!";
            resultText.color = Color.white;
        }
    }

    public void ProcessDiceRoll()
    {
        if (!waitingForRoll || !uiElementsFound) return;
        
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
            resultText.text = "Roll: " + diceRollResult + " (DC: " + difficultyClass + ")\n\n" + passText;
            resultText.color = Color.green;
        }
        else
        {
            resultText.text = "Roll: " + diceRollResult + " (DC: " + difficultyClass + ")\n\n" + failText;
            resultText.color = Color.red;
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
        
        if (interactionUI != null)
            interactionUI.SetActive(false);
        
        if (uiElementsFound)
        {
            rend.material = offMaterial;
            playerText.text = "";
            resultText.text = "";
        }
    }

    // Public method to manually set UI references if needed
    public void SetUIReferences(TMP_Text newPlayerText, GameObject newInteractionUI, TMP_Text newResultText, DiceRoll newDiceRoll)
    {
        playerText = newPlayerText;
        interactionUI = newInteractionUI;
        resultText = newResultText;
        diceRoll = newDiceRoll;
        uiElementsFound = (playerText != null && interactionUI != null && resultText != null && diceRoll != null);
    }
}