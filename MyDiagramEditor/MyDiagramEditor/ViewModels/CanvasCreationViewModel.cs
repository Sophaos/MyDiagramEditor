using Caliburn.Micro;
using MyDiagramEditor.Models;
using MyDiagramEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyDiagramEditor.ViewModels
{
    public class CanvasCreationViewModel : Screen
    {
        private IEventAggregator _eventAggregator;
        private string _canvasName = "Westeros";
        private double _canvasHeight = 1000;
        private double _canvasWidth = 1000;


        public CanvasCreationViewModel() { }
        public CanvasCreationViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void Create()
        {
            if (_canvasHeight < 0 || _canvasWidth < 0)
            {
                MessageBox.Show("The canvas size must be positive.");
                return;
            }
            if (String.IsNullOrWhiteSpace(_canvasName))
            {
                MessageBox.Show("The name is not valid");
                return;
            }
            CanvasCreationMessage ccm = new CanvasCreationMessage();
            CanvasModel canvas = new CanvasModel();
            canvas.Height = _canvasHeight;
            canvas.Width = _canvasWidth;
            canvas.Name = _canvasName;
            ccm.Canvas = canvas;
            ccm.IsDoneAdding = true;
            _eventAggregator.PublishOnCurrentThread(ccm);

            TryClose();
        }
        public void Load()
        {
            _eventAggregator.PublishOnCurrentThread(new CanvasLoadingMessage("Load Canvas"));
            TryClose();
        }


        public string CanvasName
        {
            get { return _canvasName; }
            set
            {
                _canvasName = value;
                NotifyOfPropertyChange(() => CanvasName);
            }
        }
        public double CanvasWidth
        {
            get { return _canvasWidth; }
            set
            {
                _canvasWidth = value;
                NotifyOfPropertyChange(() => CanvasWidth);
            }
        }
        public double CanvasHeight
        {
            get { return _canvasHeight; }
            set
            {
                _canvasHeight = value;
                NotifyOfPropertyChange(() => CanvasHeight);
            }
        }
    }
}