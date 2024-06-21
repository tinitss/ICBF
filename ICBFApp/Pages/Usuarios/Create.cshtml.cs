using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Data;
using static ICBFApp.Pages.Roles.IndexModel;
using static ICBFApp.Pages.TipoDoc.IndexModel;
using static ICBFApp.Pages.Usuarios.IndexModel;
using System.Reflection.PortableExecutable;

namespace ICBFApp.Pages.Usuarios
{
    public class CreateModel : PageModel
    {
        //CONEXIÓN BD
        String connectionString = "Data Source=(localdb)\\SERVIDOR_MELO;Initial Catalog=ICBF;Integrated Security=True;";
        //String connectionString = "Data Source=GAES3\\SQLEXPRESS;Initial Catalog=ICBF;Integrated Security=True;";
        public List<RolInfo> rolInfo { get; set; } = new List<RolInfo>();
        public List<TipoDocInfo> tipoDocInfo { get; set; } = new List<TipoDocInfo>();
        public UsuarioInfo usuarioInfo = new UsuarioInfo();
        public string errorMessage = "";
        public string successMessage = "";
        public void OnGet()
        {
            try
            {
                

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT pkIdRol, tipo FROM roles";
                    //Console.WriteLine(sql);
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
            string identificacion = Request.Form["identificacion"];
            string nombre = Request.Form["nombre"];
            string fechaNacimiento = Request.Form["fechaNacimiento"];
            string telefono = Request.Form["telefono"];
            string correo = Request.Form["correo"];
            string direccion = Request.Form["direccion"];

            //int rolId = int.Parse(Request.Form["nombreRol"]);
            string rolIdString = Request.Form["tipo"];
            int rolId;

            string tipoDocIdString = Request.Form["tipo"];
            int tipoDocId;

            if (string.IsNullOrEmpty(identificacion) || string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(fechaNacimiento)
                || string.IsNullOrEmpty(telefono) || string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(direccion))
            {
                errorMessage = "Todos los campos son obligatorios";
                return;
            }

            if (!int.TryParse(rolIdString, out rolId))
            {
                errorMessage = "El Rol seleccionado es inválido";
                return;
            }

            if (!int.TryParse(tipoDocIdString, out tipoDocId))
            {
                errorMessage = "El Tipo de documento seleccionado es inválido";
                return;
            }


            try
            {
                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    /*String sqlExists = "SELECT COUNT(*) FROM persona " +
                                       "WHERE Identificacion = @identificacion";
                    using (SqlCommand commandCheck = new SqlCommand(sqlExists, connection))
                    {
                        commandCheck.Parameters.AddWithValue("@identificacion", personaInfo.identificacion);

                        int count = (int)commandCheck.ExecuteScalar();
                        if (count > 0)
                        {
                            errorMessage = "El usuario " + personaInfo.nombrePersona + " con identificación " + personaInfo.identificacion + " ya existe. " +
                                           "Verifique la informaci�n e intente de nuevo";
                            return;
                        }
                    }*/

                    String sqlExists = "SELECT COUNT(*) FROM usuarios WHERE Identificacion = @identificacion";
                    using (SqlCommand commandCheck = new SqlCommand(sqlExists, connection))
                    {
                        commandCheck.Parameters.AddWithValue("@identificacion", identificacion);

                        int count = (int)commandCheck.ExecuteScalar();
                        if (count > 0)
                        {
                            errorMessage = "El usuario " + usuarioInfo.nombre + " con identificación " + usuarioInfo.identificacion + " ya existe. " +
                                           "Verifique la informaci�n e intente de nuevo";
                            return;
                        }
                    }

                    String sqlInsert = "INSERT INTO usuarios" +
                                       "(identificacion, nombre, fechaNacimiento, telefono, correo, direccion, fkIdRol, fkIdTipoDoc) " +
                                       "VALUES" +
                                       "(@identificacion, @nombre, @fechaNacimiento, @telefono, @correo, @direccion, @rolId, @tipoDocId)";

                    using (SqlCommand command = new SqlCommand(sqlInsert, connection))
                    {
                        command.Parameters.AddWithValue("@identificacion", identificacion);
                        command.Parameters.AddWithValue("@nombre", nombre);
                        command.Parameters.AddWithValue("@fechaNacimiento", fechaNacimiento);
                        command.Parameters.AddWithValue("@telefono", telefono);
                        command.Parameters.AddWithValue("@correo", correo);
                        command.Parameters.AddWithValue("@direccion", direccion);
                        command.Parameters.AddWithValue("@rolId", rolId);
                        command.Parameters.AddWithValue("@tipoDocId", tipoDocId);


                        command.ExecuteNonQuery();
                    }
                }

                successMessage = "Persona creada exitosamente";
                //Response.Redirect("/Persona/Index");
                RedirectToPage("/Usuarios/Index");
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }
    }
}
