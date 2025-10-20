using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Attributes
{
    [CanEditMultipleObjects]

    [CustomEditor(typeof(UnityEngine.Object), true, isFallback = true)]
    public class ObjectEditor : Editor
    {
		#region Variable & Properties

		List<object> properties;

        Vector2 standardSpacing = new Vector2(15f, 16f);

        #endregion

        void OnEnable()
		{
			if (serializedObject.FindProperty("properties") == null)
				return;

			this.properties = new List<object>();

			DeserializeProperties();

			List<object> properties = new List<object>();

			SerializedProperty iterator = serializedObject.GetIterator();

			iterator.NextVisible(true);

			AddProperty(properties, iterator);

			while (iterator.NextVisible(false))
				AddProperty(properties, iterator);

			this.properties = properties;

			SerializeProperties();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			if (serializedObject.FindProperty("properties") == null)
				DrawDefaultInspector();
			else
			{
				for (int i = 0, propertiesCount = properties.Count; i < propertiesCount; i++)
				{
					if (properties[i] is string)
					{
						SerializedProperty property = serializedObject.FindProperty((string)properties[i]);

						if ((string)properties[i] == "m_Script")
							GUI.enabled = false;

                        EditorGUILayout.PropertyField(property, true);

                        if (!GUI.enabled)
							GUI.enabled = true;
					}
					else
					{
						EditorGUILayout.Space();

						GUI.skin.window.padding.top = GUI.skin.window.padding.top - (int)standardSpacing.y;

						ShowFolder((Folder)properties[i], true);
					}
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		void AddProperty(List<object> properties, SerializedProperty property)
		{
			Foldout foldout = GetPropertyAttribute<Foldout>(property);

			if (foldout == null)
				properties.Add(property.propertyPath);
			else
			{
				List<string> splitPath = new List<string>(foldout.path.Split('/'));

				for (int i = 0, splitPathCount = splitPath.Count; i < splitPathCount; i++)
				{
					if (string.IsNullOrEmpty(splitPath[i]))
					{
						splitPath.RemoveAt(i);

						i--;
					}
				}

				Folder folder = GetFolder(properties, splitPath);

				if (folder == null)
					CreateFolder(properties, splitPath, property);
				else
					folder.properties.Add(property.propertyPath);
			}
		}

        T GetPropertyAttribute<T>(SerializedProperty property) where T : Attribute
        {
            T attribute = null;

            List<object> attributes = GetPropertyAttributes<PropertyAttribute>(property);

            if (attributes != null)
            {
                for (int i = 0, attributesCount = attributes.Count; i < attributesCount; i++)
                {
                    attribute = attributes[i] as T;

                    if (attribute != null)
                        break;
                }
            }

            return attribute;
        }

        List<object> GetPropertyAttributes<T>(SerializedProperty property) where T : Attribute
		{
			if (property.serializedObject.targetObject == null)
				return null;

			Type targetType = property.serializedObject.targetObject.GetType();

			BindingFlags bindingFlags = BindingFlags.GetField
									  | BindingFlags.Instance
									  | BindingFlags.NonPublic
									  | BindingFlags.Public;

			FieldInfo field = targetType.GetField(property.name, bindingFlags);

			while (field == null && targetType.BaseType != typeof(MonoBehaviour))
			{
				targetType = targetType.BaseType;

				field = targetType.GetField(property.name, bindingFlags);
			}

			if (field != null)
				return field.GetCustomAttributes(typeof(T), true).ToList();

			return null;
		}

		void SerializeProperties()
		{
			serializedObject.Update();

			string properties = "[";

			for (int i = 0, propertiesCount = this.properties.Count; i < propertiesCount; i++)
			{
				if (this.properties[i] is Folder)
				{
					properties += ((Folder)this.properties[i]).Serialize();

					if (i < this.properties.Count - 1)
						properties += ",";
				}
			}

			if (properties[properties.Length - 1] == ',')
				properties = properties.Substring(0, properties.Length - 1);

			properties += "]";

			serializedObject.FindProperty("properties").stringValue = properties;

			serializedObject.ApplyModifiedProperties();
		}

		void DeserializeProperties()
		{
			string properties = serializedObject.FindProperty("properties").stringValue;

			properties = properties.Substring(1, properties.Length - 2);

			string property = "";

			int depth = 0;

			for (int i = 0, propertiesLength = properties.Length; i < propertiesLength; i++)
			{
				switch (properties[i])
				{
					case '{':

						depth++;

						property += properties[i];

						break;
					case ',':

						if (depth == 0)
						{
							this.properties.Add(property);

							property = "";
						}
						else
							property += properties[i];

						break;
					case '}':

						depth--;

						property += properties[i];

						break;
					default:

						property += properties[i];

						break;
				}
			}

			if (property != "")
				this.properties.Add(property);

			for (int i = 0, propertiesCount = this.properties.Count; i < propertiesCount; i++)
				this.properties[i] = new Folder().Deserialize((string)this.properties[i]);
		}

		Folder GetFolder(List<object> properties, List<string> splitPath)
		{
			Folder folder = null;

			for (int i = 0, splitPathCount = splitPath.Count; i < splitPathCount; i++)
			{
				folder = GetFolder((folder == null) ? properties : folder.properties, splitPath[i]);

				if (folder == null)
					break;
			}

			return folder;
		}

		Folder GetFolder(List<object> properties, string name)
		{
			for (int i = 0, propertiesCount = properties.Count; i < propertiesCount; i++)
			{
				if (properties[i] is Folder folder)
				{
					if (folder.name == name)
						return (Folder)properties[i];
				}
			}

			return null;
		}

		void CreateFolder(List<object> properties, List<string> splitPath, SerializedProperty property)
		{
			Folder folder = null;

			for (int i = 0, splitPathCount = splitPath.Count; i < splitPathCount; i++)
			{
				Folder nextFolder = GetFolder((folder == null) ? properties : folder.properties, splitPath[i]);

				if (nextFolder == null)
				{
					if (i < splitPath.Count - 1)
						nextFolder = new Folder(splitPath[i]);
					else
						nextFolder = new Folder(splitPath[i], property.propertyPath);

					Folder olderFolder = GetFolder(this.properties, splitPath.Take(i + 1).ToList());

					if (olderFolder != null)
						nextFolder.isExpanded = olderFolder.isExpanded;

					if (folder == null)
						properties.Add(nextFolder);
					else
						folder.properties.Add(nextFolder);

					if (i == splitPath.Count - 1)
						return;
				}

				folder = nextFolder;
			}
		}

		void ShowFolder(Folder folder, bool isRoot)
		{
            EditorGUILayout.BeginHorizontal();

			if (!isRoot)
				GUILayout.Space(standardSpacing.x);

			EditorGUILayout.BeginVertical((GUIStyle)"HelpBox");

            bool isExpanded = GUILayout.Toggle(folder.isExpanded, folder.name, "foldoutHeader");

			if (isExpanded != folder.isExpanded)
			{
				folder.isExpanded = isExpanded;

				SerializeProperties();
			}

			if (folder.isExpanded)
			{
				for (int i = 0, folderPropertiesCount = folder.properties.Count; i < folderPropertiesCount; i++)
				{
					if (folder.properties[i] is string)
					{
                        EditorGUILayout.BeginHorizontal();

						GUILayout.Space(standardSpacing.x);

						string propertyPath = (string)folder.properties[i];

                        SerializedProperty property = serializedObject.FindProperty(propertyPath);

						SerializedPropertyType serializedPropertyType = serializedObject.FindProperty(propertyPath).propertyType;

                        if (serializedPropertyType == SerializedPropertyType.Generic)
							GUILayout.Space(standardSpacing.x);

						ReadOnly readOnly = GetPropertyAttribute<ReadOnly>(property);

						if (readOnly != null)
							GUI.enabled = false;

						EditorGUILayout.PropertyField(property, true);

						if (readOnly != null)
							GUI.enabled = true;

						EditorGUILayout.EndHorizontal();
					}
					else
						ShowFolder((Folder)folder.properties[i], false);
				}
			}

			EditorGUILayout.EndVertical();

			EditorGUILayout.EndHorizontal();
		}
	}
}
