using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace MVVMCashbox
{
    class UserViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Product> products;
        private ObservableCollection<Product> scoreProducts;
        private Product selectedProduct;
        private Product scoreSelectedProduct;
        private Command scoreCommand;
        private Command cancelCommand;
        private Command moveToCheckCommand;
        private Command removeToChekCommand;
        private int resultCost;

        private readonly IJsonFileInterface<Score> jsonDataScore = new JsonFileService<Score>();

        public UserViewModel()
        {
            Products = new ObservableCollection<Product>();
            ScoreProducts = new ObservableCollection<Product>();
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

        public ObservableCollection<Product> ScoreProducts
        {
            get { return scoreProducts; }
            set
            {
                scoreProducts = value;
                OnPropertyChanged("ScoreProducts");
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

        public Product ScoreSelectedProduct
        {
            get { return scoreSelectedProduct; }
            set
            {
                scoreSelectedProduct = value;
                OnPropertyChanged("ScoreSelectedProduct");
            }
        }

        public int ResultCost
        {
            get { return resultCost; }
            set
            {
                resultCost = value;
                OnPropertyChanged("ResultCost");
            }
        }

        private void ResultCostFunction()
        {
            if (ScoreProducts.Count > 0)
            {
                int summ = 0;
                foreach (var product in ScoreProducts)
                {
                    summ += product.Cost * product.Count;
                }
                ResultCost = summ;
            }
            else ResultCost = 0;
        }

        // Очистить список и оформить чек
        public Command ScoreCommand
        {
            get
            {
                return scoreCommand ?? (scoreCommand = new Command((obj) =>
                  {
                      // сохранение чека
                      ObservableCollection<Score> Scores = new ObservableCollection<Score>();
                      Score score = new Score()
                      {
                          ScoreShortDateString = DateTime.Now.ToShortDateString(),
                          ScoreLongTimeString = DateTime.Now.ToLongTimeString(),
                          Products = ScoreProducts
                      };

                      Scores = jsonDataScore.Load(@"..\..\data/scores.json");

                      Scores.Add(score);
                      jsonDataScore.Save(@"..\..\data/scores.json", Scores);

                      // очистка чека
                      ScoreProducts.Clear();

                      ResultCostFunction();

                  }, (obj) => ScoreProducts.Count > 0));
            }
        }

        // Отменить заказ
        public Command CancelCommand
        {
            get
            {
                return cancelCommand ?? (cancelCommand = new Command((obj) =>
                 {

                     foreach (var rightProduct in ScoreProducts)
                     {
                         var leftProduct = Products.Where(nameCheck => nameCheck.Name.Contains(rightProduct.Name)).FirstOrDefault();

                         if (leftProduct != null)
                         {
                             leftProduct.Count += rightProduct.Count;
                         }
                     }

                     ScoreProducts.Clear();

                     ResultCostFunction();

                 }));
            }
        }

        // Переместить из левого списка(склада) в правый(чек) определенное количество товара
        public Command MoveToChekCommand
        {
            get
            {
                return moveToCheckCommand ?? (moveToCheckCommand = new Command((obj) =>
                 {
                     if (obj is Product objProduct)
                     {
                         DialogWindow dialogWindow = new DialogWindow();
                         dialogWindow.blockDialogBox.Text = "Введите количество добавляемого товара:";

                         if (dialogWindow.ShowDialog() == true)
                         {
                             bool isInt = int.TryParse(dialogWindow.Text, out int data);

                             if (isInt)
                             {
                                 if (data <= 0) return;

                                 data = (objProduct.Count - data) < 0 ? objProduct.Count : data;
                                 var movedProduct = ScoreProducts.Where(nameCheck => nameCheck.Name.Contains(objProduct.Name)).FirstOrDefault();

                                 if (movedProduct != null)
                                 {
                                     movedProduct.Count += data;
                                     objProduct.Count -= data;
                                 }
                                 else
                                 {
                                     Product newProduct = new Product()
                                     {
                                         Name = objProduct.Name,
                                         Count = data,
                                         Cost = objProduct.Cost
                                     };

                                     ScoreProducts.Add(newProduct);
                                     objProduct.Count -= data;
                                 }
                             }

                         }

                         ResultCostFunction();
                     }
                 }, (obj) =>
                 {
                     if (obj is Product objProduct)
                     {
                         if (objProduct.Count > 0)
                         {
                             return true;
                         }
                         else
                         {
                             return false;
                         }
                     }
                     else return false;
                 }));
            }
        }

        // Переместить из правого списка(чек) в левый(склада) одну единицу товара
        public Command RemoveToChekCommand
        {
            get
            {
                return removeToChekCommand ?? (removeToChekCommand = new Command((obj) =>
                 {
                     if (obj is Product objProduct)
                     {
                         objProduct.Count -= 1;

                         var findedProduct = Products.Where(nameCheck => nameCheck.Name.Contains(objProduct.Name)).FirstOrDefault();

                         if (findedProduct != null)
                         {
                             findedProduct.Count += 1;
                         }

                         if (objProduct.Count == 0)
                         {
                             ScoreProducts.Remove(objProduct);
                         }

                         ResultCostFunction();

                     }
                 }, (obj) => ScoreProducts.Count > 0));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
