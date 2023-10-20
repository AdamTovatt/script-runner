using System.Text.Json.Serialization;

namespace ScriptRunner.OpenAi.Models.Tuning
{
    public class ModelInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("root")]
        public string Root { get; set; }
        [JsonPropertyName("parent")]
        public string? Parent { get; set; }
        [JsonPropertyName("owned_by")]
        public string Owner { get; set; }
        [JsonPropertyName("permission")]
        public List<ModelPermission> Permissions { get; set; }

        public ModelInfo(string id, string root, string? parent, string owner)
        {
            Id = id;
            Root = root;
            Owner = owner;
            Parent = parent;
            Permissions = new List<ModelPermission>();
        }

        public override string ToString()
        {
            return Id;
        }
    }
}
