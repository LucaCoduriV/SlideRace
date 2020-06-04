using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviourPunCallbacks
{
    public InputMaster inputMaster;
    

    

    [Header("Pause Panel")]
    public GameObject PausePanel;
    private bool IsPauseOpen = false;

    [Header("Options Panel")]
    public GameObject OptionsPanel;
    public TMPro.TMP_InputField sensitivityInput;
    private bool IsOptionsOpen = false;


    void Awake()
    {
        inputMaster = new InputMaster();

        inputMaster.UI.Pause.performed += ctx =>
        {
            TogglePauseMenu();
        };
    }

    public void TogglePauseMenu()
    {
        IsPauseOpen = !IsPauseOpen;

        if(PausePanel != null)
        {
            PausePanel.SetActive(IsPauseOpen);
            ShowMouse(IsPauseOpen);
            CloseAllPanel();
        }
        else
        {
            Debug.LogError("Please give a pause panel gameobject");
        }
        
    }
    public void TogglePauseMenu(bool status)
    {
        IsPauseOpen = status;

        if (PausePanel != null)
        {
            PausePanel.SetActive(IsPauseOpen);
            ShowMouse(IsPauseOpen);
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
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
        Debug.Log("Quit Game !");
        //PhotonNetwork.DestroyPlayerObjects(PlayerController.LocalPlayerInstance.GetComponent<PhotonView>());
        PhotonNetwork.LeaveRoom();
        
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel(0);
    }

    public void OnEnable()
    {
        inputMaster.Enable();
    }

    public void OnDisable()
    {
        inputMaster.Disable();
    }
}
