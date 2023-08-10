namespace CityInfo.API.Services
{
    public class LocalMailService
    {
        private readonly string _mailTo = "admin@mycomapy.com";
        private readonly string _mailFrom = "norplay@mycompany.com";

        public void Send(string subject, string message)
        {
            // send mail - output to console window
            Console.WriteLine($"Mail from {_mailFrom} to {_mailTo}, " +
                              $"with {nameof(LocalMailService)}.");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Message: {message}");
        }
    }
}
