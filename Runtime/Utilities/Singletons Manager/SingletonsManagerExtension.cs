using Utilities;

public static class SingletonsManagerExtension
{
    public static T GetSingleton<T>(this UnityEngine.MonoBehaviour monoBehaviour, bool findIfNotExists = true) where T: class
    {
        return SingletonsManager._instance.GetSingleton<T>(findIfNotExists);
    }

    public static T GetSingleton<T>(this ScriptableObject scriptableObject, bool findIfNotExists = true) where T : class
    {
        return SingletonsManager._instance.GetSingleton<T>(findIfNotExists);
    }
}
