using System;
using System.Collections.Generic;

namespace Attributes
{
    public class Folder
    {
        #region Variables & Properties

        public string name;
        public List<object> properties;
        public bool isExpanded = true;

        #endregion

        public Folder()
        {
            properties = new List<object>();
        }

        public Folder(string name)
        {
            this.name = name;

            properties = new List<object>();
        }

        public Folder(string name, string property)
        {
            this.name = name;

            properties = new List<object>()
            {
                property
            };
        }

        public string Serialize()
        {
            string folder = "{";

            string parsedName = "";

            for (int i = 0, nameLength = name.Length; i < nameLength; i++)
            {
				parsedName += (int)name[i];

                if (i < name.Length - 1)
                    parsedName += ",";
            }

            folder += "\"" + parsedName + "\"";

            folder += ",";

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

            folder += properties;

            folder += ",";

            folder += isExpanded.ToString();

            folder += "}";

            return folder;
        }

        public Folder Deserialize(string folder)
        {
            folder = folder.Substring(2, folder.Length - 3);

			for (int i = 0, folderLength = folder.Length; i < folderLength; i++)
            {
                if (folder[i] == '"')
                {
                    string[] parsedName = folder.Substring(0, i).Split(',');

                    for (int j = 0, parsedNameLength = parsedName.Length; j < parsedNameLength; j++)
                    {
                        if (int.TryParse(parsedName[j], out int parsedLetter))
                            name += Convert.ToChar(parsedLetter);
                    }

                    folder = folder.Substring(i + 3);

                    break;
                }
            }

            for (int i = folder.Length - 1; i > 0; i--)
            {
                if (folder[i] == ',')
                {
                    isExpanded = (folder.Substring(i + 1) == "True");

                    folder = folder.Substring(0, i - 1);

                    break;
                }
            }

            string olderFolder = folder;

            folder = "";

            int depth = 0;

            for (int i = 0, olderFolderLength = olderFolder.Length; i < olderFolderLength; i++)
            {
                switch (olderFolder[i])
                {
                    case '{':

                        depth++;

                        folder += olderFolder[i];

                        break;
                    case ',':

                        if (depth == 0)
                        {
                            properties.Add(folder);

                            folder = "";
                        }
                        else
                            folder += olderFolder[i];

                        break;
                    case '}':

                        depth--;

                        folder += olderFolder[i];

                        break;
					case ']':

						if (depth == 0)
							goto end;
						else
							folder += olderFolder[i];

						break;
					default:

                        folder += olderFolder[i];

                        break;
                }
            }

            end:

            if (folder != "")
                properties.Add(folder);

            for (int i = 0, propertiesCount = properties.Count; i < propertiesCount; i++)
                properties[i] = new Folder().Deserialize((string)properties[i]);

            return this;
        }
    }
}
