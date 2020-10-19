using System;
using System.Collections.Generic;
using System.Linq;


namespace SGA
{
    public class SGA
    {
        private static Random random = new Random();
        
        // public static string GeraIndividuoString(int tamanho_genotipo){
        //     string individuo = "";
            
        //     for (int i=0; i<tamanho_genotipo; ++i)
        //         individuo += ((random.Next(0, 2) == 1) ? "1" : "0");
            
        //     return individuo;
        // }

        public static List<bool> GeraIndividuoBool(int tamanho_genotipo){
            List<bool> array = new List<bool>();
            
            for (int i=0; i<tamanho_genotipo; ++i)
                array.Add( (random.Next(0, 2)==1) ? true : false );

            return array;
        }

        // private static List<string> Crossover(string individuo1, string individuo2){
        //     // Os cromossomos possuem tamanhos iguais, então obtém uma
        //     // ... posição entre 0 e o tamanho de um dos cromossomos.
        //     int pos = random.Next(0, individuo1.Length);

        //     // Cria as recombinações dos 2 novos indivíduos            
        //     string crossover1 = individuo1.Substring(pos) + individuo2.Substring(0, pos);
        //     string crossover2 = individuo2.Substring(pos) + individuo1.Substring(0, pos);
            
        //     // Retorna os dois novos indivíduos
        //     return new List<string>{ crossover1, crossover2 };
        // }

        private static List<List<bool>> CrossoverBool(List<bool> individuo1, List<bool> individuo2){
            // Os cromossomos possuem tamanhos iguais, então obtém uma
            // ... posição entre 0 e o tamanho de um dos cromossomos.
            int pos = random.Next(0, individuo1.Count);

            // Cria as recombinações dos 2 novos indivíduos            
            List<bool> crossover1 = new List<bool>();
            List<bool> crossover2 = new List<bool>();
            for(int i=0; i<individuo1.Count; i++){
                if(i <= pos){
                    crossover1.Add(individuo1[i]);
                    crossover2.Add(individuo2[i]);
                }
                else{
                    crossover1.Add(individuo2[i]);
                    crossover2.Add(individuo1[i]);
                }
            }

            // Retorna os dois novos indivíduos
            return new List<List<bool>>(){crossover1, crossover2};
        }

        private static List<bool> MutacaoBool(List<bool> cromossomo, double probabilidade)
        {
            int len = cromossomo.Count;

            for(int i=0; i<len; i++){
                if (random.NextDouble() < probabilidade){
                    cromossomo[i] = !(cromossomo[i]);
                }
            }

            return cromossomo;
        }

        private static void print_bool_array(List<bool> boolarray){
            string string_array = "";
            
            foreach(bool i in boolarray)
                string_array += (i ? "1" : "0");
            
            Console.WriteLine(string_array);
        }    
        

        static void Main(string[] args){
            double probabilidade_mutacao = 0.5;

            Console.WriteLine("Geração Individuo: ");
            
            List<bool> ind1 = GeraIndividuoBool(10);
            List<bool> ind2 = GeraIndividuoBool(10);
            Console.Write("ind1: ");
            print_bool_array(ind1);
            Console.Write("ind2: ");
            print_bool_array(ind2);
            
            // string ind2 = GeraIndividuo(10);
            // Console.WriteLine(ind2);

            // List<string> teste = Crossover(ind1, ind2).ToList();
            // Console.WriteLine(teste[0]);
            // Console.WriteLine(teste[1]);

            List<List<bool>> crossovers = CrossoverBool(ind1, ind2);
            List<bool> crossover1 = crossovers[0];
            List<bool> crossover2 = crossovers[1];
            Console.Write("crossover1: ");
            print_bool_array(crossover1);
            Console.Write("crossover2: ");
            print_bool_array(crossover2);

            List<bool> mutado1 = MutacaoBool(ind1, probabilidade_mutacao);
            List<bool> mutado2 = MutacaoBool(ind2, probabilidade_mutacao);
            Console.Write("mutação do ind1: ");
            print_bool_array(mutado1);
            Console.Write("mutação do ind2: ");
            print_bool_array(mutado2);
            // string mutado1 = Mutacao(ind1, 1);
            // string mutado2 = Mutacao(ind2, 1);
            // Console.WriteLine(mutado1);
            // Console.WriteLine(mutado2);


            // List<bool> ind = new List<bool> {false, true, true, true, false, false, true};
            // Console.WriteLine(ind);

            // string array = boolarray_to_string(ind);
            // Console.WriteLine(array);
            
            // string result = string.Join("", arr1);
            // Console.WriteLine(result);

        }
    }
}