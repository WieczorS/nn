using solutions;


namespace interfaces;

public interface INewsRepository
{
    Task<int> DeleteNewsAsync(int NewsToDelete);
    Task UpdateAsync(News updateNews);
    Task<List<News>> GetNewsAsync();
    Task<News> GetNewsById(int NewsId);
    Task InsertAsync(News NewsCreate);
}