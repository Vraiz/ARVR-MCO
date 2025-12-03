using UnityEngine;
using TMPro;
using System.Collections;

public class PerceptionCheck : ARInteractable, IDiceCheckable
{
    [Header("Dice Check Settings")]
    public int difficultyClass = 15;
    public string checkType = "Perception Check";
    
    [TextArea(2, 4)]
    public string passText = "SUCCESS! You notice the portal hums with ancient magic and reveals hidden runes.";
    
    [TextArea(2, 4)]
    public string failText = "FAILURE! The portal remains mysterious, its secrets hidden from your sight.";
    
    public float interactionUIDisplayTime = 1f;
    public float resultDisplayTime = 3f;
    public string[] clue = new string[5];
    
    public int DifficultyClass => difficultyClass;
    public string CheckType => checkType;
    public bool IsWaitingForRoll { get; set; } = false;
    
    protected override void Start()
    {
        base.Start();
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.RegisterPerceptionCheck(this);
        }
        else
        {
            StartCoroutine(RegisterWhenReady());
        }
    }

    IEnumerator RegisterWhenReady()
    {
        while (UIManager.Instance == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
        UIManager.Instance.RegisterPerceptionCheck(this);
    }

    protected override void Update()
    {
        base.Update();
        
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
            if (hit.collider.gameObject == gameObject)
            {
                StartInteraction();
            }
        }
    }

    // Made public to match base class
    public override void HandleInteraction()
    {
        StartInteraction();
    }

    void StartInteraction()
    {
        if (isInteracting) return;
        
        isInteracting = true;
        IsWaitingForRoll = true;
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetRollType(checkType);
            UIManager.Instance.PositionUIInWorldSpace(transform);
            UIManager.Instance.ShowInteractionUI();
            
            if (UIManager.Instance.perceptionResultText != null)
            {
                UIManager.Instance.perceptionResultText.text = "Tap the button to roll for " + checkType + "!";
                UIManager.Instance.perceptionResultText.color = Color.white;
            }
        }
    }

    public void ProcessDiceRoll()
    {
        if (!IsWaitingForRoll || !isInteracting) return;
        
        int rolledValue = GetDiceRollValue();
        StartCoroutine(DisplayResults(rolledValue));
    }

    public void ProcessDiceRoll(int diceRoll)
    {
        if (!IsWaitingForRoll || !isInteracting) return;
        
        StartCoroutine(DisplayResults(diceRoll));
    }

    int GetDiceRollValue()
    {
        if (UIManager.Instance != null && UIManager.Instance.perceptionDiceRoll != null)
        {
            var diceRoll = UIManager.Instance.perceptionDiceRoll;
            if (diceRoll.displayText != null && int.TryParse(diceRoll.displayText.text, out int result))
            {
                return result;
            }
        }
        
        return Random.Range(1, 21);
    }

    IEnumerator DisplayResults(int diceRollResult)
    {
        IsWaitingForRoll = false;
        isInteracting = false;
        
        string resultMessage = "";
        string clueMessage = "";
        Color resultColor = Color.white;
        
        if (diceRollResult == 20)
        {
            resultMessage = "Critical Success! " + passText;
            clueMessage = "You notice every detail with perfect clarity!";
            resultColor = Color.yellow;
        }
        else if (diceRollResult >= difficultyClass) 
        {
            int index = Random.Range(0, clue.Length);
            resultMessage = passText;
            clueMessage = "Clue: " + (index < clue.Length ? clue[index] : "No clue available");
            resultColor = Color.green;
        }
        else if(diceRollResult == 1)
        {
            resultMessage = "Critical Failure! " + failText;
            clueMessage = "You completely miss the obvious!";
            resultColor = Color.red;
        }
        else
        {
            resultMessage = failText;
            clueMessage = "No clues revealed";
            resultColor = Color.gray;
        }
        
        if (UIManager.Instance != null)
        {
            string fullMessage = $"{checkType}: Rolled {diceRollResult} vs DC {difficultyClass}\n\n{resultMessage}\n\n{clueMessage}";
            UIManager.Instance.ShowMessage(fullMessage, resultColor, 4f);
        }
        
        yield return new WaitForSeconds(resultDisplayTime);
        
        EndInteraction();
    }

    public void EndInteraction()
    {
        isInteracting = false;
        IsWaitingForRoll = false;
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideInteractionUI();
            UIManager.Instance.ClearRollType();
        }
        
        if (isGazed)
        {
            rend.material = onMaterial;
        }
        else
        {
            rend.material = offMaterial;
            if (playerText != null) playerText.text = "";
        }
    }

    public override void OnGazeEnter()
    {
        if (!isInteracting)
        {
            base.OnGazeEnter();
            if (playerText != null)
                playerText.text = "Tap for " + checkType + " (DC: " + difficultyClass + ")";
        }
    }

    public override void OnGazeExit()
    {
        if (!isInteracting)
        {
            base.OnGazeExit();
        }
    }

    // IDiceCheckable implementation
    public Transform GetTransform()
    {
        return this.transform;
    }
}