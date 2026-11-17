using System;
using System.Collections.Generic;
using System.Linq;
using Parcial2.Models;
using Parcial2.Observer;
using Parcial2.Pagos;
using Parcial2.Repository;

namespace Parcial2.Controllers
{
    // PATRÓN OBSERVER — este controller es el Notificador (Publisher).
    // Hace su trabajo (validar, calcular, guardar) y AVISA. No sabe quién escucha.
    public class PedidoController : IPedidoPublisher
    {
        public const int MaximoObservaciones = 100;

        private readonly IRepository<Pedido> _pedidos;
        private readonly IRepository<Prenda> _prendas;

        // TODO 7 · Observer
        // Este controller tiene que ser el Notificador: guardar la lista de suscriptores,
        // permitir el alta y la baja, y avisarles a todos cuando algo pasa.

        private readonly List<IPedidoObserver> _observadores = new List<IPedidoObserver>();

        public PedidoController(IRepository<Pedido> pedidos, IRepository<Prenda> prendas)
        {
            _pedidos = pedidos;
            _prendas = prendas;
        }

        public void Suscribir(IPedidoObserver observador)
        {
            if (!_observadores.Contains(observador))
                _observadores.Add(observador);
        }

        public void Desuscribir(IPedidoObserver observador)
        {
            _observadores.Remove(observador);
        }

        private void Notificar(EventoPedido evento)
        {
            // TODO 7: avisarle a cada suscriptor. Hoy no hace nada: por eso la grilla no se refresca sola.
            foreach (IPedidoObserver observador in _observadores.ToList())
                observador.Actualizar(evento);
        }

        // ------------------------------------------------------------------ CONSULTAS

        public List<Prenda> ObtenerCatalogo()
        {
            return _prendas.LeerTodos().OrderBy(p => p.Categoria).ThenBy(p => p.Nombre).ToList();
        }

        public List<Pedido> ObtenerPedidos()
        {
            return _pedidos.LeerTodos().OrderByDescending(p => p.Id).ToList();
        }

        // Lo que tiene que mirar la persona de los lavarropas: lo que espera y lo que se está lavando.
        public List<Pedido> PedidosEnProceso()
        {
            return _pedidos.LeerTodos()
                .Where(p => p.Estado == EstadoPedido.EnEspera || p.Estado == EstadoPedido.Lavando)
                .OrderBy(p => p.Id)
                .ToList();
        }

        public int Contar(EstadoPedido estado)
        {
            return _pedidos.LeerTodos().Count(p => p.Estado == estado);
        }

        public decimal Facturacion()
        {
            return _pedidos.LeerTodos().Where(p => p.Estado != EstadoPedido.Cancelado).Sum(p => p.Total);
        }

        public List<string> FormasDePago()
        {
            return PagoFactory.TiposDisponibles();
        }

        // La vista lo usa para habilitar "Cuotas" y "CUIT" según la forma de pago elegida.
        public IPagoStrategy InfoPago(string tipo)
        {
            return PagoFactory.Crear(tipo);
        }

        // ------------------------------------------------------------------ ARMADO DEL PEDIDO

        public Pedido NuevoBorrador()
        {
            return new Pedido();
        }

        public void AgregarItem(Pedido borrador, int idPrenda, int cantidad)
        {
            Prenda prenda = _prendas.BuscarPorId(idPrenda);
            borrador.AgregarItem(prenda, cantidad);
        }

        // TODO 6 · Controller
        // CotizarPago: usá la factory para obtener la estrategia y delegale el cálculo (redondeado a 0 decimales).
        // ConfirmarPedido: validar, completar el borrador, calcular ajuste y total, guardar
        //                  y notificar el alta (TODO 7). Reglas en el README.

        public decimal CotizarPago(string tipoPago, decimal subtotal, int cuotas)
        {
            IPagoStrategy pago = PagoFactory.Crear(tipoPago);
            return Math.Round(pago.CalcularAjuste(subtotal, cuotas), 0);
        }

        public Pedido ConfirmarPedido(Pedido borrador, string cliente, string telefono,
                                      string observaciones, string cuit, int cuotas, string tipoPago)
        {
            if (string.IsNullOrWhiteSpace(cliente))
                throw new ArgumentException("El cliente es obligatorio.");
            if (string.IsNullOrWhiteSpace(telefono))
                throw new ArgumentException("El telefono es obligatorio, por ahi nos comunicamos.");
            if (borrador == null || borrador.Items.Count == 0)
                throw new ArgumentException("El pedido no contiene prendas.");

            string telefonoLimpio = (telefono ?? "").Replace(" ", "").Trim();
            if (telefonoLimpio.Length == 10 && !telefonoLimpio.All(char.IsDigit))
                throw new ArgumentException("El telefono debe contener 10 digitos y ser solo numeros.");

            if (observaciones.Length > 100)
                throw new ArgumentException("El limite de caracteres para las observaciones es de 100");

            IPagoStrategy pago = PagoFactory.Crear(tipoPago);

            if (pago.RequiereCuit && cuit == null)
                throw new ArgumentException($"El {pago.Nombre} requiere cuit.");

            borrador.Cliente = cliente.Trim();
            borrador.Telefono = telefonoLimpio;
            borrador.Observaciones = observaciones.Trim();
            borrador.Cuit = pago.RequiereCuit ? cuit.Trim() : "";
            borrador.Cuotas = pago.PermiteCuotas ? cuotas : 1;

            decimal subtotal = borrador.CalcularSubtotal();

            borrador.Ajuste = Math.Round(pago.CalcularAjuste(subtotal, borrador.Cuotas), 0);
            borrador.Total = subtotal + borrador.Ajuste;
            borrador.Estado = EstadoPedido.EnEspera;
            borrador.FechaIngreso = DateTime.Now;

            _pedidos.Agregar(borrador);

            Notificar(new EventoPedido(TipoEvento.Alta, borrador, borrador.Estado));

            return borrador;
        }

        // ------------------------------------------------------------------ OPERACIONES

        public void AvanzarEstado(int id)
        {
            Pedido pedido = BuscarOFallar(id);
            EstadoPedido anterior = pedido.Estado;
            pedido.AvanzarEstado();
            _pedidos.Actualizar(pedido);

            Notificar(new EventoPedido(TipoEvento.CambioEstado, pedido, anterior));
        }

        public void Cancelar(int id)
        {
            Pedido pedido = BuscarOFallar(id);
            EstadoPedido anterior = pedido.Estado;
            pedido.Cancelar();
            _pedidos.Actualizar(pedido);

            Notificar(new EventoPedido(TipoEvento.Cancelacion, pedido, anterior));
        }

        private Pedido BuscarOFallar(int id)
        {
            return _pedidos.BuscarPorId(id) ?? throw new InvalidOperationException($"No existe el pedido #{id}.");
        }
    }
}
