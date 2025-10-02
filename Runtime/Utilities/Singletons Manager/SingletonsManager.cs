using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utilities
{
    public class SingletonsManager : MonoBehaviour
	{
        #region Variables & Properties

		private static SingletonsManager instance;
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

					DontDestroyOnLoad(instance.gameObject);

					instance.RegisterInitialSingletons();
				}

				return instance;
			}
		}

        [SerializeField] List<SingletonData> singletonsData;
        Dictionary<Type, object> Singletons { get; set; }

        #endregion

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
			if (singletonsData == null)
				singletonsData = new List<SingletonData>();

			Singletons = new Dictionary<Type, object>();

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
				if (singletonGameObject.scene.rootCount == 0)
				{
					GameObject singletonPrefab = singletonGameObject;

					singletonGameObject = singletonData.GameObject = Instantiate(singletonPrefab);

					singletonGameObject.name = singletonPrefab.name;

					singletonGameObject.transform.SetParent(gameObject.transform);

					singletonMonoBehaviour = singletonData.MonoBehaviour = (UnityEngine.MonoBehaviour)singletonGameObject.GetComponent(singletonMonoBehaviour.GetType());

					for (int j = i + 1; j < singletonsDataCount; j++)
					{
						SingletonData nextSingletonData = singletonsData[j];

						if (((nextSingletonData.Type == "None") ? null : Type.GetType(nextSingletonData.Type)) == null)
							continue;

						GameObject nextSingletonGameObject = nextSingletonData.GameObject;

						if (nextSingletonGameObject == null)
							continue;

						UnityEngine.MonoBehaviour nextSingletonMonoBehaviour = nextSingletonData.MonoBehaviour;

						if (nextSingletonGameObject == singletonPrefab)
						{
							nextSingletonData.GameObject = singletonGameObject;

							nextSingletonData.MonoBehaviour = (UnityEngine.MonoBehaviour)singletonGameObject.GetComponent(nextSingletonMonoBehaviour.GetType());
						}
					}
				}

				if (!Singletons.ContainsKey(singletonType))
					Singletons.Add(singletonType, singletonMonoBehaviour);
			}
        }

		public void RegisterMonoBehaviourAsASingleton(Type type, GameObject gameObject, UnityEngine.MonoBehaviour monoBehaviour, bool isPermanent = false)
        {
			if (!Singletons.ContainsKey(type))
			{
				singletonsData.Add(new SingletonData(type.AssemblyQualifiedName, gameObject, monoBehaviour));

				Singletons.Add(type, monoBehaviour);

				if (isPermanent && ((gameObject.transform.parent == null) ? true : gameObject.GetComponentInParent<SingletonsManager>() == null))
					gameObject.transform.SetParent(this.gameObject.transform);
			}
		}

        public T GetSingleton<T>() where T : class
        {
            if (Singletons.TryGetValue(typeof(T), out object singleton))
                return (T)singleton;
           
            return null;
        }

		private void OnSceneUnloaded(Scene scene)
        {
            List<Type> missedTypes = new List<Type>();

            foreach (KeyValuePair<Type, object> singleton in Singletons)
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
