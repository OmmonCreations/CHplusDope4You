// GENERATED AUTOMATICALLY FROM 'Assets/Modules/MobileInputs/Settings/PointerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace MobileInputs.Settings
{
    public class @PointerControls : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @PointerControls()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""PointerControls"",
    ""maps"": [
        {
            ""name"": ""pointer"",
            ""id"": ""d856a45a-83eb-4724-a566-763e580a360c"",
            ""actions"": [
                {
                    ""name"": ""point"",
                    ""type"": ""Value"",
                    ""id"": ""fa76f01b-d854-4d13-8c0e-41b3be9701bc"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""MouseAndPen"",
                    ""id"": ""258533df-c22c-4ce8-ae0f-07636cea17ee"",
                    ""path"": ""PointerInput"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""point"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""contact"",
                    ""id"": ""030f275b-d80e-403f-af7a-5c6fc73ce80d"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Mouse"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""contact"",
                    ""id"": ""06de7ac9-8251-4ecd-9f30-4071c2835df6"",
                    ""path"": ""<Pen>/tip"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Pen"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""position"",
                    ""id"": ""44525a69-e59a-4677-a652-964f92e7ba58"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Mouse"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""position"",
                    ""id"": ""81bce7c2-bf86-412d-a617-c74d51c341f0"",
                    ""path"": ""<Pen>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Pen"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""tilt"",
                    ""id"": ""13101a54-78f8-4305-968b-1649adbaad67"",
                    ""path"": ""<Pen>/tilt"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Pen"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""pressure"",
                    ""id"": ""0c41b4a5-c32d-4fa6-baea-8d727e59551f"",
                    ""path"": ""<Pen>/pressure"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Pen"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""twist"",
                    ""id"": ""8d2d8e77-5f16-43d3-a273-231dd3946d3e"",
                    ""path"": ""<Pen>/twist"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Pen"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Touch0"",
                    ""id"": ""fbf93d51-20c3-4500-92c5-c8b6fcd2342a"",
                    ""path"": ""PointerInput"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""point"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""contact"",
                    ""id"": ""6543049b-bed6-434c-8ddc-6192e3965bf4"",
                    ""path"": ""<Touchscreen>/touch0/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Touch"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""position"",
                    ""id"": ""d0d18feb-b1c2-4fd0-8958-34d5792cf6ae"",
                    ""path"": ""<Touchscreen>/touch0/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Touch"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""radius"",
                    ""id"": ""990c9715-92d2-4db9-8e68-2e053521e806"",
                    ""path"": ""<Touchscreen>/touch0/radius"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Touch"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""pressure"",
                    ""id"": ""7b350fe0-dd2d-4632-bc7a-dd44892147a5"",
                    ""path"": ""<Touchscreen>/touch0/pressure"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Touch"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""inputId"",
                    ""id"": ""5f0b5303-f6ad-4b76-ad96-ef4e9c7949aa"",
                    ""path"": ""<Touchscreen>/touch0/touchId"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Touch"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Touch1"",
                    ""id"": ""8fd1ca19-fa4f-4fe9-b678-912c91d6db08"",
                    ""path"": ""PointerInput"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""point"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""contact"",
                    ""id"": ""c0a38a35-8a3f-4959-adce-50d23fb1682d"",
                    ""path"": ""<Touchscreen>/touch1/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Touch"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""position"",
                    ""id"": ""4af933df-8a80-4f0b-a224-2730b10ec6ee"",
                    ""path"": ""<Touchscreen>/touch1/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Touch"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""radius"",
                    ""id"": ""dec9f7ed-373e-4078-a373-6cb43634188a"",
                    ""path"": ""<Touchscreen>/touch1/radius"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Touch"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""pressure"",
                    ""id"": ""72385f3e-9c7c-412d-aa7f-8018aaae06d8"",
                    ""path"": ""<Touchscreen>/touch1/pressure"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Touch"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""inputId"",
                    ""id"": ""69760c4d-86fa-4a03-ba16-509b61849598"",
                    ""path"": ""<Touchscreen>/touch1/touchId"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Touch"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Touch2"",
                    ""id"": ""eac8399b-c1c8-4637-a520-900c0e1431ff"",
                    ""path"": ""PointerInput"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""point"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""contact"",
                    ""id"": ""eeb8a16d-53a0-4dcb-9486-c4a6094c078d"",
                    ""path"": ""<Touchscreen>/touch2/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Touch"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""position"",
                    ""id"": ""cdba6c03-b3f8-4b8d-b3fc-83aa39481bb7"",
                    ""path"": ""<Touchscreen>/touch2/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Touch"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""radius"",
                    ""id"": ""6e130061-1c4b-4878-b9e7-7f46c8a47eb1"",
                    ""path"": ""<Touchscreen>/touch2/radius"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Touch"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""pressure"",
                    ""id"": ""2e3d3ed2-2a46-4d79-a746-689825688278"",
                    ""path"": ""<Touchscreen>/touch2/pressure"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Touch"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""inputId"",
                    ""id"": ""f88e50cc-027e-4c53-97be-902db5633205"",
                    ""path"": ""<Touchscreen>/touch2/touchId"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Touch"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Touch3"",
                    ""id"": ""acd24144-c39a-4d3b-aed2-e09bfda8ac1a"",
                    ""path"": ""PointerInput"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""point"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""contact"",
                    ""id"": ""077cc313-8fc2-438c-8747-de8d494e533a"",
                    ""path"": ""<Touchscreen>/touch3/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Touch"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""position"",
                    ""id"": ""34c7a28a-5c36-4644-9085-77b6f8b1dc5f"",
                    ""path"": ""<Touchscreen>/touch3/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Touch"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""radius"",
                    ""id"": ""cff250f2-4c46-4366-90d9-a15b0155dee3"",
                    ""path"": ""<Touchscreen>/touch3/radius"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Touch"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""pressure"",
                    ""id"": ""0a619b38-932c-4f46-86f5-a2d7dd9c99da"",
                    ""path"": ""<Touchscreen>/touch3/pressure"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Touch"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""inputId"",
                    ""id"": ""4cfbf4d6-e957-4207-827a-bebf24b496bb"",
                    ""path"": ""<Touchscreen>/touch3/touchId"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Touch"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Touch4"",
                    ""id"": ""aeea59de-16cb-4aa6-bee2-593cbac3b527"",
                    ""path"": ""PointerInput"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""point"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""contact"",
                    ""id"": ""60f9c558-0990-44a7-9ff1-73563ccc371e"",
                    ""path"": ""<Touchscreen>/touch4/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Touch"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""position"",
                    ""id"": ""b1a18133-0999-4fa6-b5cb-3f9b89df86bc"",
                    ""path"": ""<Touchscreen>/touch4/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Touch"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""radius"",
                    ""id"": ""73c7c3e3-f00f-4e0d-9280-1ed83663d87f"",
                    ""path"": ""<Touchscreen>/touch4/radius"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Touch"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""pressure"",
                    ""id"": ""72e0b3df-4b94-4927-9af6-89570430e4d1"",
                    ""path"": ""<Touchscreen>/touch4/pressure"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Touch"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""inputId"",
                    ""id"": ""492624ae-a9ac-43c0-b0c4-ecf6e17f4454"",
                    ""path"": ""<Touchscreen>/touch4/touchId"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Touch"",
                    ""action"": ""point"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // pointer
            m_pointer = asset.FindActionMap("pointer", throwIfNotFound: true);
            m_pointer_point = m_pointer.FindAction("point", throwIfNotFound: true);
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

        // pointer
        private readonly InputActionMap m_pointer;
        private IPointerActions m_PointerActionsCallbackInterface;
        private readonly InputAction m_pointer_point;
        public struct PointerActions
        {
            private @PointerControls m_Wrapper;
            public PointerActions(@PointerControls wrapper) { m_Wrapper = wrapper; }
            public InputAction @point => m_Wrapper.m_pointer_point;
            public InputActionMap Get() { return m_Wrapper.m_pointer; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PointerActions set) { return set.Get(); }
            public void SetCallbacks(IPointerActions instance)
            {
                if (m_Wrapper.m_PointerActionsCallbackInterface != null)
                {
                    @point.started -= m_Wrapper.m_PointerActionsCallbackInterface.OnPoint;
                    @point.performed -= m_Wrapper.m_PointerActionsCallbackInterface.OnPoint;
                    @point.canceled -= m_Wrapper.m_PointerActionsCallbackInterface.OnPoint;
                }
                m_Wrapper.m_PointerActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @point.started += instance.OnPoint;
                    @point.performed += instance.OnPoint;
                    @point.canceled += instance.OnPoint;
                }
            }
        }
        public PointerActions @pointer => new PointerActions(this);
        public interface IPointerActions
        {
            void OnPoint(InputAction.CallbackContext context);
        }
    }
}
