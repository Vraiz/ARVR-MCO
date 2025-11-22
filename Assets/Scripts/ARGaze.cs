using UnityEngine;

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
    }

    void DetectGazedObject()
    {
        // Ray from center of screen for AR gaze
        Ray ray = arCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
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