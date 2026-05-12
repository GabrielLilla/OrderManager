using System.Windows;

namespace OrderManager.Services
{
    /// <summary>
    /// Notifica o cliente por e-mail. Em um app real, esta classe usaria
    /// um servidor SMTP; aqui simulamos o envio com um MessageBox.
    /// </summary>
    public class EmailNotifier : INotifier
    {
        public void Send(string customerName, string message)
        {
            MessageBox.Show(
                $"📧 E-mail enviado para {customerName}:\n\n{message}",
                "Notificação por E-mail",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
