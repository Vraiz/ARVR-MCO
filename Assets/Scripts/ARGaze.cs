using UnityEngine;

public class ARGazeDetector : MonoBehaviour
{
    public float gazeDistance = 10f;
    public LayerMask interactableLayer;
    
    private Camera arCamera;
    private PortalScript currentPortal;
    private PerceptionCheck currentPerceptionObject;

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
        Ray ray = new Ray(arCamera.transform.position, arCamera.transform.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, gazeDistance, interactableLayer))
        {
            // Check what type of object we're looking at
            PortalScript portal = hit.collider.GetComponent<PortalScript>();
            PerceptionCheck perceptionObj = hit.collider.GetComponent<PerceptionCheck>();
            
            if (portal != null && portal != currentPortal)
            {
                if (currentPortal != null) currentPortal.OnPortalUngazed();
                currentPortal = portal;
                portal.OnPortalGazed();
            }
            else if (perceptionObj != null && perceptionObj != currentPerceptionObject)
            {
                if (currentPerceptionObject != null) currentPerceptionObject.OnObjectUngazed();
                currentPerceptionObject = perceptionObj;
                perceptionObj.OnObjectGazed();
            }
        }
        else
        {
            // Not looking at any interactable object
            if (currentPortal != null)
            {
                currentPortal.OnPortalUngazed();
                currentPortal = null;
            }
            if (currentPerceptionObject != null)
            {
                currentPerceptionObject.OnObjectUngazed();
                currentPerceptionObject = null;
            }
        }
    }
}