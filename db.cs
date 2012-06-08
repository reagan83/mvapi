/*
Database connector class for SQL Server 2005-2012.
Distributed freely under the MIT license (and if we meet someday the phk@freebsd.org license might apply)

Copyright © 2010 Reagan Williams (reagan.williams@gmail.com)

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

public class db
{
    private System.Configuration.ConnectionStringSettings _settings;
    private System.Data.SqlClient.SqlConnection _sqlConnection;

    public db()
    {
        _settings = System.Configuration.ConfigurationManager.ConnectionStrings["Database"];

        try
        {
            _sqlConnection = new System.Data.SqlClient.SqlConnection();
            _sqlConnection.ConnectionString = _settings.ToString();
            _sqlConnection.Open();
        }
        catch (Exception msg)
        {
        }
    }

    public System.Data.SqlClient.SqlConnection SqlConnection
    {
        get { return _sqlConnection; }
    }

    public void Open()
    {
        if (_sqlConnection.State != System.Data.ConnectionState.Open)
        {
            try
            {
                _sqlConnection.Open();
            }
            catch (Exception ex)
            {
            }
        }
    }

    public void Shutdown()
    {
        if (_sqlConnection.State == System.Data.ConnectionState.Open)
        {
            try
            {
                _sqlConnection.Close();
            }
            catch (Exception ex)
            {
            }
        }
    }
}

