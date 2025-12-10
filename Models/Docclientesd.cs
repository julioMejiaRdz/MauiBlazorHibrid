using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauiBlazorHibrid.Models
{
    [Table("docclientesd")]
    public class Docclientesd
    {
        [Column("pludocclientes")]
        public int Pludocclientes { get; set; }

        [Column("item")]
        public int Item { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("EXENTO")]
        public decimal? Exento { get; set; }

        [Column("lista")]
        public decimal? Lista { get; set; }

        [Column("unitario")]
        public decimal? Unitario { get; set; }

        [Column("pluempresas")]
        public int Pluempresas { get; set; }

        [Column("pluproducto")]
        public int Pluproducto { get; set; }

        [Column("tipoprecio")]
        [MaxLength(15)]
        public string? Tipoprecio { get; set; }

        [Column("BC")]
        public int? Bc { get; set; }

        [Column("DESCRIPCION1")]
        [MaxLength(250)]
        public string? Descripcion1 { get; set; }

        [Column("descripcion2")]
        [MaxLength(250)]
        public string? Descripcion2 { get; set; }

        [Column("NOTA")]
        public string? Nota { get; set; }

        [Column("descuento")]
        public decimal? Descuento { get; set; }

        [Column("CODIGO")]
        [MaxLength(50)]
        public string? Codigo { get; set; }

        [Column("costopromedio")]
        public decimal? Costopromedio { get; set; }

        [Column("UNITARIO2")]
        public decimal? Unitario2 { get; set; }

        [Column("procesado")]
        [MaxLength(1)]
        public string? Procesado { get; set; }

        [Column("pluacumulador")]
        public int? Pluacumulador { get; set; }

        [Column("EN_OFERTA")]
        [MaxLength(1)]
        public string? EnOferta { get; set; }

        [Column("horas")]
        public decimal? Horas { get; set; }

        [Column("ORIGEN_NR")]
        public int? OrigenNr { get; set; }

        [Column("plunr")]
        public int? Plunr { get; set; }

        [Column("item_nr")]
        public int? ItemNr { get; set; }

        [Column("presupuestado")]
        public bool? Presupuestado { get; set; }
    }
}
