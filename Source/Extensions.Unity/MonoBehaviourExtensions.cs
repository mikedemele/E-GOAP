using System.Diagnostics;
using System.Reflection;

using UnityEngine;

namespace EGoap.Source.Extensions.Unity
{
    public static class MonoBehaviourExtensions
    {
        [Conditional("UNITY_EDITOR")]
        public static void EnsureRequiredFieldsAreSetInEditor(this MonoBehaviour monoBehaviour)
        {
            foreach (var unused in monoBehaviour.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
            }
        }
    }
}