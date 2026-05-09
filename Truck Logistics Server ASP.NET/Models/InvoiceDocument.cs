using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace TrucksLogisticsServerAPI.Models
{
    public class InvoiceDocument : IDocument
    { 
        public void Compose(IDocumentContainer container)
        {
            var document = Document.Create(container =>
             {
                 container.Page(page =>
                 {
                     page.Header().Text("Invoice").SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);
                     page.Content().Text("This is the content of the invoice.");
                     page.Footer().AlignCenter().Text(x =>
                     {
                         x.Span("Page ");
                         x.CurrentPageNumber();
                         x.Span(" of ");
                         x.TotalPages();
                     });
                 });
             });

        }


    }

}
