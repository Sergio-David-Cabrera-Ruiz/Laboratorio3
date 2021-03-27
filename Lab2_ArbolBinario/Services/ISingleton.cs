using InventarioBST.Models;
using System.Collections.Generic;

namespace InventarioBST.Services
{
    public interface ISingleton
    {
        List<InventoryItem> DataSource { get; set; }
        TreeNode<IndexModel> IndexTree { get; set; }
    }
}