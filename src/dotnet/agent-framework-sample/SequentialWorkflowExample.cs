using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

public static class SequentialWorkflowExample
{
    public static async Task RunTranslationWorkflow(string endpoint, string deploymentName)
    {
        var agent = new AzureOpenAIClient(
            new Uri(endpoint),
            new AzureCliCredential())
            .GetChatClient(deploymentName)
            .AsIChatClient();

        static ChatClientAgent GetTranslationAgent(string targetLanguage, IChatClient chatClient) =>
            new(chatClient,
            $"You are a translation assistant who only responds in {targetLanguage}. Respond to any " +
            $"input by outputting the name of the input language and then translating the input to {targetLanguage}.");

        // Create translation agents for sequential processing
        var translationAgents = (
            from lang in (string[])["French", "Spanish", "English"]
            select GetTranslationAgent(lang, agent));

        // 3) Build sequential workflow
        var workflow = AgentWorkflowBuilder.BuildSequential(translationAgents);

        // 4) Run the workflow
        var messages = new List<ChatMessage> { new(ChatRole.User, "Hello, world!") };

        StreamingRun run = await InProcessExecution.StreamAsync(workflow, messages);
        await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

        List<ChatMessage> result = new();
        await foreach (WorkflowEvent evt in run.WatchStreamAsync().ConfigureAwait(false))
        {
            if (evt is AgentRunUpdateEvent e)
            {
                //Console.WriteLine($"{e.ExecutorId}: {e.Data}");
            }
            else if (evt is WorkflowOutputEvent completed)
            {
                result = (List<ChatMessage>)completed.Data!;
                break;
            }
        }

        // Display final result
        foreach (var message in result)
        {
            Console.WriteLine($"{message.Role}: {message.Text}");
        }
    }
}
