using UnityEngine;
using TMPro;

public class DiceRoll : MonoBehaviour
{
    public TMP_Text displayText;
    public int diceSides = 20;
    public string perceptionCheckObjectName; // Use name instead of direct reference
    
    private PerceptionCheck perceptionCheck;

    void Start()
    {
        // Clear display at start
        if (displayText != null)
            displayText.text = "";
        
        // Find PerceptionCheck by name
        FindPerceptionCheckByName();
    }

    void FindPerceptionCheckByName()
    {
        if (!string.IsNullOrEmpty(perceptionCheckObjectName))
        {
            GameObject perceptionObj = GameObject.Find(perceptionCheckObjectName);
            if (perceptionObj != null)
            {
                perceptionCheck = perceptionObj.GetComponent<PerceptionCheck>();
                if (perceptionCheck != null)
                {
                    Debug.Log($"DiceRoll connected to: {perceptionCheckObjectName}");
                }
                else
                {
                    Debug.LogWarning($"Found object '{perceptionCheckObjectName}' but it has no PerceptionCheck component");
                }
            }
            else
            {
                Debug.LogWarning($"PerceptionCheck object not found: {perceptionCheckObjectName}");
            }
        }
    }

    public void GenerateRandomNumber()
    {
        int randomNumber = Random.Range(1, diceSides + 1);
        
        if (displayText != null)
            displayText.text = randomNumber.ToString();
        
        if (perceptionCheck != null)
        {
            perceptionCheck.ProcessDiceRoll();
        }
        else
        {
            // Try to find it again in case it was instantiated after Start()
            FindPerceptionCheckByName();
            if (perceptionCheck != null)
            {
                perceptionCheck.ProcessDiceRoll();
            }
        }
    }

    public void GenerateRandomNumber(int sides)
    {
        int randomNumber = Random.Range(1, sides + 1);
        if (displayText != null)
            displayText.text = randomNumber.ToString();
        
        if (perceptionCheck != null)
        {
            perceptionCheck.ProcessDiceRoll();
        }
        else
        {
            FindPerceptionCheckByName();
            if (perceptionCheck != null)
            {
                perceptionCheck.ProcessDiceRoll();
            }
        }
    }

    // Public method to update the target name if needed
    public void SetPerceptionCheckName(string newName)
    {
        perceptionCheckObjectName = newName;
        FindPerceptionCheckByName();
    }
}