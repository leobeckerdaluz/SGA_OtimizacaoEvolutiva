using System;
// using System.Collections.Generic;
// using System.Linq;


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

        static void Main(string[] args){
            Console.WriteLine("Geração Individuo: ");
            
            string ind = GeraIndividuo(10);
            Console.WriteLine(ind);

        }
    }
}