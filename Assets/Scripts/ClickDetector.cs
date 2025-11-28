using UnityEngine;
using TMPro;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;

public class ClickDetector : MonoBehaviour
{
    [Header("UI and Visual Settings")]
    public TMP_Text playerText;
    public Material onMaterial;
    public Material offMaterial;
    public string interactionText = "Look at me!";
    
    [Header("Interaction Settings")]
    public bool enableTapInteraction = true;
    public float tapTextDisplayTime = 2f;
    
    private Renderer rend;
    private Camera arCamera;
    private XROrigin xrOrigin;
    private bool isGazed = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        FindARComponents();
    }

    void FindARComponents()
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

    void Update()
    {
        // AR Touch input - works independently of gaze (like PerceptionCheck)
        if (enableTapInteraction && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            CheckForARTouch(Input.GetTouch(0).position);
        }
    }

    void CheckForARTouch(Vector2 touchPosition)
    {
        if (arCamera == null) return;

        Ray ray = arCamera.ScreenPointToRay(touchPosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                OnObjectTapped();
            }
        }
    }

    void OnObjectTapped()
    {
        Debug.Log("ClickDetector tapped: " + gameObject.name);
        
        // Toggle material on tap - original functionality
        if (rend.material == onMaterial)
        {
            rend.material = offMaterial;
        }
        else
        {
            rend.material = onMaterial;
        }
        
        // Show tap feedback in text - original functionality
        if (playerText != null)
        {
            playerText.text = "Tapped: " + interactionText;
            
            // Clear the text after delay if not gazed
            if (!isGazed)
            {
                Invoke("ClearText", tapTextDisplayTime);
            }
        }
        
        // Also show feedback in UIManager's result text
        if (UIManager.Instance != null && UIManager.Instance.perceptionResultText != null)
        {
            UIManager.Instance.perceptionResultText.text = "Interacted with: " + interactionText;
            UIManager.Instance.perceptionResultText.color = Color.blue;
            
            // Clear the text after delay if not gazed
            if (!isGazed)
            {
                Invoke("ClearResultText", tapTextDisplayTime);
            }
        }
    }

    void ClearText()
    {
        if (playerText != null && !isGazed)
        {
            playerText.text = "";
        }
    }
    
    void ClearResultText()
    {
        if (UIManager.Instance != null && UIManager.Instance.perceptionResultText != null && !isGazed)
        {
            UIManager.Instance.perceptionResultText.text = "";
        }
    }

    // AR Gaze methods - original functionality
    public void OnGazeEnter()
    {
        isGazed = true;
        rend.material = onMaterial;
        if (playerText != null)
            playerText.text = interactionText;
    }

    public void OnGazeExit()
    {
        isGazed = false;
        rend.material = offMaterial;
        if (playerText != null)
            playerText.text = "";
            
        // Also clear result text when gaze exits
        if (UIManager.Instance != null && UIManager.Instance.perceptionResultText != null)
        {
            UIManager.Instance.perceptionResultText.text = "";
        }
    }
}