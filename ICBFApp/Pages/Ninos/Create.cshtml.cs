using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

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

        // Método POST para crear un nuevo niño
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

                // Validar y convertir datos
                int niup, epsId, jardinId, usuarioId;
                if (!int.TryParse(niupString, out niup) ||
                    !int.TryParse(epsIdString, out epsId) ||
                    !int.TryParse(jardinIdString, out jardinId) ||
                    !int.TryParse(usuarioIdString, out usuarioId))
                {
                    errorMessage = "Todos los campos son obligatorios.";
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
                            errorMessage = $"El NIUP '{niup}' ya está asignado a otro niño. Verifique la información e intente de nuevo.";
                            return Page(); // Retorna la página con el mensaje de error
                        }
                    }

                    // Insertar nuevo niño
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
                errorMessage = $"Error al crear el niño: {ex.Message}";
                return Page(); // Retorna la página con el mensaje de error
            }
        }

        // Método para generar el reporte en PDF
        public IActionResult OnPostGenerarReportePDF()
        {
            try
            {
                List<NinoInfo> ninos = new List<NinoInfo>();

                // Consulta para obtener la información de los niños desde la base de datos
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"
                        SELECT n.pkIdNino, n.niup, n.nombre, n.fechaNacimiento, n.tipoSangre, n.ciudadNacimiento, 
                               e.nombre AS nombre_eps, j.nombre AS nombre_jardin, u.nombre AS nombre_usuario
                        FROM ninos n
                        INNER JOIN eps e ON n.fkIdEps = e.pkIdEps
                        INNER JOIN jardines j ON n.fkIdJardin = j.pkIdJardin
                        INNER JOIN usuarios u ON n.fkIdUsuario = u.pkIdUsuario";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ninos.Add(new NinoInfo
                                {
                                    pkIdNino = reader.GetInt32(0).ToString(),
                                    niup = reader.GetInt32(1).ToString(),
                                    nombre = reader.GetString(2),
                                    fechaNacimiento = reader.GetDateTime(3),
                                    tipoSangre = reader.GetString(4),
                                    ciudadNacimiento = reader.GetString(5),
                                    fkIdEps = reader.GetInt32(6).ToString(),
                                    fkIdJardin = reader.GetInt32(7).ToString(),
                                    fkIdUsuario = reader.GetInt32(8).ToString()
                                });
                            }
                        }
                    }
                }

                // Generación del PDF
                string fileName = "Reporte_Ninos.pdf";
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "reports", fileName);

                using (var writer = new PdfWriter(filePath))
                {
                    using (var pdf = new PdfDocument(writer))
                    {
                        var document = new Document(pdf);
                        document.Add(new Paragraph("Reporte de Niños"));

                        // Agregar la información de los niños al PDF
                        foreach (var nino in ninos)
                        {
                            document.Add(new Paragraph($"NIUP: {nino.niup}"));
                            document.Add(new Paragraph($"Nombre: {nino.nombre}"));
                            document.Add(new Paragraph($"Fecha de Nacimiento: {nino.fechaNacimiento.ToShortDateString()}"));
                            document.Add(new Paragraph($"Tipo de Sangre: {nino.tipoSangre}"));
                            document.Add(new Paragraph($"Ciudad de Nacimiento: {nino.ciudadNacimiento}"));
                            document.Add(new Paragraph($"EPS: {nino.fkIdEps}"));
                            document.Add(new Paragraph($"Jardín: {nino.fkIdJardin}"));
                            document.Add(new Paragraph($"Usuario: {nino.fkIdUsuario}"));
                            document.Add(new AreaBreak());
                        }

                        document.Close();
                    }


                    // Descargar el archivo generado
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    string contentType = "application/pdf";
                    string fileNameDownload = "Reporte_Ninos.pdf";

                    return File(fileBytes, contentType, fileNameDownload);
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones y retorno de la página con mensaje de error
                errorMessage = $"Error al generar el reporte: {ex.Message}";
                return Page();
            }
        }

        // Clase para representar la información de cada EPS
        public class EpsInfo
        {
            public string pkIdEps { get; set; }
            public string nombre { get; set; }
        }

        // Clase para representar la información de cada Jardín
        public class JardinInfo
        {
            public string pkIdJardin { get; set; }
            public string nombre { get; set; }
        }

        // Clase para representar la información de cada Usuario
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
