using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static ICBFApp.Pages.Ninos.IndexModel;
using System.Data.SqlClient;

namespace ICBFApp.Pages.Ninos
{
    public class CreateModel : PageModel
    {

        public NinoInfo ninoInfo = new NinoInfo();
        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
        }

        public void OnPost()
        {
            ninoInfo.niup = Request.Form["niup"];
            ninoInfo.tipoSangre = Request.Form["tipoSangre"];
            ninoInfo.ciudadNacimiento = Request.Form["ciudadNacimiento"];


            if (ninoInfo.niup.Length == 0 || ninoInfo.tipoSangre.Length == 0 || ninoInfo.ciudadNacimiento.Length == 0)
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

                    String sqlExists = "SELECT COUNT(*) FROM ninos WHERE niup = @niup";
                    using (SqlCommand commandCheck = new SqlCommand(sqlExists, connection))
                    {
                        commandCheck.Parameters.AddWithValue("@niup", ninoInfo.niup);

                        int count = (int)commandCheck.ExecuteScalar();

                        if (count > 0)
                        {
                            errorMessage = "El Niño '" + ninoInfo.niup + "' ya existe. Verifique la información e intente de nuevo.";
                            return;
                        }
                    }

                    // Espacio para validar que el niño no exista
                    String sqlInsert = "INSERT INTO ninos (niup, tipoSangre, ciudadNacimiento)" +
                        "VALUES (@niup, @tipoSangre, @ciudadNacimiento);";

                    using (SqlCommand command = new SqlCommand(sqlInsert, connection))
                    {
                        command.Parameters.AddWithValue("@nombre", ninoInfo.niup);
                        command.Parameters.AddWithValue("@direccion", ninoInfo.tipoSangre);
                        command.Parameters.AddWithValue("@estado", ninoInfo.ciudadNacimiento);

                        command.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            ninoInfo.niup = "";
            ninoInfo.tipoSangre = "";
            ninoInfo.ciudadNacimiento = "";

            successMessage = "Niño agregado con éxito";
            Response.Redirect("/Ninos/Index");
        }
    }
}

