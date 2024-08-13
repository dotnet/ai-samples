#pragma warning disable
using System.Numerics.Tensors;
using System.Text.Json;
using Azure.AI.OpenAI;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Actions;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;

public class ManualIngestor
{
    record ParseResult(PageContent[] Pages);
    record PageContent(int Page, string Text);
    private readonly ITextEmbeddingGenerationService _embeddingService;

    public ManualIngestor(ITextEmbeddingGenerationService embeddingService)
    {
        _embeddingService = embeddingService;
    }

    public async Task RunAsync(string sourceDir, string outputDir)
    {
        Console.WriteLine("Ingesting manuals...");


        // Load data
        var chunks = new List<ManualChunk>();
        var paragraphIndex = 0;

        var files = Directory.GetFiles(sourceDir, "*.pdf");
        
        // Loop over each PDF file
        foreach(var file in files)
        {
            Console.WriteLine($"Generating chunks for {file}...");

            var docId = int.Parse(Path.GetFileNameWithoutExtension(file));

            // Loop over each page
            var pdf = PdfDocument.Open(file);
            var pages = pdf.GetPages();
            
            foreach (var page in pages)
            {
                // [1] Parse (PDF page -> string)
                var pageText = GetPageText(page);

                // [2] Chunk (split into shorter strings on natural boundaries)
                var paragraphs = TextChunker.SplitPlainTextParagraphs([page.Text], 200);

                // [3] Embed (string -> embedding)
                var paragraphsWithEmbeddings = paragraphs.Zip(await _embeddingService.GenerateEmbeddingsAsync(paragraphs));

                // [4] Save
                chunks.AddRange(paragraphsWithEmbeddings.Select(p => new ManualChunk
                {
                    ProductId = docId,
                    PageNumber = page.Number,
                    ChunkId = ++paragraphIndex,
                    Text = p.First,
                    Embedding = p.Second.ToArray()
                }));           
            }
        }
        var outputOptions = new JsonSerializerOptions { WriteIndented = true };
        var content = JsonSerializer.Serialize(chunks, outputOptions);
        await File.WriteAllTextAsync(Path.Combine(outputDir,"manual-chunks.json"), content);
        Console.WriteLine($"Wrote {chunks.Count} manual chunks");
    }

    private static string GetPageText(Page pdfPage)
    {
        var letters = pdfPage.Letters;
        var words = NearestNeighbourWordExtractor.Instance.GetWords(letters);
        var textBlocks = DocstrumBoundingBoxes.Instance.GetBlocks(words);
        return string.Join(Environment.NewLine + Environment.NewLine,
            textBlocks.Select(t => t.Text.ReplaceLineEndings(" ")));
    }
}