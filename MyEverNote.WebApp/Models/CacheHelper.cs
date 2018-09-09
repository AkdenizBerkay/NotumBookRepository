using MyEverNote.BusinessLayer;
using MyEverNote.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace MyEverNote.WebApp.Models
{
    public class CacheHelper
    {
        public static List<Category> GetCategoriesFromCache()
        {
            var result = WebCache.Get("categorycache");
            if (result == null)
            {
                Manager<Category> cm = new Manager<Category>();
                result = cm.GetAll().ToList();
                WebCache.Set("categorycache", result, 20, true);
            }
            return result;
        }

        public static void RemoveCategoriesFromCache()
        {
            Remove("categorycache");
        }

        public static void Remove(string key)
        {
            WebCache.Remove(key);
        }
    }
}