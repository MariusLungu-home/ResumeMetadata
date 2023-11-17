using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using PdfSharp.Pdf.IO;
using ResumeMetadataLibrary.Services;
using PdfSharp.Fonts;
using ResumeMetadataLibrary.Resolvers;

public class Utilities : IUtilities
{

    private static bool _fontResolverSet = false;

    public static void InitializeFontResolver()
    {
        if (!_fontResolverSet)
        {
            GlobalFontSettings.FontResolver = new CustomFontResolver();
            _fontResolverSet = true;
        }
    }

    public Task<Stream> InsertMetadataDocx(Stream source, string metaDataToInsert)
    {
        // Open the Word document
        using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(source, true))
        {

            // Get the main document part
            MainDocumentPart mainPart = wordDocument.MainDocumentPart 
                ?? throw new ArgumentNullException("The document has no body.");

            // Create a new table with one row and two columns
            Table table = new Table(
                        new TableProperties(
                            new TableBorders(
                                new InsideHorizontalBorder(),
                                new InsideVerticalBorder()
                            )
                        ),
                        new TableRow(
                            new TableCell(
                                new TableCellProperties(
                                    new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = "1440" } // 1 cm
                                ),
                                new Paragraph(
                                    new Run(
                                        new RunProperties(
                                            new FontSize { Val = "1" },
                                            new Color { Val = "FFFFFF" }
                                        ),
                                        new Text("Résumé Metadata")
                                    )
                                )
                            ),
                            new TableCell(
                                new TableCellProperties(
                                    new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = "28800" } // 20 cm
                                ),
                                new Paragraph(
                                    new Run(
                                        new RunProperties(
                                            new FontSize { Val = "1" },
                                            new Color { Val = "FFFFFF" }
                                        ),
                                        new Text(metaDataToInsert)
                                    )
                                )
                            )
                        ));

            // Append the table to the document body
            mainPart.Document.Body?.AppendChild(table);

            // Iterate through the table cells and set spacing after to zero for paragraphs
            foreach (var row in table.Elements<TableRow>())
            {
                foreach (var cell in row.Elements<TableCell>())
                {
                    foreach (var paragraph in cell.Elements<Paragraph>())
                    {
                        var paragraphProperties = paragraph.Elements<ParagraphProperties>().FirstOrDefault();
                        if (paragraphProperties == null)
                        {
                            paragraphProperties = new ParagraphProperties();
                            paragraph.PrependChild(paragraphProperties); // Prepend to add before the run
                        }

                        var spacingBetweenLines = paragraphProperties.Elements<SpacingBetweenLines>().FirstOrDefault();
                        if (spacingBetweenLines == null)
                        {
                            spacingBetweenLines = new SpacingBetweenLines { After = "0" };
                            paragraphProperties.Append(spacingBetweenLines);
                        }
                        else
                        {
                            spacingBetweenLines.After = "0";
                        }
                    }
                }
            }

            // Find the paragraph immediately before the table and remove it
            var carriageReturnPrecedingTable = table.Parent.ChildElements[table.Parent.ChildElements.Count - 3];
            if (carriageReturnPrecedingTable.LocalName == "p")
                table.Parent.ChildElements[table.Parent.ChildElements.Count - 3].Remove();

            // Save the document
            mainPart.Document.Save();
        }

        return Task.Run(() => source);
    }

    public Task<Stream> InsertMetadataPdf(Stream source, string metaDataToInsert)
    {
        InitializeFontResolver();

        PdfDocument document = PdfReader.Open(source, PdfDocumentOpenMode.Modify);
        PdfPage page = document.Pages[0];
        XGraphics gfx = XGraphics.FromPdfPage(page);
        XFont font = new XFont("Arial", 0.01, XFontStyleEx.Regular);
        XBrush brush = new XSolidBrush(XColor.FromArgb(1, 1, 1, 1)); // Transparent brush

        gfx.DrawString(metaDataToInsert, font, brush, new XRect(0, 0, page.Width, page.Height), XStringFormats.BottomRight);


        document.Save(source);
        document.Close();

        return Task.Run(() => source);
    }
}
