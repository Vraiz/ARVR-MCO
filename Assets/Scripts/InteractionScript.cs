using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading;
using System.Threading.Tasks;

public class InteractionScript : MonoBehaviour
{

    public TMP_Text crossHairText;
    public TMP_Text Notification;
    
    private Vector3 oldPosition;

    private System.Random rand = new System.Random();

    private bool PortalActive = false;

    public GameObject PortalHome;

    public GameObject Table;

    public GameObject Fireplace;

    public int[] correctSequence = new int[5];
    private int currentIndex = 0;
    private CancellationTokenSource cts;

    private float Adjustment = 0.25f;

    public void DieClicked(int number)
    {
        if (number == correctSequence[currentIndex])
        {
            currentIndex++;

            if (currentIndex >= correctSequence.Length)
            {
                Notification.text = "Portal Activated!";
                PortalActive = true;
                currentIndex = 0;
            } else
            {
                Notification.text = "Correct!";
            }
        }
        else
        {
            Notification.text = "Incorrect!";
            currentIndex = 0; 
        }
    }

    public void TextClick()
    {
        if (cts != null)
        {
            cts.Cancel();
            cts.Dispose();
        }
        cts = new CancellationTokenSource();
        ClearText(cts.Token);
    }

    async void ClearText(CancellationToken token)
    {
        try
        {
            await Task.Delay(Mathf.RoundToInt(5000), cts.Token);
            Notification.text = "";
        }
        catch (TaskCanceledException)
        {
            // Cancels exit
        }
    }

    void TeleportPlayer(Vector3 destination)
    {
        if(PortalHome != null)
        {
            PortalHome.transform.position = oldPosition + destination;
        }
    }

    int DiceRoll()
    {
        return rand.Next(1, 21);
    }



    void PortalEvent(int result)
    {

        
        if (result == 20)
        {
            Notification.text = $"({result})" + " The portal opens to a secret location!";
            TeleportPlayer(new Vector3(-2.9f*Adjustment,86.9f*Adjustment,-1.1f*Adjustment));
        }
        else if (result >= 12)
        {
            Notification.text = $"({result})" + " The portal opens  to another realm!";
            TeleportPlayer(new Vector3(-5.1f*Adjustment,-89.2f*Adjustment,-1.1f*Adjustment));
        }
        else if (result <= 11 && result >= 2)
        {
            Notification.text = $"({result})" + " Failure, nothing happens";
        }
        else if (result == 1)
        {
            Notification.text = $"({result})" + " The portal opens to the Transitive Planes";
            TeleportPlayer(new Vector3(-2.1f*Adjustment,-152.4f*Adjustment,12.1f*Adjustment));
        }
    }


    void Update()
    {
        if(PortalHome == null)
        {
            PortalHome = GameObject.Find("Doran(Clone)");
            Debug.Log(PortalHome.name);
            if(PortalHome!= null)
            {
                oldPosition = PortalHome.transform.position;
            }
        }

        if(Table == null)
        {
            Table = GameObject.Find("Animar(Clone)");
        }

        if (Input.GetMouseButtonDown(0))
            {
                if(crossHairText.text == "Interact with portal?")
                {
                    if(!PortalActive)
                    {
                        Notification.text = "Portal is offline.";
                    }
                    else{
                        PortalEvent(DiceRoll());
                    }
                    
                } else if(crossHairText.text == "Interact with trophy?")
                {
                } else if(crossHairText.text == "Interact to return home")
                {
                    TeleportPlayer(new Vector3(0f, 0f, 0f));
                } else if(crossHairText.text == "Interact with blue die?")
                {
                    DieClicked(3);
                } else if(crossHairText.text == "Interact with red die?")
                {
                    DieClicked(1);
                } else if(crossHairText.text == "Interact with green die?")
                {
                    DieClicked(4);
                } else if(crossHairText.text == "Interact with white die?")
                {
                    DieClicked(5);
                } else if(crossHairText.text == "Interact with yellow die?")
                {
                    DieClicked(2);
                }
                
                if(crossHairText.text != null)
                {
                    TextClick();
                }
            }
            
    }


}