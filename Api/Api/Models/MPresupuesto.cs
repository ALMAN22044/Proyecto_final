using System.Collections.Generic;

namespace Api.Models
{
    public class MPresupuesto
    {
        public Dictionary<int, Trimestre> Trimestres { get; set; } = new Dictionary<int, Trimestre>();
    }

    public class Trimestre
    {
        public List<MPresu> Asignaturas { get; set; } = new List<MPresu>();
        public int TotalCreditos { get; set; }
        public decimal TotalCosto { get; set; }
    }

    public class MPresu
    {
        public int Trimestre { get; set; }
        public string Codigo_Asignatura { get; set; }
        public string Nombre_Asignatura { get; set; }
        public int Creditos_Asignatura { get; set; }
        public byte Dificultad { get; set; }
        public string? Prerequisitos { get; set; }
        public string? Correquisitos { get; set; }
        public int? Creditos_Minimos_Requeridos { get; set; }
        public decimal Costo_Asignatura { get; set; }
        public int Total_Creditos { get; set; }
    }
}
