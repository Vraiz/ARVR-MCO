using UnityEngine;
using TMPro;

public class PortalScript : MonoBehaviour
{
    public TMP_Text playerText;
    public Material onMaterial;
    public Material offMaterial;
    public GameObject interactionUI; // Assign your UI panel in Inspector
    private Renderer rend;
    private bool isInteracting = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (interactionUI != null)
            interactionUI.SetActive(false);
    }

    void OnMouseEnter()
    {
        if (!isInteracting)
        {
            rend.material = onMaterial;
            playerText.text = "Interact with portal?";
        }
    }

    void OnMouseExit()
    {
        if (!isInteracting)
        {
            rend.material = offMaterial;
            playerText.text = "";
        }
    }

    void Update()
    {
        // Check if mouse is over object and E is pressed
        if (Input.GetKeyDown(KeyCode.E) && IsMouseOverObject())
        {
            StartInteraction();
        }

        // Optional: Add ESC key to exit interaction
        if (isInteracting && Input.GetKeyDown(KeyCode.Escape))
        {
            EndInteraction();
        }
    }

    bool IsMouseOverObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.collider.gameObject == this.gameObject;
        }
        return false;
    }

    void StartInteraction()
    {
        isInteracting = true;
        
        // Show UI
        if (interactionUI != null)
            interactionUI.SetActive(true);
        
        // Freeze camera movement
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // You might want to disable camera movement script here
        // Example: if you have a MouseLook script
        // Camera.main.GetComponent<MouseLook>().enabled = false;
    }

    public void EndInteraction()
    {
        isInteracting = false;
        
        // Hide UI
        if (interactionUI != null)
            interactionUI.SetActive(false);
        
        // Restore camera movement
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Re-enable camera movement script
        // Camera.main.GetComponent<MouseLook>().enabled = true;
        
        // Reset materials and text
        rend.material = offMaterial;
        playerText.text = "";
    }
}