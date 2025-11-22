using UnityEngine;
using TMPro;

public class ClickDetector : MonoBehaviour
{
    public TMP_Text playerText;
    public Material onMaterial;
    public Material offMaterial;
    public string interactionText;
    
    private Renderer rend;
    private Camera arCamera;

    void Start()
    {
        rend = GetComponent<Renderer>();
        arCamera = Camera.main;
    }

    // AR Gaze methods
    public void OnGazeEnter()
    {
        rend.material = onMaterial;
        playerText.text = interactionText;
    }

    public void OnGazeExit()
    {
        rend.material = offMaterial;
        playerText.text = "";
    }
}