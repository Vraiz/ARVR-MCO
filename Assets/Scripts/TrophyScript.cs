using UnityEngine;
using TMPro;

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

    // AR Gaze methods
    public void OnGazeEnter()
    {
        rend.material = onMaterial;
        playerText.text = "Interact with trophy?";
    }

    public void OnGazeExit()
    {
        rend.material = offMaterial;
        playerText.text = "";
    }
}