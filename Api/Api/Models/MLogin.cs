namespace Api.Models
{
    public class MLogin
    {
        public string Matricula { get; set; }
        public string Password { get; set; }
    }

    public class MLoginResult
    {
        public List<MEstudiantes> Estudiantes { get; set; }
        public string Token { get; set; }
    }
}
