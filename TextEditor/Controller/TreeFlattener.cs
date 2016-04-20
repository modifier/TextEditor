using Portable.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor.Controller
{
    class TreeFlattener
    {
        private ITreeParsingResult tree;

        private List<IParsingTreeTerminal> terminals;

        public TreeFlattener(ITreeParsingResult tree)
        {
            this.tree = tree;
        }

        public List<IParsingTreeTerminal> getTerminals()
        {
            if (terminals == null)
            {
                Process();
            }

            return terminals;
        }

        private void Process()
        {
            terminals = new List<IParsingTreeTerminal>();

            VisitNode(tree.Tree);
        }

        private void VisitNode(IParsingTreeNode node)
        {
            if (node is IParsingTreeTerminal)
            {
                terminals.Add((IParsingTreeTerminal) node);

                return;
            }

            if (!(node is IParsingTreeGroup))
            {
                return;
            }

            foreach (IParsingTreeNode innerNode in ((IParsingTreeGroup)node).Childs)
            {
                VisitNode(innerNode);
            }
        }
    }
}
