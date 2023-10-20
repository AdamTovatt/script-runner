using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScriptRunner.OpenAi.Models.Tuning
{
    public class TuningJobResponse : OpenAiApiResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("model")]
        public string Model { get; set; }
        [JsonPropertyName("created_at")]
        public long CreatedAt { get; set; }
        [JsonPropertyName("finished_at")]
        public long? FinishedAt { get; set; }
        [JsonPropertyName("fine_tuned_model")]
        public string? FineTunedModel { get; set; }
        [JsonPropertyName("organization_id")]
        public string OrganizationId { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("validation_file")]
        public string? ValidationFile { get; set; }
        [JsonPropertyName("training_file")]
        public string TrainingFile { get; set; }
        [JsonPropertyName("trained_tokens")]
        public int? TrainedTokens { get; set; }

        public TuningJobResponse(string id, string model, long createdAt, long? finishedAt, string? fineTunedModel, string organizationId, string status, string? validationFile, string trainingFile, int? trainedTokens)
        {
            Id = id;
            Model = model;
            CreatedAt = createdAt;
            FinishedAt = finishedAt;
            FineTunedModel = fineTunedModel;
            OrganizationId = organizationId;
            Status = status;
            ValidationFile = validationFile;
            TrainingFile = trainingFile;
            TrainedTokens = trainedTokens;
        }

        public static TuningJobResponse FromJson(string json)
        {
            TuningJobResponse? result = JsonSerializer.Deserialize<TuningJobResponse>(json);

            if (result == null) throw new JsonException($"Could not deserialize typeof @{typeof(TuningJobResponse)} from json with lengt {json.Length}: {json}");
            return result;
        }
    }
}
