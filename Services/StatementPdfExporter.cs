using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using bank_demo.Services.API;
using System.Collections.Generic;
using System.IO;

namespace bank_demo.Services
{
    public static class StatementPdfExporter
    {
        public static byte[] GenerateStatementPdf(List<TransactionModel> transactions)
        {
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("Verdana", 10, XFontStyle.Regular);

            int y = 40;
            gfx.DrawString("Account Statement", new XFont("Verdana", 14, XFontStyle.Bold), XBrushes.Black, new XRect(0, y, page.Width, page.Height), XStringFormats.TopCenter);
            y += 30;

            foreach (var txn in transactions)
            {
                string line = $"{txn.TransactionDate:dd-MM-yyyy} | {txn.Narration} | D: {txn.Deposite} | W: {txn.Withdraw} | Bal: {txn.Balance}";
                gfx.DrawString(line, font, XBrushes.Black, new XRect(40, y, page.Width - 80, page.Height), XStringFormats.TopLeft);
                y += 20;

                if (y > page.Height - 40)
                {
                    page = document.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    y = 40;
                }
            }

            using (MemoryStream stream = new MemoryStream())
            {
                document.Save(stream, false);
                return stream.ToArray();
            }
        }
    }
}
