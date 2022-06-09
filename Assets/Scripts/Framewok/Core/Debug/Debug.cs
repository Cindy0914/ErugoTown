using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Util
{
    public static class Debug
    {
        [Conditional("UNITY_EDITOR")]
        public static void Log<T>(T message)
            => UnityEngine.Debug.Log("******************" + message);
    }
}
