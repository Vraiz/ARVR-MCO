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

public class FireToggle : MonoBehaviour
{
    public TMP_Text playerText;
    public Material onMaterial;
    public Material offMaterial;
    public Material fireMaterial;

    public string interactionText;

    public bool isLit = false;
    private Renderer rend;

    private GameObject selectionText;

    public ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private Camera arCamera;
    void ToggleFire()
    {
        isLit = !isLit;
        if (isLit == true)
        {
            rend.material = fireMaterial;
        }
        else
        {
            rend.material = offMaterial;
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
        if (isLit == true)
        {
            rend.material = fireMaterial;
            playerText.text = "";
        }
        else
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


        if (Input.GetMouseButtonDown(0) && playerText.text == interactionText)
        {
            ToggleFire();
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
        ToggleFire();
    }


}
