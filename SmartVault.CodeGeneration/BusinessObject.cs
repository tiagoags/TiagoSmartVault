using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace SmartVault.Library
{
    [XmlRoot(ElementName = "property")]
    public class Property
    {
        [XmlAttribute(AttributeName = "name")]
        public string? Name { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string? Type { get; set; }
    }

    [XmlRoot(ElementName = "propertyGroup")]
    public class PropertyGroup
    {
        [XmlElement(ElementName = "property")]
        public List<Property>? Property { get; set; }

        public IEnumerable<Property> BusinessObjectProperties
        {
            get { return Property.Concat(_commonProperties); }
        }

        private IEnumerable<Property> _commonProperties = new List<Property>() { new() { Name = "CreatedOn", Type = typeof(DateTime).FullName } };
    }

    [XmlRoot(ElementName = "businessObject")]
    public class BusinessObject
    {
        [XmlElement(ElementName = "propertyGroup")]
        public PropertyGroup? PropertyGroup { get; set; }

        [XmlElement(ElementName = "script")]
        public string? Script { get; set; }

        public string? ScriptWithCreatedOn
        {
            get
            {
                return this.Script
                    .Replace( //TODO improve on how we automatically insert CreatedOn column on all businessObject scripts
                    ");",
                    ", CreatedOn datetime default (datetime(current_timestamp)) );"
                    );
            }
        }
    }
}
