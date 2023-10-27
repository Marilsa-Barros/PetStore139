// 1 - Bibliotecas
using Models;
using Newtonsoft.Json; // dependencia para o JsonConvert
using RestSharp;

// 2 - NameSpace
namespace Pet;

// 3 - Classe
public class PetTest
{
    // 3.1 - Atributos
    // Endereço da API
    private const string BASE_URL = "https://petstore.swagger.io/v2/";

    //public String token; // sseria uma forma de fazer

    // 3.2 - Funções e Métodos
    [Test, Order(1)]
    public void PostPetTest()
    {
        // Configura
        // instancia o objeto do tipo RestClient com o endereço da API
        var client = new RestClient(BASE_URL);

        // instancia o objeto do tipo RestRequest com o complemento de endereço
        // como "pet" e configurando o método para ser um post (inclusão)
        var request = new RestRequest("pet", Method.Post);
        
        // armazena o conteúdo do arquivo pet1.json na memória
        String jsonBody = File.ReadAllText(@"C:\iterasys\PetStore139\fixtures\pet1.json");

        // adiciona na requisição o conteúdo do arquivo pet1.json
        request.AddBody(jsonBody);

        // Executa
        // executa a requisição conforme a configuração realizada
        // guarda o json retornado no objeto response
        var response = client.Execute(request);

        // Valida
        var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);

        // Exibe o responseBody no console
        Console.WriteLine(responseBody);

        // Valide que na resposta, o status code é igual ao resultado esperado (200)
        Assert.That((int)response.StatusCode, Is.EqualTo(200));

        // Valida o petId
        int petId = responseBody.id;
        Assert.That(petId, Is.EqualTo(2563920));
        
        // Valida o nome do animal na resposta
        String name = responseBody.name.ToString();
        Assert.That(name, Is.EqualTo("Mag"));
        // OU
        // Assert.That(responseBody.name.ToString(), Is.EqualTo("Athena"));

        // Valida o status do animal na resposta
        String status = responseBody.status; 
        Assert.That(status, Is.EqualTo("available"));

        // Armazenar os dados obtidos para usar nos próximos testes
        Environment.SetEnvironmentVariable("petId", petId.ToString());
    }

    [Test, Order(2)]
    public void GetPetTest()
    {
        // Configura
        int petId = 2563920;            // campo de pesquisa
        String petName = "Mag";      // resultado esperado
        String categoryName = "dog";
        String tagsName = "vacinado";

        var client = new RestClient(BASE_URL);
        var request = new RestRequest($"pet/{petId}", Method.Get);

        // Executa
        var response = client.Execute(request);

        // Valida
        var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);
        Console.WriteLine(responseBody);

        Assert.That((int)response.StatusCode, Is.EqualTo(200));
        Assert.That((int)responseBody.id, Is.EqualTo(petId));
        Assert.That((String)responseBody.name, Is.EqualTo(petName));
        Assert.That((String)responseBody.category.name, Is.EqualTo(categoryName));
        Assert.That((String)responseBody.tags[0].name, Is.EqualTo(tagsName));
    }

    [Test, Order(3)]
    public void PutPetTest()
    {
        // Configura
        // Os dados de entrada vão formar o body da alteração
        // Vamos usar uma classe de modelo
        PetModel petModel = new PetModel();
        petModel.id = 2563920;
        petModel.category = new Category(1, "dog");
        petModel.name = "Mag";
        petModel.photoUrls = new String[]{""};
        petModel.tags = new Tag[]{new Tag(1, "vacinado"), 
                                  new Tag(2, "castrado")};
        petModel.status = "pending";

        // Transformar o modelo acima em um arquivo json
        String jsonBody = JsonConvert.SerializeObject(petModel, Formatting.Indented);
        Console.WriteLine(jsonBody);

        var client = new RestClient(BASE_URL);
        var request = new RestRequest("pet", Method.Put);
        request.AddBody(jsonBody);

        // Executa
        var response = client.Execute(request);

        // Valida
        var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);
        Console.WriteLine(responseBody);

        Assert.That((int)response.StatusCode, Is.EqualTo(200));
    }
  [Test, Order(4)]
    public void DeletePetTest()
    {
        // Configura
        String petId = Environment.GetEnvironmentVariable("petId");

        var client = new RestClient(BASE_URL);
        var request = new RestRequest($"pet/{petId}", Method.Delete);

        // Executa
        var response = client.Execute(request);

        // Valida
        var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);

        Assert.That((int)response.StatusCode, Is.EqualTo(200));
        Assert.That((int)responseBody.code, Is.EqualTo(200));
        Assert.That((String)responseBody.message, Is.EqualTo(petId));
    }    

    [Test, Order(6)]    
    public void GetUserLoginTest()
    {
        // Configura
        String username = "Mari";
        String password = "teste";

        var client = new RestClient(BASE_URL);
        var request = new RestRequest($"user/login?username={username}&password={password}", Method.Get);

        // Executa
        var response = client.Execute(request);
        
        // Valida
        var responseBody = JsonConvert.DeserializeObject<dynamic>(response.Content);

        Assert.That((int)response.StatusCode, Is.EqualTo(200));
        Assert.That((int)responseBody.code, Is.EqualTo(200));
        String message = responseBody.message;
        String token = message.Substring(message.LastIndexOf(":")+1);
        Console.WriteLine($"token = {token}");
        Environment.SetEnvironmentVariable("token", token);

    }

}