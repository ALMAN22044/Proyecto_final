internal class DPensum{

    private string cn = Conection.connection();

    public async Task<List<MPensum>> Get(){
        using ( var sql = new sqlconnection(cn)){
            await sql.OpenAsync()
            using ( var cmd = new sqlcommand("Pensum",sql))
            {
                cmd.CommandType =CommandType.Procedure;
                using ( var items = await s)
            }
        }
    }
}