using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public Text textHP;
    public Text deadMessage;
    

    private static HUDController instance;
    
    
    private static PlayerController playerToShowHUD;


    

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        //hide death message
        deadMessage.gameObject.SetActive(false);

    }

    public static void SetPlayerToShowHUD(PlayerController playerToShowHUD)
    {
        HUDController.playerToShowHUD = playerToShowHUD;
    }

    public static PlayerController GetPlayerToShowHUD()
    {
        return HUDController.playerToShowHUD;
    }
    

    // Update is called once per frame
    void Update()
    {
        
        
    }

    public static void UpdateHUD()
    {
        if (playerToShowHUD != null)
        {
            instance.UpdateTextHP(playerToShowHUD.Health);
            instance.UpdateDeadMessage(playerToShowHUD.IsDead);
        }
    }
    private void UpdateTextHP(float HP)
    {
        if (!HUDController.playerToShowHUD.IsDead)
        {
            textHP.transform.parent.gameObject.SetActive(true);
            textHP.text = Mathf.RoundToInt(HP).ToString() + " HP";
        }
        else
        {
            textHP.transform.parent.gameObject.SetActive(false);
        }
        
    }
    private void UpdateDeadMessage(bool isDead)
    {
        if (isDead)
        {
            deadMessage.gameObject.SetActive(true);
        }
        else
        {
            deadMessage.gameObject.SetActive(false);
        }
    }

    


}
