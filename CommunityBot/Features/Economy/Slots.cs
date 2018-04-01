using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityBot.Features.Economy
{
    public class Slot
    {
        static Random rnd = Global.Rng;
        // This is really something that shouldn't be hardcoded :D but oh well... works for now :P
        public static readonly Dictionary<string, SlotPiece> SlotPieces = new Dictionary<string, SlotPiece>
        {
            ["win"]     = new SlotPiece(":100:",        1, 1, 50),
            ["bag"]     = new SlotPiece(":moneybag:",   1, 2, 10),
            ["candy"]   = new SlotPiece(":candy:",      1, 2, 5),
            ["straw"]   = new SlotPiece(":strawberry:", 2, 2, 3),
            ["pine"]    = new SlotPiece(":pineapple: ", 3, 2, 2),
            ["grape"]   = new SlotPiece(":grapes:",     3, 2, 1),
            ["cherry"]  = new SlotPiece(":cherries:",   3, 2, 0.5),
            ["start"]   = new SlotPiece(":star:",       1, 2, -1),
            ["zap"]     = new SlotPiece(":zap:",        1, 2, -2),
        };
        public List<Cylinder> Cylinders = new List<Cylinder>();
        static int maxRandom;

        // CylinderSize is adjustable but will not be smaller than the sum of minSpawnrates of all possible SlotPieces
        public Slot(int cylinderSize = 0)
        {
            maxRandom = 0;
            foreach (var piece in SlotPieces)
            {
                maxRandom += piece.Value.spawnrate;
            }
            Cylinders.Add(new Cylinder(cylinderSize));
            Cylinders.Add(new Cylinder(cylinderSize));
            Cylinders.Add(new Cylinder(cylinderSize));
        }

        public class Cylinder {
            public List<SlotPiece> CylinderSlotPieces = new List<SlotPiece>();
            public int Pointer = 0;
            public Cylinder(int size)
            {
                // Add all the pieces according to their minSpawnCount
                foreach (var piece in SlotPieces)
                {
                    for (int i = piece.Value.minSpawnCount - 1; i >= 0; i--)
                    {
                        CylinderSlotPieces.Add(piece.Value);
                        size--;
                    }
                }
                // If more pieces are requested fill them according to spawnrate
                for (int i = size - 1; i >= 0; i--)
                {
                    int rand = rnd.Next(maxRandom);
                    foreach (var piece in Slot.SlotPieces)
                    {
                        rand -= piece.Value.spawnrate;
                        if (rand <= 0)
                        {
                            CylinderSlotPieces.Add(piece.Value);
                            break;
                        }
                    }
                }
                // Shuffle the pieces
                CylinderSlotPieces = CylinderSlotPieces.OrderBy((item) => rnd.Next()).ToList<SlotPiece>();
            }
        }

        public class SlotPiece {
            public string emoji;
            public int minSpawnCount;
            public int spawnrate;
            public double payout;

            public SlotPiece(string emoji, int minSpawnCount, int spawnrate, double payout)
            {
                this.emoji = emoji;
                this.minSpawnCount = minSpawnCount;
                this.spawnrate = spawnrate;
                this.payout = payout;
            }
        };

        public uint GetPayout(uint amount)
        {
            double sumPayout = 0;

            /*
             * Emoji coordinates (row, column):
             * -1, 0 | -1, 1 | -1, 2
             *  0, 0 |  0, 1 |  0, 2
             *  1, 0 |  1, 1 |  1, 2
             */

            for (int i = 0; i < 3; i++)
            {
                // Check columns
                sumPayout += CheckPayoutForCoordinates(-1, i, 0, i, 1, i);
                // Check rows
                sumPayout += CheckPayoutForCoordinates(-1 + i, 0, -1 + i, 1, -1 +i, 2);
            }
            // Diagonal top left to bottom right
            sumPayout += CheckPayoutForCoordinates(-1, 0, 0, 1, 1, 2);
            // Diagonal bottom left to top right
            sumPayout += CheckPayoutForCoordinates(1, 0, 0, 1, -1, 2);
            
            return (uint) (amount * sumPayout);
        }

        // Check if the set of three coordinates are the same emoji - if so get the payout ratio of that emoji
        public double CheckPayoutForCoordinates(int rowI, int columnI, int rowJ, int columnJ, int rowK, int columnK)
        {
            int count = Cylinders[0].CylinderSlotPieces.Count;
            var first = Cylinders[columnI].CylinderSlotPieces[(((Cylinders[columnI].Pointer + rowI) % count) + count) % count];
            var second = Cylinders[columnJ].CylinderSlotPieces[(((Cylinders[columnJ].Pointer + rowJ) % count) + count) % count];
            var third = Cylinders[columnK].CylinderSlotPieces[(((Cylinders[columnK].Pointer + rowK) % count) + count) % count];
            if (first.emoji == second.emoji && second.emoji == third.emoji)
                return first.payout;
            return 0;
        }

        // Returns the emoji string for the current cylinder pointers
        public string GetEmojis()
        {
            string response = "";
            int count = Cylinders[0].CylinderSlotPieces.Count;
            for (int j = -1; j < 2; j++)
            {
                for (int i = 0; i < 3; i++)
                {
                    response += Cylinders[i].CylinderSlotPieces[(((Cylinders[i].Pointer + j) % count) + count) % count].emoji;
                }
                response += "\n";
            }
            return response;
        }

        public string Spin()
        {
            int count = Cylinders[0].CylinderSlotPieces.Count;
            Cylinders[0].Pointer = rnd.Next();
            Cylinders[1].Pointer = rnd.Next();
            Cylinders[2].Pointer = rnd.Next();
            return GetEmojis();
        }
    }
}
