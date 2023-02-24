// See https://aka.ms/new-console-template for more information

using Npgsql;
using System;
using System.Globalization;
using interfaces;
using solutions;
using Microsoft.Extensions.Configuration;
using Npgsql.Replication.PgOutput.Messages;

//var connectionString = "Host=192.168.88.180:7777;Username=postgres;Password=admin;Database=hackaton";


//deklaracja zmiennej połączeniowej
// using (var connection = new NpgsqlConnection(connectionString))
// {
//     await connection.OpenAsync();
//
// }



IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true);
IConfigurationRoot root = builder.Build();

var connString = root.GetConnectionString("Main");
using (var connection = new NpgsqlConnection(connString))
{
    connection.Open();
    INewsRepository repo = new NewsRepository(connection);
    // INewsRepository repo = new NewsMemoryRepository();





    int c1 = 1;



    while (c1 != 0)
    {
        Console.WriteLine(
            "0 - wyjscie 1 - dodawanie newsa, 2 - przegląd newsow, 3 - usuwanie newsa 4 - edycja tytulu 5 - edycja zawartosci 6 - SelectNewsById");
        c1 = Convert.ToInt32(Console.ReadLine()); //wybor dzialania

        if (c1 == 1)
        {

            //1
            // for (int i = 0; i < 100; i++)
            // {
            //     var newsToInsert = new News
            //     {
            //         Content = $"Moj news {i}",
            //         EventId = 1,
            //         UserId = 1,
            //         Title = i.ToString()
            //     };
            //     newsToInsert.Content = "dddup";
            //     await repo.InsertAsync(newsToInsert);

            var NewsToEdit = new News
            {
                Content = "content model",
                Title = "title model",
                EventId = 1,
                UserId = 1
            };
            await repo.InsertAsync(NewsToEdit);


        }
        else if (c1 == 2)
        {
// odczytywanie z bd
            Console.WriteLine("podaj id newsa: ");
            var NewsId = Console.ReadLine();
            int IdParsed;
            if (Int32.TryParse(NewsId, out IdParsed))
            {
                
            }


            List<News> ReturnList = await repo.GetNewsAsync();
            foreach (var i in ReturnList)
            {

              Console.WriteLine($"id: {i.Id}, tytuł: {i.Title}, zawart.:{i.Content}");

            }
               
            
        }

        else if (c1 == 3)
        {
            var Id = 1;

            //usuwanie z bazy danych
                
            
                await repo.DeleteNewsAsync(Id);
            
            


        }
        else if (c1 == 4)
        {
            var upObj = new News
            {
                Content = "czy zadziala",
                Title = "edycja obiektem",
                Id = 1
            };
           await repo.UpdateAsync(upObj);




        }

        

     }
}