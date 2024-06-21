using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using static ICBFApp.Pages.Jardin.IndexModel;

namespace ICBFApp.Pages.Ninos
{
    public class IndexModel : PageModel
    {

        public List<NinoInfo> listNino = new List<NinoInfo>();

        public void OnGet()
        {

            try
            {
                //String connectionString = "Data Source=BOGAPRCSFFSD121\\SQLEXPRESS;Initial Catalog=icbf;Integrated Security=True;";
                String connectionString = "Data Source=(localdb)\\SERVIDOR_MELO;Initial Catalog=ICBF;Integrated Security=True;";


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sqlSelect = "SELECT * FROM ninos";

                    using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Validar si hay datos
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    NinoInfo ninoInfo = new NinoInfo();
                                    ninoInfo.pkIdNino = reader.GetInt32(0).ToString();
                                    ninoInfo.niup = reader.GetString(1);
                                    ninoInfo.tipoSangre = reader.GetString(2);
                                    ninoInfo.ciudadNacimiento = reader.GetString(3);
                                    ninoInfo.fkIdEps = reader.GetInt32(4).ToString();
                                    ninoInfo.fkIdJardin = reader.GetInt32(5).ToString();
                                    ninoInfo.fkIdUsuario = reader.GetInt32(6).ToString();
                                    ninoInfo.fkIdTipoDoc = reader.GetInt32(7).ToString();

                                    listNino.Add(ninoInfo);
                                }
                            }
                            else
                            {
                                Console.WriteLine("No hay filas en el resultado");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
        }

        public class NinoInfo
        {
            public string pkIdNino { get; set; }
            public string niup { get; set; }
            public string tipoSangre { get; set; }
            public string ciudadNacimiento { get; set; }
            public string fkIdEps { get; set; }
            public string fkIdJardin { get; set; }
            public string fkIdUsuario { get; set; }
            public string fkIdTipoDoc { get; set; }

        }
    }
}
