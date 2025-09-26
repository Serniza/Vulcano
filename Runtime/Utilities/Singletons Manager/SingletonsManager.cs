using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utilities
{
    public class SingletonsManager : UnityEngine.MonoBehaviour
	{
        #region Variables & Properties

        public static SingletonsManager Instance { get; private set; }

        [SerializeField] List<SingletonData> singletonsData;
        Dictionary<Type, UnityEngine.MonoBehaviour> Singletons { get; set; }

        #endregion

        void Awake()
        {
			if (Instance == null)
			{
				Instance = this;

				if (gameObject.transform.parent != null)
					gameObject.transform.SetParent(null, true);

				DontDestroyOnLoad(gameObject);

				if (singletonsData == null)
					singletonsData = new List<SingletonData>();

				Singletons = new Dictionary<Type, UnityEngine.MonoBehaviour>();

				RegisterInitialSingletons();
			}
			else 
				Destroy(gameObject);
		}

        void OnEnable()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        void OnDisable()
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        public void RegisterInitialSingletons()
        {
            for (int i = 0, singletonsDataCount = singletonsData.Count; i < singletonsDataCount; i++)
            {
                SingletonData singletonData = singletonsData[i];

				Type singletonType = (singletonData.Type == "None") ? null : Type.GetType(singletonData.Type);

				if (singletonType == null)
				{
					#if UNITY_EDITOR
					Debug.LogWarning($"SingletonsManager/RegisterInitialSingletons/Singleton {i}: Type is None");
					#endif

					continue;
				}

				GameObject singletonGameObject = singletonData.GameObject;

				if (singletonGameObject == null)
				{
					#if UNITY_EDITOR
					Debug.LogWarning($"SingletonsManager/RegisterInitialSingletons/Singleton {singletonData.Type}: Game object is null");
					#endif

					continue;
				}

				UnityEngine.MonoBehaviour singletonMonoBehaviour = singletonData.MonoBehaviour;

				// The singleton game object is a prefab
				if (!singletonGameObject.scene.IsValid())
				{
					GameObject singletonPrefab = singletonGameObject;

					singletonGameObject = singletonData.GameObject = Instantiate(singletonPrefab);

					singletonGameObject.transform.SetParent(gameObject.transform);

					singletonMonoBehaviour = singletonData.MonoBehaviour = (UnityEngine.MonoBehaviour)singletonGameObject.GetComponent(singletonMonoBehaviour.GetType());

					for (int j = i + 1; j < singletonsDataCount; j++)
					{
						SingletonData nextSingletonData = singletonsData[j];

						Type nextSingletonType = (nextSingletonData.Type == "None") ? null : Type.GetType(nextSingletonData.Type);

						if (nextSingletonType == null)
							continue;

						GameObject nextSingletonGameObject = nextSingletonData.GameObject;

						if (nextSingletonGameObject == null)
							continue;

						UnityEngine.Object nextSingletonMonoBehaviour = nextSingletonData.MonoBehaviour;

						if (nextSingletonGameObject == singletonPrefab)
						{
							nextSingletonData.GameObject = singletonGameObject;

							nextSingletonData.MonoBehaviour = (UnityEngine.MonoBehaviour)singletonGameObject.GetComponent(nextSingletonMonoBehaviour.GetType());
						}
					}
				}

				if (!Singletons.ContainsKey(singletonType))
				{
					Singletons.Add(singletonType, singletonMonoBehaviour);

					if (singletonGameObject.transform.parent != transform)
						singletonGameObject.transform.SetParent(transform);

					if (singletonMonoBehaviour is SingletonMonoBehaviour)
						((SingletonMonoBehaviour)singletonMonoBehaviour).OnRegister();
				}
			}
        }

		public void RegisterMonoBehaviourAsASingleton(Type type, GameObject gameObject, UnityEngine.MonoBehaviour monoBehaviour, bool isPermanent = false)
        {
			if (!Singletons.ContainsKey(type))
			{
				SingletonData singletonData = new SingletonData(type.AssemblyQualifiedName, gameObject, monoBehaviour);

				singletonsData.Add(singletonData);

				Singletons.Add(type, monoBehaviour);

				if (isPermanent && (gameObject.transform.parent == null) ? true : gameObject.GetComponentInParent<SingletonsManager>(true) == null)
					gameObject.transform.SetParent(gameObject.transform);

				if (monoBehaviour is SingletonMonoBehaviour)
					((SingletonMonoBehaviour)monoBehaviour).OnRegister();
			}
		}

        public T GetSingleton<T>() where T : class
        {
            Type type = typeof(T);

            if (Singletons.TryGetValue(type, out UnityEngine.MonoBehaviour monoBehaviour))
                return monoBehaviour as T;
           
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
