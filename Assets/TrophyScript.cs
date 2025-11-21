using UnityEngine;
using TMPro;
using System.Threading.Tasks;


public class TrophyScript : MonoBehaviour
{
    public TMP_Text playerText;
    public Material onMaterial;
    public Material offMaterial;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }
    void OnMouseEnter()
    {
        rend.material = onMaterial;
        playerText.text = "Interact with portal?";
    }

    void OnMouseExit()
    {
        rend.material = offMaterial;
        playerText.text = "";
    }
}
