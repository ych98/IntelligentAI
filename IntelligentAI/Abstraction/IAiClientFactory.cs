namespace IntelligentAI.Abstraction;

public interface IAiClientFactory
{
    AiClientBase CreateClient(string serviceName, string modelName);

    AiClientBase CreateClient(int modelEnum);
}