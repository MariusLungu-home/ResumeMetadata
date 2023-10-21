// Replace "path_to_existing_document.docx" with the actual path to your existing Word document. This code will open the document, insert the specified table, and save it.
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ResumeMetadataLibrary;

public class Utilities : IUtilities
{
    public Utilities()
    {
        
    }
    /// <summary>Create a copy of a doc, insert the new metadata into a table at the end of the doc.</summary>
    public string InsertMetadata(String sourcePath, String destinationPath, String metaDataToInsert)
    {
        // TODO: validate input
        // TODO: Use stream instead of file path
        // TODO: Return the stream and the message
        bool isFirstTime = true;
        while (!System.IO.File.Exists(path:sourcePath))
        {
            if (!isFirstTime) Console.WriteLine("Invalid path. Please try again.");
            isFirstTime = false;
            sourcePath = Console.ReadLine() ?? string.Empty;
        }

        System.IO.File.Copy(sourcePath, destinationPath, true); // Overwrite existing file if it exists

        // Open the Word document
        using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(destinationPath, true))
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
            
            // Open the Word document
            OpenWordDocument(destinationPath);
        }

        return $"New document created! Warning: A small white (non-visible) object was appended to the end of your document. " +
                  $"Please check that this has not caused an additional page to be added to your document. " +
                  $"The document was saved here: '{destinationPath}'." +
                    "Metadata Insertion Tool";
    }

    public WordprocessingDocument OpenWordDocument(string documentPath)
    {
        try
        {
            // Open the Word document at the specified path for reading and writing
            return WordprocessingDocument.Open(documentPath, true);
        }
        catch (Exception)
        {
            // Swallow exception
            return null;
        }
    }

    /// <summary>Combines the new filename with the directory of the old path filename to create a new path. 
    /// Returns the new path if it's valid, or null if not valid.</summary>
    /// <param name="oldPathFilename">The original path and filename.</param>
    /// <param name="newFilename">The new filename to be combined with the directory of the old path.</param>
    /// <returns>The new path if valid; otherwise, null.</returns>
    public string CombinePathIfValid(string oldPathFilename, string newFilename)
    {
        try
        {
            // Check if the oldPathFilename is a valid file path.
            if (!string.IsNullOrWhiteSpace(oldPathFilename) && File.Exists(oldPathFilename))
            {
                // Get the directory of the old path.
                string directory = Path.GetDirectoryName(oldPathFilename);

                // Combine the directory with the new filename to create the new path.
                string newPath = Path.Combine(directory, newFilename);

                // Check if the new path is valid.
                if (!string.IsNullOrWhiteSpace(newPath))
                {
                    return newPath;
                }
            }
        }
        catch (Exception)
        {
            // Handle any exceptions that might occur during the process.
            // For simplicity, you can log the exception or handle it as needed.
        }

        return null; // Return null if the operation is not successful.
    }
}
