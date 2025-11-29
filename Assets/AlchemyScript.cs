using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class AlchemyScript : MonoBehaviour
{
    
    public TMP_Text playerText;
    public Material onMaterial;
    public Material offMaterial;

    public string interactionText;

    public GameObject floatingPrefab;

    private Renderer rend;

    public int spawnCount = 5;
    public float floatSpeed = 1.5f;
    public float spawnSpread = 0.2f;
    private GameObject selectionText;


    void Bubbles()
    {
        Debug.Log("bubbles");
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPos = transform.position +
                new Vector3(
                    Random.Range(-spawnSpread, spawnSpread),
                    0,
                    Random.Range(-spawnSpread, spawnSpread)
                );

            GameObject obj = Instantiate(floatingPrefab, spawnPos, Quaternion.identity);
            obj.AddComponent<FloatingUp>();
            obj.GetComponent<FloatingUp>().speed = floatSpeed;
        }
    }

    void Start()
    {
        selectionText = GameObject.Find("SelectionText");

        if (selectionText == null)
        {
        } else
        {
            playerText = selectionText.GetComponent<TextMeshProUGUI>();
        }
        rend = GetComponent<Renderer>();
    }
    void OnMouseEnter()
    {
        rend.material = onMaterial;
        playerText.text = interactionText;

    }

    void OnMouseExit()
    {
        {
            rend.material = offMaterial;
            playerText.text = "";
        }

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && playerText.text == interactionText)
        {
            Bubbles();
        }
    }
}

public class FloatingUp : MonoBehaviour
{
    public float speed = 1f;

    void Update()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;

        if (transform.position.y > 20f) // Prevent infinite objects in scene
            Destroy(gameObject);
    }
}