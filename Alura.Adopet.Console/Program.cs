﻿using System.Net.Http.Headers;
using System.Net.Http.Json;
using Alura.Adopet.Console;

// cria instância de HttpClient para consumir API Adopet
Console.ForegroundColor = ConsoleColor.Green;

HttpClient client = ConfiguraHttpClient("https://localhost:5001/");

try
{
    
    string comando = args[0].Trim();
    switch (comando)
    {
        case "import":

            var import = new Import();
			await import.ImportacaoArquivoPetAsync(caminhoDoArquivoDeImportacao: args[1]);

            break;
        case "help":
            Console.WriteLine("Lista de comandos.");
            // se não passou mais nenhum argumento mostra help de todos os comandos
            if (args.Length == 1)
            {
                Console.WriteLine("adopet help <parametro> ous simplemente adopet help  " +
                     "comando que exibe informações de ajuda dos comandos.");
                Console.WriteLine("Adopet (1.0) - Aplicativo de linha de comando (CLI).");
                Console.WriteLine("Realiza a importação em lote de um arquivos de pets.");
                Console.WriteLine("Comando possíveis: ");
                Console.WriteLine($" adopet import <arquivo> comando que realiza a importação do arquivo de pets.");
                Console.WriteLine($" adopet show   <arquivo> comando que exibe no terminal o conteúdo do arquivo importado." + "\n\n\n\n");
                Console.WriteLine("Execute 'adopet.exe help [comando]' para obter mais informações sobre um comando." + "\n\n\n");
            }
            // exibe o help daquele comando específico
            else if (args.Length == 2)
            {
                string comandoASerExebido = args[1];
                if (comandoASerExebido.Equals("import"))
                {
                    Console.WriteLine(" adopet import <arquivo> " +
                        "comando que realiza a importação do arquivo de pets.");
                }
                if (comandoASerExebido.Equals("show"))
                {
                    Console.WriteLine(" adopet show <arquivo>  comando que " +
                        "exibe no terminal o conteúdo do arquivo importado.");
                }
            }
            break;
        case "show":
            string caminhoDoArquivoASerExibido = args[1];
            using (StreamReader sr = new StreamReader(caminhoDoArquivoASerExibido))
            {
                Console.WriteLine("----- Serão importados os dados abaixo -----");
                while (!sr.EndOfStream)
                {
                    // separa linha usando ponto e vírgula
                    string[] propriedades = sr.ReadLine().Split(';');
                    // cria objeto Pet a partir da separação
                    Pet pet = new Pet(Guid.Parse(propriedades[0]),
                    propriedades[1],
                    TipoPet.Cachorro
                    );
                    Console.WriteLine(pet);
                }
            }
            break;
        case "list":
            var pets = await ListPetsAsync();
            foreach(var pet in pets)
            {
                Console.WriteLine(pet);
            }
            break;
        default:
            // exibe mensagem de comando inválido
            Console.WriteLine("Comando inválido!");
            break;
    }
}
catch (Exception ex)
{
    // mostra a exceção em vermelho
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Aconteceu um exceção: {ex.Message}");
}
finally
{
    Console.ForegroundColor = ConsoleColor.White;
}


HttpClient ConfiguraHttpClient(string url)
{
	HttpClient _client = new HttpClient();
	_client.DefaultRequestHeaders.Accept.Clear();
	_client.DefaultRequestHeaders.Accept.Add(
		new MediaTypeWithQualityHeaderValue("application/json"));
	_client.BaseAddress = new Uri(url);
	return _client;
}


async Task<IEnumerable<Pet>?> ListPetsAsync()
{
    HttpResponseMessage response = await client.GetAsync("pet/list");
    return await response.Content.ReadFromJsonAsync<IEnumerable<Pet>>();
}