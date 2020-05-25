using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public Text textHP;
    public Text deadMessage;
    
    
    private PlayerController player;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTextHP(player.Life);
        UpdateDeadMessage(player.IsDead);
    }
    private void UpdateTextHP(float HP)
    {
        textHP.text = Mathf.RoundToInt(HP).ToString() + " HP";
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
