using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Pause
{
    public static class PauseManagement{
        private static float unpausedTimeScale;
        private static PlayerInput playerInput;
        private static string previousInputActionMap;
        public static bool paused = false;
        
        public static void Pause(PlayerInput pI){
            unpausedTimeScale = Time.timeScale;
            
            // Stop time
            Time.timeScale = 0;

            // Switch to menu action map
            // playerInput = pI;
            // previousInputActionMap = playerInput.currentActionMap.name;
            // playerInput.SwitchCurrentActionMap("menu");

            // Free and reveal the cursor
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;

            paused = true;
        }

        public static void Unpause(){
            // Resume time at the scale that was being used before we paused
            Time.timeScale = unpausedTimeScale;

            // Return to previous action map
            // playerInput.SwitchCurrentActionMap(previousInputActionMap);

            // Lock and conceal the cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            paused = false;
        }
    }
}
