//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sac.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Chamado
    {
        public int IdChamado { get; set; }
        public System.DateTime DataAbertura { get; set; }
        public string Descricao { get; set; }
        public int IdCliente { get; set; }
        public string Estado { get; set; }
        public string Urgencia { get; set; }
        public Nullable<int> IdAtendente { get; set; }
        public string Solucao { get; set; }
    
        public virtual Usuario Usuario { get; set; }
        public virtual Usuario Usuario1 { get; set; }
    }
}