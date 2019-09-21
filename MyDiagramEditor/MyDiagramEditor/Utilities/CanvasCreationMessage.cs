using MyDiagramEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDiagramEditor.Utilities
{
    public class CanvasCreationMessage
    {

        private bool _isDoneAdding;
        private CanvasModel _canvas;


        public CanvasCreationMessage()
        {

        }
        public bool IsDoneAdding { get => _isDoneAdding; set => _isDoneAdding = value; }
        public CanvasModel Canvas { get => _canvas; set => _canvas = value; }
    }
}
