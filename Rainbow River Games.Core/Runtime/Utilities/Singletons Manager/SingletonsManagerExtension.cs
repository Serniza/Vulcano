using Utilities;

public static class SingletonsManagerExtension
{
    public static T GetSingleton<T>(this UnityEngine.MonoBehaviour monoBehaviour, bool findInTheCurrentSceneIfDoesNotExist = true, bool createItIfDoesNotExist = true, bool createLikeAPermanent = false) where T: UnityEngine.MonoBehaviour
    {
        return SingletonsManager._instance.GetSingleton<T>(findInTheCurrentSceneIfDoesNotExist, createItIfDoesNotExist, createLikeAPermanent);
    }

    public static T GetSingleton<T>(this ScriptableObject scriptableObject, bool findInTheCurrentSceneIfDoesNotExist = true, bool createItIfDoesNotExist = true, bool createLikeAPermanent = false) where T : UnityEngine.MonoBehaviour
    {
        return SingletonsManager._instance.GetSingleton<T>(findInTheCurrentSceneIfDoesNotExist, createItIfDoesNotExist, createLikeAPermanent);
    }
}
