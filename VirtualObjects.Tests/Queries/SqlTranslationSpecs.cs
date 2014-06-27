using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Machine.Specifications;
using Machine.Specifications.Model;
using VirtualObjects.Queries;
using VirtualObjects.Tests.Models.Northwind;

namespace VirtualObjects.Tests.Queries
{
    [Tags("QueryTranslation")]
    public abstract class SqlTranslationSpecs
    {
        Establish context = () =>
        {
            Session = new Session(new SessionConfiguration
            {
                Logger = Console.Out,
                SaveGeneratedCode = true
            }, "northwind");
        };

        protected static Session Session;
        protected static string Translation;

        protected static void Translate<T>(IQueryable<T> query)
        {
            Translation = query.ToString();
        }

    }


    [Subject(typeof (IQueryTranslator))]
    public class When_Translating_a_query_with_collation : SqlTranslationSpecs
    {
        Establish context = () =>
        {
            Session = new Session(new SessionConfiguration
            {
                Logger = Console.Out,
                UniformeCollations = true
            }, "northwind");
        };

        private Because of = () => Translate(
            Session.Query<Employee>()
                   .Where(e => e.LastName == "testing")
                   .Select(e => new
                   {
                       EmployeeId = e.EmployeeId,
                       LastName = e.LastName,
                       FirstName = e.FirstName
                   })
        );

        private It should_match =
            () => Translation.Should().Be("Select [T0].[EmployeeId], [T0].[LastName] collate Latin1_General_CI_AS [LastName], [T0].[FirstName] collate Latin1_General_CI_AS [FirstName] From [Northwind].[dbo].[Employees] [T0] Where ([T0].[LastName] collate Latin1_General_CI_AS = @p0)");
    }

}
