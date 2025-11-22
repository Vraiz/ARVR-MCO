using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FireToggle : MonoBehaviour
{
    public TMP_Text playerText;
    public Material onMaterial;
    public Material offMaterial;
    public Material fireMaterial;

    public string interactionText;

    public bool isLit = false;
    private Renderer rend;

    void ToggleFire()
    {
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
        if (Input.GetMouseButtonDown(0) && playerText.text == interactionText)
        {
            isLit = !isLit;
            ToggleFire();  
        }

    }


}
