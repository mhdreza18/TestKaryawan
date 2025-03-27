using System;

namespace Reza_Web.Connection
{
    public class Connection
    {
        public SAPbobsCOM.Company companyInstance = null;
        public string identity = "";
        public bool inUse = false;

        public Connection(string Servername, string Dbuser, string Dbpassword, string SAPDBName, string SAPUser, string SAPUserPassword, string SAPLicense)
        {
            int errCode = 0;
            string errMessage = "";
            try
            {
                SAPbobsCOM.Company oCompany = new SAPbobsCOM.Company();
                //if (oCompany.Connected == true)
                //{
                //    oCompany.Disconnect();
                //}
                oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB;
                //if (Properties.Settings.Default.StrDbVersion == "2017")
                //{
                //    oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2017;
                //}
                //else if (Properties.Settings.Default.StrDbVersion == "2016")
                //{
                //    oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2016;
                //}
                oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB;
                oCompany.Server = Servername;
                oCompany.DbUserName = Dbuser;
                oCompany.DbPassword = Dbpassword;
                oCompany.CompanyDB = SAPDBName;
                oCompany.UserName = SAPUser;
                oCompany.Password = SAPUserPassword;
                oCompany.LicenseServer = SAPLicense;
                //oCompany.SLDServer = "HANASVR:40000";
                //if (oCompany.Connected == true)
                //{
                //    oCompany.Disconnect();
                //}
                int i = oCompany.Connect();
                if (i == 0)
                {
                    identity = identity + Servername + ";";
                    identity = identity + Dbuser + ";";
                    identity = identity + Dbpassword + ";";
                    identity = identity + SAPDBName + ";";
                    identity = identity + SAPUser + ";";
                    identity = identity + SAPUserPassword + ";";
                    identity = identity + SAPLicense + ";";

                    companyInstance = oCompany;
                }
                else
                {
                    oCompany.GetLastError(out errCode, out errMessage);
                    throw new Exception("Error Code : " + errCode.ToString() + " | " + "Error Desc : " + errMessage);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Release(string Servername, string Dbuser, string Dbpassword, string SAPDBName, string SAPUser, string SAPUserPassword, string SAPLicense)
        {
            try
            {
                SAPbobsCOM.Company oCompany = new SAPbobsCOM.Company();

                oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2014;
                oCompany.Server = Servername;
                oCompany.DbUserName = Dbuser;
                oCompany.DbPassword = Dbpassword;
                oCompany.CompanyDB = SAPDBName;
                oCompany.UserName = SAPUser;
                oCompany.Password = SAPUserPassword;
                oCompany.LicenseServer = SAPLicense;

                if (oCompany.Connected)
                {
                    oCompany.Disconnect();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}