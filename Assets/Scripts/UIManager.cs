using UnityEngine;
using TMPro;
using System.Collections.Generic;

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

    private List<PerceptionCheck> activePerceptionChecks = new List<PerceptionCheck>();

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
        if (perceptionCheck == null) return;

        // Add to active list
        if (!activePerceptionChecks.Contains(perceptionCheck))
        {
            activePerceptionChecks.Add(perceptionCheck);
        }

        // Set the UI references for the perception check
        perceptionCheck.SetUIReferences(perceptionCheckText, perceptionInteractionUI, perceptionResultText, perceptionDiceRoll);
        
        // Set the dice roll's perception check reference to the most recent one
        // OR use a different approach (see below)
        if (perceptionDiceRoll != null)
        {
            perceptionDiceRoll.perceptionCheck = perceptionCheck;
        }
    }

    public void UnregisterPerceptionCheck(PerceptionCheck perceptionCheck)
    {
        if (perceptionCheck != null && activePerceptionChecks.Contains(perceptionCheck))
        {
            activePerceptionChecks.Remove(perceptionCheck);
        }
    }

    // New method to find PerceptionCheck by name
    public PerceptionCheck FindPerceptionCheckByName(string objectName)
    {
        foreach (var check in activePerceptionChecks)
        {
            if (check.gameObject.name == objectName || check.gameObject.name.Contains(objectName))
            {
                return check;
            }
        }
        return null;
    }

    // New method to set DiceRoll's target by name
    public void SetDiceRollTarget(string perceptionObjectName)
    {
        if (perceptionDiceRoll != null)
        {
            PerceptionCheck target = FindPerceptionCheckByName(perceptionObjectName);
            if (target != null)
            {
                perceptionDiceRoll.perceptionCheck = target;
                Debug.Log($"DiceRoll connected to: {perceptionObjectName}");
            }
            else
            {
                Debug.LogWarning($"PerceptionCheck object not found: {perceptionObjectName}");
            }
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