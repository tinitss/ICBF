using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using static ICBFApp.Pages.TipoDoc.IndexModel;

namespace ICBFApp.Pages.TipoDoc
{
    public class IndexModel : PageModel
    {
        //String connectionString = "Data Source=BOGAPRCSFFSD121\\SQLEXPRESS;Initial Catalog=icbf;Integrated Security=True;";
        String connectionString = "Data Source=(localdb)\\SERVIDOR_MELO;Initial Catalog=ICBF;Integrated Security=True;";

        public List<TipoDocInfo> listTipoDoc = new List<TipoDocInfo>();
        public void OnGet()
        {
            try
            {            
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sqlSelect = "SELECT * FROM tipoDoc";

                    using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Validar si hay datos
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    TipoDocInfo tipoDocInfo = new TipoDocInfo();
                                    tipoDocInfo.pkIdTipoDoc = reader.GetInt32(0).ToString();
                                    tipoDocInfo.tipo = reader.GetString(1);


                                    listTipoDoc.Add(tipoDocInfo);
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

        public class TipoDocInfo
        {
            public string pkIdTipoDoc { get; set; }
            public string tipo { get; set; }

        }
    
    }
}
