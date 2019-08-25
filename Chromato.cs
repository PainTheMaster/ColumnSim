namespace ColumSim
{
    public class Chromato
    {
        /* Primary parameters */
        public Analyte analyte;
        public double injectedConc;
        public double lenColumn;
        public int divColumn;
        public double velElu;
        public double tTotal;
        public double dt;

        /* Secondary parameters */
        public double dL;
        public int translCount;
        public int numTransl;
        public int numCells;

        /* Working variables */
        public int idxHead;
        public int idxTail;
        public int idxDiffuseHead;
        public int idxDiffuseTail;


        /* cells */
        double[] c;

        public Chromato(Analyte givenAnalyte,
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
            numTransl = (int)(tTotal * velElu * analyte.rTransl / dL);
            numCells = numTransl + divColumn + 1;

            /* cells allocated and initialized */
            c = new double[numCells];
            idxHead = divColumn - 1;
            idxTail = 0;

            c[idxHead + 1] = injectedConc;
        }

        public void translate()
        {
            idxTail++;
            idxHead++;

            if (idxDiffuseTail < idxTail)
                idxDiffuseTail = idxTail;
        }

        public void diffuse()
        {
            int idxCalcHead, idxCalcTail;
            int idxFluxHead;
            int offset;
            double differential;
            double[] flux;


            if (idxDiffuseHead < idxHead)
                idxCalcHead = idxDiffuseHead + 1;
            else
                idxCalcHead = idxHead;

            if (idxTail < idxDiffuseTail)
                idxCalcTail = idxDiffuseTail - 1;
            else
                idxCalcTail = idxTail;

            idxFluxHead = idxCalcHead - idxCalcTail;
            flux = new double[idxFluxHead + 1];

            for (offset = 0; offset <= idxCalcHead - idxCalcTail - 1; offset++)
            {
                differential = (c[idxCalcHead - (offset + 1)] - c[idxCalcHead - offset]) / dL;
                flux[idxFluxHead - offset] = -1 * differential * analyte.diffuseCoeff;
            }


            offset = 0;
            c[idxCalcHead - offset] -= flux[idxFluxHead - offset] * dt;
            offset++;
            /* Now offset == 1 */

            for (; offset <= idxCalcHead - idxCalcTail - 1; offset++)
            {
                c[idxCalcHead - offset] += flux[idxFluxHead - (offset - 1)] * dt;
                c[idxCalcHead - offset] -= flux[idxFluxHead - offset] * dt;
            }
            /* Now offset == idxCalcHead - idxCalcTail */

            c[idxCalcHead - offset] += flux[idxFluxHead - (offset - 1)] * dt;

            idxDiffuseHead = idxCalcHead;
            idxDiffuseTail = idxCalcTail;
        }

        public void react(Chromato product)
        {
            int offset;

            double dc;

            for (offset = idxHead - idxDiffuseHead; idxDiffuseTail <= idxHead - offset; offset++)
            {
                dc = analyte.k * c[idxHead - offset] * dt;
                c[idxHead - offset] -= dc;
                product.c[idxHead - offset] += dc;
            }

            if (product.idxHead - product.idxDiffuseHead > idxHead - idxDiffuseHead)
                product.idxDiffuseHead = product.idxHead - (idxHead - idxDiffuseHead);

            if (product.idxDiffuseTail - product.idxTail > idxDiffuseTail - idxTail)
                product.idxDiffuseTail = product.idxTail + (idxDiffuseTail - idxTail);
        }
    }
}
