using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace modelo.Models.Softland
{
    [Table("cwtauxi", Schema = "softland")]
    public class cwtauxi
    {
        [Key]
        public string CodAux { get; set; }
        public string NomAux { get; set; }
        public string NoFAux { get; set; }
        public string RutAux { get; set; }
        public string ActAux { get; set; }
        public string GirAux { get; set; }
        public string ComAux { get; set; }
        public string PaiAux { get; set; }
        public string DirAux { get; set; }
        public string ClaCli { get; set; }
        public string ClaPro { get; set; }
        public string EMail { get; set; }
        public string WebSite { get; set; }
        public string eMailDTE { get; set; }
    }
}
