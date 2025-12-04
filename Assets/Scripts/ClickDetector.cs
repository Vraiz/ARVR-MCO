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
public class ClickDetector : MonoBehaviour
{
    public TMP_Text playerText;
    public Material onMaterial;
    public Material offMaterial;
    public string interactionText;
    private Renderer rend;
    public ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private Camera arCamera;
    public GameObject selectionText;

    

    void Start()
    {
        selectionText = GameObject.Find("SelectionText");
        if (selectionText != null)
        {
            playerText = selectionText.GetComponent<TextMeshProUGUI>();
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

    void Update()
    {
            
        if (raycastManager == null)
        {
            raycastManager = FindObjectOfType<ARRaycastManager>();
        }

        if (selectionText == null)
        {
            
            selectionText = GameObject.Find("SelectionText");
            if (selectionText != null)
            {

                playerText = selectionText.GetComponent<TextMeshProUGUI>();
            }
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
