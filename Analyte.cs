using System;


namespace ColumSim
{
    public class Analyte
    {
        public double rTransl; //nodimension: relative translation velocity of the analyte (relative to the eluent)
        public double k; // s-1: decompositon kinetic constant
        public double diffuseCoeff; //m2s-1: diffusion coefficient

        /* constructor */
        public Analyte(double givenRTransl,
                        double givenK,
                        double givenDiffuseCoeff)
        {
            rTransl = givenRTransl;
            k = givenK;
            diffuseCoeff = givenDiffuseCoeff;
        }
    }
}
