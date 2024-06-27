using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using static ICBFApp.Pages.TipoDoc.IndexModel;

namespace ICBFApp.Pages.TipoDoc
{
    public class EditModel : PageModel
    {
        public TipoDocInfo tipoDocInfo = new TipoDocInfo();
        public string errorMessage = "";
        public string successMessage = "";


        //CONEXIÓN CON BD
        //String connectionString = "Data Source=BOGAPRCSFFSD121\\SQLEXPRESS;Initial Catalog=icbf;Integrated Security=True;";
        String connectionString = "Data Source=(localdb)\\SERVIDOR_MELO;Initial Catalog=ICBF;Integrated Security=True";
        public void OnGet()
        {

            String id = Request.Query["id"];

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();



                    String sql = "SELECT * FROM tipoDoc WHERE pkIdTipoDoc = @id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                tipoDocInfo.pkIdTipoDoc = "" + reader.GetInt32(0);
                                tipoDocInfo.tipo = reader.GetString(1);
                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
        }
        public void OnPost()
        {
            tipoDocInfo.pkIdTipoDoc = Request.Form["id"];
            tipoDocInfo.tipo = Request.Form["tipo"];


            if (tipoDocInfo.pkIdTipoDoc.Length == 0 || tipoDocInfo.tipo.Length == 0)
            {
                errorMessage = "Debe completar todos los campos";
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                    //VERIFICA QUE EL TIPODOC NO EXISTA
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


                    String sqlUpdate = "UPDATE tipoDoc SET tipo = @tipo WHERE pkIdTipoDoc = @id";
                    using (SqlCommand command = new SqlCommand(sqlUpdate, connection))
                    {
                        command.Parameters.AddWithValue("@id", tipoDocInfo.pkIdTipoDoc);
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

            Response.Redirect("/TipoDoc/Index");
        }

    }
}
