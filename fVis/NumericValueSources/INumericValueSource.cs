using System;

namespace fVis.NumericValueSources
{
    /// <summary>
    /// Interface that acts as mathematical function
    /// </summary>
    interface INumericValueSource
    {
        /// <summary>
        /// Return Y for given X
        /// </summary>
        /// <param name="x">X</param>
        /// <returns>Y</returns>
        double Evaluate(double x);
    }
}