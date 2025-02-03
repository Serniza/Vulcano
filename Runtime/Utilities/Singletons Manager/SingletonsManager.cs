using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utilities
{
    public class SingletonsManager : UnityEngine.MonoBehaviour
    {
        #region Variables & Properties

        static SingletonsManager instance;
        public static SingletonsManager _instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<SingletonsManager>();

                    if (instance == null)
                    {
                        SingletonsManager prefab = Resources.Load<SingletonsManager>("Singletons Manager");

                        if (prefab != null)
                            instance = Instantiate(prefab);
                        else
                            instance = new GameObject("Singletons Manager", typeof(SingletonsManager)).GetComponent<SingletonsManager>();
                    }
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
        Dictionary<Type, UnityEngine.MonoBehaviour> _singletons { get; set; }

        public bool ContainsKey(Type type)
        {
            return _singletons.ContainsKey(type);
        }

        #endregion

        void Awake()
        {
            _instance = this;
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
            _singletons = new Dictionary<Type, UnityEngine.MonoBehaviour>();

            if (singletonsData == null)
                singletonsData = new List<SingletonData>();

            for (int i = 0, singletonsDataCount = singletonsData.Count; i < singletonsDataCount; i++)
            {
                SingletonData singletonData = singletonsData[i];

                if (singletonData._type != SingletonData.DEFAULT_SINGLETON_TYPE)
                {
                    GameObject singletonGameObject = singletonData._gameObject;

                    if (singletonGameObject != null)
                    {
                        if (singletonGameObject.scene != gameObject.scene)
                        {
                            GameObject singletonPrefab = singletonGameObject;

                            singletonGameObject = singletonData._gameObject = Instantiate(singletonPrefab);

                            singletonGameObject.transform.SetParent(gameObject.transform);

                            for (int j = i + 1; j < singletonsDataCount; j++)
                            {
                                SingletonData nextSingletonData = singletonsData[j];

                                if (nextSingletonData._type != SingletonData.DEFAULT_SINGLETON_TYPE)
                                {
                                    GameObject nextSingletonGameObject = nextSingletonData._gameObject;

                                    if (nextSingletonGameObject != null
                                        &&
                                        nextSingletonGameObject == singletonPrefab)
                                        nextSingletonData._gameObject = singletonGameObject;
                                }
                            }
                        }

                        Type type = Type.GetType(singletonData._type);

                        Type interfaceType = (singletonData._interfaceType == null) ? null : Type.GetType(singletonData._interfaceType);

                        if ((interfaceType == null) ? !_singletons.ContainsKey(type) : !_singletons.ContainsKey(interfaceType))
                        {
                            Component component = singletonGameObject.GetComponent(type);

                            if (component != null)
                            {
                                UnityEngine.MonoBehaviour monoBehaviour = (UnityEngine.MonoBehaviour)component;

                                _singletons.Add((interfaceType == null) ? type : interfaceType, monoBehaviour);

                                if (monoBehaviour is MonoBehaviourSingleton monoBehaviourSingleton)
                                    monoBehaviourSingleton.OnRegister();
                            }
                            else
                            {
                                #if UNITY_EDITOR
                                Debug.LogWarning($"SingletonsManager/RegisterMonoBehaviours/Singleton:{singletonData._type}, The game object not has this mono behaviour type");
                                #endif
                            }
                        }
                    }
                    else
                    {
                        #if UNITY_EDITOR
                        Debug.LogWarning($"SingletonsManager/RegisterMonoBehaviours/Singleton:{singletonData._type}, Game object = null");
                        #endif
                    }
                }
            }
        }

		public void RegisterAsSingleton(UnityEngine.MonoBehaviour monoBehaviour, Type type, bool isPermanent = false)
		{
			if(!_singletons.ContainsKey(type))
			{
				SingletonData singletonData = new SingletonData((type.IsInterface) ? monoBehaviour.GetType().AssemblyQualifiedName : type.AssemblyQualifiedName, (type.IsInterface) ? type.AssemblyQualifiedName : null, monoBehaviour.gameObject);

				singletonsData.Add(singletonData);

				_singletons.Add(type, monoBehaviour);

				if (isPermanent)
					monoBehaviour.gameObject.transform.SetParent(gameObject.transform);

				if (monoBehaviour is MonoBehaviourSingleton monoBehaviourSingleton)
					monoBehaviourSingleton.OnRegister();
			}
		}

        public void RegisterAsSingleton<T>(UnityEngine.MonoBehaviour monoBehaviour, bool isPermanent = false) where T : class
		{
            Type type = typeof(T);

            RegisterAsSingleton(monoBehaviour, type, isPermanent);
        }

        public T GetSingleton<T>() where T : class
        {
            Type type = typeof(T);

            if (type.IsAbstract)
            {
                foreach (KeyValuePair<Type, UnityEngine.MonoBehaviour> singleton in _singletons)
                {
                    if (singleton.Key.IsSubclassOf(type))
                        return singleton.Value as T;
                }
            }

            if (_singletons.TryGetValue(type, out UnityEngine.MonoBehaviour monoBehaviour))
                return monoBehaviour as T;

            return null;
        }

        private void OnSceneUnloaded(Scene scene)
        {
            List<Type> missedTypes = new List<Type>();

            foreach (KeyValuePair<Type, UnityEngine.MonoBehaviour> singleton in _singletons)
            {
                if (singleton.Value == null)
                    missedTypes.Add(singleton.Key);
            }

            foreach (Type missedType in missedTypes)
            {
                for (int i = 0, singletonsDataCount = singletonsData.Count; i < singletonsDataCount; i++)
                {
                    SingletonData singletonData = singletonsData[i];

                    if (singletonData._type == missedType.AssemblyQualifiedName)
                    {
                        singletonsData.Remove(singletonData);

                        break;
                    }
                }

                _singletons.Remove(missedType);
            }
        }
    }
}
