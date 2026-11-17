using System;

namespace Parcial2.Pagos
{
    public class PagoTarjeta : IPagoStrategy
    {
        public const decimal RecargoDosCuotas = 0.05m;
        public const decimal RecargoTresCuotas = 0.10m;

        public string Nombre => "Tarjeta";
        public bool PermiteCuotas => true;
        public bool RequiereCuit => false;

        public decimal CalcularAjuste(decimal subtotal, int cuotas)
        {
            
            switch(cuotas)
            {
                case 1:
                    return 0m;
                case 2:
                    return subtotal * RecargoDosCuotas;
                case 3:
                    return subtotal * RecargoTresCuotas;
                default:
                    throw new ArgumentException("Con tarjeta se paga en 1, 2 o 3 cuotas.");
            }
        }
    }
}
