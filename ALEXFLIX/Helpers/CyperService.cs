using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ALEXFLIX.Helpers
{
    public class CyperService
    {

        public static String GetSalt()
        {
            Random random = new Random();
            String salt = "";

            for (int i = 0; i < 50; i++)
            {
                int aleat = random.Next(0, 50);
                char letra = Convert.ToChar(aleat);
                salt += letra;
            }
            return salt;
        }

        public static byte[] CifrarContrasenia(String password, String salt)
        {
            String contenidosalt = password + salt;
            SHA256Managed sha = new SHA256Managed();
            byte[] salida;
            salida = Encoding.UTF8.GetBytes(contenidosalt);
            for (int i = 0; i< 100; i++)
            {
                salida = sha.ComputeHash(salida);
            }
            sha.Clear();
            return salida;
        }
    }
}
