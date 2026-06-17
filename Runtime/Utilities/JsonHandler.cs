using System;
using System.Collections.Generic;

namespace SernizaGamesCore
{
    public static class JsonHandler
    {
		public static List<string> ToList(string jsonArray)
		{
			List<string> list = new List<string>();

			if (jsonArray.Length < 3)
				return list;

			string value = "";

			int depth = 0;

			for (int i = 1, jsonLength = jsonArray.Length; i < jsonLength - 1; i++)
			{
				switch (jsonArray[i])
				{
					case '"':
						if (depth > 0)
							value += jsonArray[i];

						break;
					case ',':
						if (depth == 0)
						{
							list.Add(value);

							value = "";
						}

						else
							value += jsonArray[i];

						break;
					case '{':
						depth++;

						value += jsonArray[i];

						break;
					case '[':
						depth++;

						value += jsonArray[i];

						break;
					case ']':
						depth--;

						value += jsonArray[i];

						break;
					case '}':
						depth--;

						value += jsonArray[i];

						break;
					default:
						value += jsonArray[i];

						break;
				}
			}

			if (value != "")
				list.Add(value);

			return list;
		}

		public static Dictionary<string, string> ToDictionary(string json)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            json = json.Replace(Environment.NewLine, "");

            if (json.Length < 6)
                return dictionary;

            bool isKey = true;
            string key = "";

            bool isValue = false;
            string value = "";

            int depth = 0;

            for (int i = 2, jsonLength = json.Length; i < jsonLength - 1; i++)
            {
                switch (json[i])
                {
                    case '"':
                        if (isKey)
                        {
                            if (json[i + 1] == ':')
                            {
                                if (json[i + 2] == '"')
                                    i += 2;
                                else
                                    i++;

                                isKey = false;

                                isValue = true;
                            }
                        }
                        else
                        {
                            if (depth == 0)
                            {
                                if (json[i + 1] == ',')
                                {
                                    i += 2;

                                    isKey = true;

                                    isValue = false;

                                    dictionary.Add(key, value);

                                    key = "";

                                    value = "";
                                }
                            }
                            else
                                value += json[i];
                        }

                        break;
                    case ',':
                        if (isValue)
                        {
                            if (depth == 0)
                            {
                                if (json[i + 1] == '"')
                                {
                                    i++;

                                    isKey = true;

                                    isValue = false;

                                    dictionary.Add(key, value);

                                    key = "";

                                    value = "";
                                }
                                else
                                    value += json[i];
                            }
                            else
                                value += json[i];
                        }
                        else
                            value += json[i];

                        break;
                    case '{':
                        depth++;

                        value += json[i];

                        break;
                    case '[':
                        depth++;

                        value += json[i];

                        break;
                    case ']':
                        depth--;

                        value += json[i];

                        break;
                    case '}':
                        depth--;

                        value += json[i];

                        break;
                    default:
                        if (isKey)
                            key += json[i];
                        else
                            value += json[i];

                        break;
                }
            }
            if (key != "")
                dictionary.Add(key, value);

            return dictionary;
        }
    }
}
