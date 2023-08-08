using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace TeklaHierarchicDefinitions.Models
{
    public class MyObservableCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        private IEnumerable<T> enumerable;

        public MyObservableCollection() : base()
        {
            CollectionChanged += new NotifyCollectionChangedEventHandler(MyObservableCollection_CollectionChanged);
        }

        public MyObservableCollection(IEnumerable<T> enumerable)
        {
            this.enumerable = enumerable;
        }


        void MyObservableCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Object item in e.NewItems)
                {
                    (item as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);
                }
            }
            if (e.OldItems != null)
            {
                foreach (Object item in e.OldItems)
                {
                    (item as INotifyPropertyChanged).PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged);
                }
            }
        }

        void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(new PropertyChangedEventArgs("ItemProperty"));
        }
    }
}
