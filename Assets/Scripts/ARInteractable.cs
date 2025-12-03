using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;
using TMPro;

public abstract class ARInteractable : MonoBehaviour
{
    [Header("Common AR Settings")]
    public TMP_Text playerText;
    public Material onMaterial;
    public Material offMaterial;
    public string interactionText;
    
    protected Renderer rend;
    protected Camera arCamera;
    protected XROrigin xrOrigin;
    protected bool isGazed = false;
    protected bool isInteracting = false;

    protected virtual void Start()
    {
        rend = GetComponent<Renderer>();
        FindARComponents();
    }

    protected void FindARComponents()
    {
        xrOrigin = FindAnyObjectByType<XROrigin>();
        if (xrOrigin != null)
        {
            arCamera = xrOrigin.Camera;
        }
        
        if (arCamera == null)
        {
            arCamera = Camera.main;
        }
    }

    // Common gaze methods
    public virtual void OnGazeEnter()
    {
        isGazed = true;
        rend.material = onMaterial;
        if (playerText != null)
            playerText.text = interactionText;
    }

    public virtual void OnGazeExit()
    {
        isGazed = false;
        rend.material = offMaterial;
        if (playerText != null)
            playerText.text = "";
    }

    protected virtual void Update()
    {
        if (isGazed && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            HandleInteraction();
        }
    }

    // This must be public since it's abstract and will be called externally
    public abstract void HandleInteraction();
}