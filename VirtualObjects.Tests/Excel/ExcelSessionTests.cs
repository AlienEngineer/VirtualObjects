using System;
using System.Linq;
using NUnit.Framework;
using VirtualObjects.Mappings;

namespace VirtualObjects.Tests.Excel
{
    /// <summary>
    ///
    /// Excel session unit tests
    ///
    /// Author: Sérgio
    /// </summary>
    [TestFixture, Category("Excel")]
    public class ExcelSessionTests
    {

        private static void CaptureIfProviderIsMissing(Action action)
        {
            try
            {
                action();
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("'Microsoft.ACE.OLEDB.12.0' provider is not registered"))
                {
                    Assert.Inconclusive(
                        "The 'Microsoft.ACE.OLEDB.12.0' provider is not registered on the local machine.");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        ///
        /// Gets all products in the file.
        ///
        /// </summary>
        [Test]
        public void ExcelSession_GetData()
        {
            CaptureIfProviderIsMissing(() =>
            {
                using (var session = new ExcelSession("Excel\\book.xlsx", new SessionConfiguration {Logger = Console.Out}))
                {
                    var count = session.GetAll<Product>().ToList().Count();

                    Assert.That(count, Is.EqualTo(3));
                }
            });
        }

        /// <summary>
        ///
        /// Get data from a second sheet.
        ///
        /// </summary>
        [Test]
        public void ExcelSession_GetData1()
        {
            CaptureIfProviderIsMissing(() =>
            {
                using (var session = new ExcelSession("Excel\\book.xlsx"))
                {
                    var count = session.GetAll<Product1>().ToList().Count();


                    Assert.That(count, Is.EqualTo(4));
                }
            });
        }


        /// <summary>
        /// Get data with where clause.
        /// </summary>
        [Test]
        public void ExcelSession_GetData_Predicated()
        {
            CaptureIfProviderIsMissing(() =>
            {
                using (var session = new ExcelSession("Excel\\book.xlsx"))
                {
                    var count = session.GetAll<Product>().Count(e => e.Artigo == 1);


                    Assert.That(count, Is.EqualTo(1));
                }
            });
        }

        /// <summary>
        /// Get data with where clause.
        /// </summary>
        [Test]
        public void ExcelSession_GetLotsOfData()
        {
            CaptureIfProviderIsMissing(() =>
            {
                using (var session = new ExcelSession("Excel\\People.xlsx"))
                {
                    var count = session.GetAll<Person>()
                        .ToList()
                        .Count();

                    Assert.That(count, Is.EqualTo(5000));
                }
            });
        }

        [Test]
        public void ExcelSession_InsertPerson()
        {
            CaptureIfProviderIsMissing(() =>
            {
                using (var session = new ExcelSession("Excel\\NewPeople.xlsx", new SessionConfiguration
                {
                    Logger = Console.Out,
                    SaveGeneratedCode = true
                }))
                {
                    session.KeepAlive(() =>
                    {
                        for (int i = 0; i < 1000; i++)
                        {
                            session.Insert(new Person
                            {
                                Name = "Sérgio",
                                Age = 23,
                                Address = "Sérgio Address",
                                Active = true
                            });
                        }
                    });

                    var people = session.GetAll<Person>().ToList();

                    Assert.That(people, Is.Not.Empty);
                }
            });
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public double Age { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Title { get; set; }
        public bool Active { get; set; }
    }

    [Table(TableName = "Folha1")]
    public class Product
    {
        public int Artigo { get; set; }

        [Column(FieldName = "Descrição")]
        public string Descricao { get; set; }

        public double Quantidade { get; set; }

        [Column(FieldName = "Preço Unitário")]
        public double PrecUnit { get; set; }
    }

    [Table(TableName = "Folha2")]
    public class Product1
    {
        [Column(FieldName = "A")]
        public int Artigo { get; set; }

        [Column(FieldName = "B")]
        public string Descricao { get; set; }

        [Column(FieldName = "C")]
        public double Quantidade { get; set; }

        [Column(FieldName = "D")]
        public double PrecUnit { get; set; }
    }
}
