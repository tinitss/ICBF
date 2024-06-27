using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ICBFApp.Pages.Asistencias
{
    public class EditModel : PageModel
    {
        // CONEXIÓN A LA BASE DE DATOS
        string connectionString = "Data Source=(localdb)\\SERVIDOR_MELO;Initial Catalog=ICBF;Integrated Security=True;";

        // Propiedades para el modelo de asistencia
        [BindProperty]
        public AsistenciaInfo Asistencia { get; set; }

        // Lista para almacenar información de niños para el dropdown
        public List<NinoInfo> listNinos { get; set; } = new List<NinoInfo>();

        // Mensajes de error y éxito
        public string errorMessage { get; set; }
        public string successMessage { get; set; }

        // Método GET para cargar datos necesarios para la edición
        public IActionResult OnGet(int id)
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

                    string sqlSelectAsistencia = @"SELECT pkIdAsistencia, fecha, descripcionEstado, fkIdNino
                                                   FROM asistencias
                                                   WHERE pkIdAsistencia = @id";

                    using (SqlCommand command = new SqlCommand(sqlSelectAsistencia, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Asistencia = new AsistenciaInfo
                                {
                                    pkIdAsistencia = reader.GetInt32(0),
                                    fecha = reader.GetDateTime(1),
                                    descripcionEstado = reader.GetString(2),
                                    fkIdNino = reader.GetInt32(3).ToString()
                                };

                                return Page();
                            }
                            else
                            {
                                errorMessage = "Asistencia no encontrada.";
                                return RedirectToPage("/Asistencias/Index");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Error al cargar los datos para editar la asistencia: " + ex.Message;
                return RedirectToPage("/Asistencias/Index");
            }
        }

        // Método POST para manejar el envío del formulario de edición
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

                // Actualizar en la base de datos
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlUpdate = @"
                        UPDATE asistencias
                        SET fecha = @fecha,
                            descripcionEstado = @descripcionEstado,
                            fkIdNino = @fkIdNino
                        WHERE pkIdAsistencia = @pkIdAsistencia";

                    using (SqlCommand command = new SqlCommand(sqlUpdate, connection))
                    {
                        command.Parameters.AddWithValue("@pkIdAsistencia", Asistencia.pkIdAsistencia);
                        command.Parameters.AddWithValue("@fecha", DateTime.Parse(fecha));
                        command.Parameters.AddWithValue("@descripcionEstado", descripcionEstado);
                        command.Parameters.AddWithValue("@fkIdNino", fkIdNino);

                        command.ExecuteNonQuery();
                    }
                }

                successMessage = "Asistencia actualizada exitosamente.";
                return RedirectToPage("/Asistencias/Index");
            }
            catch (Exception ex)
            {
                errorMessage = "Error al actualizar la asistencia: " + ex.Message;
                return Page();
            }
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
            public int pkIdAsistencia { get; set; }
            public DateTime fecha { get; set; }
            public string descripcionEstado { get; set; }
            public string fkIdNino { get; set; } // Este campo maneja la clave foránea en la base de datos
        }
    }
}
