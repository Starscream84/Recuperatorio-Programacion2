using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Parcial2.Observer
{
    public class AvisoClienteObserver : IPedidoObserver
    {
        private readonly string _rutaAvisos;

        public AvisoClienteObserver(string rutaAvisos)
        {
            _rutaAvisos = rutaAvisos;

        }

        public void Actualizar(EventoPedido evento)
        {
            // Falta completar aca
            string texto;
            if (evento.Tipo == TipoEvento.Alta)
                texto = $"Hola {evento.Pedido.Cliente} recibimos tu pedido #{evento.Pedido.Id}. Total: {evento.Pedido.Total:C0}";
            else if (evento.Tipo == TipoEvento.CambioEstado) //Me falta un comparador
                texto = $"Tu pedido #{evento.Pedido.Id} esta listo para retirar.";
            else if (evento.Tipo == TipoEvento.Cancelacion)
                texto = $"Tu pedido #{evento.Pedido.Id} fue cancelado. Podes pasar a buscar tus prendas.";
            else
                return;

            File.AppendAllText(_rutaAvisos,
                $"[{evento.Fecha:dd/MM HH:mm}] WhatsApp a {evento.Pedido.Cliente} ({evento.Pedido.Telefono}):{texto}{Environment.NewLine}");
        }

    }
}
