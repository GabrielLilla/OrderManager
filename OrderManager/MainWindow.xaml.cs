using OrderManager.Models;
using OrderManager.Services;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace OrderManager
{
    /// <summary>
    /// Lógica de interação para MainWindow.xaml.
    /// É aqui que ocorre a "composição": a UI cria o notificador concreto
    /// e o injeta no <see cref="OrderService"/>. O serviço em si nunca
    /// instancia notificadores diretamente.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly OrderService _orderService;
        private readonly CultureInfo _ptBr = new("pt-BR");

        public MainWindow()
        {
            InitializeComponent();

            // ---- Composição da raiz: criamos o notificador padrão (Email) ----
            // e injetamos no OrderService via construtor (Dependency Injection).
            INotifier defaultNotifier = new EmailNotifier();
            _orderService = new OrderService(defaultNotifier);

            // Vincula o DataGrid à coleção observável do serviço — adições e
            // remoções são refletidas automaticamente.
            ProductsGrid.ItemsSource = _orderService.Products;
        }

        // ---------- Adaptação dinâmica do formulário ----------

        private void ProductTypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ExtraLabelText == null) return; // ocorre durante a inicialização do XAML
            var selected = (ProductTypeBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            ExtraLabelText.Text = selected == "Digital" ? "TAXA DE PLATAFORMA (R$)" : "FRETE (R$)";
        }

        // ---------- Troca de notificador (mantém DI: o concreto é criado AQUI, não dentro do serviço) ----------

        private void NotificationTypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_orderService == null) return;
            var selected = (NotificationTypeBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            INotifier newNotifier = selected == "SMS" ? new SmsNotifier() : new EmailNotifier();
            _orderService.ChangeNotifier(newNotifier);
        }

        // ---------- Botões ----------

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var name = ProductNameBox.Text?.Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    Warn("Informe o nome do produto.");
                    return;
                }

                if (!TryParseDecimal(PriceBox.Text, out var price) || price < 0)
                {
                    Warn("Preço inválido. Use um valor numérico não negativo (ex.: 19,90).");
                    return;
                }

                if (!TryParseDecimal(ExtraBox.Text, out var extra) || extra < 0)
                {
                    Warn("Valor de frete/taxa inválido. Use um valor numérico não negativo.");
                    return;
                }

                var type = (ProductTypeBox.SelectedItem as ComboBoxItem)?.Content?.ToString();

                // Polimorfismo: criamos a subclasse apropriada de Product
                Product product = type == "Digital"
                    ? new DigitalProduct  { Name = name, Price = price, PlatformFee = extra }
                    : new PhysicalProduct { Name = name, Price = price, Shipping    = extra };

                _orderService.AddProduct(product);
                ClearProductForm();
                UpdateTotalDisplay();
            }
            catch (Exception ex)
            {
                Error($"Erro ao cadastrar produto: {ex.Message}");
            }
        }

        private void RemoveProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsGrid.SelectedItem is Product selected)
            {
                _orderService.RemoveProduct(selected);
                UpdateTotalDisplay();
            }
            else
            {
                Warn("Selecione um produto na tabela para remover.");
            }
        }

        private void CalculateTotal_Click(object sender, RoutedEventArgs e)
        {
            var total = _orderService.CalculateTotal();
            UpdateTotalDisplay();
            MessageBox.Show(
                $"Total do pedido: {total.ToString("C", _ptBr)}\nItens: {_orderService.Products.Count}",
                "Total do Pedido",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void FinalizeOrder_Click(object sender, RoutedEventArgs e)
        {
            var customer = CustomerNameBox.Text?.Trim();
            if (string.IsNullOrWhiteSpace(customer))
            {
                Warn("Informe o nome do cliente antes de finalizar o pedido.");
                return;
            }

            if (_orderService.Products.Count == 0)
            {
                Warn("Adicione ao menos um produto antes de finalizar o pedido.");
                return;
            }

            try
            {
                _orderService.FinalizeOrder(customer);
                CustomerNameBox.Clear();
                UpdateTotalDisplay();
            }
            catch (Exception ex)
            {
                Error($"Erro ao finalizar pedido: {ex.Message}");
            }
        }

        // ---------- Helpers ----------

        private bool TryParseDecimal(string? text, out decimal value)
        {
            // Aceita tanto "19,90" (pt-BR) quanto "19.90" (invariante).
            if (string.IsNullOrWhiteSpace(text))
            {
                value = 0;
                return false;
            }

            var normalized = text.Trim().Replace(",", ".");
            return decimal.TryParse(normalized,
                                    NumberStyles.Number,
                                    CultureInfo.InvariantCulture,
                                    out value);
        }

        private void ClearProductForm()
        {
            ProductNameBox.Clear();
            PriceBox.Clear();
            ExtraBox.Clear();
            ProductNameBox.Focus();
        }

        private void UpdateTotalDisplay()
        {
            TotalText.Text = _orderService.CalculateTotal().ToString("C", _ptBr);
        }

        private static void Warn(string message)
            => MessageBox.Show(message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);

        private static void Error(string message)
            => MessageBox.Show(message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
