

using Azure;
using Azure.AI.OpenAI;
using Azure.AI.Projects;
using Azure.AI.Inference;
using Azure.Identity;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using OpenAI.Chat;

var connectionString = "eastus2.api.azureml.ms;d2e83b94-8a5a-47f2-88a5-0db8812b6a3b;rg-jomatthiai;jomatthi-samples";
var projectClient = new AIProjectClient(connectionString, new DefaultAzureCredential());

var connections = projectClient.GetConnectionsClient();
var connection = connections.GetDefaultConnection(ConnectionType.AzureAISearch, withCredential: true).Value;

var properties = connection.Properties as ConnectionPropertiesApiKeyAuth;
if (properties == null) {
    throw new Exception("Invalid auth type, expected API key auth");
}

SearchClient searchClient = new SearchClient(
    new Uri(properties.Target),
    "products",
    new AzureKeyCredential(properties.Credentials.Key));

    var embeddingsClient = projectClient.GetInferenceClient().GetEmbeddingsClient();

var indexClient = new SearchIndexClient(
        new Uri(properties.Target),
        new AzureKeyCredential(properties.Credentials.Key));

void CreateIndexDefinition(string indexName, string model)
{
    FieldBuilder fieldBuilder = new FieldBuilder();
    var searchFields = fieldBuilder.Build(typeof(SearchData));

    var definition = new SearchIndex(indexName, searchFields);
    definition.SemanticSearch = new()
    {
        Configurations = {
            new SemanticConfiguration("default", new() {
                TitleField = new SemanticField("title"),
                ContentFields = { new SemanticField("content") }
            })
        }
    };
    definition.VectorSearch = new()
    {
        Profiles = {
            new VectorSearchProfile("myHnswProfile", "myHnsw")
        },
        Algorithms = 
        {
            new HnswAlgorithmConfiguration("myHnsw")
            {
                Parameters = new HnswParameters()
                {
                    M = 4,
                    EfConstruction = 1000,
                    EfSearch = 1000,
                    Metric = VectorSearchAlgorithmMetric.Cosine
                }
            },
            new ExhaustiveKnnAlgorithmConfiguration("myExhaustiveKnn")
            {
                Parameters = new ExhaustiveKnnParameters()
                {
                    Metric = VectorSearchAlgorithmMetric.Cosine
                }
            }
        }
    };

    indexClient.CreateOrUpdateIndex(definition);
}

/* 
The fields we want to index. The "ContentVector" field is a vector field that will
be used for vector search.
*/
class SearchData 
{
    [SimpleField(IsKey = true)]
    public string Id { get; set; }
    
    [SearchableField]
    public string Content { get; set; }

    [SimpleField]
    public string FilePath { get; set; }

    [SearchableField]
    public string Title { get; set; }

    [SimpleField]
    public string Url { get; set; }
    
    [VectorSearchField(VectorSearchProfileName = "myHnswProfile",
        VectorSearchDimensions = 3072)] // Use 1536 when using text-embedding-ada-002
    public IReadOnlyList<float> ContentVector { get; set; }

}
/*

def create_index_definition(index_name: str, model: str) -> SearchIndex:
    dimensions = 1536  # text-embedding-ada-002
    if model == "text-embedding-3-large":
        dimensions = 3072

    # The fields we want to index. The "embedding" field is a vector field that will
    # be used for vector search.
    fields = [
        SimpleField(name="id", type=SearchFieldDataType.String, key=True),
        SearchableField(name="content", type=SearchFieldDataType.String),
        SimpleField(name="filepath", type=SearchFieldDataType.String),
        SearchableField(name="title", type=SearchFieldDataType.String),
        SimpleField(name="url", type=SearchFieldDataType.String),
        SearchField(
            name="contentVector",
            type=SearchFieldDataType.Collection(SearchFieldDataType.Single),
            searchable=True,
            # Size of the vector created by the text-embedding-ada-002 model.
            vector_search_dimensions=dimensions,
            vector_search_profile_name="myHnswProfile",
        ),
    ]

    # The "content" field should be prioritized for semantic ranking.
    semantic_config = SemanticConfiguration(
        name="default",
        prioritized_fields=SemanticPrioritizedFields(
            title_field=SemanticField(field_name="title"),
            keywords_fields=[],
            content_fields=[SemanticField(field_name="content")],
        ),
    )

    # For vector search, we want to use the HNSW (Hierarchical Navigable Small World)
    # algorithm (a type of approximate nearest neighbor search algorithm) with cosine
    # distance.
    vector_search = VectorSearch(
        algorithms=[
            HnswAlgorithmConfiguration(
                name="myHnsw",
                kind=VectorSearchAlgorithmKind.HNSW,
                parameters=HnswParameters(
                    m=4,
                    ef_construction=1000,
                    ef_search=1000,
                    metric=VectorSearchAlgorithmMetric.COSINE,
                ),
            ),
            ExhaustiveKnnAlgorithmConfiguration(
                name="myExhaustiveKnn",
                kind=VectorSearchAlgorithmKind.EXHAUSTIVE_KNN,
                parameters=ExhaustiveKnnParameters(metric=VectorSearchAlgorithmMetric.COSINE),
            ),
        ],
        profiles=[
            VectorSearchProfile(
                name="myHnswProfile",
                algorithm_configuration_name="myHnsw",
            ),
            VectorSearchProfile(
                name="myExhaustiveKnnProfile",
                algorithm_configuration_name="myExhaustiveKnn",
            ),
        ],
    )

    # Create the semantic settings with the configuration
    semantic_search = SemanticSearch(configurations=[semantic_config])

    # Create the search index definition
    return SearchIndex(
        name=index_name,
        fields=fields,
        semantic_search=semantic_search,
        vector_search=vector_search,
    )*/