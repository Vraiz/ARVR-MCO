using UnityEngine;

public interface IDiceCheckable
{
    int DifficultyClass { get; }
    string CheckType { get; }
    void ProcessDiceRoll(int result);
    bool IsWaitingForRoll { get; set; }
    
    // Helper method to get transform for UI positioning
    Transform GetTransform();
}

// Extension method to implement GetTransform
public static class DiceCheckableExtensions
{
    public static Transform GetTransform(this IDiceCheckable diceCheckable)
    {
        if (diceCheckable is MonoBehaviour monoBehaviour)
        {
            return monoBehaviour.transform;
        }
        return null;
    }
}