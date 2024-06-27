using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static ICBFApp.Pages.Jardin.IndexModel;
using System.Data.SqlClient;

namespace ICBFApp.Pages.Jardin
{
    public class EditModel : PageModel
    {
        public JardinInfo jardinInfo = new JardinInfo();
        public string errorMessage = "";
        public string successMessage = "";
        //String connectionString = "Data Source=BOGAPRCSFFSD121\\SQLEXPRESS;Initial Catalog=icbf;Integrated Security=True;";
        string connectionString = "Data Source=DESKTOP-VCG45TQ\\SQLEXPRESS;Initial Catalog=ICBF;Integrated Security=True;";
        public void OnGet()
        {
            String id = Request.Query["id"];

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT * FROM jardines WHERE pkIdJardin = @id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                jardinInfo.pkIdJardin = "" + reader.GetInt32(0);
                                jardinInfo.nombre = reader.GetString(1);
                                jardinInfo.direccion = reader.GetString(2);
                                jardinInfo.estado = reader.GetString(3);
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
            jardinInfo.pkIdJardin = Request.Form["id"];
            jardinInfo.nombre = Request.Form["nombre"];
            jardinInfo.direccion = Request.Form["direccion"];
            jardinInfo.estado = Request.Form["estado"];

            if (jardinInfo.pkIdJardin.Length == 0 || jardinInfo.nombre.Length == 0 || jardinInfo.direccion.Length == 0 || jardinInfo.estado.Length == 0)
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
                    String sqlUpdate = "UPDATE jardines SET nombre = @nombre, direccion = @direccion, estado = @estado WHERE pkIdJardin = @id";
                    using (SqlCommand command = new SqlCommand(sqlUpdate, connection))
                    {
                        command.Parameters.AddWithValue("@id", jardinInfo.pkIdJardin);
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

            Response.Redirect("/Jardin/Index");
        }
    }
}
