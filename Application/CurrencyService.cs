using Models;
using Microsoft.Data.SqlClient;

namespace Application;

public class CurrencyService : ICurrencyService
{
    private string _connectionString;

    public CurrencyService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<bool> AddCurrency(CurrencyRequest request)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        
        using var transaction = connection.BeginTransaction();

        try
        {
            var countryIds = new List<int>();
            foreach (var name in request.Countries)
            {
                string checkCoutryCommand = "SELECT Id FROM Country WHERE Name = @Name";
                var checkCountry = new SqlCommand(checkCoutryCommand, connection, transaction);
                checkCountry.Parameters.AddWithValue("@Name", name);
                var countryId = await checkCountry.ExecuteScalarAsync();
                if (countryId == null)
                {
                    transaction.Rollback();
                    throw new Exception($"Country '{name}' not found");
                }

                countryIds.Add((int)countryId);
            }

            int currencyId;
            string checkCurrencyCommand = "SELECT Id FROM Currency WHERE Name = @Name";
            var checkCurrency = new SqlCommand(checkCurrencyCommand, connection, transaction);
            checkCurrency.Parameters.AddWithValue("@Name", request.Name);
            var currency = await checkCurrency.ExecuteScalarAsync();

            if (currency != null)
            {
                currencyId = (int)currency;
                string updateCurrencyCommand = "UPDATE Currency SET Rate = @Rate WHERE Id = @Id";
                var updateCurrency = new SqlCommand(updateCurrencyCommand, connection, transaction);
                updateCurrency.Parameters.AddWithValue("@Rate", request.Rate);
                updateCurrency.Parameters.AddWithValue("@Id", currencyId);
                await updateCurrency.ExecuteNonQueryAsync();

                string deleteOldLinkCommand = "DELETE FROM Currency_Country WHERE Currency_Id = @Id";
                var deleteOldLink = new SqlCommand(deleteOldLinkCommand, connection, transaction);
                deleteOldLink.Parameters.AddWithValue("@Id", currencyId);
                await deleteOldLink.ExecuteNonQueryAsync();
            }
            else
            {
                string insertCurrencyCommand =
                    "INSERT INTO Currency (Name, Rate) OUTPUT INSERTED.Id VALUES (@Name, @Rate)";
                var insertCurrency = new SqlCommand(insertCurrencyCommand, connection, transaction);
                insertCurrency.Parameters.AddWithValue("@Name", request.Name);
                insertCurrency.Parameters.AddWithValue("@Rate", request.Rate);
                currencyId = (int)await insertCurrency.ExecuteScalarAsync();
            }

            foreach (var countryId in countryIds)
            {
                string insertLinkCommand =
                    "INSERT INTO Currency_Country (Currency_Id, Country_Id) VALUES (@Currency_Id, @Country_Id)";
                var insertLink = new SqlCommand(insertLinkCommand, connection, transaction);
                insertLink.Parameters.AddWithValue("@Currency_Id", currencyId);
                insertLink.Parameters.AddWithValue("@Country_Id", countryId);
                await insertLink.ExecuteNonQueryAsync();
            }

            transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw ex;
        }
    }

    public async Task<object?> SearchByType(SearchRequest request)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        if (request.Type.ToLower() == "country")
        {
            string getCountryIdCommand = "SELECT Id FROM Country WHERE Name = @Name";
            var getCountryId = new SqlCommand(getCountryIdCommand, connection);
            getCountryId.Parameters.AddWithValue("@Name", request.Name);
            var countryId = (int?)await getCountryId.ExecuteScalarAsync();
            if (countryId == null) return null;
            
            string getCurrenciesCommand = @"SELECT c.Name, c.Rate
                                          FROM Currency c
                                          JOIN Currency_Country cc ON c.Id = cc.Currency_Id
                                          WHERE cc.Country_Id = @CountryId";
            
            var getCurrencies = new SqlCommand(getCurrenciesCommand, connection);
            getCurrencies.Parameters.AddWithValue("@CountryId", countryId);
            
            var reader = await getCurrencies.ExecuteReaderAsync();
            var currencies = new List<object>();
            while (await reader.ReadAsync())
            {
                currencies.Add(new
                {
                    name = reader.GetString(0),
                    rate = reader.GetFloat(1),
                });
            }
            reader.Close();

            return new
            {
                country = request.Name,
                currencies
            };
        }
        else if (request.Type.ToLower() == "currency")
        {
            string getCurrencyIdCommand = "SELECT Id FROM Currency WHERE Name = @Name";
            var getCurrencyId = new SqlCommand(getCurrencyIdCommand, connection);
            getCurrencyId.Parameters.AddWithValue("@Name", request.Name);
            var currencyId = (int?)await getCurrencyId.ExecuteScalarAsync();
            
            if (currencyId == null) return null;
            
            string getCountriesCommand = @"SELECT c.Name
                                           FROM Country c 
                                           JOIN Currency_Country cc ON c.Id = cc.Country_Id
                                           WHERE cc.Currency_Id = @CurrencyId";
            var getCountries = new SqlCommand(getCountriesCommand, connection);
            getCountries.Parameters.AddWithValue("@CurrencyId", currencyId);
            var reader = await getCountries.ExecuteReaderAsync();
            var countries = new List<string>();
            while (await reader.ReadAsync())
            {
                countries.Add(reader.GetString(0));
            }
            reader.Close();
            return new
            {
                currency = request.Name,
                countries
            };
        }
        return null;
    }
}