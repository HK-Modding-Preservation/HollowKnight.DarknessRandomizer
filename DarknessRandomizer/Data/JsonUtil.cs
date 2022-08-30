using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;

namespace DarknessRandomizer
{
    // Borrowed from https://github.com/homothetyhk/BenchRando/blob/master/BenchRando/JsonUtil.cs
    internal static class JsonUtil
    {
        public static T DeserializeEmbedded<T>(string embeddedResourcePath)
        {
            using StreamReader sr = new(typeof(JsonUtil).Assembly.GetManifestResourceStream(embeddedResourcePath));
            using JsonTextReader jtr = new(sr);
            return _js.Deserialize<T>(jtr);
        }
        public static T DeserializeFromPath<T>(string path)
        {
            using StreamReader sr = new(path);
            using JsonTextReader jtr = new(sr);
            return _js.Deserialize<T>(jtr);
        }

        public static void Serialize(object o, string fileName)
        {
            using StreamWriter sw = new(File.OpenWrite(Path.Combine(Path.GetDirectoryName(typeof(JsonUtil).Assembly.Location), fileName)));
            _js.Serialize(sw, o);
        }

        public static void Serialize(object o, TextWriter tw)
        {
            _js.Serialize(tw, o);
        }

        internal static readonly JsonSerializer _js;

        static JsonUtil()
        {
            _js = new JsonSerializer
            {
                DefaultValueHandling = DefaultValueHandling.Include,
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto,
            };

            _js.Converters.Add(new StringEnumConverter());
        }
    }
}
