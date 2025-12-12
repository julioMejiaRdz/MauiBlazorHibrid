namespace MauiBlazorHibrid.Models
{
    public class VentasDatos
    {
        public int ITEM { get; set; }
        public decimal VENTA { get; set; }
        public decimal COSTO { get; set; }
        public decimal CANTIDADES { get; set; }
        public decimal UTILIDAD { get; set; }
        public DateTime FECHA { get; set; }
        public string? USUARIO { get; set; }
        public string? DEPARTAMENTO { get; set; }
        public string? FABRICANTES { get; set; }
        public string? REPRESENTANTE { get; set; }
        public string? CLIENTE { get; set; }
        public string? PRODFAMILIA { get; set; }
        public string? CODIGO { get; set; }
        public string? DESCRIPCION { get; set; }
        public string? DOCNUM { get; set; }
        public string? CATEGORIA { get; set; }
        public string? RUBRO { get; set; }
        public string? SECTOR_CLIENTE { get; set; }
    }

    // Modelo simplificado para el gráfico
    public class VentasPorFecha
    {
        public string Fecha { get; set; } = string.Empty;
        public decimal TotalVentas { get; set; }
        public decimal TotalCostos { get; set; }
        public decimal TotalUtilidad { get; set; }
        public decimal TotalCantidades { get; set; }
    }
}
