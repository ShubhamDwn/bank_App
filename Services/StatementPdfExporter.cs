using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using bank_demo.ViewModels.FeaturesPages;
using bank_demo.Services;


public static class StatementPdfExporter
{
    public static byte[] GeneratePdf(List<TransactionModel> transactions, string accountType, DateTime from, DateTime to)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.PageColor(QuestPDF.Helpers.Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Text($"Bank Statement - {accountType}")
                             .FontSize(20).Bold().AlignCenter();

                page.Content().Element(c =>
                {
                    c.Column(col =>
                    {
                        col.Spacing(5);

                        col.Item().Text($"From: {from:dd-MM-yyyy}  To: {to:dd-MM-yyyy}").Bold();

                        foreach (var txn in transactions)
                        {
                            col.Item().Row(row =>
                            {
                                row.RelativeItem(4).Text(txn.Description);
                                row.RelativeItem(2).Text(txn.Date.ToShortDateString());
                                row.RelativeItem(2).Text(txn.Amount.ToString("C"));
                            });
                        }
                    });
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Generated on: ");
                    x.Span(DateTime.Now.ToString("dd MMM yyyy")).SemiBold();
                });
            });
        }).GeneratePdf();
    }
}
