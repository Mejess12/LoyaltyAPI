using Dapper;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using LoyaltyAPI.Models;
using Microsoft.Extensions.Configuration;
using System;

namespace LoyaltyAPI.Services;

public class ActivityService
{
    private readonly string _connectionString;

    public ActivityService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("LoyaltyDbConnection")
            ?? throw new InvalidOperationException("Connection string 'LoyaltyDbConnection' not found.");
    }

    public async Task<List<Activity>> GetActivitiesAsync()
    {
        using var connection = new MySqlConnection(_connectionString);
        var activities = await connection.QueryAsync<Activity>("SELECT * FROM activity");
        return activities.AsList();
    }

    public async Task<Activity?> GetActivityByIdAsync(int activityId)
    {
        using var connection = new MySqlConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<Activity>(
            "SELECT * FROM activity WHERE activity_id = @ActivityId",
            new { ActivityId = activityId }
        );
    }

    public async Task<bool> InsertActivityAsync(Activity activity)
    {
        using var connection = new MySqlConnection(_connectionString);
        var query = @"
            INSERT INTO activity (description, points, date_from, date_to, date_time, system_source_id, created_at, transaction_type)
            VALUES (@description, @points, @date_from, @date_to, @date_time, @system_source_id, @created_at, @transaction_type)";

        var result = await connection.ExecuteAsync(query, new
        {
            description = activity.description,
            points = activity.points,
            date_time = activity.date_time,
            system_source_id = activity.system_source_id,
            created_at = activity.created_at,
            transaction_type = activity.transaction_type
        });

        return result > 0;
    }
}