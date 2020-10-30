using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticMosaicArtist_004
{
    public class Particle
    {
        public double[] geneValue;
        public double error;    //wartosc funkcji celu
        public double[] velocity;
        public double[] bestPosition;
        public double bestError;

        public Particle(double[] geneValues, double err, double[] vel, double[] bestPos, double bestErr)
        {
            this.geneValue = new double[geneValues.Length];
            geneValues.CopyTo(this.geneValue, 0);
            this.error = err;
            velocity = new double[vel.Length];
            vel.CopyTo(velocity, 0);
            this.bestPosition = new double[bestPos.Length];
            bestPos.CopyTo(this.bestPosition, 0);
            this.bestError = bestErr;
        }

        public override string ToString()
        {
            string s = "";
            s += "==========================\n";
            s += "GeneValue: ";
            for (int i = 0; i < this.geneValue.Length; ++i)
                s += this.geneValue[i].ToString("F4") + " ";
            s += "\n";
            s += "Error = " + this.error.ToString("F4") + "\n";
            s += "Velocity: ";
            for (int i = 0; i < this.velocity.Length; ++i)
                s += this.velocity[i].ToString("F4") + " ";
            s += "\n";
            s += "Best Position: ";
            for (int i = 0; i < this.bestPosition.Length; ++i)
                s += this.bestPosition[i].ToString("F4") + " ";
            s += "\n";
            s += "Best Error = " + this.bestError.ToString("F4") + "\n";
            s += "==========================\n";
            return s;
        }
    }
}
