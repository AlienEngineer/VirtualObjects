using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace VirtualObjects.Tests
{
    abstract class UtilityBelt
    {
        public UtilityBelt()
        {
            InitBelt();
        }
  
        private void InitBelt()
        {
            Connection = new SqlConnection(@"
                      Data Source=(LocalDB)\v11.0;
                      AttachDbFilename=" + Environment.CurrentDirectory + @"\Data\northwnd.mdf;
                      Integrated Security=True;
                      Connect Timeout=30");
        }

        public IDbConnection Connection { get; private set; }
    }
}
