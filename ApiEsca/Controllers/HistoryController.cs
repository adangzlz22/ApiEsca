using ApiEsca.Model;
using ApiEsca.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ApiEsca.Controllers
{
    [JwtAuthentication]
    public class HistoryController : ApiController
    {

        ClsModResponse clsModResponse;
        [HttpPost]
        [ActionName("CreateHistory")]
        public ClsModResponse CreateHistory([FromBody] searchs parametros)
        {
            examenEntities db = new examenEntities();
            clsModResponse = new ClsModResponse();
            try
            {
                parametros.date = DateTime.Now;
                db.searchs.Add(parametros);
                db.SaveChanges();

                clsModResponse.ITEMS = parametros;
                clsModResponse.SUCCESS = true;
                clsModResponse.MESSAGE = "";
            }
            catch (Exception ex)
            {
                clsModResponse.ITEMS = null;
                clsModResponse.SUCCESS = false;
                clsModResponse.MESSAGE = "Fallo : " + ex.Message.ToString();
            }

            return clsModResponse;
        }
    }
}