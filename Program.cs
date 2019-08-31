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

            double injectedConc = 1.0;
            double lenColumn = 10 / 100; //m 
            int divColumn = 1000;
            double vElu = 10e-2/(1*60); // m/s
            double tTotal = 15.0 * 60; // seconds
                int divTime = 10000;
            double dt = tTotal/divTime; // seconds

            Analyte analyte;
            Column column;
            
            analyte = new Analyte(10.0, 0, 4.0e-8);

            column = new Column(analyte, injectedConc, lenColumn, divColumn, vElu, tTotal, dt);

            for(clock = 0; clock <= divTime; clock++){
                column.diffuse();
                if(clock % column.translCount == 0){
                    column.translate();
                }
            }

            column.output("outputtest.csv");

        }
    }
}
