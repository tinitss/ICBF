namespace ICBFApp
{
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using Microsoft.AspNetCore.Mvc;
    using System.IO;

    public class PDFController : Controller
    {
        public IActionResult CreatePDF()
        {
            // Define el documento y el escritor de PDF
            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                // Añade contenido al documento
                document.Add(new Paragraph("Este es un ejemplo de reporte en PDF"));
                document.Add(new Paragraph("Generado usando iTextSharp en ASP.NET Core"));

                // Cierra el documento
                document.Close();
                writer.Close();

                // Devuelve el archivo PDF
                return File(ms.ToArray(), "application/pdf", "reporte.pdf");
            }
        }
    }

}
