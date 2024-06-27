using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ICBFApp.Pages.Ninos
{
    public class EditModel : PageModel
    {
        // Conexi�n a la base de datos
        string connectionString = "Data Source=DESKTOP-VCG45TQ\\SQLEXPRESS;Initial Catalog=ICBF;Integrated Security=True;";

        // Propiedad para vincular los datos del ni�o
        [BindProperty]
        public NinoInfo ninoInfo { get; set; }

        // Listas para almacenar informaci�n de EPS, Jardines y Usuarios
        public List<EpsInfo> epsInfo { get; set; } = new List<EpsInfo>();
        public List<JardinInfo> jardinInfo { get; set; } = new List<JardinInfo>();
        public List<UsuarioInfo> usuarioInfo { get; set; } = new List<UsuarioInfo>();

        // Mensajes de �xito y error
        public string errorMessage { get; set; } = "";
        public string successMessage { get; set; } = "";

        // M�todo OnGet para cargar datos del ni�o seg�n su ID
        public IActionResult OnGet(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlSelect = @"
                        SELECT n.pkIdNino, n.niup, n.nombre, n.fechaNacimiento, n.tipoSangre, 
                               n.ciudadNacimiento, n.fkIdEps, n.fkIdJardin, n.fkIdUsuario,
                               e.nombre AS nombre_eps, j.nombre AS nombre_jardin, u.nombre AS nombre_usuario
                        FROM ninos n
                        INNER JOIN eps e ON n.fkIdEps = e.pkIdEps
                        INNER JOIN jardines j ON n.fkIdJardin = j.pkIdJardin
                        INNER JOIN usuarios u ON n.fkIdUsuario = u.pkIdUsuario
                        WHERE n.pkIdNino = @id";

                    using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ninoInfo = new NinoInfo
                                {
                                    pkIdNino = reader.GetInt32(0).ToString(),
                                    niup = reader.GetInt32(1).ToString(),
                                    nombre = reader.GetString(2),
                                    fechaNacimiento = reader.GetDateTime(3),
                                    tipoSangre = reader.GetString(4),
                                    ciudadNacimiento = reader.GetString(5),
                                    fkIdEps = reader.GetInt32(6).ToString(),
                                    fkIdJardin = reader.GetInt32(7).ToString(),
                                    fkIdUsuario = reader.GetInt32(8).ToString(),
                                    nombre_eps = reader.GetString(9),
                                    nombre_jardin = reader.GetString(10),
                                    nombre_usuario = reader.GetString(11)
                                };
                            }
                            else
                            {
                                errorMessage = "No se encontr� el ni�o con el ID especificado.";
                                return RedirectToPage("/Ninos/Index");
                            }
                        }
                    }
                }

                // Obtener informaci�n de EPS
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlEps = "SELECT pkIdEps, nombre FROM eps";
                    using (SqlCommand command = new SqlCommand(sqlEps, connection))
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

                // Obtener informaci�n de Jardines
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlJardines = "SELECT pkIdJardin, nombre FROM jardines";
                    using (SqlCommand command = new SqlCommand(sqlJardines, connection))
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

                // Obtener informaci�n de Usuarios
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlUsuarios = "SELECT pkIdUsuario, nombre FROM usuarios";
                    using (SqlCommand command = new SqlCommand(sqlUsuarios, connection))
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

                return Page(); // Retorna la p�gina de edici�n de ni�o
            }
            catch (Exception ex)
            {
                errorMessage = "Error al cargar los datos del ni�o: " + ex.Message;
                return RedirectToPage("/Ninos/Index");
            }
        }

        // M�todo OnPost para actualizar los datos del ni�o
        public IActionResult OnPost()
        {
            try
            {
                // Validaciones de datos
                int niup, epsId, jardinId, usuarioId;
                DateTime fechaNacimiento;
                if (!int.TryParse(ninoInfo.niup, out niup) ||
                    !DateTime.TryParseExact(ninoInfo.fechaNacimiento.ToString("yyyy-MM-dd"), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out fechaNacimiento) ||
                    !int.TryParse(ninoInfo.fkIdEps, out epsId) ||
                    !int.TryParse(ninoInfo.fkIdJardin, out jardinId) ||
                    !int.TryParse(ninoInfo.fkIdUsuario, out usuarioId))
                {
                    errorMessage = "Error en la conversi�n de datos.";
                    return Page(); // Retorna la p�gina con el mensaje de error
                }

                // Actualizar en la base de datos
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
                            errorMessage = "El NIUP '" + niup + "' ya est� asignado a otro ni�o. Verifique la informaci�n e intente de nuevo.";
                            return Page(); // Retorna la p�gina con el mensaje de error
                        }
                    }

                    string sqlUpdate = @"
                        UPDATE ninos
                        SET niup = @niup, nombre = @nombre, fechaNacimiento = @fechaNacimiento,
                            tipoSangre = @tipoSangre, ciudadNacimiento = @ciudadNacimiento,
                            fkIdEps = @epsId, fkIdJardin = @jardinId, fkIdUsuario = @usuarioId
                        WHERE pkIdNino = @id";

                    using (SqlCommand command = new SqlCommand(sqlUpdate, connection))
                    {
                        command.Parameters.AddWithValue("@niup", niup);
                        command.Parameters.AddWithValue("@nombre", ninoInfo.nombre);
                        command.Parameters.AddWithValue("@fechaNacimiento", fechaNacimiento);
                        command.Parameters.AddWithValue("@tipoSangre", ninoInfo.tipoSangre);
                        command.Parameters.AddWithValue("@ciudadNacimiento", ninoInfo.ciudadNacimiento);
                        command.Parameters.AddWithValue("@epsId", epsId);
                        command.Parameters.AddWithValue("@jardinId", jardinId);
                        command.Parameters.AddWithValue("@usuarioId", usuarioId);
                        command.Parameters.AddWithValue("@id", ninoInfo.pkIdNino);

                        command.ExecuteNonQuery();
                    }
                }

                successMessage = "Ni�o actualizado exitosamente.";
                return RedirectToPage("/Ninos/Index"); // Redirige a la p�gina de lista de ni�os
            }
            catch (Exception ex)
            {
                errorMessage = "Error al actualizar el ni�o: " + ex.Message;
                return Page(); // Retorna la p�gina con el mensaje de error
            }
        }

        // Clase para almacenar informaci�n de EPS
        public class EpsInfo
        {
            public string pkIdEps { get; set; }
            public string nombre { get; set; }
        }

        // Clase para almacenar informaci�n de Jardines
        public class JardinInfo
        {
            public string pkIdJardin { get; set; }
            public string nombre { get; set; }
        }

        // Clase para almacenar informaci�n de Usuarios
        public class UsuarioInfo
        {
            public string pkIdUsuario { get; set; }
            public string nombre { get; set; }
        }

        // Clase para almacenar informaci�n del ni�o
        public class NinoInfo
        {
            public string pkIdNino { get; set; }
            public string niup { get; set; }
            public string nombre { get; set; }
            public DateTime fechaNacimiento { get; set; }
            public string tipoSangre { get; set; }
            public string ciudadNacimiento { get; set; }
            public string fkIdEps { get; set; }
            public string fkIdJardin { get; set; }
            public string fkIdUsuario { get; set; }

            public string nombre_eps { get; set; }
            public string nombre_jardin { get; set; }
            public string nombre_usuario { get; set; }
        }
    }
}
