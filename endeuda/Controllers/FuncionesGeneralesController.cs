using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using modelo.ViewModel;
namespace tesoreria.Controllers
{
    public class FuncionesGeneralesController : Controller
    {
        // GET: FuncionesGenerales
        public ActionResult Index()
        {
            return View();
        }
        /*funcion para validar el dato ingresado, aquí se evita la inyeccion de codigo*/
        public string ValidaStr(string strCadena)
        {
            if (strCadena != null)
            {
                strCadena = strCadena.Replace("<SCRIPT>", "SCRIPT");
                strCadena = strCadena.Replace("</SCRIPT>", "SCRIPT");
                strCadena = strCadena.Replace("~", "-");
                strCadena = strCadena.Replace("'", "''");
                strCadena = strCadena.Replace("<", "-");
                strCadena = strCadena.Replace(">", "-");
                strCadena = strCadena.Trim();
            }
            return strCadena;
        }

        /*funcion para validar que fecha de termino no sobrepase la fecha de inicio*/
        public bool EsInvalidalaHora(TimeSpan newInicio, TimeSpan newFin)
        {

            DateTime NewInicio = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + newInicio);
            DateTime NewFin = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + newFin);
            if (NewInicio > NewFin)
            {
                return true;

            }
            return false;
        }

        public bool EsRutEmpresa(string rut)
        {
            var rutSplit = rut.Split('-');
            var rutSinGuion = rutSplit[0].Replace(".", "");
            decimal rutDecimal = 0;
            if (rutSinGuion != "")
            {
                rutDecimal = Convert.ToDecimal(rutSinGuion);
            }
            bool EsEmpresa;
            EsEmpresa = false;
            if (rutDecimal >= 50000000)
            {
                EsEmpresa = true;
            }
            return EsEmpresa;
        }
    }

}