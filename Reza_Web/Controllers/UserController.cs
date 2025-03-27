using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Web.Mvc;
using Reza_Web.Models;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Xml.Linq;
using SAPbobsCOM;
using System.Linq;

namespace Reza_Web.Controllers
{
    public class UserController : Controller
    {
        
        public ActionResult Add(UserMaster userm)
        {

            //string AddUserMaster;
            DataSet dm = new DataSet();
            dm = SQLDB.GetSQLDB("select * from Tm_Users where DAPCardCode = '" + userm.DAPCardCode + "'");
            if (dm.Tables[0].Rows.Count == 0)
            {
                if (userm.Password == userm.CPassword)
                {
                    SQLDB.GetSQLDB("insert into Tm_Users values ('" + userm.CardCode + "','" + userm.DAPCardCode + "','" + userm.CardName + "','" + userm.Address + "','" + Generate.StringCipher.Encrypt("xxxxx", userm.Password) + "','1','" + Convert.ToInt32(userm.Privilege) + "','" + DateTime.Now.ToString("yyyyMMdd") + "','" + userm.Branch_Id + "')");

                    return Json(new { success = false, responseText = "New User has been Add!!" });
                }
                else
                {
                    return Json(new { success = false, responseText = "Password Not Match!!" });
                }
            }
            else
            {
                return Json(new { success = false, responseText = "Username already exists!!" });
            }

        }


        public ActionResult Edit(UserMaster userm)
        {

            DataSet dm = new DataSet();
            dm = SQLDB.GetSQLDB("select * from Tm_Users where DAPCardCode = '" + userm.DAPCardCode + "'");
            if (dm.Tables[0].Rows.Count == 1)
            {
                if (userm.Password == userm.CPassword)
                {
                    SQLDB.GetSQLDB(" UPDATE Tm_Users SET CardName = '" + userm.CardName + "', Address = '" + userm.Address + "', Password = '" + Generate.StringCipher.Encrypt("xxxxx", userm.Password) + "', Privilege = '" + Convert.ToInt32(userm.Privilege) + "', Branch_Id = '" + userm.Branch_Id + "' WHERE DAPCardCode = '" + userm.DAPCardCode + "'  ");

                    return Json(new { success = false, responseText = "User has been Edit!!" });
                }
                else
                {
                    return Json(new { success = false, responseText = "Password Not Match!!" });
                }
            }
            else
            {
                return Json(new { success = false, responseText = "Username Not Found!!" });
            }

        }



        [HttpPost]
        public JsonResult DeleteUserm(UserMaster userm)
        {
            if (userm.DAPCardCode != null)
            {
                SQLDB.GetSQLDB("delete from Tm_Users where DAPCardCode = '" + userm.DAPCardCode + "' ");
            }
            else
            {
                return Json(new { success = false, responseText = "Username Not Found!!" });
            }
            return Json("Successfully Deleting Data!");
        }



    }
}