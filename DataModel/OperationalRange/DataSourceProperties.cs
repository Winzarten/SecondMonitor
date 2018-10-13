namespace SecondMonitor.DataModel.OperationalRange
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Extensions;

    [Serializable]
    public class DataSourceProperties
    {
        public DataSourceProperties()
        {
            TyreCompoundsProperties = new List<TyreCompoundProperties>();
            CarModelsProperties = new List<CarModelProperties>();
        }

        public List<TyreCompoundProperties> TyreCompoundsProperties { get; set; }
        public List<CarModelProperties> CarModelsProperties { get; set; }
        public string SourceName { get; set; }

        public void OverrideWith(DataSourceProperties overridingProperties)
        {
            SourceName = overridingProperties.SourceName;

            TyreCompoundsProperties.RemoveAll(x => overridingProperties.TyreCompoundsProperties.Any(f => f.CompoundName == x.CompoundName));
            TyreCompoundsProperties.AddRange(overridingProperties.TyreCompoundsProperties);
            CarModelsProperties.RemoveAll(x => overridingProperties.CarModelsProperties.Any(f => f.Name == x.Name));
            CarModelsProperties.AddRange(overridingProperties.CarModelsProperties);
        }

        public CarModelProperties GetCarModel(string modelName)
        {
            return CarModelsProperties.FirstOrDefault(x => x.Name == modelName);
        }

        public void AddCarModel(CarModelProperties newCarModel)
        {
           CarModelsProperties.Add(newCarModel);
        }

        public void ReplaceCarModel(CarModelProperties replaceCarModelProperties)
        {
            CarModelsProperties.RemoveAll(x => x.Name == replaceCarModelProperties.Name);
            CarModelsProperties.Add(replaceCarModelProperties);
        }

        public TyreCompoundProperties GetTyreCompound(string compoundName)
        {
            return TyreCompoundsProperties.FirstOrDefault(x => x.CompoundName == compoundName);
        }

        public void AddTyreCompound(TyreCompoundProperties newCompound)
        {
            TyreCompoundsProperties.Add(newCompound);
        }
    }
}