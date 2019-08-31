using System;

namespace ColumSim
{
    public class Column
    {
        /* Primary parameters */
        public Analyte analyte; //analyte in this chromatography
        public double injectedConc; //injected conceetration of the analyte
        public double lenColumn; //column length
        public int divColumn; //division numbers of the column
        public double velElu; //eluent velocity
        public double tTotal; // total calculatin (simulation) time
        public double dt; // time fraction. one clock corresponds to dt

        /* Secondary parameters */
        public double dL; //cell length
        public int translCount; //clocks one translation takes
        public int numTransl; //the number traslation occurs throughout the simulation
        public int numCells; // the number of cells needed to perform the simulation

        /* Working variables */
        public int idxColumnHead; // index of the column head (in). normally divColumn -1 at count==0
        public int idxColumnTail; // index of the column tail (out). normally 0 at count ==0
        public int idxDiffuseHead; // index of diffusion front (head-side)
        public int idxDiffuseTail; // index of diffuxion front (tail-side)


        /* cells */
        double[] c;

        /*constructor*/
        public Column(Analyte givenAnalyte,
                        double givenInjectedConc,
                        double givenLenColumn,
                        int givenDivColumn,
                        double givenVelElu,
                        double givenTTotal,
                        double givenDt)
        {
            /* primary parameters set */
            analyte = givenAnalyte;
            injectedConc = givenInjectedConc;
            lenColumn = givenLenColumn;
            divColumn = givenDivColumn;
            velElu = givenVelElu;
            tTotal = givenTTotal;
            dt = givenDt;

            /* secondary paramers calculated and set*/
            dL = lenColumn / divColumn;
            translCount = (int)(dL / (velElu * analyte.rTransl * dt));
            //            numTransl = (int)(tTotal * velElu * analyte.rTransl / dL);
            numTransl = (int)(tTotal / (translCount * dt)) + 1;
            /* 
             Calculate numTransl by tTotal/(translCount*dt)+1.
             If this is calulated by using primary parameters (velElu, rTransl, dt) and dL,
             rounding error builds up, numTransl is evaluated small, and results in over-run of c[].
             the last "1" means translation at count = 0.
             */
            numCells = numTransl + divColumn + 1;

            /* cells allocated and initialized */
            c = new double[numCells];
            idxColumnHead = divColumn - 1;
            idxColumnTail = 0;

            c[idxColumnHead + 1] = injectedConc;
        }

        /* Translates 1 cell when this function is called: dt*translCount */
        public void translate()
        {
            idxColumnTail++;
            idxColumnHead++;

            if (idxDiffuseTail < idxColumnTail)
                idxDiffuseTail = idxColumnTail;
        }

        /* this function should be called in every dt (every clock) */
        public void diffuse()
        {
            int idxCalcHead, idxCalcTail; // region in which diffusion should take place in this period of time
            int idxFluxHead;
            int offset;
            double differential;
            double[] flux;

            // firstly determine the region to calculate. Don't over-run!
            if (idxDiffuseHead < idxColumnHead)
                idxCalcHead = idxDiffuseHead + 1;
            else
                idxCalcHead = idxColumnHead;

            if (idxColumnTail < idxDiffuseTail)
                idxCalcTail = idxDiffuseTail - 1;
            else
                idxCalcTail = idxColumnTail;

            // flux[idxFluxHead - i] holds flux from cell c[idxCalcHead - i] 
            idxFluxHead = idxCalcHead - idxCalcTail;
            flux = new double[idxFluxHead + 1];

            /* calculate the flux */
            for (offset = 0; offset <= idxCalcHead - idxCalcTail - 1; offset++)
            {
                differential = (c[idxCalcHead - (offset + 1)] - c[idxCalcHead - offset]) / dL;
                flux[idxFluxHead - offset] = -1.0 * differential * analyte.diffuseCoeff;
            }

            /*  calcate from the head */
            offset = 0;
            c[idxCalcHead - offset] -= flux[idxFluxHead - offset] * dt;
            offset++;

            /* Now offset == 1, middle region */
            for (; offset <= idxCalcHead - idxCalcTail - 1; offset++)
            {
                c[idxCalcHead - offset] += flux[idxFluxHead - (offset - 1)] * dt;
                c[idxCalcHead - offset] -= flux[idxFluxHead - offset] * dt;
            }

            /* Now offset == idxCalcHead - idxCalcTail: tail */
            c[idxCalcHead - offset] += flux[idxFluxHead - (offset - 1)] * dt;

            idxDiffuseHead = idxCalcHead;
            idxDiffuseTail = idxCalcTail;
        }
        
        /* this method should be called in every clock (dt)*/
        /* "this" decomposes into "product" */
        public void react(Column product)
        {
            int offset;

            double dc; // change in the concentration

            for (offset = idxColumnHead - idxDiffuseHead; idxDiffuseTail <= idxColumnHead - offset; offset++)
            {
                dc = c[idxColumnHead - offset] * (1.0 - Math.Exp(-1.0 * analyte.k * dt));
                c[idxColumnHead - offset] -= dc;
                product.c[idxColumnHead - offset] += dc;
            }

            /* reflesh the products's fiffusion region */
            if (product.idxColumnHead - product.idxDiffuseHead > idxColumnHead - idxDiffuseHead)
                product.idxDiffuseHead = product.idxColumnHead - (idxColumnHead - idxDiffuseHead);

            if (product.idxDiffuseTail - product.idxColumnTail > idxDiffuseTail - idxColumnTail)
                product.idxDiffuseTail = product.idxColumnTail + (idxDiffuseTail - idxColumnTail);
        }

        public void output(string filename)
        {
            int idxChlomato;

            System.IO.StreamWriter strWrt = new System.IO.StreamWriter(filename);

            for(idxChlomato = 0; idxChlomato <= idxColumnTail - 1; idxChlomato++){
                strWrt.WriteLine(idxChlomato * translCount * dt + "," + c[idxChlomato]);
            }

            strWrt.Close();
        }
    }
}
