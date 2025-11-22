using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [Header("Perception Check UI Elements")]
    public TMP_Text perceptionCheckText;
    public GameObject perceptionInteractionUI;
    public TMP_Text perceptionResultText;
    public DiceRoll perceptionDiceRoll;

    [Header("Other UI Elements")]
    public GameObject mainUI; // Your main AR UI if any

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Disable all UI at start
        SetAllUIActive(false);
    }

    public void RegisterPerceptionCheck(PerceptionCheck perceptionCheck)
    {
        // Set the UI references for the perception check
        perceptionCheck.SetUIReferences(perceptionCheckText, perceptionInteractionUI, perceptionResultText, perceptionDiceRoll);
        
        // Set the dice roll's perception check reference
        if (perceptionDiceRoll != null)
        {
            perceptionDiceRoll.perceptionCheck = perceptionCheck;
        }
    }

    public void SetAllUIActive(bool active)
    {
        if (perceptionInteractionUI != null)
            perceptionInteractionUI.SetActive(active);
            
        if (mainUI != null)
            mainUI.SetActive(active);
            
        // Clear text when hiding UI
        if (!active)
        {
            if (perceptionCheckText != null)
                perceptionCheckText.text = "";
            if (perceptionResultText != null)
                perceptionResultText.text = "";
        }
    }

    // Call this when you want to show UI for a specific interaction
    public void ShowInteractionUI()
    {
        SetAllUIActive(true);
    }

    // Call this when interaction is complete
    public void HideInteractionUI()
    {
        SetAllUIActive(false);
    }
}