using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ICBFApp.Pages.Asistencias
{
    public class CreateModel : PageModel
    {
        // CONEXI�N A LA BASE DE DATOS
        string connectionString = "Data Source=(localdb)\\SERVIDOR_MELO;Initial Catalog=ICBF;Integrated Security=True;";

        // Propiedades para el modelo de asistencia
        public List<NinoInfo> listNinos { get; set; } = new List<NinoInfo>();
        public AsistenciaInfo Asistencia { get; set; } = new AsistenciaInfo();
        public string errorMessage { get; set; }
        public string successMessage { get; set; }

        // M�todo GET para cargar datos necesarios
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
                errorMessage = "Error al cargar los ni�os: " + ex.Message;
            }
        }

        // M�todo POST para manejar el env�o del formulario
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
                    errorMessage = "Error en la conversi�n de datos.";
                    return Page(); // Retorna la p�gina con el mensaje de error
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

        // Clase para representar la informaci�n de cada ni�o
        public class NinoInfo
        {
            public string pkIdNino { get; set; }
            public string niup { get; set; }
        }

        // Clase para representar la informaci�n de cada asistencia
        public class AsistenciaInfo
        {
            public DateTime fecha { get; set; } = DateTime.Today;
            public string descripcionEstado { get; set; }
            public string fkIdNino { get; set; } // Este campo maneja la clave for�nea en la base de datos
        }
    }
}
