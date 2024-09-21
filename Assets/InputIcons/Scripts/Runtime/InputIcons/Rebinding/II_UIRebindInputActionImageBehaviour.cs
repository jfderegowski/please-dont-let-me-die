using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using static InputIcons.InputIconsUtility;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using System;
using System.Reflection;

namespace InputIcons
{
    public class II_UIRebindInputActionImageBehaviour : MonoBehaviour
    {

        public II_RebindData rebindData;



        private string previousBinding;



        private static InputActionRebindingExtensions.RebindingOperation rebindOperation;

        public delegate void OnRebindOperationCompleted(II_UIRebindInputActionImageBehaviour rebindBehaviour);
        public static OnRebindOperationCompleted onRebindOperationCompleted;

        //subscribe to react and display a message like "Key already bound to Jump action" for example
        public static UnityEvent<InputBinding> duplicateBindingFoundOnRebindOperation;

        private static InputDevice deviceToRebind;


        private void Awake()
        {

            //UpdateBehaviour();
        }

        private void OnEnable()
        {
            if (!rebindData.ignoreOtherButtons)
                onRebindOperationCompleted += HandleAnyRebindOperationCompleted;

            UpdateBehaviour();
            InputIconsManagerSO.onControlsChanged += HandleControlsChanged;
            InputIconsManagerSO.onBindingsChanged += UpdateBehaviour;
            InputIconsManagerSO.onBindingsReset += UpdateBehaviourDelayed;
        }

        private void OnDisable()
        {
            onRebindOperationCompleted -= HandleAnyRebindOperationCompleted;

            InputIconsManagerSO.onControlsChanged -= HandleControlsChanged;
            InputIconsManagerSO.onBindingsChanged -= UpdateBehaviour;
            InputIconsManagerSO.onBindingsReset -= UpdateBehaviourDelayed;
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
            if (!rebindData.canBeRebound)
                return;

            StartRebindProcess();
        }

       

        void StartRebindProcess()
        {
            InputDevice device = InputIconsManagerSO.GetCurrentInputDevice();
            if (rebindData.deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse && !InputIconsManagerSO.DeviceIsKeyboardOrMouse(device))
                return;

            if (rebindData.deviceType == InputIconsUtility.DeviceType.Gamepad && !(device is Gamepad))
                return;

            if (rebindOperation != null)
                rebindOperation.Cancel();



            ToggleGameObjectState(rebindData.rebindButtonObject, false);
            ToggleGameObjectState(rebindData.resetButtonObject, false);
            ToggleGameObjectState(rebindData.listeningForInputObject, true);

            rebindData.actionReference.action.Disable();

            //do not allow keyboard/mouse buttons, when rebind operation was started with gamepad and vice versa
            //and only allow the correct device to bind when the device for this button is keyboard/mouse or gamepad
            deviceToRebind = device;


            int index = rebindData.GetMyBindingIndex();
            rebindOperation = rebindData.actionReference.action.PerformInteractiveRebinding(index);
            previousBinding = rebindData.actionReference.action.bindings[index].path;


            rebindOperation
            .WithControlsExcluding("<Mouse>/position")
            .WithControlsExcluding("<Mouse>/delta")
            .OnMatchWaitForAnother(0.1f)
            .OnCancel(operation => RebindCanceled())
            .OnComplete(operation => RebindCompleted(index))
             ;


            if (InputIconsManagerSO.DeviceIsKeyboardOrMouse(deviceToRebind))
            {
                rebindOperation.WithCancelingThrough(rebindData.gamepadCancelKey); //also allow cancelling through gamepad. Keyboard will be detected in RebindComplete method
            }
            else
            {
                rebindOperation.WithCancelingThrough(rebindData.keyboardCancelKey); //also allow cancelling through keyboard. Gamepad will be detected in RebindComplete method
            }

            rebindOperation.Start();
        }


        void RebindCanceled()
        {
            //Debug.Log("rebind canceled");
            rebindData.actionReference.action.Enable();
            rebindOperation.Dispose();
            rebindOperation = null;
            rebindData.actionReference.action.Enable();

            ToggleGameObjectState(rebindData.rebindButtonObject, true);
            ToggleGameObjectState(rebindData.resetButtonObject, true);
            ToggleGameObjectState(rebindData.listeningForInputObject, false);
        }

        void RebindCompleted(int bindingIndex)
        {
            //InputIconsLogger.Log("rebind completed");
            bool canceled = rebindOperation.canceled;

            //InputIconsLogger.Log(m_Action.action.bindings[bindingIndex].overridePath);

            if (!canceled)
            {
                //cancel if any of the cancel keys was presses
                if (rebindData.actionReference.action.bindings[bindingIndex].overridePath == rebindData.keyboardCancelKey
                    || rebindData.actionReference.action.bindings[bindingIndex].overridePath == rebindData.gamepadCancelKey)
                    canceled = true;

                //cancel if a wrong device was used
                if (InputIconsManagerSO.DeviceIsKeyboardOrMouse(deviceToRebind))
                {
                    if (rebindData.actionReference.action.bindings[bindingIndex].overridePath.Contains("<Gamepad>"))
                        canceled = true;
                }
                else
                {
                    if (rebindData.actionReference.action.bindings[bindingIndex].overridePath.Contains("<Keyboard>"))
                        canceled = true;
                }
            }

            //if InputIconsManagerSO.RebindBehaviour.CancelOverrideIfBindingAlreadyExists is selected,
            //do not allowrebinding if another action already uses this action
            if (!canceled)
            {
                if (InputIconsManagerSO.Instance.rebindBehaviour == InputIconsManagerSO.RebindBehaviour.CancelOverrideIfBindingAlreadyExists)
                {
                    InputBinding duplicateBinding = CheckDuplicateBindings(rebindData.actionReference, bindingIndex);
                    if (duplicateBinding.path != "")
                    {
                        //Check if the duplicate binding is required. We can give up secondary bindings if a primary one is available
                        if (!DuplicateBindingIsSecondaryAndPrimaryAvailable(duplicateBinding))
                        {
                            InputIconsLogger.Log("Duplicate binding: " + duplicateBinding.action.ToString() + " " + duplicateBinding.effectivePath);
                            canceled = true;
                            duplicateBindingFoundOnRebindOperation?.Invoke(duplicateBinding);
                            if (rebindData.keyAlreadyUsedObject)
                                rebindData.keyAlreadyUsedObject.SetActive(true);

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
                rebindData.actionReference.action.ApplyBindingOverride(bindingIndex, previousBinding);
                RebindCanceled();
                return;
            }

            /*
            string device;
            string key;

            for (int i = 0; i < actionReference.action.bindings.Count; i++)
            {
                actionReference.action.GetBindingDisplayString(i, out device, out key);
            }
            */

            InputIconsManagerSO.SaveUserBindings();

            rebindOperation.Dispose();
            rebindOperation = null;
            rebindData.actionReference.action.Enable();

            onRebindOperationCompleted?.Invoke(this);
            InputIconsManagerSO.HandleInputBindingsChanged();

            ToggleGameObjectState(rebindData.rebindButtonObject, true);
            ToggleGameObjectState(rebindData.resetButtonObject, true);
            ToggleGameObjectState(rebindData.listeningForInputObject, false);
        }

        IEnumerator DisableAlreadyUsedObject()
        {
            yield return new WaitForSecondsRealtime(0.7f);
            if (rebindData.keyAlreadyUsedObject)
                rebindData.keyAlreadyUsedObject.SetActive(false);

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
                foreach (InputAction action in rebindData.actionReference.action.actionMap.actions)
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
                InputActionAsset actionAsset = rebindData.actionReference.action.actionMap.asset;
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

        public void HandleAnyRebindOperationCompleted(II_UIRebindInputActionImageBehaviour rebindBehaviour)
        {
            if (rebindData.ignoreOtherButtons || rebindBehaviour.rebindData.actionReference == null || rebindData.actionReference == null)
            {
                return;
            }


            if (InputIconsManagerSO.Instance.rebindBehaviour == InputIconsManagerSO.RebindBehaviour.AlwaysApplyAndKeepOtherBindings)
                return;

            if (!rebindData.canBeRebound)
                return;

            if (rebindBehaviour == this)
                return;

            if (InputIconsManagerSO.Instance.checkOnlySameActionMapsOnBindingRebound)
            {
                if (rebindBehaviour.rebindData.actionReference.action.actionMap != rebindData.actionReference.action.actionMap)
                    return;
            }


            if (rebindBehaviour.rebindData.actionReference.action == rebindData.actionReference.action)
            {
                if (ActionIsComposite(rebindBehaviour.rebindData.actionReference.action))
                {
                    if (rebindBehaviour.rebindData.bindingTypeKeyboard == rebindData.bindingTypeKeyboard)
                        return;
                }
                else
                    return;
            }

            if (rebindBehaviour.rebindData.deviceType != rebindData.deviceType
                && rebindBehaviour.rebindData.deviceType != InputIconsUtility.DeviceType.Auto
                && rebindData.deviceType != InputIconsUtility.DeviceType.Auto)
            {
                //Debug.Log(rebindBehaviour.deviceType + " -- " + deviceType);
                return;
            }

            string controlScheme;


            if (rebindData.deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse || InputIconsManagerSO.CurrentInputDeviceIsKeyboard())
                controlScheme = InputIconsManagerSO.GetKeyboardControlSchemeName(rebindData.controlSchemeIndexKeyboard);
            else
                controlScheme = InputIconsManagerSO.GetGamepadControlSchemeName(rebindData.controlSchemeIndexGamepad);

            List<InputBinding> newBinding = GetBindings(rebindBehaviour.rebindData.actionReference, rebindBehaviour.rebindData.bindingTypeKeyboard, controlScheme);
            List<InputBinding> myBindings = GetBindings(rebindData.actionReference, rebindData.bindingTypeKeyboard, controlScheme);

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
                            int bindingIndex = GetIndexOfInputBinding(rebindData.actionReference.action, myBindings[j]);
                            rebindData.actionReference.action.ApplyBindingOverride(bindingIndex, "");

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
            InputActionRebindingExtensions.RemoveBindingOverride(rebindData.actionReference.asset.FindActionMap(rebindData.actionReference.action.actionMap.id).FindAction(rebindData.actionReference.action.id), rebindData.GetMyBindingIndex());
            onRebindOperationCompleted?.Invoke(this);

            InputIconsManagerSO.SaveUserBindings();
            InputIconsManagerSO.HandleInputBindingsChanged();
            
        }


        public void UpdateBindingDisplay()
        {
            if (rebindData.actionReference == null)
                return;

            if (rebindData.actionReference.action == null)
                return;

            InputIconSetBasicSO currentIconSet = InputIconSetConfiguratorSO.GetCurrentIconSet();

            string keyboardControlScheme = InputIconsManagerSO.GetKeyboardControlSchemeName(rebindData.controlSchemeIndexKeyboard);
            string gamepadControlScheme = InputIconsManagerSO.GetGamepadControlSchemeName(rebindData.controlSchemeIndexGamepad);


            string keyName = "";

            if (rebindData.bindingSearchStrategy == II_RebindData.BindingSearchStrategy.BindingType)
            {
                if (rebindData.deviceType == InputIconsUtility.DeviceType.KeyboardAndMouse)
                {
                    currentIconSet = InputIconSetConfiguratorSO.Instance.keyboardIconSet;
                    keyName = InputIconsUtility.GetSpriteName(rebindData.actionReference, rebindData.bindingTypeKeyboard, keyboardControlScheme, rebindData.bindingIDInAvailableListKeyboard);
                }
                else if (rebindData.deviceType == InputIconsUtility.DeviceType.Gamepad)
                {
                    currentIconSet = InputIconSetConfiguratorSO.GetLastUsedGamepadIconSet();
                    keyName = InputIconsUtility.GetSpriteName(rebindData.actionReference, rebindData.bindingTypeGamepad, gamepadControlScheme, rebindData.bindingIDInAvailableListGamepad);
                }
                else
                {
                    string currentControlScheme = keyboardControlScheme;
                    BindingType bindingType = rebindData.bindingTypeKeyboard;
                    int bindingID = rebindData.bindingIDInAvailableListKeyboard;

                    if (currentIconSet is InputIconSetGamepadSO)
                    {
                        currentControlScheme = gamepadControlScheme;
                        bindingType = rebindData.bindingTypeGamepad;
                        bindingID = rebindData.bindingIDInAvailableListGamepad;
                    }
                       

                    keyName = InputIconsUtility.GetSpriteName(rebindData.actionReference, bindingType, currentControlScheme, bindingID);
                }

            }
            else if (rebindData.bindingSearchStrategy == II_RebindData.BindingSearchStrategy.BindingIndex)
            {
                if (rebindData.deviceType == InputIconsUtility.DeviceType.Gamepad
                    || (rebindData.deviceType == InputIconsUtility.DeviceType.Auto && currentIconSet is InputIconSetGamepadSO))
                {
                    rebindData.bindingDisplayImage.sprite = InputIconsUtility.GetSpriteByBindingIndex(rebindData.actionReference.action, rebindData.bindingIndexGamepad, rebindData.optionalKeyboardIconSet, rebindData.optionalGamepadIconSet);
                    return;
                }
                else
                {
                    rebindData.bindingDisplayImage.sprite = InputIconsUtility.GetSpriteByBindingIndex(rebindData.actionReference.action, rebindData.bindingIndexKeyboard, rebindData.optionalKeyboardIconSet, rebindData.optionalGamepadIconSet);
                    return;
                }
            }

            if (currentIconSet is InputIconSetKeyboardSO && rebindData.optionalKeyboardIconSet != null)
                currentIconSet = rebindData.optionalKeyboardIconSet;

            if (currentIconSet is InputIconSetGamepadSO && rebindData.optionalGamepadIconSet != null)
                currentIconSet = rebindData.optionalGamepadIconSet;

            if (rebindData.bindingDisplayImage)
                rebindData.bindingDisplayImage.sprite = currentIconSet.GetSprite(keyName);
            //bindingNameDisplayText.SetText(tmp_Text);


        }

        public void UpdateBehaviourDelayed()
        {
            StartCoroutine(WaitAndUpdateBehavior());
        }

        IEnumerator WaitAndUpdateBehavior()
        {
            yield return new WaitForEndOfFrame();
            UpdateBehaviour();
        }

        public void UpdateBehaviour()
        {
            UpdateBindingDisplay();

            if(rebindData.rebindButtonObject)
                rebindData.rebindButtonObject.GetComponent<Button>().interactable = rebindData.canBeRebound;
        }


        void ToggleGameObjectState(GameObject targetGameObject, bool newState)
        {
            if (targetGameObject)
                targetGameObject.SetActive(newState);
        }

       

        public void OnValidate()
        {
            UpdateBindingDisplay();
        }
    }
}