﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//embed Xbmim
using Xbim.Ifc;
using Xbim.Ifc4.MeasureResource;                //IfcLabel

using Xbim.Ifc4.Kernel;                         //PropertySets
using Xbim.Ifc4.PropertyResource;               //PSetResources
using Xbim.Ifc4.ProductExtension;               //IfcSite (entity mapping)

//embed IFCTerrain - JsonSettings (input values)
using BIMGISInteropLibs.IfcTerrain;

namespace BIMGISInteropLibs.IFC.Ifc4
{
    /// <summary>
    /// Class for creating property sets<para/>
    /// Used in IFCTerrain to assign metadata
    /// </summary>
    public static class PropertySet
    {
        /// <summary>
        /// An ifcPopertySet for DTM metadata according to DIN91391 is created and added to the ifcSite entity
        /// </summary>
        /// <param name="model">exchange the whole ifc model</param>
        /// <param name="jSettings">json settings where meta data are stored</param>
        /// <returns></returns>
        public static void CreatePSetMetaDin91391(IfcStore model, configDin91391 jSettings)
        {
            //create entity
            var entity = model.Instances.OfType<IfcSite>().FirstOrDefault();

            //create property set
            var pSet = model.Instances.New<IfcPropertySet>(pset =>
            {
                //title of pset
                pset.Name = "Metadata DIN SPEC 91391-2";

                //description
                pset.Description = "Storage of metadata according to DIN SPEC 91391-2";

                //read out properties from object jSettings and add them to the property set
                pset.HasProperties.AddRange(new[] {
                    model.Instances.New<IfcPropertySingleValue>(p =>
                    {
                        p.Name = "Unique Identificator";
                        p.NominalValue = new IfcLabel(jSettings.guid.ToString());
                    }),
                    model.Instances.New<IfcPropertySingleValue>(p =>
                    {
                        p.Name = "Description";
                        if (!string.IsNullOrEmpty(jSettings.description))
                        {
                            p.NominalValue = new IfcText(jSettings.description);
                        }
                    }),
                    model.Instances.New<IfcPropertySingleValue>(p =>
                    {
                        p.Name = "Creator";
                        if (!string.IsNullOrEmpty(jSettings.creator))
                        {
                            p.NominalValue = new IfcText(jSettings.creator);
                        }
                    }),
                    model.Instances.New<IfcPropertySingleValue>(p =>
                    {
                        p.Name = "Revision";
                        if (!string.IsNullOrEmpty(jSettings.revision))
                        {
                            p.NominalValue = new IfcText(jSettings.revision);
                        }
                    }),
                    model.Instances.New<IfcPropertySingleValue>(p =>
                    {
                        p.Name = "Version";
                        if (!string.IsNullOrEmpty(jSettings.version))
                        {
                            p.NominalValue = new IfcText(jSettings.version);
                        }
                    }),
                    model.Instances.New<IfcPropertySingleValue>(p =>
                    {
                        p.Name = "Project ID";
                        if (!string.IsNullOrEmpty(jSettings.projectId))
                        {
                            p.NominalValue = new IfcText(jSettings.projectId);
                        }
                    }),
                    model.Instances.New<IfcPropertySingleValue>(p =>
                    {
                        p.Name = "Meta scheme";
                        if (!string.IsNullOrEmpty(jSettings.metadataSchema))
                        {
                            p.NominalValue = new IfcText(jSettings.metadataSchema);
                        }
                    }),
                });
            });

            //add property set to site
            entity.AddPropertySet(pSet);
            
            return;
        }

        /// <summary>
        /// An ifcPopertySet for DTM metadata according to DIN18740 is created and added to the ifcSite entity
        /// </summary>
        /// <param name="model">exchange the whole ifc model</param>
        /// <param name="jSettings">json settings where meta data are stored</param>
        public static void CreatePSetMetaDin18740(IfcStore model, configDin18740 settings)
        {
            //create entity
            var entity = model.Instances.OfType<IfcSite>().FirstOrDefault();

            //create property set
            var pSet = model.Instances.New<IfcPropertySet>(pset =>
            {
                //title of pset
                pset.Name = "Metadata DIN 18740-6";

                //description
                pset.Description = "Storage of metadata according to DIN 18740-6";

                //read out properties from object jSettings and add them to the property set
                pset.HasProperties.AddRange(new[] {
                    model.Instances.New<IfcPropertySingleValue>(p =>
                    {
                        p.Name = "Model type";
                        if (!string.IsNullOrEmpty(settings.modelType))
                        {
                            p.NominalValue = new IfcText(settings.modelType);
                        }
                    }),
                    model.Instances.New<IfcPropertySingleValue>(p =>
                    {
                        p.Name = "Data model";
                        if (!string.IsNullOrEmpty(settings.dataStructure.ToString()))
                        {
                            p.NominalValue = new IfcText(settings.dataStructure.ToString());
                        }
                    }),
                    model.Instances.New<IfcPropertySingleValue>(p =>
                    {
                        p.Name = "Topicality";
                        if (!string.IsNullOrEmpty(settings.topicality.ToShortDateString()))
                        {
                            p.NominalValue = new IfcText(settings.topicality.ToShortDateString());
                        }
                    }),

                    model.Instances.New<IfcPropertySingleValue>(p =>
                    {
                        p.Name = "Positon reference system";
                        if (!string.IsNullOrEmpty(settings.positionReferenceSystem))
                        {
                            p.NominalValue = new IfcText(settings.positionReferenceSystem);
                        }
                    }),
                    model.Instances.New<IfcPropertySingleValue>(p =>
                    {
                        p.Name = "Altitude reference system";
                        if (!string.IsNullOrEmpty(settings.altitudeReferenceSystem))
                        {
                            p.NominalValue = new IfcText(settings.altitudeReferenceSystem);
                        }
                    }),
                    model.Instances.New<IfcPropertySingleValue>(p =>
                    {
                        p.Name = "Projection";
                        if (!string.IsNullOrEmpty(settings.projection))
                        {
                            p.NominalValue = new IfcText(settings.projection);
                        }
                    })
                });
            });

            //add property set to site
            entity.AddPropertySet(pSet);

            return;
        }
    }
}
