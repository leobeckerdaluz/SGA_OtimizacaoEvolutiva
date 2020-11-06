using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SGA
{
    public class SGA
    {
        // Inicializa as variáveis globais estáticas
        private static Random random = new Random();
        private static bool DEBUG_CONSOLE = false;

        
        /*
        Função para a geração de um cromossomo

        - Parâmetros:
            int: Tamanho do genótipo
        - Retorno:
            List<bool>: Lista contendo todos os bits gerados
        */
        public static List<bool> GeraCromossomo(int tamanho_genotipo){
            List<bool> cromossomo = new List<bool>();
            
            for (int i=0; i<tamanho_genotipo; ++i){
                cromossomo.Add( (random.Next(0, 2)==1) ? true : false );
            }

            return cromossomo;
        }


        /*
        Função para a realização do crossover de um ponto

        - Parâmetros:
            List<bool>: Cromossomo do pai 1
            List<bool>: Cromossomo do pai 2
        - Retorno:
            List<List<bool>>: Lista contendo dois cromossomos filhos (List<bool>)
        */
        private static List<List<bool>> CrossoverBool(List<bool> individuo1, List<bool> individuo2){
            // Os cromossomos possuem tamanhos iguais, então obtém uma
            // ... posição entre 0 e o tamanho de um dos cromossomos.
            int pos = random.Next(0, individuo1.Count);
            if(DEBUG_CONSOLE)   Console.WriteLine("Posição crossover: " + pos);

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


        /*
        Função para a realização da mutação

        - Parâmetros:
            List<bool>: Cromossomo a ser mutado
            double: probabilidade de mutação do cromossomo
        - Retorno:
            List<bool>: Cromossomo mutado
        */
        private static List<bool> MutacaoBool(List<bool> cromossomo, double probabilidade_mutacao){
            for(int i=0; i<cromossomo.Count; i++){
                if (random.NextDouble() < probabilidade_mutacao){
                    cromossomo[i] = !(cromossomo[i]);
                }
            }

            return cromossomo;
        }


        /*
        Função para calcular a frequência acumulada de uma população.
        - A geração é realizada através do método ranking

        - Parâmetros:
            int: tamanho da população
        - Retorno:
            List<double>: Lista contendo a frequência acumulada para cada cromossomo
        */
        public static List<double> CalculaFrequenciaAcumulada(int tamanho_populacao){
            
            // Cria a frequência relativa com pesos maiores para os primeiros
            const double minimo_peso_do_rank = 0.0;
            const double maximo_peso_do_rank = 5.0;

            // Calcula o peso de cada indivíduo pelo ranking
            List<double> valor_ranking = new List<double>();
            double somatorio_peso_no_rank = 0.0;
            for(int i=0; i<tamanho_populacao; i++){
                double peso_no_rank = (tamanho_populacao-i) * (maximo_peso_do_rank - minimo_peso_do_rank) / (tamanho_populacao - 1) + minimo_peso_do_rank;
                somatorio_peso_no_rank += peso_no_rank;
                valor_ranking.Add(peso_no_rank);
            }

            // Transforma cada peso do ranking em frequência relativa de 0 a 1
            List<double> freq_relativa = new List<double>();
            for(int i=0; i<tamanho_populacao; i++){
                double freq_relativa_i = valor_ranking[i] / somatorio_peso_no_rank;
                freq_relativa.Add(freq_relativa_i);
                if(DEBUG_CONSOLE)   Console.WriteLine("freq_relativa_"+i+": " + freq_relativa_i);
            }

            // Calcula o acumulado da frequência relativa
            double somatorio_frequencia_acumulada = 0.0;
            List<double> accumFitness = new List<double>();
            for (int i=0; i<freq_relativa.Count; ++i){
                somatorio_frequencia_acumulada += freq_relativa[i];
                accumFitness.Add(somatorio_frequencia_acumulada);
                if(DEBUG_CONSOLE)   Console.WriteLine("accumFitness ORDENADA ACUMULADA: "+i+": " + somatorio_frequencia_acumulada);
            }

            // Retorna a frequência acumulada
            return accumFitness;
        }



        /*
        Função para selecionar o cromossomo de um pai da população

        - Parâmetros:
            List<Individuo>: Lista contendo toda a população
            double: probabilidade de mutação do cromossomo
        - Retorno:
            List<bool>: Cromossomo mutado
        */
        public static List<bool> SelecionaGenotipoPai(List<List<bool>> populacao, List<double> freq_acumulada){
        
            // Gera um valor aleatório para selecionar o pai
            var rand_selecao_pai = random.NextDouble();
            if(DEBUG_CONSOLE)   Console.WriteLine("Random = " + rand_selecao_pai);
            
            // Seleciona o genótipo pai
            List<bool> pai_selecionado = new List<bool>();
            for (int i=0; i<freq_acumulada.Count; ++i){
                if (freq_acumulada[i] > rand_selecao_pai){
                    List<bool> genotipo_selecionado = populacao[i];
                    if(DEBUG_CONSOLE){
                        Console.Write("NOVO PAI! Escolhendo pai "+i+": ");
                        print_bool_array(genotipo_selecionado);
                    }
                    pai_selecionado = genotipo_selecionado;
                    break;
                }
            }

            // Se o pai selecionado é nulo, significa que deu erro no retorno
            if (pai_selecionado == null){
                // "ERROR: Não encontrou um pai";
                pai_selecionado = populacao[0];
                if(DEBUG_CONSOLE)   Console.WriteLine("ERROR: Não encontrou um pai!!");
            }
            
            if(DEBUG_CONSOLE){
                Console.Write("RETORNANDO PAI SELECIONADO: ");
                print_bool_array(pai_selecionado);
            }

            // Retorna o pai selecionado
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


        /*
        Função para printar um cromossomo - DEBUG
        */
        private static void print_bool_array(List<bool> boolarray){
            foreach(bool bit in boolarray){
                Console.Write( (bit ? "1" : "0") );
            }
            Console.WriteLine("");
        } 
       

        /*
        Função Objetivo - fitness

        - Parâmetros:
            List<bool>: Cromossomo a ser calculado o valor da função objetivo
        - Retorno:
            double: Valor da função objetivo
        */
        private static double funcao_objetivo(List<bool> genotipo){
            // Calcula o fenótipo de cada variável de projeto a partir do genótipo.
            const int n_variaveis_projeto = 10;  
            const double function_min = -600.0;
            const double function_max = 600.0;
            
            // ===================================================
            // Calcula o fenótipo para cada variável de projeto
            // ===================================================

            // Calcula o número de bits por variável de projeto
            int bits_por_variavel_projeto = genotipo.Count / n_variaveis_projeto;

            // Cria a lista que irá conter o fenótipo de cada variável de projeto
            List<double> fenotipo_variaveis_projeto = new List<double>();
            
            // Transforma o genótipo de cada variável em uma string para depois converter para decimal
            for (int i=0; i<bits_por_variavel_projeto; i++){
                string fenotipo_xi = "";
                
                // Percorre o número de bits de cada variável de projeto
                for(int c = n_variaveis_projeto*i; c < n_variaveis_projeto*(i+1); c++){
                    // Se o bit for true, concatena "1", senão, "0"
                    fenotipo_xi += (genotipo[c] ? "1" : "0");
                }

                // Converte essa string de bits para inteiro
                int variavel_convertida = Convert.ToInt32(fenotipo_xi, 2);

                // Mapeia o inteiro entre o intervalo mínimo e máximo
                // 0 --------- min
                // 2^bits ---- max
                // binario --- x
                // (max-min) / (2^bits - 0) ======> Variação de valor por bit
                // min + [(max-min) / (2^bits - 0)] * binario
                double fenotipo_variavel_projeto = function_min + ((function_max - function_min) * variavel_convertida / (Math.Pow(2, n_variaveis_projeto) - 1));

                // Adiciona o fenótipo da variável na lista de fenótipos
                fenotipo_variaveis_projeto.Add(fenotipo_variavel_projeto);
                // Console.WriteLine("fenotipo x"+i+": " + fenotipo_variavel_projeto);
            }

            // ===================================================
            // Calcula a função de Griewank
            // ===================================================
            
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
            
            return fx;
        }


        /*
        Função principal para a execução do Algoritmo Genético Simples

        - Parâmetros:
            double: probabilidade de crossover
            double: probabilidade de mutação
            int: tamanho da população
            int: tamanho do cromossomo
            int: número de avaliações da FO para o critério de parada
        - Retorno:
            List<double> Resultados do melhor fitness em cada NFOB
        */
        public static List<double> Algoritmo_Genetico_Simples(double probabilidade_mutacao, double probabilidade_crossover, int tamanho_populacao, int tamanho_genotipo, int criterio_parada_nro_avaliacoes_funcao){
            // Inicializa a população e alista com os melhores resultados
            List<List<bool>> populacao = new List<List<bool>>();

            double current_best_fx = 999999;
            List<double> melhores_NFOB = new List<double>();
            int NFOB = 0;

            // ========================================
            // Geração da População Inicial
            // ========================================
            
            for(int i=0; i<tamanho_populacao; i++){    
                // Gera o individuo e adiciona ele na população
                List<bool> cromossomo = GeraCromossomo(tamanho_genotipo);
                populacao.Add(cromossomo);
            }   
            if(DEBUG_CONSOLE)   Console.WriteLine("População Inicial gerada!");
            
            // ========================================
            // Gerações até o Critério de Parada
            // ========================================

            // Executa o algoritmos até que o critério de parada seja atingido
            while (NFOB < criterio_parada_nro_avaliacoes_funcao){
                
                // ========================================
                // Calcula o fitness
                // ========================================

                List<double> fx = new List<double>();
                foreach (List<bool> cromossomo in populacao){
                    // Calcula a f(x) a partir do fenótipo de cada variável de projeto
                    double f_x = funcao_objetivo(cromossomo);
                    
                    fx.Add(f_x);
                    
                    // Incrementa o número de avaliações da função objetivo
                    NFOB++;
                    
                    if (f_x < current_best_fx){
                        current_best_fx = f_x;
                    }

                    //List<int> NFOB = new List<int>(){250,500,750,1000,1500,2000,3000,4000,5000,6000,7000,8000,9000,10000,15000,20000,25000,30000,40000,50000,60000,70000,80000,90000,100000}

                    if (NFOB == 250 || NFOB == 500 || NFOB == 750 || NFOB == 1000 || NFOB == 1500 || 
                        NFOB == 2000 || NFOB == 3000 || NFOB == 4000 || NFOB == 5000 || NFOB == 6000 || 
                        NFOB == 7000 || NFOB == 8000 || NFOB == 9000 || NFOB == 10000 || NFOB == 15000 || 
                        NFOB == 20000 || NFOB == 25000 || NFOB == 30000 || NFOB == 40000 || NFOB == 50000 || 
                        NFOB == 60000 || NFOB == 70000 || NFOB == 80000 || NFOB == 90000 || NFOB == 100000)
                    {
                        melhores_NFOB.Add(current_best_fx);
                    }

                    if(DEBUG_CONSOLE)   Console.WriteLine("NFOB: " + NFOB);
                }


                
                // ========================================
                // Ordena a população com base no fitness
                // ========================================

                // Ordena a populacao com base no f(x)
                var populacao_array = populacao.ToArray();
                var fx_array = fx.ToArray();
                
                // Ordena a população com base no f(x)
                Array.Sort(fx_array, populacao_array);

                // Converte novamente os arrays para listas
                populacao = populacao_array.ToList();
                fx = fx_array.ToList();

                // Apresenta a população
                if(DEBUG_CONSOLE){
                    for(int i=0; i<populacao.Count; i++){
                        Console.Write("Genotipo: ");
                        print_bool_array(populacao[i]);
                        Console.WriteLine("f(x): " + fx[i]);
                    }   
                }
                
                // Adiciona o menor f(x) na lista de melhores resultados
                // double melhor_fx = populacao[0].f_x;



                // ========================================
                // Calcula a frequência acumulada
                // ========================================

                // Calcula a frequência acumulada da população
                List<double> freq_acumulada = CalculaFrequenciaAcumulada(populacao.Count);
                
      
      
                // ========================================
                // Cria a nova geração de indivíduos
                // ========================================

                // Cria a nova geração de filhos até que o número seja igual ao da população atual
                List<List<bool>> new_generation = new List<List<bool>>();
                while(new_generation.Count < populacao.Count){
                    // -------------------------------------------------------------
                    // -------------------------- SELEÇÃO --------------------------
                    // -------------------------------------------------------------
                    
                    List<bool> cromossomo_pai1 = SelecionaGenotipoPai(populacao, freq_acumulada);
                    List<bool> cromossomo_pai2 = SelecionaGenotipoPai(populacao, freq_acumulada);
                    
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
                        if(DEBUG_CONSOLE)   Console.WriteLine("Sem Crossover!");
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
                    List<bool> filho1 = new List<bool>(cromossomo_filho1);
                    List<bool> filho2 = new List<bool>(cromossomo_filho2);
                    new_generation.Add(filho1);
                    new_generation.Add(filho2);
                }

                // Atualiza a população
                populacao = new_generation;
                if(DEBUG_CONSOLE)   Console.WriteLine("======================================================");
            }
                        
            // // Apresenta os melhores resultados de cada geração
            // double menor_fx_todos = melhor_fx_geracoes[0];
            // if(DEBUG_CONSOLE)   Console.WriteLine("Melhor f(x) de todas as gerações: " + menor_fx_todos);
            
            // for(int i=0; i<melhor_fx_geracoes.Count; i++){
            //     double best = melhor_fx_geracoes[i];
            //     if(DEBUG_CONSOLE)   Console.WriteLine("Melhor na geração "+i+": "+best);
                
            //     // Se esse melhor foi melhor que todos, atualiza o melhor de todos
            //     if (best < menor_fx_todos){
            //         menor_fx_todos = best;
            //     }
            // }

            // Retorna os melhores a cada NFOB
            return melhores_NFOB;
        }

        

        /*
        Main
        */
        static void Main(string[] args){
            // Define os parâmetros do SGA e de apresentação
            const double probabilidade_mutacao = 0.005;
            const double probabilidade_crossover = 0.2;
            const int tamanho_populacao = 120;
            const int tamanho_genotipo = 140;
            const int criterio_parada_nro_avaliacoes_funcao = 100000;
            DEBUG_CONSOLE = false;
            
            // Inicializa o temporizador
            var total_watch = System.Diagnostics.Stopwatch.StartNew();

            // Executa o SGA e recebe com retorno os NFOB
            List<double> SGA_bests_NFOB = Algoritmo_Genetico_Simples(probabilidade_mutacao, probabilidade_crossover, tamanho_populacao, tamanho_genotipo, criterio_parada_nro_avaliacoes_funcao);

            // Apresenta os resultados
            Console.WriteLine("-----------------");
            for(int j=0; j<SGA_bests_NFOB.Count; j++){
                Console.WriteLine(SGA_bests_NFOB[j]);
            }
            Console.WriteLine("-----------------");

            // Para o temporizados
            total_watch.Stop();

            // Calcula o tempo de execução
            var elapsedMs = total_watch.ElapsedMilliseconds;
            Console.WriteLine("Tempo total de execução: " + elapsedMs/1000.0 + " segundos");




            // List<List<double>> todas_execucoes_SGA_NFOB = new List<List<double>>();

            // // // List<double> crossovers = new List<double>(){0.1, 0.2, 0.4, 0.6, 0.8, 1};
            // // // List<double> populacoes = new List<double>(){10, 20, 40, 80, 120, 160};
            // // List<double> mutacoes = new List<double>(){0.005, 0.01, 0.05, 0.1, 0.3, 0.5};
            // // foreach (double probabilidade_mutacao in mutacoes){
            //     // Console.WriteLine("Probabilidade Crossover: " + probabilidade_crossover);
            //     // Console.WriteLine("Tamanho Populacao: " + populacao);
            //     // Console.WriteLine("Probabilidade Mutação: " + probabilidade_mutacao);
            //     const int numero_execucoes = 50;
            //     for(int i=0; i<numero_execucoes; i++){
            //         // // Começa a contar o tempo de execução
            //         // var watch = System.Diagnostics.Stopwatch.StartNew();
                    
            //         // Executa o SGA
            //         List<double> SGA_bests_NFOB = Algoritmo_Genetico_Simples(probabilidade_mutacao, probabilidade_crossover, tamanho_populacao, tamanho_genotipo, criterio_parada_nro_avaliacoes_funcao);

            //         // Adiciona na lista de execuções a execução atual
            //         todas_execucoes_SGA_NFOB.Add(SGA_bests_NFOB);

                    
            //         // // Encerra o timer
            //         // watch.Stop();

            //         // Apresenta o melhor resultado
            //         // Console.WriteLine("Resultados:");
            //         // foreach (double result in SGA_bests_NFOB){
            //         //     Console.WriteLine(result);

            //         // }
            //         // Console.WriteLine("Execução " + i + ": " + SGA_bests_NFOB[SGA_bests_NFOB.Count - 1]);
            //         // Console.WriteLine("Execução " + i + ": ");
            //         // Console.WriteLine(SGA_bests_NFOB[SGA_bests_NFOB.Count - 1]);
            //         // Console.WriteLine("-----------------");
            //         // for(int j=0; j<SGA_bests_NFOB.Count; j++){
            //         //     Console.WriteLine(SGA_bests_NFOB[j]);
            //         // }
            //         // Console.WriteLine("-----------------");
            //         // Console.WriteLine("Tamanho melhores_NFOB: " + melhores_NFOB.Count);
            //         // Console.WriteLine("Tamanho SGA_bests_NFOB: " + SGA_bests_NFOB.Count);
                    
            //         // Obtém o tempo de execução
            //         // var elapsedMs = watch.ElapsedMilliseconds;
            //         // Console.WriteLine("Tempo de execução: " + elapsedMs/1000.0 + " segundos");
            //     }
            // // }

            // const int NFOBs = 25;
            // for(int NFOB=0; NFOB<NFOBs; NFOB++){
            //     double sum = 0;
            //     foreach(List<double> execution in todas_execucoes_SGA_NFOB){
            //         sum += execution[NFOB];
            //     }
            //     double media = sum / (double)todas_execucoes_SGA_NFOB.Count;
            //     // Console.WriteLine("Soma do NFOB " + NFOB + ": " + sum);
            //     // Console.WriteLine("Média do NFOB " + NFOB + ": " + media);
            //     Console.WriteLine(media);
            // }
        }
    }
}