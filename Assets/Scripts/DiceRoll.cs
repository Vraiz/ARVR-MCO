using UnityEngine;
using TMPro;

public class DiceRoll : MonoBehaviour
{
    public TMP_Text displayText;
    public int diceSides = 20;
    public string perceptionCheckObjectName;
    
    private PerceptionCheck perceptionCheck;

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

    // SINGLE GenerateRandomNumber method - no duplicates!
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
            FindPerceptionCheckByName();
            if (perceptionCheck != null)
            {
                perceptionCheck.ProcessDiceRoll();
            }
        }
    }

    public void SetPerceptionCheckName(string newName)
    {
        perceptionCheckObjectName = newName;
        FindPerceptionCheckByName();
    }
}