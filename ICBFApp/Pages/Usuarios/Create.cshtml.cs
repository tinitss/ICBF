using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ICBFApp.Pages.Usuarios
{
    public class CreateModel : PageModel
    {
        // CONEXIÓN A LA BASE DE DATOS
        string connectionString = "Data Source=(localdb)\\SERVIDOR_MELO;Initial Catalog=ICBF;Integrated Security=True;";

        // Propiedades para almacenar opciones de selección
        public List<Rol> Roles { get; set; }
        public List<TipoDocumento> TiposDocumento { get; set; }

        // Método GET para cargar opciones de selección y preparar la página
        public void OnGet()
        {
            CargarRoles();
            CargarTiposDocumento();
        }

        // Método POST para manejar el envío del formulario de creación
        public async Task<IActionResult> OnPostAsync(string identificacion, string nombre, DateTime fechaNacimiento, string telefono, string correo, string direccion, int fkIdRol, int fkIdTipoDoc)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sqlInsert = @"INSERT INTO usuarios (identificacion, nombre, fechaNacimiento, telefono, correo, direccion, fkIdRol, fkIdTipoDoc)
                                         VALUES (@Identificacion, @Nombre, @FechaNacimiento, @Telefono, @Correo, @Direccion, @FkIdRol, @FkIdTipoDoc)";

                    using (SqlCommand command = new SqlCommand(sqlInsert, connection))
                    {
                        command.Parameters.AddWithValue("@Identificacion", identificacion);
                        command.Parameters.AddWithValue("@Nombre", nombre);
                        command.Parameters.AddWithValue("@FechaNacimiento", fechaNacimiento);
                        command.Parameters.AddWithValue("@Telefono", telefono);
                        command.Parameters.AddWithValue("@Correo", correo);
                        command.Parameters.AddWithValue("@Direccion", direccion);
                        command.Parameters.AddWithValue("@FkIdRol", fkIdRol);
                        command.Parameters.AddWithValue("@FkIdTipoDoc", fkIdTipoDoc);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                return Page();
            }
        }

        // Método para cargar roles desde la base de datos
        private void CargarRoles()
        {
            Roles = new List<Rol>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sqlSelectRoles = "SELECT pkIdRol, tipo FROM roles";

                using (SqlCommand command = new SqlCommand(sqlSelectRoles, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Roles.Add(new Rol
                            {
                                pkIdRol = reader.GetInt32(0),
                                tipo = reader.GetString(1)
                            });
                        }
                    }
                }
            }
        }

        // Método para cargar tipos de documento desde la base de datos
        private void CargarTiposDocumento()
        {
            TiposDocumento = new List<TipoDocumento>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sqlSelectTipos = "SELECT pkIdTipoDoc, tipo FROM tipoDoc";

                using (SqlCommand command = new SqlCommand(sqlSelectTipos, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TiposDocumento.Add(new TipoDocumento
                            {
                                pkIdTipoDoc = reader.GetInt32(0),
                                tipo = reader.GetString(1)
                            });
                        }
                    }
                }
            }
        }

        // Clase para representar un rol
        public class Rol
        {
            public int pkIdRol { get; set; }
            public string tipo { get; set; }
        }

        // Clase para representar un tipo de documento
        public class TipoDocumento
        {
            public int pkIdTipoDoc { get; set; }
            public string tipo { get; set; }
        }
    }
}
