using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ch.Luca.MyGame;

public class UserInterface : MonoBehaviourPunCallbacks
{
    public InputMaster inputMaster;
    public static UserInterface instance;
    public bool followMouse = true;
    

    [Header("Pause Panel")]
    public GameObject PausePanel;
    private bool isPauseOpen = false;
    

    [Header("Options Panel")]
    public GameObject OptionsPanel;
    public TMPro.TMP_InputField sensitivityInput;
    private bool IsOptionsOpen = false;

    

    void Awake()
    {
        if(instance == null)
        {
            instance = this.GetComponent<UserInterface>();
        }
        else
        {
            Destroy(this.GetComponent<UserInterface>());
        }

        inputMaster = new InputMaster();

        inputMaster.UI.Pause.performed += ctx =>
        {
            TogglePauseMenu();
        };
    }

    public void TogglePauseMenu()
    {
        isPauseOpen = !isPauseOpen;

        if(PausePanel != null)
        {
            PausePanel.SetActive(isPauseOpen);
            ShowMouse(isPauseOpen);
            CloseAllPanel();
        }
        else
        {
            Debug.LogError("Please give a pause panel gameobject");
        }
        
    }
    public void TogglePauseMenu(bool status)
    {
        isPauseOpen = status;

        if (PausePanel != null)
        {
            PausePanel.SetActive(isPauseOpen);
            ShowMouse(isPauseOpen);
            if (status)
            {
                CloseAllPanel();
            }
            
        }
        else
        {
            Debug.LogError("Please give a pause panel gameobject");
        }

    }

    public void ShowPanel(string panelName)
    {
        if (OptionsPanel.name.Contains(panelName))
        {
            updateOptionsFields();
        }
        OptionsPanel.SetActive(OptionsPanel.name.Contains(panelName));

        TogglePauseMenu(false);
        ShowMouse(true);

    }

    public void CloseAllPanel()
    {
        OptionsPanel.SetActive(false);
    }

    private void ShowMouse(bool status)
    {
        if (status)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            followMouse = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            followMouse = true;
        }
        
    }

    public void updateOptionsFields()
    {
        sensitivityInput.text = ConfigManager.config.MouseSensitivity.ToString();
    }

    public void applyOptionsModifications()
    {
        ConfigManager.config.MouseSensitivity = float.Parse(sensitivityInput.text);

        ConfigManager.SaveIntoJson();
    }

    public void OnQuit()
    {
        GameManager.instance.LeaveRoom();
        
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel(0);
    }

    public override void OnEnable()
    {
        inputMaster.Enable();
    }

    public override void OnDisable()
    {
        inputMaster.Disable();
    }
}
