using System;
using System.Collections.Generic;
using System.Text;

namespace yari.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public int IdTipoUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string ClaveUsuario { get; set; }
        public int UsuarioCreador { get; set; }
    }
}
