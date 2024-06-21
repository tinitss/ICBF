using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using static ICBFApp.Pages.Roles.IndexModel;

namespace ICBFApp.Pages.Roles
{
    public class EditModel : PageModel
    {

        public  RolInfo rolInfo = new RolInfo();
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

                   

                    String sql = "SELECT * FROM roles WHERE pkIdRol = @id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                rolInfo.pkIdRol = "" + reader.GetInt32(0);
                                rolInfo.tipo = reader.GetString(1);
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
            rolInfo.pkIdRol = Request.Form["id"];
            rolInfo.tipo = Request.Form["tipo"];


            if (rolInfo.pkIdRol.Length == 0 || rolInfo.tipo.Length == 0)
            {
                errorMessage = "Debe completar todos los campos";
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                    //VERIFICA QUE EL ADMINISTRADOR NO EXISTA
                    String sqlExists = "SELECT COUNT(*) FROM roles WHERE tipo = @tipo";
                    using (SqlCommand commandCheck = new SqlCommand(sqlExists, connection))
                    {
                        commandCheck.Parameters.AddWithValue("@tipo", rolInfo.tipo);

                        int count = (int)commandCheck.ExecuteScalar();

                        if (count > 0)
                        {
                            errorMessage = "El Rol '" + rolInfo.tipo + "' ya existe. Verifique la información e intente de nuevo.";
                            return;
                        }
                    }

                    // Espacio para validar que el rol no exista
                    String sqlInsert = "UPDATE INTO roles (tipo)" +
                        "VALUES (@tipo);";

                    using (SqlCommand command = new SqlCommand(sqlInsert, connection))
                    {
                        command.Parameters.AddWithValue("@tipo", rolInfo.tipo);

                        command.ExecuteNonQuery();
                    }
                    String sqlUpdate = "UPDATE roles SET tipo = @tipo WHERE pkIdRol = @id";
                    using (SqlCommand command = new SqlCommand(sqlUpdate, connection))
                    {
                        command.Parameters.AddWithValue("@id", rolInfo.pkIdRol);
                        command.Parameters.AddWithValue("@tipo", rolInfo.tipo);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/Roles/Index");
        }
        
    }
}
