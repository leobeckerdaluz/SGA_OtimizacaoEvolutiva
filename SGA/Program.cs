using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;


namespace SGA
{
    public class SGA
    {

        private static Random random = new Random();

        private static int iteracoes = 0;
            
        public static List<bool> GeraIndividuoBool(int tamanho_genotipo){
            List<bool> array = new List<bool>();
            
            for (int i=0; i<tamanho_genotipo; ++i){
                array.Add( (random.Next(0, 2)==1) ? true : false );
            }

            return array;
        }


        private static List<List<bool>> CrossoverBool(List<bool> individuo1, List<bool> individuo2){
            // Os cromossomos possuem tamanhos iguais, então obtém uma
            // ... posição entre 0 e o tamanho de um dos cromossomos.
            int pos = random.Next(0, individuo1.Count);
            Console.WriteLine("Posição crossover: " + pos);

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


        public static List<bool> SelecionaPai(List<List<bool>> populacao, List<double> fitnesses, double sum){

            Console.WriteLine("================ SELECIONA PAI =======================");
            List<double> local_fitness = new List<double>(fitnesses);
            Console.WriteLine("local_fitness size: " + local_fitness.Count);

            // Calcula a frequência relativa
            for (int i = 0; i < local_fitness.Count; ++i){
                Console.WriteLine("before normalize: " + i + ": " + local_fitness[i]);
                
                // Maior valor em módulo tem mais peso
                local_fitness[i] = Math.Abs(local_fitness[i]) / sum;
            }

            // Mostra a frequencia relativa
            for (int i = 0; i < local_fitness.Count; ++i){
                Console.WriteLine("freq.relativa " + i + ": " + local_fitness[i]);
            }

            // Ordena os arrays
            var fitnesses_array = local_fitness.ToArray();
            var populacao_array = populacao.ToArray();
            Array.Sort(fitnesses_array, populacao_array);

            // Mostra o melhor resultado
            Console.WriteLine("Melhor resultado -> f(x) = " + fitnesses_array[fitnesses_array.Length - 1]);

            // Calcula o acumulado dos valores fitness normalizados
            sum = 0.0;
            List<double> accumFitness = new List<double>();
            for (int i=0; i<fitnesses_array.Length; ++i){
                // Console.WriteLine("fitness array: " + i + ": " + fitnesses_array[i]);
                sum += fitnesses_array[i]);
                accumFitness.Add(sum);
                Console.WriteLine("accumFitness: " + i + ": " + sum);
            }
            
            // Gera um valor aleatório para selecionar o pai
            var rand_selecao_pai = random.NextDouble();
            Console.WriteLine("Random = " + rand_selecao_pai);
            
            // Seleciona o pai para retornar
            // for (int i=0; i<accumFitness.Count; ++i){
            //     Console.WriteLine(i + ": " + accumFitness[i]);
            // }
            for (int i=0; i<accumFitness.Count; ++i){
                if (accumFitness[i] > rand_selecao_pai){
                    List<bool> ret = (List<bool>) populacao_array[i];
                    Console.WriteLine("NOVO PAI! Escolhendo pai: " + i);
                    print_bool_array(populacao[i]);
                    // return ret;
                    return populacao_array[i];
                }
            }
            
            // "ERROR: Não encontrou um pai";
            Console.WriteLine("DEU PZUASSSSO!!!!!!!!!!!!!!!!!!! ");
            return new List<bool>();
        }


        private static void print_bool_array(List<bool> boolarray){
            string string_array = "";
            
            foreach(bool i in boolarray)
                string_array += (i ? "1" : "0");
            
            Console.WriteLine(string_array);
        }    
        

        private static double objective_function(double x){
            double fitness = -Math.Pow(x,2) + 2;

            iteracoes++;
            Console.WriteLine("iteracoes: " + iteracoes);
            
            // n = size(x, 2);
            
            // double sumcomp = 0;
            // double prodcomp = 1;
            
            // for(int i=1; i<n; i++){
            //     sumcomp += (x(:, i) .^ 2);
            //     prodcomp *= (cos(x(:, i) / sqrt(i)));
            // }
            
            // scores = (sumcomp / 4000) - prodcomp + 1;
            return fitness;
        }


        private static double listbool_to_double(List<bool> array){
            double number = 0;

            for(int i=0; i<array.Count; i++){
                if (array[i]){
                    number += (double)Math.Pow(2, (array.Count-1-i));
                }
            }

            return number;
        }


        static void Main(string[] args){
            double probabilidade_mutacao = 0.5;
            double probabilidade_crossover = 0.5;
            int tamanho_populacao = 10;
            const int tamanho_genotipo = 10;
            const int criterio_parada_nro_avaliacoes_funcao = 11;
            int nro_geracoes_completas = 0;


            // Geração da população
            List<List<bool>> populacao = new List<List<bool>>();
            for(int i=0; i<tamanho_populacao; i++){
                populacao.Add(GeraIndividuoBool(tamanho_genotipo));
            }
            

            // Loop de operações
            while (iteracoes < criterio_parada_nro_avaliacoes_funcao){
                List<List<bool>> new_generation = new List<List<bool>>();

                Console.WriteLine("----------------------------- NEW GENERATION ---------------------------------------");
                nro_geracoes_completas++;
            
                // Apresenta a população
                for(int i=0; i<tamanho_populacao; i++){
                    Console.Write(i + ": ");
                    print_bool_array(populacao[i]);
                }
                Console.WriteLine("População gerada!");
                
                // Calcula o fitness (valor da funcao)
                double sum = 0.0;
                List<double> fitnesses = new List<double>();
                for (int i=0; i<populacao.Count; i++){
                    double x = listbool_to_double(populacao[i]);
                    double value = objective_function(x);
                    Console.WriteLine("x = " + x + "  |  f(x) = " + value);
                    fitnesses.Add(value);
                    sum += value;
                }
                Console.WriteLine("SOMA = " + sum);

                // Enquanto a quantidade de filhos gerada for menor que n, gera filhos
                while(new_generation.Count < populacao.Count){
                    // Seleciona 2 da população
                    List<bool> pai1 = SelecionaPai(populacao, fitnesses, sum);
                    List<bool> pai2 = SelecionaPai(populacao, fitnesses, sum);
                    Console.Write("Pai1 criado: ");
                    print_bool_array(pai1);
                    Console.Write("Pai2 criado: ");
                    print_bool_array(pai2);

                    // Gera osCria os filhos
                    List<bool> new1 = pai1;
                    List<bool> new2 = pai2;
                    
                    // Crossover
                    if (random.NextDouble() < probabilidade_crossover){
                        List<List<bool>> crossovers = CrossoverBool(pai1, pai2);
                        
                        new1 = crossovers[0];
                        Console.Write("new1: ");
                        print_bool_array(new1);

                        new2 = crossovers[1];
                        Console.Write("new2: ");
                        print_bool_array(new2);
                    }
                    else{
                        Console.WriteLine("Sem Crossover!");
                    }

                    // Mutação
                    new1 = MutacaoBool(new1, probabilidade_mutacao);
                    Console.Write("mutação do new1: ");
                    print_bool_array(new1);
                    
                    new2 = MutacaoBool(new2, probabilidade_mutacao);
                    Console.Write("mutação do new2: ");
                    print_bool_array(new2);

                    new_generation.Add(new1);
                    new_generation.Add(new2);
                }


                // Substitui a população pela nova
                populacao = new_generation;
            }

            Console.WriteLine("======================================================");
            Console.WriteLine("======================================================");
            Console.WriteLine("======================================================");
            // Acabou as gerações, então vê a melhor resposta
            // List<bool> melhor_resposta = choose_best_chromosome(populacao);

            Console.WriteLine("Número de gerações completas: " + nro_geracoes_completas);
        }
    }
}