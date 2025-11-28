using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

public class ARGazeDetector : MonoBehaviour
{
    private Camera arCamera;
    private GameObject currentGazedObject;
    private ARRaycastManager arRaycastManager;

    void Start()
    {
        arCamera = Camera.main;
        arRaycastManager = FindAnyObjectByType<ARRaycastManager>();
    }

    void Update()
    {
        DetectGazedObject();
    }

    void DetectGazedObject()
    {
        // Use AR raycast for better AR detection
        var screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        var hits = new List<ARRaycastHit>();
        
        // First try AR raycast for real-world surfaces
        if (arRaycastManager.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            // AR raycast hit - you could add AR-specific logic here
        }
        
        // Then try physics raycast for virtual objects
        Ray ray = arCamera.ScreenPointToRay(screenCenter);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 10f))
        {
            if (hit.collider.gameObject != currentGazedObject)
            {
                // Handle previous object
                if (currentGazedObject != null)
                {
                    CallGazeExit(currentGazedObject);
                }
                
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

    void CallGazeEnter(GameObject obj)
    {
        // Try PerceptionCheck first
        PerceptionCheck perception = obj.GetComponent<PerceptionCheck>();
        if (perception != null)
        {
            perception.OnGazeEnter();
            return;
        }
        
        // Try other interactable types...
        PortalScript portal = obj.GetComponent<PortalScript>();
        if (portal != null)
        {
            portal.OnGazeEnter();
            return;
        }
        
        TrophyScript trophy = obj.GetComponent<TrophyScript>();
        if (trophy != null)
        {
            trophy.OnGazeEnter();
            return;
        }
        
        ClickDetector click = obj.GetComponent<ClickDetector>();
        if (click != null)
        {
            click.OnGazeEnter();
            return;
        }
        
        FireToggle fire = obj.GetComponent<FireToggle>();
        if (fire != null)
        {
            fire.OnGazeEnter();
            return;
        }
    }

    void CallGazeExit(GameObject obj)
    {
        PerceptionCheck perception = obj.GetComponent<PerceptionCheck>();
        if (perception != null)
        {
            perception.OnGazeExit();
            return;
        }
        
        PortalScript portal = obj.GetComponent<PortalScript>();
        if (portal != null)
        {
            portal.OnGazeExit();
            return;
        }
        
        TrophyScript trophy = obj.GetComponent<TrophyScript>();
        if (trophy != null)
        {
            trophy.OnGazeExit();
            return;
        }
        
        ClickDetector click = obj.GetComponent<ClickDetector>();
        if (click != null)
        {
            click.OnGazeExit();
            return;
        }
        
        FireToggle fire = obj.GetComponent<FireToggle>();
        if (fire != null)
        {
            fire.OnGazeExit();
            return;
        }
    }
}