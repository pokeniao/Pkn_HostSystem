using Newtonsoft.Json.Linq;

namespace Pkn_HostSystem.Service.UserDefined
{
    public class BydBase003OrderList
    {
        public string Run(JObject jObject)
        {
            var items = jObject.SelectToken("data") as JArray;

            if (items != null)
            {
                foreach (var item in items)
                {
       

                }
            }
            return null;
        }
    }
}