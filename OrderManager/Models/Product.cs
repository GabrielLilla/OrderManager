namespace OrderManager.Models
{
    /// <summary>
    /// Classe abstrata que representa um produto genérico do pedido.
    /// As subclasses devem definir como calcular o preço final e
    /// como descrever o produto. As demais propriedades existem
    /// para facilitar o binding com o DataGrid da interface WPF.
    /// </summary>
    public abstract class Product
    {
        /// <summary>Identificador único do produto dentro do pedido.</summary>
        public int Id { get; set; }

        /// <summary>Nome do produto exibido para o cliente.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Preço base do produto, sem frete ou taxa de plataforma.</summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Calcula o preço final do produto (preço base + acréscimos).
        /// Implementação obrigatória nas subclasses.
        /// </summary>
        public abstract decimal GetFinalPrice();

        /// <summary>
        /// Retorna uma descrição amigável do produto, usada na notificação
        /// enviada ao cliente quando o pedido é finalizado.
        /// </summary>
        public abstract string GetDescription();

        // --------- Propriedades auxiliares para binding com o DataGrid ---------

        /// <summary>Rótulo do tipo do produto ("Físico" ou "Digital").</summary>
        public abstract string TypeLabel { get; }

        /// <summary>Nome do acréscimo aplicado ("Frete" ou "Taxa").</summary>
        public abstract string ExtraLabel { get; }

        /// <summary>Valor do acréscimo (frete ou taxa de plataforma).</summary>
        public abstract decimal ExtraValue { get; }

        /// <summary>Wrapper de leitura para exibir <see cref="GetFinalPrice"/> no DataGrid.</summary>
        public decimal FinalPrice => GetFinalPrice();
    }
}
