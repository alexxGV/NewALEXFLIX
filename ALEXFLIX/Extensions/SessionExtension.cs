using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALEXFLIX.Helpers;

namespace ALEXFLIX.Extensions
{
    public static class SessionExtension
    {
        public static void SetObject(this ISession session, String key, Object objecto)
        {
            String data = ToolkitService.SerializeJsonObject(objecto);
            session.SetString(key, data);
        }

        public static T GetObject<T>(this ISession session, String key)
        {
            String data = session.GetString(key);
            if(data == null)
            {
                return default(T);
            }
            return ToolkitService.DeserializeJsonObject<T>(data);
        }
    }
}
