using UnityEngine;

public class ObjectDisabler : MonoBehaviour
{
    [Header("Object to Disable/Enable")]
    public GameObject targetObject;
    
    [Header("Action Type")]
    public bool disableObject = true;
    
    public void ExecuteDisable()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(disableObject ? false : true);
            Debug.Log($"{(disableObject ? "Disabled" : "Enabled")}: {targetObject.name}");
        }
        else
        {
            Debug.LogWarning("ObjectDisabler: No target object assigned!");
        }
    }
    
    public void DisableTarget()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(false);
            Debug.Log("Disabled: " + targetObject.name);
        }
    }
    
    public void EnableTarget()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(true);
            Debug.Log("Enabled: " + targetObject.name);
        }
    }
    
    public void ToggleTarget()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(!targetObject.activeSelf);
            Debug.Log("Toggled: " + targetObject.name + " to " + targetObject.activeSelf);
        }
    }
}