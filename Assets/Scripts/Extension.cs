using UnityEngine;

public static class UnityExtensions
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