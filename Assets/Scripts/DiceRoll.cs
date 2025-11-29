//DiceRoll.cs
using UnityEngine;
using TMPro;

public class DiceRoll : MonoBehaviour
{
    public TMP_Text displayText;
    public int diceSides = 20;
    public string perceptionCheckObjectName;
    
    private PerceptionCheck perceptionCheck;
    private MaterialResizeOnClick arcanaCheck;

    void Start()
    {
        if (displayText != null)
            displayText.text = "";
        
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
            }
        }
    }

    public void GenerateRandomNumber()
    {
        int randomNumber = Random.Range(1, diceSides + 1);
        
        if (displayText != null)
            displayText.text = randomNumber.ToString();
        
        Debug.Log($"Dice rolled: {randomNumber}");
        
        // Try MaterialResizeOnClick (Arcana Check) FIRST - check if any are waiting
        MaterialResizeOnClick[] arcanaChecks = FindObjectsByType<MaterialResizeOnClick>(FindObjectsSortMode.None);
        foreach (MaterialResizeOnClick check in arcanaChecks)
        {
            if (check.waitingForRoll)
            {
                Debug.Log($"Found waiting arcana check: {check.gameObject.name}");
                check.ProcessArcanaCheck(randomNumber);
                return;
            }
        }
        
        // Try PerceptionCheck second
        if (perceptionCheck != null && perceptionCheck.waitingForRoll)
        {
            perceptionCheck.ProcessDiceRoll();
            return;
        }
        
        // Fallback: try to find perception check again
        FindPerceptionCheckByName();
        if (perceptionCheck != null && perceptionCheck.waitingForRoll)
        {
            perceptionCheck.ProcessDiceRoll();
            return;
        }
        
        Debug.LogWarning("Dice rolled but no waiting check found!");
    }

    public void SetPerceptionCheckName(string newName)
    {
        perceptionCheckObjectName = newName;
        FindPerceptionCheckByName();
    }
}