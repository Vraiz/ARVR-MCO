using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlaceCube : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    bool isPlacing = false;

    // Update is called once per frame
    void Update()
    {
        if (!raycastManager)
            return;

        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began || Input.GetMouseButtonDown(0)) && !isPlacing)
        {
           isPlacing = true;

           if (Input.touchCount > 0)
           {
               PlaceObject(Input.GetTouch(0).position);
           }
           else
           {
               PlaceObject(Input.mousePosition);
           }
        } 
    }



    void PlaceObject(Vector2 touchPosition)
    {
        var rayHits = new List <ARRaycastHit>();
        raycastManager.Raycast(touchPosition, rayHits, TrackableType.AllTypes);

        if (rayHits.Count > 0)
        {
            Vector3 hitPosition = rayHits[0].pose.position;
            Quaternion hitRotation = rayHits[0].pose.rotation;
            Instantiate(raycastManager.raycastPrefab, hitPosition, hitRotation);

        }

        StartCoroutine(SetIsPLacingToFalseWithDelay());
    }

    IEnumerator SetIsPLacingToFalseWithDelay()
    {
        yield return new WaitForSeconds(0.25f);
        isPlacing = false;
    }
}