using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ICBFApp.Pages.Usuarios
{
    public class CreateModel : PageModel
    {
        // CONEXIÓN BD
        string connectionString = "Data Source=(localdb)\\SERVIDOR_MELO;Initial Catalog=ICBF;Integrated Security=True;";

        // Propiedades para la página
        public UsuarioInfo usuarioInfo { get; set; } = new UsuarioInfo();
        public List<RolInfo> rolesInfo { get; set; } = new List<RolInfo>();
        public List<TipoDocInfo> tiposDocInfo { get; set; } = new List<TipoDocInfo>();
        public string errorMessage = "";
        public string successMessage = "";

        // Método GET
        public void OnGet()
        {
            // Cargar información necesaria para la página de creación
            try
            {
                // Consulta para obtener información de roles
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT pkIdRol, tipo FROM roles";
                    using (SqlCommand command = new SqlCommand(sql, connection))
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

                // Consulta para obtener información de tipos de documento
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT pkIdTipoDoc, tipo FROM tipoDoc";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tiposDocInfo.Add(new TipoDocInfo
                                {
                                    pkIdTipoDoc = reader.GetInt32(0).ToString(),
                                    tipo = reader.GetString(1)
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

        // Método POST
        public IActionResult OnPost()
        {
            try
            {
                usuarioInfo.identificacion = Request.Form["usuarioInfo.identificacion"];
                usuarioInfo.nombre = Request.Form["usuarioInfo.nombre"];
                usuarioInfo.fechaNacimiento = Convert.ToDateTime(Request.Form["usuarioInfo.fechaNacimiento"]);
                usuarioInfo.telefono = Request.Form["usuarioInfo.telefono"];
                usuarioInfo.correo = Request.Form["usuarioInfo.correo"];
                usuarioInfo.direccion = Request.Form["usuarioInfo.direccion"];
                usuarioInfo.fkIdRol = Convert.ToInt32(Request.Form["usuarioInfo.fkIdRol"]);
                usuarioInfo.fkIdTipoDoc = Convert.ToInt32(Request.Form["usuarioInfo.fkIdTipoDoc"]);

                // Validar que todos los campos están completos
                if (string.IsNullOrEmpty(usuarioInfo.identificacion) ||
                    string.IsNullOrEmpty(usuarioInfo.nombre) ||
                    string.IsNullOrEmpty(usuarioInfo.telefono) ||
                    string.IsNullOrEmpty(usuarioInfo.correo) ||
                    string.IsNullOrEmpty(usuarioInfo.direccion) ||
                    usuarioInfo.fechaNacimiento == DateTime.MinValue ||
                    usuarioInfo.fkIdRol == 0 ||
                    usuarioInfo.fkIdTipoDoc == 0)
                {
                    errorMessage = "Todos los campos son obligatorios.";
                    return Page();
                }

                // Insertar en la base de datos
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    String sqlExists = "SELECT COUNT(*) FROM usuarios WHERE identificacion = @identificacion";
                    using (SqlCommand commandCheck = new SqlCommand(sqlExists, connection))
                    {
                        commandCheck.Parameters.AddWithValue("@identificacion", usuarioInfo.identificacion);

                        int count = (int)commandCheck.ExecuteScalar();

                        if (count > 0)
                        {
                            errorMessage = "El Usuario '" + usuarioInfo.identificacion + "' ya existe. Verifique la información e intente de nuevo.";
                            return Page();
                        }
                    }


                    string sqlInsert = @"
                        INSERT INTO usuarios (identificacion, nombre, fechaNacimiento, telefono, correo, direccion, fkIdRol, fkIdTipoDoc)
                        VALUES (@identificacion, @nombre, @fechaNacimiento, @telefono, @correo, @direccion, @fkIdRol, @fkIdTipoDoc)";
                    using (SqlCommand command = new SqlCommand(sqlInsert, connection))
                    {
                        command.Parameters.AddWithValue("@identificacion", usuarioInfo.identificacion);
                        command.Parameters.AddWithValue("@nombre", usuarioInfo.nombre);
                        command.Parameters.AddWithValue("@fechaNacimiento", usuarioInfo.fechaNacimiento);
                        command.Parameters.AddWithValue("@telefono", usuarioInfo.telefono);
                        command.Parameters.AddWithValue("@correo", usuarioInfo.correo);
                        command.Parameters.AddWithValue("@direccion", usuarioInfo.direccion);
                        command.Parameters.AddWithValue("@fkIdRol", usuarioInfo.fkIdRol);
                        command.Parameters.AddWithValue("@fkIdTipoDoc", usuarioInfo.fkIdTipoDoc);

                        command.ExecuteNonQuery();
                    }
                }

                // Redirigir a la página de lista de usuarios o a donde sea necesario
                return RedirectToPage("/Usuarios/Index");
            }
            catch (Exception ex)
            {
                errorMessage = "Error al crear el usuario: " + ex.Message;
                return Page(); // Retorna la página con el mensaje de error
            }
        }

        // Clase para representar la información de cada usuario
        public class UsuarioInfo
        {
            public string pkIdUsuario { get; set; }
            public string identificacion { get; set; }
            public string nombre { get; set; }
            public DateTime fechaNacimiento { get; set; } = DateTime.Today;
            public string telefono { get; set; }
            public string correo { get; set; }
            public string direccion { get; set; }
            public int fkIdRol { get; set; }
            public int fkIdTipoDoc { get; set; }
        }

        // Clase para representar la información de roles
        public class RolInfo
        {
            public string pkIdRol { get; set; }
            public string tipo { get; set; }
        }

        // Clase para representar la información de tipos de documento
        public class TipoDocInfo
        {
            public string pkIdTipoDoc { get; set; }
            public string tipo { get; set; }
        }
    }
}
