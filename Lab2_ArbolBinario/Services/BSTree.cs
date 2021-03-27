using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventarioBST.Services
{
    public class TreeNode<T>
    {
        private readonly T _value;
        private readonly List<TreeNode<T>> _children = new List<TreeNode<T>>();

        public TreeNode(T value)
        {
            _value = value;
        }

        public TreeNode<T> this[int i]
        {
            get { return _children[i]; }
        }

        public TreeNode<T> Parent { get; private set; }

        public T Value { get { return _value; } }
        
        public TreeNode<T> AddChild(T value)
        {
            var node = new TreeNode<T>(value) { Parent = this };
            _children.Add(node);
            return node;
        }

        public TreeNode<T>[] AddChildren(params T[] values)
        {
            return values.Select(AddChild).ToArray();
        }

        public bool RemoveChild(TreeNode<T> node)
        {
            return _children.Remove(node);
        }

        public void Traverse(Action<T> action)
        {
            action(Value);
            foreach (var child in _children)
                child.Traverse(action);
        }

        public IEnumerable<T> Flatten()
        {
            return new[] { Value }.Concat(_children.SelectMany(x => x.Flatten()));
        }
        public virtual void storeBSTNodes(TreeNode<T> root, List<TreeNode<T>> nodes)
        {
            // Base case 
            if (root == null)
            {
                return;
            }

            // Store nodes in Inorder (which is sorted 
            // order for BST) 
            storeBSTNodes(root, nodes);
            nodes.Add(root);
            storeBSTNodes(root, nodes);
        }

        /* Recursive function to construct binary tree */
        public virtual TreeNode<T> buildTreeUtil(List<TreeNode<T>> nodes, int start, int end)
        {
            // base case 
            if (start > end)
            {
                return null;
            }

            /* Get the middle element and make it root */
            int mid = (start + end) / 2;
            TreeNode<T> node = nodes[mid];

            /* Using index in Inorder traversal, construct 
               left and right subtress */
            node = buildTreeUtil(nodes, start, mid - 1);
            node = buildTreeUtil(nodes, mid + 1, end);

            return node;
        }

        // This functions converts an unbalanced BST to 
        // a balanced BST 
        public virtual TreeNode<T> buildTree(TreeNode<T> root)
        {
            // Store nodes of given BST in sorted order 
            List<TreeNode<T>> nodes = new List<TreeNode<T>>();
            storeBSTNodes(root, nodes);

            // Constucts BST from nodes[] 
            int n = nodes.Count;
            return buildTreeUtil(nodes, 0, n - 1);
        }
    }
}
