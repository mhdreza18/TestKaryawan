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
using System.Text.RegularExpressions;
using Sap.Data.Hana;

namespace Reza_Web
{
    public class SQLDB
    {
        static SqlConnection con = new SqlConnection();
        static SqlCommand com = new SqlCommand();
        static SqlDataReader dr;
        static SqlDataAdapter da = new SqlDataAdapter();
        static DataSet ds = new DataSet();

        static SqlConnection con2 = new SqlConnection();
        static SqlCommand com2 = new SqlCommand();
        static SqlDataReader dr2;
        static SqlDataAdapter da2 = new SqlDataAdapter();
        static DataSet ds2 = new DataSet();

        static SqlConnection con3 = new SqlConnection();
        static SqlCommand com3 = new SqlCommand();
        static SqlDataReader dr3;
        static SqlDataAdapter da3 = new SqlDataAdapter();
        static DataSet ds3 = new DataSet();

        static HanaConnection conSAP;
        static HanaCommand comSAP = new HanaCommand();
        static HanaDataReader drSAP;
        static HanaDataAdapter daSAP = new HanaDataAdapter();
        static DataSet dsSAP = new DataSet();
        public static DataSet GetSQLDB(string cmd)
        {
            if (ds.Tables.Count > 0)
            {
                ds.Tables.RemoveAt(0);
            }
            ds.Clear();
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            connectionString();
            com = con.CreateCommand();
            com.CommandText = cmd;
            da.SelectCommand = com;
            con.Open();
            da.Fill(ds);
            con.Close();
            return ds;
        }
        public static DataSet GetSQLDB2(string cmd)
        {
            if (ds2.Tables.Count > 0)
            {
                ds2.Tables.RemoveAt(0);
            }
            ds2.Clear();
            if (con2.State == ConnectionState.Open)
            {
                con2.Close();
            }
            else
            {
                connectionString2();
            }
            
            com2 = con2.CreateCommand();
            com2.CommandText = cmd;
            da2.SelectCommand = com2;
            if (con2.State == ConnectionState.Open)
            {
                con2.Close();
            }
            con2.Open();
            da2.Fill(ds2);
            con2.Close();
            return ds2;
        }
        public static DataSet GetSQLDB3(string cmd)
        {
            if (ds3.Tables.Count > 0)
            {
                ds3.Tables.Remove(ds3.Tables[0].TableName);
            }
            ds3.Clear();
            connectionString3();
            com3 = con3.CreateCommand();
            com3.CommandText = cmd;
            da3.SelectCommand = com3;
            con3.Open();
            da3.Fill(ds3);
            con3.Close();
            return ds3;
        }

        public static DataSet GetSQLDBSAP(string cmd)
        {
            dsSAP.Clear();
            connectionStringSAP();
            comSAP = conSAP.CreateCommand();
            comSAP.CommandText = cmd;
            daSAP.SelectCommand = comSAP;
            conSAP.Open();
            daSAP.Fill(dsSAP);
            conSAP.Close();
            return dsSAP;
        }
        static void connectionString()
        {
            con.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Local"].ConnectionString;
        }
        static void connectionString2()
        {
            con2.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Local"].ConnectionString;
        }

        static void connectionString3()
        {
            con3.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Local"].ConnectionString;
        }
        static void connectionStringSAP()
        {
            //conSAP = new HanaConnection("Server=192.168.20.181:30015;UserName=B1ADMIN;Password=Admin#123");
            //conSAP.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DAP_SAP"].ConnectionString;
            //    HanaConnection conx = new HanaConnection(
            //"serverNode=192.168.20.181:30013;UID=B1ADMIN;PWD=Admin#123;databaseName=DAP_TESTSAPWEB");
        //    HanaConnection conn = new HanaConnection(
        //"Server=192.168.20.181:30015;UserID=B1ADMIN;Password=Admin#123;" +
        //"Current Schema=DAP_TESTSAPWEB");
            HanaConnection conn = new HanaConnection();
            conn.ConnectionString = "Server=192.168.20.181:30015;Database=DAP_TESTSAPWEB;" +
                "UID=B1ADMIN;PWD=Admin#123;" +
                "InitString=ALTER SYSTEM LOGGING ON";
            try
            {
                conn.Open();

                conn.Close();
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}