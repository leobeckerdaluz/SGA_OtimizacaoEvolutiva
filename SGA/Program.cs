using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SGA
{
    public class SGA
    {
        public class Individuo{  
            public List<bool> genotipo { get; set; }  
            public List<double> fenotipo { get; set; }  
            public double f_x { get; set; }  
            public double freq_relativa { get; set; }  
            
            public Individuo(List<bool> new_genotipo){
                // Seta o cromossomo
                genotipo = new_genotipo;

                // Calcula o fenótipo de cada variável de projeto a partir do genótipo.
                int n_variaveis_projeto = 10;  
                double function_min = -600.0;
                double function_max = 600.0;
                fenotipo = CalculaFenotipos(new_genotipo, n_variaveis_projeto, function_min, function_max);
                
                // Calcula a f(x) a partir do fenótipo de cada variável de projeto
                f_x = funcao_objetivo(fenotipo);
                
                // Inicializa a frequência relativa
                freq_relativa = 0.0;
            }
        }

        private static Random random = new Random();

        private static int iteracoes = 0;

        public static bool DEBUG_CONSOLE = false;
            
        public static List<bool> GeraCromossomo(int tamanho_genotipo){
            List<bool> cromossomo = new List<bool>();
            
            for (int i=0; i<tamanho_genotipo; ++i){
                cromossomo.Add( (random.Next(0, 2)==1) ? true : false );
            }

            return cromossomo;
        }


        private static List<List<bool>> CrossoverBool(List<bool> individuo1, List<bool> individuo2){
            // Os cromossomos possuem tamanhos iguais, então obtém uma
            // ... posição entre 0 e o tamanho de um dos cromossomos.
            int pos = random.Next(0, individuo1.Count);
            if(DEBUG_CONSOLE)
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


        public static List<bool> SelecionaGenotipoPai(List<Individuo> populacao){
            // // Mostra o menor resultado
            // Console.WriteLine("Melhor resultado -> f(x) = " + populacao[0].f_x);

            // Cria a frequência relativa com pesos maiores para os primeiros
            const double minimo_peso_do_rank = 0.0;
            const double maximo_peso_do_rank = 5.0;

            // Calcula o peso de cada indivíduo pelo ranking
            List<double> valor_ranking = new List<double>();
            double somatorio_peso_no_rank = 0.0;
            int N = populacao.Count;
            for(int i=0; i<N; i++){
                double peso_no_rank = (N-i) * (maximo_peso_do_rank - minimo_peso_do_rank) / (N - 1) + minimo_peso_do_rank;
                somatorio_peso_no_rank += peso_no_rank;
                valor_ranking.Add(peso_no_rank);
            }

            // Transforma cada peso do ranking em frequência relativa de 0 a 1
            List<double> freq_relativa = new List<double>();
            for(int i=0; i<N; i++){
                double freq_relativa_i = valor_ranking[i] / somatorio_peso_no_rank;
                freq_relativa.Add(freq_relativa_i);
                if(DEBUG_CONSOLE)
                    Console.WriteLine("freq_relativa_"+i+": " + freq_relativa_i);
            }

            // Calcula o acumulado da frequência relativa
            double somatorio_frequencia_acumulada = 0.0;
            List<double> accumFitness = new List<double>();
            for (int i=0; i<freq_relativa.Count; ++i){
                somatorio_frequencia_acumulada += freq_relativa[i];
                accumFitness.Add(somatorio_frequencia_acumulada);
                if(DEBUG_CONSOLE)
                    Console.WriteLine("accumFitness ORDENADA ACUMULADA: "+i+": " + somatorio_frequencia_acumulada);
            }

            // Gera um valor aleatório para selecionar o pai
            var rand_selecao_pai = random.NextDouble();
            if(DEBUG_CONSOLE)
                Console.WriteLine("Random = " + rand_selecao_pai);
            
            // Seleciona o genótipo pai
            List<bool> pai_selecionado = new List<bool>();
            for (int i=0; i<accumFitness.Count; ++i){
                if (accumFitness[i] > rand_selecao_pai){
                    List<bool> genotipo = populacao[i].genotipo;
                    if(DEBUG_CONSOLE){
                        Console.Write("NOVO PAI! Escolhendo pai "+i+": ");
                        print_bool_array(genotipo);
                    }
                    pai_selecionado = genotipo;
                    break;
                }
            }
            if (pai_selecionado == null){
                // "ERROR: Não encontrou um pai";
                pai_selecionado = populacao[0].genotipo;
                if(DEBUG_CONSOLE)
                    Console.WriteLine("ERROR: Não encontrou um pai!!");
            }
            
            if(DEBUG_CONSOLE){
                Console.Write("RETORNANDO PAI SELECIONADO: ");
                print_bool_array(pai_selecionado);
            }
            return pai_selecionado;


            /*
            List<double> inverso = new List<double>(f_x);
            List<double> freq_relativa = new List<double>(f_x);
            Console.WriteLine("frequencia_relativa size: " + freq_relativa.Count);
            Console.WriteLine("inverso size: " + freq_relativa.Count);

            // Mostra os f(x)
            for (int i=0; i<f_x_array.Length; ++i){
                Console.WriteLine("f(x) " + i + ": " + f_x_array[i]);
            }

            // Calcula o inverso de cada e vai fazendo o somatório
            double somatorio_inversos = 0.0;
            for (int i=0; i<f_x_array.Length; ++i){
                inverso[i] = 1 / f_x_array[i];
                somatorio_inversos += inverso[i];
                Console.WriteLine("inverso " + i + ": " + inverso[i]);
            }
            Console.WriteLine("SOMA TOTAL INVERSO: " + somatorio_inversos);

            // Calcula a frequencia relativa
            for (int i=0; i<inverso.Count; ++i){
                freq_relativa[i] = inverso[i] / somatorio_inversos;
                Console.WriteLine("freq.relativa " + i + ": " + freq_relativa[i]);
            }

            // Ordena com base na frequencia relativa
            var freq_relativa_array = freq_relativa.ToArray();
            Array.Sort(freq_relativa_array, populacao_array);

            // Mostra o menor resultado
            Console.WriteLine("Menor frequencia relativa = " + freq_relativa_array[0]);
            */


            /*
            // fitness proportionate selection.

            var fitArr = fitnesses.ToArray();
            if (sum == 0.0)
            {
                foreach (var fit in fitnesses)
                {
                    sum += fit;
                }
            }

            // normalize.
            for (int i = 0; i < fitArr.Length; ++i)
            {
                fitArr[i] /= sum;
            }

            var popArr = population.ToArray();

            Array.Sort(fitArr, popArr);

            sum = 0.0;

            var accumFitness = new double[fitArr.Length];

            // calculate accumulated normalized fitness values.
            for (int i = 0; i < accumFitness.Length; ++i)
            {
                sum += fitArr[i];
                accumFitness[i] = sum;
            }

            var val = random.NextDouble();

            for (int i = 0; i < accumFitness.Length; ++i)
            {
                if (accumFitness[i] > val)
                {
                    return popArr[i];
                }
            }
            return "";
            */
        }


        private static void print_bool_array(List<bool> boolarray){
            foreach(bool bit in boolarray){
                Console.Write( (bit ? "1" : "0") );
            }
            Console.WriteLine("");
        } 
       

        private static double funcao_objetivo(List<double> fenotipo_variaveis_projeto){
            iteracoes++;
            if(DEBUG_CONSOLE)
                Console.WriteLine("iteracoes: " + iteracoes);

            double laco_somatorio = 0;
            double laco_produto = 1;

            // Laço para os somatórios e pi
            for(int i=0; i<fenotipo_variaveis_projeto.Count; i++){
                laco_somatorio += Math.Pow(fenotipo_variaveis_projeto[i], 2);
                // laco_produto *= Math.Cos( Math.PI * fenotipo_variaveis_projeto[i] / Math.Sqrt(i+1) );
                laco_produto *= Math.Cos( fenotipo_variaveis_projeto[i] / Math.Sqrt(i+1) );
            }

            // Expressão final de f(x)
            double fx = (1 + laco_somatorio/4000.0 - laco_produto);
            // Console.WriteLine("f(x) calculada: " + fx);

            return fx;
        }


        public static List<double> CalculaFenotipos(List<bool> genotipo, int n_variaveis_projeto, double min, double max){
            // Calcula o número de bits por variável de projeto
            int bits_por_variavel_projeto = genotipo.Count / n_variaveis_projeto;

            // Cria a lista que irá conter o fenótipo de cada variável de projeto
            List<double> fenotipo = new List<double>();

            for (int i=0; i<bits_por_variavel_projeto; i++){
                string fenotipo_xi = "";
                
                // Percorre no genótipo o número de bits de cada variável de projeto
                for(int c = n_variaveis_projeto*i; c < n_variaveis_projeto*(i+1); c++){
                    // Se o bit for true, concatena "1", senão, "0"
                    fenotipo_xi += (genotipo[c] ? "1" : "0");
                }

                // Converte os bits da variável de projeto ṕara decimal
                int variavel_convertida = Convert.ToInt32(fenotipo_xi, 2);

                // Mapeia o valor binário entre o intervalo mínimo e máximo
                // 0 --------- min
                // 2^bits ---- max
                // binario --- x
                // (max-min) / (2^bits - 0) ======> Variação de valor por bit
                // min + [(max-min) / (2^bits - 0)] * binario
                double fenotipo_variavel_projeto = min + ((max - min) * variavel_convertida / (Math.Pow(2, n_variaveis_projeto) - 1));

                // Adiciona o fenótipo da variável na lista de fenótipos
                fenotipo.Add(fenotipo_variavel_projeto);
                // Console.WriteLine("fenotipo x"+i+": " + fenotipo_variavel_projeto);
            }

            // Retorna o fenótipo 
            return fenotipo;
        }

    
        public static List<double> Algoritmo_Genetico_Simples(double probabilidade_mutacao, double probabilidade_crossover, int tamanho_populacao, int tamanho_genotipo, int criterio_parada_nro_avaliacoes_funcao){
        
            int nro_geracoes_completas = 0;
            List<Individuo> populacao = new List<Individuo>();
            List<double> melhor_fx_geracoes = new List<double>();

            // Gera a população inicial
            for(int i=0; i<tamanho_populacao; i++){    
                // Gera o individuo e adiciona ele na população
                List<bool> cromossomo = GeraCromossomo(tamanho_genotipo);
                Individuo new_individuo = new Individuo(cromossomo);
                populacao.Add(new_individuo);
            }   
            if(DEBUG_CONSOLE)
                Console.WriteLine("População Inicial gerada!");
            
            // Loop de operações
            while (iteracoes < criterio_parada_nro_avaliacoes_funcao){
                // Atualiza o número total de gerações completas
                nro_geracoes_completas++;
                Console.WriteLine("Geração atual: " + nro_geracoes_completas);
                
                // Ordena a populacao com base no f(x)
                populacao.Sort(delegate(Individuo ind1, Individuo ind2) { return ind1.f_x.CompareTo(ind2.f_x); });

                // Apresenta a população
                if(DEBUG_CONSOLE){
                    foreach(Individuo ind in populacao){
                        Console.Write("Genotipo: ");
                        print_bool_array(ind.genotipo);
                        Console.WriteLine("f(x): " + ind.f_x);
                    }   
                }
                
                // Adiciona o menor f(x) na lista de melhores resultados
                double melhor_fx = populacao[0].f_x;
                melhor_fx_geracoes.Add(melhor_fx);
                
                // Cria a nova geração de filhos
                List<Individuo> new_generation = new List<Individuo>();
                while(new_generation.Count < populacao.Count){
                    // -------------------------------------------------------------
                    // -------------------------- SELEÇÃO --------------------------
                    // -------------------------------------------------------------
                    
                    List<bool> cromossomo_pai1 = SelecionaGenotipoPai(populacao);
                    List<bool> cromossomo_pai2 = SelecionaGenotipoPai(populacao);
                    
                    if(DEBUG_CONSOLE){
                        Console.Write("cromossomo_pai1 criado: ");
                        print_bool_array(cromossomo_pai1);
                        Console.Write("cromossomo_pai2 criado: ");
                        print_bool_array(cromossomo_pai2);
                    }

                    // Cria os 2 filhos cópias dos pais
                    List<bool> cromossomo_filho1 = cromossomo_pai1;
                    List<bool> cromossomo_filho2 = cromossomo_pai2;
                    
                    // -------------------------------------------------------------
                    // ------------------------- CROSSOVER -------------------------
                    // -------------------------------------------------------------
                    
                    // Realiza o crossover conforme a probabilidade
                    if (random.NextDouble() > probabilidade_crossover){
                        List<List<bool>> crossovers = CrossoverBool(cromossomo_pai1, cromossomo_pai2);
                        
                        cromossomo_filho1 = crossovers[0];
                        cromossomo_filho2 = crossovers[1];
                        
                        if(DEBUG_CONSOLE){
                            Console.Write("cromossomo_filho1: ");
                            print_bool_array(cromossomo_filho1);
                            Console.Write("cromossomo_filho2: ");
                            print_bool_array(cromossomo_filho2);
                        }
                    }
                    else{
                        if(DEBUG_CONSOLE)
                            Console.WriteLine("Sem Crossover!");
                    }

                    // -------------------------------------------------------------
                    // -------------------------- MUTAÇÃO --------------------------
                    // -------------------------------------------------------------        
                    
                    cromossomo_filho1 = MutacaoBool(cromossomo_filho1, probabilidade_mutacao);
                    cromossomo_filho2 = MutacaoBool(cromossomo_filho2, probabilidade_mutacao);

                    if(DEBUG_CONSOLE){
                        Console.Write("mutação do cromossomo_filho1: ");
                        print_bool_array(cromossomo_filho1);
                        Console.Write("mutação do cromossomo_filho2: ");
                        print_bool_array(cromossomo_filho2);
                    }

                    // Cria os novos indivíduos e adiciona na população da nova geração
                    Individuo filho1 = new Individuo(cromossomo_filho1);
                    Individuo filho2 = new Individuo(cromossomo_filho2);
                    new_generation.Add(filho1);
                    new_generation.Add(filho2);
                }

                // Atualiza a população
                populacao = new_generation;
                if(DEBUG_CONSOLE)
                    Console.WriteLine("======================================================");
            }

            if(DEBUG_CONSOLE)
                Console.WriteLine("======================================================");
            
            
            // Apresenta os melhores resultados de cada geração
            Console.WriteLine("Melhores Resultados das Gerações:");
            double menor_fx_todos = melhor_fx_geracoes[0];
            for(int i=0; i<melhor_fx_geracoes.Count; i++){
                double best = melhor_fx_geracoes[i];
                
                Console.WriteLine("Melhor na geração "+i+": "+best);
                
                // Se esse melhor foi melhor que todos, atualiza o melhor de todos
                if (best < menor_fx_todos){
                    menor_fx_todos = best;
                }
            }
            Console.WriteLine("Melhor f(x) de todas as gerações: " + menor_fx_todos);

            return melhor_fx_geracoes;
        }

        
        static void Main(string[] args){
            double probabilidade_mutacao = 0.05;
            double probabilidade_crossover = 0.5;
            int tamanho_populacao = 10;
            const int tamanho_genotipo = 60;
            const int criterio_parada_nro_avaliacoes_funcao = 10000; // Múltiplo da população????
            
            DEBUG_CONSOLE = false;
            
            List<double> melhor_fx_geracoes = Algoritmo_Genetico_Simples(probabilidade_mutacao, probabilidade_crossover, tamanho_populacao, tamanho_genotipo, criterio_parada_nro_avaliacoes_funcao);
        }
    }
}