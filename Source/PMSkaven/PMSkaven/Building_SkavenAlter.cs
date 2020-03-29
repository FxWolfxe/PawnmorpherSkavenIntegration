// Building_SkavenAlter.cs created by Iron Wolf for PMSkaven on 03/26/2020 3:36 PM
// last updated 03/26/2020  3:36 PM

using System.Collections.Generic;
using RimWorld;
using Verse;

namespace PMSkaven
{
    public class Building_SkavenAlter : Building
    {
        private IntVec3[] _peripheralCells; 

        public IReadOnlyList<IntVec3> PeripheralCells
        {
            get
            {
                if (_peripheralCells == null)
                {
                    _peripheralCells = new IntVec3[12];

                    //interact cell is the top center cell of the table 
                    var iCel = InteractionCell;


                    //top cells 

                    for (int i = 0; i < 3; i++)
                    {
                        int x = i - 1;
                        _peripheralCells[i] = iCel + new IntVec3(x, 0, 1);
                    }
                    
                    //right side 
                    for (int i = 0; i < 3; i++)
                    {
                        var index = i + 3;
                        var y = 0 - i;
                        _peripheralCells[index] = iCel + new IntVec3(2, 0, y); 
                    }

                    //bottom 
                    for (int i = 0; i < 3; i++)
                    {
                        var index = i + 6;
                        var x = i - 1;
                        _peripheralCells[index] = iCel + new IntVec3(x, 0, -3); 
                    }

                    //left
                    for (int i = 0; i < 3; i++)
                    {
                        var index = i + 9;
                        var y = 0 - i;
                        _peripheralCells[index] = iCel + new IntVec3(-2, 0, y); 
                    }
                }

                return _peripheralCells; 
            }
        }


    }
}