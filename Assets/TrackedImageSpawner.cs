using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class TrackedImageSpawner : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager;
    public GameObject prefab;

    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        // Spawn new prefabs when tracked images are detected
        foreach (var trackedImage in args.added)
        {
            var obj = Instantiate(prefab, trackedImage.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
        }

        // Optionally, handle updated tracking (e.g., hide when not visible)
        foreach (var trackedImage in args.updated)
        {
            trackedImage.gameObject.SetActive(trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking);
        }

        // Remove objects for removed tracked images
        foreach (var trackedImage in args.removed)
        {
            Destroy(trackedImage.gameObject);
        }
    }
}

