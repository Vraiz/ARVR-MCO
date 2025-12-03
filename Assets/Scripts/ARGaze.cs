using UnityEngine;

public class ARGazeDetector : MonoBehaviour
{
    private Camera arCamera;
    private GameObject currentGazedObject;
    private ARInteractable currentInteractable;
    private MaterialResizeOnClick currentMaterialResize;
    private PerceptionCheck currentPerceptionCheck;

    void Start()
    {
        arCamera = Camera.main;
    }

    void Update()
    {
        DetectGazedObject();
        
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            HandleTap(Input.GetTouch(0).position);
        }
    }

    void DetectGazedObject()
    {
        Ray ray = arCamera.ScreenPointToRay(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 10f))
        {
            if (hit.collider.gameObject != currentGazedObject)
            {
                ClearCurrentGaze();
                currentGazedObject = hit.collider.gameObject;
                
                // Try ARInteractable first
                currentInteractable = currentGazedObject.GetComponent<ARInteractable>();
                if (currentInteractable != null)
                {
                    currentInteractable.OnGazeEnter();
                    return;
                }
                
                // Try MaterialResizeOnClick
                currentMaterialResize = currentGazedObject.GetComponent<MaterialResizeOnClick>();
                if (currentMaterialResize != null)
                {
                    currentMaterialResize.OnGazeEnter();
                    return;
                }
                
                // Try PerceptionCheck
                currentPerceptionCheck = currentGazedObject.GetComponent<PerceptionCheck>();
                if (currentPerceptionCheck != null)
                {
                    currentPerceptionCheck.OnGazeEnter();
                    return;
                }
            }
        }
        else
        {
            ClearCurrentGaze();
        }
    }

    void ClearCurrentGaze()
    {
        if (currentInteractable != null)
        {
            currentInteractable.OnGazeExit();
            currentInteractable = null;
        }
        
        if (currentMaterialResize != null)
        {
            currentMaterialResize.OnGazeExit();
            currentMaterialResize = null;
        }
        
        if (currentPerceptionCheck != null)
        {
            currentPerceptionCheck.OnGazeExit();
            currentPerceptionCheck = null;
        }
        
        currentGazedObject = null;
    }

    void HandleTap(Vector2 touchPosition)
    {
        Ray ray = arCamera.ScreenPointToRay(touchPosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("TAP DETECTED on: " + hit.collider.gameObject.name);
            
            // Try MaterialResizeOnClick first
            MaterialResizeOnClick resize = hit.collider.GetComponent<MaterialResizeOnClick>();
            if (resize != null)
            {
                resize.HandleClick();
                return;
            }
            
            // Try ARInteractable - call HandleInteraction which is now public
            ARInteractable interactable = hit.collider.GetComponent<ARInteractable>();
            if (interactable != null)
            {
                interactable.HandleInteraction();
                return;
            }
            
            // Try PerceptionCheck
            PerceptionCheck perception = hit.collider.GetComponent<PerceptionCheck>();
            if (perception != null)
            {
                perception.HandleInteraction();
                return;
            }
        }
    }
}