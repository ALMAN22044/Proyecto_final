using Api.Models;
using Microsoft.Data.SqlClient;

namespace Api.Date
{
    public class DPresupuesto
    {
        public async Task<MPresupuesto> GetD(int ProgramaID, int trimestre)
        {
            var presupuesto = new MPresupuesto();
            var presup = new List<MPresu>();

            presup = await GetDi(ProgramaID, trimestre);
            int suma = 0;
            decimal total = 0;
            foreach (var pre in presup)
            {
                
                if (!presupuesto.Trimestres.ContainsKey(pre.Trimestre))
                {
                    presupuesto.Trimestres[pre.Trimestre] = new Trimestre();
                    suma = 0;
                    total = 0;
                }
                presupuesto.Trimestres[pre.Trimestre].Asignaturas.Add(pre);
                suma += pre.Creditos_Asignatura;
                total += pre.Costo_Asignatura;
                presupuesto.Trimestres[pre.Trimestre].TotalCreditos =  suma;
                presupuesto.Trimestres[pre.Trimestre].TotalCosto = total;
            }

            return presupuesto;


        }
        public async Task<MPresupuesto> GetP(int ProgramaID, decimal presupuestoc)
        {
            var presupuesto = new MPresupuesto();
            var presup = new List<MPresu>();

            presup = await GetPr(ProgramaID, presupuestoc);
            int suma = 0;
            decimal total = 0;
            foreach (var pre in presup)
            {

                if (!presupuesto.Trimestres.ContainsKey(pre.Trimestre))
                {
                    presupuesto.Trimestres[pre.Trimestre] = new Trimestre();
                    suma = 0;
                    total = 0;
                }
                presupuesto.Trimestres[pre.Trimestre].Asignaturas.Add(pre);
                suma += pre.Creditos_Asignatura;
                total += pre.Costo_Asignatura;
                presupuesto.Trimestres[pre.Trimestre].TotalCreditos = suma;
                presupuesto.Trimestres[pre.Trimestre].TotalCosto = total;
            }

            return presupuesto;


        }


        private async Task<List<MPresu>> GetPr(int ProgramaID, decimal presupuesto)
        {

            var presup = new List<MPresu>();
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
                            presup.Add(pre);

                        }
                    }
                }
            }
            XDificultad dificultad = new XDificultad();
            return dificultad.PresupuestoCliente(presupuesto, presup);
        }
        private async Task<List<MPresu>> GetDi(int ProgramaID, int trimestre)
        {

            var presup = new List<MPresu>();
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
                            presup.Add(pre);

                        }
                    }
                }
            }
            XDificultad dificultad = new XDificultad();
            return dificultad.Dificultad(trimestre, presup);
        }

    }
}


