using Google.Apis.Calendar.v3;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace ElderlyApp
{
    public static class MauiProgram
    {
        [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddSingleton<CalendarService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // Configuration for SMTP settings (environment variables are safer)
            builder.Services.AddSingleton<EmailSettings>(new EmailSettings
            {
                FromEmail = "elderlyapp4@gmail.com", // Replace with your Gmail address
                FromPassword = Environment.GetEnvironmentVariable("GMAIL_APP_PASSWORD") ?? "fcsk buzh sotz fwaa" // Use environment variable or fallback value
            });

            // Return the builder
            return builder.Build();
        }
    }

    // Custom class to hold SMTP Email settings
    public class EmailSettings
    {
        public string FromEmail { get; set; } = "elderlyapp4@gmail.com";
        public string FromPassword { get; set; } = "fcsk buzh sotz fwaa";
    }
}
