using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ICBFApp.Pages.AvancesAcademicos
{
    public class IndexModel : PageModel
    {
        // CONEXIÓN A LA BASE DE DATOS
        string connectionString = "Data Source=(localdb)\\SERVIDOR_MELO;Initial Catalog=ICBF;Integrated Security=True;";

        // Lista para almacenar la información de los avances académicos
        public List<AvanceAcademicoInfo> listAvancesAcademicos = new List<AvanceAcademicoInfo>();

        // Método GET para cargar la lista de avances académicos
        public void OnGet()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlSelect = @"SELECT aa.pkIdAvance, aa.fechaNota, aa.descripcion, aa.anoEscolar, aa.nivel, aa.notas, n.niup
                                        FROM avances_academicos aa
                                        INNER JOIN ninos n ON aa.fkIdNino = n.pkIdNino";

                    using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AvanceAcademicoInfo avanceInfo = new AvanceAcademicoInfo
                                {
                                    pkIdAvance = reader.GetInt32(0),
                                    fechaNota = reader.GetDateTime(1),
                                    descripcion = reader.GetString(2),
                                    anoEscolar = reader.GetInt32(3),
                                    nivel = reader.GetString(4),
                                    notas = reader.GetString(5),
                                    niupNino = reader.GetInt32(6)
                                };

                                listAvancesAcademicos.Add(avanceInfo);
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

        // Clase para representar la información de cada avance académico
        public class AvanceAcademicoInfo
        {
            public int pkIdAvance { get; set; }
            public DateTime fechaNota { get; set; }
            public string descripcion { get; set; }
            public int anoEscolar { get; set; }
            public string nivel { get; set; }
            public string notas { get; set; }
            public int niupNino { get; set; }
        }
    }
}
