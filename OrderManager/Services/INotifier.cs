namespace OrderManager.Services
{
    /// <summary>
    /// Abstração para qualquer canal de notificação ao cliente.
    /// Implementações concretas: <see cref="EmailNotifier"/> e <see cref="SmsNotifier"/>.
    /// </summary>
    public interface INotifier
    {
        /// <summary>
        /// Envia uma mensagem ao cliente pelo canal específico da implementação.
        /// </summary>
        /// <param name="customerName">Nome do cliente que receberá a notificação.</param>
        /// <param name="message">Conteúdo da mensagem.</param>
        void Send(string customerName, string message);
    }
}
