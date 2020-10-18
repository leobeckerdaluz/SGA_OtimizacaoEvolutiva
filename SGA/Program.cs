using System;
using System.Collections.Generic;
using System.Linq;


namespace SGA
{
    public class SGA
    {
        private static Random random = new Random();
        
        public static string GeraIndividuo(int tamanho_genotipo){
            string individuo = "";
            
            for (int i=0; i<tamanho_genotipo; ++i)
                individuo += ((random.Next(0, 2) == 1) ? "1" : "0");
            
            return individuo;
        }

        private static List<string> Crossover(string individuo1, string individuo2){
            // Os cromossomos possuem tamanhos iguais, então obtém uma
            // ... posição entre 0 e o tamanho de um dos cromossomos.
            int pos = random.Next(0, individuo1.Length);

            // Cria as recombinações dos 2 novos indivíduos            
            string crossover1 = individuo1.Substring(pos) + individuo2.Substring(0, pos);
            string crossover2 = individuo2.Substring(pos) + individuo1.Substring(0, pos);
            
            // Retorna os dois novos indivíduos
            return new List<string>{ crossover1, crossover2 };
        }

        static void Main(string[] args){
            Console.WriteLine("Geração Individuo: ");
            
            string ind1 = GeraIndividuo(10);
            Console.WriteLine(ind1);

            string ind2 = GeraIndividuo(10);
            Console.WriteLine(ind2);

            List<string> teste = Crossover(ind1, ind2).ToList();
            Console.WriteLine(teste[0]);
            Console.WriteLine(teste[1]);

        }
    }
}