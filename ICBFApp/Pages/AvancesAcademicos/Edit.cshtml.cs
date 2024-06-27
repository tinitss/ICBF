using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ICBFApp.Pages.AvancesAcademicos
{
    public class EditModel : PageModel
    {
        // CONEXIÓN A LA BASE DE DATOS
        string connectionString = "Data Source=DESKTOP-VCG45TQ\\SQLEXPRESS;Initial Catalog=ICBF;Integrated Security=True;";

        // Propiedad para el modelo de avance académico
        [BindProperty]
        public AvanceAcademicoInfo AvanceAcademico { get; set; }

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
                                    pkIdNino = reader.GetInt32(0),
                                    niup = reader.GetInt32(1)
                                });
                            }
                        }
                    }

                    string sqlSelectAvance = @"SELECT pkIdAvance, fechaNota, descripcion, anoEscolar, nivel, notas, fkIdNino
                                               FROM avances_academicos
                                               WHERE pkIdAvance = @id";

                    using (SqlCommand command = new SqlCommand(sqlSelectAvance, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                AvanceAcademico = new AvanceAcademicoInfo
                                {
                                    pkIdAvance = reader.GetInt32(0),
                                    fechaNota = reader.GetDateTime(1),
                                    descripcion = reader.GetString(2),
                                    anoEscolar = reader.GetInt32(3),
                                    nivel = reader.GetString(4),
                                    notas = reader.GetString(5),
                                    fkIdNino = reader.GetInt32(6)
                                };

                                return Page();
                            }
                            else
                            {
                                errorMessage = "Avance académico no encontrado.";
                                return RedirectToPage("/AvancesAcademicos/Index");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Error al cargar los datos para editar el avance académico: " + ex.Message;
                return RedirectToPage("/AvancesAcademicos/Index");
            }
        }

        // Método POST para manejar el envío del formulario de edición
        public IActionResult OnPost()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlUpdate = @"
                        UPDATE avances_academicos
                        SET fechaNota = @fechaNota,
                            descripcion = @descripcion,
                            anoEscolar = @anoEscolar,
                            nivel = @nivel,
                            notas = @notas,
                            fkIdNino = @fkIdNino
                        WHERE pkIdAvance = @pkIdAvance";

                    using (SqlCommand command = new SqlCommand(sqlUpdate, connection))
                    {
                        command.Parameters.AddWithValue("@pkIdAvance", AvanceAcademico.pkIdAvance);
                        command.Parameters.AddWithValue("@fechaNota", AvanceAcademico.fechaNota);
                        command.Parameters.AddWithValue("@descripcion", AvanceAcademico.descripcion);
                        command.Parameters.AddWithValue("@anoEscolar", AvanceAcademico.anoEscolar);
                        command.Parameters.AddWithValue("@nivel", AvanceAcademico.nivel);
                        command.Parameters.AddWithValue("@notas", AvanceAcademico.notas);
                        command.Parameters.AddWithValue("@fkIdNino", AvanceAcademico.fkIdNino);

                        command.ExecuteNonQuery();
                    }
                }

                successMessage = "Avance académico actualizado exitosamente.";
                return RedirectToPage("/AvancesAcademicos/Index");
            }
            catch (Exception ex)
            {
                errorMessage = "Error al actualizar el avance académico: " + ex.Message;
                return Page();
            }
        }

        // Clase para representar la información de cada niño
        public class NinoInfo
        {
            public int pkIdNino { get; set; }
            public int niup { get; set; }
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
            public int fkIdNino { get; set; }
        }
    }
}
