// GENERATED AUTOMATICALLY FROM 'Assets/InputMaster.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputMaster : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputMaster()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputMaster"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""f41ea668-d5be-4ab0-9d84-114fc3f54cde"",
            ""actions"": [
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""43c4ddb7-544a-4a07-9166-b0a0d6432d0f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Movement"",
                    ""type"": ""Button"",
                    ""id"": ""0c88974a-78f3-4299-a3c8-094911740991"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""6dca1695-4a89-488b-b727-e8f352d25496"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LookX"",
                    ""type"": ""Value"",
                    ""id"": ""b5e9b159-22b1-4f80-8e50-57d336f9d4ca"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LookY"",
                    ""type"": ""Value"",
                    ""id"": ""7f92f421-58d7-45ca-853c-be0213c673a8"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""UseItem"",
                    ""type"": ""Button"",
                    ""id"": ""5e3eca23-1465-4a97-b611-2bcfa5cf6e8b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""NextItem"",
                    ""type"": ""Button"",
                    ""id"": ""2a39bc17-9e90-45ab-93bd-af526dcbb834"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PreviousItem"",
                    ""type"": ""Button"",
                    ""id"": ""fc5bc4a0-9727-4225-a792-c9df99b2fe1c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""fed34f6c-f9c9-47b3-af62-e0925bcedc7c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MouseAim"",
                    ""type"": ""Value"",
                    ""id"": ""768d4fad-41d2-4c65-9bb7-47a1f9350eff"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""bf06f1cc-3f48-4df3-b6b0-804b682286eb"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""a9ba7b1c-8891-4514-afc7-a8288c685dd3"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""3b596ed1-4ec2-4dd5-a2d7-c39a2ea17b9e"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""6095583e-4e97-433b-a72a-0cda02c8a9f6"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""dada44b0-3d71-457d-ac82-3ab363066a47"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""00e5e85c-482d-49d2-b6e8-4f6149244961"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""03e6491b-b6a2-4b78-a564-0cc5b52a93ad"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""09bcf8f0-5b74-43cf-83e3-0c68fa9cb177"",
                    ""path"": ""<Mouse>/delta/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LookX"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0b7210e7-a302-4841-a676-6553a4289a70"",
                    ""path"": ""<Mouse>/delta/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LookY"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0eef89aa-181e-4610-ae29-01a31c3807cc"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UseItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4678cc40-0b75-422f-b3bb-02aa15f143c7"",
                    ""path"": ""<Mouse>/backButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PreviousItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""90b150af-8f70-447a-b4e3-5b9fea4657f9"",
                    ""path"": ""<Mouse>/forwardButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NextItem"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""98832264-d147-4b31-9416-36c0bf4ea74c"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a8589602-5e52-4cb3-bbf8-d168ce5649b5"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseAim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UI"",
            ""id"": ""b323295f-37aa-4bfb-ab47-19d9a3a601c1"",
            ""actions"": [
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""67403f36-05ce-4938-a44c-f4790600ed98"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""5e384831-6a14-44c7-ba7c-8c7c5132b439"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Spectator"",
            ""id"": ""ca12b61e-0657-44a0-85e1-91433a3fef54"",
            ""actions"": [
                {
                    ""name"": ""MouseAim"",
                    ""type"": ""Value"",
                    ""id"": ""b61508f5-bdd7-443e-9a8b-1def05635faa"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Movement"",
                    ""type"": ""Button"",
                    ""id"": ""279eab30-08d0-40d2-9d85-0e8e5868a038"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MovementVertical"",
                    ""type"": ""Button"",
                    ""id"": ""65a4b2cb-e046-4af4-9e03-863fedb0416e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""NextPlayer"",
                    ""type"": ""Button"",
                    ""id"": ""85a39969-c8c7-45d5-b545-17dce4c72365"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PreviousPlayer"",
                    ""type"": ""Button"",
                    ""id"": ""db2d3124-0c48-4884-b0ec-a50de156298c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""FreeLook"",
                    ""type"": ""Button"",
                    ""id"": ""32f0c517-e98a-4b24-90ed-2e3a60103bbd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""f00c0fc4-1b74-4e74-a948-da6de46e4695"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseAim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""258ea997-5ff0-443d-8365-0f33ff161863"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""175fac00-64d2-4f42-b69a-5209b5180510"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""5b7c296c-badb-458d-bf68-408a6e68cc5a"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""ce47aa40-2285-4f7b-b326-c50e14cf78eb"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""0c21edc5-9d08-4814-8104-d3f7b9b852d1"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""cf9b669d-3afe-4f1d-b981-ab1864cb497e"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementVertical"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""dac5b73c-09ee-403c-b1a9-24be55fa400c"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementVertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""33fb7b1e-9902-4e05-aefa-6b33e7d109bf"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MovementVertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""cb0e168b-5020-47f2-927c-e837f132947d"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""NextPlayer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e71ff9cd-c8e5-42ec-b45b-26bd5113ad88"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PreviousPlayer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ae814b90-a436-4e04-aa59-9a17eae518c3"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""FreeLook"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Shoot = m_Player.FindAction("Shoot", throwIfNotFound: true);
        m_Player_Movement = m_Player.FindAction("Movement", throwIfNotFound: true);
        m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
        m_Player_LookX = m_Player.FindAction("LookX", throwIfNotFound: true);
        m_Player_LookY = m_Player.FindAction("LookY", throwIfNotFound: true);
        m_Player_UseItem = m_Player.FindAction("UseItem", throwIfNotFound: true);
        m_Player_NextItem = m_Player.FindAction("NextItem", throwIfNotFound: true);
        m_Player_PreviousItem = m_Player.FindAction("PreviousItem", throwIfNotFound: true);
        m_Player_Crouch = m_Player.FindAction("Crouch", throwIfNotFound: true);
        m_Player_MouseAim = m_Player.FindAction("MouseAim", throwIfNotFound: true);
        // UI
        m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
        m_UI_Pause = m_UI.FindAction("Pause", throwIfNotFound: true);
        // Spectator
        m_Spectator = asset.FindActionMap("Spectator", throwIfNotFound: true);
        m_Spectator_MouseAim = m_Spectator.FindAction("MouseAim", throwIfNotFound: true);
        m_Spectator_Movement = m_Spectator.FindAction("Movement", throwIfNotFound: true);
        m_Spectator_MovementVertical = m_Spectator.FindAction("MovementVertical", throwIfNotFound: true);
        m_Spectator_NextPlayer = m_Spectator.FindAction("NextPlayer", throwIfNotFound: true);
        m_Spectator_PreviousPlayer = m_Spectator.FindAction("PreviousPlayer", throwIfNotFound: true);
        m_Spectator_FreeLook = m_Spectator.FindAction("FreeLook", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Shoot;
    private readonly InputAction m_Player_Movement;
    private readonly InputAction m_Player_Jump;
    private readonly InputAction m_Player_LookX;
    private readonly InputAction m_Player_LookY;
    private readonly InputAction m_Player_UseItem;
    private readonly InputAction m_Player_NextItem;
    private readonly InputAction m_Player_PreviousItem;
    private readonly InputAction m_Player_Crouch;
    private readonly InputAction m_Player_MouseAim;
    public struct PlayerActions
    {
        private @InputMaster m_Wrapper;
        public PlayerActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Shoot => m_Wrapper.m_Player_Shoot;
        public InputAction @Movement => m_Wrapper.m_Player_Movement;
        public InputAction @Jump => m_Wrapper.m_Player_Jump;
        public InputAction @LookX => m_Wrapper.m_Player_LookX;
        public InputAction @LookY => m_Wrapper.m_Player_LookY;
        public InputAction @UseItem => m_Wrapper.m_Player_UseItem;
        public InputAction @NextItem => m_Wrapper.m_Player_NextItem;
        public InputAction @PreviousItem => m_Wrapper.m_Player_PreviousItem;
        public InputAction @Crouch => m_Wrapper.m_Player_Crouch;
        public InputAction @MouseAim => m_Wrapper.m_Player_MouseAim;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Shoot.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShoot;
                @Shoot.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShoot;
                @Shoot.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnShoot;
                @Movement.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Jump.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @LookX.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLookX;
                @LookX.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLookX;
                @LookX.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLookX;
                @LookY.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLookY;
                @LookY.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLookY;
                @LookY.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLookY;
                @UseItem.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUseItem;
                @UseItem.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUseItem;
                @UseItem.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUseItem;
                @NextItem.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNextItem;
                @NextItem.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNextItem;
                @NextItem.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnNextItem;
                @PreviousItem.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPreviousItem;
                @PreviousItem.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPreviousItem;
                @PreviousItem.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnPreviousItem;
                @Crouch.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCrouch;
                @Crouch.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCrouch;
                @Crouch.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnCrouch;
                @MouseAim.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMouseAim;
                @MouseAim.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMouseAim;
                @MouseAim.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMouseAim;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Shoot.started += instance.OnShoot;
                @Shoot.performed += instance.OnShoot;
                @Shoot.canceled += instance.OnShoot;
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @LookX.started += instance.OnLookX;
                @LookX.performed += instance.OnLookX;
                @LookX.canceled += instance.OnLookX;
                @LookY.started += instance.OnLookY;
                @LookY.performed += instance.OnLookY;
                @LookY.canceled += instance.OnLookY;
                @UseItem.started += instance.OnUseItem;
                @UseItem.performed += instance.OnUseItem;
                @UseItem.canceled += instance.OnUseItem;
                @NextItem.started += instance.OnNextItem;
                @NextItem.performed += instance.OnNextItem;
                @NextItem.canceled += instance.OnNextItem;
                @PreviousItem.started += instance.OnPreviousItem;
                @PreviousItem.performed += instance.OnPreviousItem;
                @PreviousItem.canceled += instance.OnPreviousItem;
                @Crouch.started += instance.OnCrouch;
                @Crouch.performed += instance.OnCrouch;
                @Crouch.canceled += instance.OnCrouch;
                @MouseAim.started += instance.OnMouseAim;
                @MouseAim.performed += instance.OnMouseAim;
                @MouseAim.canceled += instance.OnMouseAim;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // UI
    private readonly InputActionMap m_UI;
    private IUIActions m_UIActionsCallbackInterface;
    private readonly InputAction m_UI_Pause;
    public struct UIActions
    {
        private @InputMaster m_Wrapper;
        public UIActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Pause => m_Wrapper.m_UI_Pause;
        public InputActionMap Get() { return m_Wrapper.m_UI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
        public void SetCallbacks(IUIActions instance)
        {
            if (m_Wrapper.m_UIActionsCallbackInterface != null)
            {
                @Pause.started -= m_Wrapper.m_UIActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnPause;
            }
            m_Wrapper.m_UIActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
            }
        }
    }
    public UIActions @UI => new UIActions(this);

    // Spectator
    private readonly InputActionMap m_Spectator;
    private ISpectatorActions m_SpectatorActionsCallbackInterface;
    private readonly InputAction m_Spectator_MouseAim;
    private readonly InputAction m_Spectator_Movement;
    private readonly InputAction m_Spectator_MovementVertical;
    private readonly InputAction m_Spectator_NextPlayer;
    private readonly InputAction m_Spectator_PreviousPlayer;
    private readonly InputAction m_Spectator_FreeLook;
    public struct SpectatorActions
    {
        private @InputMaster m_Wrapper;
        public SpectatorActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @MouseAim => m_Wrapper.m_Spectator_MouseAim;
        public InputAction @Movement => m_Wrapper.m_Spectator_Movement;
        public InputAction @MovementVertical => m_Wrapper.m_Spectator_MovementVertical;
        public InputAction @NextPlayer => m_Wrapper.m_Spectator_NextPlayer;
        public InputAction @PreviousPlayer => m_Wrapper.m_Spectator_PreviousPlayer;
        public InputAction @FreeLook => m_Wrapper.m_Spectator_FreeLook;
        public InputActionMap Get() { return m_Wrapper.m_Spectator; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(SpectatorActions set) { return set.Get(); }
        public void SetCallbacks(ISpectatorActions instance)
        {
            if (m_Wrapper.m_SpectatorActionsCallbackInterface != null)
            {
                @MouseAim.started -= m_Wrapper.m_SpectatorActionsCallbackInterface.OnMouseAim;
                @MouseAim.performed -= m_Wrapper.m_SpectatorActionsCallbackInterface.OnMouseAim;
                @MouseAim.canceled -= m_Wrapper.m_SpectatorActionsCallbackInterface.OnMouseAim;
                @Movement.started -= m_Wrapper.m_SpectatorActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_SpectatorActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_SpectatorActionsCallbackInterface.OnMovement;
                @MovementVertical.started -= m_Wrapper.m_SpectatorActionsCallbackInterface.OnMovementVertical;
                @MovementVertical.performed -= m_Wrapper.m_SpectatorActionsCallbackInterface.OnMovementVertical;
                @MovementVertical.canceled -= m_Wrapper.m_SpectatorActionsCallbackInterface.OnMovementVertical;
                @NextPlayer.started -= m_Wrapper.m_SpectatorActionsCallbackInterface.OnNextPlayer;
                @NextPlayer.performed -= m_Wrapper.m_SpectatorActionsCallbackInterface.OnNextPlayer;
                @NextPlayer.canceled -= m_Wrapper.m_SpectatorActionsCallbackInterface.OnNextPlayer;
                @PreviousPlayer.started -= m_Wrapper.m_SpectatorActionsCallbackInterface.OnPreviousPlayer;
                @PreviousPlayer.performed -= m_Wrapper.m_SpectatorActionsCallbackInterface.OnPreviousPlayer;
                @PreviousPlayer.canceled -= m_Wrapper.m_SpectatorActionsCallbackInterface.OnPreviousPlayer;
                @FreeLook.started -= m_Wrapper.m_SpectatorActionsCallbackInterface.OnFreeLook;
                @FreeLook.performed -= m_Wrapper.m_SpectatorActionsCallbackInterface.OnFreeLook;
                @FreeLook.canceled -= m_Wrapper.m_SpectatorActionsCallbackInterface.OnFreeLook;
            }
            m_Wrapper.m_SpectatorActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MouseAim.started += instance.OnMouseAim;
                @MouseAim.performed += instance.OnMouseAim;
                @MouseAim.canceled += instance.OnMouseAim;
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @MovementVertical.started += instance.OnMovementVertical;
                @MovementVertical.performed += instance.OnMovementVertical;
                @MovementVertical.canceled += instance.OnMovementVertical;
                @NextPlayer.started += instance.OnNextPlayer;
                @NextPlayer.performed += instance.OnNextPlayer;
                @NextPlayer.canceled += instance.OnNextPlayer;
                @PreviousPlayer.started += instance.OnPreviousPlayer;
                @PreviousPlayer.performed += instance.OnPreviousPlayer;
                @PreviousPlayer.canceled += instance.OnPreviousPlayer;
                @FreeLook.started += instance.OnFreeLook;
                @FreeLook.performed += instance.OnFreeLook;
                @FreeLook.canceled += instance.OnFreeLook;
            }
        }
    }
    public SpectatorActions @Spectator => new SpectatorActions(this);
    public interface IPlayerActions
    {
        void OnShoot(InputAction.CallbackContext context);
        void OnMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnLookX(InputAction.CallbackContext context);
        void OnLookY(InputAction.CallbackContext context);
        void OnUseItem(InputAction.CallbackContext context);
        void OnNextItem(InputAction.CallbackContext context);
        void OnPreviousItem(InputAction.CallbackContext context);
        void OnCrouch(InputAction.CallbackContext context);
        void OnMouseAim(InputAction.CallbackContext context);
    }
    public interface IUIActions
    {
        void OnPause(InputAction.CallbackContext context);
    }
    public interface ISpectatorActions
    {
        void OnMouseAim(InputAction.CallbackContext context);
        void OnMovement(InputAction.CallbackContext context);
        void OnMovementVertical(InputAction.CallbackContext context);
        void OnNextPlayer(InputAction.CallbackContext context);
        void OnPreviousPlayer(InputAction.CallbackContext context);
        void OnFreeLook(InputAction.CallbackContext context);
    }
}
