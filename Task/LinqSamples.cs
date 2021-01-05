// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using SampleSupport;
using Task.Data;

// Version Mad01

namespace SampleQueries
{
	[Title("LINQ Module")]
	[Prefix("Linq")]
	public class LinqSamples : SampleHarness
	{

		private DataSource dataSource = new DataSource();

		[Category("Restriction Operators")]
		[Title("Where - Task 1")]
		[Description("This sample uses the where clause to find all elements of an array with a value less than 5.")]
		public void Linq1()
		{
			int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

			var lowNums =
				from num in numbers
				where num < 5
				select num;

			Console.WriteLine("Numbers < 5:");
			foreach (var x in lowNums)
			{
				Console.WriteLine(x);
			}
		}

		[Category("Restriction Operators")]
		[Title("Where - Task 2")]
		[Description("This sample return return all presented in market products")]

		public void Linq2()
		{
			var products =
				from p in dataSource.Products
				where p.UnitsInStock > 0
				select p;

			foreach (var p in products)
			{
				ObjectDumper.Write(p);
			}
		}


        [Category("Restriction Operators")]
        [Title("Where - Task 3")]
        [Description("Выдайте список всех клиентов, чей суммарный оборот (сумма всех заказов) превосходит некоторую величину X. Продемонстрируйте выполнение запроса с различными X (подумайте, можно ли обойтись без копирования запроса несколько раз)")]

        public void Linq001()
        {
            int x = 50000;
            var _customersList = dataSource.Customers

                .Where(_fieldOne => _fieldOne.Orders.Sum(_fieldTwo => _fieldTwo.Total) > x)
                .Select(_fieldOne => new
                {
                    CustomerId = _fieldOne.CustomerID,
                    Sum = _fieldOne.Orders.Sum(_fieldTwo => _fieldTwo.Total)
                });

            Console.WriteLine("More then 50000");

            foreach (var _fieldOne in _customersList)
            {
                ObjectDumper.Write("CustomerId = " + _fieldOne.CustomerId + "TotalSum = " + _fieldOne.Sum);
            }

            x = 100000;

            Console.WriteLine("More then 100000");

            foreach (var _fieldOne in _customersList)
            {
                ObjectDumper.Write("CustomerId = " + _fieldOne.CustomerId + "TotalSum = " + _fieldOne.Sum);
            }
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 4")]
        [Description("Для каждого клиента составьте список поставщиков, находящихся в той же стране и том же городе. Сделайте задания с использованием группировки и без.")]

        public void Linq002()
        {
            var _supplierList = dataSource.Customers
                .Select(_fieldOne => new
                {
                    _customer = _fieldOne,
                    _supplier = dataSource.Suppliers.Where(_fieldTwo => _fieldTwo.City == _fieldOne.City && _fieldTwo.Country == _fieldOne.Country)
                });

            Console.WriteLine("No grouping");

            foreach (var _fieldOne in _supplierList)
            {
                ObjectDumper.Write($"CustomerId: {_fieldOne._customer.CustomerID} " +
                    $"List of suppliers: {string.Join(", ", _fieldOne._supplier.Select(s => s.SupplierName))}");
            }

            var result = dataSource.Customers.GroupJoin(dataSource.Suppliers,
                       _fieldOne => new
                       {
                           _fieldOne.City,
                           _fieldOne.Country
                       },

                       _fieldTwo => new
                       {
                           _fieldTwo.City,
                           _fieldTwo.Country
                       },
            (_fieldOne, _fieldTwo) => new
            {
                _customer = _fieldOne,
                _supplier = _fieldTwo
            });

            Console.WriteLine("With grouping");

            foreach(var _fieldOne in result)
            {
                ObjectDumper.Write($"CustomerId: {_fieldOne._customer.CustomerID} " +
                    $"List of suppliers: {string.Join(", ", _fieldOne._supplier.Select(s => s.SupplierName))}");
            }
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 5")]
        [Description("Найдите всех клиентов, у которых были заказы, превосходящие по сумме величину X")]

        public void Linq003()
        {
            int x = 5000;
            var _customersList = dataSource.Customers
                .Where(_fieldOne => _fieldOne.Orders.Any(_fieldTwo => _fieldTwo.Total > x));
                                                    
            foreach (var _fieldOne in _customersList)
            {
                ObjectDumper.Write(_fieldOne);
            }
        }


        [Category("Restriction Operators")]
        [Title("Where - Task 6")]
        [Description("Выдайте список клиентов с указанием, начиная с какого месяца какого года они стали клиентами (принять за таковые месяц и год самого первого заказа)")]

        public void Linq004()
        {
            var _customersList = dataSource.Customers
                .Where(_fieldOne => _fieldOne.Orders.Any())
                .Select(_fieldOne => new
                {
                    _customerId = _fieldOne.CustomerID,
                    _date = _fieldOne.Orders.OrderBy(_fieldTwo => _fieldTwo.OrderDate).Select(_fieldTwo => _fieldTwo.OrderDate).First()
                });

            foreach (var _fieldOne in _customersList)
            {
                ObjectDumper.Write(" CustomersId =  " + _fieldOne._customerId + " Month = " + _fieldOne._date.Month + " Year = " + _fieldOne._date.Year);
            }
        }


        [Category("Restriction Operators")]
        [Title("Where - Task 7")]
        [Description("Сделайте предыдущее задание, но выдайте список отсортированным по году, месяцу, оборотам клиента (от максимального к минимальному) и имени клиента")]

        public void Linq005()
        {
            var _customersList = dataSource.Customers
               .Where(_fieldOne => _fieldOne.Orders.Any())
               .Select(_fieldOne => new
               {
                   _customerId = _fieldOne.CustomerID,
                   _date = _fieldOne.Orders.OrderBy(_fieldTwo => _fieldTwo.OrderDate).Select(_fieldTwo => _fieldTwo.OrderDate).First(),
                   _sum = _fieldOne.Orders.Sum(_fieldTwo => _fieldTwo.Total)
               })
               .OrderByDescending(_fieldOne => _fieldOne._date.Year)
               .OrderByDescending(_fieldOne => _fieldOne._date.Month)
               .OrderByDescending(_fieldOne => _fieldOne._sum)
               .OrderByDescending(_fieldOne => _fieldOne._customerId);

            foreach (var _fieldOne in _customersList)
            {
                ObjectDumper.Write(" CustomersId = " +  _fieldOne._customerId + " Sum = " + _fieldOne._sum + " Month = " + _fieldOne._date.Month + " Year = " + _fieldOne._date.Year);
            }
        }


        [Category("Restriction Operators")]
        [Title("Where - Task 8")]
        [Description("Укажите всех клиентов, у которых указан нецифровой почтовый код или не заполнен регион или в телефоне не указан код оператора (для простоты считаем, что это равнозначно «нет круглых скобочек в начале»).")]

        public void Linq006()
        {
            var _customersList = dataSource.Customers
               .Where(_fieldOne => _fieldOne.PostalCode != null && _fieldOne.PostalCode.Any(_numbs => _numbs < '0' || _numbs > '9') || string.IsNullOrWhiteSpace(_fieldOne.Region) || _fieldOne.Phone.FirstOrDefault() != '('); 
               
            foreach(var _fieldOne in _customersList)
            {
                ObjectDumper.Write(_fieldOne);
            }
        }

        [Category("Restriction Operators")]
        [Title("Where - Task 9")]
        [Description("Сгруппируйте все продукты по категориям, внутри – по наличию на складе, внутри последней группы отсортируйте по стоимости")]

        public void Linq007()
        {
            var _products = dataSource.Products.OrderBy(products => products.Category)
                .ThenBy(products => products.UnitsInStock)
                .ThenBy(products => products.UnitPrice);

            foreach(var _fieldOne in _products)
            {
                ObjectDumper.Write(_fieldOne.Category + "      "  + _fieldOne.UnitsInStock + "      "  + _fieldOne.UnitPrice);
            }
        }


        [Category("Restriction Operators")]
        [Title("Where - Task 10")]
        [Description("Сгруппируйте товары по группам «дешевые», «средняя цена», «дорогие». Границы каждой группы задайте сами")]

        public void Linq008()
        {
            var _cheapPrice = 12;
            var _mediumPrice = 100;

            var _firstProducts = dataSource.Products.Where(_fieldOne => _fieldOne.UnitPrice <= _cheapPrice);
            var _secondProducts = dataSource.Products.Where(_fieldOne => _fieldOne.UnitPrice <= _mediumPrice && _fieldOne.UnitPrice >= _cheapPrice);
            var _thirdProducts = dataSource.Products.Where(_fieldOne => _fieldOne.UnitPrice >= _mediumPrice);
            var _products = _firstProducts.Concat(_secondProducts).Concat(_thirdProducts);
            
            foreach (var _fieldOne in _products)
            {
                ObjectDumper.Write(_fieldOne.ProductName + "    " + _fieldOne.UnitPrice);
            }

        }

        [Category("Restriction Operators")]
        [Title("Where - Task 11")]
        [Description("Рассчитайте среднюю прибыльность каждого города (среднюю сумму заказа по всем клиентам из данного города) и среднюю интенсивность (среднее количество заказов, приходящееся на клиента из каждого города)")]

        public void Linq009()
        {
            var _outcome = dataSource.Customers
                .GroupBy(_fieldOne => _fieldOne.City)
                .Select(_fieldOne => new
                {
                    _city= _fieldOne.Key,
                    _intensive = _fieldOne.Average(_fieldTwo => _fieldTwo.Orders.Length),
                    _averageSalary = _fieldOne.Average(_fieldTwo => _fieldTwo.Orders.Sum(_salary => _salary.Total))
                });

            foreach (var _fieldOne in _outcome)
            {
                ObjectDumper.Write(" City " + _fieldOne._city + "; Intensive " + _fieldOne._intensive + "; Salary " + _fieldOne._averageSalary);
                //ObjectDumper.Write(" Intensive " + _fieldOne._intensive);
                //ObjectDumper.Write(" Salary " + _fieldOne._averageSalary);
            }
        }


        [Category("Restriction Operators")]
        [Title("Where - Task 12")]
        [Description("Сделайте среднегодовую статистику активности клиентов по месяцам (без учета года), статистику по годам, по годам и месяцам (т.е. когда один месяц в разные годы имеет своё значение).")]

        public void Linq010()
        {
            var _stata = dataSource.Customers
                .Select(_fieldOne => new
                {
                    _fieldOne.CustomerID,
                    _stataForMonths = _fieldOne.Orders.GroupBy(_fieldTwo => _fieldTwo.OrderDate.Month).Select(_fieldThree => new
                    {
                        _month = _fieldThree.Key, _count = _fieldThree.Count()
                    }),
                    _stataForYear = _fieldOne.Orders.GroupBy(_fieldTwo => _fieldTwo.OrderDate.Year).Select(_fieldThree => new
                    {
                        _year = _fieldThree.Key, _count = _fieldThree.Count()
                    }),
                    _stataForYearAndMounts = _fieldOne.Orders.GroupBy(_fieldTwo => new
                    {
                        _fieldTwo.OrderDate.Year,
                        _fieldTwo.OrderDate.Month
                    })
                    .Select(_fieldThree => new
                    {
                        _fieldThree.Key.Year,
                        _fieldThree.Key.Month,
                        _count = _fieldThree.Count()
                    })
                });

            foreach (var best in _stata)
            {
                ObjectDumper.Write(" CustomersId " + best.CustomerID);

                Console.WriteLine(" Stata of month: ");

                foreach (var _month in best._stataForMonths)
                {
                    ObjectDumper.Write(" Month " + _month._month + " Count " + _month._count);
                }

                Console.WriteLine(" Stata of years: ");

                foreach(var _year in best._stataForYear)
                {
                    ObjectDumper.Write(" Years " + _year._year + " Count " + _year._count);
                }

                Console.WriteLine("Years and Month: ");

                foreach(var _yearsAndMonth in best._stataForYearAndMounts)
                {
                    ObjectDumper.Write(" Year " + _yearsAndMonth.Year + " Month " + _yearsAndMonth.Month + " Count " + _yearsAndMonth._count);
                }
            }
        }
    }
}
