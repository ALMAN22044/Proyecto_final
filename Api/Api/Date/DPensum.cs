using Api.Models;
using Microsoft.Data.SqlClient;


namespace Api.Date
{
    public class DPensum
    {
        private string cn  = Connection.cn;
        public async Task<List<MPensum>> GetPensum()
        {
            var list = new List<MPensum>();
            using (var sql = new SqlConnection(cn))
            {
                using (var cmd = new SqlCommand("RPensum", sql))
                {
                   
                    await sql.OpenAsync();
                    using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            var pensum = new MPensum();
                            pensum.Trimestre= (int)dr[0];
                            pensum.Codigo_Asignatura = (string)dr[1];
                            pensum.Nombre_Asignatura = (string)dr[2];
                            pensum.Creditos_Asignatura = (int)dr[3];
                            pensum.Prerequisitos = dr[4] as string;
                            pensum.Correquisitos = dr[5] as string;
                            pensum.Creditos_Minimos_Requeridos = dr[6] as int?;
                            list.Add(pensum);
                        }
                    }
                    return list;
                }
            }
        }   
    }
}
