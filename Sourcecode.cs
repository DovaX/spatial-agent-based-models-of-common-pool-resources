using System;   
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThesisApplication
{
    public static class G //Global variables
    {
        public static int maxx1, maxy1, minx1, miny1, maxx2, maxy2, minx2, miny2; //highest and lowest coordinates of player 1 and 2, respectively
        public static int res = 100; // resolution of the 2D simulation is (res+1)x(res+1) 
        public static double e = 0.02, s = 0.2; //parameter e and s
        public static double step = 1.42; //marginal step is lower than this value i.e. either a diagonal step Math.sqrt(2) or step of length 1.
        public static double radius, sradius; //radius=e*res,sradius=s*res
        public static int intradius, intsradius; //integer parts of the variables above
       
    }
    class Subroutine2D
    {
        /// <summary>
        /// Linear distribution adjusting two players in two stages (not used in the thesis)
        /// </summary>
        public void Setup1(int[] xint, int[] yint, double[,]Matrix,double[,]Payoff,int[]optimalcoordinates,double[,]sMatrix,double[,]Payoff2, System.IO.StreamWriter file) {
            int player;
            Initialize(xint, yint);
            Generatelineardistribution(Matrix);
            PlayerPayoff(Payoff, Matrix);
            FindMax(Payoff, optimalcoordinates);
            player = 0;
            Adjust(player, xint, yint, optimalcoordinates);
            Initialize(xint, yint);
            Generateplayer1(xint, yint, Matrix, sMatrix,0);
            PlayerPayoff(Payoff2, Matrix);
            FindMax(Payoff2, optimalcoordinates);
            player = 1;
            Adjust(player, xint, yint, optimalcoordinates);
            Initialize(xint, yint);
            Generateplayer2(xint, yint, Matrix, sMatrix,1);
            Summary(0, xint, yint, Payoff, Payoff2,file);
        }
        /// <summary>
        /// Linear distribution with player1 prediction and player2 prediction with respect to location of player 1 (not used in the thesis)
        /// </summary>
        public void Setup2(int[]xint,int[]yint,double[,]Matrix,double[,]Payoff,int[]optimalcoordinates,double[,]sMatrix,double[,]Payoff2, System.IO.StreamWriter file)
        {
            int player;
            Initialize(xint, yint);
            Generatelineardistribution(Matrix);
            player = 0;
            PredictsMatrix(player, xint, yint, Matrix, sMatrix,G.sradius);
            PlayerPayoff(Payoff, sMatrix);
            FindMax(Payoff, optimalcoordinates);
            Adjust(player, xint, yint, optimalcoordinates);
            Initialize(xint, yint);
            player = 1;
            PredictsMatrix(player, xint, yint, Matrix, sMatrix,G.sradius);
            Generateplayer1(xint, yint, Matrix, sMatrix,0);
            PlayerPayoff(Payoff2, sMatrix);
            FindMax(Payoff2, optimalcoordinates);
            Adjust(player, xint, yint, optimalcoordinates);
            Initialize(xint, yint);
            Generateplayer2(xint, yint, Matrix, sMatrix,1);
            Summary(1, xint, yint, Payoff, Payoff2,file);
        }
        /// <summary>
        /// The last simulation - 2D, 2 players alternate and predict until find their equilibrium (chapter 4.5)
        /// </summary>
        /// <param name="xint"></param>
        /// <param name="yint"></param>
        /// <param name="Matrix"></param>
        /// <param name="Payoff"></param>
        /// <param name="optimalcoordinates"></param>
        /// <param name="sMatrix"></param>
        /// <param name="Payoff2"></param>
        /// <param name="Payoffandsonar"></param>
        /// <param name="file"></param>
        public void Setup3(int[] xint, int[] yint, double[,] Matrix, double[,] Payoff, int[] optimalcoordinates, double[,] sMatrix, double[,] Payoff2, double[,] Payoffandsonar, System.IO.StreamWriter file)
        {
            int player;
            Initialize(xint, yint);
            Generatelineardistribution(Matrix);
            player = 0;
            PredictsMatrix(player, xint, yint, Matrix, sMatrix, G.sradius);
            PlayerPayoff(Payoff, sMatrix);
            FindMax(Payoff, optimalcoordinates);
            AdjustStep(player, xint, yint, optimalcoordinates, G.step);
            Initialize(xint, yint);
            player = 1;
            PredictsMatrix(player, xint, yint, Matrix, sMatrix, G.radius);
            Generateplayer1(xint, yint, Matrix, sMatrix, 0);
            PlayerPayoff(Payoff2, sMatrix);
            FindMax(Payoff2, optimalcoordinates);
            AdjustStep(player, xint, yint, optimalcoordinates, G.step);
            Initialize(xint, yint);
            Generateplayer2(xint, yint, Matrix, sMatrix, 1);
            //Summary(1, xint, yint, Payoff, Payoff2);
            int[] xintmemory = new int[2];
            int[] yintmemory = new int[2];
            /////////////////////REPEAT//////////////////
            int doba = 0;
            while ((xintmemory[0] != xint[0]) || (xintmemory[1] != xint[1]) || (yintmemory[0] != yint[0]) || (yintmemory[1] != yint[1]))
            {

                xintmemory = (int[])xint.Clone();
                yintmemory = (int[])yint.Clone();
                Deleteplayer2(xint, yint, Matrix, sMatrix, 1);
                Deleteplayer1(xint, yint, Matrix, sMatrix, 0);
                player = 0;
                PredictsMatrix(player, xint, yint, Matrix, sMatrix, G.sradius);
                Generateplayer2(xint, yint, Matrix, sMatrix, 0);               
                PlayerPayoff(Payoff, sMatrix);
                FindMax(Payoff, optimalcoordinates);
                AdjustStep(player, xint, yint, optimalcoordinates, G.step);
                Initialize(xint, yint);
                Deleteplayer2(xint, yint, Matrix, sMatrix, 0);
                player = 1;
                PredictsMatrix(player, xint, yint, Matrix, sMatrix, G.radius);
                Generateplayer1(xint, yint, Matrix, sMatrix, 0);
                PlayerPayoff(Payoff2, sMatrix);
                FindMax(Payoff2, optimalcoordinates);
                AdjustStep(player, xint, yint, optimalcoordinates, G.step);
                Initialize(xint, yint);
                Generateplayer2(xint, yint, Matrix, sMatrix, 1);
                doba++; //writes down the summary if the Nash equilibrium cannot be found and after 200 rounds leaves the iteration.
                if (doba >= 100 && doba <= 200)
                {
                    Summary(1, xint, yint, Payoff, Payoff2, file);
                }
                if (doba >= 200)
                {
                    break;
                }
            }
            Deleteplayer1(xint, yint, Matrix, sMatrix, 1);
            PlayerPayoff(Payoff, Matrix);
            int poradi = (int)Math.Floor(Payoffandsonar[0, 5]);
            Payoffandsonar[poradi, 0] = Payoff[xint[0], yint[0]];
            Console.WriteLine("Player 1, Payoff:" + Payoff[xint[0], yint[0]]);
            Generateplayer1(xint, yint, Matrix, sMatrix, 1);
            Deleteplayer2(xint, yint, Matrix, sMatrix, 1);
            PlayerPayoff(Payoff, Matrix);
            Payoffandsonar[poradi, 1] = Payoff[xint[1], yint[1]];
            Console.WriteLine("Player 2, Payoff:" + Payoff[xint[1], yint[1]]);
        }
        /// <summary>
        /// Setup with perfect information for two players and different parameter e with repeated game (chapter 4.5)
        /// </summary>
        /// <param name="xint"></param>
        /// <param name="yint"></param>
        /// <param name="Matrix"></param>
        /// <param name="Payoff"></param>
        /// <param name="optimalcoordinates"></param>
        /// <param name="sMatrix"></param>
        /// <param name="Payoff2"></param>
        /// <param name="Payoffandsonar"></param>
        /// <param name="file"></param>
        public void Setup2Doptimal2players(int[] xint, int[] yint, double[,] Matrix, double[,] Payoff, int[] optimalcoordinates, double[,] sMatrix, double[,] Payoff2, double[,] Payoffandsonar, System.IO.StreamWriter file)
        {
            int player;
            Initialize(xint, yint);
            Generatelineardistribution(Matrix);
            player = 0;
            
            PlayerPayoff(Payoff, Matrix);
            //Depict(Payoff);
            FindMax(Payoff, optimalcoordinates);
            Adjust(player, xint, yint, optimalcoordinates);
            Initialize(xint, yint);
            player = 1;
            Generateplayer1(xint, yint, Matrix, sMatrix, 0);
            PlayerPayoff(Payoff2, Matrix);
            //Depict(Payoff2);
            FindMax(Payoff2, optimalcoordinates);
            Adjust(player, xint, yint, optimalcoordinates);
            Initialize(xint, yint);
            Generateplayer2(xint, yint, Matrix, sMatrix, 1);
            //Depict(Matrix);
            ////Summary(1, xint, yint, Payoff, Payoff2);
            int[] xintmemory = new int[2];
            int[] yintmemory = new int[2];
            /////////////////////REPEAT//////////////////
            
            while ((xintmemory[0] != xint[0]) || (xintmemory[1] != xint[1]) || (yintmemory[0] != yint[0]) || (yintmemory[1] != yint[1]))
            {

                xintmemory = (int[])xint.Clone();
                yintmemory = (int[])yint.Clone();
                Deleteplayer2(xint, yint, Matrix, sMatrix, 1);
                Deleteplayer1(xint, yint, Matrix, sMatrix, 0);
                player = 0;                
                Generateplayer2(xint, yint, Matrix, sMatrix, 0);                
                PlayerPayoff(Payoff, Matrix);      
                FindMax(Payoff, optimalcoordinates);
                Adjust(player, xint, yint, optimalcoordinates);
                Initialize(xint, yint);
                Deleteplayer2(xint, yint, Matrix, sMatrix, 0);
                player = 1;
                Generateplayer1(xint, yint, Matrix, sMatrix, 0);
                //Depict(Matrix);
                PlayerPayoff(Payoff2, Matrix);
                //Depict(Payoff2);
                FindMax(Payoff2, optimalcoordinates);
                AdjustStep(player, xint, yint, optimalcoordinates, G.step);
                Initialize(xint, yint);
                Generateplayer2(xint, yint, Matrix, sMatrix, 1);
                //Depict(Matrix);

            }
            Deleteplayer1(xint, yint, Matrix, sMatrix, 1);
            PlayerPayoff(Payoff, Matrix);
            int poradi = (int)Math.Floor(Payoffandsonar[0, 5]);
            Payoffandsonar[poradi, 0] = Payoff[xint[0], yint[0]];
            Payoffandsonar[poradi, 2] = xint[0];
            Payoffandsonar[poradi, 3] = yint[0];
            Payoffandsonar[poradi, 4] = xint[1];
            Payoffandsonar[poradi, 5] = yint[1];

            Console.WriteLine("Player 1, Payoff:" + Payoff[xint[0], yint[0]]);
            Generateplayer1(xint, yint, Matrix, sMatrix, 1);
            Deleteplayer2(xint, yint, Matrix, sMatrix, 1);
            PlayerPayoff(Payoff, Matrix);
            Payoffandsonar[poradi, 1] = Payoff[xint[1], yint[1]];
            Console.WriteLine("Player 2, Payoff:" + Payoff[xint[1], yint[1]]);
            //Summary(0, xint, yint, Payoff, Payoff2);
        }
        /// <summary>
        /// Setup with perfect information for one player (not repeated) (
        /// </summary>
        public void Setup1optimalplayer(int[] xint, int[] yint, double[,] Matrix, double[,] Payoff, int[] optimalcoordinates, double[,] sMatrix, double[,] Payoff2, System.IO.StreamWriter file)
        {
            int player;
            Initialize(xint, yint);
            Generatelineardistribution(Matrix);
            PlayerPayoff(Payoff, Matrix);
            file.Write(Payoff[xint[0], yint[0]] + "\t");
            FindMax(Payoff, optimalcoordinates);
            Adjust(0, xint, yint, optimalcoordinates);
            Initialize(xint, yint);
            PlayerPayoff(Payoff, Matrix);
            Generateplayer1(xint, yint, Matrix, sMatrix, 0);
            file.WriteLine(Payoff[xint[0], yint[0]] + "\t" + xint[0] + "\t" + yint[0] + "\t" + G.e + "\t" + G.s);
        }
        /// <summary>
        /// First player's location is random, then he adjusts his position (chapter 4.4)
        /// </summary>
        public void Setup1stage1player(int[] xint, int[] yint, double[,] Matrix, double[,] Payoff, int[] optimalcoordinates, double[,] sMatrix, double[,] Payoff2,System.IO.StreamWriter file)
        {
            int player;
            Initialize(xint, yint);
            Generatelineardistribution(Matrix);
            player = 0;
            PredictsMatrix(player, xint,yint, Matrix, sMatrix,G.sradius);
            Depict(sMatrix);
            PlayerPayoff(Payoff, Matrix);
            Depict(Payoff);
            file.Write(Payoff[xint[0], yint[0]]+"\t");

            PlayerPayoff(Payoff, sMatrix);
            Depict(Payoff);
            FindMax(Payoff, optimalcoordinates);
            Adjust(player,xint,yint,optimalcoordinates);
            Initialize(xint,yint);
            PlayerPayoff(Payoff, Matrix);
            
            Generateplayer1(xint,yint, Matrix, sMatrix, 0);
            file.WriteLine(Payoff[xint[0], yint[0]] + "\t" + xint[0] + "\t" + yint[0] + "\t"+G.e+"\t"+G.s+" yint,e,s");
        }
        /// <summary>
        /// Both players' location is random, then the second player adjusts his position (chapter 4.5)
        /// </summary>
        /// <param name="xint"></param>
        /// <param name="yint"></param>
        /// <param name="Matrix"></param>
        /// <param name="Payoff"></param>
        /// <param name="optimalcoordinates"></param>
        /// <param name="sMatrix"></param>
        /// <param name="Payoff2"></param>
        /// <param name="file"></param>
        public void Setup2stages2players(int[] xint, int[] yint, double[,] Matrix, double[,] Payoff, int[] optimalcoordinates, double[,] sMatrix, double[,] Payoff2, System.IO.StreamWriter file) {
            int player;
            file.Write("\t 2STAGE");
            Initialize(xint,yint);
            Generatelineardistribution(Matrix);
            //player = 0;
            //sub1D.PredictsMatrix(player, xint, Matrix, sMatrix);
            //sub1D.PlayerPayoff(PayoffMatrix, Matrix);
            //sub1D.FindMax(PayoffMatrix, optimalcoordinates);
            //sub1D.Adjust(player, xint,     optimalcoordinates);
            //sub1D.Initialize(xint);
            player = 1;
            PredictsMatrix(player, xint,yint, Matrix, sMatrix,G.sradius);
            Generateplayer1(xint,yint, Matrix, sMatrix, 0);
            PlayerPayoff(Payoff2, sMatrix);
            //sub1D.Depict(sMatrix);
            FindMax(Payoff2, optimalcoordinates);
            Adjust(player, xint,yint, optimalcoordinates);
            Initialize(xint,yint);
            Generateplayer2(xint,yint, Matrix, sMatrix,1);
            //sub1D.Depict(Matrix);
            //Summary(1, xint,yint, Payoff, Payoff2, file);
            Generatelineardistribution(Matrix);
            Generateplayer1(xint,yint, Matrix, sMatrix, 0);
            PlayerPayoff(Payoff, Matrix);
            file.Write("\t xint[1]\t" + xint[1] + "\t" + yint[1]);
            file.Write("\t Player 2: \t" + Payoff[xint[1],yint[1]] + "\t");
            Generateplayer2(xint,yint, Matrix, sMatrix,0);
            Deleteplayer1(xint,yint, Matrix, sMatrix, 1);
            PlayerPayoff(Payoff, Matrix);
            file.WriteLine("\t Player 1: \t" + Payoff[xint[0],xint[0]] + "\t e \t" + G.e + "\t s \t" + G.s);
            //xintmemory[0] = 0;
            //xintmemory[1] = 0;

        }
        /// <summary>
        /// Generates uniform density function
        /// </summary>
        public void Generateuniformdistribution(double[,] Matrix)
        {
            for (int i = 0; i <= G.res; i++)
            {
                for (int j = 0; j <= G.res; j++)
                {
                    Matrix[i, j] = 1;
                }
            }
        }
        /// <summary>
        /// Generates linear density function
        /// </summary>
        /// <param name="Matrix"></param>
        public void Generatelineardistribution(double[,] Matrix)
        {
            for (int i = 0; i <= G.res; i++)
            {
                for (int j = 0; j <= G.res; j++)
                {
                    Matrix[i, j] = (double)(i + j) / G.res;
                }
            }
        }
        /// <summary>
        /// Calculates payoff in specific points
        /// </summary>
        /// <param name="inputi"></param>
        /// <param name="inputj"></param>
        /// <param name="Payoff"></param>
        /// <param name="Matrix"></param>
        public void PointPayoff(int inputi,int inputj,double[,]Payoff,double[,]Matrix) {
            Payoff[inputi, inputj] = 0;
            for (int i = Math.Max(0, inputi - G.intradius); i <= Math.Min(G.res, inputi + G.intradius); i++) {
                for (int j = Math.Max(0, inputj - G.intradius); j <= Math.Min(G.res, inputj + G.intradius); j++)
                {
                    if (Math.Sqrt(Math.Pow((double)(i - inputi),2) + Math.Pow((double)((j - inputj)),2))<=G.radius) {
                        Payoff[inputi, inputj] = Payoff[inputi, inputj] + Matrix[i, j];
                        
                    }
                }
            }
        }
        /// <summary>
        /// Calculates payoff everywhere
        /// </summary>
        /// <param name="Payoff"></param>
        /// <param name="Matrix"></param>
        public void PlayerPayoff(double[,]Payoff,double[,]Matrix) {
            for (int i = 0; i <= G.res; i++) {
                for (int j = 0; j<= G.res; j++)
                {
                    PointPayoff(i, j, Payoff, Matrix);
                }
            }
        }
        /// <summary>
        /// Depicts the arbitrary matrix (in the console)
        /// </summary>
        /// <param name="Matrix"></param>
        public void Depict(double[,] Matrix)
        {
            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    Console.Write(Matrix[i, j]); Console.Write("\t");
                }

                Console.WriteLine();
            }
            Console.WriteLine();
        }        
        /// <summary>
       /// Depicts the arbitrary matrix(in a file)
       /// </summary>
        public void Depictfile(double[,] Matrix, System.IO.StreamWriter file) {
            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    file.Write(Matrix[i, j]);file.Write("\t");
                }

                file.WriteLine();
            }
            file.WriteLine();
        }
        /// <summary>
        /// Summarizes the payoff and locations (message)
        /// </summary>
        /// <param name="predict"></param>
        /// <param name="xint"></param>
        /// <param name="yint"></param>
        /// <param name="Payoff"></param>
        /// <param name="Payoff2"></param>
        /// <param name="file"></param>
        public void Summary(int predict,int[]xint,int[]yint,double[,]Payoff,double[,]Payoff2,System.IO.StreamWriter file) {
            if (predict == 0)
            {
                file.Write("Optimal: \t Player 1: [{0},{1}] + payoff: [{2}] \t", xint[0], yint[0],Payoff[xint[0],yint[0]]);
                file.WriteLine("Player 2: [{0},{1}] + payoff: [{2}] \t", xint[1], yint[1], Payoff2[xint[1], yint[1]]);
            }
            else
            {
                file.WriteLine("\t" + xint[0] + "\t" + yint[0] + "\t" + xint[1] + "\t" + yint[1] +"\t"+ Payoff[xint[0], yint[0]]+"\t"+Payoff2[xint[1], yint[1]]);
                //Console.Write("Prediction: \t Player 1: [{0},{1}] \t", xint[0], yint[0]);
                //Console.WriteLine("Player 2: [{0},{1}]", xint[1], yint[1]);
            }
        }
        /// <summary>
        /// Generates player 1 and thus lower the density by half
        /// </summary>
        public void Generateplayer1(int[] xint, int[] yint, double[,] Matrix, double[,]sMatrix,int doesplayer2exist) {
            for (int i = G.minx1; i <= G.maxx1; i++) {
                for (int j = G.miny1; j <= G.maxy1; j++) {
                    if (Math.Sqrt(Math.Pow((double)(i - xint[0]),2) + Math.Pow((double)((j - yint[0])),2))<=G.radius) {
                        if ((Math.Sqrt(Math.Pow((double)(i - xint[1]), 2) + Math.Pow((double)((j - yint[1])), 2)) <= G.radius) && (doesplayer2exist ==1))
                        { } else {
                            Matrix[i, j] = Matrix[i, j] / 2;
                            sMatrix[i, j] = sMatrix[i, j] / 2;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Generates player 2 and thus lower the density by half
        /// </summary>        
        public void Generateplayer2(int[] xint, int[] yint, double[,] Matrix, double[,] sMatrix,int doesplayer1exist)
        {
            for (int i = G.minx2; i <= G.maxx2; i++)
            {
                for (int j = G.miny2; j <= G.maxy2; j++)
                {
                    if (Math.Sqrt(Math.Pow((double)(i - xint[1]), 2) + Math.Pow((double)((j - yint[1])), 2)) <= G.radius)
                    {
                        if ((Math.Sqrt(Math.Pow((double)(i - xint[0]), 2) + Math.Pow((double)((j - yint[0])), 2)) <= G.radius) && (doesplayer1exist==1))
                        { }
                        else {
                            Matrix[i, j] = Matrix[i,j]/2;
                            sMatrix[i, j] = sMatrix[i, j] / 2;
                        }
                        
                    
                    }

                }

            }
        }
        /// <summary>
        /// Deletes player 1 and thus increases the density by half
        /// </summary>
        public void Deleteplayer1(int[] xint,int[]yint, double[,] Matrix, double[,] sMatrix, int doesplayer2exist)
        {
            for (int i = G.minx1; i <= G.maxx1; i++)
            {
                for (int j = G.miny1; j <= G.maxy1; j++)
                {
                    if (Math.Sqrt(Math.Pow((double)(i - xint[0]), 2) + Math.Pow((double)((j - yint[0])), 2)) <= G.radius)
                    {
                        if ((Math.Sqrt(Math.Pow((double)(i - xint[1]), 2) + Math.Pow((double)((j - yint[1])), 2)) <= G.radius) && (doesplayer2exist == 1))
                        { }
                        else
                        {
                            Matrix[i, j] = Matrix[i, j] * 2;
                            sMatrix[i, j] = sMatrix[i, j] * 2;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Deletes player 2 and thus increases the density by half
        /// </summary>
        public void Deleteplayer2(int[] xint, int[] yint, double[,] Matrix, double[,] sMatrix, int doesplayer1exist)
        {
            for (int i = G.minx2; i <= G.maxx2; i++)
            {
                for (int j = G.miny2; j <= G.maxy2; j++)
                {
                    if (Math.Sqrt(Math.Pow((double)(i - xint[1]), 2) + Math.Pow((double)((j - yint[1])), 2)) <= G.radius)
                    {
                        if ((Math.Sqrt(Math.Pow((double)(i - xint[0]), 2) + Math.Pow((double)((j - yint[0])), 2)) <= G.radius) && (doesplayer1exist==1))
                        { }
                        else
                        {
                            Matrix[i, j] = Matrix[i, j] * 2;
                            sMatrix[i, j] = sMatrix[i, j] * 2;
                        }


                    }

                }

            }
        }
        /// <summary>
        /// Finds the location with the highest payoff available
        /// </summary>
        public void FindMax(double[,] Payoff,int[] optimalcoordinates) {
            double max=0;
            for (int i = 0; i < Payoff.GetLength(0); i++) {
                for (int j = 0; j < Payoff.GetLength(1); j++)
                {
                    if (Payoff[i, j] > max)
                    {
                        
                        max = Payoff[i, j];
                        optimalcoordinates[0] = i;
                        optimalcoordinates[1] = j;
                        
                    }
                }
            }
            ////Console.WriteLine("souradnice: \t" + optimalcoordinates[0] + "\t" + optimalcoordinates[1]);

        }

        /// <summary>
        /// Adjusts the position of the specific player
        /// </summary>
        public void Adjust(int player,int[]xint,int[]yint,int[]optimalcoordinates) {
            xint[player] = optimalcoordinates[0];
            yint[player] = optimalcoordinates[1];
        }
        /// <summary>
        /// Adjusts the position of the specific player only marginally (can be set by parameter "step")
        /// </summary>
        public void AdjustStep(int player, int[] xint, int[] yint, int[] optimalcoordinates,double step)
        {
            double distancefromoptimal, ratio;
            distancefromoptimal= Math.Sqrt(Math.Pow(optimalcoordinates[0] - xint[player], 2) + Math.Pow(optimalcoordinates[1] - yint[player], 2));
            ratio = step / distancefromoptimal;
            if (ratio <= 1)
            {
                ////Console.WriteLine("BeforAdjust player:\t" + player + "\t xint:\t" + xint[player] + "\t yint:\t" + yint[player]+"\t distance: \t"+distancefromoptimal+"\t ratio \t"+ratio);
                xint[player] = (int)(Math.Floor(xint[player] + (optimalcoordinates[0] - xint[player]) * ratio));
                yint[player] = (int)(Math.Floor(yint[player] + (optimalcoordinates[1] - yint[player]) * ratio));
                ////Console.WriteLine("AfterAdjust player:\t" + player + "\t xint:\t" + xint[player] + "\t yint:\t" + yint[player] + "\t distance: \t" + distancefromoptimal + "\t ratio \t" + ratio);
            }
            else
            {
                Adjust(player, xint, yint, optimalcoordinates);
            }
        }
        /// <summary>
        /// Initialize integer values based on the values of global variables etc.
        /// </summary>
        /// <param name="xint"></param>
        /// <param name="yint"></param>
        public void Initialize(int[] xint,int[]yint) {
            G.radius = G.e * G.res;
            G.sradius = G.s * G.res;
            G.intradius = Convert.ToInt32(G.radius);
            G.intsradius = Convert.ToInt32(G.sradius);
            G.maxx1 = Math.Min(G.res, xint[0] + G.intradius);
            G.maxy1 = Math.Min(G.res, yint[0] + G.intradius);
            G.minx1 = Math.Max(0, xint[0] - G.intradius);
            G.miny1 = Math.Max(0, yint[0] - G.intradius);
            G.maxx2 = Math.Min(G.res, xint[1] + G.intradius);
            G.maxy2 = Math.Min(G.res, yint[1] + G.intradius);
            G.minx2 = Math.Max(0, xint[1] - G.intradius);
            G.miny2 = Math.Max(0, yint[1] - G.intradius);
        }
        /// <summary>
        /// Predicts the density at one point
        /// </summary>
        public void PredictPoint(int player,int[] xint,int[]yint,int[]coordinates,double sradius) {
            double distance, ratio;
            distance = Math.Sqrt(Math.Pow(coordinates[0] - xint[player], 2) + Math.Pow(coordinates[1] - yint[player], 2));
            ratio = sradius / distance;
            coordinates[1] = (int)(Math.Floor(yint[player] + ratio * (coordinates[1] - yint[player])));
            coordinates[0] = (int)(Math.Floor(xint[player] + ratio * (coordinates[0] - xint[player])));
        }
        /// <summary>
        /// Predicts the density everywhere
        /// </summary>
        public void PredictsMatrix(int player,int[]xint,int[]yint,double[,]Matrix,double[,]sMatrix,double sradius) {
            int[] coordinates=new int[2];
            int a, b;
            for (int i = 0; i<=G.res; i++) {
                for (int j = 0; j <= G.res; j++) {
                    if (Math.Sqrt(Math.Pow(i - xint[player],2) + Math.Pow(j - yint[player],2))<=G.sradius){
                        sMatrix[i, j] = Matrix[i, j];
                    }
                    else
                    {
                        coordinates[0] = i;
                        coordinates[1] = j;
                        PredictPoint(player, xint, yint, coordinates, sradius);
                        a = coordinates[0];
                        b = coordinates[1];
                        
                        sMatrix[i, j] = Matrix[a, b];

                    }

                }

            }

            
            



        }
    }
    class Subroutine1D //The subroutines have the same meaning as in 2D, only not commented
    {
        /// <summary>
        /// Generates 1D linear distribution, puts player 1 to the optimal payoff location, then adds player 2 to the best response location
        /// </summary>
        /// <param name="xint"></param>
        /// <param name="Vector"></param>
        /// <param name="PayoffVector"></param>
        /// <param name="optimalcoordinates"></param>
        /// <param name="sVector"></param>
        /// <param name="Payoff2Vector"></param>
        /// <param name="file"></param>
        public void setup1(int[] xint, double[] Vector, double[] PayoffVector, int[] optimalcoordinates, double[] sVector, double[] Payoff2Vector,System.IO.StreamWriter file) {
            int player;
            Initialize(xint);
            Generatelineardistribution(Vector);
            //Depict(Vector);
            PlayerPayoff(PayoffVector, Vector);
            //Depict(PayoffVector);
            FindMax(PayoffVector, optimalcoordinates);
            player = 0;
            Adjust(player, xint, optimalcoordinates);
            Initialize(xint);
            Generateplayer1(xint, Vector, sVector, 0);
            //Depict(Vector);
            PlayerPayoff(Payoff2Vector, Vector);
            //Depict(Payoff2Vector);
            FindMax(Payoff2Vector, optimalcoordinates);
            player = 1;
            Adjust(player, xint, optimalcoordinates);
            Initialize(xint);
            Generateplayer2(xint, Vector, sVector);
            //Depict(Vector);
            Summary(0, xint, PayoffVector, Payoff2Vector,file);
        }
        /// <summary>
        /// Finding Nash equilibrium in 1D repeated game (used in thesis only for sine)
        /// </summary>
        public void setup1repeat(int[] xint, int[]xintmemory, double[] Vector, double[] PayoffVector, int[] optimalcoordinates, double[] sVector, double[] Payoff2Vector, System.IO.StreamWriter file) {
            int player;
            while ((xintmemory[0] != xint[0]) || (xintmemory[1] != xint[1]))
            {
                xintmemory = (int[])xint.Clone();
                Deleteplayer1(xint, Vector, sVector, 1);
                //Depict(Vector);
                PlayerPayoff(PayoffVector, Vector);
                //Depict(PayoffVector);
                FindMax(PayoffVector, optimalcoordinates);
                player = 0;
                Adjust(player, xint, optimalcoordinates);
                Initialize(xint);
                Generateplayer1(xint, Vector, sVector, 1);
                Deleteplayer2(xint, Vector, sVector);
                //Depict(Vector);
                PlayerPayoff(Payoff2Vector, Vector);
                //Depict(Payoff2Vector);
                FindMax(Payoff2Vector, optimalcoordinates);
                player = 1;
                Adjust(player, xint, optimalcoordinates);
                Initialize(xint);
                Generateplayer2(xint, Vector, sVector);
                //Depict(Vector);
                Summary(0, xint, PayoffVector, Payoff2Vector, file);
            }


        }
        /// <summary>
        /// One player predicts and then adjusts (chapter 4.2)
        /// </summary>
        public void setup1stageprediction1player(int[]xint,double[]Vector,double[]PayoffVector,int[]optimalcoordinates,double[]sVector,System.IO.StreamWriter file) {
            int player;
            Initialize(xint);
            Generatelineardistribution(Vector);
            player = 0;
            PredictsMatrix(player, xint, Vector, sVector);
            //Depict(Vector);
            //Depict(sVector);
            PlayerPayoff(PayoffVector, sVector);
            FindMax(PayoffVector, optimalcoordinates);
            Adjust(player, xint, optimalcoordinates);
            Initialize(xint);
            PlayerPayoff(PayoffVector, Vector); //ABY BYLO V SUMMARY SKUTECNEJ PAYOFF
            Generateplayer1(xint, Vector, sVector, 0);
            Summary1player(1, xint, PayoffVector, file);

        }
        /// <summary>
        /// One player predicts sine density and then adjusts (chapter 4.2)
        /// </summary>
        public void setupSINUS1stageprediction1player(int[] xint, double[] Vector, double[] PayoffVector, int[] optimalcoordinates, double[] sVector, System.IO.StreamWriter file)
        {
            int player;
            Initialize(xint);
            Generatesinusdistribution(Vector,1,1);
            player = 0;
            PredictsMatrixCircle(player, xint, Vector, sVector);
            sVector[G.res]= 0;
            //Depict(Vector);
            //Depict(sVector);
            PlayerPayoffCircle(PayoffVector, sVector);
            //Depict(PayoffVector);
            FindMax(PayoffVector, optimalcoordinates);
            Adjust(player, xint, optimalcoordinates);
            Initialize(xint);
            PlayerPayoffCircle(PayoffVector, Vector); //ABY BYLO V SUMMARY SKUTECNEJ PAYOFF
            Generateplayer1(xint, Vector, sVector, 0);
            Summary1player(1, xint, PayoffVector, file);

        }
        /// <summary>
        /// Finding optimal locations for sine function in 1D (not used)
        /// </summary>
        /// <param name="xint"></param>
        /// <param name="Vector"></param>
        /// <param name="PayoffVector"></param>
        /// <param name="Payoff2Vector"></param>
        /// <param name="optimalcoordinates"></param>
        /// <param name="sVector"></param>
        /// <param name="file"></param>
        public void setupSINUS1stageoptimal2players(int[] xint, double[] Vector, double[] PayoffVector, double[] Payoff2Vector, int[] optimalcoordinates, double[] sVector, System.IO.StreamWriter file)
        {
            int player;
            Initialize(xint);
            Generatesinusdistribution(Vector, 1, 1);
            player = 0;
            //PredictsMatrixCircle(player, xint, Vector, sVector);
            //sVector[G.res] = 0;
            //Depict(Vector);
            //Depict(sVector);
            PlayerPayoffCircle(PayoffVector, Vector);
            //Depict(PayoffVector);
            FindMax(PayoffVector, optimalcoordinates);
            Adjust(player, xint, optimalcoordinates);
            Initialize(xint);
            PlayerPayoffCircle(PayoffVector, Vector);
            Generateplayer1circle(xint, Vector, sVector, 0);
            PlayerPayoffCircle(Payoff2Vector, Vector);
            //Depict(Payoff2Vector);
            FindMax(Payoff2Vector, optimalcoordinates);
            player = 1;
            Adjust(player, xint, optimalcoordinates);
            Initialize(xint);
            Generateplayer2circle(xint, Vector, sVector);
            Summary(1, xint, PayoffVector, Payoff2Vector, file);
            file.WriteLine();
        }
        /// <summary>
        /// subroutine generating repeated game at circle space
        /// </summary>
        public void setup1repeatcircle(int[] xint, int[] xintmemory, double[] Vector, double[] PayoffVector, int[] optimalcoordinates, double[] sVector, double[] Payoff2Vector, System.IO.StreamWriter file)
        {
            int player;
            while ((xintmemory[0] != xint[0]) || (xintmemory[1] != xint[1]))
            {
                xintmemory = (int[])xint.Clone();
                Deleteplayer1circle(xint, Vector, sVector, 1);
                //Depict(Vector);
                PlayerPayoffCircle(PayoffVector, Vector);
                //Depict(PayoffVector);
                FindMax(PayoffVector, optimalcoordinates);
                player = 0;
                Adjust(player, xint, optimalcoordinates);
                Initialize(xint);
                Generateplayer1circle(xint, Vector, sVector, 1);
                Deleteplayer2circle(xint, Vector, sVector);
                //Depict(Vector);
                PlayerPayoffCircle(Payoff2Vector, Vector);
                //Depict(Payoff2Vector);
                FindMax(Payoff2Vector, optimalcoordinates);
                player = 1;
                Adjust(player, xint, optimalcoordinates);
                Initialize(xint);
                Generateplayer2circle(xint, Vector, sVector);
                Console.WriteLine("Depict: VECTOR,Payoff,Payoff2");
                Depict(Vector);
                Depict(PayoffVector);
                Depict(Payoff2Vector);
                Console.WriteLine("KONEC");
                Summary(0, xint, PayoffVector, Payoff2Vector, file);
            }


        }
        /// <summary>
        /// Sine two player prediction simulation (chapter 4.3)
        /// </summary>
        public void setupSINUS2stageprediction2players(int[] xint, double[] Vector, double[] PayoffVector, int[] optimalcoordinates, double[] sVector, double[] Payoff2Vector, System.IO.StreamWriter file)
        {
            int player;
            Initialize(xint);
            Generatesinusdistribution(Vector,1,1);
            //Depict(Vector);
            PlayerPayoffCircle(PayoffVector, Vector);
            //Depict(PayoffVector);
            //FindMax(PayoffVector, optimalcoordinates);
            //player = 0;
            //Adjust(player, xint, optimalcoordinates);
            //Initialize(xint)
            player = 1;
            PredictsMatrixCircle(player, xint, Vector, sVector);
            Generateplayer1circle(xint, Vector, sVector, 0);
            //Depict(Vector);
            PlayerPayoffCircle(Payoff2Vector, sVector);
            //Depict(Payoff2Vector);
            FindMax(Payoff2Vector, optimalcoordinates);
            Adjust(player, xint, optimalcoordinates);
            Initialize(xint);
            PlayerPayoffCircle(Payoff2Vector, Vector); //DO SUMMARY
            //Generateplayer2(xint, Vector, sVector);
            //Depict(Vector);
            Generateplayer2circle(xint, Vector, sVector);
            Deleteplayer1circle(xint, Vector, sVector, 1);
            PlayerPayoffCircle(PayoffVector, Vector); // DO SUMMARY
            Summary(0, xint, PayoffVector, Payoff2Vector, file);
        }
        public void Generateuniformdistribution(double[] Vector)
        {
            for (int i = 0; i <= G.res; i++)
            {
                Vector[i] = 1;
            }
        }
        public void Generatelineardistribution(double[] Vector) {
            for (int i = 0; i <= G.res; i++) {
                Vector[i] = 2*(double)i / G.res;
            }
        }
        /// <summary>
        /// Distribution on the circle space (requires one less resolution)
        /// </summary>
        public void Generatesinusdistribution(double[] Vector,double a,int k)
        {
            for (int i = 0; i <= G.res-1; i++)
            {  
                 Vector[i] = a*Math.Sin(2*Math.PI*k*i/G.res)+1; 
     
            }
        } 
        public void PointPayoff(int inputi, double[] PayoffVector, double[] Vector)
        {

            PayoffVector[inputi] = 0;
            for (int i = Math.Max(0, inputi - G.intradius); i <= Math.Min(G.res, inputi + G.intradius); i++)
            {
                if (Math.Abs((double)(i - inputi)) <= G.intradius)
                {
                    PayoffVector[inputi] = PayoffVector[inputi] + Vector[i];
                }
            }
        }
        /// <summary>
        /// Point payoff on the circle space (all subroutines containing word circle are on the circle space)
        /// </summary>
        /// <param name="inputi"></param>
        /// <param name="PayoffVector"></param>
        /// <param name="Vector"></param>
        public void PointPayoffCircle(int inputi, double[] PayoffVector, double[] Vector)
        {

            PayoffVector[inputi] = 0;
            for (int i = Math.Max(0, inputi - G.intradius); i <= Math.Min(G.res, inputi + G.intradius); i++)
            {
                if (Math.Abs((double)(i - inputi)) <= G.intradius)
                {
                    PayoffVector[inputi] = PayoffVector[inputi] + Vector[i];
                }
            }

            if (inputi - G.intradius < 0) {
                for (int i = G.res + inputi - G.intradius; i <= G.res; i++) {
                    PayoffVector[inputi] = PayoffVector[inputi] + Vector[i];
                }
            }

            if (inputi + G.intradius >= G.res)
            {
                for (int i = 0; i <= inputi+G.intradius-G.res; i++)
                {
                    PayoffVector[inputi] = PayoffVector[inputi] + Vector[i];
                }
            }

        }
        public void PlayerPayoff(double[] PayoffVector, double[] Vector)
        {
            for (int i = 0; i <= G.res; i++)
            {
                PointPayoff(i, PayoffVector, Vector);
            }
        }
        public void PlayerPayoffCircle(double[] PayoffVector, double[] Vector)
        {
            for (int i = 0; i <= G.res-1; i++)
            {
                PointPayoffCircle(i, PayoffVector, Vector);
            }
        }
        public void Depict(double[] Vector)
        {
            for (int i = 0; i < Vector.GetLength(0); i++)
            {
                
                    Console.Write(Vector[i]); Console.Write("\t");
            }
            Console.WriteLine();
        }
        public void Summary(int predict, int[] xint, double[] PayoffVector, double[] Payoff2Vector, System.IO.StreamWriter file)
        {
            if (predict == 0)
            {
               
                file.Write("Optimal: \t Player 1: \t{0}\t + payoff: \t{1}\t", xint[0], PayoffVector[xint[0]]);
                file.WriteLine("Player 2: \t{0}\t + payoff: \t{1}\t", xint[1], Payoff2Vector[xint[0]]);
            }
            else
            {
                file.Write("e=" + G.e + "\t");
                file.Write("\t \t \t" + xint[0] + "\t" + xint[1] + "\t" + PayoffVector[xint[0]] + "\t" + Payoff2Vector[xint[1]]);
                //Console.Write("Prediction: \t Player 1: [{0},{1}] \t", xint[0], yint[0]);
                //Console.WriteLine("Player 2: [{0},{1}]", xint[1], yint[1]);
            }
        }
        public void Summary1player(int predict, int[] xint, double[] PayoffVector, System.IO.StreamWriter file)
        {
            if (predict == 0)
            {
                file.WriteLine("Optimal: \t Player 1: [{0}] + payoff: [{1}] \t", xint[0], PayoffVector[xint[0]]);
                
            }
            else
            {
                file.Write("\t" + xint[0] + "\t" + PayoffVector[xint[0]]);
                //Console.Write("Prediction: \t Player 1: [{0},{1}] \t", xint[0], yint[0]);
                //Console.WriteLine("Player 2: [{0},{1}]", xint[1], yint[1]);
            }
        }
        public void Generateplayer1(int[] xint, double[] Vector, double[] sVector,int doesplayer2exist)
        {
            for (int i = G.minx1; i <= G.maxx1; i++)
            {

                if (Math.Abs((double)(i - xint[0])) <= G.radius)
                {
                    if ((Math.Abs((double)(i - xint[1])) <= G.radius) && (doesplayer2exist == 1))
                    {
                        Vector[i] = Vector[i];
                        sVector[i] = sVector[i];
                    }
                    else
                    {
                        Vector[i] = Vector[i] / 2;
                        sVector[i] = sVector[i] / 2;
                    }
                }


            }
            
        }       
        public void Generateplayer2(int[] xint, double[] Vector, double[] sVector)
        {
            for (int i = G.minx2; i <= G.maxx2; i++)
            {

                if (Math.Abs((double)(i - xint[1])) <= G.radius)
                {
                    if (Math.Abs((double)(i - xint[0])) <= G.radius)
                    {
                        Vector[i] = Vector[i];
                        sVector[i] = sVector[i];
                    }
                    else
                    {
                        Vector[i] = Vector[i] / 2;//!!!!!!!!!!!!
                        sVector[i] = sVector[i] / 2;/////////!!!
                    }
                }
            }
        }
        public void Generateplayer1circle(int[] xint, double[] Vector, double[] sVector, int doesplayer2exist)
        {
            for (int i = 0; i <= G.res; i++)
            {

                if (Math.Abs((double)(i - xint[0])) <= G.radius || ((G.res - i + xint[0] <= G.radius) && (i > xint[0])) || ((G.res + i - xint[0] <= G.radius) && (i < xint[0])))
                {
                    if (
                        (Math.Abs((double)(i - xint[1])) <= G.radius || ((G.res - i + xint[1] <= G.radius) && (i > xint[1])) || ((G.res + i - xint[1] <= G.radius) && (i < xint[1]))
                        ) 
                        && (doesplayer2exist == 1))
                    {
                        Vector[i] = Vector[i];
                        sVector[i] = sVector[i];
                    }
                    else
                    {
                        Vector[i] = Vector[i] / 2;
                        sVector[i] = sVector[i] / 2;
                    }
                }
            }
        }
        public void Generateplayer2circle(int[] xint, double[] Vector, double[] sVector)
        {
            for (int i = 0; i <= G.res; i++)
            {

                if (Math.Abs((double)(i - xint[1])) <= G.radius || ((G.res - i + xint[1] <= G.radius) && (i > xint[1])) || ((G.res + i - xint[1] <= G.radius) && (i < xint[1])))
                {
                    if (Math.Abs((double)(i - xint[0])) <= G.radius || ((G.res - i + xint[0] <= G.radius) && (i > xint[0])) || ((G.res + i - xint[0] <= G.radius) && (i < xint[0])))
                    {
                        Vector[i] = Vector[i];
                        sVector[i] = sVector[i];
                    }
                    else
                    {
                        Vector[i] = Vector[i] / 2;//!!!!!!!!!!!!
                        sVector[i] = sVector[i] / 2;/////////!!!
                    }
                }
            }
        }
        public void Deleteplayer1(int[] xint, double[] Vector, double[] sVector,int doesplayer2exist) {
            for (int i = G.minx1; i <= G.maxx1; i++)
            {
                if (Math.Abs((double)(i - xint[0])) <= G.radius)
                {
                    if ((Math.Abs((double)(i - xint[1])) <= G.radius) && (doesplayer2exist == 1))
                    {
                        Vector[i] = Vector[i];
                        sVector[i] = sVector[i];
                    }
                    else
                    {
                        Vector[i] = Vector[i] * 2;
                        sVector[i] = sVector[i] * 2;
                    }
                }
            }
        }
        public void Deleteplayer1circle(int[] xint, double[] Vector, double[] sVector, int doesplayer2exist)
        {
            for (int i = 0; i <= G.res; i++)
            {
                if (Math.Abs((double)(i - xint[0])) <= G.radius || ((G.res - i + xint[0] <= G.radius) && (i > xint[0])) || ((G.res + i - xint[0] <= G.radius) && (i < xint[0])))
                {
                    if ((Math.Abs((double)(i - xint[1])) <= G.radius || ((G.res - i + xint[1] <= G.radius) && (i > xint[1])) || ((G.res + i - xint[1] <= G.radius) && (i < xint[1]))) && (doesplayer2exist == 1))
                    {
                        Vector[i] = Vector[i];
                        sVector[i] = sVector[i];
                    }
                    else
                    {
                        Vector[i] = Vector[i] * 2;
                        sVector[i] = sVector[i] * 2;
                    }
                }
            }
        }
        public void Deleteplayer2(int[] xint, double[] Vector, double[] sVector)
        {
            for (int i = G.minx2; i <= G.maxx2; i++)
            {
                if (Math.Abs((double)(i - xint[1])) <= G.radius)
                {
                    if (Math.Abs((double)(i - xint[0])) <= G.radius)
                    {
                        Vector[i] = Vector[i];
                        sVector[i] = sVector[i];
                    }
                    else
                    {
                        Vector[i] = Vector[i] * 2;
                        sVector[i] = sVector[i] * 2;
                    }
                }
            }
        }
        public void Deleteplayer2circle(int[] xint, double[] Vector, double[] sVector)
        {
            for (int i = 0; i <= G.res; i++)
            {
                if (Math.Abs((double)(i - xint[1])) <= G.radius || ((G.res - i + xint[1] <= G.radius) && (i > xint[1])) || ((G.res + i - xint[1] <= G.radius) && (i < xint[1])))
                {
                    if (Math.Abs((double)(i - xint[0])) <= G.radius || ((G.res - i + xint[0] <= G.radius) && (i > xint[0])) || ((G.res + i - xint[0] <= G.radius) && (i < xint[0])))
                    {
                        Vector[i] = Vector[i];
                        sVector[i] = sVector[i];
                    }
                    else
                    {
                        Vector[i] = Vector[i] * 2;
                        sVector[i] = sVector[i] * 2;
                    }
                }
            }
        }
        public void FindMax(double[] PayoffVector, int[]optimalcoordinates)
        {
            double max = 0;
            for (int i = 0; i < PayoffVector.GetLength(0); i++)
            {
                    if (PayoffVector[i] > max)
                    {
                        max = PayoffVector[i];
                        optimalcoordinates[0] = i;
                    }
                
            }
        }      
        public void Adjust(int player, int[] xint, int[]optimalcoordinates)
        {
            xint[player] = optimalcoordinates[0];
        }
        public void Initialize(int[] xint)
        {
            G.radius = G.e * G.res;
            G.sradius = G.s * G.res;
            G.intradius = Convert.ToInt32(G.radius);
            G.intsradius = Convert.ToInt32(G.sradius);
            G.maxx1 = Math.Min(G.res, xint[0] + G.intradius);
            G.minx1 = Math.Max(0, xint[0] - G.intradius);
            G.maxx2 = Math.Min(G.res, xint[1] + G.intradius);
            G.minx2 = Math.Max(0, xint[1] - G.intradius);
       
        }
        public void PredictPoint(int player, int[] xint, int[] coordinates)
        {
            double distance, ratio;
            distance = Math.Abs(coordinates[0] - xint[player]);
            ratio = G.sradius / distance;
            coordinates[0] = (int)(Math.Round(xint[player] + ratio * (coordinates[0] - xint[player])));

        }
        public void PredictsMatrix(int player, int[] xint, double[] Vector, double[] sVector)
        {
            int[] coordinates = new int[2];
            int a, b;
            for (int i = 0; i <= G.res; i++)
            {
                if (Math.Abs(i - xint[player]) <= G.sradius)
                {
                    sVector[i] = Vector[i];
                }
                else
                {
                    coordinates[0] = i;
                    PredictPoint(player, xint, coordinates);
                    a = coordinates[0];
                    sVector[i] = Vector[a];
                }
            }
        }
        public void PredictPointCircle(int player, int[] xint, int[] coordinates)
        {
            double leftdistance,rightdistance, leftratio,rightratio;
            if (coordinates[0] - xint[player] < 0)
            {
                leftdistance = Math.Abs(coordinates[0] - xint[player]);
                rightdistance = G.res - leftdistance;
            }
            else {
                rightdistance = Math.Abs(coordinates[0] - xint[player]);
                leftdistance = G.res - rightdistance;
            }
            leftratio = G.sradius / leftdistance;
            rightratio = G.sradius / rightdistance;
            //Console.WriteLine("leftdistance" + leftdistance + "rightdistance" + rightdistance + "leftratio" + leftratio + "rightratio" + rightratio);
            if (xint[player] - G.sradius < 0)
            {
                coordinates[1] = (int)(Math.Round(xint[player] + G.sradius));
                coordinates[0] = (int)(Math.Round(xint[player] - G.sradius + G.res));
            }
            else {
                if (xint[player] + G.sradius > G.res)
                {
                    coordinates[1] = (int)(Math.Round(xint[player] + G.sradius - G.res));
                    coordinates[0] = (int)(Math.Round(xint[player] - G.sradius));
                }
                else {
                    coordinates[1] = (int)(Math.Round(xint[player] + G.sradius));
                    coordinates[0] = (int)(Math.Round(xint[player] - G.sradius));
                }
            }                      
        }
        public void PredictsMatrixCircle(int player, int[] xint, double[] Vector, double[] sVector)
        {
            int difference;
            int[] coordinates = new int[2];
            int a, b;
            double lambda;
            for (int i = 0; i <= G.res; i++)
            {
                if (Math.Abs(i - xint[player]) <= G.sradius || ((G.res-i+xint[player] <= G.sradius)&&(i>xint[player])) || ((G.res + i - xint[player] <= G.sradius) && (i < xint[player])))
                {
                    sVector[i] = Vector[i];
                }
                else
                {
                    coordinates[0] = i;
                    PredictPointCircle(player, xint, coordinates);
                    a = coordinates[0];
                    b = coordinates[1];
                    if (a <= b) { difference = G.res-(b - a); if (i <= a) { lambda = (double)(a - i) / difference; } else { lambda = (double)(G.res-(i-a)) / difference; } }
                    else {
                        difference = a - b; if (i <= a) { lambda = (double)(a - i) / difference; } else { lambda = (double)(G.res - (i - a)) / difference; }

                    };
                    //Console.WriteLine("difference"+difference+"a"+a+"b"+b+"i"+i+"xint"+xint[player]);
                    
                    //Console.WriteLine(lambda);
                    sVector[i] = (lambda)*Vector[b]+(1-lambda)*Vector[a];
                }
            }
            sVector[G.res] = 0;
        }



    }
    class Program
    {
        static void Main(string[] args)
        {
            //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\a.txt", true))    // output to file
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Console.OpenStandardOutput())) // output to console
            {
                int[] optimalcoordinates = new int[2];
                double[,] Matrix = new double[G.res + 1, G.res + 1]; //Density function in 2D
                double[,] sMatrix = new double[G.res + 1, G.res + 1]; //Predicted density function in 2D
                double[,] Payoff = new double[G.res + 1, G.res + 1]; //Payoff of the player 1
                double[,] Payoff2 = new double[G.res + 1, G.res + 1]; //Payoff of the player 2
                double[,] Payoffandsonar = new double[5000, 10]; //Array used for saving values (not any real interpretation)
                int[] xint = new int[2]; //x coordinates of first and second player (rounded on the closest integer)
                int[] yint = new int[2]; //y coordinates of first and second player (rounded on the closest integer)
                double[] x = new double[2]; //x coordinates of first and second player
                double[] y = new double[2]; //y coordinates of first and second player
                int player; // (player == 0) means player 1; (player == 1) means player 2
                Subroutine2D sub = new Subroutine2D(); // object which launches two-dimensional subroutines
                Random rnd = new Random(); // object used as a random generator
                double[] Vector = new double[G.res + 1]; //Density function in 1D
                double[] sVector = new double[G.res + 1]; //Predicted density function in 1D
                double[] PayoffVector = new double[G.res + 1]; //Payoff of the player 1 in 1D
                double[] Payoff2Vector = new double[G.res + 1]; //Payoff of the player 2 in 2D
                int[] xintmemory = new int[2]; //keep memorized value of the xint variable
                Subroutine1D sub1D = new Subroutine1D(); // object which launches one-dimensional subroutines


                ////////////////1D////////////////////////////
                /*
                for (G.e = 0.02; G.e <= 0.51; G.e = G.e + 0.02) {
                    Console.WriteLine("G.e" + G.e);*/
                //////////////CLASSICAL MODEL/////////////////
                //Console.WriteLine("e=\t" + G.e);
                //sub1D.setup1(xint, Vector, PayoffVector, optimalcoordinates, sVector, Payoff2Vector, file);
                /////////////////////REPEAT//////////////////
                //sub1D.setup1repeat(xint, xintmemory, Vector, PayoffVector, optimalcoordinates, sVector, Payoff2Vector, file);
                ////////////SINE1STAGE///////////////
                //sub1D.setupSINUS1stageprediction1player(xint, Vector, PayoffVector, optimalcoordinates, sVector, file);
                //sub1D.setup1repeatcircle(xint, xintmemory, Vector, PayoffVector, optimalcoordinates, sVector, Payoff2Vector, file);
                ////////////////////SINEOPTIMAL///////////////////////
                //sub1D.setupSINUS1stageoptimal2players(xint, Vector, PayoffVector, Payoff2Vector, optimalcoordinates, sVector, file);
                //sub1D.setup1repeatcircle(xint, xintmemory, Vector, PayoffVector, optimalcoordinates, sVector, Payoff2Vector, file);
                ///////////////////////////////////////////////
                ////////////////PREDICTIVE MODEL1STAGE///////////////PLAYER1 RANDOM POSITION AND THEN ADJUSTS HIS POSITION
                /*for (G.s = G.e; G.s <= 0.51; G.s = G.s + 0.02) {
                    for (int i = 0; i <= 49; i++) {                         
                        x[0] = (double)i / G.res;
                        optimalcoordinates[0] = 0;
                        optimalcoordinates[1] = 0;
                        xint[0] = Convert.ToInt32(x[0] * G.res);
                        file.Write(i+"\t 1STAGE");
                        sub1D.setupSINUS1stageprediction1player(xint, Vector, PayoffVector, optimalcoordinates, sVector, file);
                        file.WriteLine("\t Player 1: \t" + PayoffVector[i] + "\t INCENTIVES: \t" + (PayoffVector[xint[0]] - PayoffVector[i]) + "\t e \t" + G.e + "\t s \t" + G.s);
                    }
                }
            }
            for (G.e = 0.02; G.e <= 0.51; G.e = G.e + 0.02)
            {
                //Console.WriteLine("e=\t" + G.e);
                for (G.s = G.e; G.s <= 0.51; G.s = G.s + 0.02)
                {
                    //Console.WriteLine("s=\t" + G.s);
                    for (int i = 0; i <= 50; i++)
                    {
                        for (int j = 0; j <= 50; j++)
                        {

                            file.Write(i + "\t" + j);
                            ///////////////////////////////////////////////
                            x[0] = (double)i / G.res;
                            x[1] = (double)j / G.res;
                            optimalcoordinates[0] = 0;
                            optimalcoordinates[1] = 0;
                            xint[0] = Convert.ToInt32(x[0] * G.res);
                            xint[1] = Convert.ToInt32(x[1] * G.res);



                            ///////////////////////////////////////////////
                            ////////////////PREDICTIVE MODEL 2STAGES///////////////PLAYER1 RANDOM POSITION, PLAYER2 RANDOM POSITION AND THEN ADJUSTS HIS POSITION,
                            file.Write("\t 2STAGE");
                            sub1D.Initialize(xint);
                            sub1D.Generatelineardistribution(Vector);
                            //player = 0;
                            //sub1D.PredictsMatrix(player, xint, Vector, sVector);
                            //sub1D.PlayerPayoff(PayoffVector, Vector);
                            //sub1D.FindMax(PayoffVector, optimalcoordinates);
                            //sub1D.Adjust(player, xint, optimalcoordinates);
                            //sub1D.Initialize(xint);
                            player = 1;
                            sub1D.PredictsMatrix(player, xint, Vector, sVector);
                            sub1D.Generateplayer1(xint, Vector, sVector, 0);
                            sub1D.PlayerPayoff(Payoff2Vector, sVector);
                            //sub1D.Depict(sVector);
                            sub1D.FindMax(Payoff2Vector, optimalcoordinates);
                            sub1D.Adjust(player, xint, optimalcoordinates);
                            sub1D.Initialize(xint);
                            sub1D.Generateplayer2(xint, Vector, sVector);
                            //sub1D.Depict(Vector);
                            sub1D.Summary(1, xint, PayoffVector, Payoff2Vector, file);
                            sub1D.Generatelineardistribution(Vector);
                            sub1D.Generateplayer1(xint, Vector, sVector, 0);
                            sub1D.PlayerPayoff(PayoffVector, Vector);
                            file.Write("\t Player 2: \t" + PayoffVector[xint[1]] + "\t");
                            sub1D.Generateplayer2(xint, Vector, sVector);
                            sub1D.Deleteplayer1(xint, Vector, sVector, 1);
                            sub1D.PlayerPayoff(PayoffVector, Vector);
                            file.WriteLine("\t Player 1: \t" + PayoffVector[xint[0]] + "\t e \t" + G.e + "\t s \t" + G.s);
                            //xintmemory[0] = 0;
                            //xintmemory[1] = 0;

                            /*
                            ///////////////////////////////////////////////
                            ////////////////SINE PREDICTIVE MODEL 2STAGES///////////////PLAYER1 RANDOM POSITION, PLAYER2 RANDOM POSITION AND THEN ADJUSTS HIS POSITION,
                            file.Write("\t 2STAGE");
                            sub1D.Initialize(xint);
                            sub1D.Generatesinusdistribution(Vector,1,1);
                            //player = 0;
                            //sub1D.PredictsMatrix(player, xint, Vector, sVector);
                            //sub1D.PlayerPayoff(PayoffVector, Vector);
                            //sub1D.FindMax(PayoffVector, optimalcoordinates);
                            //sub1D.Adjust(player, xint, optimalcoordinates);
                            //sub1D.Initialize(xint);
                            player = 1;
                            sub1D.PredictsMatrixCircle(player, xint, Vector, sVector);
                            sub1D.Generateplayer1circle(xint, Vector, sVector, 0);
                            sub1D.PlayerPayoffCircle(Payoff2Vector, sVector);
                            //sub1D.Depict(sVector);
                            //sub1D.Depict(Payoff2Vector);
                            sub1D.FindMax(Payoff2Vector, optimalcoordinates);
                            sub1D.Adjust(player, xint, optimalcoordinates);
                            sub1D.Initialize(xint);
                            sub1D.Generateplayer2circle(xint, Vector, sVector);
                            //sub1D.Depict(Vector);
                            sub1D.Summary(1, xint, PayoffVector, Payoff2Vector, file);
                            sub1D.Generatesinusdistribution(Vector,1,1);
                            sub1D.Generateplayer1circle(xint, Vector, sVector, 0);
                            sub1D.PlayerPayoffCircle(PayoffVector, Vector);

                            file.Write("\t Player 2: \t" + PayoffVector[xint[1]] + "\t");
                            sub1D.Generateplayer2circle(xint, Vector, sVector);
                            sub1D.Deleteplayer1circle(xint, Vector, sVector, 1);
                            sub1D.PlayerPayoffCircle(PayoffVector, Vector);

                            file.WriteLine("\t Player 1: \t" + PayoffVector[xint[0]] + "\t e \t" + G.e + "\t s \t" + G.s);
                            //xintmemory[0] = 0;
                            //xintmemory[1] = 0;

                            ////////////////////////REPEAT//////////////////
                            ///
                            //sub1D.setup1repeat(xint, xintmemory, Vector, PayoffVector, optimalcoordinates, sVector, Payoff2Vector, file);

                        }
                    }
                }
            } 
                        }*/
                /////////////////////2D///////////////////////

                //////////////CLASSICAL MODEL/////////////////
                //sub.Setup1(xint, yint, Matrix, Payoff, optimalcoordinates, sMatrix, Payoff2);


                /*for (G.e = 0.0; G.e <= 0.51; G.e = G.e + 0.01)
                {
                    
                    //for (G.s = G.e; G.s <= 0.51; G.s = G.s + 0.05)
                    //{
                        Console.WriteLine("e \t" + G.e+"\t s \t"+ G.s);
                        for (int i = 0; i <=0; i=i+1)
                        {
                            for (int j = 0; j <= 0; j=j+1)
                            { 
                               // for (int k = 0; k <= 20; k=k+2)
                               // {

                                  //  for (int l = 0; l <= 20; l=l+2)
                                 //   {
                                        //Console.Write("\t G.e \t"+G.e+" \t G.s \t"+G.s+"\t i\t"+i + "\t" + j);
                                        ///////////////////////////////////////////////
                                        x[0] = (double)i / G.res;
                                        y[0] = (double)j / G.res;
                                        //x[1] = (double)k / G.res;
                                        //y[1] = (double)l / G.res;
                                        optimalcoordinates[0] = 0;
                                        optimalcoordinates[1] = 0;
                                        xint[0] = Convert.ToInt32(x[0] * G.res);
                                        yint[0] = Convert.ToInt32(y[0] * G.res);
                                        xint[1] = Convert.ToInt32(x[1] * G.res);
                                        yint[1] = Convert.ToInt32(y[1] * G.res);
                                ///////////////////////////////////////////////
                                ////////////////PREDICTIVE MODEL///////////////

                                //sub.Setup3(xint, yint, Matrix, Payoff, optimalcoordinates, sMatrix, Payoff2, Payoffandsonar);
                                /////////////1stage model/////////////////
                                //sub.Setup1optimalplayer(xint, yint, Matrix, Payoff, optimalcoordinates, sMatrix, Payoff2, file);
                                        ////////////2stage model////////////////////
                                      //  sub.Setup2stages2players(xint, yint, Matrix, Payoff, optimalcoordinates, sMatrix, Payoff2, file);

                                 //   }
                               // }
                                   
                      //      }
                        }
                    }
                }

                /////////////End of CLASSICAL MODEL///////////////
                /////////////////////////////////////////////////
                
                
                int poradi=0;
                for (G.e = 0.50; G.e <= 0.50; G.e = G.e + 0.01) {
                    for (G.s = 0.50; G.s <= 0.50; G.s = G.s + 0.02) {
                            for (int m = 1; m <= 1; m++) {
                            Console.WriteLine("e\t" + G.e + "\t s \t" + G.s + "\t m \t" + m);
                            //////////////RANDOM SONAR MODEL/////////////////
                            int i; int j; int k; int l;
                            i = 90;//rnd.Next(G.res + 1);
                            j = 90;//rnd.Next(G.res + 1);
                            k = 90;//rnd.Next(G.res + 1);
                            l = 90;//rnd.Next(G.res + 1);
                            Console.WriteLine(i + "\t" + j + "\t" + k + "\t" + l);
                            ///////////////////////////////////////////////
                            x[0] = 0.5;//i / G.res;
                            y[0] = 0.5;//j / G.res;
                            x[1] = 0.5;//k / G.res;
                            y[1] = 0.5;//l / G.res;
                            optimalcoordinates[0] = 0;
                            optimalcoordinates[1] = 0;
                            xint[0] = Convert.ToInt32(x[0] * G.res);
                            yint[0] = Convert.ToInt32(y[0] * G.res);
                            xint[1] = Convert.ToInt32(x[1] * G.res);
                            yint[1] = Convert.ToInt32(y[1] * G.res);
                            ///////////////////////////////////////////////
                            ////////////////PREDICTIVE MODEL///////////////

                            Payoffandsonar[poradi, 2] = G.e;
                            Payoffandsonar[poradi, 3] = G.s;
                            Payoffandsonar[poradi, 4] = m;
                            Payoffandsonar[poradi, 5] = poradi;
                            Payoffandsonar[0, 5] = poradi;
                            Payoffandsonar[poradi, 6] = i;
                            Payoffandsonar[poradi, 7] = j;
                            Payoffandsonar[poradi, 8] = k;
                            Payoffandsonar[poradi, 9] = l;

                            sub.Initialize(xint, yint);
                            sub.Generatelineardistribution(Matrix);
                            sub.PlayerPayoff(Payoff, Matrix);  
                            //sub.Setup2Doptimal2players(xint, yint, Matrix, Payoff, optimalcoordinates, sMatrix, Payoff2, Payoffandsonar, file);
                            //sub.Setup3(xint, yint, Matrix, Payoff, optimalcoordinates, sMatrix, Payoff2, Payoffandsonar);
                            poradi++;
                            sub.PlayerPayoff(Payoff, Matrix);
                            Payoffandsonar[poradi, 0] = Payoff[xint[0], yint[0]];
                            Console.WriteLine("Player 1, Payoff:" + Payoff[xint[0], yint[0]]);
                            //Generateplayer1(xint, yint, Matrix, sMatrix, 1);
                            //Deleteplayer2(xint, yint, Matrix, sMatrix, 1);
                            //PlayerPayoff(Payoff, Matrix);
                            //Payoffandsonar[poradi, 1] = Payoff[xint[1], yint[1]];
                            //Console.WriteLine("Player 2, Payoff:" + Payoff[xint[1], yint[1]]);
                        }
                    }                                         
                }
                sub.Depictfile(Payoffandsonar,file);
                /////////////END of 2D////////////////

                */
                Console.ReadKey();
            }
        }
    }
}
