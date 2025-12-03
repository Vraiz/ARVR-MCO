using UnityEngine;
using TMPro;

public class FireToggle : ARInteractable
{
    [Header("Fire Settings")]
    public Material fireMaterial;
    public bool isLit = false;
    
    // Changed from protected to public to match base class
    public override void HandleInteraction()
    {
        ToggleFire();
    }

    void ToggleFire()
    {
        isLit = !isLit;
        rend.material = isLit ? fireMaterial : offMaterial;
    }

    public override void OnGazeExit()
    {
        base.OnGazeExit();
        if (isInteracting) return;
        rend.material = isLit ? fireMaterial : offMaterial;
    }
}