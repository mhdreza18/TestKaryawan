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
    public class KaryawanController : Controller
    {
        
        public ActionResult Add(KaryawanMaster Karyawanm)
        {

            //string AddKaryawanMaster;
            DataSet dm = new DataSet();
            dm = SQLDB.GetSQLDB("select * from Karyawan where IDKaryawan = '" + Karyawanm.IDKaryawan + "'");
            if (dm.Tables[0].Rows.Count == 0)
            {
                
                    SQLDB.GetSQLDB("insert into Karyawan values ('" + Karyawanm.IDKaryawan + "','" + Karyawanm.NmKaryawan + "','" + Karyawanm.TglMasukKerja + "','" + Karyawanm.Usia + "')");

                    return Json(new { success = false, responseText = "Karyawan Baru berhasil ditambah !!" });
               
               
            }
            else
            {
                return Json(new { success = false, responseText = "Karyawan dengan ID tersebut Sudah Terdaftar!!" });
            }

        }


        public ActionResult Edit(KaryawanMaster Karyawanm)
        {

            DataSet dm = new DataSet();
            dm = SQLDB.GetSQLDB("select * from Karyawan where IDKaryawan = '" + Karyawanm.IDKaryawan + "'");
            if (dm.Tables[0].Rows.Count == 1)
            {
            
                    SQLDB.GetSQLDB(" UPDATE Karyawan SET NmKaryawan = '" + Karyawanm.NmKaryawan + "', TglMasukKerja = '" + Karyawanm.TglMasukKerja + "', Usia = " + Karyawanm.Usia + " where IDKaryawan = '" + Karyawanm.IDKaryawan + "' ");

                    return Json(new { success = false, responseText = "Karyawan " + Karyawanm.IDKaryawan + " berhasil di Edit!!" });
                
            }
            else
            {
                return Json(new { success = false, responseText = "Karyawanname " + Karyawanm.IDKaryawan + " Tidak Ditemukan!!" });
            }

        }



        [HttpPost]
        public JsonResult DeleteKaryawanm(KaryawanMaster Karyawanm)
        {
            if (Karyawanm.IDKaryawan != null)
            {
                SQLDB.GetSQLDB("delete from Karyawan where IDKaryawan = '" + Karyawanm.IDKaryawan + "' ");
            }
            else
            {
                return Json(new { success = false, responseText = "Karyawan name Not Found!!" });
            }
            return Json("Karyawan " + Karyawanm.IDKaryawan + " Berhasil di Hapus!");
        }


        [HttpPost]
        public ActionResult KaryawanMDelete(string delList)
        {
            if (delList != "")
            {
                SQLDB.GetSQLDB("delete from Karyawan where IDKaryawan in (" + delList + ")");
                return Json(new { result = true });
            }
            else
            {
                return Json(new { result = false });
            }

        }



        [HttpPost]
        public ActionResult EditCHECK(KaryawanMaster Karyawanm)
        {

            DataSet dm = new DataSet();
            dm = SQLDB.GetSQLDB("select * from Karyawan where IDKaryawan = '" + Karyawanm.IDKaryawan + "'");
            if (dm.Tables[0].Rows.Count == 1)
            {

                SQLDB.GetSQLDB(" UPDATE Karyawan SET NmKaryawan = '" + Karyawanm.NmKaryawan + "', TglMasukKerja = '" + Karyawanm.TglMasukKerja + "', Usia = " + Karyawanm.Usia + " where IDKaryawan = '" + Karyawanm.IDKaryawan + "' ");

                return Json(new { success = false, responseText = "Karyawan " + Karyawanm.IDKaryawan + " berhasil di Edit!!" });

            }
            else
            {
                return Json(new { success = false, responseText = "Karyawanname " + Karyawanm.IDKaryawan + " Tidak Ditemukan!!" });
            }

        }


        public JsonResult GetKaryawanList(string NamaAwal = "", string NamaAkhir = "", string UsiaAwal = "", string UsiaAkhir = "", string TanggalAwal = "", string TanggalAkhir = "")
        {
            string id = Session["Login"] as string;
          

            DataSet dsSQDtl = new DataSet();
            DataSet dsWeek = new DataSet();
            List<KaryawanMaster> header = new List<KaryawanMaster>();

            string txtNama = "";
            string txtUsia = "";
            string txtTanggal = "";



            string year = DateTime.Now.ToString("yyyy");
            string month = DateTime.Now.ToString("MM");
            string date = DateTime.Now.ToString("dd");


            //Mencari tanggal saat ini berada di Hari apa
            DateTime dt = DateTime.Now;
            DayOfWeek dow = dt.DayOfWeek;
            string dayName = dow.ToString();

            var CariNamaAwal = NamaAwal.ToString().ToUpper();
            var CariNamaAkhir = NamaAkhir.ToString().ToUpper();

            
            txtNama = "  NmKaryawan LIKE '%" + CariNamaAwal + "%' ";

            if (NamaAkhir != "")
            {
                txtNama = "  NmKaryawan LIKE '[" + CariNamaAwal + "-" + CariNamaAkhir + "]%' ";
            }
            
            if (UsiaAwal != "")
            {
                txtUsia = " AND Usia = '" + UsiaAwal + "' ";

                if (UsiaAkhir != "")
                {
                    txtUsia = " AND Usia BETWEEN '" + UsiaAwal + "' AND '" + UsiaAkhir + "' ";
                }
            }

            if (TanggalAwal != "")
            {
                txtTanggal = " AND  TglMasukKerja LIKE '" + TanggalAwal + "%' ";

                if (TanggalAkhir != "")
                {
                    txtTanggal = " AND  TglMasukKerja BETWEEN '" + TanggalAwal + "' AND '" + TanggalAkhir + "' ";
                }
            }



            if (id != "")
            {
                dsSQDtl.Clear();
                dsSQDtl = SQLDB.GetSQLDB2("Select * From Karyawan Where " + txtNama + " " + txtUsia + " " + txtTanggal + " ");

                if (dsSQDtl.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow re in dsSQDtl.Tables[0].Rows)
                    {
                        header.Add(new KaryawanMaster()
                        {
                            IDKaryawan = re["IDKaryawan"] as string,
                            NmKaryawan = re["NmKaryawan"] as string,
                            TglMasukKerja = Convert.ToDateTime(re["TglMasukKerja"]).ToString("yyyy/MM/dd"),
                            Usia = Convert.ToDecimal(re["Usia"]).ToString("N0")
                        });
                    }
                }
                return Json(header);
            }
            else
            {
                return Json(header);
            }

        }



    }
}
