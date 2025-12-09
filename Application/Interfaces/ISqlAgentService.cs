namespace Application.Interfaces;

public interface ISqlAgentService
{
    Task<string> ProcessUserQueryAsync(string userMessage);
}