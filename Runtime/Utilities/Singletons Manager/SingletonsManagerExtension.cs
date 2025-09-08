using Utilities;

public static class SingletonsManagerExtension
{
    public static T GetSingleton<T>(this UnityEngine.MonoBehaviour monoBehaviour, bool findIfNotExists = false) where T: class
    {
        return SingletonsManager.Instance.GetSingleton<T>(findIfNotExists);
    }
}
