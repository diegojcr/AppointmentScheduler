using Microsoft.AspNetCore.Mvc;
using ProgramaCitas.Models;
using System.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace ProgramaCitas.Controllers
{
    public class AccesoController : Controller
    {

        static string cadena = "Data Source=DESKTOP-E707APC\\SQLEXPRESS01;Initial Catalog=ProgramadorCitas;Integrated Security=True";

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Registrar()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Registrar(Usuario oUsuario)
        {
            bool registrado;
            string mensaje;

            if(oUsuario.Clave == oUsuario.ConfirmarClave)
            {
                oUsuario.Clave = ConvertirSha256(oUsuario.Clave);
            }
            else
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_RegistrarUsuario", cn);
                cmd.Parameters.AddWithValue("Correo", oUsuario.Correo);
                cmd.Parameters.AddWithValue("Clave", oUsuario.Clave);
                cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                cmd.ExecuteNonQuery();

                registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                mensaje = cmd.Parameters["Mensaje"].Value.ToString();
            }

            ViewData["Mensaje"] = mensaje;

            if (registrado)
            {
                return RedirectToAction("Login", "Acceso");
            }
            else
            {
                return View();
            }

        }
        [HttpPost]
        public IActionResult Login(Usuario oUsuario)
        {
            oUsuario.Clave = ConvertirSha256(oUsuario.Clave);

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_ValidarUsuario", cn);
                cmd.Parameters.AddWithValue("Correo", oUsuario.Correo);
                cmd.Parameters.AddWithValue("Clave", oUsuario.Clave);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                oUsuario.IdUsuario = Convert.ToInt32(cmd.ExecuteScalar().ToString());

            }
            if(oUsuario.IdUsuario != 0)
            {
                HttpContext.Session.SetString("Usuario", oUsuario.Correo);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["Mensaje"] = "Usuario no encontrado";
                return View();
            }

            
        }


        public static string ConvertirSha256(string texto)
        {
            StringBuilder sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach(byte b in result)
                {
                    sb.Append(b.ToString("x2"));
                }
            }
            return sb.ToString();
        }

    }
}
