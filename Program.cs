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

            double injectedConc = 1.0; //M
            double lenColumn = 10.0 / 100.0; //m 
            int divColumn = 1000; // no dimension
            double vElu = 10e-2/(1*60); // m/s
            double tTotal = 15.0 * 60; // seconds
                int divTime = 900000;
            double dt = tTotal/divTime; // seconds

            Analyte analyte;
            Column column;
            
            analyte = new Analyte(/*rTransl*/ 0.1,/*k of decomp*/ 0,/* D 4e-8 */ 4.0e-6);

            column = new Column(analyte, injectedConc, lenColumn, divColumn, vElu, tTotal, dt);

            for(clock = 0; clock <= divTime-1; clock++){
                column.diffuse();
                if(clock % column.translCount == 0){
                    column.translate();
                }
            }

            column.output("outputtest.csv");

        }
    }
}
