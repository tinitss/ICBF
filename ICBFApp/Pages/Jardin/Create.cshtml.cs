using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static ICBFApp.Pages.Jardin.IndexModel;
using System.Data.SqlClient;

namespace ICBFApp.Pages.Jardin
{
    public class CreateModel : PageModel
    {
        //String connectionString = "Data Source=BOGAPRCSFFSD121\\SQLEXPRESS;Initial Catalog=icbf;Integrated Security=True;";
        string connectionString = "Data Source=DESKTOP-VCG45TQ\\SQLEXPRESS;Initial Catalog=ICBF;Integrated Security=True;";

        public JardinInfo jardinInfo = new JardinInfo();
        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
        }

        public void OnPost()
        {
            jardinInfo.nombre = Request.Form["nombre"];
            jardinInfo.direccion = Request.Form["direccion"];
            jardinInfo.estado = Request.Form["estado"];

            if (jardinInfo.nombre.Length == 0 || jardinInfo.direccion.Length == 0 || jardinInfo.estado.Length == 0)
            {
                errorMessage = "Debe completar todos los campos";
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    String sqlExists = "SELECT COUNT(*) FROM jardines WHERE nombre = @nombre";
                    using (SqlCommand commandCheck = new SqlCommand(sqlExists, connection))
                    {
                        commandCheck.Parameters.AddWithValue("@nombre", jardinInfo.nombre);

                        int count = (int)commandCheck.ExecuteScalar();

                        if (count > 0)
                        {
                            errorMessage = "El Jardín '" + jardinInfo.nombre + "' ya existe. Verifique la información e intente de nuevo.";
                            return;
                        }
                    }

                    // Espacio para validar que el jadin no exista
                    String sqlInsert = "INSERT INTO jardines (nombre, direccion, estado)" +
                        "VALUES (@nombre, @direccion, @estado);";

                    using (SqlCommand command = new SqlCommand(sqlInsert, connection))
                    {
                        command.Parameters.AddWithValue("@nombre", jardinInfo.nombre);
                        command.Parameters.AddWithValue("@direccion", jardinInfo.direccion);
                        command.Parameters.AddWithValue("@estado", jardinInfo.estado);

                        command.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            jardinInfo.nombre = "";
            jardinInfo.direccion = "";
            jardinInfo.estado = "";

            successMessage = "Jardín agregado con éxito";
            Response.Redirect("/Jardin/Index");
        }
    }
}
