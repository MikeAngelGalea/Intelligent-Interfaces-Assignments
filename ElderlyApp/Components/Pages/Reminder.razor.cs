using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace ElderlyApp.Components.Pages
{
    public partial class Reminder
    {
        [Inject]
        private EmailSettings EmailSettings { get; set; } = default!;

        public string MedicineName { get; set; } = string.Empty; // Stores the medicine name
        public DateTime? ReminderDate { get; set; } // Stores the reminder date
        public TimeOnly? ReminderTime { get; set; } // Stores the reminder time
        public string UserEmail { get; set; } = string.Empty; // Stores the user's email
        public string Message { get; set; } = string.Empty; // Stores success or error messages

        // Asynchronous method to handle reminder scheduling
        private async Task SetReminder()
        {
            if (string.IsNullOrWhiteSpace(MedicineName) || ReminderDate == null || ReminderTime == null || string.IsNullOrWhiteSpace(UserEmail))
            {
                Message = "Please fill out all fields.";
                return;
            }

            DateTime reminderDateTime = ReminderDate.Value.Date.Add(ReminderTime.Value.ToTimeSpan());

            try
            {
                // Send the reminder email
                await SendReminderEmail(UserEmail, MedicineName, reminderDateTime);

                Message = $"Reminder for {MedicineName} set for {reminderDateTime:MMMM dd, yyyy hh:mm tt}, and an email has been sent!";
            }
            catch (Exception ex)
            {
                Message = $"Failed to set the reminder. Error: {ex.Message}";
            }
        }

        // Helper method to send the email reminder
        private async Task SendReminderEmail(string userEmail, string medicineName, DateTime reminderDateTime)
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
                    Subject = "Medicine Reminder",
                    Body = $"Hello,\n\nThis is a reminder to take your medicine: {medicineName}.\n\n" +
                           $"Reminder Details:\nDate: {reminderDateTime:MMMM dd, yyyy}\n" +
                           $"Time: {reminderDateTime:hh\\:mm tt}.\n\n" +
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
