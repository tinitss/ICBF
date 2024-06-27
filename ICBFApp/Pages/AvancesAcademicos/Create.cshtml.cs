using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ICBFApp.Pages.AvancesAcademicos
{
    public class CreateModel : PageModel
    {
        // CONEXI�N A LA BASE DE DATOS
        string connectionString = "Data Source=(localdb)\\SERVIDOR_MELO;Initial Catalog=ICBF;Integrated Security=True;";

        // Propiedad para el modelo de avance acad�mico
        [BindProperty]
        public AvanceAcademicoInfo AvanceAcademico { get; set; }

        // Lista para almacenar informaci�n de ni�os para el dropdown
        public List<NinoInfo> listNinos { get; set; } = new List<NinoInfo>();

        // Mensajes de error y �xito
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
                                    pkIdNino = reader.GetInt32(0),
                                    niup = reader.GetInt32(1)
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
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlInsert = @"
                        INSERT INTO avances_academicos (fechaNota, descripcion, anoEscolar, nivel, notas, fkIdNino)
                        VALUES (@fechaNota, @descripcion, @anoEscolar, @nivel, @notas, @fkIdNino)";

                    using (SqlCommand command = new SqlCommand(sqlInsert, connection))
                    {
                        command.Parameters.AddWithValue("@fechaNota", AvanceAcademico.fechaNota);
                        command.Parameters.AddWithValue("@descripcion", AvanceAcademico.descripcion);
                        command.Parameters.AddWithValue("@anoEscolar", AvanceAcademico.anoEscolar);
                        command.Parameters.AddWithValue("@nivel", AvanceAcademico.nivel);
                        command.Parameters.AddWithValue("@notas", AvanceAcademico.notas);
                        command.Parameters.AddWithValue("@fkIdNino", AvanceAcademico.fkIdNino);

                        command.ExecuteNonQuery();
                    }
                }

                successMessage = "Avance acad�mico creado exitosamente.";
                return RedirectToPage("/AvancesAcademicos/Index");
            }
            catch (Exception ex)
            {
                errorMessage = "Error al crear el avance acad�mico: " + ex.Message;
                return Page();
            }
        }

        // Clase para representar la informaci�n de cada ni�o
        public class NinoInfo
        {
            public int pkIdNino { get; set; }
            public int niup { get; set; }
        }

        // Clase para representar la informaci�n de cada avance acad�mico
        public class AvanceAcademicoInfo
        {
            public DateTime fechaNota { get; set; }
            public string descripcion { get; set; }
            public int anoEscolar { get; set; }
            public string nivel { get; set; }
            public string notas { get; set; }
            public int fkIdNino { get; set; }
        }
    }
}
