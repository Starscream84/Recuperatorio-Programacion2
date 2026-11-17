using System;

namespace Parcial2.Pagos
{
    
    public class PagoCuentaEmpresa : IPagoStrategy
    {
        public const decimal Descuento = 0.05m;
        public const decimal DescuentoUmbral = 0.08m;
        public const decimal Umbral = 100000m;
        public const decimal Minimo = 20000m;

        public string Nombre => "Efectivo";
        public bool PermiteCuotas => false;
        public bool RequiereCuit => true;

        public decimal CalcularAjuste(decimal subtotal, int cuotas)
        {
            if (subtotal >= Umbral)
                return -subtotal * DescuentoUmbral;
            if (subtotal < Minimo)
                throw new InvalidOperationException($"La cuenta empresa admite pedidos desde {Minimo}.");

            return -subtotal * Descuento;
        }
    }
}
