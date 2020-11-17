using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SGA
{
    public class SGA
    {
        // Inicializa as variáveis globais estáticas
        public static Random random = new Random();
        public static bool DEBUG_CONSOLE = false;

        

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
        public static List<List<bool>> CrossoverBool(List<bool> individuo1, List<bool> individuo2){
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

            // Apresenta os filhos gerados
            if (DEBUG_CONSOLE){
                Console.Write("cromossomo_filho1: ");
                ApresentaCromossomoBool(novo_cromossomo1);
                Console.Write("cromossomo_filho2: ");
                ApresentaCromossomoBool(novo_cromossomo2);
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
        public static List<bool> MutacaoBool(List<bool> cromossomo, double probabilidade_mutacao){
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
            - Essa função recebe como parâmetro a população e a frequência acumulada dela.
            ... Como processamento, ela utiliza o método da roda da roleta pra selecionar
            ... o novo cromossomo pai.
            
        - Parâmetros:
            List<Individuo>: Lista contendo toda a população ordenada pelo fitness
            List<double>: Lista contendo as aptidões acumuladas dessa população ordenada
        - Retorno:
            List<bool>: Cromossomo pai selecionado
        */
        public static List<bool> SelecionaGenotipoPai(List<List<bool>> populacao, List<double> freq_acumulada){  
            // Gera um valor aleatório para selecionar o pai
            var rand_selecao_pai = random.NextDouble();
            if(DEBUG_CONSOLE)   Console.WriteLine("Random para seleção de pai: " + rand_selecao_pai);
            
            // Seleciona o pai através da roda da roleta
            List<bool> pai_selecionado = new List<bool>();
            for (int i=0; i<freq_acumulada.Count; ++i){
                // Se a frequência acumulada é maior que o número gerado, escolhe esse pai
                if (freq_acumulada[i] > rand_selecao_pai){
                    pai_selecionado = populacao[i];

                    if(DEBUG_CONSOLE){
                        Console.Write("NOVO PAI! Escolhendo pai "+i+": ");
                        ApresentaCromossomoBool(pai_selecionado);
                    }
                    
                    // Retorna o pai selecionado
                    return pai_selecionado;
                }
            }

            // Teoricamente, essa parte do código nunca será atingida, pois já retornou antes
            Console.Write("Não encontrou um pai! Retornando o primeiro!");
            return populacao[0];
        }


        
        /*
        Cálculo da aptidão acumulada da população
            - Essa função recebe como parâmetro o tamanho da população e gera a aptidão acumulada
            ... de cada indivíduo. Na hora de aplicar a operação de seleção, essa lista de aptidões
            ... são colocadas lado a lado para a escolha do cromossomo.
            
        - Parâmetros:
            int: tamanho da população
        - Retorno:
            List<double>: Lista contendo a aptidão acumulada para cada cromossomo
        */
        public static List<double> CalculaAptidaoAcumulada(int tamanho_populacao){
            
            // ========================================
            // Calcula a Aptidão
            // ========================================

            // Mínimo e máximo para a criação do ranking
            const double minimo_peso_do_rank = 0.0;
            const double maximo_peso_do_rank = 2.0;
            
            // Armazena o somatório de cada aptidão
            double somatorio_aptidoes = 0.0;
            
            // Lista para armazenar a aptidão de cada indivíduo
            List<double> Aptidoes = new List<double>();
            
            // Calcula a aptidão de cada indivíduo
            for(int i=0; i<tamanho_populacao; i++){
                double aptidao = (tamanho_populacao-i) * (maximo_peso_do_rank - minimo_peso_do_rank) / (tamanho_populacao - 1) + minimo_peso_do_rank;
                
                // Adiciona a aptidão na lista de todas as aptidões
                Aptidoes.Add(aptidao);
                
                // Atualiza o somatório das aptidões
                somatorio_aptidoes += aptidao;
            }

            // ========================================
            // Calcula a Aptidão Relativa
            // ========================================

            // Lista para armazenar a aptidão relativa de cada indivíduo
            List<double> AptidoesRelativas = new List<double>();
            
            // Calcula a aptidão relativa (0.0 a 1.0) de cada indivíduo
            for(int i=0; i<tamanho_populacao; i++){
                double aptidao_relativa = Aptidoes[i] / somatorio_aptidoes;
                
                // Adiciona a aptidão relativa na lista de todas as aptidões relativas
                AptidoesRelativas.Add(aptidao_relativa);

                if(DEBUG_CONSOLE)   Console.WriteLine("AptidoesRelativas_"+i+": " + aptidao_relativa);
            }

            // ========================================
            // Calcula a Aptidão Relativa Acumulada
            // ========================================

            // Lista para armazenar a aptidão relativa acumulada de cada indivíduo
            List<double> AptidaoAcumulada = new List<double>();
            
            // Calcula a aptidão relativa acumulada de cada indivíduo
            double somatio_aptidoes_relativas = 0.0;
            for (int i=0; i<AptidoesRelativas.Count; ++i){
                somatio_aptidoes_relativas += AptidoesRelativas[i];
                
                // Adiciona a aptidão relativa acumulada na lista
                AptidaoAcumulada.Add(somatio_aptidoes_relativas);
                
                if(DEBUG_CONSOLE)   Console.WriteLine("AptidaoAcumulada_"+i+": " + somatio_aptidoes_relativas);
            }

            // Retorna as aptidoes acumuladas
            return AptidaoAcumulada;
        }



        /*
        Função que somente é executada no modo DEBUG. Ela serve para printar um cromossomo na tela
        */
        public static void ApresentaCromossomoBool(List<bool> boolarray){
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
        public static double funcao_objetivo(List<bool> cromossomo, int n_variaveis_projeto, double function_min, double function_max){
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
            int: número de variáveis de projeto
            double: limite inferior da função objetivo
            double: limite superior da função objetivo
            int: número de avaliações da FO para o critério de parada
            List<int>: lista contendo todos os pontos NFOBs desejados
        - Retorno:
            List<double> Resultados do melhor fitness em cada NFOB
        */
        public static List<double> Algoritmo_Genetico_Simples(double probabilidade_mutacao, double probabilidade_crossover, int tamanho_populacao, int tamanho_genotipo, int n_variaveis_projeto, double function_min, double function_max, int criterio_parada_nro_avaliacoes_funcao, List<int> NFOBs){
            // ========================================
            // Inicializa algumas variáveis de controle do algoritmo
            // ========================================
            
            // Armazena o melhor f(x) até o momento
            double current_best_fx = function_max;
            // Lista a ser retornada pelo algoritmo. Ela conterá os
            // ... melhores fitness em cada NFOB desejado
            List<double> melhores_NFOB = new List<double>();
            // Número de avaliações da função objetivo
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
            
            // ========================================
            // Gerações até o Critério de Parada
            // ========================================

            // Executa o algoritmos até que o critério de parada (número de avaliações na FO) seja atingido
            while (NFOB < criterio_parada_nro_avaliacoes_funcao){
                
                // ========================================
                // Calcula o fitness
                // ========================================

                // Para cada cromossomo, calcula o f(x) e armazena
                List<double> fxs = new List<double>();
                foreach (List<bool> cromossomo in populacao){
                    // Calcula a f(x) a partir do fenótipo de cada variável de projeto
                    double fx = funcao_objetivo(cromossomo, n_variaveis_projeto, function_min, function_max);
                    
                    // Adiciona o valor da FO na lista de f(x)'s
                    fxs.Add(fx);
                    
                    // Incrementa o número de avaliações da função objetivo
                    NFOB++;
                    if(DEBUG_CONSOLE)   Console.WriteLine("NFOB atual: " + NFOB);
                    
                    // Se o valor dessa FO for o menor de todos, atualiza o melhor da história
                    if ( fx < current_best_fx ){
                        current_best_fx = fx;
                    }
                        
                    // Se o número atual de avaliações da FO (NFOB) for algum dos NFOBs desejados, 
                    // ... armazena esse melhor resultado da FO até aqui
                    if ( NFOBs.Contains(NFOB) ){
                        melhores_NFOB.Add(current_best_fx);
                    }
                }

                
                // ========================================
                // Ordena a população com base no fitness (f(x))
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
                        ApresentaCromossomoBool(populacao[i]);
                        Console.WriteLine("f(x): " + fxs[i]);
                    }   
                }
               

                // ========================================
                // Calcula as aptidões acumuladas
                // ========================================

                // Calcula as aptidões acumuladas da população
                List<double> AptidoesAcumuladas = CalculaAptidaoAcumulada(populacao.Count);
                
      
                // ========================================
                // Cria a nova geração de indivíduos
                // ========================================

                // Cria a nova geração de filhos até que a quantidade seja igual ao da população atual
                List<List<bool>> nova_geracao = new List<List<bool>>();
                while(nova_geracao.Count < populacao.Count){
                    // ==========================================
                    // Operador de Seleção 
                    // ==========================================
                    
                    // Seleciona os pais através da regra da roleta
                    List<bool> cromossomo_pai1 = SelecionaGenotipoPai(populacao, AptidoesAcumuladas);
                    List<bool> cromossomo_pai2 = SelecionaGenotipoPai(populacao, AptidoesAcumuladas);
                   
                    // Cria os 2 filhos como cópias dos pais
                    List<bool> cromossomo_filho1 = cromossomo_pai1;
                    List<bool> cromossomo_filho2 = cromossomo_pai2;
                    
                    // ==========================================
                    // Operador de Crossover
                    // ==========================================
                    
                    // Realiza o crossover conforme a probabilidade Pc
                    if (random.NextDouble() > probabilidade_crossover){
                        // Se o crossover é realizado, atualiza os filhos com o crossover dos pais
                        List<List<bool>> crossovers = CrossoverBool(cromossomo_pai1, cromossomo_pai2);
                        cromossomo_filho1 = crossovers[0];
                        cromossomo_filho2 = crossovers[1];
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
                        ApresentaCromossomoBool(cromossomo_filho1);
                        Console.Write("mutação do cromossomo_filho2: ");
                        ApresentaCromossomoBool(cromossomo_filho2);
                    }

                    // Adiciona os 2 filhos na população da nova geração
                    nova_geracao.Add(cromossomo_filho1);
                    nova_geracao.Add(cromossomo_filho2);
                }

                // Atualiza a população para a nova geração
                populacao = nova_geracao;
                if(DEBUG_CONSOLE)   Console.WriteLine("======================================================");
            }

            // Retorna os melhores valores fitness a cada NFOB
            return melhores_NFOB;
        }

        

        /*
        Main - Testes Realizados
            - Essa é a função principal da aplicação. Aqui são realizados todos os testes
            para a geração dos resultados. Há 5 blocos de código abaixo. Portanto, para
            executar cada um desses testes é necessário descomentar o bloco de código
            relacionado ao teste. 
            Os testes estão listados abaixo:
                ===> 1 execução do SGA: Pop = 120 | Pc = 0,1 | Pm = 0,005
                ===> 50 execuções do SGA: Pop = VARIANDO | Pc = 0,6 | Pm = 0,01
                ===> 50 execuções do SGA: Pop = 120 | Pc = VARIANDO | Pm = 0,01
                ===> 50 execuções do SGA: Pop = 120 | Pc = 0,1 | Pm = VARIANDO
                ===> 50 execuções do SGA: Pop = 120 | Pc = 0,1 | Pm = 0,005 + Média 

        */
        public static void Main(string[] args){
            // Parâmetros de execução do algoritmo
            const int bits_por_variavel = 14;
            const int n_variaveis_projeto = 10;
            const double function_min = -600.0;
            const double function_max = 600.0;
            const int criterio_parada_nro_avaliacoes_funcao = 100000;
            
            // Definição se o SGA apresenta no console o passo a passo ou se só executa diretamente
            DEBUG_CONSOLE = false;

            // Essa lista contém todos os pontos NFOB onde se deseja saber o valor fitness do SGA. 
            // O SGA executa e retorna uma lista contendo o melhor valor fitness naquele NFOB.
            List<int> NFOBs_desejados = new List<int>(){250,500,750,1000,1500,2000,3000,4000,5000,6000,7000,8000,9000,10000,15000,20000,25000,30000,40000,50000,60000,70000,80000,90000,100000};
            



            
            // ================================================================
            // 1 EXECUÇÃO
            // ================================================================
            
            // Parâmetros de ajuste
            const double probabilidade_mutacao = 0.005;
            const double probabilidade_crossover = 0.1;
            const int tamanho_populacao = 120;
            
            // Inicializa o temporizador
            var total_watch = System.Diagnostics.Stopwatch.StartNew();

            // Executa o SGA e recebe como retorno o valor fitness em cada NFOBs_desejados
            List<double> SGA_bests_NFOB = Algoritmo_Genetico_Simples(probabilidade_mutacao, probabilidade_crossover, tamanho_populacao, bits_por_variavel*n_variaveis_projeto, n_variaveis_projeto, function_min, function_max, criterio_parada_nro_avaliacoes_funcao, NFOBs_desejados);

            // Para o temporizador
            total_watch.Stop();

            // Apresenta os resultados
            Console.WriteLine("-----------------");
            for(int j=0; j<SGA_bests_NFOB.Count; j++){
                Console.WriteLine(NFOBs_desejados[j] + ": " + SGA_bests_NFOB[j]);
            }
            Console.WriteLine("-----------------");

            // Calcula o tempo de execução
            var elapsedMs = total_watch.ElapsedMilliseconds;
            Console.WriteLine("Tempo total de execução: " + elapsedMs/1000.0 + " segundos");
            // ================================================================
            



            /*
            // ================================================================
            // 50 EXECUÇÕES === VARIANDO POPULAÇÃO + CROSSOVER=0,6 + MUTAÇÃO=0,01
            // ================================================================
            // Parâmetros de Ajuste            
            const double probabilidade_crossover = 0.6;
            const double probabilidade_mutacao = 0.01;
            
            // Para cada tamanho da população, executa o algoritmo
            List<int> populacoes = new List<int>(){10, 20, 40, 80, 120, 160};
            foreach (int tamanho_populacao in populacoes){
                Console.WriteLine("===> Probabilidade Crossover: " + probabilidade_crossover);
                Console.WriteLine("===> Tamanho Populacao: " + tamanho_populacao);
                Console.WriteLine("===> Probabilidade Mutação: " + probabilidade_mutacao);
                
                // Inicializa o temporizador
                var total_watch = System.Diagnostics.Stopwatch.StartNew();

                // Executa o SGA por 50 vezes
                for(int i=0; i<50; i++){
                    // Executa o SGA
                    List<double> SGA_bests_NFOB = Algoritmo_Genetico_Simples(probabilidade_mutacao, probabilidade_crossover, tamanho_populacao, bits_por_variavel*n_variaveis_projeto, n_variaveis_projeto, function_min, function_max, criterio_parada_nro_avaliacoes_funcao, NFOBs_desejados);
                    Console.WriteLine("Execução " + i + ": " + SGA_bests_NFOB[SGA_bests_NFOB.Count - 1]);
                }

                // Para o temporizador
                total_watch.Stop();

                // Calcula o tempo de execução
                var elapsedMs = total_watch.ElapsedMilliseconds;
                Console.WriteLine("Tempo total das 50 execuções: " + elapsedMs/1000.0 + " segundos");
                
                Console.WriteLine("=========================================================");
            }
            // ================================================================
            */



            /*
            // ================================================================
            // 50 EXECUÇÕES === POPULAÇÃO=120 + CROSSOVER VARIANDO + MUTAÇÃO=0,01
            // ================================================================
            // Parâmetros de Ajuste            
            const int tamanho_populacao = 120;
            const double probabilidade_mutacao = 0.01;
            
            // Para cada probabilidade de crossover, executa o algoritmo
            List<double> crossovers = new List<double>(){0.1, 0.2, 0.4, 0.6, 0.8, 1};
            foreach (double probabilidade_crossover in crossovers){
                Console.WriteLine("===> Probabilidade Crossover: " + probabilidade_crossover);
                Console.WriteLine("===> Tamanho Populacao: " + tamanho_populacao);
                Console.WriteLine("===> Probabilidade Mutação: " + probabilidade_mutacao);
                
                // Inicializa o temporizador
                var total_watch = System.Diagnostics.Stopwatch.StartNew();

                // Executa o SGA por 50 vezes
                for(int i=0; i<50; i++){
                    // Executa o SGA
                    List<double> SGA_bests_NFOB = Algoritmo_Genetico_Simples(probabilidade_mutacao, probabilidade_crossover, tamanho_populacao, bits_por_variavel*n_variaveis_projeto, n_variaveis_projeto, function_min, function_max, criterio_parada_nro_avaliacoes_funcao, NFOBs_desejados);
                    // Apresenta o melhor resultado
                    Console.WriteLine("Execução " + i + ": " + SGA_bests_NFOB[SGA_bests_NFOB.Count - 1]);
                }

                // Para o temporizador
                total_watch.Stop();

                // Calcula o tempo de execução
                var elapsedMs = total_watch.ElapsedMilliseconds;
                Console.WriteLine("Tempo total das 50 execuções: " + elapsedMs/1000.0 + " segundos");

                Console.WriteLine("=========================================================");
            }
            // ================================================================
            */
           

            /*
            // ================================================================
            // 50 EXECUÇÕES === POPULAÇÃO=120 + CROSSOVER=0,1 + MUTAÇÃO VARIANDO
            // ================================================================
            // Parâmetros de Ajuste            
            const int tamanho_populacao = 120;
            const double probabilidade_crossover = 0.1;

            // Para cada probabilidade de crossover, executa o algoritmo
            List<double> mutacoes = new List<double>(){0.005, 0.01, 0.05, 0.1, 0.3, 0.5};
            foreach (double probabilidade_mutacao in mutacoes){
                Console.WriteLine("===> Probabilidade Crossover: " + probabilidade_crossover);
                Console.WriteLine("===> Tamanho Populacao: " + tamanho_populacao);
                Console.WriteLine("===> Probabilidade Mutação: " + probabilidade_mutacao);
                
                // Inicializa o temporizador
                var total_watch = System.Diagnostics.Stopwatch.StartNew();

                // Executa o SGA por 50 vezes
                for(int i=0; i<50; i++){
                    // Executa o SGA
                    List<double> SGA_bests_NFOB = Algoritmo_Genetico_Simples(probabilidade_mutacao, probabilidade_crossover, tamanho_populacao, bits_por_variavel*n_variaveis_projeto, n_variaveis_projeto, function_min, function_max, criterio_parada_nro_avaliacoes_funcao, NFOBs_desejados);
                    // Apresenta o melhor resultado
                    Console.WriteLine("Execução " + i + ": " + SGA_bests_NFOB[SGA_bests_NFOB.Count - 1]);
                }

                // Para o temporizador
                total_watch.Stop();

                // Calcula o tempo de execução
                var elapsedMs = total_watch.ElapsedMilliseconds;
                Console.WriteLine("Tempo total das 50 execuções: " + elapsedMs/1000.0 + " segundos");

                Console.WriteLine("=========================================================");
            }
            // ================================================================
            */
            

            
            /*
            // ================================================================
            // 50 EXECUÇÕES === POPULAÇÃO=120 + CROSSOVER=0,1 + MUTAÇÃO=0,005
            // ================================================================

            // Cria lista para armazenar os valores fitness a cada NFOB desejado
            List<List<double>> todas_execucoes_SGA_NFOB = new List<List<double>>();
            
            // Parâmetros de Ajuste            
            const double probabilidade_crossover = 0.1;
            const double probabilidade_mutacao = 0.005;
            const int tamanho_populacao = 120;
        
            Console.WriteLine("===> Probabilidade Crossover: " + probabilidade_crossover);
            Console.WriteLine("===> Tamanho Populacao: " + tamanho_populacao);
            Console.WriteLine("===> Probabilidade Mutação: " + probabilidade_mutacao);
            
            // Inicializa o temporizador
            var total_watch = System.Diagnostics.Stopwatch.StartNew();

            // Executa o SGA por 50 vezes
            for(int i=0; i<50; i++){
                // Executa o SGA
                List<double> SGA_bests_NFOB = Algoritmo_Genetico_Simples(probabilidade_mutacao, probabilidade_crossover, tamanho_populacao, bits_por_variavel*n_variaveis_projeto, n_variaveis_projeto, function_min, function_max, criterio_parada_nro_avaliacoes_funcao, NFOBs_desejados);
                // Apresenta o melhor resultado
                Console.WriteLine("Execução " + i + ": " + SGA_bests_NFOB[SGA_bests_NFOB.Count - 1]);
                // Adiciona na lista de execuções a execução atual
                todas_execucoes_SGA_NFOB.Add(SGA_bests_NFOB);
            }

            // Para o temporizador
            total_watch.Stop();

            // Calcula o tempo de execução
            var elapsedMs = total_watch.ElapsedMilliseconds;
            Console.WriteLine("Tempo total das 50 execuções: " + elapsedMs/1000.0 + " segundos");
            
            // Para cada NFOB desejado, calcula a média das N execuções
            Console.WriteLine("===> Médias das 50 execuções para cada NFOB desejado:");
            for(int NFOB=0; NFOB<NFOBs_desejados.Count; NFOB++){
                double sum = 0;
                // Percorre a lista de cada execução para fazer o somatório
                foreach(List<double> execution in todas_execucoes_SGA_NFOB){
                    sum += execution[NFOB];
                }
                double media = sum / (double)todas_execucoes_SGA_NFOB.Count;
                Console.WriteLine("Média do NFOB " + NFOB + ": " + media);
            }
            Console.WriteLine("=========================================================");
            // ================================================================
            */
        }
    }
}