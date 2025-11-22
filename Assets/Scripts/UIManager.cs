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

    [Header("Dice Roll Target")]
    public string perceptionCheckName; // Just type the name of your PerceptionCheck object here

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

    void Start()
    {
        // Set the dice roll target by name
        if (perceptionDiceRoll != null && !string.IsNullOrEmpty(perceptionCheckName))
        {
            perceptionDiceRoll.SetPerceptionCheckName(perceptionCheckName);
        }
    }

    public void RegisterPerceptionCheck(PerceptionCheck perceptionCheck)
    {
        // Set the UI references for the perception check
        perceptionCheck.SetUIReferences(perceptionCheckText, perceptionInteractionUI, perceptionResultText, perceptionDiceRoll);
    }

    public void SetAllUIActive(bool active)
    {
        if (perceptionInteractionUI != null)
            perceptionInteractionUI.SetActive(active);
            
        // Clear text when hiding UI
        if (!active)
        {
            if (perceptionCheckText != null)
                perceptionCheckText.text = "";
            if (perceptionResultText != null)
                perceptionResultText.text = "";
        }
    }

    public void ShowInteractionUI()
    {
        SetAllUIActive(true);
    }

    public void HideInteractionUI()
    {
        SetAllUIActive(false);
    }
}