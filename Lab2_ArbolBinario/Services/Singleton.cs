using InventarioBST.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventarioBST.Services
{
    public class Singleton : ISingleton
    {
        public List<InventoryItem> DataSource { get; set; } = new List<InventoryItem>();
        public TreeNode<IndexModel> IndexTree { get; set; }

        public Singleton()
        {
        }
    }
}
