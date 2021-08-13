using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace modelo.ViewModel
{
    public class PermisoViewModel
    {
        public int Id { get; set; }

        public int IdMenu { get; set; }

        public string NombreMenu { get; set; }
        public string NombreGrupoPerfil { get; set; }
        public int NivelMenu { get; set; }

        public int IdPerfil { get; set; }

        public bool Crear { get; set; }

        public bool Editar { get; set; }

        public bool Eliminar { get; set; }

        public bool Acceder { get; set; }

        public bool TieneCrear { get; set; }

        public bool TieneEditar { get; set; }

        public bool TieneEliminar { get; set; }

        public bool TieneAcceder { get; set; }
    }

}
