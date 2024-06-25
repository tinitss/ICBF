using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;

namespace ICBFApp.Pages.EPS
{
    public class EditModel : PageModel
    {
        public EpsInfo epsInfo = new EpsInfo();
        public string errorMessage = "";
        public string successMessage = "";
        private readonly string connectionString = "Data Source=(localdb)\\SERVIDOR_MELO;Initial Catalog=ICBF;Integrated Security=True";

        public void OnGet()
        {
            String id = Request.Query["id"];

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT * FROM eps WHERE pkIdEps = @id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                epsInfo.pkIdEps = reader.GetInt32(0).ToString();
                                epsInfo.nit = reader.GetInt32(1).ToString();
                                epsInfo.nombre = reader.GetString(2);
                                epsInfo.centroMedico = reader.GetString(3);
                                epsInfo.direccion = reader.GetString(4);
                                epsInfo.telefono = reader.GetInt32(5).ToString();
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
            epsInfo.pkIdEps = Request.Form["id"];
            epsInfo.nit = Request.Form["nit"];
            epsInfo.nombre = Request.Form["nombre"];
            epsInfo.centroMedico = Request.Form["centroMedico"];
            epsInfo.direccion = Request.Form["direccion"];
            epsInfo.telefono = Request.Form["telefono"];

            if (string.IsNullOrEmpty(epsInfo.pkIdEps) || string.IsNullOrEmpty(epsInfo.nit) || string.IsNullOrEmpty(epsInfo.nombre) || string.IsNullOrEmpty(epsInfo.centroMedico) || string.IsNullOrEmpty(epsInfo.direccion) || string.IsNullOrEmpty(epsInfo.telefono))
            {
                errorMessage = "Debe completar todos los campos";
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sqlUpdate = "UPDATE eps SET nit = @nit, nombre = @nombre, centro_medico = @centroMedico, direccion = @direccion, telefono = @telefono WHERE pkIdEps = @id";
                    using (SqlCommand command = new SqlCommand(sqlUpdate, connection))
                    {
                        command.Parameters.AddWithValue("@id", int.Parse(epsInfo.pkIdEps));
                        command.Parameters.AddWithValue("@nit", int.Parse(epsInfo.nit));
                        command.Parameters.AddWithValue("@nombre", epsInfo.nombre);
                        command.Parameters.AddWithValue("@centroMedico", epsInfo.centroMedico);
                        command.Parameters.AddWithValue("@direccion", epsInfo.direccion);
                        command.Parameters.AddWithValue("@telefono", int.Parse(epsInfo.telefono));

                        command.ExecuteNonQuery();
                    }
                }

                successMessage = "EPS actualizada correctamente";
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/EPS/Index");
        }

        public class EpsInfo
        {
            public string pkIdEps { get; set; }
            public string nit { get; set; }
            public string nombre { get; set; }
            public string centroMedico { get; set; }
            public string direccion { get; set; }
            public string telefono { get; set; }
        }
    }
}
