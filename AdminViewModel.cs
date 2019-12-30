using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace MVVMCashbox
{
    class AdminViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Product> products;
        private Product selectedProduct;
        private Command addCommand;
        private Command deleteCommand;

        public AdminViewModel()
        {
            Products = new ObservableCollection<Product>();
        }

        public ObservableCollection<Product> Products
        {
            get { return products; }
            set
            {
                products = value;
                OnPropertyChanged("Products");
            }
        }

        public Product SelectedProduct
        {
            get { return selectedProduct; }
            set
            {
                selectedProduct = value;
                OnPropertyChanged("SelectedProduct");
            }
        }

        public Command AddCommand
        {
            get
            {
                return addCommand ?? (addCommand = new Command((obj) =>
                  {
                      Product product = new Product();
                      Products.Insert(0, product);
                      SelectedProduct = product;
                  }));
            }
        }
        
        public Command DeleteCommand
        {
            get
            {
                return deleteCommand ?? (deleteCommand = new Command((obj) =>
                    {
                        if (obj is Product product)
                        {
                            Products.Remove(product);
                        }
                    }, (obj) => Products.Count > 0));
            }
        }

        public void CheckProducts()
        {
            ObservableCollection<Product> save = new ObservableCollection<Product>();
            foreach (var product in products)
            {
                if (product.Name != null) save.Add(product);
            }
            products = save;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
