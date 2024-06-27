using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace ICBFApp.Pages.Asistencias
{
    public class CreateModel : PageModel
    {
        // CONEXIÓN A LA BASE DE DATOS
        string connectionString = "Data Source=DESKTOP-VCG45TQ\\SQLEXPRESS;Initial Catalog=ICBF;Integrated Security=True;";

        // Propiedades para el modelo de asistencia
        public List<NinoInfo> listNinos { get; set; } = new List<NinoInfo>();
        public AsistenciaInfo Asistencia { get; set; } = new AsistenciaInfo();
        public string errorMessage { get; set; }
        public string successMessage { get; set; }

        // Método GET para cargar datos necesarios
        public void OnGet()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlSelectNinos = "SELECT pkIdNino, niup FROM ninos";

                    using (SqlCommand command = new SqlCommand(sqlSelectNinos, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listNinos.Add(new NinoInfo
                                {
                                    pkIdNino = reader.GetInt32(0).ToString(),
                                    niup = reader.GetInt32(1).ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Error al cargar los niños: " + ex.Message;
            }
        }

        // Método POST para manejar el envío del formulario
        public IActionResult OnPost()
        {
            try
            {
                // Obtener datos del formulario
                string fecha = Request.Form["Asistencia.fecha"];
                string descripcionEstado = Request.Form["Asistencia.descripcionEstado"];
                string fkIdNinoString = Request.Form["Asistencia.fkIdNino"];

                int fkIdNino;
                if (!int.TryParse(fkIdNinoString, out fkIdNino))
                {
                    errorMessage = "Error en la conversión de datos.";
                    return Page(); // Retorna la página con el mensaje de error
                }

                // Insertar en la base de datos
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlInsert = @"
                        INSERT INTO asistencias (fecha, descripcionEstado, fkIdNino)
                        VALUES (@fecha, @descripcionEstado, @fkIdNino)";

                    using (SqlCommand command = new SqlCommand(sqlInsert, connection))
                    {
                        command.Parameters.AddWithValue("@fecha", DateTime.Parse(fecha));
                        command.Parameters.AddWithValue("@descripcionEstado", descripcionEstado);
                        command.Parameters.AddWithValue("@fkIdNino", fkIdNino);

                        command.ExecuteNonQuery();
                    }
                }

                successMessage = "Asistencia registrada exitosamente.";
                return RedirectToPage("/Asistencias/Index");
            }
            catch (Exception ex)
            {
                errorMessage = "Error al registrar la asistencia: " + ex.Message;
                return Page();
            }
        }

        // Método de acción para generar reporte
        public async Task<IActionResult> OnGetGenerateReportAsync()
        {
            try
            {
                // Obtener datos para el reporte (ejemplo básico)
                List<AsistenciaInfo> asistencias = await GetAsistenciasAsync();

                // Generar contenido del reporte CSV
                StringBuilder csvContent = new StringBuilder();
                csvContent.AppendLine("Fecha,Descripción de Estado,NIUP");

                foreach (var asistencia in asistencias)
                {
                    csvContent.AppendLine($"{asistencia.fecha.ToShortDateString()},{asistencia.descripcionEstado},{asistencia.fkIdNino}");
                }

                // Preparar la respuesta como un archivo CSV
                byte[] buffer = Encoding.UTF8.GetBytes(csvContent.ToString());
                return File(buffer, "text/csv", "reporte_asistencias.csv");
            }
            catch (Exception ex)
            {
                errorMessage = "Error al generar el reporte: " + ex.Message;
                return Page();
            }
        }

        // Método auxiliar para obtener las asistencias desde la base de datos (ejemplo básico)
        private async Task<List<AsistenciaInfo>> GetAsistenciasAsync()
        {
            List<AsistenciaInfo> asistencias = new List<AsistenciaInfo>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string sqlSelectAsistencias = "SELECT fecha, descripcionEstado, fkIdNino FROM asistencias";

                using (SqlCommand command = new SqlCommand(sqlSelectAsistencias, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            asistencias.Add(new AsistenciaInfo
                            {
                                fecha = reader.GetDateTime(0),
                                descripcionEstado = reader.GetString(1),
                                fkIdNino = reader.GetInt32(2).ToString()
                            });
                        }
                    }
                }
            }

            return asistencias;
        }

        // Clase para representar la información de cada niño
        public class NinoInfo
        {
            public string pkIdNino { get; set; }
            public string niup { get; set; }
        }

        // Clase para representar la información de cada asistencia
        public class AsistenciaInfo
        {
            public DateTime fecha { get; set; } = DateTime.Today;
            public string descripcionEstado { get; set; }
            public string fkIdNino { get; set; } // Este campo maneja la clave foránea en la base de datos
        }
    }
}
