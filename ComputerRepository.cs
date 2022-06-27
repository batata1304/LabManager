
using LabManager.Database;
using LabManager.Models;
using Microsoft.Data.Sqlite;
using Dapper;


namespace LabManager.Repositories;


class ComputerRepository
{

    private readonly DatabaseConfig _databaseConfig;

    public ComputerRepository(DatabaseConfig databaseConfig)
    {
        _databaseConfig = databaseConfig;
    }

     public List<Computer> GetAll()
    {
     
     var connection = new SqliteConnection(_databaseConfig.ConnectionString);

      connection.Open();

     var computers = connection.Query<Computer>("SELECT * FROM Computers").ToList();

     connection.Close();

     return computers;
}


    public Computer Save(Computer computer)
    {   
        

        var connection = new SqliteConnection(_databaseConfig.ConnectionString);
        connection.Open();



         connection.Execute("INSERT INTO Computers VALUES(@Id, @Ram, @Processor)",computer);
         
         connection.Close();

         return computer;

    }

    public Computer GetById(int id)
    {   
        var connection = new SqliteConnection(_databaseConfig.ConnectionString);
        connection.Open();
        var command = connection.CreateCommand();

        command.CommandText = "SELECT * FROM Computers WHERE ID = $id;";

        command.Parameters.AddWithValue("$id", id);

        var reader = command.ExecuteReader();
        reader.Read();

        var computer = ReaderToComputer(reader);

        connection.Close();

        return computer;

    }

    public Computer Update(Computer computer)
    {   
        var connection = new SqliteConnection(_databaseConfig.ConnectionString);
        connection.Open();
        var command = connection.CreateCommand();

        command.CommandText = @"
            UPDATE Computers 
            SET ram = $ram, processor = $processor 
            WHERE ID = $id;
        ";

        command.Parameters.AddWithValue("$id", computer.Id);
        command.Parameters.AddWithValue("$ram", computer.Ram);
        command.Parameters.AddWithValue("$processor", computer.Processor);

        command.ExecuteNonQuery();

        connection.Close();

        return computer;
    }

    public void Delete(int id)
    {
        var connection = new SqliteConnection(_databaseConfig.ConnectionString);
        connection.Open();
        var command = connection.CreateCommand();

        connection.Execute("DELETE FROM Computers WHERE id = @Id", new { Id = id });

        connection.Close();
    }

    private Computer ReaderToComputer(SqliteDataReader reader)
    {
        var computer = new Computer(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
        return computer;
    }

    public bool ExistsById(int id)
    {   
        var connection = new SqliteConnection(_databaseConfig.ConnectionString);
        connection.Open();
        
        var command = connection.CreateCommand();
        command.CommandText = "SELECT count(ID) FROM Computers WHERE ID = $id;";
        command.Parameters.AddWithValue("$id", id);

        var reader = command.ExecuteReader();
        reader.Read();

        var result = reader.GetBoolean(0);
        connection.Close();

        return result;
    }

}