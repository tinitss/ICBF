using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace ICBFApp.Pages.Usuarios
{
    public class IndexModel : PageModel
    {
        // CONEXI�N A LA BASE DE DATOS
        string connectionString = "Data Source=DESKTOP-VCG45TQ\\SQLEXPRESS;Initial Catalog=ICBF;Integrated Security=True;";

        // Lista para almacenar la informaci�n de los usuarios
        public List<UsuarioInfo> listUsuarios = new List<UsuarioInfo>();

        // M�todo GET para cargar la lista de usuarios
        public void OnGet()
        {
            LoadUsuarios();
        }

        // M�todo para cargar la lista de usuarios desde la base de datos
        private void LoadUsuarios()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlSelect = @"SELECT u.pkIdUsuario, u.identificacion, u.nombre, u.fechaNacimiento, 
                                        u.telefono, u.correo, u.direccion, r.tipo AS nombre_rol, 
                                        td.tipo AS nombre_tipo_doc
                                        FROM usuarios u
                                        INNER JOIN roles r ON u.fkIdRol = r.pkIdRol
                                        INNER JOIN tipoDoc td ON u.fkIdTipoDoc = td.pkIdTipoDoc";

                    using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UsuarioInfo usuarioInfo = new UsuarioInfo
                                {
                                    pkIdUsuario = reader.GetInt32(0).ToString(),
                                    identificacion = reader.GetString(1),
                                    nombre = reader.GetString(2),
                                    fechaNacimiento = reader.GetDateTime(3),
                                    telefono = reader.GetString(4),
                                    correo = reader.GetString(5),
                                    direccion = reader.GetString(6),
                                    nombre_rol = reader.GetString(7),
                                    nombre_tipo_doc = reader.GetString(8)
                                };

                                listUsuarios.Add(usuarioInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
        }

        // M�todo POST para generar el PDF
        public IActionResult OnPostGeneratePDF()
        {
            // Recargar los datos de los usuarios
            LoadUsuarios();
            List<UsuarioInfo> usuarios = listUsuarios;

            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                document.Add(new Paragraph("Lista de Usuarios"));

                PdfPTable table = new PdfPTable(8);
                table.AddCell("Identificaci�n");
                table.AddCell("Nombre");
                table.AddCell("Fecha de Nacimiento");
                table.AddCell("Tel�fono");
                table.AddCell("Correo");
                table.AddCell("Direcci�n");
                table.AddCell("Rol");
                table.AddCell("Tipo de Documento");

                foreach (var usuario in usuarios)
                {
                    table.AddCell(usuario.identificacion);
                    table.AddCell(usuario.nombre);
                    table.AddCell(usuario.fechaNacimiento.ToShortDateString());
                    table.AddCell(usuario.telefono);
                    table.AddCell(usuario.correo);
                    table.AddCell(usuario.direccion);
                    table.AddCell(usuario.nombre_rol);
                    table.AddCell(usuario.nombre_tipo_doc);
                }

                document.Add(table);

                document.Close();
                writer.Close();

                return File(ms.ToArray(), "application/pdf", "UsuariosReporte.pdf");
            }
        }

        // Clase para representar la informaci�n de cada usuario
        public class UsuarioInfo
        {
            public string pkIdUsuario { get; set; }
            public string identificacion { get; set; }
            public string nombre { get; set; }
            public DateTime fechaNacimiento { get; set; }
            public string telefono { get; set; }
            public string correo { get; set; }
            public string direccion { get; set; }
            public string nombre_rol { get; set; }
            public string nombre_tipo_doc { get; set; }
        }
    }
}
