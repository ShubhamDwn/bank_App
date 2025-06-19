using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using bank_demo.Services.API;
using System;
using System.Collections.Generic;
using System.IO;

namespace bank_demo.Services
{
    public static class StatementPdfExporter
    {
        public static byte[] GenerateStatementPdf(
            List<TransactionModel> transactions,
            string customerName,
            string accountNumber,
            string SubSchemeName,
            DateTime fromDate,
            DateTime toDate)
        {
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            page.Orientation = PdfSharpCore.PageOrientation.Landscape;

            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont headerFont = new XFont("Verdana", 14, XFontStyle.Bold);
            XFont cellFont = new XFont("Verdana", 7, XFontStyle.Regular);

            int margin = 30;
            int y = margin;

            // 🔹 Page width and scaling
            double pageWidth = page.Width - 2 * margin;

            // Table headers
            string[] headers = new string[]
            {
                "Date", "Narration", "Deposit", "Withdraw", "Plain", "PlainCr", "PlainDr",
                "Penalty", "PenaltyCr", "PenaltyDr", "Payable", "Receivable", "Dr/Cr", "Balance"
            };

            int[] originalWidths = new int[]
            {
                60, 120, 60, 60, 50, 50, 50,
                60, 60, 60, 60, 60, 40, 60
            };

            int totalOriginalWidth = originalWidths.Sum();
            double scaleFactor = pageWidth / totalOriginalWidth;

            int[] columnWidths = originalWidths
                .Select(w => (int)(w * scaleFactor))
                .ToArray();

            // 🔹 Header Title
            gfx.DrawString("Account Statement", headerFont, XBrushes.Black,
                new XRect(0, y, page.Width, page.Height), XStringFormats.TopCenter);
            y += 30;

            // 🔹 Account Info
            gfx.DrawString($"Name: {customerName}", cellFont, XBrushes.Black,
                new XRect(margin, y, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString($"Account Number: {accountNumber}", cellFont, XBrushes.Black,
                new XRect(margin + 300, y, page.Width, page.Height), XStringFormats.TopLeft);
            y += 15;

            gfx.DrawString($"Account Type: {SubSchemeName}", cellFont, XBrushes.Black,
                new XRect(margin, y, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString($"Period: {fromDate:dd-MM-yyyy} to {toDate:dd-MM-yyyy}", cellFont, XBrushes.Black,
                new XRect(margin + 300, y, page.Width, page.Height), XStringFormats.TopLeft);
            y += 25;

            // 🔹 Table Header
            int x = margin;
            for (int i = 0; i < headers.Length; i++)
            {
                gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, x, y, columnWidths[i], 20);
                gfx.DrawString(headers[i], cellFont, XBrushes.Black,
                    new XRect(x + 2, y + 3, columnWidths[i], 20), XStringFormats.TopLeft);
                x += columnWidths[i];
            }
            y += 22;

            // 🔹 Transaction Rows
            foreach (var txn in transactions)
            {
                x = margin;

                string[] row = new string[]
                {
                    txn.TransactionDate.ToString("dd-MM-yyyy"),
                    txn.Narration,
                    txn.Deposite.ToString(),
                    txn.Withdraw.ToString(),
                    txn.Plain.ToString(),
                    txn.PlainCr.ToString(),
                    txn.PlainDr.ToString(),
                    txn.Penalty.ToString(),
                    txn.PenaltyCr.ToString(),
                    txn.PenaltyDr.ToString(),
                    txn.Payable.ToString(),
                    txn.Receivable.ToString(),
                    txn.DrCr,
                    txn.Balance.ToString()
                };

                for (int i = 0; i < row.Length; i++)
                {
                    gfx.DrawRectangle(XPens.Black, x, y, columnWidths[i], 20);
                    gfx.DrawString(row[i], cellFont, XBrushes.Black,
                        new XRect(x + 2, y + 3, columnWidths[i], 20), XStringFormats.TopLeft);
                    x += columnWidths[i];
                }

                y += 22;

                // 🔁 If page overflow, start a new page
                if (y > page.Height - 40)
                {
                    page = document.AddPage();
                    page.Orientation = PdfSharpCore.PageOrientation.Landscape;
                    gfx = XGraphics.FromPdfPage(page);
                    y = margin;

                    // Reprint header on new page
                    gfx.DrawString("Account Statement", headerFont, XBrushes.Black,
                        new XRect(0, y, page.Width, page.Height), XStringFormats.TopCenter);
                    y += 30;
                }
            }

            using var stream = new MemoryStream();
            document.Save(stream, false);
            return stream.ToArray();
        }

        public static async Task<string> ExportToDownloadAsync(
            List<TransactionModel> transactions,
            string customerName,
            string accountNumber,
            string accountType,
            DateTime fromDate,
            DateTime toDate){
            string fileName = $"{customerName.Replace(" ", "_")}_{accountType}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.pdf"; //{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}_{Date}.pdf";
                string fullPath = GetDownloadPath(fileName);

                var bytes = GenerateStatementPdf(transactions, customerName, accountNumber, accountType, fromDate, toDate);
                File.WriteAllBytes(fullPath, bytes);

                return fullPath;
        }

        private static string GetDownloadPath(string fileName)
        {
            #if ANDROID
                // For API 29+ (Scoped Storage), consider using MediaStore
                return Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath, fileName);
            #elif WINDOWS
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", fileName);
            #elif IOS
                        return Path.Combine(FileSystem.AppDataDirectory, fileName); // Limited sandbox access
            #else
                return Path.Combine(FileSystem.AppDataDirectory, fileName); // Fallback
            #endif
        }

    }
}
