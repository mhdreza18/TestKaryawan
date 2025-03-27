using System;
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
using System.Web;
using System.Data;
using System.Web.Mvc;
using Reza_Web.Models;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Reza_Web.Connection;
using System.Xml.Linq;

namespace Reza_Web.Controllers
{
    public class HomeController : Controller
    {
        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;
        SqlDataAdapter da = new SqlDataAdapter();
        DataSet ds = new DataSet();
        public ActionResult Index()
        {
            
            if (!string.IsNullOrEmpty(Session["login"] as string))
            {
                
                connectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = "select * from Tm_Users where DAPCardCode = '" + Session["login"] as string + "' And Active = 1";
                dr = com.ExecuteReader();
                if (dr.Read())
                {
                    con.Close();
                    

                    if (Session["privilege"] as string == "1")
                    {
                        ViewModel mymodel = new ViewModel();
                        mymodel.Admin = GetAdmin(Session["Login"] as string);
                        return View(mymodel);

                    }
                    else if (Session["privilege"] as string == "2")
                    {
                        ViewModel mymodel = new ViewModel();
                        mymodel.Admin = GetAdmin(Session["Login"] as string);
                        
                        return View(mymodel);
                    }
                    else
                    {
                        ViewModel mymodel = new ViewModel();
                        
                        mymodel.Admin = GetAdmin(Session["Login"] as string);
                       
                        return View(mymodel);
                    }

                   
                }
                else
                {
                    con.Close();
                    var eRr = new Models.Home.Error() { txt = "" };
                    return View("Login", eRr);
                }
            }
            else
            {
                var eRr = new Models.Home.Error() { txt = "" };
                return View("Login", eRr);
            }
        }
        public ActionResult rLogin()
        {
            
                var eRr = new Models.Home.Error() { txt = "" };
                return View("Login", eRr);
            
        }
        public ActionResult SignOut()
        {
            Session.Clear();
            Session.Abandon();

            var eRr = new Models.Home.Error() { txt = "" };
            return View("Login", eRr);

        }
        
        void connectionString()
        {
           
                con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["local"].ConnectionString;           
        }

        public ActionResult UpdateAutho(AuthoUpdate change)
        {

            SQLDB.GetSQLDB("delete from Tm_Autho where DAPCardCode = '" + change.DAPCardCode + "'");
            foreach (AuthoUpdate.ChildrenUpdate item in change.ListChildren)
            {
                if (item.Status == true)
                {
                    SQLDB.GetSQLDB("insert into Tm_Autho values ('" + change.DAPCardCode + "','" + item.Id + "',NULL)");
                }

            }
            return Json(new { success = true, responseText = "Authorization Has Been Updated!!" });
        }

        [HttpPost]
        public ActionResult Add(Login data)
        {
            connectionString();
            con.Open();
            com.Connection = con;
           com.CommandText = "select T0.* from Tm_Users T0 where DAPCardCode = 'REZA' ";
            dr = com.ExecuteReader();
            if (dr.Read())
            {
                if ("xxxxx" == "xxxxx")
                {
                    Session["login"] = "REZA";
                    Session["loginName"] = dr["CardName"].ToString();
                    Session["privilege"] = dr["Privilege"].ToString();
                
                    con.Close();
                    return Json(new { success = true, responseText = "Password Oke" });

                }
                else
                {
                    con.Close();
                    return Json(new { success = false, responseText = "Invalid Password!!" });
                }

            }
            else
            {
                con.Close();
                return Json(new { success = false, responseText = "Invalid User!!" });

            }
        }

        public JsonResult GetUpdateAutho(string DAPCardCode)
        {
            try
            {
                List<Autho> header = new List<Autho>();

                header = getAutho(DAPCardCode, "");
                return Json(header);

            }

            catch (Exception ex)
            {
                if (ex.Message != null)
                    Console.WriteLine("IOException source: {0}", ex.Message);
                return Json("Empty!");

            }
        }
        public List<Autho> getAutho(string param = "",string typ = "")
        {
            string sql = "";
            sql = "select T1.DAPCardCode,Alias, T0.Id, Module, (Case when T1.DAPCardCode IS NULL then 0 else 1 end) Status from Tm_Modules T0 left join (select * from Tm_Autho where DAPCardCode = '" + param + "') T1 on T0.id = T1.ModuleId  where SubModule IS NULL order by T0.id ";
            
            DataSet dsAuthoChil = new DataSet();
            DataSet dsAutho = new DataSet();
            
            List<Autho> header = new List<Autho>();
            
            dsAutho = SQLDB.GetSQLDB(sql);

            foreach (DataRow item in dsAutho.Tables[0].Rows)
            {
                var headerAdd = new Autho();
                dsAuthoChil.Clear();
                dsAuthoChil = SQLDB.GetSQLDB2("select T1.DAPCardCode,Alias, T0.Id, SubModule, (Case when T1.DAPCardCode IS NULL then 0 else 1 end) Status from Tm_Modules T0 left join (select * from Tm_Autho where DAPCardCode = '" + param + "') T1 on T0.id = T1.ModuleId  where ParentId = '" + item["Id"] as string + "' order by T1.Id");
                                    
                if (dsAuthoChil.Tables[0].Rows.Count > 0)
                {
                    List<Autho.Children> details = new List<Autho.Children>();
                    foreach (DataRow re in dsAuthoChil.Tables[0].Rows)
                    {
                        details.Add(new Autho.Children()
                        {
                            Id = Convert.ToInt32(re["Id"]),
                            SubAlias = re["Alias"] as string,
                            Status = Convert.ToInt32(re["Status"]),
                            Module = re["SubModule"] as string
                        });
                    }
                    headerAdd.ListChildren = details;
                }
                headerAdd.Id = Convert.ToInt32(item["Id"]);
                headerAdd.DAPCardCode = item["DAPCardCode"] as string;
                headerAdd.Alias = item["Alias"] as string;
                headerAdd.Status = Convert.ToInt32(item["Status"]);
                headerAdd.Module = item["Module"] as string;
                header.Add(headerAdd);
            }
            if (typ == "view")
            {
                header = header.Where(w => w.Status == 1).ToList();
                foreach (var q in header)
                {
                    if (q.ListChildren != null)
                    {
                        q.ListChildren.RemoveAll(o => o.Status == 0);
                    }
                    
                }
            }
            return header;
        }

        public JsonResult SetMenu()
        {
            try
            {
                string id = Session["Login"] as string;
                List<Autho> header = new List<Autho>();

                header = getAutho(id,"view");
                return Json(header);

            }

            catch (Exception ex)
            {
                if (ex.Message != null)
                    Console.WriteLine("IOException source: {0}", ex.Message);
                return Json("Empty!");

            }

        }

        public ActionResult CustProfile()
        {
            if (!string.IsNullOrEmpty(Session["login"] as string))
                {
                string id = Session["Login"] as string;
                
                if (Session["privilege"] as string == "1")
                {
                    ViewModel mymodel = new ViewModel();
                    //mymodel.Customer = GetCustomerProfile(id);
                  
                    mymodel.Admin = GetAdmin(id);
                  
                    return View("Index", mymodel);
                   
                }
                else
                {
                    ViewModel mymodel = new ViewModel();
                   
                    mymodel.Admin = GetAdmin(id);
                  
                    return View("Index", mymodel);
                }
            }
            else
            {
                var eRr = new Models.Home.Error() { txt = "" };
                return View("Login", eRr);
            }

            //return View("Details",mymodel);
        }
        
        public ActionResult Change()
        {
            if (Session["login"] == null)
            {
                var eRr = new Models.Home.Error() { txt = "" };
                return Redirect("~/");
            }
            else
            {
                string id = Session["Login"] as string;
              
                ViewModelAdmin myModel = new ViewModelAdmin();
                myModel.Admin = GetAdmin(Session["Login"] as string);
                return View(myModel);
            }
            

        }


        public ActionResult UserMaster()
        {
            if (Session["login"] == null)
            {
                var eRr = new Models.Home.Error() { txt = "" };
                return Redirect("~/");
            }
            else
            {
                string id = Session["Login"] as string;
              
                ViewModelAdmin myModel = new ViewModelAdmin();
                myModel.Admin = GetAdmin(Session["Login"] as string);
                myModel.UserMaster = GetUserMaster();
                return View(myModel);
            }


        }


        public ActionResult KaryawanMaster()
        {
            if (Session["login"] == null)
            {
                var eRr = new Models.Home.Error() { txt = "" };
                return Redirect("~/");
            }
            else
            {
                string id = Session["Login"] as string;
               
                ViewModelAdmin myModel = new ViewModelAdmin();
                myModel.Admin = GetAdmin(Session["Login"] as string);
                myModel.KaryawanMaster = GetKaryawanMaster();
                return View(myModel);
            }


        }




        public ActionResult ReportSettings()
        {
            if (Session["login"] == null)
            {
                var eRr = new Models.Home.Error() { txt = "" };
                return Redirect("~/");
            }
            else
            {
                string id = Session["Login"] as string;
                ViewModelAdmin mymodel = new ViewModelAdmin();
                mymodel.Admin = GetAdmin(id);
             
                return View(mymodel);
            }


        }

        public ActionResult Reports()
        {
            if (Session["login"] == null)
            {
                var eRr = new Models.Home.Error() { txt = "" };
                return Redirect("~/");
            }
            else
            {
                string id = Session["Login"] as string;
                ViewModelAdmin mymodel = new ViewModelAdmin();
                mymodel.Admin = GetAdmin(id);
              
                return View(mymodel);
            }


        }


        [HttpGet]
        public ActionResult Download(string fileName)
        {
            string fullPath = Path.Combine(Server.MapPath("~/Content/reports"), fileName);
            byte[] fileByteArray = System.IO.File.ReadAllBytes(fullPath);
            System.IO.File.Delete(fullPath);
            return File(fileByteArray, "application/vnd.ms-excel", fileName);
        }

        public static string dataSetToJsonSt(DataSet dataDs)
        {

            int rowsCountIn = dataDs.Tables[0].Rows.Count;
            int colsCountIn = dataDs.Tables[0].Columns.Count;
            string[] columnNamesAr = new string[dataDs.Tables[0].Columns.Count];

            for (int i = 0; i < colsCountIn; i++)
            {

                columnNamesAr[i] = dataDs.Tables[0].Columns[i].ColumnName;
            }

            string jsonSt = "[";

            for (int i = 0; i < rowsCountIn; i++)
            {
                jsonSt += "{";

                for (int j = 0; j < colsCountIn; j++)
                {
                    jsonSt += ((jsonSt.Substring(jsonSt.Length - 1) == "{" ? "" : ",") + "\"" + columnNamesAr[j] + "\":" + (dataDs.Tables[0].Rows[i][j] == DBNull.Value ? "null" : ("\"" + dataDs.Tables[0].Rows[i][j] + "\"")));
                }

                jsonSt += ("}" + (i < (rowsCountIn - 1) ? "," : ""));
            }

            jsonSt += "]";

            return jsonSt;
        }
        public static List<Login> GetAdmin(string id)
        {

            Utils control = new Utils();
            List<Login> header = new List<Login>();
            DataSet dsLogin = new DataSet();
            dsLogin = SQLDB.GetSQLDB("select DAPCardCode,CardName,Privilege from Tm_Users where DAPCardCode = '" + id + "' order by CardName");
            if (dsLogin.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow ro in dsLogin.Tables[0].Rows)
                {
                    Login model = new Login()
                    {
                        CardCode = ro["DAPCardCode"] as string,
                        DAPCardCode = ro["DAPCardCode"] as string,
                        Privilege = Convert.ToString(ro["Privilege"]),
                        CardName = ro["CardName"] as string
                    };
                    header.Add(model);
                }
            }

            return header;
        }



        public ActionResult Login()
        {
            if (Session["login"] == null)
            {
                var eRr = new Models.Home.Error() { txt = "" };
                return Redirect("~/");
            }
            string id = Session["Login"] as string;
           
            if (Session["privilege"] as string == "1")
            {
                ViewModel mymodel = new ViewModel();
                mymodel.Admin = GetAdmin(id);
                return View("Index", mymodel);
                
            }
            else if (Session["privilege"] as string == "2")
            {
                ViewModel mymodel = new ViewModel();
                mymodel.Admin = GetAdmin(id);
                return View("Index", mymodel);
            }
            else
            {
                ViewModel mymodel = new ViewModel();
                mymodel.Admin = GetAdmin(id);
                return View("Index", mymodel);
            }
        }
        public ActionResult Autho()
        {
            if (Session["login"] == null)
            {
                var eRr = new Models.Home.Error() { txt = "" };
                return Redirect("~/");
            }
            else
            {
                string id = Session["Login"] as string;
              
                if (Session["privilege"] as string == "1")
                {
                    ViewModelAdmin mymodel = new ViewModelAdmin();
                    mymodel.Admin = GetAdmin(Session["Login"] as string);
                    mymodel.Autho = getAutho(Session["Login"] as string);
                    return View(mymodel);
                }
                else
                {
                    ViewModel mymodel = new ViewModel();
                
                    mymodel.Admin = GetAdmin(Session["Login"] as string);
                    return View(mymodel);
                }
            }


        }

        public static List<UserMaster> GetUserMaster()
        {
            DataSet dsSQDtl = new DataSet();
            List<UserMaster> details = new List<UserMaster>();
            try
            {

                dsSQDtl.Clear();
                dsSQDtl = SQLDB.GetSQLDB2("select DAPCardCode,CardName,Address,Privilege from Tm_Users");

                if (dsSQDtl.Tables[0].Rows.Count > 0)
                {

                    foreach (DataRow re in dsSQDtl.Tables[0].Rows)
                    {
                        details.Add(new UserMaster()
                        {
                          
                            DAPCardCode = re["DAPCardCode"] as string,
                            CardName = re["CardName"] as string,
                            Privilege = Convert.ToDecimal(re["Privilege"]).ToString("N0"),
                             Address = re["Address"] as string,
                        });
                    }
                }

                return details;
            }


            catch (Exception ex)
            {
                if (ex.Message != null)
                    Console.WriteLine("IOException source: {0}", ex.Message);
                return details;

            }
        }


        public static List<KaryawanMaster> GetKaryawanMaster()
        {
            DataSet dsSQDtl = new DataSet();
            List<KaryawanMaster> details = new List<KaryawanMaster>();
            try
            {

                dsSQDtl.Clear();
                dsSQDtl = SQLDB.GetSQLDB2("select * from Karyawan");

                if (dsSQDtl.Tables[0].Rows.Count > 0)
                {

                    foreach (DataRow re in dsSQDtl.Tables[0].Rows)
                    {
                        details.Add(new KaryawanMaster()
                        {

                            IDKaryawan = re["IDKaryawan"] as string,
                            NmKaryawan = re["NmKaryawan"] as string,
                            TglMasukKerja = Convert.ToDateTime(re["TglMasukKerja"]).ToString("MM/dd/yyyy"),
                            Usia = Convert.ToDecimal(re["Usia"]).ToString("N0")
                        });
                    }
                }

                return details;
            }


            catch (Exception ex)
            {
                if (ex.Message != null)
                    Console.WriteLine("IOException source: {0}", ex.Message);
                return details;

            }
        }









    }
}