using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ICBFApp.Pages.Usuarios
{
    public class EditModel : PageModel
    {
        string connectionString = "Data Source=DESKTOP-VCG45TQ\\SQLEXPRESS;Initial Catalog=ICBF;Integrated Security=True;";

        public UsuarioInfo usuarioInfo = new UsuarioInfo();


        public List<RolInfo> rolesInfo { get; set; } = new List<RolInfo>();
        public List<TipoDocumentoInfo> tiposDocumentoInfo { get; set; } = new List<TipoDocumentoInfo>();
        public string errorMessage { get; set; } = "";
        public string successMessage { get; set; } = "";

        public IActionResult OnGet()
        {
            String id = Request.Query["id"];

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlSelect = @"
                        SELECT u.pkIdUsuario, u.identificacion, u.nombre, u.fechaNacimiento, u.telefono, 
                               u.correo, u.direccion, u.fkIdRol, u.fkIdTipoDoc, r.tipo AS nombre_rol, td.tipo AS nombre_tipo_doc
                        FROM usuarios u
                        INNER JOIN roles r ON u.fkIdRol = r.pkIdRol
                        INNER JOIN tipoDoc td ON u.fkIdTipoDoc = td.pkIdTipoDoc
                        WHERE u.pkIdUsuario = @id";

                    using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                usuarioInfo = new UsuarioInfo
                                {
                                    pkIdUsuario = reader.GetInt32(0).ToString(),
                                    identificacion = reader.GetString(1),
                                    nombre = reader.GetString(2),
                                    fechaNacimiento = reader.GetDateTime(3),
                                    telefono = reader.GetString(4),
                                    correo = reader.GetString(5),
                                    direccion = reader.GetString(6),
                                    fkIdRol = reader.GetInt32(7).ToString(),
                                    fkIdTipoDoc = reader.GetInt32(8).ToString(),
                                    nombre_rol = reader.GetString(9),
                                    nombre_tipo_doc = reader.GetString(10)
                                };
                            }
                            else
                            {
                                errorMessage = "No se encontró el usuario con el ID especificado.";
                                return RedirectToPage("/Usuarios/Index");
                            }
                        }
                    }
                }

                // Obtener información de Roles
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlRoles = "SELECT pkIdRol, tipo FROM roles";
                    using (SqlCommand command = new SqlCommand(sqlRoles, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                rolesInfo.Add(new RolInfo
                                {
                                    pkIdRol = reader.GetInt32(0).ToString(),
                                    tipo = reader.GetString(1)
                                });
                            }
                        }
                    }
                }

                // Obtener información de Tipos de Documento
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlTiposDocumento = "SELECT pkIdTipoDoc, tipo FROM tipoDoc";
                    using (SqlCommand command = new SqlCommand(sqlTiposDocumento, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tiposDocumentoInfo.Add(new TipoDocumentoInfo
                                {
                                    pkIdTipoDoc = reader.GetInt32(0).ToString(),
                                    tipo = reader.GetString(1)
                                });
                            }
                        }
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {
                errorMessage = "Error al cargar los datos del usuario: " + ex.Message;
                return RedirectToPage("/Usuarios/Index");
            }
        }

        public IActionResult OnPost()
        {
            try
            {
                // Validaciones de datos
                int rolId, tipoDocId;
                DateTime fechaNacimiento;
                if (!int.TryParse(usuarioInfo.fkIdRol, out rolId) ||
                    !int.TryParse(usuarioInfo.fkIdTipoDoc, out tipoDocId) ||
                    !DateTime.TryParseExact(usuarioInfo.fechaNacimiento.ToString("yyyy-MM-dd"), "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out fechaNacimiento))
                {
                    errorMessage = "Error en la conversión de datos.";
                    return Page(); // Retorna la página con el mensaje de error
                }

                // Actualizar en la base de datos
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sqlExists = "SELECT COUNT(*) FROM usuarios WHERE identificacion = @identificacion";
                    using (SqlCommand commandCheck = new SqlCommand(sqlExists, connection))
                    {
                        commandCheck.Parameters.AddWithValue("@identificacion", usuarioInfo.identificacion);

                        int count = (int)commandCheck.ExecuteScalar();

                        //if (count > 0)
                        //{
                        //    errorMessage = "El Usuario '" + usuarioInfo.identificacion + "' ya existe. Verifique la información e intente de nuevo.";
                        //    return Page();
                        //}
                    }
                    string sqlUpdate = @"
                        UPDATE usuarios
                        SET identificacion = @identificacion, nombre = @nombre, fechaNacimiento = @fechaNacimiento,
                            telefono = @telefono, correo = @correo, direccion = @direccion, fkIdRol = @rolId, fkIdTipoDoc = @tipoDocId
                        WHERE pkIdUsuario = @id";

                    using (SqlCommand command = new SqlCommand(sqlUpdate, connection))
                    {
                        command.Parameters.AddWithValue("@identificacion", usuarioInfo.identificacion);
                        command.Parameters.AddWithValue("@nombre", usuarioInfo.nombre);
                        command.Parameters.AddWithValue("@fechaNacimiento", fechaNacimiento);
                        command.Parameters.AddWithValue("@telefono", usuarioInfo.telefono);
                        command.Parameters.AddWithValue("@correo", usuarioInfo.correo);
                        command.Parameters.AddWithValue("@direccion", usuarioInfo.direccion);
                        command.Parameters.AddWithValue("@rolId", rolId);
                        command.Parameters.AddWithValue("@tipoDocId", tipoDocId);
                        command.Parameters.AddWithValue("@id", usuarioInfo.pkIdUsuario);

                        command.ExecuteNonQuery();
                    }
                }

                successMessage = "Usuario actualizado exitosamente.";
                return RedirectToPage("/Usuarios/Index"); // Redirige a la página de lista de usuarios
            }
            catch (Exception ex)
            {
                errorMessage = "Error al actualizar el usuario: " + ex.Message;
                return Page(); // Retorna la página con el mensaje de error
            }
        }

        public class RolInfo
        {
            public string pkIdRol { get; set; }
            public string tipo { get; set; }
        }

        public class TipoDocumentoInfo
        {
            public string pkIdTipoDoc { get; set; }
            public string tipo { get; set; }
        }

        public class UsuarioInfo
        {
            public string pkIdUsuario { get; set; }
            public string identificacion { get; set; }
            public string nombre { get; set; }
            public DateTime fechaNacimiento { get; set; }
            public string telefono { get; set; }
            public string correo { get; set; }
            public string direccion { get; set; }
            public string fkIdRol { get; set; }
            public string fkIdTipoDoc { get; set; }

            public string nombre_rol { get; set; }
            public string nombre_tipo_doc { get; set; }
        }
    }
}
