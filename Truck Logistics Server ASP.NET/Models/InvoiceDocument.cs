using TrucksLogisticsServerAPI.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace TrucksLogisticsServerAPI.Models
{

    

    public class InvoiceDocument : IDocument
    {
        private readonly Invoice _invoice;

        public InvoiceDocument(Invoice invoice)
        {
            _invoice = invoice;
            
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(11));

                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Images", "tes-skyrim-icon-16.png");
                page.Background().AlignBottom().Image(imagePath, ImageScaling.FitArea);

                page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text($"Invoice {_invoice.ID}/{_invoice.IssueDate.Year}").Bold().FontSize(20);
                                c.Item().PaddingTop(4).Text($"Status: {_invoice.Status.ToUpper()}");
                            });

                            row.RelativeItem().AlignRight().Column(c =>
                            {
                                c.Item().Text($"Issue Date: {_invoice.IssueDate.ToShortDateString()}");
                                c.Item().Text($"Due Date: {_invoice.DueDate.ToShortDateString()}");
                            });
                        });

                        col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                    });

                // kolumna w kolumnieeeee crazy bro
                page.Content().Column(col =>
                {
                    col.Item().Row(row =>
                    {

                    });
                    col.Item().PaddingTop(20).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                    col.Item().PaddingTop(20).Column(col2 =>
                    {
                        col2.Item().Text("Job Details:").Bold().FontSize(12);
                        col2.Item().PaddingTop(4).Text($"Job: {_invoice.Job.Name}");
                        col2.Item().Text($"Company Name: {_invoice.Job.CompanyName}");
                        col2.Item().Text($"Route: {_invoice.Job.LocationFrom} -> {_invoice.Job.LocationTo}");
                        col2.Item().Text($"Description: {_invoice.Job.Description}");
                    });

                    col.Item().PaddingTop(20).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                    col.Item().PaddingTop(20).Table(table =>
                    {
                        table.ColumnsDefinition(tcol =>
                        {
                            tcol.RelativeColumn(4);
                            tcol.RelativeColumn(2);
                            tcol.RelativeColumn(2);
                            tcol.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Description").ExtraBold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Netto").ExtraBold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text($"VAT ({_invoice.VatRate}%)").ExtraBold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Brutto").ExtraBold();
                        });

                        table.Cell().Background(Colors.LightBlue.Medium).Padding(5).Text($"Transportation Service - {_invoice.Job.Name}");
                        table.Cell().Background(Colors.LightBlue.Medium).Padding(5).Text($"{_invoice.NetAmount:F2} zł");
                        table.Cell().Background(Colors.LightBlue.Medium).Padding(5).Text($"{_invoice.GrossAmount - _invoice.NetAmount} zł");
                        table.Cell().Background(Colors.LightBlue.Medium).Padding(5).Text($"{_invoice.GrossAmount:F2} zł");
                    });

                    col.Item().PaddingTop(20).Text($"Net Amount: {_invoice.NetAmount:F2} zł");
                    col.Item().Text($"$VAT ({_invoice.VatRate}%): {_invoice.GrossAmount - _invoice.NetAmount:F2} zł");
                    col.Item().PaddingTop(4).Text($"Total Amount: {_invoice.GrossAmount:F2} zł").Bold().FontSize(14);
                });

                page.Footer().AlignCenter().Text(fut =>
                {
                    fut.Span("Page ");
                    fut.CurrentPageNumber();
                    fut.Span(" of ");
                    fut.TotalPages();
                });

            }
            );
        }
    }

}
