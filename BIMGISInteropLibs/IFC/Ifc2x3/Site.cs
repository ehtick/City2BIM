﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//embed BimGisCad
using BimGisCad.Representation.Geometry;

//embed Xbim                                    //below selected examples that show why these are included
using Xbim.Ifc;                                 //IfcStore
using Xbim.Ifc2x3.MeasureResource;              //Enumeration for Unit
using Xbim.Ifc2x3.ProductExtension;             //IfcSite

//Logging
using BIMGISInteropLibs.Logging;                                 //need for LogPair
using LogWriter = BIMGISInteropLibs.Logging.LogWriterIfcTerrain; //to set log messages

namespace BIMGISInteropLibs.IFC.Ifc2x3
{
    /// <summary>
    /// provides method to create IfcSite
    /// </summary>
    public static class Site
    {
        /// <summary>
        /// creates site in project
        /// </summary>
        /// <param name="model">Location for all information that will be inserted into the IFC file</param>
        /// <param name="name">Terrain designation</param>
        /// <param name="placement">Parameter provided by "createLocalPlacement"</param>
        /// <param name="refLatitude">Latitude</param>
        /// <param name="refLongitude">Longitude</param>
        /// <param name="refElevation">Height</param>
        /// <param name="compositionType">DO NOT CHANGE</param>
        /// <returns>IfcSite</returns>
        public static IfcSite Create(IfcStore model,
             string name,
             IFC.LoGeoRef loGeoRef,
             Axis2Placement3D placement = null,
             double? refLatitude = null,
             double? refLongitude = null,
             double? refElevation = null,
             IfcElementCompositionEnum compositionType = IfcElementCompositionEnum.ELEMENT)
        {
            //start transaction (according to ACID)
            using (var txn = model.BeginTransaction("Create Site"))
            {
                //Create new instance
                var site = model.Instances.New<IfcSite>(s =>
                {
                    //set terrain designation
                    s.Name = name;
                    s.CompositionType = compositionType; //DO NOT CHANGE
                    LogWriter.Add(LogType.verbose, "[IfcSite] Name ('" + s.Name + "') set.");

                    //set latitude and longitude
                    if (refLatitude.HasValue)
                    {
                        s.RefLatitude = IfcCompoundPlaneAngleMeasure.FromDouble(refLatitude.Value);
                        LogWriter.Add(LogType.verbose, "[IfcSite] Latitude ('" + s.RefLatitude.Value + "') set.");
                    }
                    if (refLongitude.HasValue)
                    {
                        s.RefLongitude = IfcCompoundPlaneAngleMeasure.FromDouble(refLongitude.Value);
                        LogWriter.Add(LogType.verbose, "[IfcSite] Longitude ('" + s.RefLongitude.Value + "') set.");
                    }
                    s.RefElevation = refElevation;

                    //get placement
                    placement = placement ?? Axis2Placement3D.Standard;

                    if(loGeoRef == IFC.LoGeoRef.LoGeoRef30)
                    {
                        //set local placement to site
                        s.ObjectPlacement = LoGeoRef.Level30.Create(model, placement);
                    }
                    
                });
                //commit transaction (acccording to ACID) otherwise the site would not be provided
                txn.Commit();
                LogWriter.Add(LogType.verbose, "[IfcSite] Transaction commited.");
                LogWriter.Add(LogType.debug, "[IfcSite] Site created.");
                return site;
            }
        }
    }
}