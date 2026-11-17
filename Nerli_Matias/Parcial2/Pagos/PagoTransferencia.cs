namespace Parcial2.Pagos
{
   
    public class PagoTransferencia : IPagoStrategy
    {
        public const decimal Descuento = 0.03m;

        public string Nombre => "Efectivo";
        public bool PermiteCuotas => false;
        public bool RequiereCuit => false;

        public decimal CalcularAjuste(decimal subtotal, int cuotas)
        {
            return -subtotal * Descuento;
        }
    }
}
