using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Adapter
{
    /// <summary>
    /// Адаптер для работы с иностранными кассовыми аппаратами как с обычными.
    /// </summary>
    public class ForeignCashMachineAdapter : ICashMachine
    {
        /// <summary>
        /// Иностранный кассовый аппарат.
        /// </summary>
        private ForeignCashMachine.ForeignCashMachine _foreignCashMachine;

        /// <inheritdoc />
        public string Number => _foreignCashMachine.Name;

        /// <inheritdoc />
        public IEnumerable<Product> Products
        {
            get
            {
                var productsTuple = _foreignCashMachine.CurrentCheck.Products;
                var products = productsTuple.Select(s => new Product(s.Name, Convert.ToDecimal(s.Price)));
                return products;
            }
        }

        /// <summary>
        /// Создать экземпляр адаптера иностранного кассового аппарата под обычный.
        /// </summary>
        /// <param name="foreignCashMachine">Иностранный кассовый аппарат.</param>
        public ForeignCashMachineAdapter(ForeignCashMachine.ForeignCashMachine foreignCashMachine)
        {
            _foreignCashMachine = foreignCashMachine;
        }

        /// <inheritdoc />
        public void AddProduct(Product product)
        {
            _foreignCashMachine.Add(product.Name, Convert.ToDouble(product.Price));
        }

        /// <inheritdoc />
        public string PrintCheck()
        {
            var check = _foreignCashMachine.Save();
            var checkText = GetCheckText(check);
            Save(checkText);
            return checkText;
        }

        /// <inheritdoc />
        public void Save(string checkText)
        {
            using (var sr = new StreamWriter("checks2.txt", true, Encoding.Default))
            {
                sr.WriteLine(checkText);
            }
        }

        /// <summary>
        /// Сформировать текст чека для вывода на печать и сохранения в файл.
        /// </summary>
        /// <param name="check">Чек иностранного кассового аппарата.</param>
        /// <returns>Форматированный текст чека.</returns>
        private string GetCheckText(ForeignCashMachine.Check check)
        {
            var date = check.DateTime.ToString("dd MMMM yyyy HH:mm");
            var checkText = $"Кассовый чек от {date}\r\n";
            checkText += $"Идентификатор чека: {check.Number}\r\n";
            checkText += "Список товаров:\r\n";
            foreach (var (Name, Price) in check.Products)
            {
                checkText += $"{Name}\t\t\t{Price}\r\n";
            }
            var sum = check.Products.Sum(p => p.Price);
            checkText += $"ИТОГО\t\t\t{sum}\r\n";
            return checkText;
        }
    }
}
