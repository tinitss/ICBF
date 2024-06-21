using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using static ICBFApp.Pages.Roles.IndexModel;

namespace ICBFApp.Pages.Roles
{
    public class CreateModel : PageModel
    {

        public RolInfo rolInfo = new RolInfo();
        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
        }

        public void OnPost()
        {
            rolInfo.tipo = Request.Form["tipo"];
            
            if (rolInfo.tipo.Length == 0 )
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
                    String sqlInsert = "INSERT INTO roles (tipo)" +
                        "VALUES (@tipo);";

                    using (SqlCommand command = new SqlCommand(sqlInsert, connection))
                    {
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

            rolInfo.tipo = "";


            successMessage = "Rol agregado con éxito";
            Response.Redirect("/Roles/Index");
        }
    }
}
