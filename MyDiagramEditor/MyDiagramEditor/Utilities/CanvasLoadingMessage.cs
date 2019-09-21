using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDiagramEditor.Utilities
{
    public class CanvasLoadingMessage
    {
        private string _loading;
        public CanvasLoadingMessage(string load) { _loading = load; }

        public string Loading { get => _loading; set => _loading = value; }
    }
}
