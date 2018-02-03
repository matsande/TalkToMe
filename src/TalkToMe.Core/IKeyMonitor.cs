using System;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace TalkToMe.Core
{
    public interface IKeyMonitor
    {
        IObservable<KeyInfo> KeysObservable
        {
            get;
        }
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class KeyInfo : IEquatable<KeyInfo>
    {
        public KeyInfo(Keys key, Keys modifier)
        {
            this.key = key;
            this.modifier = modifier;
        }

        public Keys Key => this.key;
        public Keys Modifier => this.modifier;

        public override int GetHashCode()
        {
            return this.Key.GetHashCode() ^ this.Modifier.GetHashCode();
        }

        public bool Equals(KeyInfo other)
        {
            return this.Key.Equals(other.Key) && this.Modifier.Equals(other.Modifier);
        }

        [JsonProperty]
        private readonly Keys key;

        [JsonProperty]
        private readonly Keys modifier;
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class KeyInfoAndCommand
    {
        [JsonProperty]
        private readonly KeyInfo keyInfo;

        [JsonProperty]
        private readonly CommandType command;

        public KeyInfoAndCommand(KeyInfo keyInfo, CommandType command)
        {
            this.keyInfo = keyInfo;
            this.command = command;
        }

        public KeyInfo KeyInfo => this.keyInfo;
        public CommandType Command => this.command;
    }

    //public class KeyInfoConverter : Newtonsoft.Json.JsonConverter
    //{
    //    public override bool CanConvert(Type objectType)
    //    {
    //        return objectType == typeof(KeyInfo);
    //    }

    //    public override object ReadJson(
    //        JsonReader reader,
    //        Type objectType,
    //        object existingValue,
    //        JsonSerializer serializer)
    //    {
    //        if (reader.TokenType == Newtonsoft.Json.JsonToken.Null)
    //            return null;

    //        var jObject = Newtonsoft.Json.Linq.JObject.Load(reader);

    //        var target = new KeyInfo(
    //            (Keys)(int)jObject["key"], (Keys)(int)jObject["modifier"]);

    //        return target;
    //    }

    //    public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
    //    {
    //        serializer.Serialize(writer, value);
    //    }
    //}
}
