namespace Parcial2.Comprobantes
{
    // PATRÓN FACTORY METHOD — Creador concreto: solo sobrescribe el método fábrica.
    // TODO 8 · Factory Method: usá este archivo y FormatoTxt.cs de modelo para crear la etiqueta para la bolsa.
    public class GeneradorComprobanteEtiqueta : GeneradorComprobante
    {
        public override string Descripcion => "Etiqueta para la bolsa";
        public override string Extension => "txt";

        protected override IFormatoComprobante CrearFormato()
        {
            return new FormatoEtiquetaTxt();
        }
    }
}
