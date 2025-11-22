using UnityEngine;
using TMPro;

public class PortalScript : MonoBehaviour
{
    public TMP_Text playerText;
    public Material onMaterial;
    public Material offMaterial;
    public GameObject interactionUI;
    
    private Renderer rend;
    private bool isInteracting = false;
    private Camera arCamera;

    void Start()
    {
        rend = GetComponent<Renderer>();
        arCamera = Camera.main;
        if (interactionUI != null)
            interactionUI.SetActive(false);
    }

    void Update()
    {
        // AR Touch input
        if (!isInteracting && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            CheckForARTouch(Input.GetTouch(0).position);
        }
    }

    void CheckForARTouch(Vector2 touchPosition)
    {
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

    // AR Gaze methods
    public void OnGazeEnter()
    {
        if (!isInteracting)
        {
            rend.material = onMaterial;
            playerText.text = "Tap to interact with portal";
        }
    }

    public void OnGazeExit()
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
        
        if (interactionUI != null)
            interactionUI.SetActive(true);
    }

    public void EndInteraction()
    {
        isInteracting = false;
        
        if (interactionUI != null)
            interactionUI.SetActive(false);
        
        rend.material = offMaterial;
        playerText.text = "";
    }
}