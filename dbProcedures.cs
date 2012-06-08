/*
Database procedures class (MVC framework)
Distributed freely under the MIT license (and if we meet someday the phk@freebsd.org license might apply)

Copyright © 2012 Reagan Williams (reagan.williams@gmail.com)

Permission is hereby granted, free of charge, to any person obtaining a copy of this
software and associated documentation files (the “Software”), to deal in the Software
without restriction, including without limitation the rights to use, copy, modify, merge,
publish, distribute, sublicense, and/or sell copies of the Software, and to permit
persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies
or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;

/// <summary>
/// Database procedures class to store methods of interfacing with the database.
/// 
/// Usage: create a new method for each interaction your app needs with the database and use the dbError helper method to report issues.
/// 
/// TODO: Refactor code to not have to write a new method for every database call
/// TODO: Improve error handling, reporting
/// TODO: Encrypt email password below
/// 
/// </summary>
public class dbProcedures
{
    public db sql;

    string LogFile = System.Configuration.ConfigurationManager.AppSettings.Get("LogFile");

    public dbProcedures()
    {
        sql = new db();
    }

    public void Close()
    {
        sql.Shutdown();
    }

    public string dbError(string strMessage, string strSubject)
    {
        // Send error email
        string applicationName = System.Configuration.ConfigurationManager.AppSettings.Get("applicationName");
        string smtpServer = System.Configuration.ConfigurationManager.AppSettings.Get("smtpServer");
        string errorEmail = System.Configuration.ConfigurationManager.AppSettings.Get("errorEmail");
        string systemEmail = System.Configuration.ConfigurationManager.AppSettings.Get("systemEmail");

        // This should really be encrypted, I know....
        string systemEmailPassword = "opentext123!";

        string strResponse = "";

        strMessage = strSubject + "\r\n" + strMessage;
        strSubject = "Database Error (" + applicationName + ")" + " " + strSubject;

        System.Net.Mail.MailAddress systemAddress = new System.Net.Mail.MailAddress(systemEmail, "Trendworks Support");
        System.Net.Mail.MailAddress recepientAddress = new System.Net.Mail.MailAddress(errorEmail);


        System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage(systemAddress, recepientAddress);
        msg.Subject = strSubject;
        msg.Body = strMessage;

        System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(smtpServer);
        System.Net.NetworkCredential authInfo = new System.Net.NetworkCredential(systemEmail, systemEmailPassword);

        smtp.EnableSsl = true;
        smtp.Port = 587;
        smtp.Host = smtpServer;
        smtp.UseDefaultCredentials = false;
        smtp.Credentials = authInfo;

        using (System.IO.StreamWriter file = new System.IO.StreamWriter(LogFile, true))
        {
            file.WriteLine("[" + DateTime.Now.ToString() + "] Error: " + msg.Body);
        }

        try
        {
            smtp.Send(msg);
        }
        catch (Exception e)
        {
            strResponse = e.Message;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(LogFile, true))
            {
                file.WriteLine("[" + DateTime.Now.ToString() + "] Exception: " + e.Message);
            }
        }

        return strResponse;
    }


    public bool AddNewEOB(string strUsername, string strSourceId, string strFilename)
    {

        try
        {
            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("[usp_AddNewEOB]", sql.SqlConnection);

            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Username", strUsername));
            sqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SourceId", strSourceId));
            sqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Filename", strFilename));

            sqlCommand.ExecuteNonQuery();

        }
        catch (Exception ex)
        {
            dbError(ex.StackTrace, ex.Message);
            return false;
        }

        return true;
    }

    public bool AddNewEOB(string strUsername, string strSourceId, string strFilename, string strCarrier)
    {

        try
        {
            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("[usp_AddNewEOB]", sql.SqlConnection);

            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Username", strUsername));
            sqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SourceId", strSourceId));
            sqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Filename", strFilename));
            sqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Carrier", strCarrier));

            sqlCommand.ExecuteNonQuery();

        }
        catch (Exception ex)
        {
            dbError(ex.StackTrace, ex.Message);
            return false;
        }

        return true;
    }


    public bool MemberInvalidCredentials(string strUsername)
    {

        try
        {
            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("[usp_MemberInvalidCredentials]", sql.SqlConnection);

            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Username", strUsername));

            sqlCommand.ExecuteNonQuery();

        }
        catch (Exception ex)
        {
            dbError(ex.StackTrace, ex.Message);
            return false;
        }

        return true;
    }

    public bool MemberInvalidCredentials(string strUsername, string strCarrier)
    {

        try
        {
            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("[usp_MemberInvalidCredentials]", sql.SqlConnection);

            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Username", strUsername));
            sqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Carrier", strCarrier));

            sqlCommand.ExecuteNonQuery();

        }
        catch (Exception ex)
        {
            dbError(ex.StackTrace, ex.Message);
            return false;
        }

        return true;
    }



    public bool LogAction(string strUsername, string strLogType, string strLogMessage)
    {
        try
        {
            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("[usp_AddSystemLog]", sql.SqlConnection);

            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Username", strUsername));
            sqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@LogType", strLogType));
            sqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@LogMessage", strLogMessage));

            sqlCommand.ExecuteNonQuery();

        }
        catch (Exception ex)
        {
            dbError(ex.StackTrace, ex.Message);
            return false;
        }

        return true;
    }


    public void UpdateMemberProcessedDate(string strUsername)
    {
        try
        {
            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("[usp_UpdateMemberProcessedDate]", sql.SqlConnection);

            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Username", strUsername));

            sqlCommand.ExecuteNonQuery();

        }
        catch (Exception ex)
        {
            dbError(ex.StackTrace, ex.Message);
        }
    }

    public void UpdateMemberProcessedDate(string strUsername, string strCarrier)
    {
        try
        {
            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("[usp_UpdateMemberProcessedDate]", sql.SqlConnection);

            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Username", strUsername));
            sqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Carrier", strCarrier));

            sqlCommand.ExecuteNonQuery();

        }
        catch (Exception ex)
        {
            dbError(ex.StackTrace, ex.Message);
        }
    }

    public System.Collections.Hashtable GetMemberList()
    {
        Hashtable ht = new Hashtable();

        try
        {
            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("[usp_GetMemberList]", sql.SqlConnection);
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

            System.Data.SqlClient.SqlDataReader reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                if ((!object.ReferenceEquals(reader[0], System.DBNull.Value)))
                {
                    ht.Add(reader[1], reader[2]);
                }
            }

            reader.Close();

        }
        catch (Exception ex)
        {
            dbError(ex.StackTrace, ex.Message);
            return null;
        }

        return ht;
    }

    public System.Collections.Hashtable GetMemberList(String carrier)
    {
        Hashtable ht = new Hashtable();

        try
        {
            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("[usp_GetMemberList]", sql.SqlConnection);
            sqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Carrier", carrier));
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

            System.Data.SqlClient.SqlDataReader reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                if ((!object.ReferenceEquals(reader[0], System.DBNull.Value)))
                {
                    ht.Add(reader[1], reader[2]);
                }
            }

            reader.Close();

        }
        catch (Exception ex)
        {
            dbError(ex.StackTrace, ex.Message);
            return null;
        }

        return ht;
    }

    public bool CheckStartupSchedule()
    {
        bool ReturnVar = false;
        System.Data.SqlClient.SqlDataReader reader = null;

        try
        {
            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("[usp_CheckStartupSchedule]", sql.SqlConnection);
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

            reader = sqlCommand.ExecuteReader();

            if (reader.Read())
            {
                if ((!object.ReferenceEquals(reader[0], System.DBNull.Value)))
                {
                    if (reader[0].ToString() == "1")
                        ReturnVar = true;
                }
            }

            reader.Close();
        }
        catch (Exception ex)
        {
            dbError(ex.StackTrace, ex.Message);
        }

        return ReturnVar;
    }

    public int StartNewBatch()
    {
        int ReturnVar = 0;
        System.Data.SqlClient.SqlDataReader reader = null;
        try
        {
            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("[usp_StartNewBatch]", sql.SqlConnection);
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

            reader = sqlCommand.ExecuteReader();

            if (reader.Read())
            {
                if ((!object.ReferenceEquals(reader[0], System.DBNull.Value)))
                {
                    ReturnVar = Convert.ToInt32(reader[0].ToString());
                }
            }

            reader.Close();
        }
        catch (Exception ex)
        {
            dbError(ex.StackTrace, ex.Message);
        }

        return ReturnVar;
    }

    public bool CloseBatch(int BatchId, int MemberProcessedCount, int EOBDownloadCount, int ErrorCount)
    {
        try
        {
            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("[usp_CloseBatch]", sql.SqlConnection);

            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@BatchId", BatchId));
            sqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@MemberProcessedCount", MemberProcessedCount));
            sqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@EOBDownloadCount", EOBDownloadCount));
            sqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@TotalErrorCount", ErrorCount));

            sqlCommand.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            dbError(ex.StackTrace, ex.Message);
            return false;
        }

        return true;
    }

    public bool VerifyEOBStatus(string strUsername, string strSourceId)
    {
        bool ReturnVar = false;
        System.Data.SqlClient.SqlDataReader reader = null;
        try
        {
            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("[usp_VerifyEOBStatus]", sql.SqlConnection);
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Username", strUsername));
            sqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SourceId", strSourceId));

            reader = sqlCommand.ExecuteReader();

            if (reader.Read())
            {
                if ((!object.ReferenceEquals(reader[0], System.DBNull.Value)))
                {
                    if (reader[0].ToString() == "1")
                        ReturnVar = true;
                }
            }
        }
        catch (Exception ex)
        {
            if (!reader.IsClosed)
                reader.Close();

            dbError(ex.StackTrace, ex.Message);
        }

        if (!reader.IsClosed)
            reader.Close();

        return ReturnVar;
    }


    public bool VerifyEOBStatus(string strUsername, string strSourceId, string strCarrier)
    {
        bool ReturnVar = false;
        System.Data.SqlClient.SqlDataReader reader = null;
        try
        {
            System.Data.SqlClient.SqlCommand sqlCommand = new System.Data.SqlClient.SqlCommand("[usp_VerifyEOBStatus]", sql.SqlConnection);
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Username", strUsername));
            sqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@SourceId", strSourceId));
            sqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Carrier", strCarrier));

            reader = sqlCommand.ExecuteReader();

            if (reader.Read())
            {
                if ((!object.ReferenceEquals(reader[0], System.DBNull.Value)))
                {
                    if (reader[0].ToString() == "1")
                        ReturnVar = true;
                }
            }
        }
        catch (Exception ex)
        {
            if (!reader.IsClosed)
                reader.Close();

            dbError(ex.StackTrace, ex.Message);
        }

        if (!reader.IsClosed)
            reader.Close();

        return ReturnVar;
    }


}//End of dbProcedures Class
