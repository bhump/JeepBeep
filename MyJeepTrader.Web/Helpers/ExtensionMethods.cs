using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyJeepTrader.Data;
using MyJeepTrader.Web.ViewModels;

namespace MyJeepTrader.Web.Helpers
{
    public static class ExtensionMethods
    {

        public static tModelWithSelected ToModelWithSelected(this tModel model)
        {
            var modelWithSelected = new tModelWithSelected();
            modelWithSelected.TModel = model;
            return modelWithSelected;
        }

        public static List<tModelWithSelected> ToModelWithSelected(this List<tModel> models)
        {
            var modelWithSelectedList = new List<tModelWithSelected>();

            foreach (var model in models)
            {
                modelWithSelectedList.Add(model.ToModelWithSelected());
            }

            return modelWithSelectedList;
        } 
    }
}
