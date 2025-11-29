using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
public class ClickDetector : MonoBehaviour
{
    public TMP_Text playerText;
    public Material onMaterial;
    public Material offMaterial;

    public string interactionText;
    private Renderer rend;

    public ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    Camera cam;

    public GameObject selectionText;



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
        cam = Camera.main;
    }

        
    void Update()
    {


        if (raycastManager == null)
        {
            raycastManager = FindObjectOfType<ARRaycastManager>();
            if (raycastManager != null)
            {
            }
            else
            {
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    rend.material = onMaterial;
                    playerText.text = interactionText;
                }
            }
        }

    }
    void OnMouseEnter()
    {
        rend.material = onMaterial;
        playerText.text = interactionText;
    }

    void OnMouseExit()
    {
        rend.material = offMaterial;
        playerText.text = "";
    }
}
