using System.Collections.Generic;

namespace InventarioBST.Models
{
    public class DataTableResponse
    {
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public List<InventoryItem> Data { get; set; } = new List<InventoryItem>();
    }
}
