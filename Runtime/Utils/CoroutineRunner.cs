using System.Collections;
using UnityEngine;

namespace PlayFabToolkit.Utils
{
    /// <summary>
    /// A utility class to run coroutines from non-MonoBehaviour classes.
    /// </summary>
    /// <remarks>
    /// This class creates a GameObject that persists across scenes to run coroutines.
    /// Use the static Run method to start a coroutine.
    /// </remarks>
    /// <example>
    /// CoroutineRunner.Run(MyCoroutine());
    /// </example>
    /// <seealso cref="MonoBehaviour"/>
    /// <seealso cref="IEnumerator"/>
    /// </example>
    public class CoroutineRunner : MonoBehaviour
    {
        private static CoroutineRunner _instance;

        public static void Run(IEnumerator coroutine)
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("CoroutineRunner");
                _instance = go.AddComponent<CoroutineRunner>();
                DontDestroyOnLoad(go);
            }

            _instance.StartCoroutine(coroutine);
        }
    }
}