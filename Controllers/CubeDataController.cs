using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace mvapi.Controllers
{
    public class CubeDataController : ApiController
    {
        dbProcedures db = new dbProcedures();

        public int Get()
        {
            return 69;
        }

    }
}
