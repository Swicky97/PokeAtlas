using System.Text.Json;
using System.Text.Json.Serialization;

namespace PokeAtlas.Models;

public class RectangleJsonConverter : JsonConverter<Rectangle>
{
    public override Rectangle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return ReadRectangle(ref reader);
    }

    public override void Write(Utf8JsonWriter writer, Rectangle value, JsonSerializerOptions options)
    {
        WriteRectangle(writer, value);
    }

    public static Rectangle ReadRectangle(ref Utf8JsonReader reader)
    {
        int x = 0, y = 0, width = 0, height = 0;

        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            string propertyName = reader.GetString()!;
            reader.Read();

            switch (propertyName.ToLowerInvariant())
            {
                case "x": x = reader.GetInt32(); break;
                case "y": y = reader.GetInt32(); break;
                case "width": width = reader.GetInt32(); break;
                case "height": height = reader.GetInt32(); break;
            }
        }

        return new Rectangle(x, y, width, height);
    }

    public static void WriteRectangle(Utf8JsonWriter writer, Rectangle value)
    {
        writer.WriteStartObject();
        writer.WriteNumber("x", value.X);
        writer.WriteNumber("y", value.Y);
        writer.WriteNumber("width", value.Width);
        writer.WriteNumber("height", value.Height);
        writer.WriteEndObject();
    }
}
