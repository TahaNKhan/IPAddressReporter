using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace IPAddressReporter.Exceptions
{
    public class CombinedException : Exception
    {
        public List<Exception> InnerExceptions { get; set; }
        public CombinedException(params Exception[] exceptions)
        {
            InnerExceptions = new List<Exception>(exceptions);
        }

        public CombinedException(IEnumerable<Exception> innerExceptions)
        {
            InnerExceptions = innerExceptions.ToList();
        }

        public override string ToString() => 
            string.Join(",", InnerExceptions.Select(s => s.ToString())) + base.ToString();
    }
}
