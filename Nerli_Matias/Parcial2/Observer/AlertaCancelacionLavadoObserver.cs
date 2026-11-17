using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parcial2.Services;

namespace Parcial2.Observer
{
    public class AlertaCancelacionLavadoObserver : IPedidoObserver
    {
        public void Actualizar(EventoPedido evento)
        {
            /*
            if () //Si pedido == lavando && pedido == cancelado)
                Bitacora.Instancia.Registrar($"ATENCION encargada: se cancelo el pedido: {evento.Pedido.Id} que ya estaba lavado {evento.Pedido.Cliente}");
            */
        }
    }
}
