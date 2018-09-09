using MyEverNote.Common;
using MyEverNote.DataAccessLayer;
using MyEverNote.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyEverNote.DataAccessLayer.EFDBFirst
{
    public class EFRepository<T> : IRepository<T> where T : class
    {
        private NoteBookEntities dbcontext;
        private DbSet<T> dbset;
        private static object LockObject = new object();

        public EFRepository()
        {
            dbcontext = RepositoryBase.CreateContext();
            dbset = dbcontext.Set<T>();
        }

        public void Add(T entity)
        {
            dbset.Add(entity);
            if (entity is EntityBase)
            {
                EntityBase obj = entity as EntityBase;
                DateTime Now = DateTime.Now;
                entity.GetType().GetProperty("CreateOn").SetValue(entity,Now);
                entity.GetType().GetProperty("ModifieOn").SetValue(entity, Now);
                entity.GetType().GetProperty("ModifiedUser").SetValue(entity, App.Common.GetUserName());
            }

            dbcontext.SaveChanges();
        
        }

        public void Delete(T entity)
        {
            dbset.Remove(entity);
            dbcontext.SaveChanges();
        }

        public void Delete(int id)
        {
            T entity = dbset.Find(id);
            if (entity != null)
            {
                dbset.Remove(entity);
            }
        }

        public T Get(Expression<Func<T, bool>> predicate)
        {
            return dbset.Where(predicate).FirstOrDefault();
        }

        public IQueryable<T> GetAll()
        {
            return dbset;
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate)
        {
            return dbset.Where(predicate);
        }

        public T GetById(int id)
        {
            return dbset.Find(id);
        }

        public void Update(T entity)
        {
            if (entity is EntityBase)
            {
                EntityBase obj = entity as EntityBase;
                entity.GetType().GetProperty("ModifieOn").SetValue(entity, DateTime.Now);
                entity.GetType().GetProperty("ModifiedUser").SetValue(entity, App.Common.GetUserName());
            }
           
            dbcontext.SaveChanges();
        }
    }
}
