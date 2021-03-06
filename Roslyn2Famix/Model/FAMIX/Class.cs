using Dynamix;
using Fame;
using FAMIX;
using FILE;
using System;
using System.Collections.Generic;

namespace FAMIX
{
    [FamePackage("FAMIX")]
    [FameDescription("Class")]
    public class Class : FAMIX.Type
    {
        private List<FAMIX.Exception> exceptions = new List<FAMIX.Exception>();

        [FameProperty(Name = "exceptions", Opposite = "exceptionClass")]
        public List<FAMIX.Exception> Exceptions
        {
            get { return exceptions; }
            set { exceptions = value; }
        }
        public void AddException(FAMIX.Exception one)
        {
            exceptions.Add(one);
        }

        [FameProperty(Name = "isInterface")]
        public Boolean isInterface { get; set; }

    }
}
