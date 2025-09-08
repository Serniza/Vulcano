using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utilities
{
    public class SingletonsManager : UnityEngine.MonoBehaviour
    {
        #region Variables & Properties

        static SingletonsManager instance;
        public static SingletonsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<SingletonsManager>();

                    if (instance == null)
                        instance = new GameObject("Singletons Manager", typeof(SingletonsManager)).GetComponent<SingletonsManager>();
                    else
                    {
                        if (instance.gameObject.transform.parent != null)
                            instance.gameObject.transform.SetParent(null, true);
                    }

                    instance.RegisterMonoBehaviours();

                    DontDestroyOnLoad(instance.gameObject);
                }

                return instance;
            }
            set
            {
                if (instance == null)
                {
                    instance = value;

                    if (instance.gameObject.transform.parent != null)
                        instance.gameObject.transform.SetParent(null, true);

                    instance.RegisterMonoBehaviours();

                    DontDestroyOnLoad(instance.gameObject);
                }
                else if (value != instance)
                    Destroy(value.gameObject);
            }
        }

        [SerializeField] List<SingletonData> singletonsData;
        Dictionary<Type, UnityEngine.MonoBehaviour> Singletons { get; set; }

        #endregion

        void Awake()
        {
            Instance = this;
        }

        void OnEnable()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        void OnDisable()
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        public void RegisterMonoBehaviours()
        {
			if (singletonsData == null)
				singletonsData = new List<SingletonData>();

			Singletons = new Dictionary<Type, UnityEngine.MonoBehaviour>();

            for (int i = 0, singletonsDataCount = singletonsData.Count; i < singletonsDataCount; i++)
            {
                SingletonData singletonData = singletonsData[i];

                if (singletonData.Type == SingletonData.defaultType)
                    continue;

				GameObject singletonGameObject = singletonData.GameObject;

				if (singletonGameObject != null)
				{
					if (singletonGameObject.scene != gameObject.scene)
					{
						GameObject singletonPrefab = singletonGameObject;

						singletonGameObject = singletonData.GameObject = Instantiate(singletonPrefab);

						singletonGameObject.transform.SetParent(gameObject.transform);

						for (int j = i + 1; j < singletonsDataCount; j++)
						{
							SingletonData nextSingletonData = singletonsData[j];

                            if (nextSingletonData.Type == SingletonData.defaultType)
                                continue;

							GameObject nextSingletonGameObject = nextSingletonData.GameObject;

							if (nextSingletonGameObject != null
								&&
								nextSingletonGameObject == singletonPrefab)
							    nextSingletonData.GameObject = singletonGameObject;
						}
					}

					Type type = Type.GetType(singletonData.Type);

					Type interfaceType = null;

					if (singletonData.InterfaceType != SingletonData.defaultType)
						interfaceType = Type.GetType(singletonData.InterfaceType);

					if ((interfaceType == null) ? !Singletons.ContainsKey(type) : !Singletons.ContainsKey(interfaceType))
					{
						UnityEngine.MonoBehaviour monoBehaviour = (UnityEngine.MonoBehaviour)singletonGameObject.GetComponent(type);

						if (monoBehaviour != null)
						{
							Singletons.Add((interfaceType == null) ? type : interfaceType, monoBehaviour);

							if (monoBehaviour is SingletonMonoBehaviour monoBehaviourSingleton)
								monoBehaviourSingleton.OnRegister();
						}
						else
						{
                            #if UNITY_EDITOR
							Debug.LogWarning($"SingletonsManager/RegisterMonoBehaviours/Singleton:{singletonData.Type}, The game object not has this mono behaviour type");
                            #endif
						}
					}
				}
				else
				{
                    #if UNITY_EDITOR
					Debug.LogWarning($"SingletonsManager/RegisterMonoBehaviours/Singleton:{singletonData.Type}, Game object is null");
                    #endif
				}
			}
        }

		public void RegisterMonoBehaviourAsASingleton<T>(UnityEngine.MonoBehaviour monoBehaviour, bool isPermanent = false) where T : class
		{
			Type type = typeof(T);

            RegisterMonoBehaviourAsASingleton(type, monoBehaviour, isPermanent);
		}

		public void RegisterMonoBehaviourAsASingleton(Type type, UnityEngine.MonoBehaviour monoBehaviour, bool isPermanent = false)
        {
			if (!Singletons.ContainsKey(type) && (type.IsSubclassOf(typeof(UnityEngine.MonoBehaviour)) || type.IsInterface))
			{
				SingletonData singletonData = new SingletonData((type.IsSubclassOf(typeof(UnityEngine.MonoBehaviour))) ? type.AssemblyQualifiedName : monoBehaviour.GetType().AssemblyQualifiedName, (type.IsInterface) ? type.AssemblyQualifiedName : null, monoBehaviour.gameObject);

				singletonsData.Add(singletonData);

				Singletons.Add(type, monoBehaviour);

				if (isPermanent && (monoBehaviour.gameObject.transform.parent == null) ? true : monoBehaviour.gameObject.GetComponentInParent<SingletonsManager>() == null)
					monoBehaviour.gameObject.transform.SetParent(gameObject.transform);

				if (monoBehaviour is SingletonMonoBehaviour monoBehaviourSingleton)
					monoBehaviourSingleton.OnRegister();
			}
		}

        public T GetSingleton<T>(bool findIfNotExists = false) where T : class
        {
            Type type = typeof(T);

            if (type.IsAbstract)
            {
                foreach (KeyValuePair<Type, UnityEngine.MonoBehaviour> singleton in Singletons)
                {
                    if (singleton.Key.IsSubclassOf(type))
                        return singleton.Value as T;
                }
            }

            if (Singletons.TryGetValue(type, out UnityEngine.MonoBehaviour monoBehaviour))
                return monoBehaviour as T;
            else if(type.IsSubclassOf(typeof(MonoBehaviour)) && findIfNotExists)
            {
				monoBehaviour = (UnityEngine.MonoBehaviour)FindObjectOfType(type);

				if (monoBehaviour != null)
				{
					RegisterMonoBehaviourAsASingleton<T>(monoBehaviour);

					return monoBehaviour as T;
				}
			}

            return null;
        }

        private void OnSceneUnloaded(Scene scene)
        {
            List<Type> missedTypes = new List<Type>();

            foreach (KeyValuePair<Type, UnityEngine.MonoBehaviour> singleton in Singletons)
            {
                if (singleton.Value == null)
                    missedTypes.Add(singleton.Key);
            }

            foreach (Type missedType in missedTypes)
            {
                for (int i = 0, singletonsDataCount = singletonsData.Count; i < singletonsDataCount; i++)
                {
                    SingletonData singletonData = singletonsData[i];

                    if (singletonData.Type == missedType.AssemblyQualifiedName)
                    {
                        singletonsData.Remove(singletonData);

                        break;
                    }
                }

                Singletons.Remove(missedType);
            }
        }
    }
}
