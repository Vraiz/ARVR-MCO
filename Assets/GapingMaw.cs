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
using NUnit.Framework;

public class GapingMaw : MonoBehaviour
{

    public TMP_Text playerText;
    public TMP_Text consoleText;
    public Material onMaterial;
    public Material offMaterial;

    private GameObject selectionText;
    private GameObject crossHairText;

    private GameObject MawImage;
    private RawImage DeathImage;

    public string interactionText;

    private float triggerDistance = 1f;
    private System.Random rand = new System.Random();

    public ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private Renderer rend;

    private Camera arCamera;

    private bool hasrolled = false;
    private bool grossed = false;
    private Button MawButton;
    private int ClickCount = 0;

    IEnumerator Roll()
    {
        if (grossed == true)
        {
            consoleText.text = "Even the maw is repulsed by your taste it no longer wants to interact with you";
        }
        else
        {
            int roll = rand.Next(1, 21);
            if (roll >= 11)
            {
                if (hasrolled == true)
                {
                    consoleText.text = $"({roll})" + " Success you have spotted the maw";
                }else
                {
                    consoleText.text = $"({roll})" + " Success you have succesfully poked the bear or maw in this case and live!";
                }
                
            }else if (roll < 11 && roll > 1)
            {
                consoleText.text = $"({roll})" + " Failure You have been eaten by the maw but it does not have a taste for you so it spits you out";
                DeathImage.enabled = true;
                MawButton.enabled = true;
                yield return new WaitForSeconds(2.0f);
                DeathImage.enabled = false;
                MawButton.enabled = false;

            }else if (roll == 1)
            {
                grossed = true;
                consoleText.text = $"({roll})" + "Critical Failure: You fall into the maw but are instantly spat out (stinky)";
            }
        }
        
    }
    void Start()
    {
        selectionText = GameObject.Find("SelectionText");
        crossHairText = GameObject.Find("ConsoleText");
        MawImage = GameObject.Find("MawImage");

        if (selectionText != null)
        {
            playerText = selectionText.GetComponent<TextMeshProUGUI>();
        }

        if (consoleText != null)
        {
            consoleText = crossHairText.GetComponent<TextMeshProUGUI>();
        }

        if (MawImage != null)
        {
            DeathImage = MawImage.GetComponent<RawImage>();
            MawButton = MawImage.GetComponent<Button>();
            DeathImage.enabled = false;
            MawButton.enabled = false;
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
        if(hasrolled == true)
        {
            playerText.text = interactionText;
            rend.material = onMaterial;
        }
        

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
        
        if (arCamera == null)
        {
            arCamera = Camera.main;
        }

        float distance = Vector3.Distance(arCamera.transform.position, transform.position);
        if (distance <= triggerDistance && hasrolled == false)
        {
            hasrolled = true;
            StartCoroutine(Roll());
        }

        if (raycastManager == null)
        {
            raycastManager = FindObjectOfType<ARRaycastManager>();
        }


        if (Input.GetMouseButtonDown(0) && playerText.text == interactionText && hasrolled == true)
        {
            StartCoroutine(Roll());
        }

        if (consoleText == null)
        {
            crossHairText = GameObject.Find("ConsoleText");
            consoleText = crossHairText.GetComponent<TextMeshProUGUI>();
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

    public void TapImage()
    {
        if(ClickCount < 10)
        {
            ClickCount++;
        }else if(ClickCount >= 10)
        {
            DeathImage.enabled = false;
            MawButton.enabled = false;
            ClickCount = 0;
        }
    }

    void OnTapped()
    {
        rend.material = onMaterial;
        playerText.text = interactionText;
        

        if (hasrolled == true)
        {
            StartCoroutine(Roll());
        }
    }
}
