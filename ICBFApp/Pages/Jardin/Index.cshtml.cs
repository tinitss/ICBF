using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace ICBFApp.Pages.Jardin
{
    public class IndexModel : PageModel
    {
        public List<JardinInfo> listJardin = new List<JardinInfo>();

        public void OnGet()
        {
            try
            {
                //String connectionString = "Data Source=BOGAPRCSFFSD121\\SQLEXPRESS;Initial Catalog=icbf;Integrated Security=True;";
                String connectionString = "Data Source=(localdb)\\SERVIDOR_MELO;Initial Catalog=ICBF;Integrated Security=True;";


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sqlSelect = "SELECT * FROM jardines";

                    using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Validar si hay datos
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    JardinInfo jardinInfo = new JardinInfo();
                                    jardinInfo.pkIdJardin = reader.GetInt32(0).ToString();
                                    jardinInfo.nombre = reader.GetString(1);
                                    jardinInfo.direccion = reader.GetString(2);
                                    jardinInfo.estado = reader.GetString(3);

                                    listJardin.Add(jardinInfo);
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

        public class JardinInfo
        {
            public string pkIdJardin { get; set; }
            public string nombre { get; set; }
            public string direccion { get; set; }
            public string estado { get; set; }

        }
    }
}
