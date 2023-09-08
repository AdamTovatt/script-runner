using ScriptRunner;
using ScriptRunner.DocumentationAttributes;
using ScriptRunner.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SkippyBackend.PrecompiledScripts
{
    public class ShowCameraImageScript : CompiledScript
    {
        public ShowCameraImageScript(ScriptContext context) : base(context) { }

        [ScriptStart]
        [Parameter("cameraId", "The id of the camera to show a picture from. The value 1 represents the camera that points out the window. The value 3 represents the camera that points inside the room and on the plants. ")]
        public async Task<string> ShowCameraImage(int cameraId)
        {
            string url = "https://sakurapi.se/camera-server/camera/list";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    List<Camera>? cameras = JsonSerializer.Deserialize<List<Camera>>(responseBody);

                    if(cameras == null || cameras.Count == 0)   return "No cameras found in answer from api. ";

                    Camera? camera = cameras.SingleOrDefault(x => x.Id == cameraId);

                    if (camera == null) return $"No camera with id {cameraId} was found in the respose from the api. ";
                    if (string.IsNullOrEmpty(camera.Preview)) return "The camera preview was null";

                    Context.Conversation.AddSystemMessage("[file content]");
                    Context.Conversation.Communicator.InvokeOnFileWasSent(this, Convert.FromBase64String(camera.Preview), ContentType.Image, "camera_image.png");
                    return "The image was displayed to the user in the previous message, the user should be informed. ";
                }
                else
                {
                    return "There was an error when reading the camera images from the camera api. ";
                }
            }
        }

        public class Camera
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("name")]
            public string? Name { get; set; }

            [JsonPropertyName("description")]
            public string? Description { get; set; }

            [JsonPropertyName("lastActive")]
            public DateTime LastActive { get; set; }

            [JsonPropertyName("preview")]
            public string? Preview { get; set; }
        }
    }
}
