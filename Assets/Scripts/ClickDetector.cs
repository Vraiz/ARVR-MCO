using UnityEngine;
using TMPro;

public class ClickDetector : ARInteractable
{
    [Header("Tap Settings")]
    public float tapTextDisplayTime = 2f;
    
    // Changed from protected to public to match base class
    public override void HandleInteraction()
    {
        OnObjectTapped();
    }

    void OnObjectTapped()
    {
        Debug.Log("ClickDetector tapped: " + gameObject.name);
        
        // Toggle material
        rend.material = (rend.material == onMaterial) ? offMaterial : onMaterial;
        
        ShowFeedback();
    }

    void ShowFeedback()
    {
        if (UIManager.Instance?.perceptionResultText != null)
        {
            UIManager.Instance.ShowMessage($"Interacted with: {interactionText}", Color.blue, tapTextDisplayTime);
        }
    }

    public override void OnGazeExit()
    {
        base.OnGazeExit();
        if (UIManager.Instance?.perceptionResultText != null && !isGazed)
        {
            UIManager.Instance.perceptionResultText.text = "";
        }
    }
}