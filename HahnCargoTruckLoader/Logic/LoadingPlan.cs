using HahnCargoTruckLoader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HahnCargoTruckLoader.Logic
{
  public class LoadingPlan
  {
        private readonly Dictionary<int, LoadingInstruction> _instructions;
        private readonly Truck _truck;
        private readonly List<Crate> _crates; 

    public LoadingPlan(Truck truck, List<Crate> crates) 
    {
      
           _instructions = new Dictionary<int, LoadingInstruction>();
           _truck = truck;
           _crates = crates;
    }

    public Dictionary<int, LoadingInstruction> GetLoadingInstructions() 
    {

         int totalCrateVolume = _crates.Sum(c => c.Width * c.Height * c.Length);
         int truckVolume = _truck.Width *  _truck.Height *  _truck.Length;
 
         if (totalCrateVolume > truckVolume)
            {
                throw new Exception($"Cannot load crates: total volume {totalCrateVolume} exceeds truck capacity {truckVolume}.");
            }
            return _instructions;
    }

  }
}
