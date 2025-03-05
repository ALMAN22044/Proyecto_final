using Api.Models;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Api.Date
{
    public class DLogin
    {
        public string error = "";
        public async Task<List<MEstudiantes>> Login(MLogin login)
        {
            var list = new List<MEstudiantes>();

            using (var sql = new SqlConnection(Connection.cn))
            {
                await sql.OpenAsync();
                using (var cmd = new SqlCommand("Login", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Matricula", login.Matricula);
                    cmd.Parameters.AddWithValue("@Password", login.Password);
                    try
                    {
                        using (var dr = await cmd.ExecuteReaderAsync())
                        {
                            if (await dr.ReadAsync())
                            {
                                var estudiante = new MEstudiantes();
                                estudiante.EstudianteId = (int)dr[0];
                                estudiante.Nombre = (string)dr[1];
                                estudiante.Matricula = (string)dr[2];
                                estudiante.ProgramaID = (int)dr[3];
                                list.Add(estudiante);
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        error = e.Message;
                    }
                }
            }
            return list;
        }


    }
}
