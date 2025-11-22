using UnityEngine;
using TMPro;

public class FireToggle : MonoBehaviour
{
    public TMP_Text playerText;
    public Material onMaterial;
    public Material offMaterial;
    public Material fireMaterial;
    public string interactionText;
    public bool isLit = false;
    
    private Renderer rend;
    private Camera arCamera;
    private bool isGazed = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        arCamera = Camera.main;
    }

    void Update()
    {
        // AR Touch input
        if (isGazed && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            ToggleFire();
        }
    }

    void ToggleFire()
    {
        isLit = !isLit;
        if (isLit)
        {
            rend.material = fireMaterial;
        }
        else
        {
            rend.material = offMaterial;
        }
    }

    // AR Gaze methods
    public void OnGazeEnter()
    {
        isGazed = true;
        rend.material = onMaterial;
        playerText.text = interactionText;
    }

    public void OnGazeExit()
    {
        isGazed = false;
        if (isLit)
        {
            rend.material = fireMaterial;
        }
        else
        {
            rend.material = offMaterial;
        }
        playerText.text = "";
    }
}