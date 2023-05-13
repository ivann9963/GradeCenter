using Newtonsoft.Json;

namespace GradeCenter.API.Common
{
    public static class FactoryBuilder
    {
        public static T ToObject<T>(object fromObject)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(fromObject));
        }
    }
}
