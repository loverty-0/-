using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Yakupova_A_Obuv
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
            var list = new List<ProductViewModel>();

            try
            {
                using (var db = new Obuv_YakupovaEntities1())
                {
                    var products = db.Product.ToList();

                    foreach (var p in products)
                    {
                        // Общее количество на складе
                        int totalQty = 0;
                        var positions = db.CommodityPosition
                            .Where(cp => cp.ProductID == p.ProductID)
                            .ToList();

                        foreach (var cp in positions)
                        {
                            totalQty += cp.QuantityInStock;
                        }

                        // Производитель
                        var manufacturer = db.Manufacture
                            .FirstOrDefault(m => m.ManufactureID == p.ManufactureID);

                        // Категория
                        var category = db.Category
                            .FirstOrDefault(c => c.CategoryID == p.CategoryID);

                        var vm = new ProductViewModel
                        {
                            ProductId = p.ProductID,
                            ImagePath = GetImagePath(p.Images),
                            Manufacturer = manufacturer != null ? manufacturer.Manufacture1 : "",
                            ProductName = p.ProductName,
                            CategoryName = category != null ? category.CategoryName : "",
                            Composition = p.Compound ?? "",
                            Price = p.Price.ToString("N0"),
                            QuantityLabel = totalQty > 5 ? "много" : "мало",
                            RowBackground = totalQty <= 3
                                ? new SolidColorBrush(Color.FromRgb(0xFF, 0x80, 0x80))
                                : Brushes.White
                        };

                        list.Add(vm);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message);
            }

            icProducts.ItemsSource = list;
        }

        private string GetImagePath(string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
                return "pack://application:,,,/Resources/picture.png";

            return "pack://application:,,,/Resources/" + imageName;
        }
    }

    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public string ImagePath { get; set; }
        public string Manufacturer { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public string Composition { get; set; }
        public string Price { get; set; }
        public string QuantityLabel { get; set; }
        public Brush RowBackground { get; set; }
    }
}