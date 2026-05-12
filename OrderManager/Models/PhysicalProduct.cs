namespace OrderManager.Models
{
    /// <summary>
    /// Produto físico: o preço final é o preço base somado ao frete.
    /// </summary>
    public class PhysicalProduct : Product
    {
        /// <summary>Valor do frete em reais.</summary>
        public decimal Shipping { get; set; }

        public override decimal GetFinalPrice() => Price + Shipping;

        public override string GetDescription()
            => $"[Físico] {Name} — Preço base: {Price:C} + Frete: {Shipping:C} = Total: {GetFinalPrice():C}";

        public override string TypeLabel => "Físico";
        public override string ExtraLabel => "Frete";
        public override decimal ExtraValue => Shipping;
    }
}
