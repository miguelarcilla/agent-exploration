using Microsoft.Extensions.Configuration;

// Set up configuration to read from appsettings.json
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var endpoint = configuration["AZURE_OPENAI_ENDPOINT"] ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT not found in configuration");
var deploymentName = configuration["AZURE_OPENAI_DEPLOYMENT_NAME"] ?? throw new InvalidOperationException("AZURE_OPENAI_DEPLOYMENT_NAME not found in configuration");

//await ToolCalling.RunTranslationWorkflow(endpoint, deploymentName);
await WorkflowExample.RunTranslationWorkflow(endpoint, deploymentName);