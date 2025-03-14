namespace Api.Models
{
    public class MPensum
    {
        public int Trimestre { set; get; }
        public string Codigo_Asignatura { get; set; }
        public string Nombre_Asignatura { get; set; }
        public int Creditos_Asignatura { get; set; }
        public string? Prerequisitos { get; set; }
        public string? Correquisitos { get; set; }
        public string? Nota { get; set; }
    }
}
