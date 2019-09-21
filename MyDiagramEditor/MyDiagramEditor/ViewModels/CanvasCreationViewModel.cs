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
        private string _password = "Aegon";
        private string _privacy = "Public";
        private string _protection = "Unprotected";
        private bool _isProtected = false;
        private string _username = "";
        private string _thumbnail = "";

        public CanvasCreationViewModel() { }
        public CanvasCreationViewModel(IEventAggregator eventAggregator, string username, string thumbnail)
        {
            _eventAggregator = eventAggregator;
            _username = username;
            _thumbnail = thumbnail;
        }

        public bool isPrivate()
        {
            return _privacy == "private";
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
            canvas.Privacy = _privacy;
            canvas.Protection = _protection;
            canvas.Password = _password;
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

        public void SelectProtection(string type)
        {
            if (type == "Unprotected")
            {

                Protection = "Unprotected";
                IsProtected = false;
            }
            else if (type == "Protected")
            {

                Protection = "Protected";
                IsProtected = true;
            }
        }

        public void SelectPrivacy(string type)
        {
            if (type == "Public")
            {
                Privacy = "Public";
            }
            else if (type == "Private")
            {
                Privacy = "Private";
            }
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
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
            }
        }
        public string Protection
        {
            get { return _protection; }
            set
            {
                _protection = value;
                NotifyOfPropertyChange(() => Protection);
            }
        }

        public string Privacy
        {
            get { return _privacy; }
            set
            {
                _privacy = value;
                NotifyOfPropertyChange(() => Privacy);
            }
        }
        public bool IsProtected
        {
            get { return _isProtected; }
            set
            {
                _isProtected = value;
                NotifyOfPropertyChange(() => IsProtected);
            }
        }
    }
}