using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pause
{
    public class PauseManagement{
        private static float unpausedTimeScale;
        public static bool paused = false;
        
        public static void Pause(){
            unpausedTimeScale = Time.timeScale;
            
            // Stop time
            Time.timeScale = 0;
            
            // Free and reveal the cursor
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;

            paused = true;
        }

        public static void Unpause(){
            // Resume time at the scale that was being used before we paused
            Time.timeScale = unpausedTimeScale;

            // Lock and conceal the cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            paused = false;
        }
    }
}
