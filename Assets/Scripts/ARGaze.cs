using UnityEngine;

public class ARGazeDetector : MonoBehaviour
{
    public DiceRoll diceRoll; // Assign in inspector
    
    private Camera arCamera;
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
        
        if (Physics.Raycast(ray, out hit, 10f))
        {
            PerceptionCheck perceptionObj = hit.collider.GetComponent<PerceptionCheck>();
            
            if (perceptionObj != null && perceptionObj != currentPerceptionObject)
            {
                currentPerceptionObject = perceptionObj;
                
                // Connect the dice roll to this perception check by NAME
                if (diceRoll != null && UIManager.Instance != null)
                {
                    UIManager.Instance.SetDiceRollTarget(perceptionObj.gameObject.name);
                }
                
                perceptionObj.OnObjectGazed();
            }
        }
        else
        {
            if (currentPerceptionObject != null)
            {
                currentPerceptionObject.OnObjectUngazed();
                currentPerceptionObject = null;
            }
        }
    }
}