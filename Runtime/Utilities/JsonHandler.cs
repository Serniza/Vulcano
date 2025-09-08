using System;
using System.Collections.Generic;

public static class JsonHandler
{
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

    public static List<string> ToList(string json)
    {
        List<string> list = new List<string>();

        if (json.Length < 3)
            return list;

        string value = "";

        int depth = 0;

        for (int i = 1, jsonLength = json.Length; i < jsonLength - 1; i++)
        {
            switch (json[i])
            {
                case '"':
                    if (depth > 0)
                        value += json[i];

                    break;
                case ',':
                    if (depth == 0)
                    {
                        list.Add(value);

                        value = "";
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
                    value += json[i];

                    break;
            }
        }

        if (value != "")
            list.Add(value);

        return list;
    }
}
