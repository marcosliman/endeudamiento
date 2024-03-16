using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using modelo.Models;
using modelo.Models.Local;
using modelo.ViewModel;
using tesoreria.Helper;
using System.Text;
using System.IO;
using System.Security.Cryptography;
namespace tesoreria.Controllers
{
    public class UsuarioController : Controller
    {
        private ErpContext db = new ErpContext();
        tesoreria.Helper.Seguridad seguridad = System.Web.HttpContext.Current.Session["Seguridad"] as tesoreria.Helper.Seguridad;
        LoginController loginCtrl = new LoginController();
        // GET: Usuario
        public ActionResult Index()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else if (seguridad != null && !seguridad.TienePermiso("Usuarios", Helper.TipoAcceso.Acceder))
            {
                return RedirectToAction("Inicio", "Home");
            }
            else
            {
                return View();
            }
        }
        public JsonResult Usuarios_Read(bool? interno)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "Usuarios" }, Helper.TipoAcceso.Acceder);
            if (acceso.AccesoValido == true)
            {
                var lista = db.Usuario.Select(c =>
                new {
                    c.IdUsuario,
                    c.RutUsuario,
                    c.NombreUsuario,
                    c.ApellidoUsuario,
                    c.CorreoElectronico,
                    c.Activo
                }).ToList();
                return Json(lista, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Create(int? id)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "Usuarios" }, Helper.TipoAcceso.Editar);
            if (acceso.AccesoValido == false)
            {
                return View("~/Views/Login/SinAcceso.cshtml");
            }
            modelo.Models.Local.Usuario registro = new modelo.Models.Local.Usuario();
            registro.Activo = true;
            if (id != null)
            {
                if (id > 0)
                {
                    registro = db.Usuario.Find(id);
                    registro.Clave = "";
                }
            }
            var perfiles = (from p in db.Perfil.ToList()
                            join up in db.UsuarioPerfil.Where(x => x.IdUsuario == id) on p.IdPerfil equals up.IdPerfil into t_up
                            from l_up in t_up.DefaultIfEmpty()
                            select new RetornoGenerico
                            {
                                Id = (int)p.IdPerfil,
                                Nombre = p.NombrePerfil,
                                Seleccionado = (l_up == null) ? false : true,
                                Editable = true
                            }).OrderBy(c => c.Nombre).ToList();

            //SelectList listaperfil = new SelectList(perfiles.OrderBy(c => c.Nombre), "Id", "Nombre");
            ViewData["listaPerfiles"] = perfiles;


            return View(registro);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(modelo.Models.Local.Usuario registro, int[] perfiles)
        {
            var acceso = loginCtrl.ValidaAcceso(new string[] { "Usuarios" }, Helper.TipoAcceso.Acceder);
            var tieneCrear = loginCtrl.ValidaAcceso(new string[] { "Usuarios" }, Helper.TipoAcceso.Crear);
            var tieneEditar = loginCtrl.ValidaAcceso(new string[] { "Usuarios" }, Helper.TipoAcceso.Editar);
            if (acceso.AccesoValido == false)
            {
                return Json(new { acceso.Estado, acceso.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
            }
            dynamic showMessageString = string.Empty;
            var valido = true;
            modelo.Models.Local.Usuario registroEdit = new modelo.Models.Local.Usuario();
            var keybytes = Encoding.UTF8.GetBytes("8080808080808080");
            var iv = Encoding.UTF8.GetBytes("8080808080808080");

            var ClaveConfirmcrypted = Convert.FromBase64String(registro.Clave);
            var ClaveConfirmDecrypt = DecryptStringFromBytes(ClaveConfirmcrypted, keybytes, iv);
            if (registro.IdUsuario > 0)
            {
                if (tieneEditar.AccesoValido == false)
                {
                    return Json(new { tieneEditar.Estado, tieneEditar.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
                }
                showMessageString = new { Estado = 0, Mensaje = "Usuario Actualizado" };
                registroEdit = db.Usuario.Find(registro.IdUsuario);
                if (registroEdit != null)
                {
                    if (ClaveConfirmDecrypt != "" && ClaveConfirmDecrypt != null)
                    {
                        registroEdit.Clave = Crypto.Hash(ClaveConfirmDecrypt);
                    }
                    registroEdit.RutUsuario = registro.RutUsuario;
                    registroEdit.NombreUsuario = registro.NombreUsuario;
                    registroEdit.ApellidoUsuario = registro.ApellidoUsuario;
                    var emailExiste = db.Usuario.Where(c => c.CorreoElectronico.ToUpper() == registro.CorreoElectronico.ToUpper() && c.IdUsuario != registro.IdUsuario).FirstOrDefault();
                    if (emailExiste != null)
                    {
                        showMessageString = new { Estado = 0, Mensaje = "Usuario Actualizado, menos el correo (asociado a otro usuario)" };
                    }
                    else
                    {
                        registroEdit.CorreoElectronico = registro.CorreoElectronico;
                    }
                    registroEdit.Activo = registro.Activo;
                }
            }
            else
            {
                if (tieneCrear.AccesoValido == false)
                {
                    return Json(new { tieneCrear.Estado, tieneCrear.Mensaje, tabla = "" }, JsonRequestBehavior.AllowGet);
                }
                var mensajeRetorno = "";
                var rutExiste = db.Usuario.Where(c => c.RutUsuario.Replace("-", "").Replace(".", "").ToUpper() == registro.RutUsuario.Replace("-", "").Replace(".", "").ToUpper()).FirstOrDefault();
                if (rutExiste != null)
                {
                    mensajeRetorno = "Rut ya existe en el sistema";
                    valido = false;
                }
                var emailExiste = db.Usuario.Where(c => c.CorreoElectronico.ToUpper() == registro.CorreoElectronico.ToUpper()).FirstOrDefault();
                if (emailExiste != null)
                {
                    mensajeRetorno = (mensajeRetorno == "") ? mensajeRetorno : mensajeRetorno + "<br>";
                    mensajeRetorno += "Correo ya existe en el sistema";
                    valido = false;
                }
                if (valido == true)
                {
                    registroEdit = registro;
                    registroEdit.RutUsuario = registro.RutUsuario.ToUpper();
                    registroEdit.Clave = Crypto.Hash(ClaveConfirmDecrypt);
                    registroEdit.FechaRegistro = DateTime.Now;
                    registroEdit.CambiarClave = false;
                    showMessageString = new { Estado = 0, Mensaje = "Usuario Registrado" };
                    db.Usuario.Add(registroEdit);
                }
                else
                {
                    showMessageString = new { Estado = 100, Mensaje = mensajeRetorno };
                }
            }
            db.SaveChanges();
            //eliminar perfiles
            var uPerfil = db.UsuarioPerfil.Where(c => c.IdUsuario == registroEdit.IdUsuario);

            foreach (var c in uPerfil)
            {
                db.UsuarioPerfil.Remove(c);
            }
            db.SaveChanges();
            //Asociar perfiles
            if (perfiles != null)
            {
                foreach (var id in perfiles)
                {
                    UsuarioPerfil newPerfil = new UsuarioPerfil();
                    newPerfil.IdPerfil = id;
                    newPerfil.IdUsuario = registroEdit.IdUsuario;
                    newPerfil.Activo = true;
                    db.UsuarioPerfil.Add(newPerfil);
                }
            }
            db.SaveChanges();
            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SolicitaClave()
        {
            return View();
        }
        public ActionResult SolicitarClave(string CorreoElectronico)
        {
            dynamic showMessageString = string.Empty;

            var usuario = db.Usuario.Where(c => c.CorreoElectronico == CorreoElectronico && c.Activo == true).FirstOrDefault();
            if (usuario != null)
            {
                Random rdn = new Random();
                string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890%$#@";
                int longitud = caracteres.Length;
                char letra;
                int longitudContrasenia = 10;
                string contraseniaAleatoria = string.Empty;
                for (int i = 0; i < longitudContrasenia; i++)
                {
                    letra = caracteres[rdn.Next(longitud)];
                    contraseniaAleatoria += letra.ToString();
                }
                usuario.Clave = Crypto.Hash(contraseniaAleatoria);
                usuario.CambiarClave = true;
                db.SaveChanges();
                showMessageString = new { Estado = 0, Mensaje = "Correcto", ToUrl = "" };
                //Enviar correo
                var mensaje = "";
                HelperFunciones funciones = new HelperFunciones();
                mensaje = "<h3>Estimado(a) " + usuario.NombreUsuario + " " + usuario.ApellidoUsuario + ".</h3><br>";
                mensaje += "Tu contraseña para acceder al sistema de Inmobiliaria ha sido generada con éxito";
                mensaje += "<br>";
                mensaje += "<h3>" + contraseniaAleatoria + "</h3><br>";
                mensaje += "<br><b>Este mail se genera en forma automática, por favor, no responder</b>";
                var resApro = funciones.envioCorreo(mensaje, usuario.CorreoElectronico, "Recuperar Contraseña",null);
            }
            else
            {
                showMessageString = new { Estado = 100, Mensaje = "Correo Ingresado no Existe o se encuentra Inactivo", ToUrl = "" };

            }
            return Json(new { result = showMessageString }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RespuestaClave()
        {
            return View();
        }
        public ActionResult CambiarClave()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
                var usuario = db.Usuario.Find((int)seguridad.IdUsuario);
                return View(usuario);
            }
        }
        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments. 
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            // Declare the string used to hold 
            // the decrypted text. 
            string plaintext = null;

            // Create an RijndaelManaged object 
            // with the specified key and IV. 
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings 
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform. 
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                try
                {
                    // Create the streams used for decryption. 
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {

                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream 
                                // and place them in a string. 
                                plaintext = srDecrypt.ReadToEnd();

                            }

                        }
                    }
                }
                catch
                {
                    plaintext = "keyError";
                }
            }

            return plaintext;
        }
        [HttpPost]
        public ActionResult ActualizarClave(modelo.Models.Local.Usuario registro, string ClaveConfirm)
        {
            dynamic showMessageString = string.Empty;
            var keybytes = Encoding.UTF8.GetBytes("8080808080808080");
            var iv = Encoding.UTF8.GetBytes("8080808080808080");

            var ClaveConfirmcrypted = Convert.FromBase64String(ClaveConfirm);
            HomeController homeCtrl = new HomeController();
            var ClaveConfirmDecrypt = DecryptStringFromBytes(ClaveConfirmcrypted, keybytes, iv);

            var Clavecrypted = Convert.FromBase64String(registro.Clave);
            var ClaveDecrypt = DecryptStringFromBytes(Clavecrypted, keybytes, iv);
            if (ClaveDecrypt.Length < 8 || ClaveConfirmDecrypt.Length < 8)
            {
                showMessageString = new { Estado = 100, Mensaje = "Por favor, no escribas menos de 8 caracteres.", ToUrl = "" };
                return Json(new { result = showMessageString }, JsonRequestBehavior.AllowGet);
            }
            var usuario = db.Usuario.Find(registro.IdUsuario);
            if (registro.Clave == ClaveConfirm)
            {
                //clave actual
                var claveActual = usuario.Clave;
                var nuevaClave = Crypto.Hash(ClaveDecrypt);
                if (claveActual == nuevaClave)
                {
                    showMessageString = new { Estado = 100, Mensaje = "La Nueva Clave debe ser distinta a la Actual", ToUrl = "" };
                    return Json(new { result = showMessageString }, JsonRequestBehavior.AllowGet);
                }
                usuario.Clave = nuevaClave;
                usuario.CambiarClave = false;
                db.SaveChanges();
                showMessageString = new { Estado = 0, Mensaje = "Contraseña actualizada Exitosamente", claveCuenta = registro.Clave };
            }
            else
            {
                showMessageString = new { Estado = 100, Mensaje = "Las contraseñas no coinciden", ToUrl = "" };

            }
            return Json(new { result = showMessageString }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DatosUsuario()
        {
            if (seguridad == null)
            {
                return RedirectToAction("LogOut", "Login");
            }
            else
            {
                var usuario = db.Usuario.Find((int)seguridad.IdUsuario);
                return View(usuario);
            }
        }
        public ActionResult ModificarClave(modelo.Models.Local.Usuario registro, string NuevaClave,string ClaveConfirm)
        {
            dynamic showMessageString = string.Empty;
            var keybytes = Encoding.UTF8.GetBytes("8080808080808080");
            var iv = Encoding.UTF8.GetBytes("8080808080808080");

            var ClaveConfirmcrypted = Convert.FromBase64String(ClaveConfirm);
            HomeController homeCtrl = new HomeController();
            var ClaveConfirmDecrypt = DecryptStringFromBytes(ClaveConfirmcrypted, keybytes, iv);

            var Clavecrypted = Convert.FromBase64String(registro.Clave);
            var ClaveDecrypt = DecryptStringFromBytes(Clavecrypted, keybytes, iv);

            var NuevaClavecrypted = Convert.FromBase64String(NuevaClave);
            var NuevaClaveDecrypt = DecryptStringFromBytes(NuevaClavecrypted, keybytes, iv);


            if (NuevaClaveDecrypt.Length < 8 || ClaveConfirmDecrypt.Length < 8)
            {
                showMessageString = new { Estado = 100, Mensaje = "Por favor, no escribas menos de 8 caracteres.", ToUrl = "" };
                return Json(new { result = showMessageString }, JsonRequestBehavior.AllowGet);
            }
            var usuario = db.Usuario.Find(registro.IdUsuario);
            var claveActual = usuario.Clave;
            var ClaveInputHash = Crypto.Hash(ClaveDecrypt);
            if (claveActual == ClaveInputHash)
            {
                if (NuevaClave == ClaveConfirm)
                {
                    //clave actual                    
                    var nuevaClave = Crypto.Hash(NuevaClaveDecrypt);
                    if (claveActual == nuevaClave)
                    {
                        showMessageString = new { Estado = 100, Mensaje = "La Nueva Clave debe ser distinta a la Actual", ToUrl = "" };
                        return Json(showMessageString, JsonRequestBehavior.AllowGet);
                    }
                    usuario.Clave = nuevaClave;
                    db.SaveChanges();
                    showMessageString = new { Estado = 0, Mensaje = "Contraseña actualizada Exitosamente", claveCuenta = registro.Clave };
                }
                else
                {
                    showMessageString = new { Estado = 100, Mensaje = "Las contraseñas no coinciden", ToUrl = "" };

                }
            }
            else
            {
                showMessageString = new { Estado = 100, Mensaje = "La Contraseña Actual es Incorrecta", ToUrl = "" };

            }

            return Json(showMessageString, JsonRequestBehavior.AllowGet);
        }
    }
}