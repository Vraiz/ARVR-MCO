using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClickDetector : MonoBehaviour
{
    public TMP_Text playerText;
    public Material onMaterial;
    public Material offMaterial;

    public string interactionText;
    private Renderer rend;

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
        rend.material = offMaterial;
        playerText.text = "";
    }
}
