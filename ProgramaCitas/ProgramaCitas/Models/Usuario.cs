namespace ProgramaCitas.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Correo { get; set; }
        public string Clave { get; set; }

        public string ConfirmarClave { get; set; }
    }
}
