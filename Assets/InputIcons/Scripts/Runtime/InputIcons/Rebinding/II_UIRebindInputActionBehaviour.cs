using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using static InputIcons.InputIconsUtility;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using System.Text.RegularExpressions;

namespace InputIcons
{
    public class II_UIRebindInputActionBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Reference to the action that is to be rebound.
        /// </summary>
        public InputActionReference actionReference
        {
            get => m_Action;
            set
            {
                m_Action = value;
                UpdateBindingDisplay();
            }
        }

        [Tooltip("Reference to action that is to be rebound from the UI.")]
        [SerializeField]
        private InputActionReference m_Action;

        public int m_bindingIndex = 0;

        [SerializeField]
        private BindingType m_BindingType = BindingType.Up;

        [SerializeField]
        private InputIconsUtility.DeviceType m_RebindType = InputIconsUtility.DeviceType.Auto;

        public int keyboardControlSchemeIndex = 0;
        public int gamepadControlSchemeIndex = 0;

        public enum DisplayType { Sprites, Font };
        public DisplayType displayType = DisplayType.Sprites;

        [SerializeField]
        public InputBindingComposite part;


        private string previousBinding;

        public InputIconsUtility.DeviceType deviceType
        {
            get => m_RebindType;
            set
            {
                m_RebindType = value;
                UpdateBindingDisplay();
            }
        }

        public BindingType bindingType
        {
            get => m_BindingType;
            set
            {
                m_BindingType = value;
                UpdateBindingDisplay();
            }
        }

        private static InputActionRebindingExtensions.RebindingOperation rebindOperation;

        public delegate void OnRebindOperationCompleted(II_UIRebindInputActionBehaviour rebindBehaviour);
        public static OnRebindOperationCompleted onRebindOperationCompleted;

        //subscribe to react and display a message like "Key already bound to Jump action" for example
        public static UnityEvent<InputBinding> duplicateBindingFoundOnRebindOperation;

        public bool canBeRebound = true;
        public bool ignoreOtherButtons = false;

        [Header("UI Display - Action Text")]
        public TextMeshProUGUI actionNameDisplayText;

        [Header("UI Display - Binding Text")]
        public TextMeshProUGUI bindingNameDisplayText;

        [Header("UI Display - Buttons")]
        public GameObject rebindButtonObject;
        public GameObject resetButtonObject;

        [Header("UI Display - Listening Text")]
        public GameObject listeningForInputObject;

        [Header("UI Display - Key Already Used")]
        public GameObject keyAlreadyUsedObject;

        public string keyboardCancelKey = "<Keyboard>/escape";
        public string gamepadCancelKey = "<Gamepad>/start";

        private static InputDevice deviceToRebind;

        private void Awake()
        {

            //UpdateBehaviour();
        }

        private void OnEnable()
        {
            if (!ignoreOtherButtons)
                onRebindOperationCompleted += HandleAnyRebindOperationCompleted;

            UpdateBehaviour();
            InputIconsManagerSO.onControlsChanged += HandleControlsChanged;
            InputIconsManagerSO.onBindingsChanged += UpdateBehaviour;
        }

        private void OnDisable()
        {
            onRebindOperationCompleted -= HandleAnyRebindOperationCompleted;

            InputIconsManagerSO.onControlsChanged -= HandleControlsChanged;
            InputIconsManagerSO.onBindingsChanged -= UpdateBehaviour;
        }

        private void OnDestroy()
        {

        }

        public void HandleControlsChanged(InputDevice inputDevice)
        {
            UpdateBehaviour();
        }


        public void ButtonPressedStartRebind()
        {
            if (!canBeRebound)
                return;

            StartRebindProcess();
        }

        public string GetBindingOverride()
        {
            return actionReference.action.bindings[GetMyBindingIndex()].overridePath;
        }

        public int GetMyBindingIndex()
        {
            string deviceString = GetActiveDeviceString();

            if (deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse)
                deviceString = InputIconsManagerSO.GetKeyboardControlSchemeName(0);


            if (deviceType == InputIconsUtility.DeviceType.Gamepad)
                deviceString = InputIconsManagerSO.GetGamepadControlSchemeName(0);


            int index = GetIndexOfBindingType(m_Action.action, bindingType, deviceString, m_bindingIndex);
            return index;
        }

        void StartRebindProcess()
        {
            InputDevice device = InputIconsManagerSO.GetCurrentInputDevice();
            if (deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse && !InputIconsManagerSO.DeviceIsKeyboardOrMouse(device))
                return;

            if (deviceType == InputIconsUtility.DeviceType.Gamepad && !(device is Gamepad))
                return;

            if (rebindOperation != null)
                rebindOperation.Cancel();



            ToggleGameObjectState(rebindButtonObject, false);
            ToggleGameObjectState(resetButtonObject, false);
            ToggleGameObjectState(listeningForInputObject, true);

            m_Action.action.Disable();

            //do not allow keyboard/mouse buttons, when rebind operation was started with gamepad and vice versa
            //and only allow the correct device to bind when the device for this button is keyboard/mouse or gamepad
            deviceToRebind = device;


            int index = GetMyBindingIndex();
            rebindOperation = m_Action.action.PerformInteractiveRebinding(index);
            previousBinding = m_Action.action.bindings[index].path;


            rebindOperation
            .WithControlsExcluding("<Mouse>/position")
            .WithControlsExcluding("<Mouse>/delta")
            .OnMatchWaitForAnother(0.1f)
            .OnCancel(operation => RebindCanceled())
            .OnComplete(operation => RebindCompleted(index))
             ;


            if (InputIconsManagerSO.DeviceIsKeyboardOrMouse(deviceToRebind))
            {
                rebindOperation.WithCancelingThrough(gamepadCancelKey); //also allow cancelling through gamepad. Keyboard will be detected in RebindComplete method
            }
            else
            {
                rebindOperation.WithCancelingThrough(keyboardCancelKey); //also allow cancelling through keyboard. Gamepad will be detected in RebindComplete method
            }

            rebindOperation.Start();
        }


        void RebindCanceled()
        {
            //Debug.Log("rebind canceled");
            m_Action.action.Enable();
            rebindOperation.Dispose();
            rebindOperation = null;
            m_Action.action.Enable();

            ToggleGameObjectState(rebindButtonObject, true);
            ToggleGameObjectState(resetButtonObject, true);
            ToggleGameObjectState(listeningForInputObject, false);
        }

        void RebindCompleted(int bindingIndex)
        {
            //InputIconsLogger.Log("rebind completed");
            bool canceled = rebindOperation.canceled;

            //InputIconsLogger.Log(m_Action.action.bindings[bindingIndex].overridePath);

            if (!canceled)
            {
                //cancel if any of the cancel keys was presses
                if (m_Action.action.bindings[bindingIndex].overridePath == keyboardCancelKey
                    || m_Action.action.bindings[bindingIndex].overridePath == gamepadCancelKey)
                    canceled = true;

                //cancel if a wrong device was used
                if (InputIconsManagerSO.DeviceIsKeyboardOrMouse(deviceToRebind))
                {
                    if (m_Action.action.bindings[bindingIndex].overridePath.Contains("<Gamepad>"))
                        canceled = true;
                }
                else
                {
                    if (m_Action.action.bindings[bindingIndex].overridePath.Contains("<Keyboard>"))
                        canceled = true;
                }
            }

            //if InputIconsManagerSO.RebindBehaviour.CancelOverrideIfBindingAlreadyExists is selected,
            //do not allowrebinding if another action already uses this action
            if (!canceled)
            {
                if (InputIconsManagerSO.Instance.rebindBehaviour == InputIconsManagerSO.RebindBehaviour.CancelOverrideIfBindingAlreadyExists)
                {
                    InputBinding duplicateBinding = CheckDuplicateBindings(m_Action, bindingIndex);
                    if (duplicateBinding.path != "")
                    {
                        //Check if the duplicate binding is required. We can give up secondary bindings if a primary one is available
                        if (!DuplicateBindingIsSecondaryAndPrimaryAvailable(duplicateBinding))
                        {
                            InputIconsLogger.Log("Duplicate binding: " + duplicateBinding.action.ToString() + " " + duplicateBinding.effectivePath);
                            canceled = true;
                            duplicateBindingFoundOnRebindOperation?.Invoke(duplicateBinding);
                            if (keyAlreadyUsedObject)
                                keyAlreadyUsedObject.SetActive(true);

                            StartCoroutine(DisableAlreadyUsedObject());

                        }
                        else
                        {
                            //if a the duplicate binding is "hidden" - e.g. if we have "W" and "Space" for jump, W being the primary binding (the first one in the list
                            //of bindings), we can give up the secondary "Space" binding and assign it to another action instead

                            InputIconsLogger.Log("duplicate binding is secondary and primary is available. Accept rebind: " + duplicateBinding.action.ToString() + " " + duplicateBinding.effectivePath);
                        }
                    }
                }
            }

            if (canceled)
            {
                m_Action.action.ApplyBindingOverride(bindingIndex, previousBinding);
                RebindCanceled();
                return;
            }


            string device;
            string key;

            for (int i = 0; i < m_Action.action.bindings.Count; i++)
            {
                m_Action.action.GetBindingDisplayString(i, out device, out key);
            }

            InputIconsManagerSO.SaveUserBindings();

            rebindOperation.Dispose();
            rebindOperation = null;
            m_Action.action.Enable();

            onRebindOperationCompleted?.Invoke(this);
            InputIconsManagerSO.HandleInputBindingsChanged();

            ToggleGameObjectState(rebindButtonObject, true);
            ToggleGameObjectState(resetButtonObject, true);
            ToggleGameObjectState(listeningForInputObject, false);
        }

        IEnumerator DisableAlreadyUsedObject()
        {
            yield return new WaitForSecondsRealtime(0.7f);
            if (keyAlreadyUsedObject)
                keyAlreadyUsedObject.SetActive(false);

        }

        private InputBinding CheckDuplicateBindings(InputAction action, int bindingIndex)
        {

            if (InputIconsManagerSO.Instance.checkOnlySameActionMapsOnBindingRebound)
            {
                InputBinding foundBinding = CheckDuplicateBindings(action.actionMap, action, bindingIndex);
                if (foundBinding.path != "")
                    return foundBinding;
            }
            else
            {
                InputActionAsset actionAsset = action.actionMap.asset;
                foreach (InputActionMap actionMap in actionAsset.actionMaps)
                {
                    InputBinding foundBinding = CheckDuplicateBindings(actionMap, action, bindingIndex);
                    if (foundBinding.path != "")
                        return foundBinding;
                }
            }


            return new InputBinding("");
        }

        private InputBinding CheckDuplicateBindings(InputActionMap actionMap, InputAction action, int bindingIndex)
        {
            InputBinding bindingToCheck = action.bindings[bindingIndex];
            if (bindingToCheck == null)
                return new InputBinding("");

            foreach (InputBinding binding in actionMap.bindings)
            {
                if (binding == bindingToCheck)
                {
                    if (!binding.isPartOfComposite)
                        continue;

                    if (binding.action == bindingToCheck.action)
                    {
                        if (binding.id == bindingToCheck.id)
                            continue;
                    }
                }

                if (bindingToCheck.effectivePath == "") continue;


                if (binding.effectivePath == bindingToCheck.effectivePath) //duplicate binding found
                {
                    return binding;
                }
            }

            return new InputBinding("");
        }

        public bool DuplicateBindingIsSecondaryAndPrimaryAvailable(InputBinding duplicateBinding)
        {
            InputAction action = GetActionOfBinding(duplicateBinding);
            //Debug.Log("count: "+action.bindings.Count);
            for (int i = 0; i < action.bindings.Count; i++)
            {

                if (action.bindings[i].groups == duplicateBinding.groups)
                {
                    //Debug.Log("compare: " + action.bindings[i].effectivePath + " " + duplicateBinding.effectivePath);
                    if (action.bindings[i].id == duplicateBinding.id)
                    {
                        return false;
                    }
                    else
                    {
                        if (action.bindings[i].effectivePath != "")
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public InputAction GetActionOfBinding(InputBinding inputBinding)
        {
            if (InputIconsManagerSO.Instance.checkOnlySameActionMapsOnBindingRebound)
            {
                foreach (InputAction action in m_Action.action.actionMap.actions)
                {
                    foreach (InputBinding binding in action.bindings)
                    {
                        if (binding == inputBinding)
                            return action;
                    }
                }
            }
            else
            {
                InputActionAsset actionAsset = m_Action.action.actionMap.asset;
                foreach (InputActionMap map in actionAsset.actionMaps)
                {
                    foreach (InputAction action in map.actions)
                    {
                        foreach (InputBinding binding in action.bindings)
                        {
                            if (binding == inputBinding)
                                return action;
                        }
                    }
                }

            }

            return null;
        }

        public void HandleAnyRebindOperationCompleted(II_UIRebindInputActionBehaviour rebindBehaviour)
        {
            if (ignoreOtherButtons)
            {
                return;
            }


            if (InputIconsManagerSO.Instance.rebindBehaviour == InputIconsManagerSO.RebindBehaviour.AlwaysApplyAndKeepOtherBindings)
                return;

            if (!canBeRebound)
                return;

            if (rebindBehaviour == this)
                return;

            if (InputIconsManagerSO.Instance.checkOnlySameActionMapsOnBindingRebound)
            {
                if (rebindBehaviour.actionReference.action.actionMap != m_Action.action.actionMap)
                    return;
            }


            if (rebindBehaviour.actionReference.action == m_Action.action)
            {
                if (ActionIsComposite(rebindBehaviour.actionReference.action))
                {
                    if (rebindBehaviour.m_BindingType == m_BindingType)
                        return;
                }
                else
                    return;
            }

            if (rebindBehaviour.deviceType != deviceType)
            {
                //Debug.Log(rebindBehaviour.deviceType + " -- " + deviceType);
                return;
            }

            string controlScheme;


            if (m_RebindType == InputIconsUtility.DeviceType.KeyboardAndMouse || InputIconsManagerSO.CurrentInputDeviceIsKeyboard())
                controlScheme = InputIconsManagerSO.GetKeyboardControlSchemeName(keyboardControlSchemeIndex);
            else
                controlScheme = InputIconsManagerSO.GetGamepadControlSchemeName(gamepadControlSchemeIndex);

            List<InputBinding> newBinding = GetBindings(rebindBehaviour.actionReference, rebindBehaviour.bindingType, controlScheme);
            List<InputBinding> myBindings = GetBindings(m_Action, m_BindingType, controlScheme);

            for (int i = 0; i < newBinding.Count; i++)
            {
                for (int j = 0; j < myBindings.Count; j++)
                {
                    //remove my binding if user bound another action to the same key as my current binding
                    if (newBinding[i].effectivePath == myBindings[j].effectivePath)
                    {
                        if (newBinding[i].id != myBindings[j].id)
                        {
                            //Debug.Log("bindings are equal: "+rebindBehaviour.bindingType+newBinding[i]+" old: "+m_BindingType+myBindings[j]);
                            int bindingIndex = GetIndexOfInputBinding(m_Action.action, myBindings[j]);
                            m_Action.action.ApplyBindingOverride(bindingIndex, "");

                            InputIconsManagerSO.SaveUserBindings();
                        }
                    }
                }
            }
        }

        public void ButtonPressedResetBinding()
        {
            ResetBinding();
        }

        public void ResetBinding()
        {
            InputActionRebindingExtensions.RemoveAllBindingOverrides(m_Action.action);
            onRebindOperationCompleted?.Invoke(this);

            InputIconsManagerSO.HandleInputBindingsChanged();
            InputIconsManagerSO.SaveUserBindings();
        }


        public void UpdateBindingDisplay()
        {
            if (m_Action == null)
                return;

            if (m_Action.action == null)
                return;

            string bindingName = m_Action.action.actionMap.name+"/"+m_Action.action.name;

            if (InputIconsUtility.ActionIsComposite(m_Action.action))
            {
                bindingName += "/" + bindingType.ToString().ToLower();
            }

            InputStyleData style;
            bool gamepad = false;

            if (deviceType == InputIconsUtility.DeviceType.Gamepad)
            {
                gamepad = true;
            }

            //Display Sprites if sprites selected
            if (displayType == DisplayType.Sprites)
            {
                if (deviceType == InputIconsUtility.DeviceType.Auto)
                {
                    style = InputIconsManagerSO.GetInputStyleData(bindingName, m_bindingIndex);
                }
                else
                {
                    style = InputIconsManagerSO.GetInputStyleDataSpecific(bindingName, gamepad, m_bindingIndex);
                }

                string tmp_Text = "";
                if (style != null)
                {
                    tmp_Text = style.inputStyleString_singleInput;
                }
                bindingNameDisplayText.SetText(tmp_Text);
            }
            else if (displayType == DisplayType.Font) //Display Fonts
            {
                if (deviceType == InputIconsUtility.DeviceType.Auto)
                {
                    if (!InputIconsManagerSO.DeviceIsKeyboardOrMouse(InputIconsManagerSO.GetCurrentInputDevice()))
                    {
                        gamepad = true;
                    }
                }

                string fontOutput = GetFontDisplaySingleInputDeviceSpecific(bindingName, gamepad, m_bindingIndex);
                fontOutput = Regex.Unescape(fontOutput);
                bindingNameDisplayText.text = fontOutput;
            }
        }

        public void UpdateBehaviour()
        {
            UpdateBindingDisplay();

            rebindButtonObject.GetComponent<Button>().interactable = canBeRebound;
        }


        void ToggleGameObjectState(GameObject targetGameObject, bool newState)
        {
            if (targetGameObject)
                targetGameObject.SetActive(newState);
        }

        public int GetNumberOfAvailableBindings()
        {
            string constrolScheme = InputIconsManagerSO.GetKeyboardControlSchemeName(keyboardControlSchemeIndex);
            if (deviceType == InputIconsUtility.DeviceType.Gamepad)
            {
                constrolScheme = InputIconsManagerSO.GetGamepadControlSchemeName(gamepadControlSchemeIndex);
            }

            if (deviceType == InputIconsUtility.DeviceType.Auto)
            {
                if (!InputIconsManagerSO.DeviceIsKeyboardOrMouse(InputIconsManagerSO.GetCurrentInputDevice()))
                {
                    constrolScheme = InputIconsManagerSO.GetGamepadControlSchemeName(gamepadControlSchemeIndex);
                }
            }

            CompositeType compositeType = CompositeType.NonComposite;
            if (InputIconsUtility.ActionIsComposite(m_Action))
                compositeType = CompositeType.Composite;

            return InputIconsUtility.GetIndexesOfBindingTypeDeviceSpecific(m_Action, compositeType, m_BindingType, constrolScheme).Count - 1;
        }

        public void OnValidate()
        {
            int maxID = GetNumberOfAvailableBindings();
            if (m_bindingIndex > maxID)
            {
                m_bindingIndex = maxID;
                UpdateBindingDisplay();
            }
            if (m_bindingIndex < 0)
            {
                m_bindingIndex = 0;
                UpdateBindingDisplay();
            }
        }
    }
}
