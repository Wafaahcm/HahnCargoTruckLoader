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
            // Calculate total volume of all crates and the truck's cargo space volume

            int totalCrateVolume = _crates.Sum(c => c.Width * c.Height * c.Length);
            int truckVolume = _truck.Width * _truck.Height * _truck.Length;

            // Check if the total volume of crates exceeds the truck's volume

            if (totalCrateVolume > truckVolume)
            {
                throw new Exception($"Cannot load crates: total volume {totalCrateVolume} exceeds truck capacity {truckVolume}.");
            }

            // Create a 3D array to represent the truck's cargo space

            var cargoSpace = new bool[_truck.Width, _truck.Height, _truck.Length];

            // Sort crates by volume in descending order to try to place larger crates first

            var sortedCrates = _crates.OrderByDescending(c => c.Width * c.Height * c.Length).ToList();


            

            int stepNumber = 1;

            foreach (var crate in sortedCrates)   // Iterate over each crate to place it in the truck
            {
                bool placed = false;
                var orientations = new List<(int w, int h, int l)>   // List of possible orientations for the crate
                {
                    (crate.Width, crate.Height, crate.Length),
                    (crate.Length, crate.Height, crate.Width),
                    (crate.Width, crate.Length, crate.Height)

                };

                foreach (var (width, height, length) in orientations)  // Try placing the crate in each orientation
                {
                    if (placed) break;

                    placed = TryPlaceCrateAtOrientation(crate, width, height, length, cargoSpace, ref stepNumber);
                }

                

                if (!placed)
                {
                    throw new Exception($"Unable to place crate with ID {crate.CrateID}.");
                }

            }

            return _instructions;


        }

        // Method to attempt placing a crate in a given orientation
        private bool TryPlaceCrateAtOrientation(Crate crate, int width, int height, int length, bool[,,] cargoSpace, ref int stepNumber)
        {
            // Iterate through all possible positions to place the crate
            for (int x = 0; x <= _truck.Width - width; x++)
            {
                for (int y = 0; y <= _truck.Height - height; y++)
                {
                    for (int z = 0; z <= _truck.Length - length; z++)
                    {
                        // Check if the crate can fit in the given position
                        if (CanFit(width, height, length, x, y, z, cargoSpace))
                        {
                            // Place the crate in the cargo space
                            for (int dx = 0; dx < width; dx++)
                            {
                                for (int dy = 0; dy < height; dy++)
                                {
                                    for (int dz = 0; dz < length; dz++)
                                    {
                                        cargoSpace[x + dx, y + dy, z + dz] = true;
                                    }
                                }
                            }

                            // Add the loading instruction for the crate
                            var instruction = new LoadingInstruction
                            {
                                LoadingStepNumber = stepNumber++,
                                CrateId = crate.CrateID,
                                TopLeftX = x,
                                TopLeftY = y,
                                TurnHorizontal = (width == crate.Length && height == crate.Height && length == crate.Width),
                                TurnVertical = (width == crate.Width && height == crate.Length && length == crate.Height)
                            };

                            // Add the instruction to the dictionary
                            _instructions[crate.CrateID] = instruction;

                            return true; // The crate was successfully placed
                        }
                    }
                }
            }

            return false; // The crate could not be placed
        }

        // Method to check if a crate can fit in a given position
        private bool CanFit(int width, int height, int length, int x, int y, int z, bool[,,] cargoSpace)
        {
            // Check if the space is already occupied
            for (int dx = 0; dx < width; dx++)
            {
                for (int dy = 0; dy < height; dy++)
                {
                    for (int dz = 0; dz < length; dz++)
                    {
                        if (cargoSpace[x + dx, y + dy, z + dz])
                        {
                            return false; // The space is already occupied
                        }
                    }
                }
            }

            return true;  // The space is available
        }




    }
}

