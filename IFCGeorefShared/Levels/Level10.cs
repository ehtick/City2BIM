﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Text;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFCGeorefShared.Levels
{
    public class Level10 : Level00
    {

        public IIfcPostalAddress? PostalAddress { get; set; }


        /*public Level10(IfcProduct referencedProduct, IfcPostalAddress PostalAddress)
        {
            this.ReferencedEntity = referencedProduct;
            this.PostalAddress = PostalAddress;
        }
        */

        
    }
}
