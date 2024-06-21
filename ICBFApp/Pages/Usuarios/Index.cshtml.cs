using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ICBFApp.Pages.Usuarios
{
    public class IndexModel : PageModel
    {
        public List<UsuarioInfo> listUsuario = new List<UsuarioInfo>();
        public void OnGet()
        {
            try
            {
                 String connectionString = "Data Source=GAES3\\SQLEXPRESS;Initial Catalog=ICBF;Integrated Security=True;";
                //String connectionString = "Data Source=(localdb)\\SERVIDOR_MELO;Initial Catalog=ICBF;Integrated Security=True;";


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sqlSelect = "SELECT u.*, r.tipo AS nombre_rol, td.tipo AS nombre_tipodoc " +
                   "FROM usuarios u " +
                   "INNER JOIN roles r ON u.fkIdRol = r.pkIdRol " +
                   "INNER JOIN tipoDoc td ON u.fkIdTipoDoc = td.pkIdTipoDoc";




                    using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Validar si hay datos
                            //if (reader.HasRows)
                            //{
                                Console.WriteLine("ssi hay copssitas");
                                while (reader.Read())
                                {
                                    UsuarioInfo usuarioInfo = new UsuarioInfo();
                                    usuarioInfo.pkIdUsuario = "" + reader.GetInt32(0);
                                    usuarioInfo.identificacion = reader.GetString(1);
                                    usuarioInfo.nombre = reader.GetString(2);
                                    usuarioInfo.fechaNacimiento = reader.GetDateTime(3).ToString();
                                    usuarioInfo.telefono = reader.GetString(4);
                                    usuarioInfo.correo = reader.GetString(5);
                                    usuarioInfo.direccion = reader.GetString(6);
                                    usuarioInfo.fkIdRol = reader.GetInt32(7).ToString();
                                    usuarioInfo.fkIdTipoDoc = reader.GetInt32(8).ToString();
                                    usuarioInfo.nombre_rol = reader.GetString(9);
                                    usuarioInfo.nombre_tipodoc = reader.GetString(10);

                                listUsuario.Add(usuarioInfo);
                                }
                            //}
                            //else
                            //{
                              // Console.WriteLine("No hay filas en el resultado");
                            //}
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
        }

        public class UsuarioInfo
        {
            public string pkIdUsuario { get; set; }
            public string identificacion { get; set; }
            public string nombre { get; set; }
            public string fechaNacimiento { get; set; }
            public string telefono { get; set; }
            public string correo { get; set; }
            public string direccion { get; set; }
            public string fkIdRol { get; set; }
            public string fkIdTipoDoc { get; set; }
            public string nombre_rol { get; set; }
            public string nombre_tipodoc { get; set; }

        }

    }
}
        