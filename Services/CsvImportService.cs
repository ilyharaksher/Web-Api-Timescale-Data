using System.Globalization;
using Web_Api_Timescale_Data.Entities;
using Web_Api_Timescale_Data.Exceptions;
using Web_Api_Timescale_Data.Data;
using Microsoft.EntityFrameworkCore;


namespace Web_Api_Timescale_Data.Services
{
    public class CsvImportService : ICsvImportService
    {
        private readonly ApplicationDbContext _dbContext;
        public CsvImportService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task ImportAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);

            string fileName = file.FileName;

            var header = await reader.ReadLineAsync();

            if (header != "Date;ExecutionTime;Value")
            {
                throw new CsvValidationException("Неверный формат заголовков. Необходимый формат: \"Date;ExecutionTime;Value\"");
            }

            var line = await reader.ReadLineAsync();
            int lineNumber = 1;

            var valuesList = new List<ValueEntity>();

            while (line != null)
            {
                if (lineNumber > 10000)
                {
                    throw new CsvValidationException("Количество записей в файле не может быть больше 10000");
                }

                string[] parts = line.Split(";");

                if (parts.Length != 3)
                {
                    throw new CsvValidationException($"Ошибка в Записи {lineNumber}: Запись должна содержать Date, ExecutionTime, Value");
                }

                if (!DateTime.TryParseExact(
                    parts[0],
                    "yyyy-MM-dd'T'HH-mm-ss.ffff'Z'",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out DateTime date
                    ))
                {
                    throw new CsvValidationException($"Ошибка в Записи {lineNumber}: Неверный формат даты. Нужно: yyyy-MM-dd'T'HH-mm-ss.ffff'Z' ");
                }
                
                if (date > DateTime.Now || date < new DateTime(2000, 1, 1))
                {
                    throw new CsvValidationException($"Ошибка в Записи {lineNumber}: Дата не может быть позже текущей и раньше 01.01.2000");
                }

                string part1 = parts[1].Replace(",", ".");

                if (!double.TryParse(part1, CultureInfo.InvariantCulture, out double executionTime))
                {
                    throw new CsvValidationException($"Ошибка в Записи {lineNumber}: Неверный формат ExecutionTime");
                }

                if (executionTime < 0)
                {
                    throw new CsvValidationException($"Ошибка в Записи {lineNumber}: Время выполнения не может быть меньше 0");
                }

                string part2 = parts[2].Replace(",", ".");

                if (!double.TryParse(part2, CultureInfo.InvariantCulture, out double value))
                {
                    throw new CsvValidationException($"Ошибка в Записи {lineNumber}: Неверный формат Value");
                }

                if (value < 0)
                {
                    throw new CsvValidationException($"Ошибка в Записи {lineNumber}: Value не может быть меньше 0");
                }

                ValueEntity measure = new ValueEntity
                {
                    FileName = fileName,
                    Date = date,
                    ExecutionTime = executionTime,
                    Value = value
                };

                valuesList.Add(measure);
                lineNumber++;
                line = await reader.ReadLineAsync();
            }

            if (valuesList.Count < 1)
            {
                throw new CsvValidationException("Количество записей в файле не может быть меньше 1");
            }


            DateTime minDate = valuesList.Min(x => x.Date);
            DateTime maxDate = valuesList.Max(x => x.Date);
            Double delta = (maxDate - minDate).TotalSeconds;
            Double avgExecTime = valuesList.Average(x => x.ExecutionTime);
            Double avgValue = valuesList.Average(x => x.Value);
            Double maxValue = valuesList.Max(x => x.Value);
            Double minValue = valuesList.Min(x => x.Value);

            var sorted = valuesList.OrderBy(x => x.Value).ToList();
            int length = sorted.Count();
            Double medianValue = length % 2 == 0
                ? (sorted[length / 2].Value + sorted[length / 2 - 1].Value) / 2
                : sorted[length / 2].Value;


            ResultEntity result = new ResultEntity
            {
                FileName = fileName,
                Delta = delta,
                MinDate = minDate,
                AvgExecutionTime = avgExecTime,
                AvgValue = avgValue,
                MedianValue = medianValue,
                MaxValue = maxValue,
                MinValue = minValue
            };


            var oldValues = await _dbContext.Values
                .Where(x => x.FileName == fileName)
                .ToListAsync();
            var oldResult = await _dbContext.Results
                .FirstOrDefaultAsync(x => x.FileName == fileName);

            _dbContext.RemoveRange(oldValues);

            if (oldResult != null)
            {
                _dbContext.Remove(oldResult);
            }

            _dbContext.Values.AddRange(valuesList);
            _dbContext.Results.Add(result);


            await _dbContext.SaveChangesAsync();
        }
    }
}