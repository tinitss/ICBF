using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data.SqlClient;

namespace ICBFApp.Pages.EPS
{
    public class CreateModel : PageModel
    {
        private readonly string connectionString = "Data Source=DESKTOP-VCG45TQ\\SQLEXPRESS;Initial Catalog=ICBF;Integrated Security=True;";




        public EpsInfo epsInfo = new EpsInfo();
        public string errorMessage = "";
        public string successMessage = "";

        public void OnGet()
        {
        }

        public void OnPost()
        {
            epsInfo.nit = Request.Form["nit"];
            epsInfo.nombre = Request.Form["nombre"];
            epsInfo.centroMedico = Request.Form["centroMedico"];
            epsInfo.direccion = Request.Form["direccion"];
            epsInfo.telefono = Request.Form["telefono"];

            if (string.IsNullOrEmpty(epsInfo.nit) || string.IsNullOrEmpty(epsInfo.nombre) || string.IsNullOrEmpty(epsInfo.centroMedico) || string.IsNullOrEmpty(epsInfo.direccion) || string.IsNullOrEmpty(epsInfo.telefono))
            {
                errorMessage = "Debe completar todos los campos";
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();


                    string sqlExists = "SELECT COUNT(*) FROM EPS WHERE nombre = @nombre";
                    using (SqlCommand commandCheck = new SqlCommand(sqlExists, connection))
                    {
                        commandCheck.Parameters.AddWithValue("@nombre", epsInfo.nombre);
                        int count = (int)commandCheck.ExecuteScalar();

                        if (count > 0)
                        {
                            errorMessage = $"La EPS '{epsInfo.nombre}' ya existe. Verifique la información e intente de nuevo.";
                            return;
                        }
                    }

                    string sqlInsert = "INSERT INTO EPS (nit, nombre, centro_medico, direccion, telefono) VALUES (@nit, @nombre, @centroMedico, @direccion, @telefono)";
                    using (SqlCommand command = new SqlCommand(sqlInsert, connection))
                    {
                        command.Parameters.AddWithValue("@nit", epsInfo.nit);
                        command.Parameters.AddWithValue("@nombre", epsInfo.nombre);
                        command.Parameters.AddWithValue("@centroMedico", epsInfo.centroMedico);
                        command.Parameters.AddWithValue("@direccion", epsInfo.direccion);
                        command.Parameters.AddWithValue("@telefono", epsInfo.telefono);

                        command.ExecuteNonQuery();
                    }
                }

                epsInfo.nit = "";
                epsInfo.nombre = "";
                epsInfo.centroMedico = "";
                epsInfo.direccion = "";
                epsInfo.telefono = "";

                successMessage = "EPS agregada con éxito";
                Response.Redirect("/EPS/Index");
            }
            catch (Exception ex)
            {
                errorMessage = $"Ocurrió un error: {ex.Message}";
            }
        }
    }

    public class EpsInfo
    {
        public string nit { get; set; }
        public string nombre { get; set; }
        public string centroMedico { get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }
    }
}
