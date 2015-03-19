using System;
using System.Collections.Generic;
using System.Text;

namespace yari.Models
{
    public class HistoricoJuego
    {
        public int IdHistoricoJuego { get; set; }
        public int IdUsuario { get; set; }
        public int IdNivelJuego { get; set; }
        public float PuntuacionTotal { get; set; }
        public string Fecha { get; set; }
    }
}
