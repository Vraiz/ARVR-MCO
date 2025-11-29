using UnityEngine;
using System.Collections;

public class AlchemyScript : MonoBehaviour
{
    public GameObject spherePrefab; // Assign a small transparent sphere prefab
    public int numberOfSpheres = 5; // How many spheres to spawn
    public float riseHeight = 2f; // How high they rise
    public float duration = 1f; // How long before they disappear
    public float spread = 0.5f; // Random horizontal spread

    void Update()
    {
        // Handle touch input
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            DetectTouch(Input.GetTouch(0).position);
        }

        // Handle mouse click for testing in Editor
        if (Input.GetMouseButtonDown(0))
        {
            DetectTouch(Input.mousePosition);
        }
    }

    void DetectTouch(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform == transform)
            {
                SpawnSpheres();
            }
        }
    }

    void SpawnSpheres()
    {
        for (int i = 0; i < numberOfSpheres; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-spread, spread),
                0,
                Random.Range(-spread, spread)
            );
            GameObject sphere = Instantiate(spherePrefab, transform.position + randomOffset, Quaternion.identity);
            StartCoroutine(RiseAndDisappear(sphere));
        }
    }

    IEnumerator RiseAndDisappear(GameObject sphere)
    {
        Vector3 startPos = sphere.transform.position;
        Vector3 endPos = startPos + Vector3.up * riseHeight;
        float elapsed = 0f;

        // Optional: fade out
        Renderer rend = sphere.GetComponent<Renderer>();
        Color startColor = rend.material.color;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            sphere.transform.position = Vector3.Lerp(startPos, endPos, t);
            if (rend != null)
                rend.material.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(startColor.a, 0, t));

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(sphere);
    }
}
