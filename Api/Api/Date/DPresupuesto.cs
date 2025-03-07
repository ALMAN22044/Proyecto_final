using Api.Models;
using Microsoft.Data.SqlClient;

namespace Api.Date
{
    public class DPresupuesto
    {
        public async Task<MPresupuesto> Get(int ProgramaID)
        {
            var presupuesto = new MPresupuesto();

            using (var sql = new SqlConnection(Connection.cn))
            {
                await sql.OpenAsync();
                using (var cmd = new SqlCommand("Presupuesto", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("ProgramaID", ProgramaID);
                    using (var item = await cmd.ExecuteReaderAsync())
                    {
                        while (await item.ReadAsync())
                        {
                            var pre = new MPresu
                            {
                                Trimestre = (int)item[0],
                                Codigo_Asignatura = (string)item[1],
                                Nombre_Asignatura = (string)item[2],
                                Creditos_Asignatura = (int)item[3],
                                Dificultad = (byte)item[4],
                                Prerequisitos = item.IsDBNull(5) ? null : (string)item[5],
                                Correquisitos = item.IsDBNull(6) ? null : (string)item[6],
                                Creditos_Minimos_Requeridos = item.IsDBNull(7) ? null : (int?)item[7],
                                Costo_Asignatura = (decimal)item[8],
                                Total_Creditos = (int)item[9]
                            };

                            if (!presupuesto.Trimestres.ContainsKey(pre.Trimestre))
                            {
                                presupuesto.Trimestres[pre.Trimestre] = new Trimestre();
                            }

                            presupuesto.Trimestres[pre.Trimestre].Asignaturas.Add(pre);
                            presupuesto.Trimestres[pre.Trimestre].TotalCreditos = pre.Total_Creditos;
                        }
                    }
                }
            }

            return presupuesto;
        }
    }
}
                        
