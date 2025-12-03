using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Threading;
using System.Threading.Tasks;



public class AlchemyScript : MonoBehaviour
{
    
    public TMP_Text playerText;
    public TMP_Text consoleText;
    public Material onMaterial;
    public Material offMaterial;

    public string interactionText;
    

    public GameObject floatingPrefab;
    public GameObject floating20;
    public GameObject floating1;

    private Renderer rend;

    public int spawnCount = 5;
    public float floatSpeed = 1.5f;
    public float spawnSpread = 0.2f;
    private GameObject selectionText;
    private GameObject crossHairText;

    private bool isBroken = false;

    private System.Random rand = new System.Random();

    public ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private Camera arCamera;


    void Bubbles(GameObject sphere)
    {

        for (int i = 0; i < spawnCount*2; i++)
        {
            Vector3 spawnPos = transform.position +
                new Vector3(
                    Random.Range(-spawnSpread*1, spawnSpread*1),
                    0,
                    Random.Range(-spawnSpread*1, spawnSpread*1)
                );

            GameObject obj = Instantiate(sphere, spawnPos, Quaternion.identity);
            obj.AddComponent<FloatingUp>();
            obj.GetComponent<FloatingUp>().speed = floatSpeed;
        }
    }

    void Roll()
    {
        int roll = rand.Next(1, 21);

        if(roll == 20)
        {
            Bubbles(floating20);
            consoleText.text = $"({roll})" + "Critical Success you have brewed an elixir of eternal youth";
        } else if (roll >= 11)
        {
            int rollPotion = rand.Next(0, 6);
            string[] potion = { "healing", "giant strength", "invisibility", "flight", "heroism", "hair loss" };
            Bubbles(floatingPrefab);
            consoleText.text = $"({roll})" + " Success you have brewed a potion of " + potion[rollPotion];
        }else if (roll < 11 && roll > 1)
        {
            consoleText.text = $"({roll})" + " The reagents failed to activate the catalyst";
        }else if (roll == 1)
        {
            Bubbles(floating1);
            consoleText.text = $"({roll})" + "Critical Failure you've caused the reagents to congeal";
            isBroken = true;
        }
    }

    void Start()
    {
        selectionText = GameObject.Find("SelectionText");
        crossHairText = GameObject.Find("ConsoleText");

        if (selectionText != null)
        {
            playerText = selectionText.GetComponent<TextMeshProUGUI>();
        }

        if (consoleText != null)
        {
            consoleText = crossHairText.GetComponent<TextMeshProUGUI>();
        }
        rend = GetComponent<Renderer>();
        arCamera = Camera.main;
        
        offMaterial.DisableKeyword("_EMISSION");
        StartCoroutine(AnimateEmission());
    }

    private IEnumerator AnimateEmission()
    {
        while (true)
        {
            offMaterial.DisableKeyword("_EMISSION");
            yield return new WaitForSeconds(0.5f);
            offMaterial.EnableKeyword("_EMISSION");
            yield return new WaitForSeconds(0.5f);
        }
    }
    void OnMouseEnter()
    {
        if(isBroken == false)
        {
            playerText.text = interactionText;
        }else if(isBroken == true)
        {
            playerText.text = "The reagents have been ruined you can no longer interact with the table";
        }
        rend.material = onMaterial;
        

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

        if (raycastManager == null)
        {
            raycastManager = FindObjectOfType<ARRaycastManager>();
        }


        if (Input.GetMouseButtonDown(0) && playerText.text == interactionText && isBroken == false)
        {
            Roll();
        }

        if (consoleText == null)
        {
            crossHairText = GameObject.Find("ConsoleText");
            consoleText = crossHairText.GetComponent<TextMeshProUGUI>();
        }

        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;

            // Raycast
            Ray ray = arCamera.ScreenPointToRay(touch.position);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider != null && hit.collider.gameObject == this.gameObject)
                {
                    OnTapped();
                }
            }
        }
    }

    void OnTapped()
    {
        rend.material = onMaterial;
        playerText.text = interactionText;
    }
}

public class FloatingUp : MonoBehaviour
{
    public float speed = 1f;

    void Update()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;

        if (transform.position.y > 12f) // Prevent infinite objects in scene
            Destroy(gameObject);
    }
}

