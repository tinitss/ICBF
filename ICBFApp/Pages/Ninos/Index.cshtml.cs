using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ICBFApp.Pages.Ninos
{
    public class IndexModel : PageModel
    {
        // CONEXIÓN A LA BASE DE DATOS
        string connectionString = "Data Source=(localdb)\\SERVIDOR_MELO;Initial Catalog=ICBF;Integrated Security=True;";

        // Lista para almacenar la información de los niños
        public List<NinoInfo> listNinos = new List<NinoInfo>();

        // Método GET para cargar la lista de niños
        public void OnGet()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlSelect = @"SELECT n.pkIdNino, n.niup, n.nombre, n.fechaNacimiento, n.tipoSangre, 
                                        n.ciudadNacimiento, e.nombre AS nombre_eps, j.nombre AS nombre_jardin, 
                                        u.nombre AS nombre_usuario
                                        FROM ninos n
                                        INNER JOIN eps e ON n.fkIdEps = e.pkIdEps
                                        INNER JOIN jardines j ON n.fkIdJardin = j.pkIdJardin
                                        INNER JOIN usuarios u ON n.fkIdUsuario = u.pkIdUsuario";

                    using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                NinoInfo ninoInfo = new NinoInfo
                                {
                                    pkIdNino = reader.GetInt32(0).ToString(),
                                    niup = reader.GetInt32(1).ToString(),
                                    nombre = reader.GetString(2),
                                    fechaNacimiento = reader.GetDateTime(3),
                                    tipoSangre = reader.GetString(4),
                                    ciudadNacimiento = reader.GetString(5),
                                    nombre_eps = reader.GetString(6),
                                    nombre_jardin = reader.GetString(7),
                                    nombre_usuario = reader.GetString(8)
                                };

                                listNinos.Add(ninoInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
        }

        // Clase para representar la información de cada niño
        public class NinoInfo
        {
            public string pkIdNino { get; set; }
            public string niup { get; set; }
            public string nombre { get; set; }
            public DateTime fechaNacimiento { get; set; }
            public string tipoSangre { get; set; }
            public string ciudadNacimiento { get; set; }
            public string nombre_eps { get; set; }
            public string nombre_jardin { get; set; }
            public string nombre_usuario { get; set; }
        }
    }
}
