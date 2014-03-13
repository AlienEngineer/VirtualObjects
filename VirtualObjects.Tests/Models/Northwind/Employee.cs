using System;
using System.Linq;
using VirtualObjects.Mappings;

namespace VirtualObjects.Tests.Models.Northwind
{

    [Table(TableName = "Employees")]
    public class Employee
    {
        [Identity]
        public int EmployeeId { get; set; }

        public String LastName { get; set; }

        public String FirstName { get; set; }

        public String Title { get; set; }

        public String TitleOfCourtesy { get; set; }

        public DateTime BirthDate { get; set; }

        public DateTime HireDate { get; set; }

        public String Address { get; set; }

        public String City { get; set; }

        public String Region { get; set; }

        public String PostalCode { get; set; }

        public String Country { get; set; }

        public String HomePhone { get; set; }

        public String Extension { get; set; }

        public String Notes { get; set; }

        public Byte[] Photo { get; set; }

        [Association(FieldName = "ReportsTo", OtherKey = "EmployeeId")]
        public virtual Employee ReportsTo { get; set; }

        public String PhotoPath { get; set; }

        public virtual IQueryable<EmployeeTerritories> Territories { get; set; }

        [Ignore]
        public bool NonExistingField { get; set; }

        public Byte[] Version { get; set; }
    }

    public class EmployeeProxy : VirtualObjects.Tests.Models.Northwind.Employee
    {
        private ISession Session { get; set; }

        public EmployeeProxy(ISession session)
        {
            Session = session;
        }


        VirtualObjects.Tests.Models.Northwind.Employee _ReportsTo;
        Boolean _ReportsToLoaded;

        public override VirtualObjects.Tests.Models.Northwind.Employee ReportsTo
        {
            get
            {
                if ( !_ReportsToLoaded )
                {
                    _ReportsTo = Session.GetById(_ReportsTo);
                    _ReportsToLoaded = _ReportsTo != null;
                }

                return _ReportsTo;
            }
            set
            {
                _ReportsTo = value;
            }
        }

        IQueryable<VirtualObjects.Tests.Models.Northwind.EmployeeTerritories> _Territories;

        public override IQueryable<VirtualObjects.Tests.Models.Northwind.EmployeeTerritories> Territories
        {
            get
            {
                return _Territories ?? 
                    Session.GetAll<VirtualObjects.Tests.Models.Northwind.EmployeeTerritories>()
                        .Where(e => e.Employee == this);
            }
            set
            {
                _Territories = value;
            }
        }

    }

}
