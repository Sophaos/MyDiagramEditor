using MyDiagramEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDiagramEditor.Utilities
{
    public class TextAlternationMessage
    {
        private ItemModel _item;
        public TextAlternationMessage(ItemModel item)
        {
            _item = item;
        }

        public ItemModel Item { get => _item; set => _item = value; }
    }
}
