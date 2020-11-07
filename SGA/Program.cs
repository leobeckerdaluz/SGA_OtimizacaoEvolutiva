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
        Geração de Cromossomo
            - Essa função recebe como parâmetro o tamanho do cromossomo e gera 
            ... uma lista de booleanos representando o cromossomo.

        - Parâmetros:
            int: Tamanho do cromossomo
        - Retorno:
            List<bool>: Lista contendo todos os bits gerados
        */
        public static List<bool> GeraCromossomo(int tamanho_cromossomo){
            // Inicializa o cromossomo como uma lista
            List<bool> cromossomo = new List<bool>();
            
            // Gera mu bit para cada posição do cromossomo
            for (int i=0; i<tamanho_cromossomo; ++i){
                cromossomo.Add( (random.Next(0, 2)==1) ? true : false );
            }

            // Retorna o cromossomo criado
            return cromossomo;
        }



        /*
        Operação de Crossover
            - Essa função recebe como parâmetro os dois pais e realiza
            ... o crossover de um ponto, gerando dois filhos.

        - Parâmetros:
            List<bool>: Cromossomo do pai 1
            List<bool>: Cromossomo do pai 2
        - Retorno:
            List<List<bool>>: Lista contendo os dois cromossomos filhos gerados
        */
        private static List<List<bool>> CrossoverBool(List<bool> individuo1, List<bool> individuo2){
            // Os cromossomos possuem tamanhos iguais, então obtém uma
            // ... posição entre 0 e o tamanho de um dos cromossomos.
            int posicao_crossover = random.Next(0, individuo1.Count);
            if(DEBUG_CONSOLE)   Console.WriteLine("Posição crossover: " + posicao_crossover);

            // Cria as recombinações dos 2 novos indivíduos            
            List<bool> novo_cromossomo1 = new List<bool>();
            List<bool> novo_cromossomo2 = new List<bool>();

            // Cria os filhos com partes dos pais
            for(int i=0; i<individuo1.Count; i++){
                if(i <= posicao_crossover){
                    novo_cromossomo1.Add(individuo1[i]);
                    novo_cromossomo2.Add(individuo2[i]);
                }
                else{
                    novo_cromossomo1.Add(individuo2[i]);
                    novo_cromossomo2.Add(individuo1[i]);
                }
            }

            // Retorna os dois novos indivíduos
            return new List<List<bool>>(){novo_cromossomo1, novo_cromossomo2};
        }



        /*
        Operacao de Mutação
            - Essa função recebe como parâmetro o cromossomo e a probabilidade de mutação
            ... e realiza a mutação de um bit com determinada probabilidade.

        - Parâmetros:
            List<bool>: Cromossomo a ser mutado
            double: probabilidade de mutação do cromossomo
        - Retorno:
            List<bool>: Cromossomo mutado
        */
        private static List<bool> MutacaoBool(List<bool> cromossomo, double probabilidade_mutacao){
            // Percorre o cromossomo e muta cada bit com determinada probabilidade
            for(int i=0; i<cromossomo.Count; i++){
                if (random.NextDouble() < probabilidade_mutacao){
                    cromossomo[i] = !(cromossomo[i]);
                }
            }

            // Retorna o cromossomo mutado
            return cromossomo;
        }



        /*
        Operação de Seleção de pai
            - Essa função recebe como parâmetro a população e a frequência acumulada dela
            ... e seleciona com certa probabilidade um cromossomo.
            
        - Parâmetros:
            List<Individuo>: Lista contendo toda a população
            double: probabilidade de mutação do cromossomo
        - Retorno:
            List<bool>: Cromossomo mutado
        */
        public static List<bool> SelecionaGenotipoPai(List<List<bool>> populacao, List<double> freq_acumulada){  
            // Gera um valor aleatório para selecionar o pai
            var rand_selecao_pai = random.NextDouble();
            if(DEBUG_CONSOLE)   Console.WriteLine("Random para seleção de pai: " + rand_selecao_pai);
            
            // Seleciona o genótipo pai
            List<bool> pai_selecionado = new List<bool>();
            for (int i=0; i<freq_acumulada.Count; ++i){
                if (freq_acumulada[i] > rand_selecao_pai){
                    pai_selecionado = populacao[i];

                    if(DEBUG_CONSOLE){
                        Console.Write("NOVO PAI! Escolhendo pai "+i+": ");
                        print_bool_array(pai_selecionado);
                    }
                    
                    // Retorna o pai selecionado
                    return pai_selecionado;
                }
            }

            // Teoricamente, essa parte do código nunca será atingida, pois já retornou antes
            // "ERROR: Não encontrou um pai";
            Console.Write("Não encontrou um pai! Retornando o primeiro!");
            return populacao[0];
        }


        
        /*
        Cálculo da Frequência acumulada da população
            - Essa função recebe como parâmetro o tamanho da população e gera a frequência acumulada
            ... de cada indivíduo. Essa frequência é utilizada na hora da seleção de um pai.
            
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
        Função para printar um cromossomo - DEBUG
        */
        private static void print_bool_array(List<bool> boolarray){
            foreach(bool bit in boolarray){
                Console.Write( (bit ? "1" : "0") );
            }
            Console.WriteLine("");
        } 
       


        /*
        Função Objetivo 
            - Essa função é a função fitness do algoritmo. Ela recebe como parâmetro o 
            ... cromossomo, calcula o fenótipo de cada variável de projeto e calcula o 
            ... valor da função objetivo.

        - Parâmetros:
            List<bool>: Cromossomo a ser calculado o valor da função objetivo
        - Retorno:
            double: Valor da função objetivo
        */
        private static double funcao_objetivo(List<bool> cromossomo){
            // Calcula o fenótipo de cada variável de projeto a partir do genótipo.
            const int n_variaveis_projeto = 10;  
            const double function_min = -600.0;
            const double function_max = 600.0;
            
            // ===================================================
            // Calcula o fenótipo para cada variável de projeto
            // ===================================================

            // Calcula o número de bits por variável de projeto
            int bits_por_variavel_projeto = cromossomo.Count / n_variaveis_projeto;

            // Cria a lista que irá conter o fenótipo de cada variável de projeto
            List<double> fenotipo_variaveis_projeto = new List<double>();
            
            // Transforma o genótipo de cada variável em uma string para depois converter para decimal
            for (int i=0; i<bits_por_variavel_projeto; i++){
                // Cria string representando os bits da variável
                string fenotipo_xi = "";
                
                // Percorre o número de bits de cada variável de projeto
                for(int c = n_variaveis_projeto*i; c < n_variaveis_projeto*(i+1); c++){
                    // Se o bit for true, concatena "1", senão, "0"
                    fenotipo_xi += (cromossomo[c] ? "1" : "0");
                }

                // Converte essa string de bits para inteiro
                int variavel_convertida = Convert.ToInt32(fenotipo_xi, 2);

                // Mapeia o inteiro entre o intervalo mínimo e máximo da função
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
        Função Principal - Algoritmo Genético Simples
            - Essa função é a função principal para a execução do algoritmo. Aqui está contida grande 
            ... parte da lógica do algoritmo. Ela recebe como parâmetro as probabilidades, bem como os 
            ... tamanhos de população, cromossomos e o critério de parada (número de avaliações da FO).
            O algoritmo irá armazenar o valor da FO para cada NFOB desejado e retornará isso ao final.

        - Parâmetros:
            double: probabilidade de crossover
            double: probabilidade de mutação
            int: tamanho da população
            int: tamanho do cromossomo
            int: número de avaliações da FO para o critério de parada
            List<int>: lista contendo todos os pontos NFOBs desejados
        - Retorno:
            List<double> Resultados do melhor fitness em cada NFOB
        */
        public static List<double> Algoritmo_Genetico_Simples(double probabilidade_mutacao, double probabilidade_crossover, int tamanho_populacao, int tamanho_genotipo, int criterio_parada_nro_avaliacoes_funcao, List<int> NFOBs){
            // Inicializa algumas variáveis de controle do algoritmo
            double current_best_fx = 999999;
            List<double> melhores_NFOB = new List<double>();
            int NFOB = 0;

            // ========================================
            // Geração da População Inicial
            // ========================================
            
            List<List<bool>> populacao = new List<List<bool>>();
            for(int i=0; i<tamanho_populacao; i++){    
                // Gera um cromossomo e adiciona ele na população
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

                List<double> fxs = new List<double>();
                foreach (List<bool> cromossomo in populacao){
                    // Calcula a f(x) a partir do fenótipo de cada variável de projeto
                    double fx = funcao_objetivo(cromossomo);
                    
                    // Adiciona o valor da FO na lista de resultados da FO
                    fxs.Add(fx);
                    
                    // Incrementa o número de avaliações da função objetivo
                    NFOB++;
                    if(DEBUG_CONSOLE)   Console.WriteLine("NFOB: " + NFOB);
                    
                    // Se o valor da FO for o menor de todos, armazena esse melhor
                    current_best_fx = (fx < current_best_fx) ? fx : current_best_fx;
                        
                    // Se o número de avaliações da FO for algum dos NFOBs desejados, armazena o 
                    // ... melhor resultado da FO até aqui
                    if ( NFOBs.Contains(NFOB) ){
                        melhores_NFOB.Add(current_best_fx);
                    }
                }

                
                // ========================================
                // Ordena a população com base no fitness
                // ========================================

                // Transforma a população e os fitness para array para poder ordenar
                var populacao_array = populacao.ToArray();
                var fxs_array = fxs.ToArray();
                
                // Ordena a população com base no fitness
                Array.Sort(fxs_array, populacao_array);

                // Converte novamente os arrays para listas
                populacao = populacao_array.ToList();
                fxs = fxs_array.ToList();

                // Apresenta a população ordenada
                if(DEBUG_CONSOLE){
                    for(int i=0; i<populacao.Count; i++){
                        Console.Write("Cromossomo: ");
                        print_bool_array(populacao[i]);
                        Console.WriteLine("f(x): " + fxs[i]);
                    }   
                }
               

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
                    // ==========================================
                    // Operador de Seleção 
                    // ==========================================
                    
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
                    
                    // ==========================================
                    // Operador de Crossover
                    // ==========================================
                    
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

                    // ==========================================
                    // Operador de Mutação
                    // ==========================================
                    
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

            // Retorna os melhores valores fitness a cada NFOB
            return melhores_NFOB;
        }

        

        /*
        Main
        */
        static void Main(string[] args){
            // Parâmetros de ajuste
            const double probabilidade_mutacao = 0.005;
            const double probabilidade_crossover = 0.2;
            const int tamanho_populacao = 120;
            
            // Parâmetros de execução
            const int tamanho_genotipo = 140;
            const int criterio_parada_nro_avaliacoes_funcao = 100000;
            DEBUG_CONSOLE = false;
            List<int> NFOBs = new List<int>(){250,500,750,1000,1500,2000,3000,4000,5000,6000,7000,8000,9000,10000,15000,20000,25000,30000,40000,50000,60000,70000,80000,90000,100000};
            
            // Inicializa o temporizador
            var total_watch = System.Diagnostics.Stopwatch.StartNew();

            // Executa o SGA e recebe com retorno os NFOB
            List<double> SGA_bests_NFOB = Algoritmo_Genetico_Simples(probabilidade_mutacao, probabilidade_crossover, tamanho_populacao, tamanho_genotipo, criterio_parada_nro_avaliacoes_funcao, NFOBs);

            // Apresenta os resultados
            Console.WriteLine("-----------------");
            for(int j=0; j<SGA_bests_NFOB.Count; j++){
                Console.WriteLine(NFOBs[j] + ": " + SGA_bests_NFOB[j]);
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