using ApiEsca.Model;
using ApiEsca.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Http;

namespace ApiEsca.Controllers
{

    public class LoginController : ApiController
    {
        //EXTANCIA DE NUESTRAS CLASES
        ClsModResponse clsModResponse;
        [AllowAnonymous]
        [HttpPost]
        [ActionName("LogUser")]
        public ClsModResponse LogUser([FromBody] users parametros)
        {
            //DECLARAMOS MODELO DE LA BASE DE DATOS 
            examenEntities db = new examenEntities();
            clsModResponse = new ClsModResponse();
            try
            {
                //BUSCAMOS TIEMPO DE EXPIRACION DE LA TOKEN
                var tiempo = ConfigurationManager.AppSettings["Tokenexpired"];
                int timeHoras;
                int.TryParse(tiempo, out timeHoras);
                //BUSCAMOS USUARIO EN LA BASE DE DATOS
                users resultado = db.users.Where(r => r.users1 == parametros.users1 && r.pass == parametros.pass).FirstOrDefault();
                if (resultado == null)
                {
                    clsModResponse.ITEMS = null;
                    clsModResponse.SUCCESS = false;
                    clsModResponse.MESSAGE = "Usuario o contraseña incorrecta.";

                }
                else
                {
                    //GENERAMOS OBJETO DE RETORNO CON EL USUARIO ENCONTRADO Y LA TOKEN ACTIVA
                    var resu = new
                    {
                        userLog = resultado,
                        token = JwtManager.GenerateToken(parametros.users1, parametros.id.ToString(), timeHoras)
                    };
                    clsModResponse.ITEMS = resu;
                    clsModResponse.SUCCESS = true;
                    clsModResponse.MESSAGE = "";
                    //INTRODUCIR CLAVES DE UN CORREO DE PRUEBA Y INTRODUCIR CORREO DESTINO
                    //EnviarCorreoParaAcceso( "SE A LOGEADO CORRECTAMENTE",  HTMLCuerpoCorreo,  HTMLDestinatario,  SMTPUsuario,  SMTPUsuarioContrasenia,  SMTPServidor,  SMTPPuerto);
                }
            }
            catch (Exception ex)
            {
                clsModResponse.ITEMS = null;
                clsModResponse.SUCCESS = false;
                clsModResponse.MESSAGE = "Fallo : " + ex.Message.ToString();
            }

            return clsModResponse;
        }


        public bool EnviarCorreoParaAcceso(string Asunto, string HTMLCuerpoCorreo, string HTMLDestinatario, string SMTPUsuario, string SMTPUsuarioContrasenia, string SMTPServidor, int SMTPPuerto)
        {
            bool enviado = false;
            try
            {
                SmtpClient smtp = new SmtpClient();
                smtp.Host = SMTPServidor;
                smtp.Port = SMTPPuerto;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(SMTPUsuario, SMTPUsuarioContrasenia);
                smtp.EnableSsl = true;

                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                MailMessage mm = new MailMessage();
                mm.IsBodyHtml = true;
                mm.Priority = MailPriority.Normal;
                mm.From = new MailAddress(SMTPUsuario);
                mm.Sender = new MailAddress(HTMLDestinatario);
                mm.Subject = Asunto;
                mm.Body = HTMLCuerpoCorreo;

                smtp.Send(mm); // Enviar el mensaje
                enviado = true;
            }
            catch (Exception ex)
            {

                string fileName = AppDomain.CurrentDomain.BaseDirectory + "/error3.txt";
                StreamWriter sr = new StreamWriter(fileName, true);
                sr.WriteLine(ex.Message + "    SI TRONE EN EL RESPONSE");
                sr.Close();
            }
            return enviado;
        }




    }
}