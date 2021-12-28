﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace modelo.ViewModel
{
    public class ActivoViewModel
    {
        public int IdActivo { get; set; }
        public int? IdLicitacionActivo { get; set; }
        public int? IdContratoActivo { get; set; }
        public int? IdEmpresa { get; set; }
        public string RazonSocial { get; set; }
        public int? NumeroInterno { get; set; }
        public string CodSoftland { get; set; }
        public string Familia { get; set; }
        public string Descripcion { get; set; }
        public string Capacidad { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int? Anio { get; set; }
        public int? IdCuenta { get; set; }
        public string NombreCuenta { get; set; }
        public double? Valor { get; set; }
        public int? IdProveedor { get; set; }
        public string NombreProveedor { get; set; }
        public string NumeroFactura { get; set; }
        public string MotorChasis { get; set; }
        public string Serie { get; set; }
        public string Patente { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string TituloBoton { get; set; }
        public bool Editable { get; set; }
    }

}
