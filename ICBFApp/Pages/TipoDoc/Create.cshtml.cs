using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using static ICBFApp.Pages.TipoDoc.IndexModel;

namespace ICBFApp.Pages.TipoDoc
{
    public class CreateModel : PageModel
    {
        public TipoDocInfo tipoDocInfo = new TipoDocInfo();
        public string errorMessage = "";
        public string successMessage = "";
        public void OnGet()
        {
            tipoDocInfo.tipo = Request.Form["tipo"];



            if (tipoDocInfo.tipo.Length == 0 )
            {
                errorMessage = "Debe completar todos los campos";
                return;
            }

            try
            {
                //String connectionString = "Data Source=BOGAPRCSFFSD121\\SQLEXPRESS;Initial Catalog=icbf;Integrated Security=True;";
                String connectionString = "Data Source=(localdb)\\SERVIDOR_MELO;Initial Catalog=ICBF;Integrated Security=True";


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    String sqlExists = "SELECT COUNT(*) FROM tipoDoc WHERE tipo = @tipo";
                    using (SqlCommand commandCheck = new SqlCommand(sqlExists, connection))
                    {
                        commandCheck.Parameters.AddWithValue("@tipo", tipoDocInfo.tipo);

                        int count = (int)commandCheck.ExecuteScalar();

                        if (count > 0)
                        {
                            errorMessage = "El Jardín '" + tipoDocInfo.tipo + "' ya existe. Verifique la información e intente de nuevo.";
                            return;
                        }
                    }

                    // Espacio para validar que el jadin no exista
                    String sqlInsert = "INSERT INTO tipoDoc (tipo)" +
                        "VALUES (@tipo);";

                    using (SqlCommand command = new SqlCommand(sqlInsert, connection))
                    {
                        command.Parameters.AddWithValue("@nombre", tipoDocInfo.tipo);                      
                        command.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            tipoDocInfo.tipo = "";


            successMessage = "Tipo de documento agregado con éxito";
            Response.Redirect("/TipoDoc/Index");
        }
    }
}
