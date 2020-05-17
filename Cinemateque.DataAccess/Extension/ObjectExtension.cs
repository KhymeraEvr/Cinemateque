using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Cinemateque.DataAccess.Extension
{
   public static class ObjectExtension
   {
      public static dynamic DictionaryToObject(IDictionary<String, Object> dictionary)
      {
         var expandoObj = new ExpandoObject();
         var expandoObjCollection = (ICollection<KeyValuePair<String, Object>>)expandoObj;

         foreach (var keyValuePair in dictionary)
         {
            expandoObjCollection.Add(keyValuePair);
         }
         dynamic eoDynamic = expandoObj;
         return eoDynamic;
      }
   }
}
