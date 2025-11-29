using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
public class ClickDetector : MonoBehaviour
{
    public TMP_Text playerText;
    public Material onMaterial;
    public Material offMaterial;

    public string interactionText;
    private Renderer rend;

    public ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();


    void Start()
    {
        rend = GetComponent<Renderer>();
    }

        
    void Update()
    {
        if (playerText == null) // Only search if not already assigned
        {
            playerText = FindObjectOfType<TextMeshPro>();
            if (playerText != null && playerText.gameObject.name == "SelectionText")
            {
                
            }
            else
            {
                playerText = null; // Reset if not the correct one
            }
        }

        if (raycastManager == null)
        {
            raycastManager = FindObjectOfType<ARRaycastManager>();
            if (raycastManager != null)
            {
                Debug.Log("Found ARRaycastManager");
            }
            else
            {
                Debug.Log("ARRaycastManager not found");
            }
        }

        if (Input.touchCount > 0) // Check if there's a touch
        {
            Touch touch = Input.GetTouch(0);

            // Convert touch position to a ray
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject) // Did we touch THIS object?
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        rend.material = onMaterial;
                        playerText.text = interactionText;
                    }
                    else if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
                    {
                        rend.material = onMaterial;
                        playerText.text = interactionText;
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        playerText.text = "";
                    }
                }
            }
        }
    }
    void OnMouseEnter()
    {
        rend.material = onMaterial;
        playerText.text = interactionText;
    }

    void OnMouseExit()
    {
        rend.material = offMaterial;
        playerText.text = "";
    }
}
