using Caliburn.Micro;
using MyDiagramEditor.Models;
using MyDiagramEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDiagramEditor.ViewModels
{
    public class MultiplicityViewModel : Screen
    {
        private IEventAggregator _eventAggregator;
        private ItemModel _item;
        private string _type;
        private bool _isApplied = false;
        private string _currentMultiplicity;
        private string _savedMultiplicity;
        public MultiplicityViewModel() { }
        public MultiplicityViewModel(IEventAggregator eventAggregator, ItemModel item, string type)
        {
            _eventAggregator = eventAggregator;
            _item = item;
            _type = type;
            if (_type == "First")
            {
                CurrentMultiplicity = _item.Text2;
                _savedMultiplicity = _item.Text2;
            }
            else if (_type == "Second")
            {
                CurrentMultiplicity = _item.Text3;
                _savedMultiplicity = _item.Text3;
            }
        }
        public void SelectMultiplicity(string type)
        {

            if (type == "1") { CurrentMultiplicity = "1"; }
            if (type == "0..1") { CurrentMultiplicity = "0..1"; }
            if (type == "0..*") { CurrentMultiplicity = "0..*"; }
            if (type == "1..*") { CurrentMultiplicity = "1..*"; }
            if (type == " ") { CurrentMultiplicity = " "; }


        }

        public void Apply()
        {
            // envoie server ici
            _eventAggregator.PublishOnCurrentThread(new TextAlternationMessage(_item));
            _isApplied = true;
            TryClose();
        }

        public void Cancel()
        {
            if (_type == "First")
            {
                _item.Text2 = _savedMultiplicity;
            }
            else if (_type == "Second")
            {
                _item.Text3 = _savedMultiplicity;
            }
            TryClose();
        }

        public override void CanClose(Action<bool> callback)
        {

            if (!_isApplied)
            {
                if (_type == "First")
                {
                    _item.Text2 = _savedMultiplicity;
                }
                else if (_type == "Second")
                {
                    _item.Text3 = _savedMultiplicity;
                }
            }
            callback(true);
        }

        public string CurrentMultiplicity
        {
            get { return _currentMultiplicity; }
            set
            {
                _currentMultiplicity = value;
                NotifyOfPropertyChange(() => CurrentMultiplicity);
                if (_type == "First")
                {
                    _item.Text2 = _currentMultiplicity;
                }
                else if (_type == "Second")
                {
                    _item.Text3 = _currentMultiplicity;
                }
            }
        }
    }
}
