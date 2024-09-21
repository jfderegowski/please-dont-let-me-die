using UnityEngine;
using UnityEngine.InputSystem;


// This is an example script, showing you how you can react to device changes (new and removed devices)
// The InputIconsManagerSO manages a list containing playerIDs, devices and device descriptions
// When a device connects, we try to (re-)assign it to a playerID which is managed by the InputIconsManagerSO
// When a device disconnects, we remove that device from the list on the InputIconsManagerSO
// The playerIDs and corresponding devices can be used to display user specific icons for input prompts (like in the II_LocalMultiplayerSpritePrompt script)

namespace InputIcons
{
    public class II_LocalMultiplayerUserManager : MonoBehaviour
    {
        [Header("-- This component handles (re-)connecting devices --", order = 1)]
        [Space(10, order = 2)]
        public int maxPlayers = 4;

        void Start()
        {
            // Get all connected input devices and assign players
            InitializesConnectedDevices();

            // Listen for device changes (connection/disconnection)
            InputSystem.onDeviceChange += OnDeviceChange;
        }


        void OnDestroy()
        {
            // Unsubscribe from the onDeviceChange event when the object is destroyed
            InputSystem.onDeviceChange -= OnDeviceChange;
        }

        //Go through all connected devices (e.g. keyboard/mouse, XBox controller, PS controller, ...) and assign a player to them
        //Depending on your needs, you might want to ignore some devices.
        public void InitializesConnectedDevices()
        {
            // Get all connected input devices
            InputDevice[] devices = InputSystem.devices.ToArray();
            bool keyboardOrMouseAssigned = false;
            int playerID = 0;

            // Assign players to connected devices
            for (int i = 0; i < devices.Length; i++)
            {
                if (playerID >= maxPlayers)
                    break;

                // Treat mouse and keyboard as the same device
                if (InputIconsManagerSO.DeviceIsKeyboardOrMouse(devices[i]))
                {
                    if (keyboardOrMouseAssigned)
                    {
                        continue;
                    }
                    keyboardOrMouseAssigned = true;
                }

                InputIconsLogger.Log("adding user device: " + devices[i].description.ToString() + " " + devices[i].name.ToString() + " - Player ID: " + playerID);
                InputIconsManagerSO.localMultiplayerManagement.AssignDeviceToPlayer(playerID, devices[i], true);
                playerID++;
            }
        }

        //We have subscribed to the onDeviceChange event
        //Handle lost connections or new controllers here
        public void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (change == InputDeviceChange.Disconnected)
            {
                UnassignDeviceFromPlayer(device);
            }
            else if (change == InputDeviceChange.Added)
            {
                AddUserForConnectedDevice(device);
            }
        }

        //Remove device from the list of playerIDs/devices/devicedescriptions.
        //The playerID and devicedescription remains so we can reassign a reconnecting device to the original playerID
        public void UnassignDeviceFromPlayer(InputDevice disconnectedDevice)
        {
            int playerID = InputIconsManagerSO.localMultiplayerManagement.GetPlayerIDForDevice(disconnectedDevice);

            if (playerID != -1)
            {
                InputIconsLogger.Log("unassign device: " + disconnectedDevice.description.ToString() + " ... " + playerID);
                InputIconsManagerSO.localMultiplayerManagement.UnassignDeviceFromPlayer(playerID);
            }
        }

        //Try to assign a device to a playerID. Only works if there aren't too many players already.
        //You might want to change this based on your needs.
        public void AddUserForConnectedDevice(InputDevice connectedDevice)
        {
            // Assign a player for the connected device
            int availablePlayerID = GetNextAvailablePlayerID();

            if (availablePlayerID != -1)
            {
                InputIconsLogger.Log("add user for device: " + connectedDevice.description.ToString() + " ... " + connectedDevice.name.ToString() + " " + availablePlayerID);
                InputIconsManagerSO.localMultiplayerManagement.AssignDeviceToPlayer(availablePlayerID, connectedDevice, true);
            }
            else
            {
                InputIconsLogger.Log("assigning device failed, no player slot available for device: " + connectedDevice.description.ToString());
            }

        }

        public int GetNextAvailablePlayerID()
        {
            // You can implement your logic to find the next available player ID here
            // For example, iterate through existing players and find an unused ID
            // Return -1 if no available ID is found (adjust as per your implementation)
            // This is a sample implementation, you might need to adjust based on your needs
            for (int i = 0; i < maxPlayers; i++)
            {
                if (InputIconsManagerSO.localMultiplayerManagement.PlayerHasNoDevice(i))
                {
                    return i;
                }
            }
            return -1; // No available player ID found
        }
    }
}
