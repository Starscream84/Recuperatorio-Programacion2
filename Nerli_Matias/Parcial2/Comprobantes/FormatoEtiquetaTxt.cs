using System.Text;
using Parcial2.Models;

namespace Parcial2.Comprobantes
{
    // Producto concreto: comprobante de texto plano.
    public class FormatoEtiquetaTxt : IFormatoComprobante
    {
        private readonly StringBuilder _sb = new StringBuilder();

        public void Encabezado(Pedido pedido)
        {
            _sb.AppendLine("        LAVADERO LA PERLA");
            _sb.AppendLine($"Pedido #{pedido.Id} - {pedido.Cliente}");
        }

        public void Item(ItemPedido item)
        {
            _sb.AppendLine($"{item.Cantidad,3} x {item.Descripcion,-28}");
        }

        public void Totales(decimal subtotal, decimal ajuste, decimal total)
        {
            _sb.AppendLine($"{"TOTAL",-34}{total,12:C0}");   
        }

        public string Resultado()
        {
            return _sb.ToString();
        }
    }
}
