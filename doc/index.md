# Dapplo.Confluence
This is a simple REST based Confluence client, written for Greenshot, by using Dapplo.HttpExtension

- Current build status: [![Build status](https://ci.appveyor.com/api/projects/status/3vp7h9n40n4v680n?svg=true)](https://ci.appveyor.com/project/dapplo/dapplo-confluence)
- Coverage Status: [![Coverage Status](https://coveralls.io/repos/github/dapplo/Dapplo.Confluence/badge.svg?branch=master)](https://coveralls.io/github/dapplo/Dapplo.Confluence?branch=master)
- NuGet package: [![NuGet package](https://badge.fury.io/nu/Dapplo.Confluence.svg)](https://badge.fury.io/nu/Dapplo.Confluence)

The Confluence client supports most REST methods, and has a fluent API for building a CQL (Confluence Query Language) string to search with.

An example on how to use this Confluence client:
```
var confluenceClient = ConfluenceClient.Create(new Uri("https://confluence"));
confluenceClient.SetBasicAuthentication(username, password);
var query = Where.And(Where.Type.IsPage, Where.Text.Contains("Test Home"));
var searchResult = await confluenceClient.Content.SearchAsync(query, limit:1);
foreach (var content in searchResult.Results)
{
	Debug.WriteLine(content.Body);
}
```

If you want to extend the API, for example to add logic for a *plugin*, you can write an extension method to extend the IConfluenceClientPlugins.
Your "plugin" extension will now be available, if the developer has a using statement of your namespace, on the .Plugins property of the IConfluenceClient
