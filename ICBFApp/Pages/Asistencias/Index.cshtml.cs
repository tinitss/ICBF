using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ICBFApp.Pages.Asistencias
{
    public class IndexModel : PageModel
    {
        // CONEXIÓN A LA BASE DE DATOS
        string connectionString = "Data Source=DESKTOP-VCG45TQ\\SQLEXPRESS;Initial Catalog=ICBF;Integrated Security=True;";

        // Lista para almacenar la información de asistencias
        public List<AsistenciaInfo> listAsistencias { get; set; } = new List<AsistenciaInfo>();

        // Mensajes de error y éxito
        public string ErrorMessage { get; set; }

        // Método GET para cargar la lista de asistencias
        public void OnGet()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlSelect = @"SELECT a.pkIdAsistencia, a.fecha, a.descripcionEstado, n.niup
                                         FROM asistencias a
                                         JOIN ninos n ON a.fkIdNino = n.pkIdNino";

                    using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AsistenciaInfo asistencia = new AsistenciaInfo
                                {
                                    pkIdAsistencia = reader.GetInt32(0),
                                    fecha = reader.GetDateTime(1),
                                    descripcionEstado = reader.GetString(2),
                                    niupNino = reader.GetInt32(3)
                                };

                                listAsistencias.Add(asistencia);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error al cargar las asistencias: " + ex.Message;
            }
        }

        // Clase para representar la información de cada asistencia
        public class AsistenciaInfo
        {
            public int pkIdAsistencia { get; set; }
            public DateTime fecha { get; set; }
            public string descripcionEstado { get; set; }
            public int niupNino { get; set; }
        }
    }
}
