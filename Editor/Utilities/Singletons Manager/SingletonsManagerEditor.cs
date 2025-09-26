using System;
using System.Collections.Generic;
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

					List<KeyValuePair<Type, MonoBehaviour>> typeMonoBehaviourPairs = new List<KeyValuePair<Type, MonoBehaviour>>
					{
						new KeyValuePair<Type, MonoBehaviour>(null, null)
					};

					if (gameObjectProperty.objectReferenceValue != null)
					{
						List<MonoBehaviour> monoBehaviours = new List<MonoBehaviour>(((GameObject)gameObjectProperty.objectReferenceValue).GetComponents<MonoBehaviour>());

						for (int j = 0, monoBehavioursCount = monoBehaviours.Count; j < monoBehavioursCount; j++)
						{
							MonoBehaviour monoBehaviour = monoBehaviours[j];

							if (monoBehaviour == null)
								continue;

							Type type = monoBehaviour.GetType();

							typeMonoBehaviourPairs.Add(new KeyValuePair<Type, MonoBehaviour>(type, monoBehaviour));

							Type[] interfacesTypes = type.GetInterfaces();

							for (int k = 0, interfacesTypesLength = interfacesTypes.Length; k < interfacesTypes.Length; k++)
							{
								Type interfaceType = interfacesTypes[k];

								typeMonoBehaviourPairs.Add(new KeyValuePair<Type, MonoBehaviour>(interfaceType, monoBehaviour));
							}

							while (type.BaseType != typeof(MonoBehaviour))
							{
								type = type.BaseType;

								typeMonoBehaviourPairs.Add(new KeyValuePair<Type, MonoBehaviour>(type, monoBehaviour));
							}
						}
					}

					int typeIndex = 0;

					SerializedProperty typeProperty = singletonData.FindPropertyRelative("type");

					SerializedProperty monoBehaviourProperty = singletonData.FindPropertyRelative("monoBehaviour");

					if (typeProperty.stringValue != "None")
					{
						for (int j = 1, typeMonoBehaviourPairsCount = typeMonoBehaviourPairs.Count; j < typeMonoBehaviourPairsCount; j++)
						{
							KeyValuePair<Type, MonoBehaviour> typeMonoBehaviourPair = typeMonoBehaviourPairs[j];

							if (typeMonoBehaviourPair.Key.AssemblyQualifiedName == typeProperty.stringValue && typeMonoBehaviourPair.Value == (MonoBehaviour)monoBehaviourProperty.objectReferenceValue)
							{
								typeIndex = j;

								break;
							}
						}
					}

					EditorGUI.LabelField(new Rect(position.x, position.y, labelWidth, position.height), "Type");

					typeIndex = EditorGUI.Popup(new Rect(position.x + labelWidth, position.y + 3f, fieldSize.x, fieldSize.y), typeIndex, Array.ConvertAll(typeMonoBehaviourPairs.ToArray(), typeMonoBehaviourPair =>
					{
						if (typeMonoBehaviourPair.Key == null && typeMonoBehaviourPair.Value == null)
							return "None";

						Type monoBehaviourType = typeMonoBehaviourPair.Value.GetType();

						if (typeMonoBehaviourPair.Key == monoBehaviourType)
							return typeMonoBehaviourPair.Key.FullName.Replace('.', '/');
						else
						{
							string type = monoBehaviourType.FullName.Replace('.', '/');

							string[] splitType = type.Split("/");

							splitType[splitType.Length - 1] = $"({typeMonoBehaviourPair.Key.Name}) {splitType[splitType.Length - 1]}";

							type = string.Join("/", splitType);

							return type;
						}

					}));

					typeProperty.stringValue = (typeIndex == 0) ? "None": typeMonoBehaviourPairs[typeIndex].Key.AssemblyQualifiedName;

					monoBehaviourProperty.objectReferenceValue = (typeIndex == 0) ? null: typeMonoBehaviourPairs[typeIndex].Value;
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
