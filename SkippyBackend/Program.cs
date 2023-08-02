using SkippyBackend.Hubs.SignalRWebpack;

namespace SkippyBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSignalR(options => options.MaximumParallelInvocationsPerClient = 2);

            builder.Services.AddCors();

            WebApplication app = builder.Build();

            app.UseCors(options =>
            {
                options.AllowAnyHeader()
                       .AllowAnyMethod()
                       .SetIsOriginAllowed(origin => true) // Allow any origin
                       .AllowCredentials(); // Allow credentials (e.g., cookies)
            });

            app.MapHub<ChatHub>("/hub");

            app.Run();
        }
    }
}