// GENERATED AUTOMATICALLY FROM 'Assets/New Input System/PCInputs.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PCInputs : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PCInputs()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PCInputs"",
    ""maps"": [
        {
            ""name"": ""Player Actions"",
            ""id"": ""dc95d0a3-839d-4cd9-8ce0-beea964334b3"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""3b2a4429-0c46-4ad4-b9f1-a94273d99f36"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""7b3a4438-10d3-4d72-8843-224c043d71d0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""PassThrough"",
                    ""id"": ""1afdb638-d52c-49cc-a4e6-6483e04d3470"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""TestSpawn"",
                    ""type"": ""Button"",
                    ""id"": ""6e35f6df-3bbe-4c15-8a91-b8dc02b884d5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""TestDespawn"",
                    ""type"": ""Button"",
                    ""id"": ""bbd9e29e-5634-4a1a-851d-90ea27ebdcf1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""117d6388-0c4d-4776-b5e1-5335f7f7a841"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""de77ec1d-30a8-45b5-aa53-b6d7a27e05ba"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""dd5a00de-3ef7-4d0c-b911-68a05313df36"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""f12f4711-bb42-490a-b7f0-c5d0f4f70ed1"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""5043a25c-df41-46df-b3b5-1b03cb31b7be"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""969cfcec-224f-4985-b9dc-7c422b5064d6"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1db3fef7-09ef-48ca-9be8-36058f0539f1"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fa5ae38f-77f6-4f5d-afc8-6d8d5a81c53d"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f72b099d-9501-4120-b635-306e7b88050a"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9b46792c-48b1-4262-9380-651ed66d9fe2"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""TestSpawn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""31e90a78-5f14-4bf2-926e-d9554eab4d1a"",
                    ""path"": ""<Keyboard>/v"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""TestDespawn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""PC"",
            ""bindingGroup"": ""PC"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player Actions
        m_PlayerActions = asset.FindActionMap("Player Actions", throwIfNotFound: true);
        m_PlayerActions_Move = m_PlayerActions.FindAction("Move", throwIfNotFound: true);
        m_PlayerActions_Interact = m_PlayerActions.FindAction("Interact", throwIfNotFound: true);
        m_PlayerActions_Look = m_PlayerActions.FindAction("Look", throwIfNotFound: true);
        m_PlayerActions_TestSpawn = m_PlayerActions.FindAction("TestSpawn", throwIfNotFound: true);
        m_PlayerActions_TestDespawn = m_PlayerActions.FindAction("TestDespawn", throwIfNotFound: true);
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

    // Player Actions
    private readonly InputActionMap m_PlayerActions;
    private IPlayerActionsActions m_PlayerActionsActionsCallbackInterface;
    private readonly InputAction m_PlayerActions_Move;
    private readonly InputAction m_PlayerActions_Interact;
    private readonly InputAction m_PlayerActions_Look;
    private readonly InputAction m_PlayerActions_TestSpawn;
    private readonly InputAction m_PlayerActions_TestDespawn;
    public struct PlayerActionsActions
    {
        private @PCInputs m_Wrapper;
        public PlayerActionsActions(@PCInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_PlayerActions_Move;
        public InputAction @Interact => m_Wrapper.m_PlayerActions_Interact;
        public InputAction @Look => m_Wrapper.m_PlayerActions_Look;
        public InputAction @TestSpawn => m_Wrapper.m_PlayerActions_TestSpawn;
        public InputAction @TestDespawn => m_Wrapper.m_PlayerActions_TestDespawn;
        public InputActionMap Get() { return m_Wrapper.m_PlayerActions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActionsActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActionsActions instance)
        {
            if (m_Wrapper.m_PlayerActionsActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnMove;
                @Interact.started -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnInteract;
                @Look.started -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnLook;
                @Look.performed -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnLook;
                @Look.canceled -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnLook;
                @TestSpawn.started -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnTestSpawn;
                @TestSpawn.performed -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnTestSpawn;
                @TestSpawn.canceled -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnTestSpawn;
                @TestDespawn.started -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnTestDespawn;
                @TestDespawn.performed -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnTestDespawn;
                @TestDespawn.canceled -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnTestDespawn;
            }
            m_Wrapper.m_PlayerActionsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
                @TestSpawn.started += instance.OnTestSpawn;
                @TestSpawn.performed += instance.OnTestSpawn;
                @TestSpawn.canceled += instance.OnTestSpawn;
                @TestDespawn.started += instance.OnTestDespawn;
                @TestDespawn.performed += instance.OnTestDespawn;
                @TestDespawn.canceled += instance.OnTestDespawn;
            }
        }
    }
    public PlayerActionsActions @PlayerActions => new PlayerActionsActions(this);
    private int m_PCSchemeIndex = -1;
    public InputControlScheme PCScheme
    {
        get
        {
            if (m_PCSchemeIndex == -1) m_PCSchemeIndex = asset.FindControlSchemeIndex("PC");
            return asset.controlSchemes[m_PCSchemeIndex];
        }
    }
    public interface IPlayerActionsActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnTestSpawn(InputAction.CallbackContext context);
        void OnTestDespawn(InputAction.CallbackContext context);
    }
}
