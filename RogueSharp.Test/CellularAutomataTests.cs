using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace RogueSharp.Test
{
    [TestClass]
   public class CellularAutomataTests
   {


      [TestMethod]
      public void BasicNeighborTest()
      {
         Map map = new Map( 5, 5 );
         map.Clear( true, true );
         map.SetCellProperties(1,2, false, false);
         map.SetCellProperties(2,2, false, false);
         map.SetCellProperties(3,2, false, false);
         var neighbors = new List<int>();
         foreach(var cell in map.GetAllCells())
         {
            neighbors.Add(CellularAutomata.CountWallsNear(map, cell, 1));
         }
         var expected = new List<int>(){0,0,0,0,0,
                                       1,2,3,2,1,
                                       1,1,2,1,1,
                                       1,2,3,2,1,
                                       0,0,0,0,0};
         var actualString = String.Join(",", neighbors);
         var expectedString = String.Join(",", expected);
         Trace.Write( $"{expectedString}\n{actualString}" );
         Assert.AreEqual( expectedString, actualString);
      }

      [TestMethod]
      public void BasicConwayTest()
      {
         Map map = new Map( 5, 5 );
         map.Clear( true, true );
         map.SetCellProperties(1,2, false, false);
         map.SetCellProperties(2,2, false, false);
         map.SetCellProperties(3,2, false, false);
         var born = new HashSet<int>(){3};
         var survive = new HashSet<int>(){2,3};

         Trace.WriteLine( map );
         Trace.WriteLine("");

         var actualMap = CellularAutomata.CellularAutomataRunGeneration(map, born, survive);
         var expectedMapRepresentation = @".....
                          ..#..
                          ..#..
                          ..#..
                          .....";
         Trace.Write( actualMap );
         Assert.AreEqual( MapTest.RemoveWhiteSpace( expectedMapRepresentation ), MapTest.RemoveWhiteSpace( actualMap.ToString() ) );
      }
   }
}
