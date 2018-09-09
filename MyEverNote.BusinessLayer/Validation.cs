using MyEverNote.DataAccessLayer.EFDBFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyEverNote.BusinessLayer
{
    public class Validation<T> where T : class
    {

        public bool IsUnique(T entity, string property,bool ignoreown=false)
        {
            //bool yenikayit = true;
            

            EFRepository<T> ef = new EFRepository<T>();
            
            PropertyInfo[] Props = entity.GetType().GetProperties();
            string pro = Props.Where(k => k.Name.ToLower().Equals(property)).FirstOrDefault().Name;


            List<T> liste = ef.GetAll().ToList();
            List<string> DbProValues = new List<string>();

            int id = Convert.ToInt32(entity.GetType().GetProperties().Where(k => k.Name.Equals("Id")).FirstOrDefault().GetValue(entity));

            T own = (T)Activator.CreateInstance(typeof(T));

            if (id != 0)
            {
                own = ef.GetById(id);

                liste.Remove(own);
            }

            foreach (T item in liste)
            {
           

                string DbProVal = item.GetType().GetProperty(pro).GetValue(item).ToString();
                if (!string.IsNullOrEmpty(DbProVal))
                {
                    DbProValues.Add(DbProVal);
                }
            }
            liste.Add(own);

            if (entity.GetType().GetProperty(pro).GetValue(entity)!=null)
            {
                string val = entity.GetType().GetProperty(pro).GetValue(entity).ToString();



                if (DbProValues.Exists(k => k.Equals(val)))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
            //}

            //else
            //{
            //    DbProValues.Remove(DbProValues.Where(k => k == val).FirstOrDefault());

            //    if (DbProValues.Count(k => k == val) > 0)
            //    {
            //        return false;
            //    }
            //    else
            //    {
            //        return true;
            //    }
            //}
            //T instance = (T)Activator.CreateInstance(typeof(T));

            //List<T> liste = ef.GetAll().ToList();
            //string ss =liste[0].GetType().GetProperty(pro).GetValue(entity).ToString();

            //List<> DbProValues = ef.GetAll().ToList()[0].GetType().GetProperty(pro).GetValue(liste);

            //T same = ef.GetAll().Where(k => k.GetType().GetProperty(pro.ToString()).GetValue(entity).ToString().ToLower().Equals(property.ToLower())).FirstOrDefault();

            //var param = Expression.Parameter(typeof(T));
            //var a = Expression.Equal(Expression.Property(param, pro),Expression.Constant(property));

            //if (same != null)
            //{
            //    return false;
            //}
            //else
            //{
            //return true;
            //}
        }
    }
}
