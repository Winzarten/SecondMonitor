namespace SecondMonitor.DataModel.OperationalRange
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SecondMonitor.DataModel.Extensions;

    [Serializable]
    public class DataSourceProperties
    {
        public DataSourceProperties()
        {
            TyreCompoundProperties = new Dictionary<string, TyreCompoundProperties>();
            CarModelsProperties = new Dictionary<string, CarModelProperties>();
        }

        public Dictionary<string, TyreCompoundProperties> TyreCompoundProperties { get; set; }
        public Dictionary<string, CarModelProperties> CarModelsProperties { get; set; }
        public string SourceName { get; set; }

        public void OverrideWith(DataSourceProperties overridingProperties)
        {
            SourceName = overridingProperties.SourceName;

            TyreCompoundProperties.RemoveAll(x => overridingProperties.TyreCompoundProperties.Keys.Contains(x));
            overridingProperties.TyreCompoundProperties.ForEach( x => TyreCompoundProperties[x.Key] = x.Value );

            CarModelsProperties.RemoveAll(x => overridingProperties.CarModelsProperties.Keys.Contains(x));
            overridingProperties.CarModelsProperties.ForEach(x => CarModelsProperties[x.Key] = x.Value);
        }
    }
}