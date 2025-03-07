using Api.Models;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Date
{
    public class DLogin
    {
        public string error = "";
        public async Task<MLoginResult> Login(MLogin login)
        {
            var list = new List<MEstudiantes>();
            string token = "";

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
                                token = encriptar(estudiante.Matricula, estudiante.Nombre);
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

            return new MLoginResult
            {
                Estudiantes = list,
                Token = token
            };
        }

        private string encriptar(string matricula, string nombre)
        {
            // Clave secreta para firmar el JWT. **¡Nunca la hardcodees en producción!**
            // Debe almacenarse en una variable de entorno o configuración segura.
            string secretKey = "supersecreto12345678armandoproyectofinal"; // Ensure the key is at least 16 characters long

            // Crea una clave simétrica a partir de la clave secreta
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            // Define las credenciales de firma utilizando el algoritmo HMAC SHA-256
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Crea el payload con la matricula y el nombre
            var claims = new[]
            {
                new Claim("matricula", matricula),
                new Claim("nombre", nombre)
            };

            // Crea el descriptor del token con los claims y la expiración
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(60), // Expira en 60 minutos
                SigningCredentials = signingCredentials
            };

            // Genera el token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            // Obtiene el JWT como una cadena
            var encryptedToken = tokenHandler.WriteToken(securityToken);

            return encryptedToken;
        }


    }
}
