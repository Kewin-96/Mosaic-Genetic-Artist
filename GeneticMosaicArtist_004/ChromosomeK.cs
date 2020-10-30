using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticMosaicArtist_004
{
    public class ChromosomeK
    {
        public double[] mins;
        public double[] maxs;
        public double[] geneValues;
        public ChromosomeK(double[] minimumValues, double[] maximumValues, double[] geneValues)
        {
            mins = new double[minimumValues.Length];
            minimumValues.CopyTo(mins,0);
            maxs = new double[maximumValues.Length];
            maximumValues.CopyTo(maxs,0);
            this.geneValues = new double[geneValues.Length];
            geneValues.CopyTo(this.geneValues,0);
        }
    }
}
