using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using static ICBFApp.Pages.Roles.IndexModel;
using static ICBFApp.Pages.TipoDoc.IndexModel;
using static ICBFApp.Pages.Usuarios.IndexModel;

namespace ICBFApp.Pages.Usuarios
{
    public class EditModel : PageModel
    {
        //CONEXIÓN
        //String connectionString = "Data Source=GAES3\\SQLEXPRESS;Initial Catalog=ICBF;Integrated Security=True;";
        String connectionString = "Data Source=(localdb)\\SERVIDOR_MELO;Initial Catalog=ICBF;Integrated Security=True";

        public List<RolInfo> rolInfo { get; set; } = new List<RolInfo>();
        public List<TipoDocInfo> tipoDocInfo { get; set; } = new List<TipoDocInfo>();
        public UsuarioInfo usuarioInfo = new UsuarioInfo();
        public string errorMessage = "";
        public string successMessage = "";
        
        public void OnGet()
        {
            String id = Request.Query["id"];

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT * FROM usuarios WHERE pkIdUsuario = @id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                usuarioInfo.pkIdUsuario = "" + reader.GetInt32(0);
                                usuarioInfo.identificacion = reader.GetString(1);
                                usuarioInfo.nombre = reader.GetString(2);
                                usuarioInfo.fechaNacimiento = reader.GetDateTime(3).ToString();
                                usuarioInfo.telefono = reader.GetString(4);
                                usuarioInfo.correo = reader.GetString(5);
                                usuarioInfo.direccion = reader.GetString(6);
                                usuarioInfo.fkIdRol = reader.GetInt32(7).ToString();
                                usuarioInfo.fkIdTipoDoc = reader.GetInt32(8).ToString();

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
        

            try
            {               

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT pkIdRol, tipo FROM roles";
                    
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Console.WriteLine(reader);
                            // Verificar si hay filas en el resultado antes de intentar leer
                            if (reader.HasRows)
                            {
                                Console.WriteLine("Se encontraron filas en la tabla roles.");
                                while (reader.Read())
                                {
                                    var pkIdRol = reader.GetInt32(0).ToString();
                                    var tipo = reader.GetString(1);

                                    Console.WriteLine("pkIdRol: {0}, tipo: {1}", pkIdRol, tipo);

                                    rolInfo.Add(new RolInfo
                                    {
                                        pkIdRol = reader.GetInt32(0).ToString(),
                                        tipo = reader.GetString(1)
                                    });

                                    Console.WriteLine("rolInfo Count: " + rolInfo.Count);
                                    foreach (var rol in rolInfo)
                                    {
                                        Console.WriteLine("List item - pkIdRol: {0}, tipo: {1}", rol.pkIdRol, rol.tipo);
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("No hay filas en el resultado.");
                                Console.WriteLine("No se encontraron datos en la tabla roles.");
                            }
                        }
                    }

                }
            }

            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }


            try
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT pkIdTipoDoc, tipo FROM tipoDoc";
                    //Console.WriteLine(sql);
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Console.WriteLine(reader);
                            // Verificar si hay filas en el resultado antes de intentar leer
                            if (reader.HasRows)
                            {
                                Console.WriteLine("Se encontraron filas en la tabla Tipo de Documento.");
                                while (reader.Read())
                                {
                                    var pkIdTipoDoc = reader.GetInt32(0).ToString();
                                    var tipo = reader.GetString(1);

                                    Console.WriteLine("pkIdTipoDoc: {0}, tipo: {1}", pkIdTipoDoc, tipo);

                                    tipoDocInfo.Add(new TipoDocInfo
                                    {
                                        pkIdTipoDoc = reader.GetInt32(0).ToString(),
                                        tipo = reader.GetString(1)
                                    });

                                    Console.WriteLine("tipoDocInfo Count: " + tipoDocInfo.Count);
                                    foreach (var tipoDoc in tipoDocInfo)
                                    {
                                        Console.WriteLine("List item - pkIdTipoDoc: {0}, tipo: {1}", tipoDoc.pkIdTipoDoc, tipoDoc.tipo);
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("No hay filas en el resultado.");
                                Console.WriteLine("No se encontraron datos en la tabla Tipo de Documento.");
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
        
        public void OnPost()
        {
            usuarioInfo.pkIdUsuario = Request.Form["id"];
            usuarioInfo.identificacion = Request.Form["identificacion"];
            usuarioInfo.nombre = Request.Form["nombre"];
            usuarioInfo.fechaNacimiento = Request.Form["fechaNacimiento"];
            usuarioInfo.telefono = Request.Form["telefono"];
            usuarioInfo.correo = Request.Form["correo"];
            usuarioInfo.direccion = Request.Form["direccion"];
            usuarioInfo.fkIdRol = Request.Form["fkIdRol"];
            usuarioInfo.fkIdTipoDoc = Request.Form["fkIdTipoDoc"];
            



            if (usuarioInfo.pkIdUsuario.Length == 0 || usuarioInfo.identificacion.Length == 0 
                || usuarioInfo.nombre.Length == 0 || usuarioInfo.fechaNacimiento.Length == 0
                || usuarioInfo.telefono.Length == 0 || usuarioInfo.correo.Length == 0
                || usuarioInfo.direccion.Length == 0 )
            {
                errorMessage = "Debe completar todos los campos";
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sqlUpdate = "UPDATE usuarios SET identificacion = @identificacion, nombre = @nombre, " +
                        "fechaNacimiento = @fechaNacimiento, telefono = @telefono,  correo = @correo,  direccion = @direccion," +
                        " fkIdRol = @fkIdRol,  fkIdTipoDoc = @fkIdTipoDoc WHERE pkIdUsuario = @id";
                    using (SqlCommand command = new SqlCommand(sqlUpdate, connection))
                    {
                        command.Parameters.AddWithValue("@id", usuarioInfo.pkIdUsuario);
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
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/Usuarios/Index");
        }
    }
}

