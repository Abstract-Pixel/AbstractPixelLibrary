using Newtonsoft.Json.Linq;

namespace AbstractPixel.Utility.Save
{
    public static class SaveDataConverter
    {
        public static T Convert<T>(object data)
        {
            if (data is T correctType) return correctType;
            if (data is JObject jObject) return jObject.ToObject<T>();
            return default;
        }
    }
}