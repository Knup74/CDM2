namespace CDM.Service
{
    public static class EmailService
    {
        public static void Send(string to, string subject, string body)
        {
            // TODO : branchement SMTP réel
            Console.WriteLine($"MAIL TO {to} : {subject}\n{body}");
        }
    }
}