using UnityEngine;
using TMPro;
using UnityEngine.XR.ARFoundation;

public class PortalScript : MonoBehaviour
{
    public TMP_Text playerText;
    public Material onMaterial;
    public Material offMaterial;
    public GameObject interactionUI;
    private Renderer rend;
    private bool isInteracting = false;
    private ARRaycastManager raycastManager;
    private Camera arCamera;

    void Start()
    {
        rend = GetComponent<Renderer>();
        arCamera = Camera.main;
        raycastManager = FindFirstObjectByType<ARRaycastManager>();
        
        if (interactionUI != null)
            interactionUI.SetActive(false);
    }

    void Update()
    {
        // Handle touch input for AR
        if (!isInteracting && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            CheckForPortalTouch(Input.GetTouch(0).position);
        }
    }

    void CheckForPortalTouch(Vector2 touchPosition)
    {
        // Raycast to detect if portal was touched
        Ray ray = arCamera.ScreenPointToRay(touchPosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                StartInteraction();
            }
        }
    }

    // Visual feedback when portal is being looked at (optional for AR)
    public void OnPortalGazed()
    {
        if (!isInteracting)
        {
            rend.material = onMaterial;
            playerText.text = "Tap to interact with portal";
        }
    }

    public void OnPortalUngazed()
    {
        if (!isInteracting)
        {
            rend.material = offMaterial;
            playerText.text = "";
        }
    }

    void StartInteraction()
    {
        isInteracting = true;
        
        // Show UI
        if (interactionUI != null)
            interactionUI.SetActive(true);
        
        // For AR, we don't need to lock cursor
        // Just make sure UI is touch-friendly
    }

    public void EndInteraction()
    {
        isInteracting = false;
        
        // Hide UI
        if (interactionUI != null)
            interactionUI.SetActive(false);
        
        // Reset materials and text
        rend.material = offMaterial;
        playerText.text = "";
    }
}