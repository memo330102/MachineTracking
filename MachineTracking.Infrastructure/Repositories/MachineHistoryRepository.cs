using MachineTracking.Domain.DTOs.MachineHistory;
using MachineTracking.Domain.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineTracking.Infrastructure.Repositories
{
    public class MachineHistoryRepository : IMachineHistoryRepository
    {
        private readonly ISqlQuery _sqlQuery;
        public MachineHistoryRepository(ISqlQuery sqlQuery)
        {
            _sqlQuery = sqlQuery;
        }
        public async Task AddAsync(MachineHistoryDTO entity)
        {
            const string query = @"
                 INSERT INTO machinehistory (MachineId, Status,StatusId,Topic, ChainMovesPerSecond, ArticleNumber)
                 VALUES (@MachineId, @Status,@StatusId,@Topic, @ChainMovesPerSecond, @ArticleNumber)
                 RETURNING Id";

            var response = await _sqlQuery.ExecuteAsync(query, new
            {
                MachineId = entity.MachineId,
                Status = entity.Status,
                StatusId = entity.StatusId,
                Topic = entity.Topic,
                ChainMovesPerSecond = entity.ChainMovesPerSecond,
                ArticleNumber = entity.ArticleNumber,
            });

        }

        public async Task<IEnumerable<MachineHistoryDTO>> GetAllAsync()
        {
            const string query = "SELECT * FROM MachineHistory";
            return await _sqlQuery.QueryAsync<MachineHistoryDTO>(query);
        }

        public async Task<IEnumerable<MachineHistoryDTO>> GetLastestDataOfAllMachinesAsync()
        {
            const string query = @"WITH TblLatestMachineData AS (
                                     SELECT MachineId,MAX(DataReceivedTimestamp) AS LatestTimestamp
                                     FROM MachineHistory
                                     GROUP BY MachineId)
                                   SELECT mh.*
                                   FROM MachineHistory mh
                                   INNER JOIN TblLatestMachineData lmd
                                   ON mh.MachineId = lmd.MachineId AND mh.DataReceivedTimestamp = lmd.LatestTimestamp;";
         
            return await _sqlQuery.QueryAsync<MachineHistoryDTO>(query);
        }

        public async Task<MachineHistoryDTO?> GetByIdAsync(int id)
        {
            const string query = "SELECT * FROM MachineHistory WHERE Id = @Id";
            return await _sqlQuery.QueryAsyncFirstOrDefault<MachineHistoryDTO>(query, new { Id = id });
        }

        public async Task<IEnumerable<MachineHistoryDTO>> GetMachineHistoriesAsync(string machineId)
        {
            const string query = "SELECT * FROM MachineHistory WHERE machineid = @MachineId ORDER BY datareceivedtimestamp DESC ";
            return await _sqlQuery.QueryAsync<MachineHistoryDTO>(query, new { MachineId = machineId });
        }
    }

}
