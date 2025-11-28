using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;

public class PerceptionCheck : MonoBehaviour
{
    [Header("UI References - Assign in Inspector")]
    public TMP_Text playerText;
    public Material onMaterial;
    public Material offMaterial;
    public GameObject interactionUI;
    public TMP_Text diceText;
    public DiceRoll diceRoll;
    
    [Header("Perception Check Settings")]
    public int difficultyClass = 15;
    [TextArea(2, 4)]
    public string passText = "SUCCESS! You notice the portal hums with ancient magic and reveals hidden runes.";
    [TextArea(2, 4)]
    public string failText = "FAILURE! The portal remains mysterious, its secrets hidden from your sight.";
    public float interactionUIDisplayTime = 1f;
    public float resultDisplayTime = 3f;
    public string[] clue = new string[5];
    
    [Header("AR Settings")]
    public bool useGazeDetection = true;
    
    private Renderer rend;
    private bool isInteracting = false;
    private bool waitingForRoll = false;
    private Camera arCamera;
    private bool uiElementsSet = false;
    private XROrigin xrOrigin;

    // In PerceptionCheck.cs - Add null checks
    void Start()
    {
        rend = GetComponent<Renderer>();
        FindARComponents();
        
        // Register with UIManager - but UIManager might not be ready yet
        if (UIManager.Instance != null)
        {
            UIManager.Instance.RegisterPerceptionCheck(this);
        }
        else
        {
            // Retry registration if UIManager isn't ready
            StartCoroutine(RegisterWhenReady());
        }
        
        interactionUI?.SetActive(false);
    }

    IEnumerator RegisterWhenReady()
    {
        while (UIManager.Instance == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        UIManager.Instance.RegisterPerceptionCheck(this);
    }
    void FindARComponents()
    {
        xrOrigin = FindAnyObjectByType<XROrigin>();
        if (xrOrigin != null)
        {
            arCamera = xrOrigin.Camera;
        }
        
        if (arCamera == null)
        {
            arCamera = Camera.main;
        }
    }

    void Update()
    {
        // AR Touch input
        if (!isInteracting && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            CheckForARTouch(Input.GetTouch(0).position);
        }
    }

    void CheckForARTouch(Vector2 touchPosition)
    {
        if (arCamera == null) return;

        Ray ray = arCamera.ScreenPointToRay(touchPosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                StartInteraction();
            }
        }
    }

    // AR Gaze methods
    public void OnGazeEnter()
    {
        if (!isInteracting && uiElementsSet)
        {
            rend.material = onMaterial;
            if (playerText != null)
                playerText.text = "Tap for Perception Check (DC: " + difficultyClass + ")";
        }
    }

    public void OnGazeExit()
    {
        if (!isInteracting && uiElementsSet)
        {
            rend.material = offMaterial;
            if (playerText != null)
                playerText.text = "";
        }
    }

    public void SetUIReferences(TMP_Text newPlayerText, GameObject newInteractionUI, TMP_Text newResultText, DiceRoll newDiceRoll)
    {
        playerText = newPlayerText;
        interactionUI = newInteractionUI;
        diceRoll = newDiceRoll;
        uiElementsSet = (playerText != null && interactionUI != null && diceRoll != null);
        
        if (uiElementsSet)
        {
            Debug.Log("UI references set successfully for PerceptionCheck");
        }
        else
        {
            Debug.LogWarning("Some UI references are null in PerceptionCheck");
        }
    }

    void StartInteraction()
    {
        if (!uiElementsSet) return;

        isInteracting = true;
        waitingForRoll = true;
        
        // Position UI in AR space
        if (UIManager.Instance != null)
        {
            UIManager.Instance.PositionUIInWorldSpace(this.transform);
            UIManager.Instance.ShowInteractionUI();
        }
        
        // Reset result text and wait for dice roll
        if (UIManager.Instance != null && UIManager.Instance.perceptionResultText != null)
        {
            UIManager.Instance.perceptionResultText.text = "Tap the button to roll the dice!";
            UIManager.Instance.perceptionResultText.color = Color.white;
        }
            
        if (diceText != null)
        {
            diceText.text = "";
        } 
    }

    public void ProcessDiceRoll()
    {
        if (!waitingForRoll || !uiElementsSet) return;
        
        int rolledValue = GetDiceRollValue();
        StartCoroutine(DisplayResults(rolledValue));
    }

    int GetDiceRollValue()
    {
        if (diceRoll != null && diceRoll.displayText != null)
        {
            if (int.TryParse(diceRoll.displayText.text, out int result))
            {
                return result;
            }
        }
        
        return Random.Range(1, 21);
    }

    IEnumerator DisplayResults(int diceRollResult)
    {
        waitingForRoll = false;
        
        string resultMessage = "";
        string clueMessage = "";
        Color resultColor = Color.white;
        
        if (diceRollResult == 20)
        {
            resultMessage = "Roll: " + diceRollResult + " (DC: " + difficultyClass + ")\n\n" + passText + "\n\nCritical Success!";
            clueMessage = "Password revealed: Blue -> Yellow -> Red -> Green -> White";
            resultColor = Color.yellow;
            
            // Find portal and reveal password - MORE ROBUST SEARCH
            PortalScript portal = FindAnyObjectByType<PortalScript>();
            if (portal != null)
            {
                Debug.Log("PERCEPTION: Found portal! Calling RevealPassword()");
                portal.RevealPassword();
            }
            else
            {
                Debug.LogError("PERCEPTION: No PortalScript found in scene!");
                // Try alternative search methods
                GameObject portalObj = GameObject.FindGameObjectWithTag("Portal");
                if (portalObj != null)
                {
                    portal = portalObj.GetComponent<PortalScript>();
                    if (portal != null)
                    {
                        Debug.Log("PERCEPTION: Found portal via tag! Calling RevealPassword()");
                        portal.RevealPassword();
                    }
                }
            }
        }
        else if (diceRollResult >= difficultyClass) 
        {
            int index = Random.Range(0, clue.Length);
            resultMessage = "Roll: " + diceRollResult + " (DC: " + difficultyClass + ")\n\n" + passText;
            clueMessage = "Clue: " + (index < clue.Length ? clue[index] : "No clue available");
            resultColor = Color.green;
        }
        else if(diceRollResult == 1)
        {
            resultMessage = "Roll: " + diceRollResult + " (DC: " + difficultyClass + ")\n\n" + failText + "\n\nCritical Failure!";
            clueMessage = "The trophy laughs at you";
            resultColor = Color.red;
        }
        else
        {
            resultMessage = "Roll: " + diceRollResult + " (DC: " + difficultyClass + ")\n\n" + failText;
            clueMessage = "No clues revealed";
            resultColor = Color.black;
        }
        
        // Display result using UIManager's result text
        if (UIManager.Instance != null && UIManager.Instance.perceptionResultText != null)
        {
            // Combine result and clue into one message
            string fullMessage = resultMessage + "\n\n" + clueMessage;
            UIManager.Instance.perceptionResultText.text = fullMessage;
            UIManager.Instance.perceptionResultText.color = resultColor;
        }
        
        yield return new WaitForSeconds(interactionUIDisplayTime);
        
        if (interactionUI != null)
            interactionUI.SetActive(false);
        
        yield return new WaitForSeconds(resultDisplayTime - interactionUIDisplayTime);
        
        EndInteraction();
    }

    public void EndInteraction()
    {
        isInteracting = false;
        waitingForRoll = false;
        
        // Hide UI via UIManager
        if (UIManager.Instance != null)
            UIManager.Instance.HideInteractionUI();
        
        if (uiElementsSet)
        {
            rend.material = offMaterial;
            if (playerText != null) playerText.text = "";
        }
    }
}