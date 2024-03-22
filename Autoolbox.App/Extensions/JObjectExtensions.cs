using Newtonsoft.Json.Linq;

namespace Autoolbox.App.Extensions;

public static class JObjectExtensions
{
    public static T CloneAs<T>(this JToken token) where T: JToken
    {
        return token.DeepClone() as T ?? throw new InvalidOperationException();
    }

    public static JObject MergeWith(this JObject source, JObject target)
    {
        var clone = source.CloneAs<JObject>();

        clone.Merge(target, new JsonMergeSettings
        {
            MergeArrayHandling = MergeArrayHandling.Union
        });

        return clone;
    }
}