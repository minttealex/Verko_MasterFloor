using System;
using System.Linq;

namespace Verko_MasterFloor.MaterialCalculation
{
    public static class MaterialCalculator
    {
        /// <summary>
        /// Расчет количества материала для производства продукции
        /// </summary>
        /// <param name="productTypeId"></param>
        /// <param name="materialTypeId"></param>
        /// <param name="productQuantity"></param>
        /// <param name="parameter1"></param>
        ///<param name="parameter2">
        public static int CalculateRequiredMaterial(int productTypeId, int materialTypeId,
            int productQuantity, double parameter1, double parameter2)
        {
            if (productQuantity <= 0 || parameter1 <= 0 || parameter2 <= 0)
                return -1;

            try
            {
                using (var db = new Verko_MasterFloorEntities())
                {
                    var productType = db.ProductTypes.FirstOrDefault(pt => pt.ProductTypeID == productTypeId);
                    if (productType == null)
                        return -1;

                    var materialType = db.MaterialTypes.FirstOrDefault(mt => mt.MaterialTypeID == materialTypeId);
                    if (materialType == null)
                        return -1;

                    double materialPerUnit = parameter1 * parameter2 * (double)productType.ProductCoefficient;

                    double defectMultiplier = 1.0 + (double)materialType.MaterialTypePercent;

                    double totalMaterial = materialPerUnit * productQuantity * defectMultiplier;

                    return (int)Math.Ceiling(totalMaterial);
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}