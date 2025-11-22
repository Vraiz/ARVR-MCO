using UnityEngine;
using TMPro;
using System.Collections;


public class PerceptionCheck : MonoBehaviour
{
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
    public float interactionUIDisplayTime = 1f; // How long the interaction UI stays after roll
    public float resultDisplayTime = 3f; // How long the result text stays after roll

    public string[] clue = new string[5];
    
    private Renderer rend;
    private bool isInteracting = false;
    private bool waitingForRoll = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        interactionUI?.SetActive(false);
    }

    void OnMouseEnter()
    {
        if (!isInteracting)
        {
            rend.material = onMaterial;
            playerText.text = "Press E for Perception Check (DC: " + difficultyClass + ")";
        }
    }

    void OnMouseExit()
    {
        if (!isInteracting)
        {
            rend.material = offMaterial;
            playerText.text = "";
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && IsMouseOverObject() && !isInteracting)
        {
            StartInteraction();
        }

        if (isInteracting && Input.GetKeyDown(KeyCode.Escape))
        {
            EndInteraction();
        }
    }

    bool IsMouseOverObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.collider.gameObject == this.gameObject;
        }
        return false;
    }

    void StartInteraction()
    {
        isInteracting = true;
        waitingForRoll = true;
        
        // Show UI
        if (interactionUI != null)
            interactionUI.SetActive(true);
        
        // Freeze camera movement
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Disable camera movement script
        MonoBehaviour cameraScript = Camera.main.GetComponent<MonoBehaviour>();
        if (cameraScript != null)
            cameraScript.enabled = false;
        
        // Reset result text and wait for dice roll
        if (resultText != null)
        {
            resultText.text = "Click the button to roll the dice!";
            resultText.color = Color.white;
        }
        if (diceText != null)
        {
            diceText.text = "";
        } 
    }

    public void ProcessDiceRoll()
    {
        if (!waitingForRoll) return;
        
        // Get the rolled value from the DiceRoll component
        int rolledValue = GetDiceRollValue();
        
        // Process the result
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
        
        // Fallback: generate a random roll
        return Random.Range(1, 21);
    }

    IEnumerator DisplayResults(int diceRollResult)
    {
        waitingForRoll = false;
        
        // Determine success or failure and display appropriate text
        if (diceRollResult == 20)
        {
            resultText.text = "Roll: " + diceRollResult + " (DC: " + difficultyClass + ")\n\n" + passText + "\n\nCritical Success!";
            notification.text = "Password is Blue -> Yellow -> Red -> Green -> White";
            resultText.color = Color.yellow;
        }
        else if (diceRollResult >= difficultyClass)
        {
            int index = Random.Range(0, clue.Length);
            resultText.text = "Roll: " + diceRollResult + " (DC: " + difficultyClass + ")\n\n" + passText;
            notification.text = clue[index];
            resultText.color = Color.green;
        }
        else if(diceRollResult == 1)
        {
            resultText.text = "Roll: " + diceRollResult + " (DC: " + difficultyClass + ")\n\n" + failText + "\n\nCritical Failure!";
            notification.text = "The trophy laughs at you";
            resultText.color = Color.red;
        }
        else
        {
            resultText.text = "Roll: " + diceRollResult + " (DC: " + difficultyClass + ")\n\n" + failText;
            notification.text = "";
            resultText.color = Color.black;
        }
        
        // Wait for interaction UI display time, then hide the interaction UI
        yield return new WaitForSeconds(interactionUIDisplayTime);
        
        // Hide interaction UI but keep result text visible
        if (interactionUI != null)
            interactionUI.SetActive(false);
        
        // Wait for the remaining result display time
        yield return new WaitForSeconds(resultDisplayTime - interactionUIDisplayTime);
        
        // Close everything
        EndInteraction();
    }

    public void EndInteraction()
    {
        isInteracting = false;
        waitingForRoll = false;
        
        // Hide UI
        if (interactionUI != null)
            interactionUI.SetActive(false);
        
        // Restore camera movement
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Re-enable camera movement script
        MonoBehaviour cameraScript = Camera.main.GetComponent<MonoBehaviour>();
        if (cameraScript != null)
            cameraScript.enabled = true;
        
        // Reset materials and text
        rend.material = offMaterial;
        playerText.text = "";
        resultText.text = "";
    }
}