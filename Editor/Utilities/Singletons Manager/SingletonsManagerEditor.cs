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
                    SerializedProperty singletonData = singletonsData.GetArrayElementAtIndex(i);

                    float labelWidth = 54f;

                    EditorGUI.LabelField(new Rect(position.x, position.y - 11f, labelWidth, position.height), "Type");

                    float fieldWidth = (position.width - labelWidth - 8f) / 2f;

                    SerializedProperty gameObject = singletonData.FindPropertyRelative("gameObject");

                    EditorGUI.PropertyField(new Rect(position.x + labelWidth + fieldWidth + 8f, position.y + 3f, fieldWidth, position.height), gameObject, GUIContent.none, true);

                    List<Type> types = new List<Type>
                    {
                        null
                    };

                    if (gameObject.objectReferenceValue == null)
                        GUI.enabled = false;
                    else
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

                    int typeIndex = 0;

                    string currentType = singletonData.FindPropertyRelative("type").stringValue;

                    if (currentType != SingletonData.DEFAULT_SINGLETON_TYPE)
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

                    List<string> typesPaths = new List<string>()
                    {
                        SingletonData.DEFAULT_SINGLETON_TYPE
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

                            string typePath = type.FullName.Replace('.','/');

                            typesPaths.Add(typePath);

                            while (type.BaseType != typeof(UnityEngine.MonoBehaviour))
                            {
                                type = type.BaseType;

                                string[] lastTypePath = typePath.Split(' ');

                                if(lastTypePath.Length == 1 ) 
                                {
                                    List<string> nameSpaces = new List<string>(lastTypePath[0].Split('/'));

                                    if (nameSpaces.Count >= 2)
                                    {
                                        lastTypePath[lastTypePath.Length - 1] = nameSpaces[nameSpaces.Count - 1];

                                        nameSpaces.RemoveAt(nameSpaces.Count - 1);

                                        lastTypePath[lastTypePath.Length - 1] = $"{string.Join("/", nameSpaces.ToArray())}/({lastTypePath[lastTypePath.Length - 1]})";
                                    }
                                    else
                                        lastTypePath[lastTypePath.Length - 1] = $"({lastTypePath[lastTypePath.Length - 1]})";
                                }
                                else
                                    lastTypePath[lastTypePath.Length - 1] = $"({lastTypePath[lastTypePath.Length - 1]})";

                                typePath = string.Join(" ", lastTypePath);

                                typePath = $"{typePath} {type.Name}";

                                typesPaths.Add(typePath);
                            }
                        }
                    }

                    typeIndex = EditorGUI.Popup(new Rect(position.x + labelWidth, position.y + 3f, fieldWidth, position.height), typeIndex, typesPaths.ToArray());

                    currentType = singletonData.FindPropertyRelative("type").stringValue = (typeIndex == 0) ? "None" : types[typeIndex].AssemblyQualifiedName;

					labelWidth = 54f;

					EditorGUI.LabelField(new Rect(position.x, position.y + 11f, labelWidth, position.height), "Interface");

					List<Type> interfacesTypes = new List<Type>
					{
						null
					};

                    if (currentType == SingletonData.DEFAULT_SINGLETON_TYPE)
                        GUI.enabled = false;
                    else
                        interfacesTypes.AddRange(Type.GetType(currentType).GetInterfaces());

					int interfaceTypeIndex = 0;

					string currentInterfaceType = singletonData.FindPropertyRelative("interfaceType").stringValue;

					if (currentInterfaceType != SingletonData.DEFAULT_SINGLETON_TYPE)
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

					List<string> interfacesPaths = new List<string>()
					{
						SingletonData.DEFAULT_SINGLETON_TYPE
					};

                    for (int j = 1, interfacesTypesCount = interfacesTypes.Count; j < interfacesTypesCount; j++)
                    {
						Type interfaceType = interfacesTypes[j];

						string interfaceTypePath = interfaceType.FullName.Replace('.', '/');

						interfacesPaths.Add(interfaceTypePath);
					}

					interfaceTypeIndex = EditorGUI.Popup(new Rect(position.x + labelWidth, position.y + 24f, fieldWidth, position.height), typeIndex, interfacesPaths.ToArray());

					singletonData.FindPropertyRelative("interfaceType").stringValue = (interfaceTypeIndex == 0) ? "None" : interfacesTypes[interfaceTypeIndex].AssemblyQualifiedName;

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
