using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;


namespace ALEXFLIX.Helpers
{
    public class MailService
    {
        IConfiguration Configuration;
        public MailService(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        private MailMessage ConfigureMail(String receptor, String asunto, String mensaje)
        {
            MailMessage mail = new MailMessage();
            String username = this.Configuration["usuariomail"];

            mail.From = new MailAddress(username);
            mail.To.Add(new MailAddress(receptor));
            mail.Subject = asunto;
            mail.Body = mensaje;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.Normal;

            return mail;
        }
        private void ConfigureSmtp(MailMessage mail)
        {
            String username = this.Configuration["usuariomail"];
            String password = this.Configuration["passwordmail"];
            String smtpserver = this.Configuration["host"];
            int port = int.Parse(this.Configuration["port"]);
            bool ssl = bool.Parse(this.Configuration["ssl"]);
            bool defaultcredentias = bool.Parse(this.Configuration["defaultcredentials"]);

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = smtpserver;
            smtpClient.Port = port;
            smtpClient.EnableSsl = ssl;
            smtpClient.UseDefaultCredentials = defaultcredentias;

            NetworkCredential usercredential = new NetworkCredential(username, password);

            smtpClient.Credentials = usercredential;
            smtpClient.Send(mail);
        }

        public void SendMail(String receptor, String asunto, String mensaje)
        {
            MailMessage mail = this.ConfigureMail(receptor, asunto, mensaje);
            this.ConfigureSmtp(mail);
        }

        public void SendMail(String receptor, String asunto, String mensaje, String filepath)
        {
            MailMessage mail = this.ConfigureMail(receptor, asunto, mensaje);

            Attachment attachment = new Attachment(filepath);
            mail.Attachments.Add(attachment);

            this.ConfigureSmtp(mail);
        }

        public void EnviarCorreoValidacion(String correo, String numSecret, String username)
        {
            String mensaje = @"<html>
                      <body>
                      <h3 style='color:red'>PRECAUCIÓN. SE ESTA INTENTADO CAMBIAR SU CONTRASEÑA</h3>
                      <p style='color:red'>Si usted no esta autorizando este cambio ignore este mensaje</p>
                      <hr />
                      <h3>Buenos días, " + username + @"</h3>
                      <p>Le enviamos el codigo secreto para completar la validacion de cambio de contraseña</p>
                        <p>CODIGO SECRETO: <br/><span style='font-weight:bold'>" + numSecret + @"</span></p>
                      <p>Un saludo,<br>-ALEXFLIX</br></p>
                      </body>
                      </html>
                     ";
            this.SendMail(correo, "NUMERO SECRETO", mensaje);
        }

        public void EnviarCorreoBienvenida(String correo, String username)
        {
            String mensaje = @"<html>
                      <body>
                      <h3>Buenos días, " + username + @"</h3>
                      <p>Muchas gracias por registrarse a ALEXFLIX, donde podras encontrar una infinidad de titulos y generos, 
                      ahora solo queda disfrutar ;)</p>
                      <p>Un saludo,<br>-ALEXFLIX</br></p>
                      </body>
                      </html>
                     ";
            this.SendMail(correo, "BIENVENIDO A ALEXFLIX", mensaje);
        }
    }
}