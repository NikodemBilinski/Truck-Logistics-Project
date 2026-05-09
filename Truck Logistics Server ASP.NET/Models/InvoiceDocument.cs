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
                page.Margin(50);
                page.Header().Height(100).Background(Colors.Grey.Lighten1);
                page.Content().Background(Colors.Grey.Lighten3);
                page.Footer().Height(50).Background(Colors.Grey.Lighten1).Row(row => 
                {
                        row.RelativeItem().Text(text =>
                        {
                            text.Span("Invoice ID: ").SemiBold();
                            text.Span(_invoice.ID.ToString());
                        });
                        row.RelativeItem().Text(text =>
                        {
                            text.Span("Issue Date: ").SemiBold();
                            text.Span(_invoice.IssueDate.ToShortDateString());
                        });
                        row.RelativeItem().Text(text =>
                        {
                            text.Span("Due Date: ").SemiBold();
                            text.Span(_invoice.DueDate.ToShortDateString());
                        });
                        row.RelativeItem().Text(text =>
                        {
                            text.Span("Client Name: ").SemiBold();
                            text.Span(_invoice.Client.Name);
                        });

                });


            }
            );
        }
    }

}
