using DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class Car
    {
        private readonly IEngine _engine;
        private readonly EcuDbContext _db;
        public readonly DateTime ManufactureDate = new DateTime(2018, 1, 1);

        public Car(IEngine engine, EcuDbContext db)
        {
            _engine = engine;
            _db = db;
        }

        public bool Start()
        {
            try
            {
                if (_engine.Start())
                {
                    _engine.IsRunning = true;
                }
                else
                {
                    throw new CarStartException("Something went wrong");
                }
            }
            catch (EngineStartException ese)
            {
                // Some logging code
                throw new CarStartException("Handled EngineStartException", ese);
            }

            return _engine.IsRunning;
        }

        public async Task<IEnumerable<EcuData>> GetSensorData(DateTime sinceTime)
        {
            if (sinceTime < ManufactureDate)
            {
                throw new EcuDataException($"The sinceTime '{sinceTime.ToString("yyyy-mm-dd HH:mi")}' provided is earlier than the ManufactureDate '{ManufactureDate.ToString("yyyy-mm-dd HH:mi")}' for the car.");
            }

            return await _db.EcuData.Where(e => e.Timestamp >= sinceTime).OrderBy(e => e.Timestamp).ToListAsync();
        }

        public int CalculateCurrentRange(int gallons, int avgMpg)
        {
            return gallons * avgMpg;
        }

        public bool IsRunning => _engine.IsRunning;
    }
}