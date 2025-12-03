using UnityEngine;
using TMPro;

public class TrophyScript : ARInteractable
{
    // Changed from protected to public to match base class
    public override void HandleInteraction()
    {
        // Trophy-specific interaction
        Debug.Log("Trophy interacted with!");
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowMessage("You examine the trophy closely...", Color.yellow, 2f);
        }
    }
}