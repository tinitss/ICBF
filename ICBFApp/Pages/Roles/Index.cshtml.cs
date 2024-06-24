using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using static ICBFApp.Pages.Jardin.IndexModel;
using static ICBFApp.Pages.Roles.IndexModel;

namespace ICBFApp.Pages.Roles
{
    public class IndexModel : PageModel
    {
        //String connectionString = "Data Source=BOGAPRCSFFSD121\\SQLEXPRESS;Initial Catalog=icbf;Integrated Security=True;";
        String connectionString = "Data Source=(localdb)\\SERVIDOR_MELO;Initial Catalog=ICBF;Integrated Security=True";

        public List<RolInfo> listRol = new List<RolInfo>();

        public void OnGet()
        {
            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sqlSelect = "SELECT * FROM roles";

                    using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Validar si hay datos
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    RolInfo rolInfo = new RolInfo();
                                    rolInfo.pkIdRol = reader.GetInt32(0).ToString();
                                    rolInfo.tipo = reader.GetString(1);

                                    listRol.Add(rolInfo);
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

        public class RolInfo
        {
            public string pkIdRol { get; set; }
            public string tipo { get; set; }

        }
    }
}
