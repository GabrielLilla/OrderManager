using System.Windows;

namespace OrderManager.Services
{
    /// <summary>
    /// Notifica o cliente por SMS. Em um app real, esta classe chamaria
    /// uma API de SMS (Twilio, Zenvia, etc.); aqui o envio é simulado.
    /// </summary>
    public class SmsNotifier : INotifier
    {
        public void Send(string customerName, string message)
        {
            MessageBox.Show(
                $"📱 SMS enviado para {customerName}:\n\n{message}",
                "Notificação por SMS",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
