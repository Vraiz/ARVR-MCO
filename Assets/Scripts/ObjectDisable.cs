//ObjectDisable.cs
using UnityEngine;

public class ObjectDisabler : MonoBehaviour
{
    [Header("Object to Disable/Enable")]
    public GameObject targetObject;
    
    [Header("Action Type")]
    public bool disableObject = true;
    
    // Simple method that works with Unity's Button OnClick()
    public void ExecuteDisable()
    {
        if (targetObject != null)
        {
            if (disableObject)
            {
                targetObject.SetActive(false);
                Debug.Log("Disabled: " + targetObject.name);
            }
            else
            {
                targetObject.SetActive(true);
                Debug.Log("Enabled: " + targetObject.name);
            }
        }
        else
        {
            Debug.LogWarning("ObjectDisabler: No target object assigned!");
        }
    }
    
    // Alternative: Direct disable (no condition)
    public void DisableTarget()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(false);
            Debug.Log("Disabled: " + targetObject.name);
        }
    }
    
    // Alternative: Direct enable
    public void EnableTarget()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(true);
            Debug.Log("Enabled: " + targetObject.name);
        }
    }
    
    // Toggle between active states
    public void ToggleTarget()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(!targetObject.activeSelf);
            Debug.Log("Toggled: " + targetObject.name + " to " + targetObject.activeSelf);
        }
    }
}