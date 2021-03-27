using CsvHelper.Configuration.Attributes;

namespace InventarioBST.Models
{
    public class InventoryItem
    {
        [Name("id")]
        public int Id { get; set; }

        [Name("nombre")]
        public string Nombre { get; set; }

        [Name("descripcion")]
        public string Descripcion { get; set; }
        
        [Name("casa_productora")]
        public string CasaProductora { get; set; }

        [Name("precio")]
        public string Precio { get; set; }

        [Name("existencia")]
        public int Existencia { get; set; }
    }
}
