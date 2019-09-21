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
    public class ProcessViewModel : Screen
    {
        private IEventAggregator _eventAggregator;
        private ItemModel _item;
        private string _itemName;
        private string _savedItemName;
        private bool _isApplied = false;
        public ProcessViewModel() { }
        public ProcessViewModel(IEventAggregator eventAggregator, ItemModel item)
        {
            _eventAggregator = eventAggregator;
            _item = item;
            ItemName = item.Text1;

            _savedItemName = item.Text1;
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
            TryClose();
        }

        public override void CanClose(Action<bool> callback)
        {

            if (!_isApplied)
            {
                _item.Text1 = _savedItemName;
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
    }
}
