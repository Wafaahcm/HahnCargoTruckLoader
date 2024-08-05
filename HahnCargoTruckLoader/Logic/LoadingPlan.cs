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
            int truckVolume = _truck.Width * _truck.Height * _truck.Length;

            if (totalCrateVolume > truckVolume)
            {
                throw new Exception($"Cannot load crates: total volume {totalCrateVolume} exceeds truck capacity {truckVolume}.");
            }
            //Initialisation de l'Espace de Chargement et Tri des Caisses

            var cargoSpace = new bool[_truck.Width, _truck.Height, _truck.Length];
            var sortedCrates = _crates.OrderByDescending(c => c.Width * c.Height * c.Length).ToList();


            //Placement des caisses :

            int stepNumber = 1;

            foreach (var crate in sortedCrates)
            {
                bool placed = false;
                var orientations = new List<(int w, int h, int l)>
                {
                    (crate.Width, crate.Height, crate.Length),
                    (crate.Length, crate.Height, crate.Width),
                    (crate.Width, crate.Length, crate.Height)

                };

                foreach (var (width, height, length) in orientations)
                {
                    if (placed) break;

                    placed = TryPlaceCrateAtOrientation(crate, width, height, length, cargoSpace, ref stepNumber);
                }

                //Gestion des erreurs de placement :

                if (!placed)
                {
                    throw new Exception($"Unable to place crate with ID {crate.CrateID}.");
                }

            }

            return _instructions;


        }

        // Méthode pour essayer de placer une caisse dans une orientation donnée
        private bool TryPlaceCrateAtOrientation(Crate crate, int width, int height, int length, bool[,,] cargoSpace, ref int stepNumber)
        {
            // Parcourir toutes les positions possibles pour placer la caisse
            for (int x = 0; x <= _truck.Width - width; x++)
            {
                for (int y = 0; y <= _truck.Height - height; y++)
                {
                    for (int z = 0; z <= _truck.Length - length; z++)
                    {
                        // Vérifier si la caisse peut tenir à la position donnée
                        if (CanFit(width, height, length, x, y, z, cargoSpace))
                        {
                            // Placer la caisse dans l'espace de chargement
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

                            // Ajouter l'instruction de chargement pour la caisse
                            var instruction = new LoadingInstruction
                            {
                                LoadingStepNumber = stepNumber++,
                                CrateId = crate.CrateID,
                                TopLeftX = x,
                                TopLeftY = y,
                                TurnHorizontal = (width == crate.Length && height == crate.Height && length == crate.Width),
                                TurnVertical = (width == crate.Width && height == crate.Length && length == crate.Height)
                            };

                            // Ajouter l'instruction au dictionnaire
                            _instructions[crate.CrateID] = instruction;

                            return true; // La caisse a été placée avec succès
                        }
                    }
                }
            }

            return false; // La caisse n'a pas pu être placée
        }

        


    }
}

