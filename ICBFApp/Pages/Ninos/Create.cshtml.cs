using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;

namespace ICBFApp.Pages.Ninos
{
    public class CreateModel : PageModel
    {
        // CONEXIÓN BD
        string connectionString = "Data Source=(localdb)\\SERVIDOR_MELO;Initial Catalog=ICBF;Integrated Security=True;";

        // Propiedades para la página
        public List<EpsInfo> epsInfo { get; set; } = new List<EpsInfo>();
        public List<JardinInfo> jardinInfo { get; set; } = new List<JardinInfo>();
        public List<UsuarioInfo> usuarioInfo { get; set; } = new List<UsuarioInfo>();
        public NinoInfo ninoInfo { get; set; } = new NinoInfo();
        public string errorMessage { get; set; } = "";
        public string successMessage { get; set; } = "";

        // Método GET
        public void OnGet()
        {
            try
            {
                // Consulta para obtener información de EPS
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT pkIdEps, nombre FROM eps";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                epsInfo.Add(new EpsInfo
                                {
                                    pkIdEps = reader.GetInt32(0).ToString(),
                                    nombre = reader.GetString(1)
                                });
                            }
                        }
                    }
                }

                // Consulta para obtener información de Jardines
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT pkIdJardin, nombre FROM jardines";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                jardinInfo.Add(new JardinInfo
                                {
                                    pkIdJardin = reader.GetInt32(0).ToString(),
                                    nombre = reader.GetString(1)
                                });
                            }
                        }
                    }
                }

                // Consulta para obtener información de Usuarios
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT pkIdUsuario, nombre FROM usuarios";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                usuarioInfo.Add(new UsuarioInfo
                                {
                                    pkIdUsuario = reader.GetInt32(0).ToString(),
                                    nombre = reader.GetString(1)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        // Método POST
        public IActionResult OnPost()
        {
            try
            {

                // Obtener datos del formulario
                string niupString = Request.Form["ninoInfo.niup"];
                string nombre = Request.Form["ninoInfo.nombre"];
                string tipoSangre = Request.Form["ninoInfo.tipoSangre"];
                string ciudadNacimiento = Request.Form["ninoInfo.ciudadNacimiento"];
                string epsIdString = Request.Form["ninoInfo.fkIdEps"];
                string jardinIdString = Request.Form["ninoInfo.fkIdJardin"];
                string usuarioIdString = Request.Form["ninoInfo.fkIdUsuario"];

                // Validaciones de datos
                int niup, epsId, jardinId, usuarioId;
                DateTime fechaNacimiento;
                if (!int.TryParse(niupString, out niup) ||
                    !int.TryParse(epsIdString, out epsId) ||
                    !int.TryParse(jardinIdString, out jardinId) ||
                    !int.TryParse(usuarioIdString, out usuarioId))
                {
                    errorMessage = "Error en la conversión de datos.";
                    return Page(); // Retorna la página con el mensaje de error
                }
                // Verificar si el NIUP ya existe
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlExists = "SELECT COUNT(*) FROM ninos WHERE niup = @niup";
                    using (SqlCommand commandCheck = new SqlCommand(sqlExists, connection))
                    {
                        commandCheck.Parameters.AddWithValue("@niup", niup);

                        int count = (int)commandCheck.ExecuteScalar();

                        if (count > 0)
                        {
                            errorMessage = "El NIUP '" + niup + "' ya está asignado a otro niño. Verifique la información e intente de nuevo.";
                            return Page(); // Retorna la página con el mensaje de error
                        }
                    }
                }
                // Insertar en la base de datos
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlInsert = @"
                    INSERT INTO ninos (niup, nombre, fechaNacimiento, tipoSangre, ciudadNacimiento, fkIdEps, fkIdJardin, fkIdUsuario)
                    VALUES (@niup, @nombre, @fechaNacimiento, @tipoSangre, @ciudadNacimiento, @epsId, @jardinId, @usuarioId)";
                    using (SqlCommand command = new SqlCommand(sqlInsert, connection))
                    {
                        command.Parameters.AddWithValue("@niup", niup);
                        command.Parameters.AddWithValue("@nombre", nombre);
                        command.Parameters.AddWithValue("@fechaNacimiento", ninoInfo.fechaNacimiento); // Usar DateTime directamente
                        command.Parameters.AddWithValue("@tipoSangre", tipoSangre);
                        command.Parameters.AddWithValue("@ciudadNacimiento", ciudadNacimiento);
                        command.Parameters.AddWithValue("@epsId", epsId);
                        command.Parameters.AddWithValue("@jardinId", jardinId);
                        command.Parameters.AddWithValue("@usuarioId", usuarioId);

                        command.ExecuteNonQuery();
                    }
                }

                successMessage = "Niño creado exitosamente.";
                return RedirectToPage("/Ninos/Index"); // Redirige a la página de lista de niños
            }
            catch (Exception ex)
            {
                errorMessage = "Error al crear el niño: " + ex.Message;
                return Page(); // Retorna la página con el mensaje de error
            }
        }

        

    // Clase para representar la información de cada niño
    public class EpsInfo
    {
        public string pkIdEps { get; set; }
        public string nombre { get; set; }
    }

    public class JardinInfo
    {
        public string pkIdJardin { get; set; }
        public string nombre { get; set; }
    }

    public class UsuarioInfo
    {
        public string pkIdUsuario { get; set; }
        public string nombre { get; set; }
    }

        // Clase para representar la información de cada niño
        public class NinoInfo
        {
            public string pkIdNino { get; set; }
            public string niup { get; set; }
            public string nombre { get; set; }
            public DateTime fechaNacimiento { get; set; } = DateTime.Today; // Inicializar con la fecha actual
            public string tipoSangre { get; set; }
            public string ciudadNacimiento { get; set; }
            public string fkIdEps { get; set; }
            public string fkIdJardin { get; set; }
            public string fkIdUsuario { get; set; }
        }
    
}
    }

