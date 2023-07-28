﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ssbot.Models.Cqhttp;

public abstract class JsonCreationConverter<T>:JsonConverter
{
    protected abstract T Create(Type objectType, JObject jsonObject);
    public override bool CanConvert(Type objectType)
    {
        return typeof(T).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var jsonObject = JObject.Load(reader);
        var target = Create(objectType, jsonObject);
        serializer.Populate(jsonObject.CreateReader(), target);
        return target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}

public class RequestJsonConverter : JsonCreationConverter<RequestBase>
{
    protected override RequestBase? Create(Type objectType, JObject jsonObject)
    {
        return jsonObject["post_type"].ToString() switch
        {
            "message" => jsonObject["message_type"].ToString() switch
            {
                "private" => new PrivateRequestMessage(),
                "group" => new GroupRequestMessage(),
                _ => null
            },
            _ => null
        };
    }
}
