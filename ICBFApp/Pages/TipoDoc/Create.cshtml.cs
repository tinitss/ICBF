using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using static ICBFApp.Pages.TipoDoc.IndexModel;

namespace ICBFApp.Pages.TipoDoc
{
    public class CreateModel : PageModel
    {
        //String connectionString = "Data Source=BOGAPRCSFFSD121\\SQLEXPRESS;Initial Catalog=icbf;Integrated Security=True;";
        string connectionString = "Data Source=DESKTOP-VCG45TQ\\SQLEXPRESS;Initial Catalog=ICBF;Integrated Security=True;";

        public TipoDocInfo tipoDocInfo = new TipoDocInfo();
        public string errorMessage = "";
        public string successMessage = "";
        public void OnGet()
        {
        }

        public void OnPost()
        {
            tipoDocInfo.tipo = Request.Form["tipo"];

            if (tipoDocInfo.tipo.Length == 0)
            {
                errorMessage = "Debe completar todos los campos";
                return;
            }

            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    //VERIFICA QUE EL TIPO DE DOCUMENTO NO EXISTA
                    String sqlExists = "SELECT COUNT(*) FROM tipoDoc WHERE tipo = @tipo";
                    using (SqlCommand commandCheck = new SqlCommand(sqlExists, connection))
                    {
                        commandCheck.Parameters.AddWithValue("@tipo", tipoDocInfo.tipo);

                        int count = (int)commandCheck.ExecuteScalar();

                        if (count > 0)
                        {
                            errorMessage = "El Tipo de Documento '" + tipoDocInfo.tipo + "' ya existe. Verifique la información e intente de nuevo.";
                            return;
                        }
                    }

                    
                    String sqlInsert = "INSERT INTO tipoDoc (tipo)" +
                        "VALUES (@tipo);";

                    using (SqlCommand command = new SqlCommand(sqlInsert, connection))
                    {
                        command.Parameters.AddWithValue("@tipo", tipoDocInfo.tipo);

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


            successMessage = "Tipo de Documento agregado con éxito";
            Response.Redirect("/TipoDoc/Index");
        }
    }
}
