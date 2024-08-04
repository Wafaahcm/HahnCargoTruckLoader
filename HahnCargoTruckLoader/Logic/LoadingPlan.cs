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
    private readonly Dictionary<int, LoadingInstruction> instructions;
    public LoadingPlan(Truck truck, List<Crate> crates) {
      instructions = [];
    }

    public Dictionary<int, LoadingInstruction> GetLoadingInstructions() {

      //ToDo

      return instructions;
    }

  }
}
