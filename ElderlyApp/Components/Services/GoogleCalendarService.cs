using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class GoogleCalendarService
{
    private static readonly string[] Scopes = { CalendarService.Scope.Calendar };
    private const string ApplicationName = "ElderlyApp";

    // Hardcoded valid IANA time zone (e.g., "UTC" or "America/New_York")
    private const string HardcodedTimeZone = "UTC";

    /// <summary>
    /// Adds an event to Google Calendar and sends email invites to attendees.
    /// </summary>
    public async Task<Event> AddEventAndSendEmail(
        string title,
        string description,
        DateTime start,
        DateTime end,
        List<string> attendeeEmails,
        string userEmail)
    {
        try
        {
            // Path to client_secret.json
            string clientSecretPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "client_secret.json");

            if (!File.Exists(clientSecretPath))
            {
                throw new FileNotFoundException($"The client secret file was not found at: {clientSecretPath}");
            }

            // Authenticate using OAuth 2.0
            UserCredential credential;
            using (var stream = new FileStream(clientSecretPath, FileMode.Open, FileAccess.Read))
            {
                string credPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "token.json");
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true));
            }

            // Initialize the Calendar API
            var service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Create the event
            var newEvent = new Event
            {
                Summary = title,
                Description = description,
                Start = new EventDateTime
                {
                    DateTime = start,
                    TimeZone = HardcodedTimeZone, // Use hardcoded time zone
                },
                End = new EventDateTime
                {
                    DateTime = end,
                    TimeZone = HardcodedTimeZone, // Use hardcoded time zone
                },
                Attendees = attendeeEmails.Select(email => new EventAttendee { Email = email }).ToList()
            };

            // Insert event into Google Calendar
            var request = service.Events.Insert(newEvent, "primary");
            request.SendUpdates = EventsResource.InsertRequest.SendUpdatesEnum.All; // Notify attendees
            Event createdEvent = await request.ExecuteAsync();

            Console.WriteLine("Event created successfully.");

            // Send confirmation email
            Console.WriteLine($"Sending email to {userEmail}...");
            await SendConfirmationEmail(userEmail, start);
            Console.WriteLine("Email sent successfully.");

            return createdEvent;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating event or sending email: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Sends a confirmation email to the user.
    /// </summary>
    private async Task SendConfirmationEmail(string userEmail, DateTime start)
    {
        // Add your email sending logic here (SMTP or other service)
        Console.WriteLine($"Confirmation email sent to {userEmail} for event starting at {start}.");
    }

    /// <summary>
    /// Retrieves upcoming events from Google Calendar.
    /// </summary>
    public async Task<List<Event>> GetUpcomingEventsAsync()
    {
        try
        {
            // Path to the client_secret.json file
            string clientSecretPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "client_secret.json");

            if (!File.Exists(clientSecretPath))
            {
                throw new FileNotFoundException($"The client secret file was not found at: {clientSecretPath}");
            }

            // Authenticate using OAuth 2.0
            UserCredential credential;
            using (var stream = new FileStream(clientSecretPath, FileMode.Open, FileAccess.Read))
            {
                string credPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "token.json");
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true));
            }

            // Initialize the Google Calendar API
            var service = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Fetch upcoming events
            var request = service.Events.List("primary");
            request.TimeMin = DateTime.UtcNow; // Fetch events starting from now
            request.ShowDeleted = false;
            request.SingleEvents = true; // Expand recurring events into single instances
            request.MaxResults = 20; // Fetch up to 20 events
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            var events = await request.ExecuteAsync();
            return events.Items?.ToList() ?? new List<Event>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching events: {ex.Message}");
            return new List<Event>(); // Return an empty list on error
        }
    }
}
