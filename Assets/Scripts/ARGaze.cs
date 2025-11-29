//ARGAZE.cs
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARGazeDetector : MonoBehaviour
{
    private Camera arCamera;
    private GameObject currentGazedObject;

    void Start()
    {
        arCamera = Camera.main;
    }

    void Update()
    {
        DetectGazedObject();
        
        // HANDLE CLICKS DIRECTLY HERE - SIMPLE!
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            HandleTap(Input.GetTouch(0).position);
        }
    }

    void DetectGazedObject()
    {
        // Your existing gaze detection code...
        Ray ray = arCamera.ScreenPointToRay(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 10f))
        {
            if (hit.collider.gameObject != currentGazedObject)
            {
                if (currentGazedObject != null) CallGazeExit(currentGazedObject);
                currentGazedObject = hit.collider.gameObject;
                CallGazeEnter(currentGazedObject);
            }
        }
        else
        {
            if (currentGazedObject != null)
            {
                CallGazeExit(currentGazedObject);
                currentGazedObject = null;
            }
        }
    }

    // SIMPLE TAP HANDLING
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
            
            // Try other clickable types...
            PerceptionCheck perception = hit.collider.GetComponent<PerceptionCheck>();
            if (perception != null)
            {
                // Perception check will handle its own click
                return;
            }
            
            ClickDetector click = hit.collider.GetComponent<ClickDetector>();
            if (click != null)
            {
                // ClickDetector will handle its own click  
                return;
            }
        }
    }

    // In ARGaze.cs, update the CallGazeEnter and CallGazeExit methods:

    void CallGazeEnter(GameObject obj)
    {
        MaterialResizeOnClick resize = obj.GetComponent<MaterialResizeOnClick>();
        if (resize != null)
        {
            resize.OnGazeEnter();
            return;
        }
        
        // Add PerceptionCheck gaze handling
        PerceptionCheck perception = obj.GetComponent<PerceptionCheck>();
        if (perception != null)
        {
            perception.OnGazeEnter();
            return;
        }
    }

    void CallGazeExit(GameObject obj)
    {
        MaterialResizeOnClick resize = obj.GetComponent<MaterialResizeOnClick>();
        if (resize != null)
        {
            resize.OnGazeExit();
            return;
        }
        
        // Add PerceptionCheck gaze handling
        PerceptionCheck perception = obj.GetComponent<PerceptionCheck>();
        if (perception != null)
        {
            perception.OnGazeExit();
            return;
        }
    }
}