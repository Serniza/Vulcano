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

        SerializedProperty singletonsData;
        ReorderableList singletonsDataList;

        #endregion

        void OnEnable()
        {
            singletonsData = serializedObject.FindProperty("singletonsData");

            singletonsDataList = new ReorderableList(serializedObject, singletonsData, true, false, true, true)
			{
				drawElementCallback = (Rect position, int i, bool isActive, bool isFocused) =>
				{
					float labelWidth = 54f;

					Vector2 fieldSize = new Vector2((position.width - labelWidth - 8f) / 2f, 18f);

					SerializedProperty singletonData = singletonsData.GetArrayElementAtIndex(i);

					SerializedProperty gameObject = singletonData.FindPropertyRelative("gameObject");

					EditorGUI.PropertyField(new Rect(position.x + labelWidth + fieldSize.x + 8f, position.y + 3f, fieldSize.x, fieldSize.y), gameObject, GUIContent.none, true);

					List<Type> types = new List<Type>
					{
						null
					};

					if (gameObject.objectReferenceValue != null)
					{
						List<UnityEngine.MonoBehaviour> monobehaviours = new List<UnityEngine.MonoBehaviour>(((GameObject)gameObject.objectReferenceValue).GetComponents<UnityEngine.MonoBehaviour>());

						for (int j = 0, monobehavioursCount = monobehaviours.Count; j < monobehavioursCount; j++)
						{
							UnityEngine.MonoBehaviour monobehaviour = monobehaviours[j];

							if (monobehaviour == null)
								continue;

							Type type = monobehaviour.GetType();

							types.Add(type);

							while (type.BaseType != typeof(UnityEngine.MonoBehaviour))
							{
								type = type.BaseType;

								types.Add(type);
							}
						}
					}

                    List<string> typesPaths = new List<string>()
                    {
                        SingletonData.defaultType
                    };

					if (gameObject.objectReferenceValue != null)
					{
						List<UnityEngine.MonoBehaviour> monoBehaviours = new List<UnityEngine.MonoBehaviour>(((GameObject)gameObject.objectReferenceValue).GetComponents<UnityEngine.MonoBehaviour>());

						for (int j = 0, monoBehavioursCount = monoBehaviours.Count; j < monoBehavioursCount; j++)
						{
							UnityEngine.MonoBehaviour monoBehaviour = monoBehaviours[j];

							if (monoBehaviour == null)
								continue;

							Type type = monoBehaviour.GetType();

							string typePath = type.FullName.Replace('.', '/');

							typesPaths.Add(typePath);

							while (type.BaseType != typeof(UnityEngine.MonoBehaviour))
							{
								type = type.BaseType;

								string[] splitType = typePath.Split(' ');

								if (splitType.Length == 1)
								{
									List<string> nameSpaces = new List<string>(splitType[0].Split('/'));

									if (nameSpaces.Count >= 2)
									{
										splitType[splitType.Length - 1] = nameSpaces[nameSpaces.Count - 1];

										nameSpaces.RemoveAt(nameSpaces.Count - 1);

										splitType[splitType.Length - 1] = $"{string.Join("/", nameSpaces.ToArray())}/({splitType[splitType.Length - 1]})";
									}
									else
										splitType[splitType.Length - 1] = $"({splitType[splitType.Length - 1]})";
								}
								else
									splitType[splitType.Length - 1] = $"({splitType[splitType.Length - 1]})";

								typePath = string.Join(" ", splitType);

								typePath = $"{typePath} {type.Name}";

								typesPaths.Add(typePath);
							}
						}
					}
					int typeIndex = 0;

					string currentType = singletonData.FindPropertyRelative("type").stringValue;

					if (currentType != SingletonData.defaultType)
					{
						for (int j = 1, typesCount = types.Count; j < typesCount; j++)
						{
							if (types[j].AssemblyQualifiedName == currentType)
							{
								typeIndex = j;

								break;
							}
						}
					}

					EditorGUI.LabelField(new Rect(position.x, position.y - 11f, labelWidth, position.height), "Type");

					if(typesPaths.Count == 1)
							GUI.enabled = false;

					typeIndex = EditorGUI.Popup(new Rect(position.x + labelWidth, position.y + 3f, fieldSize.x, fieldSize.y), typeIndex, typesPaths.ToArray());

                    currentType = singletonData.FindPropertyRelative("type").stringValue = (typeIndex == 0) ? SingletonData.defaultType : types[typeIndex].AssemblyQualifiedName;

					List<Type> interfacesTypes = new List<Type>
					{
						null
					};

                    if (currentType != SingletonData.defaultType)
						interfacesTypes.AddRange(Type.GetType(currentType).GetInterfaces());                 

					List<string> interfacesTypesPaths = new List<string>()
					{
						SingletonData.defaultType
					};

                    for (int j = 1, interfacesTypesCount = interfacesTypes.Count; j < interfacesTypesCount; j++)
                    {
						Type interfaceType = interfacesTypes[j];

						string interfaceTypePath = interfaceType.FullName.Replace('.', '/');

						interfacesTypesPaths.Add(interfaceTypePath);
					}

					int interfaceTypeIndex = 0;

					string currentInterfaceType = singletonData.FindPropertyRelative("interfaceType").stringValue;

					if (currentInterfaceType != SingletonData.defaultType)
					{
						for (int j = 1, interfacesTypesCount = interfacesTypes.Count; j < interfacesTypesCount; j++)
						{
							if (interfacesTypes[j].AssemblyQualifiedName == currentInterfaceType)
							{
								interfaceTypeIndex = j;

								break;
							}
						}
					}

					if (!GUI.enabled)
						GUI.enabled = true;

					EditorGUI.LabelField(new Rect(position.x, position.y + 11f, labelWidth, position.height), "Interface");

					if (interfacesTypesPaths.Count == 1)
						GUI.enabled = false;

					interfaceTypeIndex = EditorGUI.Popup(new Rect(position.x + labelWidth, position.y + 24f, fieldSize.x, fieldSize.y), interfaceTypeIndex, interfacesTypesPaths.ToArray());

					singletonData.FindPropertyRelative("interfaceType").stringValue = (interfaceTypeIndex == 0) ? SingletonData.defaultType : interfacesTypes[interfaceTypeIndex].AssemblyQualifiedName;

					if (!GUI.enabled)
                        GUI.enabled = true;
                },
				elementHeight = 43f
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
