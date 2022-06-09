using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public static class Util
    {
        public static IEnumerator VibrateController(float waitTime, float frequency, float amplitude, OVRInput.Controller controller = OVRInput.Controller.Active)
        {
            OVRInput.SetControllerVibration(frequency, amplitude, controller);
            yield return new WaitForSeconds(waitTime);
            OVRInput.SetControllerVibration(0, 0, controller);
        }

        public static IEnumerator ViberateRepeat(int repeatCount, float waitTime, float frequency, float amplitude, OVRInput.Controller controller = OVRInput.Controller.Active)
        {
            int count = 0;
            while (count < repeatCount)
            {
                OVRInput.SetControllerVibration(frequency, amplitude, controller);
                yield return new WaitForSeconds(waitTime);
                OVRInput.SetControllerVibration(0, 0, controller);
                count++;
                yield return new WaitForSeconds(waitTime);
            }
        }
    }
}
