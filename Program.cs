using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColumSim;

namespace ColumSim
{
    class Program
    {

        
        static void Main()
        {
            int clock;

            double injectedConc1 = 1.0; //M
            double lenColumn = 10.0 / 100.0; //m 
            int divColumn = 1000; // no dimension
            double vElu = 10e-2/(1*60); // m/s
            double tTotal = 15.0 * 60; // seconds
                int divTime = 900000;
            double dt = tTotal/divTime; // seconds

            double injectedConc2 = 0.02; //M


            Analyte analyte1, analyte2;
            Column column1, column2;
            
            analyte1 = new Analyte(/*rTransl*/ 0.1,/*k of decomp*/ 5.0e-4,/* D 4e-8 */ 4.0e-6);
            analyte2 = new Analyte(0.15, 0, 4.0e-6);

            column1 = new Column(analyte1, injectedConc1, lenColumn, divColumn, vElu, tTotal, dt);
            column2 = new Column(analyte2, injectedConc2, lenColumn, divColumn, vElu, tTotal, dt);


            for (clock = 0; clock <= divTime-1; clock++){
                column1.diffuse();
                column2.diffuse();
                column1.react(column2);

                if(clock % column1.translCount == 0){
                    column1.translate();
                }

                if (clock % column2.translCount == 0)
                {
                    column2.translate();
                }
            }

            column1.output("outputtest1.csv");
            column2.output("outputtest2.csv");
        }
    }
}
