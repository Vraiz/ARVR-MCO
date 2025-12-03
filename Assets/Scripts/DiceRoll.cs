using UnityEngine;
using TMPro;

public class DiceRoll : MonoBehaviour
{
    public TMP_Text displayText;
    public int diceSides = 20;
    
    private PerceptionCheck perceptionCheck;
    private MaterialResizeOnClick[] arcanaChecks;

    void Start()
    {
        if (displayText != null)
            displayText.text = "";
        
        FindInteractableChecks();
    }

    void FindInteractableChecks()
    {
        // Find all potential dice checkable objects
        perceptionCheck = FindAnyObjectByType<PerceptionCheck>();
        arcanaChecks = FindObjectsByType<MaterialResizeOnClick>(FindObjectsSortMode.None);
    }

    public void GenerateRandomNumber()
    {
        int randomNumber = Random.Range(1, diceSides + 1);
        
        if (displayText != null)
            displayText.text = randomNumber.ToString();
        
        Debug.Log($"Dice rolled: {randomNumber}");
        
        // Use UIManager to handle the dice roll
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HandleDiceRoll(randomNumber);
        }
        else
        {
            // Fallback handling
            HandleDiceRollFallback(randomNumber);
        }
    }

    void HandleDiceRollFallback(int randomNumber)
    {
        // Try MaterialResizeOnClick (Arcana Check) first
        if (arcanaChecks != null)
        {
            foreach (MaterialResizeOnClick check in arcanaChecks)
            {
                if (check.IsWaitingForRoll)
                {
                    Debug.Log($"Found waiting arcana check: {check.gameObject.name}");
                    check.ProcessDiceRoll(randomNumber);
                    return;
                }
            }
        }
        
        // Try PerceptionCheck
        if (perceptionCheck != null && perceptionCheck.IsWaitingForRoll)
        {
            perceptionCheck.ProcessDiceRoll();
            return;
        }
        
        Debug.LogWarning("Dice rolled but no waiting check found!");
    }

    public void SetPerceptionCheckName(string newName)
    {
        // This method is kept for compatibility with UIManager
        FindInteractableChecks();
    }
}