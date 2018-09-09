using MyEverNote.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEverNote.DataAccessLayer.EFDBFirst
{
    public class RepositoryBase
    {
        private static NoteBookEntities dbcontext;
        private static object loc = new object();

        protected RepositoryBase()
        {

        }
        public static NoteBookEntities CreateContext()
        {
            if (dbcontext == null)
            {
                lock (loc)
                {
                    if (dbcontext == null)
                    {
                        dbcontext = new NoteBookEntities();
                    }
                }
            }
            return dbcontext;
        }
    }
}
