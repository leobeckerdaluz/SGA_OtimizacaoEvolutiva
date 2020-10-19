using System;
using System.Collections.Generic;
using System.Linq;


namespace SGA
{
    public class SGA
    {

        private static Random random = new Random();
        

        public static List<bool> GeraIndividuoBool(int tamanho_genotipo){
            List<bool> array = new List<bool>();
            
            for (int i=0; i<tamanho_genotipo; ++i)
                array.Add( (random.Next(0, 2)==1) ? true : false );

            return array;
        }


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


        private static List<bool> MutacaoBool(List<bool> cromossomo, double probabilidade){
            int len = cromossomo.Count;

            for(int i=0; i<len; i++){
                if (random.NextDouble() < probabilidade){
                    cromossomo[i] = !(cromossomo[i]);
                }
            }

            return cromossomo;
        }


        private static List<List<bool>> SelecaoPais(List<List<bool>> populacao, int tamanho_populacao){
            List<bool> pai1 = populacao[random.Next(0, tamanho_populacao)];
            List<bool> pai2 = populacao[random.Next(0, tamanho_populacao)];

            return new List<List<bool>>(){pai1, pai2};
        }


        private static void print_bool_array(List<bool> boolarray){
            string string_array = "";
            
            foreach(bool i in boolarray)
                string_array += (i ? "1" : "0");
            
            Console.WriteLine(string_array);
        }    
        

        static void Main(string[] args){
            double probabilidade_mutacao = 0.5;
            double probabilidade_crossover = 0.5;
            int tamanho_populacao = 10;
            const int tamanho_genotipo = 10;
            const int criterio_parada_nro_avaliacoes_funcao = 10;

            // Geração da população
            List<List<bool>> populacao = new List<List<bool>>();
            for(int i=0; i<tamanho_populacao; i++){
                populacao.Add(GeraIndividuoBool(tamanho_genotipo));
            }
            Console.WriteLine("População gerada!");
            
            // Apresenta a população
            for(int i=0; i<tamanho_populacao; i++)
                print_bool_array(populacao[i]);
            
            
            // Loop de operações
            int iteracoes = 0;
            while (iteracoes < criterio_parada_nro_avaliacoes_funcao){
                Console.WriteLine("------------------------------------------------------------------------------");
                // Seleciona 2 da população
                List<List<bool>> pais = SelecaoPais(populacao, tamanho_populacao);
                List<bool> pai1 = pais[0];
                List<bool> pai2 = pais[1];


                // Cria os filhos
                List<bool> filho1 = pai1;
                List<bool> filho2 = pai2;


                // Crossover
                if (random.NextDouble() < probabilidade_crossover){
                    List<List<bool>> crossovers = CrossoverBool(pai1, pai2);
                    
                    filho1 = crossovers[0];
                    Console.Write("crossover1: ");
                    print_bool_array(crossovers[0]);
                    Console.Write("filho1: ");
                    print_bool_array(filho1);

                    filho2 = crossovers[1];
                    Console.Write("crossover2: ");
                    print_bool_array(crossovers[1]);
                    Console.Write("filho2: ");
                    print_bool_array(filho2);
                }
                else{
                    Console.WriteLine("Sem Crossover!");
                }

                
                // Mutação
                filho1 = MutacaoBool(filho1, probabilidade_mutacao);
                filho2 = MutacaoBool(filho2, probabilidade_mutacao);
                Console.Write("mutação do filho1: ");
                print_bool_array(filho1);
                Console.Write("mutação do filho2: ");
                print_bool_array(filho2);
        

                // Adiciona
                iteracoes++;
            }
        }
    }
}