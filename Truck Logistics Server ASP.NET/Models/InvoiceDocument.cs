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

                page.Content();
                page.Footer();


            }
            );
        }
    }

}
