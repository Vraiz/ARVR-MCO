using UnityEngine;
using TMPro;

public class DiceRoll : MonoBehaviour
{
    public TMP_Text displayText;
    public int diceSides = 20;
    public PerceptionCheck perceptionCheck;

    void Start()
    {
        // Clear display at start
        if (displayText != null)
            displayText.text = "";
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
    }
}