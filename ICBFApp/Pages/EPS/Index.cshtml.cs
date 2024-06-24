using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ICBFApp.Pages.EPS
{
    public class IndexModel : PageModel
    {
        // Cadena de conexión a la base de datos
        string connectionString = "Data Source=(localdb)\\SERVIDOR_MELO;Initial Catalog=ICBF;Integrated Security=True;";

        // Lista para almacenar la información de EPS
        public List<EpsInfo> listEps = new List<EpsInfo>();

        public void OnGet()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sqlSelect = "SELECT pkIdEps, nit, nombre, centro_medico, direccion, telefono FROM eps"; // Especifica las columnas explícitamente en el SELECT

                    using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Validar si hay datos
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    EpsInfo epsInfo = new EpsInfo();

                                    // Asignación de valores a partir de los índices correctos y tipos de datos
                                    epsInfo.pkIdEps = reader.GetInt32(0).ToString();        // pkIdEps es un int en la base de datos
                                    epsInfo.nit = reader.GetInt32(1).ToString();            // nit es un int en la base de datos
                                    epsInfo.nombre = reader.GetString(2);                   // nombre es una cadena varchar(120)
                                    epsInfo.centroMedico = reader.GetString(3);             // centro_medico es una cadena varchar(120)
                                    epsInfo.direccion = reader.GetString(4);                // direccion es una cadena varchar(120)
                                    epsInfo.telefono = reader.GetInt32(5).ToString();        // telefono es un int en la base de datos

                                    listEps.Add(epsInfo);
                                }
                            }
                            else
                            {
                                Console.WriteLine("No hay filas en el resultado");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                // Puedes manejar la excepción de manera más detallada según tus necesidades
            }
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
