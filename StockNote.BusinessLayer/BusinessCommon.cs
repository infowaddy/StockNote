using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StockNote.BusinessLayer
{
    public static class BusinessCommon
    {
        public static T UpdateStatus<T>(T _updateObject)
        {
            foreach (PropertyInfo pinfo in _updateObject.GetType().GetProperties())
            {
                if (pinfo.Name.ToLower() == "modifiedon")
                    pinfo.SetValue(_updateObject, DateTime.Now, null);
            }
            return _updateObject;
        }

        public static T CreateStatus<T>(T _updateObject)
        {
            foreach (PropertyInfo pinfo in _updateObject.GetType().GetProperties())
            {
                if (pinfo.Name.ToLower() == "createdon")
                    pinfo.SetValue(_updateObject, DateTime.Now, null);
            }
            return _updateObject;
        }
    }
}
