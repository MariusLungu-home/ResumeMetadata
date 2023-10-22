// Replace "path_to_existing_document.docx" with the actual path to your existing Word document. This code will open the document, insert the specified table, and save it.
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ResumeMetadataLibrary;

public class Utilities : IUtilities
{
    public Task<Stream> InsertMetadata(Stream source, string metaDataToInsert)
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
}
