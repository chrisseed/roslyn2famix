using Fame;
using System;
using FILE;
using Dynamix;
using FAMIX;
using System.Collections.Generic;

namespace FAMIX
{
  [FamePackage("FAMIX")]
  [FameDescription("Access")]
  public class Access : FAMIX.Association
  {
    [FameProperty(Name = "accessor",  Opposite = "accesses")]    
    public BehaviouralEntity accessor { get; set; }
    
    [FameProperty(Name = "isReadWriteUnknown")]    
    public Boolean isReadWriteUnknown { get; set; }
    
    [FameProperty(Name = "isRead")]    
    public Boolean isRead { get; set; }
    
    [FameProperty(Name = "isWrite")]    
    public Boolean isWrite { get; set; }
    
    [FameProperty(Name = "variable",  Opposite = "incomingAccesses")]    
    public StructuralEntity variable { get; set; }
    
  }
}