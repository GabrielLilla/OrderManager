using OrderManager.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace OrderManager.Services
{
    /// <summary>
    /// Serviço responsável por gerenciar o pedido: adicionar, remover,
    /// calcular total e finalizar enviando notificação ao cliente.
    ///
    /// Esta classe demonstra Injeção de Dependência:
    ///   - Recebe <see cref="INotifier"/> exclusivamente pelo construtor.
    ///   - Nunca instancia notificadores com <c>new</c> internamente.
    ///   - Depende da abstração, não das implementações concretas
    ///     (princípio da Inversão de Dependência - SOLID).
    /// </summary>
    public class OrderService
    {
        private INotifier _notifier;
        private int _nextId = 1;

        /// <summary>
        /// Lista observável de produtos. Como é uma
        /// <see cref="ObservableCollection{T}"/>, o DataGrid da interface
        /// WPF é atualizado automaticamente ao adicionar/remover itens.
        /// </summary>
        public ObservableCollection<Product> Products { get; } = new();

        /// <summary>
        /// Construtor exige um <see cref="INotifier"/> — DI obrigatória.
        /// Não há construtor padrão para impedir a criação sem dependência.
        /// </summary>
        public OrderService(INotifier notifier)
        {
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
        }

        /// <summary>
        /// Permite trocar o notificador em tempo de execução, sem violar
        /// o princípio de DI: o novo notificador também vem de fora — esta
        /// classe nunca usa <c>new</c> para criar implementações de <see cref="INotifier"/>.
        /// Útil quando o usuário muda o canal de notificação na interface.
        /// </summary>
        public void ChangeNotifier(INotifier notifier)
        {
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
        }

        /// <summary>Adiciona um produto ao pedido. O Id é atribuído automaticamente.</summary>
        public void AddProduct(Product product)
        {
            if (product is null) throw new ArgumentNullException(nameof(product));
            product.Id = _nextId++;
            Products.Add(product);
        }

        /// <summary>Remove um produto do pedido. Retorna <c>true</c> se removido.</summary>
        public bool RemoveProduct(Product product)
        {
            if (product is null) return false;
            return Products.Remove(product);
        }

        /// <summary>
        /// Soma o preço final de todos os produtos do pedido.
        /// Polimorfismo em ação: cada produto calcula seu próprio total.
        /// </summary>
        public decimal CalculateTotal()
            => Products.Sum(p => p.GetFinalPrice());

        /// <summary>
        /// Finaliza o pedido, envia uma notificação ao cliente pelo canal
        /// configurado e limpa a lista de produtos.
        /// </summary>
        public void FinalizeOrder(string customerName)
        {
            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentException("O nome do cliente é obrigatório.", nameof(customerName));

            if (Products.Count == 0)
                throw new InvalidOperationException("Não é possível finalizar um pedido sem produtos.");

            var total = CalculateTotal();

            var sb = new StringBuilder();
            sb.AppendLine("Seu pedido foi finalizado com sucesso! 🎉");
            sb.AppendLine();
            sb.AppendLine("Itens do pedido:");
            foreach (var p in Products)
                sb.AppendLine("• " + p.GetDescription());
            sb.AppendLine();
            sb.AppendLine($"Total: {total:C}");
            sb.AppendLine();
            sb.AppendLine("Obrigado pela preferência!");

            _notifier.Send(customerName, sb.ToString());

            Products.Clear();
            _nextId = 1;
        }
    }
}
