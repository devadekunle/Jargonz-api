using FluentEmail.Core;
using jargonz.api.Common.Configuration;
using Resend;

namespace jargonz.api.Common.EmailNotifications;

public class EmailNotificationService
{
    private readonly IFluentEmail _email;
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailNotificationService> _logger;
    private readonly IResend _resend;

    public EmailNotificationService(
        IFluentEmailFactory emailFactory,
        ILogger<EmailNotificationService> logger,
        IResend resend,
        EmailSettings emailSettings)
    {
        _email = emailFactory.Create();
        _logger = logger;
        _resend = resend;
        _emailSettings = emailSettings;
    }

    public async Task SendEmailAsync<TModel>(
        string toEmail,
        string subject,
        string templateResourcePath,
        TModel model,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Rendering email template for {ToEmail} with template {TemplateResourcePath}",
                toEmail, templateResourcePath);

            // Use FluentEmail to render the HTML template
            var email = _email
                .To(toEmail)
                .Subject(subject)
                .UsingTemplateFromEmbedded(
                    templateResourcePath,
                    model,
                    typeof(EmailNotificationService).Assembly);

            _logger.LogInformation("Sending email via Resend to {ToEmail}", toEmail);

            // Use native Resend SDK to send the email
            var resendMessage = new EmailMessage
            {
                From = _emailSettings.FromEmail,
                To = new[] { toEmail },
                Subject = subject,
                HtmlBody = email.Data.Body
            };

            var result = await _resend.EmailSendAsync(resendMessage, cancellationToken);

            if (result == null) throw new EmailSendException("Failed to send email: No response from Resend");

            _logger.LogInformation("Email sent successfully to {ToEmail}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {ToEmail}", toEmail);
            throw;
        }
    }
}

public class EmailSendException : Exception
{
    public EmailSendException(string message) : base(message)
    {
    }

    public EmailSendException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
