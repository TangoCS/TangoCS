using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nephrite.Metamodel.Model
{
    public class N_AppVarRepository
    {
        modelDataContext db;

        public N_AppVarRepository()
        {
            db = AppMM.DataContext;
        }

        public N_AppVar Get(int id)
        {
            return db.N_AppVars.SingleOrDefault(o => o.AppVarID == id);
        }

        public void Save()
        {
            db.SubmitChanges();
        }

        public void Delete(N_AppVar obj)
        {
            db.N_AppVars.DeleteOnSubmit(obj);
        }

        public IQueryable<N_AppVar> GetList()
        {
            return db.N_AppVars;
        }

        public N_AppVar CreateNew()
        {
            N_AppVar obj = new N_AppVar();
            db.N_AppVars.InsertOnSubmit(obj);
            return obj;
        }
    }
}
