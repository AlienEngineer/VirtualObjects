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
    [TestFixture]
    public class ExcelSessionTests
    {

        /// <summary>
        /// 
        /// Gets all products in the file.
        /// 
        /// </summary>
        [Test]
        public void ExcelSession_GetData()
        {
            using ( var session = new ExcelSession("Excel\\book.xlsx") )
            {
                var count = session.GetAll<Product>().ToList().Count();

                Assert.That(count, Is.EqualTo(3));
            }
        }

        /// <summary>
        /// 
        /// Get data from a second sheet.
        /// 
        /// </summary>
        [Test]
        public void ExcelSession_GetData1()
        {
            using ( var session = new ExcelSession("Excel\\book.xlsx") )
            {
                var count = session.GetAll<Product1>().ToList().Count();

                Assert.That(count, Is.EqualTo(4));
            }
        }


        /// <summary>
        /// Get data with where clause.
        /// </summary>
        [Test]
        public void ExcelSession_GetData_Predicated()
        {
            using ( var session = new ExcelSession("Excel\\book.xlsx") )
            {
                var count = session.GetAll<Product>().Count(e => e.Artigo == 1);


                Assert.That(count, Is.EqualTo(1));
            }
        }

        /// <summary>
        /// Get data with where clause.
        /// </summary>
        [Test]
        public void ExcelSession_GetLotsOfData()
        {
            using ( var session = new ExcelSession("Excel\\People.xlsx") )
            {
                var count = session.GetAll<Person>()
                    .ToList()
                    .Count();

                Assert.That(count, Is.EqualTo(5000));
            }
        }

        [Test]
        public void ExcelSession_InsertPerson()
        {
            using ( var session = new ExcelSession("Excel\\NewPeople.xlsx") )
            {
                for ( int i = 0; i < 1000; i++ )
                {
                    session.Insert(new Person
                    {
                        Name = "Sérgio",
                        Age = 23,
                        Address = "Sérgio Address"
                    });
                }
            }
        }
    }

    class Person
    {
        public String Name { get; set; }
        public Double Age { get; set; }
        public String Address { get; set; }
        public String City { get; set; }
        public String Title { get; set; }
    }

    [Table(TableName = "Folha1")]
    class Product
    {
        public int Artigo { get; set; }

        [Column(FieldName = "Descrição")]
        public String Descricao { get; set; }

        public Double Quantidade { get; set; }

        [Column(FieldName = "Preço Unitário")]
        public Double PrecUnit { get; set; }
    }

    [Table(TableName = "Folha2")]
    class Product1
    {
        [Column(FieldName = "A")]
        public int Artigo { get; set; }

        [Column(FieldName = "B")]
        public String Descricao { get; set; }

        [Column(FieldName = "C")]
        public Double Quantidade { get; set; }

        [Column(FieldName = "D")]
        public Double PrecUnit { get; set; }
    }
}