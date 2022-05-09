using System.Text.Json;

namespace GameUtilities.System.Serialization;

public class JsonObjectParser
{
    public static void Parse(ref Utf8JsonReader jsonReader, Action action)
    {
        var stack = new Stack<JsonTokenType>();
        stack.Push(JsonTokenType.StartObject);

        //todo need to handle finishing an entire component at EndObject
        //or just find a better way to parse an object
        while (jsonReader.Read() && stack.Count > 0)
        {
            switch (jsonReader.TokenType)
            {
                case JsonTokenType.StartObject:
                    {
                        stack.Push(JsonTokenType.StartObject);
                        break;
                    }
                case JsonTokenType.EndObject:
                    {
                        stack.Pop();
                        break;
                    }
                case JsonTokenType.PropertyName:
                    {
                        action();
                        break;
                    }
            }
        }
    }
}