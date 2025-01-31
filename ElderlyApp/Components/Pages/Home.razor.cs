using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace ElderlyApp.Components.Pages
{
    public partial class Home
    {
        [Inject]
        private EmailSettings EmailSettings { get; set; } = default!;

        public string UserEmail { get; set; } = string.Empty; // Stores user's email input
        public string Message { get; set; } = string.Empty;   // Stores success or error messages
        public DateTime? AppointmentDate { get; set; }        // Stores appointment date input
        public TimeOnly? AppointmentTime { get; set; }        // Stores appointment time input

        // Asynchronous method to handle the appointment submission
        private async Task SubmitAppointment()
        {
            if (string.IsNullOrWhiteSpace(UserEmail) || AppointmentDate == null || AppointmentTime == null)
            {
                Message = "Please fill out all fields.";
                return;
            }

            DateTime appointmentDateTime = AppointmentDate.Value.Date.Add(AppointmentTime.Value.ToTimeSpan());
            DateTime endDateTime = appointmentDateTime.AddHours(1); // Default 1-hour duration

            try
            {
                var googleCalendarService = new GoogleCalendarService();
                await googleCalendarService.AddEventAndSendEmail(
                    "ElderlyApp Appointment",
                    "Your scheduled appointment.",
                    appointmentDateTime,
                    endDateTime,
                    new List<string> { UserEmail }, // Add attendees here
                    UserEmail // Email address to send the confirmation
                );

                Message = "Appointment scheduled successfully, and an invite has been sent!";
            }
            catch (Exception ex)
            {
                Message = $"Failed to schedule the appointment. Error: {ex.Message}";
            }
        }


        // Helper method to send the email confirmation
        private async Task SendConfirmationEmail(string userEmail, DateTime appointmentDateTime)
        {
            try
            {
                string fromEmail = EmailSettings.FromEmail;
                string fromPassword = EmailSettings.FromPassword;

                // Set up the SMTP client using Gmail's settings
                using var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(fromEmail, fromPassword),
                    EnableSsl = true,
                };

                // Create the email message
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = "ElderlyApp Appointment Confirmation",
                    Body = $"Hello,\n\nThank you for scheduling an appointment.\n\n" +
                           $"Appointment Details:\nDate: {appointmentDateTime:MMMM dd, yyyy}\n" +
                           $"Time: {appointmentDateTime:hh\\:mm tt}.\n\n" +
                           $"Best regards,\nThe ElderlyApp Team.",
                    IsBodyHtml = false,
                };

                // Add the recipient (the user who submitted their email)
                mailMessage.To.Add(userEmail);

                // Send the email asynchronously
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email. Error: {ex.Message}");
                throw; // Rethrow exception to handle it in the parent method
            }
        }

    }
}
