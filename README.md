<h1 align="center" id="title">FiveAnts.Endpoint</h1>

<p align="center"><img src="https://socialify.git.ci/bslie/FiveAnts.Endpoint/image?description=1&amp;descriptionEditable=C%23%20Endpoint%3A%20HTTP%20client%20wrapper%20for%20RESTful%20API%20calls%20with%20authentication%20and%20request%20manipulation.&amp;font=Jost&amp;language=1&amp;name=1&amp;owner=1&amp;theme=Auto" alt="project-image"></p>

<p id="description">The Endpoint class in C# is a versatile HTTP client wrapper for making RESTful API calls. It supports various HTTP methods like GET POST PUT DELETE and PATCH and allows for header cookie and query parameter manipulation. It also provides basic and bearer token authentication.</p>

  
  
<h2>🧐 Features</h2>

Here're some of the project's best features:

<h2>🛠️ Installation Steps:</h2>

<p>Installation via command line using dotnet CLI.</p>

```
dotnet add package FiveAnts.Endpoint
```

<h2>Example</h2>

```C#

public class Todo
{
    [JsonProperty("userId")]
    public int UserId;

    [JsonProperty("id")]
    public int Id;

    [JsonProperty("title")]
    public string? Title;

    [JsonProperty("completed")]
    public bool Completed;
}

public static async Task<Todo> GetTodo(int id)
{
    var endpoint = new Endpoint<Todo>("https://jsonplaceholder.typicode.com");

    //https://jsonplaceholder.typicode.com/todos/1
    return await endpoint
        .AppendPathSegments("todos", $"{id}")
        .GetAsync();
}

```


```C#

  public class Comments
  {
      [JsonProperty("postId")]
      public int PostId;

      [JsonProperty("id")]
      public int Id;

      [JsonProperty("name")]
      public string? Name;

      [JsonProperty("email")]
      public string? Email;

      [JsonProperty("body")]
      public string? Body;
  }

  public static async Task<List<Comments>> GetComment(int id)
  {
      var endpoint = new Endpoint<List<Comments>>("https://jsonplaceholder.typicode.com");

      //https://jsonplaceholder.typicode.com/comments?postId=1
      return await endpoint
          .AppendPathSegment("comments")
          .SetQueryParam("postId", id)
          .GetAsync();
  }

```
  
  
<h2>💻 Built with</h2>

Technologies used in the project:

*   .NET 8.0
*   Newtonsot.Json

<h2>🛡️ License:</h2>

This project is licensed under the MIT License
