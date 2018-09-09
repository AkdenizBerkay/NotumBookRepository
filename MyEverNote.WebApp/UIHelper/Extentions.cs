using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace MyEverNote.WebApp.UIHelper
{
    public static class Extentions
    {
        public enum ButtonClass
        {
            primary=0,
            success = 1,
            info = 2,
            warning = 3,
            danger = 4
        }

        public enum IconClass
        {

            plus = 1,
            edit = 2,
            remove = 3

        }

        public static MvcHtmlString MyButton(this HtmlHelper helper, string id, ButtonClass buttonClass, string value, IconClass? icon = null, string controller = "", string view = "")
        {
            string btn = string.Format("<a id='{0}' href='{5}'style='border-radius:30px' type='submit' class='btn btn-{1}'><span class='glyphicon glyphicon-{3}'></span>{2}</a>", id, buttonClass, value, icon, controller, view);
            return MvcHtmlString.Create(btn);
        }


        public static MvcHtmlString GridMBA<T>(this HtmlHelper helper, List<T> Datas, string controller, string editaction, string deleteaction, string klass = "table", string Id = "MBAGrid",List<T> FKs=null)
        {

            string columnNames = "";
            List<string> Rows = new List<string>();


            int propertycount = Datas[0].GetType().GetProperties().Count();

            PropertyInfo[] Props = Datas[0].GetType().GetProperties();

            for (int i = 0; i < propertycount; i++)
            {
                if (Datas[0].GetType().GetProperties()[i].GetGetMethod().IsVirtual == false)
                {
                    columnNames += "<th>" + Datas[0].GetType().GetProperties()[i].ToString().Split(' ')[1] + "</th>";
                }
            }

            for (int i = 0; i < Datas.Count; i++)
            {
                string rows = "";
                string tds = "";
                rows += "<tr>";

                for (int j = 0; j < propertycount; j++)
                {
                    if (Datas[0].GetType().GetProperties()[j].GetGetMethod().IsVirtual == false)
                    {
                        //if (Datas[0].GetType().GetProperties()[j].Name.ToUpper().Contains("ID") == true)
                        //{
                        //    string fkcolumnname = Datas[0].GetType().GetProperties()[j].Name;
                        //    string fktablename = fkcolumnname.Split('I')[0];
                        //    var a= (T)Convert.ChangeType(fktablename, typeof(T));


                        //}
                        tds += "<td>" + Datas[i].GetType().GetProperties()[j].GetValue(Datas[i]) + "</td>";
                    }
                }
                rows += tds;
                rows += "<td>"
                          + "<a class='btn btn-success' href='/" + controller + "/" + editaction + "/" + Datas[i].GetType().GetProperties().Where(k=>k.Name.ToUpper().Equals("ID")).FirstOrDefault().GetValue(Datas[i]) + "'>"
                          + "<span class='glyphicon glyphicon-edit'>Düzenle</span>"
                          + "</a></td>"
                          + " "
                          + "<td><a class='btn btn-danger' href='/" + controller + "/" + deleteaction + "/" + Datas[i].GetType().GetProperties().Where(k => k.Name.ToUpper().Equals("ID")).FirstOrDefault().GetValue(Datas[i]) + "'>"
                          + "<span class='glyphicon glyphicon-remove'>Sil</span>"
                          + "</a></td></tr>";

                Rows.Add(rows);
            }

            string grid = "<div class='container'><div class='row'><table id=" + Id + " class=" + klass + "><thead><tr>" + columnNames + "</tr ></thead><tbody>";

            foreach (var r in Rows)
            {
                grid += r.ToString();
            }

            grid += "</tbody></table></div></div>";


            return MvcHtmlString.Create(grid);
        }
    }
}