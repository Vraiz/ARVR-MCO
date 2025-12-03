using UnityEngine;
using TMPro;

public class DiceRoll : MonoBehaviour
{
    public TMP_Text displayText;
    public int diceSides = 20; // Default to d20
    public PerceptionCheck perceptionCheck; // Reference to the PerceptionCheck component

    // This method will be called by the button's OnClick event
    public void GenerateRandomNumber()
    {
        // Generate random number between 1-diceSides
        int randomNumber = Random.Range(1, diceSides + 1);
        
        // Update the text display
        displayText.text = randomNumber.ToString();
        
        // Notify the PerceptionCheck that a roll was made
        if (perceptionCheck != null)
        {
            perceptionCheck.ProcessDiceRoll();
        }
    }

    // Overloaded method for specific dice types
    public void GenerateRandomNumber(int sides)
    {
        int randomNumber = Random.Range(1, sides + 1);
        displayText.text = randomNumber.ToString();
        
        // Notify the PerceptionCheck that a roll was made
        if (perceptionCheck != null)
        {
            perceptionCheck.ProcessDiceRoll();
        }
    }
}