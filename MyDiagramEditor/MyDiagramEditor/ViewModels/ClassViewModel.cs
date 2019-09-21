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
    public class ClassViewModel : Screen
    {
        private IEventAggregator _eventAggregator;
        private ItemModel _item;
        private string _itemName;
        private string _attributs;
        private string _methods;

        private string _savedItemName;
        private string _savedAttributs;
        private string _savedMethods;

        private bool _isApplied = false;

        public ClassViewModel() { }
        public ClassViewModel(IEventAggregator eventAggregator, ItemModel item)
        {
            _eventAggregator = eventAggregator;
            _item = item;
            ItemName = _item.Text1;
            Attributs = _item.Text2;
            Methods = _item.Text3;

            _savedItemName = _item.Text1;
            _savedAttributs = _item.Text2;
            _savedMethods = _item.Text3;
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
            _item.Text1 = _savedItemName;
            _item.Text2 = _savedAttributs;
            _item.Text3 = _savedMethods;
            TryClose();
        }

        public override void CanClose(Action<bool> callback)
        {
            if (!_isApplied)
            {
                _item.Text1 = _savedItemName;
                _item.Text2 = _savedAttributs;
                _item.Text3 = _savedMethods;
            }

            callback(true);
        }

        public string ItemName
        {
            get { return _itemName; }
            set
            {
                _itemName = value;
                NotifyOfPropertyChange(() => ItemName);
                _item.Text1 = _itemName;
            }
        }

        public string Attributs
        {
            get { return _attributs; }
            set
            {
                _attributs = value;
                NotifyOfPropertyChange(() => Attributs);
                _item.Text2 = _attributs;
            }
        }

        public string Methods
        {
            get { return _methods; }
            set
            {
                _methods = value;
                NotifyOfPropertyChange(() => Methods);
                _item.Text3 = _methods;
            }
        }
    }
}
