using System.Data;
using System.Data.SqlClient;

internal class DPensum{

    private string cn = Conection.connection;

    public async Task<List<MPensum>> Get(){
        var list = new List<MPensum>();
        using ( var sql = new SqlConnection(cn)){
            await sql.OpenAsync();
            using ( var cmd = new SqlCommand("Pensum",sql))
            {
                cmd.CommandType =CommandType.StoredProcedure;
                using (var items = await cmd.ExecuteReaderAsync())
                {
                    while (await items.ReadAsync())
                    {
                        var pensum = new MPensum();
                        pensum.Trimestre = items.GetInt32(0);
                        pensum.Codigo_Asignatura = items.GetString(1);
                        pensum.Nombre_Asignatura = items.GetString(2);
                        pensum.Creditos_Asignatura = items.GetInt32(3);
                        pensum.Prerequisitos = DBNull.Value.Equals(items[4]) ? null : items.GetString(4);
                        pensum.Correquisitos = DBNull.Value.Equals(items[5]) ? null : items.GetString(5);
                        pensum.Creditos_Minimos_Requeridos = DBNull.Value.Equals(items[6]) ? null : items.GetInt32(6);
                        list.Add(pensum);
                    }
                    
            }
        }
            return list;
        }
    }
}