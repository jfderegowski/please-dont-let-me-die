using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputIcons
{
    public class II_ShowDeviceIconsAfterDelay : MonoBehaviour
    {
        private static II_ShowDeviceIconsAfterDelay instance;

        private Coroutine deviceChangeCoroutine;
        private bool deviceChangeScheduled = false;

        public string deviceToDisplay;
        public static II_ShowDeviceIconsAfterDelay Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<II_ShowDeviceIconsAfterDelay>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject("II_DeviceDisplayChanger");
                        instance = obj.AddComponent<II_ShowDeviceIconsAfterDelay>();
                        DontDestroyOnLoad(obj);
                    }

                }
                return instance;

            }
            set => instance = value;
        }

        public void ScheduleDeviceDisplayChange(InputDevice newDevice, float delay)
        {
            if (deviceChangeScheduled)
                return;

            deviceChangeCoroutine = StartCoroutine(WaitAndShowDevice(newDevice, delay));
        }

        public void CancelScheduledDeviceChange()
        {
            deviceChangeScheduled = false;
            if (deviceChangeCoroutine != null)
            {
                StopCoroutine(deviceChangeCoroutine);
            }
        }

        IEnumerator WaitAndShowDevice(InputDevice device, float delay)
        {
            deviceChangeScheduled = true;
            deviceToDisplay = device.displayName;
            //Debug.Log("device change scheduled: " + device.displayName);
            yield return new WaitForSecondsRealtime(delay);
            if (deviceChangeScheduled)
            {
                InputIconsManagerSO.SetDeviceAndRefreshDisplayedIcons(device);
                //Debug.Log("should be changing to device: " + device.displayName);
            }
            deviceChangeScheduled = false;
        }

        public void ShowGamepadIconsAfterDelay()
        {
            StartCoroutine(WaitAndSwitchToGamepadIcons());
        }

        IEnumerator WaitAndSwitchToGamepadIcons()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            InputIconsManagerSO.ShowGamepadIconsIfGamepadAvailable();
        }

        public void ShowKeyboardIconsAfterDelay()
        {
            StartCoroutine(WaitAndSwitchToKeyboardIcons());
        }

        IEnumerator WaitAndSwitchToKeyboardIcons()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            InputIconsManagerSO.ShowKeyboardIconsIfKeyboardAvailable();
        }

        public void DestroyAfterDelay(float delay)
        {
            Destroy(this.gameObject, delay);
        }
    }
}
