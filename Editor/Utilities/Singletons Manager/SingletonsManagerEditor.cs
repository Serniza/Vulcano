using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Utilities
{
    [CustomEditor(typeof(SingletonsManager))]
    public class SingletonsManagerData : Editor
    {
        #region Variables & Properties

        SerializedProperty singletonsDataProperty;
        ReorderableList singletonsDataList;

        #endregion

        void OnEnable()
        {
            singletonsDataProperty = serializedObject.FindProperty("singletonsData");

            singletonsDataList = new ReorderableList(serializedObject, singletonsDataProperty, true, false, true, true)
			{
				drawElementCallback = (Rect position, int i, bool isActive, bool isFocused) =>
				{
					float labelWidth = 54f;

					Vector2 fieldSize = new Vector2((position.width - labelWidth - 8f) / 2f, 18f);

					SerializedProperty singletonData = singletonsDataProperty.GetArrayElementAtIndex(i);

					SerializedProperty gameObjectProperty = singletonData.FindPropertyRelative("gameObject");

					EditorGUI.PropertyField(new Rect(position.x + labelWidth + fieldSize.x + 8f, position.y + 3f, fieldSize.x, fieldSize.y), gameObjectProperty, GUIContent.none, true);

					List<KeyValuePair<Type, UnityEngine.MonoBehaviour>> singletons = new List<KeyValuePair<Type, UnityEngine.MonoBehaviour>>
					{
						new KeyValuePair<Type, UnityEngine.MonoBehaviour>(null, null)
					};

					if (gameObjectProperty.objectReferenceValue != null)
					{
						List<UnityEngine.MonoBehaviour> monoBehaviours = new List<UnityEngine.MonoBehaviour>(((GameObject)gameObjectProperty.objectReferenceValue).GetComponents<UnityEngine.MonoBehaviour>());

						for (int j = 0, monoBehavioursCount = monoBehaviours.Count; j < monoBehavioursCount; j++)
						{
							UnityEngine.MonoBehaviour monoBehaviour = monoBehaviours[j];

							if (monoBehaviour == null)
								continue;

							Type type = monoBehaviour.GetType();

							singletons.Add(new KeyValuePair<Type, UnityEngine.MonoBehaviour>(type, monoBehaviour));

							Type[] interfacesTypes = type.GetInterfaces();

							for (int k = 0, interfacesTypesLength = interfacesTypes.Length; k < interfacesTypesLength; k++)
								singletons.Add(new KeyValuePair<Type, UnityEngine.MonoBehaviour>(interfacesTypes[k], monoBehaviour));

							while (type.BaseType != null && 
								   type.BaseType != typeof(MonoBehaviour) && 
								   type.BaseType != typeof(UnityEngine.MonoBehaviour))
							{
								type = type.BaseType;

								if (type.IsGenericType)
									continue;

								singletons.Add(new KeyValuePair<Type, UnityEngine.MonoBehaviour>(type, monoBehaviour));

								interfacesTypes = type.GetInterfaces();

								for (int k = 0, interfacesTypesLength = interfacesTypes.Length; k < interfacesTypesLength; k++)
								{
									Type interfaceType = interfacesTypes[k];

									if (singletons.Any(singleton => singleton.Key == interfaceType))
										continue;
	
									singletons.Add(new KeyValuePair<Type, UnityEngine.MonoBehaviour>(interfaceType, monoBehaviour));
								}
							}
						}
					}

					int typeIndex = 0;

					SerializedProperty typeProperty = singletonData.FindPropertyRelative("type");

					SerializedProperty monoBehaviourProperty = singletonData.FindPropertyRelative("monoBehaviour");

					if (typeProperty.stringValue != "None")
					{
						for (int j = 1, typeMonoBehaviourPairsCount = singletons.Count; j < typeMonoBehaviourPairsCount; j++)
						{
							KeyValuePair<Type, UnityEngine.MonoBehaviour> KeyValuePair = singletons[j];

							if (KeyValuePair.Key.AssemblyQualifiedName == typeProperty.stringValue && KeyValuePair.Value == (UnityEngine.MonoBehaviour)monoBehaviourProperty.objectReferenceValue)
							{
								typeIndex = j;

								break;
							}
						}
					}

					EditorGUI.LabelField(new Rect(position.x, position.y, labelWidth, position.height), "Type");

					typeIndex = EditorGUI.Popup(new Rect(position.x + labelWidth, position.y + 3f, fieldSize.x, fieldSize.y), typeIndex, Array.ConvertAll(singletons.ToArray(), keyValuePair =>
					{
						if (keyValuePair.Key == null && keyValuePair.Value == null)
							return "None";

						Type monoBehaviourType = keyValuePair.Value.GetType();

						if (keyValuePair.Key == monoBehaviourType)
							return keyValuePair.Key.FullName.Replace('.', '/');
						else
						{
							string type = monoBehaviourType.FullName.Replace('.', '/');

							string[] splitType = type.Split("/");

							splitType[splitType.Length - 1] = $"({keyValuePair.Key.Name}) {splitType[splitType.Length - 1]}";

							type = string.Join("/", splitType);

							return type;
						}

					}));

					typeProperty.stringValue = (typeIndex == 0) ? "None": singletons[typeIndex].Key.AssemblyQualifiedName;

					monoBehaviourProperty.objectReferenceValue = (typeIndex == 0) ? null: singletons[typeIndex].Value;
				},
			};
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUI.enabled = false;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));

            GUI.enabled = true;

            EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing * 3f);

            SerializedProperty singletonsData = serializedObject.FindProperty("singletonsData");

            singletonsData.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(singletonsData.isExpanded, "Singletons");

            EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);

            if (singletonsData.isExpanded)
                singletonsDataList.DoLayoutList();

            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
