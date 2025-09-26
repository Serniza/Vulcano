using Utilities;
using UnityEngine;

public static class SingletonsManagerExtension
{
    public static T GetSingleton<T>(this MonoBehaviour monoBehaviour) where T: class
    {
        return SingletonsManager.Instance.GetSingleton<T>();
    }
}
