namespace OrderManager.Models
{
    /// <summary>
    /// Produto digital: o preço final é o preço base somado à taxa de plataforma.
    /// </summary>
    public class DigitalProduct : Product
    {
        /// <summary>Taxa de plataforma cobrada (ex.: comissão da loja digital).</summary>
        public decimal PlatformFee { get; set; }

        public override decimal GetFinalPrice() => Price + PlatformFee;

        public override string GetDescription()
            => $"[Digital] {Name} — Preço base: {Price:C} + Taxa: {PlatformFee:C} = Total: {GetFinalPrice():C}";

        public override string TypeLabel => "Digital";
        public override string ExtraLabel => "Taxa";
        public override decimal ExtraValue => PlatformFee;
    }
}
