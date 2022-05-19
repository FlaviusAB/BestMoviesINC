
using Newtonsoft.Json;

namespace Client.Models
{
	public class Genre
	{
		
            [JsonProperty("id")]
			public int? Id;

			[JsonProperty("name")]
			public string? Name;
		
	}
}

