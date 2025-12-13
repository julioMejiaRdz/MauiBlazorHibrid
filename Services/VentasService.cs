using MauiBlazorHibrid.Data;
using MauiBlazorHibrid.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MauiBlazorHibrid.Services
{
    public interface IVentasService
    {
        Task<List<VentasPorFecha>> ObtenerVentasPorFechaAsync(DateTime fechaInicio, DateTime fechaFin, string agrupacion = "mes");
        Task<List<VentasDatos>> ObtenerVentasDetalladasAsync(DateTime fechaInicio, DateTime fechaFin);
    }

    public class VentasService : IVentasService
    {
        private readonly AppDbContext _context;

        public VentasService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<VentasPorFecha>> ObtenerVentasPorFechaAsync(DateTime fechaInicio, DateTime fechaFin, string agrupacion = "mes")
        {
            var resultado = new List<VentasPorFecha>();
            var connectionString = _context.Database.GetConnectionString();

            // Cambiar agrupación según el parámetro
            string groupBy = agrupacion.ToLower() switch
            {
                "dia" => "M.FECHA",
                "semana" => "YEAR(M.FECHA), DATEPART(WEEK, M.FECHA)",
                "mes" => "YEAR(M.FECHA), MONTH(M.FECHA)",
                "año" => "YEAR(M.FECHA)",
                _ => "YEAR(M.FECHA), MONTH(M.FECHA)"
            };

            string selectFecha = agrupacion.ToLower() switch
            {
                "dia" => "M.FECHA AS Periodo",
                "semana" => "DATEADD(week, DATEDIFF(week, 0, M.FECHA), 0) AS Periodo",
                "mes" => "DATEFROMPARTS(YEAR(M.FECHA), MONTH(M.FECHA), 1) AS Periodo",
                "año" => "DATEFROMPARTS(YEAR(M.FECHA), 1, 1) AS Periodo",
                _ => "DATEFROMPARTS(YEAR(M.FECHA), MONTH(M.FECHA), 1) AS Periodo"
            };

            var sql = $@"
                SELECT
                    {selectFecha},
                    SUM((D.CANTIDAD + D.ORIGEN_NR) * D.UNITARIO * T.FACTOR) AS TotalVentas,
                    SUM((D.CANTIDAD + D.ORIGEN_NR) * D.COSTOPROMEDIO * T.FACTOR) AS TotalCostos,
                    SUM((D.CANTIDAD + D.ORIGEN_NR) * (D.UNITARIO - D.COSTOPROMEDIO) * T.FACTOR) AS TotalUtilidad,
                    SUM((D.CANTIDAD + D.ORIGEN_NR) * T.FACTOR) AS TotalCantidades
                FROM DOCCLIENTESD D
                INNER JOIN DOCCLIENTESM M ON D.PLUDOCCLIENTES = M.PLUDOCCLIENTES
                INNER JOIN TIPODOCFACT T ON M.TIPO = T.CODIGO
                WHERE
                    M.PLUEMPRESAS = 1
                    AND M.FECHA >= @fechaInicio
                    AND M.FECHA <= @fechaFin
                    AND M.TIPO IN ('CF','FC','FX','NC')
                    AND M.POST IN ('T','P')
                GROUP BY {groupBy}
                ORDER BY Periodo
            ";

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@fechaInicio", SqlDbType.DateTime) { Value = fechaInicio });
                    command.Parameters.Add(new SqlParameter("@fechaFin", SqlDbType.DateTime) { Value = fechaFin });

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var periodo = (DateTime)reader["Periodo"];
                            string fechaFormato = agrupacion.ToLower() switch
                            {
                                "dia" => periodo.ToString("dd/MMM"),
                                "semana" => periodo.ToString("dd/MMM"),
                                "mes" => periodo.ToString("MMM yyyy"),
                                "año" => periodo.ToString("yyyy"),
                                _ => periodo.ToString("MMM yyyy")
                            };

                            resultado.Add(new VentasPorFecha
                            {
                                Fecha = fechaFormato,
                                TotalVentas = reader["TotalVentas"] != DBNull.Value ? Convert.ToDecimal(reader["TotalVentas"]) : 0,
                                TotalCostos = reader["TotalCostos"] != DBNull.Value ? Convert.ToDecimal(reader["TotalCostos"]) : 0,
                                TotalUtilidad = reader["TotalUtilidad"] != DBNull.Value ? Convert.ToDecimal(reader["TotalUtilidad"]) : 0,
                                TotalCantidades = reader["TotalCantidades"] != DBNull.Value ? Convert.ToDecimal(reader["TotalCantidades"]) : 0
                            });
                        }
                    }
                }
            }

            return resultado;
        }

        public async Task<List<VentasDatos>> ObtenerVentasDetalladasAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var sql = @"
                SELECT
                    D.ITEM,
                    (D.CANTIDAD + D.ORIGEN_NR) * D.UNITARIO * T.FACTOR AS VENTA,
                    (D.CANTIDAD + D.ORIGEN_NR) * D.COSTOPROMEDIO * T.FACTOR AS COSTO,
                    (D.CANTIDAD + D.ORIGEN_NR) * T.FACTOR AS CANTIDADES,
                    (D.CANTIDAD + D.ORIGEN_NR) * (D.UNITARIO - D.COSTOPROMEDIO) * T.FACTOR AS UTILIDAD,
                    M.FECHA,
                    VEND.USUARIO,
                    DEP.NOMBRE AS DEPARTAMENTO,
                    F.NOMBRE AS FABRICANTES,
                    (REP.NOMBRE + ' ' + REP.APELLIDO) AS REPRESENTANTE,
                    C.NOMBRE AS CLIENTE,
                    S.NOMBRE AS PRODFAMILIA,
                    P.CODIGO,
                    P.DESCRIPCION,
                    M.CODIGO AS DOCNUM,
                    CAT.NOMBRE AS CATEGORIA,
                    RUB.NOMBRE AS RUBRO,
                    SEC.trial_NOMBRE_2 AS SECTOR_CLIENTE
                FROM DOCCLIENTESD D
                INNER JOIN DOCCLIENTESM M ON D.PLUDOCCLIENTES = M.PLUDOCCLIENTES
                INNER JOIN TIPODOCFACT T ON M.TIPO = T.CODIGO
                INNER JOIN EMPLEADOS VEND ON M.PLUEMPLEADOS = VEND.PLUEMPLEADOS
                INNER JOIN PRODUCTO P ON D.PLUPRODUCTO = P.PLUPRODUCTO
                INNER JOIN FABRICANTE F ON P.PLUFABRICANTE = F.PLUFABRICANTE
                INNER JOIN CLIENTES C ON M.PLUCLIENTE = C.PLUCLIENTE
                INNER JOIN SUBCATEGORIA S ON P.PLUSUBCATEGORIA = S.PLUSUBCATEGORIA
                INNER JOIN CLCATEGORIA CAT ON C.PLUCLCATEGORIA = CAT.PLUCLCATEGORIA
                INNER JOIN CLRUBRO RUB ON C.PLUCLRUBRO = RUB.PLUCLRUBRO
                LEFT JOIN CLSECTOR SEC ON C.PLUCLSECTOR = SEC.trial_PLUCLSECTOR_1
                LEFT JOIN GERENTE_LINEASFAB GLINEA
                    ON F.PLUFABRICANTE = GLINEA.PLUFABRICANTE AND GLINEA.COMISIONGL > 0
                LEFT JOIN EMPLEADOS EMP
                    ON GLINEA.PLUEMPLEADO = EMP.PLUEMPLEADOS AND EMP.ACTIVO = 'T'
                LEFT JOIN RH_EMPLEADOS REP
                    ON EMP.PLUEMPLEADOS = REP.USUARIOID AND REP.ESTADO = 'A'
                LEFT JOIN RH_EMPLEADOS RH ON M.PLUEMPLEADOS = RH.USUARIOID
                LEFT JOIN RH_PUESTOS PU ON RH.PUESTOID = PU.PUESTOID
                LEFT JOIN RH_DEPARTAMENTOS DEP ON PU.DEPTOID = DEP.DEPTOID
                WHERE
                    M.PLUEMPRESAS = 1
                    AND M.FECHA >= @fechaInicio
                    AND M.FECHA <= @fechaFin
                    AND M.TIPO IN ('CF','FC','FX','NC')
                    AND M.POST IN ('T','P')
                ORDER BY M.FECHA
            ";

            var resultado = new List<VentasDatos>();
            var connectionString = _context.Database.GetConnectionString();

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add(new SqlParameter("@fechaInicio", SqlDbType.DateTime) { Value = fechaInicio });
                    command.Parameters.Add(new SqlParameter("@fechaFin", SqlDbType.DateTime) { Value = fechaFin });

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            resultado.Add(new VentasDatos
                            {
                                ITEM = reader["ITEM"] != DBNull.Value ? Convert.ToInt32(reader["ITEM"]) : 0,
                                VENTA = reader["VENTA"] != DBNull.Value ? Convert.ToDecimal(reader["VENTA"]) : 0,
                                COSTO = reader["COSTO"] != DBNull.Value ? Convert.ToDecimal(reader["COSTO"]) : 0,
                                CANTIDADES = reader["CANTIDADES"] != DBNull.Value ? Convert.ToDecimal(reader["CANTIDADES"]) : 0,
                                UTILIDAD = reader["UTILIDAD"] != DBNull.Value ? Convert.ToDecimal(reader["UTILIDAD"]) : 0,
                                FECHA = reader["FECHA"] != DBNull.Value ? Convert.ToDateTime(reader["FECHA"]) : DateTime.MinValue,
                                USUARIO = reader["USUARIO"]?.ToString(),
                                DEPARTAMENTO = reader["DEPARTAMENTO"]?.ToString(),
                                FABRICANTES = reader["FABRICANTES"]?.ToString(),
                                REPRESENTANTE = reader["REPRESENTANTE"]?.ToString(),
                                CLIENTE = reader["CLIENTE"]?.ToString(),
                                PRODFAMILIA = reader["PRODFAMILIA"]?.ToString(),
                                CODIGO = reader["CODIGO"]?.ToString(),
                                DESCRIPCION = reader["DESCRIPCION"]?.ToString(),
                                DOCNUM = reader["DOCNUM"]?.ToString(),
                                CATEGORIA = reader["CATEGORIA"]?.ToString(),
                                RUBRO = reader["RUBRO"]?.ToString(),
                                SECTOR_CLIENTE = reader["SECTOR_CLIENTE"]?.ToString()
                            });
                        }
                    }
                }
            }

            return resultado;
        }
    }
}
