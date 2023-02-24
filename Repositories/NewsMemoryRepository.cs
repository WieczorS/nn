using interfaces;

namespace solutions;

public class NewsMemoryRepository: INewsRepository
{

    private List<News> _memory = new List<News>();
    
    
    public async Task InsertAsync(News newsCreate)
    {
        newsCreate.Id = _memory.Count + 1;
        newsCreate.CreateDate = DateTime.Now;
        _memory.Add(newsCreate);
    }
    public async Task<int> DeleteNewsAsync(int newsToDelete)
    {
        if (_memory != null)
        {
            _memory.RemoveAt(newsToDelete-1);
            return 1;
        }
        else
        {
            return 0;
        }


    }

    public async Task UpdateAsync(News updateNews)
    {
       

        var obj = _memory.FirstOrDefault(x => x.Id == updateNews.Id);
        if(updateNews.Content != null)  obj.Content = updateNews.Content;
        if(updateNews.Title != null)  obj.Title = updateNews.Title;
        
    }
//dd
    public async Task<List<News>> GetNewsAsync()
    {
        var list = new List<News>();
        foreach (var i in _memory)
        {
           
            list.Add(i);
        }

        return list;
    }

    public Task<News> GetNewsById(int NewsId)
    {
        


}